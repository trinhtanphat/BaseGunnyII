using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Rooms;

public class BaseChristmasRoom
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public static int LIVIN = 0;

	public static int DEAD = 2;

	public static int FIGHTING = 1;

	public static int MonterAddCount = 15;

	protected int lastMonterID = 1000;

	private int[] monterType = new int[3] { 0, 1, 2 };

	private Point[] brithPoint = new Point[15]
	{
		new Point(353, 570),
		new Point(246, 760),
		new Point(593, 590),
		new Point(466, 898),
		new Point(800, 950),
		new Point(946, 748),
		new Point(1152, 873),
		new Point(1172, 874),
		new Point(1766, 630),
		new Point(1342, 581),
		new Point(1732, 401),
		new Point(1462, 326),
		new Point(1187, 207),
		new Point(878, 236),
		new Point(1590, 521)
	};

	private Dictionary<int, GamePlayer> m_players;

	private Dictionary<int, MonterInfo> m_monters;

	public int DefaultPosX = 500;

	public int DefaultPosY = 500;

	public Dictionary<int, MonterInfo> Monters => m_monters;

	public BaseChristmasRoom()
	{
		m_players = new Dictionary<int, GamePlayer>();
		m_monters = new Dictionary<int, MonterInfo>();
		AddFistMonters();
	}

	public void AddFistMonters()
	{
		lock (m_monters)
		{
			for (int i = 0; i < MonterAddCount; i++)
			{
				MonterInfo monterInfo = new MonterInfo();
				monterInfo.ID = lastMonterID;
				monterInfo.type = random.Next(monterType.Length);
				monterInfo.MonsterPos = brithPoint[i];
				monterInfo.MonsterNewPos = brithPoint[i];
				monterInfo.state = LIVIN;
				monterInfo.PlayerID = 0;
				if (!m_monters.ContainsKey(monterInfo.ID))
				{
					m_monters.Add(monterInfo.ID, monterInfo);
				}
				lastMonterID++;
			}
		}
	}

	private int GetFreeMonter()
	{
		int num = 0;
		foreach (MonterInfo value in Monters.Values)
		{
			if (value.state == LIVIN)
			{
				num++;
			}
		}
		return num;
	}

	public void AddMoreMonters()
	{
		if (GetFreeMonter() < m_players.Count)
		{
			AddMonters();
		}
	}

	public void AddMonters()
	{
		lock (m_monters)
		{
			MonterInfo monterInfo = new MonterInfo();
			monterInfo.ID = lastMonterID;
			monterInfo.type = random.Next(monterType.Length);
			int num = random.Next(brithPoint.Length);
			monterInfo.MonsterPos = brithPoint[num];
			monterInfo.MonsterNewPos = brithPoint[num];
			monterInfo.state = LIVIN;
			monterInfo.PlayerID = 0;
			if (!m_monters.ContainsKey(monterInfo.ID))
			{
				m_monters.Add(monterInfo.ID, monterInfo);
			}
			lastMonterID++;
		}
	}

	public bool SetFightMonter(int Id, int playerId)
	{
		bool result = false;
		lock (m_monters)
		{
			if (m_monters.ContainsKey(Id))
			{
				m_monters[Id].state = FIGHTING;
				m_monters[Id].PlayerID = playerId;
				result = true;
			}
		}
		AddMonters();
		return result;
	}

	public void SetMonterDie(int playerId)
	{
		int num = -1;
		foreach (MonterInfo value in m_monters.Values)
		{
			if (value.PlayerID == playerId)
			{
				num = value.ID;
				break;
			}
		}
		if (num <= -1)
		{
			return;
		}
		lock (m_monters)
		{
			if (m_monters.ContainsKey(num))
			{
				m_monters.Remove(num);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(22);
		gSPacketIn.WriteByte(1);
		gSPacketIn.WriteInt(num);
		SendToALL(gSPacketIn);
	}

	public void AddPlayer(GamePlayer player)
	{
		lock (m_players)
		{
			if (!m_players.ContainsKey(player.PlayerId))
			{
				player.IsInChristmasRoom = true;
				m_players.Add(player.PlayerId, player);
				player.Actives.BeginChristmasTimer();
			}
		}
		UpdateRoom();
	}

	public void UpdateRoom()
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(18);
		gSPacketIn.WriteInt(playersSafe.Length);
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
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
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
			gSPacketIn.WriteInt(gamePlayer.X);
			gSPacketIn.WriteInt(gamePlayer.Y);
			gSPacketIn.WriteByte(gamePlayer.States);
		}
		SendToALL(gSPacketIn);
	}

	public void ViewOtherPlayerRoom(GamePlayer player)
	{
		GamePlayer[] playersSafe = GetPlayersSafe();
		GSPacketIn gSPacketIn = new GSPacketIn(145);
		gSPacketIn.WriteByte(18);
		gSPacketIn.WriteInt(playersSafe.Length);
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
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
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.FightPower);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Win);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Total);
			gSPacketIn.WriteInt(gamePlayer.PlayerCharacter.Offer);
			gSPacketIn.WriteInt(gamePlayer.X);
			gSPacketIn.WriteInt(gamePlayer.Y);
			gSPacketIn.WriteByte(gamePlayer.States);
		}
		player.SendTCP(gSPacketIn);
	}

	public bool RemovePlayer(GamePlayer player)
	{
		bool flag = false;
		lock (m_players)
		{
			player.Actives.StopChristmasTimer();
			flag = m_players.Remove(player.PlayerId);
		}
		if (flag)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(145);
			gSPacketIn.WriteByte(19);
			gSPacketIn.WriteInt(player.PlayerId);
			SendToALL(gSPacketIn);
			player.IsInChristmasRoom = false;
			player.Out.SendSceneRemovePlayer(player);
		}
		return flag;
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		lock (m_players)
		{
			array = new GamePlayer[m_players.Count];
			m_players.Values.CopyTo(array, 0);
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
		GamePlayer[] playersSafe = GetPlayersSafe();
		if (playersSafe == null)
		{
			return;
		}
		GamePlayer[] array = playersSafe;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != null && gamePlayer != except)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}
}
