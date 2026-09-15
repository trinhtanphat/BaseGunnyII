using System;
using System.Collections.Generic;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Rooms;

public class BaseWorldBossRoom
{
	private Dictionary<int, GamePlayer> m_list;

	private long MAX_BLOOD;

	private long m_blood;

	private string _name;

	private string _bossResourceId;

	private DateTime _begin_time;

	private DateTime _end_time;

	private int _currentPVE;

	private bool _fightOver;

	private bool _roomClose;

	private bool _worldOpen;

	private int _fight_time;

	private bool m_die;

	public int playerDefaultPosX = 265;

	public int playerDefaultPosY = 1030;

	public int ticketID = 11573;

	public int need_ticket_count;

	public int timeCD = 15;

	public int reviveMoney = 10000;

	public int reFightMoney = 12000;

	public int addInjureBuffMoney = 30000;

	public int addInjureValue = 200;

	public long MaxBlood => MAX_BLOOD;

	public long Blood
	{
		get
		{
			return m_blood;
		}
		set
		{
			m_blood = value;
		}
	}

	public string name => _name;

	public string bossResourceId => _bossResourceId;

	public DateTime begin_time => _begin_time;

	public DateTime end_time => _end_time;

	public int currentPVE => _currentPVE;

	public bool fightOver => _fightOver;

	public bool roomClose => _roomClose;

	public bool worldOpen => _worldOpen;

	public int fight_time => _fight_time;

	public bool IsDie
	{
		get
		{
			return m_die;
		}
		set
		{
			m_die = value;
		}
	}

	public BaseWorldBossRoom()
	{
		m_list = new Dictionary<int, GamePlayer>();
		m_die = false;
		_worldOpen = false;
		_fightOver = true;
		_roomClose = true;
		_name = "boss";
		_bossResourceId = "0";
		_currentPVE = 0;
	}

	public void UpdateRank(int damage, int honor, string nickName)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(81);
		gSPacketIn.WriteInt(damage);
		gSPacketIn.WriteInt(honor);
		gSPacketIn.WriteString(nickName);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public void ShowRank()
	{
		GSPacketIn packet = new GSPacketIn(86);
		GameServer.Instance.LoginServer.SendPacket(packet);
	}

