using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using Game.Base;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;

namespace Game.Server.Battle;

public class BattleServer
{
	private int m_serverId;

	private FightServerConnector m_server;

	private int m_retryCount;

	private DateTime m_lastRetryTime;

	private Dictionary<int, BaseRoom> m_rooms;

	private string m_ip;

	private int m_port;

	private string m_loginKey;

	public int RetryCount
	{
		get
		{
			return m_retryCount;
		}
		set
		{
			m_retryCount = value;
		}
	}

	public DateTime LastRetryTime
	{
		get
		{
			return m_lastRetryTime;
		}
		set
		{
			m_lastRetryTime = value;
		}
	}

	public FightServerConnector Server => m_server;

	public FightServerConnector Connector => m_server;

	public string LoginKey => m_loginKey;

	public int ServerId => m_serverId;

	public bool IsActive => m_server.IsConnected;

	public string Ip => m_ip;

	public int Port => m_port;

	public event EventHandler Disconnected;

	public BattleServer(int serverId, string ip, int port, string loginKey)
	{
		m_serverId = serverId;
		m_ip = ip;
		m_port = port;
		m_loginKey = loginKey;
		m_retryCount = 0;
		m_lastRetryTime = DateTime.Now;
		m_server = new FightServerConnector(this, ip, port, loginKey);
		m_rooms = new Dictionary<int, BaseRoom>();
		m_server.Disconnected += m_server_Disconnected;
		m_server.Connected += m_server_Connected;
	}

	public BattleServer Clone()
	{
		return new BattleServer(m_serverId, m_ip, m_port, m_loginKey);
	}

	public void Start()
	{
		if (!m_server.Connect())
		{
			ThreadPool.QueueUserWorkItem(InvokeDisconnect);
		}
	}

	private void InvokeDisconnect(object state)
	{
		m_server_Disconnected(m_server);
	}

	private void m_server_Connected(BaseClient client)
	{
	}

	private void m_server_Disconnected(BaseClient client)
	{
		RemoveAllRoom();
		if (Disconnected != null)
		{
			Disconnected(this, null);
		}
	}

	public void RemoveAllRoom()
	{
		BaseRoom[] array = null;
		lock (m_rooms)
		{
			array = m_rooms.Values.ToArray();
			m_rooms.Clear();
		}
		BaseRoom[] array2 = array;
		foreach (BaseRoom baseRoom in array2)
		{
			if (baseRoom != null)
			{
				baseRoom.RemoveAllPlayer();
				RoomMgr.StopProxyGame(baseRoom);
			}
		}
	}

	public BaseRoom FindRoom(int roomId)
	{
		BaseRoom result = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(roomId))
			{
				result = m_rooms[roomId];
			}
		}
		return result;
	}

	public bool AddRoom(BaseRoom room)
	{
		bool flag = false;
		BaseRoom baseRoom = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(room.RoomId))
			{
				baseRoom = m_rooms[room.RoomId];
				m_rooms.Remove(room.RoomId);
			}
		}
		if (baseRoom != null && baseRoom.Game != null)
		{
			baseRoom.Game.Stop();
		}
		lock (m_rooms)
		{
			if (!m_rooms.ContainsKey(room.RoomId))
			{
				m_rooms.Add(room.RoomId, room);
				flag = true;
			}
		}
		if (flag)
		{
			m_server.SendAddRoom(room);
		}
		return flag;
	}

	public bool RemoveRoom(BaseRoom room)
	{
		bool flag = false;
		lock (m_rooms)
		{
			flag = m_rooms.ContainsKey(room.RoomId);
		}
		if (flag)
		{
			m_server.SendRemoveRoom(room);
		}
		return flag;
	}

	public void RemoveRoomImp(int roomId)
	{
		BaseRoom baseRoom = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(roomId))
			{
				baseRoom = m_rooms[roomId];
				m_rooms.Remove(roomId);
			}
		}
		if (baseRoom != null)
		{
			if (baseRoom.IsPlaying && baseRoom.Game == null)
			{
				RoomMgr.CancelPickup(this, baseRoom);
			}
			else
			{
				RoomMgr.StopProxyGame(baseRoom);
			}
		}
	}

	public void StartGame(int roomId, ProxyGame game)
	{
		BaseRoom baseRoom = FindRoom(roomId);
		if (baseRoom != null)
		{
			RoomMgr.StartProxyGame(baseRoom, game);
		}
	}

	public void StopGame(int roomId, int gameId)
	{
		BaseRoom baseRoom = FindRoom(roomId);
		if (baseRoom != null)
		{
			RoomMgr.StopProxyGame(baseRoom);
			lock (m_rooms)
			{
				m_rooms.Remove(roomId);
			}
		}
	}

	public void SendToRoom(int roomId, GSPacketIn pkg, int exceptId, int exceptGameId)
	{
		BaseRoom baseRoom = FindRoom(roomId);
		if (baseRoom == null)
		{
			return;
		}
		if (exceptId != 0)
		{
			GamePlayer playerById = WorldMgr.GetPlayerById(exceptId);
			if (playerById != null)
			{
				if (playerById.GamePlayerId == exceptGameId)
				{
					baseRoom.SendToAll(pkg, playerById);
				}
				else
				{
					baseRoom.SendToAll(pkg);
				}
			}
		}
		else
		{
			baseRoom.SendToAll(pkg);
		}
	}

	public void SendToUser(int playerid, GSPacketIn pkg)
	{
		WorldMgr.GetPlayerById(playerid)?.SendTCP(pkg);
	}

	public void UpdatePlayerGameId(int playerid, int gamePlayerId)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(playerid);
		if (playerById != null)
		{
			playerById.GamePlayerId = gamePlayerId;
		}
	}

	public override string ToString()
	{
		return $"ServerID:{m_serverId},Ip:{m_server.RemoteEP.Address},Port:{m_server.RemoteEP.Port},IsConnected:{m_server.IsConnected},RoomCount:{m_rooms.Count}";
	}
}
