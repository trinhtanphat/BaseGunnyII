using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Fighting.Server.Games;
using Game.Logic;
using log4net;

namespace Fighting.Server.Rooms;

public class ProxyRoomMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly int THREAD_INTERVAL = 40;

	public static readonly int PICK_UP_INTERVAL = 10000;

	public static readonly int CLEAR_ROOM_INTERVAL = 1000;

	private static bool m_running = false;

	private static int m_serverId = 1;

	private static Queue<IAction> m_actionQueue = new Queue<IAction>();

	private static Thread m_thread;

	private static Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

	private static int RoomIndex = 0;

	private static long m_nextPickTick = 0L;

	private static long m_nextClearTick = 0L;

	public static bool Setup()
	{
		m_thread = new Thread(RoomThread);
		return true;
	}

	public static void Start()
	{
		if (!m_running)
		{
			m_running = true;
			m_thread.Start();
		}
	}

	public static void Stop()
	{
		if (m_running)
		{
			m_running = false;
			m_thread.Join();
		}
	}

	public static void AddAction(IAction action)
	{
		lock (m_actionQueue)
		{
			m_actionQueue.Enqueue(action);
		}
	}

	private static void RoomThread()
	{
		long num = 0L;
		m_nextClearTick = TickHelper.GetTickCount();
		m_nextPickTick = TickHelper.GetTickCount();
		while (m_running)
		{
			long tickCount = TickHelper.GetTickCount();
			try
			{
				ExecuteActions();
				if (m_nextPickTick <= tickCount)
				{
					m_nextPickTick += PICK_UP_INTERVAL;
					PickUpRooms(tickCount);
				}
				if (m_nextClearTick <= tickCount)
				{
					m_nextClearTick += CLEAR_ROOM_INTERVAL;
					ClearRooms(tickCount);
				}
			}
			catch (Exception exception)
			{
				log.Error("Room Mgr Thread Error:", exception);
			}
			long tickCount2 = TickHelper.GetTickCount();
			num += THREAD_INTERVAL - (tickCount2 - tickCount);
			if (num > 0)
			{
				Thread.Sleep((int)num);
				num = 0L;
			}
			else if (num < -1000)
			{
				log.WarnFormat("Room Mgr is delay {0} ms!", num);
				num += 1000;
			}
		}
	}

	private static void ExecuteActions()
	{
		IAction[] array = null;
		lock (m_actionQueue)
		{
			if (m_actionQueue.Count > 0)
			{
				array = new IAction[m_actionQueue.Count];
				m_actionQueue.CopyTo(array, 0);
				m_actionQueue.Clear();
			}
		}
		if (array == null)
		{
			return;
		}
		IAction[] array2 = array;
		foreach (IAction action in array2)
		{
			try
			{
				action.Execute();
			}
			catch (Exception exception)
			{
				log.Error("RoomMgr execute action error:", exception);
			}
		}
	}

	private static void PickUpRooms(long tick)
	{
		List<ProxyRoom> waitMatchRoomUnsafe = GetWaitMatchRoomUnsafe();
		foreach (ProxyRoom item in waitMatchRoomUnsafe)
		{
			int num = 5000;
			int num2 = 2;
			ProxyRoom proxyRoom = null;
			if (item.IsPlaying)
			{
				break;
			}
			eRoomType roomType = item.RoomType;
			if (roomType <= eRoomType.Encounter)
			{
				if (roomType != eRoomType.Match)
				{
					if (roomType != eRoomType.BattleRoom)
					{
						if (roomType == eRoomType.Encounter)
						{
							foreach (ProxyRoom item2 in waitMatchRoomUnsafe)
							{
								if (item2 != item && item2.RoomType == eRoomType.Encounter && !item2.IsPlaying && item2.PlayerCount == item.PlayerCount)
								{
									proxyRoom = item2;
								}
							}
						}
					}
					else
					{
						foreach (ProxyRoom item3 in waitMatchRoomUnsafe)
						{
							if (item3 != item && item3.RoomType == eRoomType.BattleRoom && !item3.IsPlaying && item3.PlayerCount == item.PlayerCount)
							{
								proxyRoom = item3;
							}
						}
					}
				}
				else if (item.GameType == eGameType.Guild)
				{
					foreach (ProxyRoom item4 in waitMatchRoomUnsafe)
					{
						if ((item4.GuildId == 0 || item4.GuildId != item.GuildId) && item4 != item && item4.PlayerCount == item.PlayerCount && !item4.IsPlaying && item4.GameType == eGameType.Guild)
						{
							int num3 = CalculateScore(item, item4);
							if (num3 < num || item.PickUpCount > num2)
							{
								proxyRoom = item4;
							}
						}
					}
				}
				else
				{
					foreach (ProxyRoom item5 in waitMatchRoomUnsafe)
					{
						if (item5 != item && item5.PlayerCount == item.PlayerCount && !item5.IsPlaying && (item5.GameType == eGameType.ALL || item5.GameType == eGameType.Free))
						{
							int num3 = CalculateScore(item, item5);
							if (num3 < num || item.PickUpCount > num2)
							{
								proxyRoom = item5;
							}
						}
					}
					Console.WriteLine("MatchGame free mode");
				}
			}
			else if (roomType != eRoomType.ConsortiaBattle)
			{
				if (roomType != eRoomType.SingleBattle)
				{
					if (roomType == eRoomType.FightFootballTime)
					{
						foreach (ProxyRoom item6 in waitMatchRoomUnsafe)
						{
							if (item6 != item && item6.RoomType == eRoomType.FightFootballTime && !item6.IsPlaying && item6.PlayerCount == item.PlayerCount)
							{
								proxyRoom = item6;
							}
						}
					}
				}
				else
				{
					foreach (ProxyRoom item7 in waitMatchRoomUnsafe)
					{
						if (item7 != item && item7.RoomType == eRoomType.SingleBattle && !item7.IsPlaying && item7.PlayerCount == item.PlayerCount)
						{
							proxyRoom = item7;
						}
					}
				}
			}
			else
			{
				foreach (ProxyRoom item8 in waitMatchRoomUnsafe)
				{
					if (item8 != item && item8.RoomType == eRoomType.ConsortiaBattle && !item8.IsPlaying && item8.PlayerCount == item.PlayerCount)
					{
						proxyRoom = item8;
					}
				}
			}
			if (proxyRoom != null)
			{
				StartMatchGame(item, proxyRoom);
				continue;
			}
			item.PickUpCount++;
			int pickUpCount = item.PickUpCount;
			int num4 = num2 * 2;
		}
	}

	public static ProxyRoom CloneTeam(ProxyRoom red)
	{
		return null;
	}

	private static int CalculateScore(ProxyRoom red, ProxyRoom blue)
	{
		return Math.Abs(red.FightPower - blue.FightPower);
	}

	private static void ClearRooms(long tick)
	{
		List<ProxyRoom> list = new List<ProxyRoom>();
		foreach (ProxyRoom value in m_rooms.Values)
		{
			if (!value.IsPlaying && value.Game != null)
			{
				list.Add(value);
			}
		}
		foreach (ProxyRoom item in list)
		{
			m_rooms.Remove(item.RoomId);
			try
			{
				item.Dispose();
			}
			catch (Exception exception)
			{
				log.Error("Room dispose error:", exception);
			}
		}
	}

	private static void StartMatchGame(ProxyRoom red, ProxyRoom blue)
	{
		int mapIndex = MapMgr.GetMapIndex(0, 0, m_serverId);
		eGameType gameType = eGameType.Free;
		eRoomType roomType = eRoomType.Match;
		if (red.GameType == blue.GameType)
		{
			gameType = red.GameType;
			roomType = red.RoomType;
		}
		if (red.RoomType == eRoomType.FightFootballTime)
		{
			mapIndex = 1313;
		}
		BaseGame baseGame = GameMgr.StartBattleGame(red.GetPlayers(), red, blue.GetPlayers(), blue, mapIndex, roomType, gameType, 2);
		if (baseGame != null)
		{
			blue.StartGame(baseGame);
			red.StartGame(baseGame);
		}
		if (baseGame.GameType == eGameType.Guild)
		{
			red.Client.SendConsortiaAlly(red.GetPlayers()[0].PlayerCharacter.ConsortiaID, blue.GetPlayers()[0].PlayerCharacter.ConsortiaID, baseGame.Id);
		}
	}

	private static void StartAutoBotGame(ProxyRoom red)
	{
		int mapIndex = MapMgr.GetMapIndex(0, 0, m_serverId);
		eGameType gameType = eGameType.Free;
		eRoomType roomType = eRoomType.Match;
		BaseGame baseGame = GameMgr.StartBattleGame(red.GetPlayers(), red, null, null, mapIndex, roomType, gameType, 2);
		if (baseGame != null)
		{
			red.StartGame(baseGame);
		}
	}

	public static bool AddRoomUnsafe(ProxyRoom room)
	{
		if (!m_rooms.ContainsKey(room.RoomId))
		{
			m_rooms.Add(room.RoomId, room);
			return true;
		}
		return false;
	}

	public static bool RemoveRoomUnsafe(int roomId)
	{
		if (m_rooms.ContainsKey(roomId))
		{
			m_rooms.Remove(roomId);
			return true;
		}
		return false;
	}

	public static ProxyRoom GetRoomUnsafe(int roomId)
	{
		if (m_rooms.ContainsKey(roomId))
		{
			return m_rooms[roomId];
		}
		return null;
	}

	public static ProxyRoom[] GetAllRoom()
	{
		lock (m_rooms)
		{
			return GetAllRoomUnsafe();
		}
	}

	public static ProxyRoom[] GetAllRoomUnsafe()
	{
		ProxyRoom[] array = new ProxyRoom[m_rooms.Values.Count];
		m_rooms.Values.CopyTo(array, 0);
		return array;
	}

	public static List<ProxyRoom> GetWaitMatchRoomUnsafe()
	{
		List<ProxyRoom> list = new List<ProxyRoom>();
		foreach (ProxyRoom value in m_rooms.Values)
		{
			if (!value.IsPlaying && value.Game == null)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static int NextRoomId()
	{
		return Interlocked.Increment(ref RoomIndex);
	}

	public static void AddRoom(ProxyRoom room)
	{
		AddAction(new AddRoomAction(room));
	}

	public static void RemoveRoom(ProxyRoom room)
	{
		AddAction(new RemoveRoomAction(room));
	}
}