	public void UpdateWorldBoss(GSPacketIn pkg)
	{
		long mAX_BLOOD = pkg.ReadLong();
		long blood = pkg.ReadLong();
		string text = pkg.ReadString();
		string text2 = pkg.ReadString();
		int num = pkg.ReadInt();
		_fightOver = pkg.ReadBoolean();
		_roomClose = pkg.ReadBoolean();
		_begin_time = pkg.ReadDateTime();
		_end_time = pkg.ReadDateTime();
		_fight_time = pkg.ReadInt();
		bool flag = pkg.ReadBoolean();
		if (!_worldOpen)
		{
			MAX_BLOOD = mAX_BLOOD;
			m_blood = blood;
			_name = text;
			_bossResourceId = text2;
			_currentPVE = num;
			_worldOpen = flag;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.Out.SendOpenWorldBoss(gamePlayer.X, gamePlayer.Y);
			}
		}
	}

	public void WorldBossClose()
	{
		_worldOpen = false;
	}

	public void FightOver()
	{
		GSPacketIn packet = new GSPacketIn(82);
		GameServer.Instance.LoginServer.SendPacket(packet);
	}

	public void ReduceBlood(int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(84);
		gSPacketIn.WriteInt(value);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public void SendFightOver()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(102);
		gSPacketIn.WriteByte(8);
		gSPacketIn.WriteBoolean(val: true);
		SendToALLPlayers(gSPacketIn);
	}

	public void SendRoomClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(102);
		gSPacketIn.WriteByte(9);
		SendToALLPlayers(gSPacketIn);
	}

	public void UpdateWorldBossRankCrosszone(GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(102);
		gSPacketIn.WriteByte(10);
		bool flag = packet.ReadBoolean();
		int num = packet.ReadInt();
		gSPacketIn.WriteBoolean(flag);
		gSPacketIn.WriteInt(num);
		for (int i = 0; i < num; i++)
		{
			int val = packet.ReadInt();
			string str = packet.ReadString();
			int val2 = packet.ReadInt();
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteString(str);
			gSPacketIn.WriteInt(val2);
		}
		if (flag)
		{
			SendToALLPlayers(gSPacketIn);
		}
		else
		{
			SendToALL(gSPacketIn);
		}
	}

	public void SendPrivateInfo(string name)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(85);
		gSPacketIn.WriteString(name);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public void SendPrivateInfo(string name, int damage, int honor)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(102);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteInt(damage);
		gSPacketIn.WriteInt(honor);
		GamePlayer[] playersSafe = GetPlayersSafe();
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == name)
			{
				gamePlayer.Out.SendTCP(gSPacketIn);
				break;
			}
		}
	}

	public void SendUpdateBlood(GSPacketIn packet)
	{
		long val = packet.ReadLong();
		m_blood = packet.ReadLong();
		GSPacketIn gSPacketIn = new GSPacketIn(102);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteLong(val);
		gSPacketIn.WriteLong(m_blood);
		SendToALL(gSPacketIn);
	}

	public bool AddPlayer(GamePlayer player)
	{
		bool flag = false;
		lock (m_list)
		{
			if (!m_list.ContainsKey(player.PlayerId))
			{
				player.IsInWorldBossRoom = true;
				m_list.Add(player.PlayerId, player);
				flag = true;
				ShowRank();
				SendPrivateInfo(player.PlayerCharacter.NickName);
			}
		}
		if (flag)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(player.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(player.PlayerCharacter.Hide);
			gSPacketIn.WriteInt(player.PlayerCharacter.Repute);
			gSPacketIn.WriteInt(player.PlayerCharacter.ID);
			gSPacketIn.WriteString(player.PlayerCharacter.NickName);
			gSPacketIn.WriteByte(player.PlayerCharacter.typeVIP);
			gSPacketIn.WriteInt(player.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(player.PlayerCharacter.Sex);
			gSPacketIn.WriteString(player.PlayerCharacter.Style);
			gSPacketIn.WriteString(player.PlayerCharacter.Colors);
			gSPacketIn.WriteString(player.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(player.X);
			gSPacketIn.WriteInt(player.Y);
			gSPacketIn.WriteInt(player.PlayerCharacter.FightPower);
			gSPacketIn.WriteInt(player.PlayerCharacter.Win);
			gSPacketIn.WriteInt(player.PlayerCharacter.Total);
			gSPacketIn.WriteInt(player.PlayerCharacter.Offer);
			gSPacketIn.WriteByte(player.States);
			SendToALL(gSPacketIn);
		}
		return flag;
	}

	public void ViewOtherPlayerRoom(GamePlayer player)
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != player)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(102);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Hide);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Repute);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.ID);
				gSPacketIn.WriteString(gamePlayer.PlayerCharacter.NickName);
				gSPacketIn.WriteByte(gamePlayer.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteBoolean(gamePlayer.PlayerCharacter.Sex);
				gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Style);
				gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Colors);
				gSPacketIn.WriteString(gamePlayer.PlayerCharacter.Skin);
				gSPacketIn.WriteInt(gamePlayer.X);
				gSPacketIn.WriteInt(gamePlayer.Y);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
				gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
				gSPacketIn.WriteByte(gamePlayer.States);
				player.SendTCP(gSPacketIn);
			}
		}
	}

	public bool RemovePlayer(GamePlayer player)
	{
		bool flag = false;
		lock (m_list)
		{
			flag = m_list.Remove(player.PlayerId);
			GSPacketIn gSPacketIn = new GSPacketIn(102);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(player.PlayerId);
			SendToALL(gSPacketIn);
		}
		if (flag)
		{
			player.Out.SendSceneRemovePlayer(player);
		}
		return true;
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		lock (m_list)
		{
			array = new GamePlayer[m_list.Count];
			m_list.Values.CopyTo(array, 0);
		}
		if (array != null)
		{
			return array;
		}
		return new GamePlayer[0];
	}

	public void SendToALLPlayers(GSPacketIn packet)
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.SendTCP(packet);
		}
	}

	public void SendToALL(GSPacketIn packet)
	{
		SendToALL(packet, null);
	}

	public void SendToALL(GSPacketIn packet, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_list)
		{
			array = new GamePlayer[m_list.Count];
			m_list.Values.CopyTo(array, 0);
		}
		if (array == null)
		{
			return;
		}
		GamePlayer[] array2 = array;
		foreach (GamePlayer gamePlayer in array2)
		{
			if (gamePlayer != null && gamePlayer != except)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}
}
