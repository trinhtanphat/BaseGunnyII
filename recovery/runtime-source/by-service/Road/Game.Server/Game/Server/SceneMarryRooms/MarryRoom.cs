using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.SceneMarryRooms;

public class MarryRoom
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static object _syncStop = new object();

	private List<GamePlayer> _guestsList;

	private IMarryProcessor _processor;

	private int _count;

	public MarryRoomInfo Info;

	private eRoomState _roomState;

	private Timer _timer;

	private Timer _timerForHymeneal;

	private List<int> _userForbid;

	private List<int> _userRemoveList;

	public eRoomState RoomState
	{
		get
		{
			return _roomState;
		}
		set
		{
			if (_roomState != value)
			{
				_roomState = value;
				SendMarryRoomInfoUpdateToScenePlayers(this);
			}
		}
	}

	public int Count => _count;

	public MarryRoom(MarryRoomInfo info, IMarryProcessor processor)
	{
		Info = info;
		_processor = processor;
		_guestsList = new List<GamePlayer>();
		_count = 0;
		_roomState = eRoomState.FREE;
		_userForbid = new List<int>();
		_userRemoveList = new List<int>();
	}

	public bool AddPlayer(GamePlayer player)
	{
		lock (_syncStop)
		{
			if (player.CurrentRoom != null || player.IsInMarryRoom)
			{
				return false;
			}
			if (_guestsList.Count > Info.MaxCount)
			{
				player.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("MarryRoom.Msg1"));
				return false;
			}
			_count++;
			_guestsList.Add(player);
			player.CurrentMarryRoom = this;
			player.MarryMap = 1;
			if (player.CurrentRoom != null)
			{
				player.CurrentRoom.RemovePlayerUnsafe(player);
			}
		}
		return true;
	}

	public void RemovePlayer(GamePlayer player)
	{
		lock (_syncStop)
		{
			if (RoomState == eRoomState.FREE)
			{
				_count--;
				_guestsList.Remove(player);
				GSPacketIn packet = player.Out.SendPlayerLeaveMarryRoom(player);
				player.CurrentMarryRoom.SendToPlayerExceptSelfForScene(packet, player);
				player.CurrentMarryRoom = null;
				player.MarryMap = 0;
			}
			else if (RoomState == eRoomState.Hymeneal)
			{
				_userRemoveList.Add(player.PlayerCharacter.ID);
				_count--;
				_guestsList.Remove(player);
				player.CurrentMarryRoom = null;
			}
			SendMarryRoomInfoUpdateToScenePlayers(this);
		}
	}

	public void BeginTimer(int interval)
	{
		if (_timer == null)
		{
			_timer = new Timer(OnTick, null, interval, interval);
		}
		else
		{
			_timer.Change(interval, interval);
		}
	}

	protected void OnTick(object obj)
	{
		_processor.OnTick(this);
	}

	public void StopTimer()
	{
		if (_timer != null)
		{
			_timer.Dispose();
			_timer = null;
		}
	}

	public void BeginTimerForHymeneal(int interval)
	{
		if (_timerForHymeneal == null)
		{
			_timerForHymeneal = new Timer(OnTickForHymeneal, null, interval, interval);
		}
		else
		{
			_timerForHymeneal.Change(interval, interval);
		}
	}

	protected void OnTickForHymeneal(object obj)
	{
		try
		{
			_roomState = eRoomState.FREE;
			GSPacketIn gSPacketIn = new GSPacketIn(249);
			gSPacketIn.WriteByte(9);
			SendToAll(gSPacketIn);
			StopTimerForHymeneal();
			SendUserRemoveLate();
			SendMarryRoomInfoUpdateToScenePlayers(this);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("OnTickForHymeneal", exception);
			}
		}
	}

	public void StopTimerForHymeneal()
	{
		if (_timerForHymeneal != null)
		{
			_timerForHymeneal.Dispose();
			_timerForHymeneal = null;
		}
	}

	public GamePlayer[] GetAllPlayers()
	{
		lock (_syncStop)
		{
			return _guestsList.ToArray();
		}
	}

	public void SendToRoomPlayer(GSPacketIn packet)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		if (allPlayers != null)
		{
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void SendToAll(GSPacketIn packet)
	{
		SendToAll(packet, null, isChat: false);
	}

	public void SendToAll(GSPacketIn packet, GamePlayer self, bool isChat)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		if (allPlayers == null)
		{
			return;
		}
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (!isChat || !gamePlayer.IsBlackFriend(self.PlayerCharacter.ID))
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void SendToAllForScene(GSPacketIn packet, int sceneID)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		if (allPlayers == null)
		{
			return;
		}
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.MarryMap == sceneID)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void SendToPlayerExceptSelf(GSPacketIn packet, GamePlayer self)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		if (allPlayers == null)
		{
			return;
		}
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != self)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void SendToPlayerExceptSelfForScene(GSPacketIn packet, GamePlayer self)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		if (allPlayers == null)
		{
			return;
		}
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != self && gamePlayer.MarryMap == self.MarryMap)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public void SendToScenePlayer(GSPacketIn packet)
	{
		WorldMgr.MarryScene.SendToALL(packet);
	}

	public void ProcessData(GamePlayer player, GSPacketIn data)
	{
		lock (_syncStop)
		{
			_processor.OnGameData(this, player, data);
		}
	}

	public void ReturnPacket(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = packet.Clone();
		gSPacketIn.ClientID = player.PlayerCharacter.ID;
		SendToPlayerExceptSelf(gSPacketIn, player);
	}

	public void ReturnPacketForScene(GamePlayer player, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = packet.Clone();
		gSPacketIn.ClientID = player.PlayerCharacter.ID;
		SendToPlayerExceptSelfForScene(gSPacketIn, player);
	}

	public bool KickPlayerByUserID(GamePlayer player, int userID)
	{
		GamePlayer playerByUserID = GetPlayerByUserID(userID);
		if (playerByUserID != null && playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.GroomID && playerByUserID.PlayerCharacter.ID != player.CurrentMarryRoom.Info.BrideID)
		{
			RemovePlayer(playerByUserID);
			playerByUserID.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));
			GSPacketIn packet = player.Out.SendMessage(eMessageType.ChatERROR, playerByUserID.PlayerCharacter.NickName + "  " + LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom2"));
			player.CurrentMarryRoom.SendToPlayerExceptSelf(packet, player);
			return true;
		}
		return false;
	}

	public void KickAllPlayer()
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			RemovePlayer(gamePlayer);
			gamePlayer.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryRoom.TimeOver"));
		}
	}

	public GamePlayer GetPlayerByUserID(int userID)
	{
		lock (_syncStop)
		{
			foreach (GamePlayer guests in _guestsList)
			{
				if (guests.PlayerCharacter.ID == userID)
				{
					return guests;
				}
			}
		}
		return null;
	}

	public void RoomContinuation(int time)
	{
		TimeSpan timeSpan = DateTime.Now - Info.BeginTime;
		int num = Info.AvailTime * 60 - timeSpan.Minutes + time * 60;
		Info.AvailTime += time;
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			playerBussiness.UpdateMarryRoomInfo(Info);
		}
		BeginTimer(60000 * num);
	}

	public void SetUserForbid(int userID)
	{
		lock (_syncStop)
		{
			_userForbid.Add(userID);
		}
	}

	public bool CheckUserForbid(int userID)
	{
		lock (_syncStop)
		{
			return _userForbid.Contains(userID);
		}
	}

	public void SendUserRemoveLate()
	{
		lock (_syncStop)
		{
			foreach (int userRemove in _userRemoveList)
			{
				GSPacketIn packet = new GSPacketIn(244, userRemove);
				SendToAllForScene(packet, 1);
			}
			_userRemoveList.Clear();
		}
	}

	public GSPacketIn SendMarryRoomInfoUpdateToScenePlayers(MarryRoom room)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(255);
		bool flag = room != null;
		gSPacketIn.WriteBoolean(flag);
		if (flag)
		{
			gSPacketIn.WriteInt(room.Info.ID);
			gSPacketIn.WriteBoolean(room.Info.IsHymeneal);
			gSPacketIn.WriteString(room.Info.Name);
			gSPacketIn.WriteBoolean(!(room.Info.Pwd == ""));
			gSPacketIn.WriteInt(room.Info.MapIndex);
			gSPacketIn.WriteInt(room.Info.AvailTime);
			gSPacketIn.WriteInt(room.Count);
			gSPacketIn.WriteInt(room.Info.PlayerID);
			gSPacketIn.WriteString(room.Info.PlayerName);
			gSPacketIn.WriteInt(room.Info.GroomID);
			gSPacketIn.WriteString(room.Info.GroomName);
			gSPacketIn.WriteInt(room.Info.BrideID);
			gSPacketIn.WriteString(room.Info.BrideName);
			gSPacketIn.WriteDateTime(room.Info.BeginTime);
			gSPacketIn.WriteByte((byte)room.RoomState);
			gSPacketIn.WriteString(room.Info.RoomIntroduction);
		}
		SendToScenePlayer(gSPacketIn);
		return gSPacketIn;
	}
}
