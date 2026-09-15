using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Fighting.Server.GameObjects;
using Fighting.Server.Rooms;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using log4net;

namespace Fighting.Server.Games;

public class GameMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly long THREAD_INTERVAL = 40L;

	private static Dictionary<int, BaseGame> m_games;

	private static Thread m_thread;

	private static bool m_running;

	private static int m_serverId;

	private static int m_boxBroadcastLevel;

	private static int m_gameId;

	private static readonly int CLEAR_GAME_INTERVAL = 60000;

	private static long m_clearGamesTimer;

	public static int BoxBroadcastLevel => m_boxBroadcastLevel;

	public static bool Setup(int serverId, int boxBroadcastLevel)
	{
		m_thread = new Thread(GameThread);
		m_games = new Dictionary<int, BaseGame>();
		m_serverId = serverId;
		m_boxBroadcastLevel = boxBroadcastLevel;
		m_gameId = 0;
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

	private static void GameThread()
	{
		long num = 0L;
		m_clearGamesTimer = TickHelper.GetTickCount();
		while (m_running)
		{
			long tickCount = TickHelper.GetTickCount();
			try
			{
				UpdateGames(tickCount);
				ClearStoppedGames(tickCount);
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

	private static void UpdateGames(long tick)
	{
		IList games = GetGames();
		if (games == null)
		{
			return;
		}
		foreach (BaseGame item in games)
		{
			try
			{
				item.Update(tick);
			}
			catch (Exception exception)
			{
				log.Error("Game  updated error:", exception);
			}
		}
	}

	private static void ClearStoppedGames(long tick)
	{
		if (m_clearGamesTimer > tick)
		{
			return;
		}
		m_clearGamesTimer += CLEAR_GAME_INTERVAL;
		ArrayList arrayList = new ArrayList();
		lock (m_games)
		{
			foreach (BaseGame value in m_games.Values)
			{
				if (value.GameState == eGameState.Stopped)
				{
					arrayList.Add(value);
				}
			}
			foreach (BaseGame item in arrayList)
			{
				m_games.Remove(item.Id);
				try
				{
					item.Dispose();
				}
				catch (Exception exception)
				{
					log.Error("game dispose error:", exception);
				}
			}
		}
	}

	public static List<BaseGame> GetGames()
	{
		List<BaseGame> list = new List<BaseGame>();
		lock (m_games)
		{
			list.AddRange(m_games.Values);
		}
		return list;
	}

	public static BaseGame FindGame(int id)
	{
		lock (m_games)
		{
			if (m_games.ContainsKey(id))
			{
				return m_games[id];
			}
		}
		return null;
	}

	public static BaseGame StartPVPGame(List<IGamePlayer> red, List<IGamePlayer> blue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				PVPGame pVPGame = new PVPGame(m_gameId++, 0, red, blue, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(pVPGame.Id, pVPGame);
				}
				pVPGame.Prepare();
				return pVPGame;
			}
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create game error:", exception);
			return null;
		}
	}

	public static BattleGame StartBattleGame(List<IGamePlayer> red, ProxyRoom roomRed, List<IGamePlayer> blue, ProxyRoom roomBlue, int mapIndex, eRoomType roomType, eGameType gameType, int timeType)
	{
		try
		{
			int mapIndex2 = MapMgr.GetMapIndex(mapIndex, (byte)roomType, m_serverId);
			Map map = MapMgr.CloneMap(mapIndex2);
			if (map != null)
			{
				BattleGame battleGame = new BattleGame(m_gameId++, red, roomRed, blue, roomBlue, map, roomType, gameType, timeType);
				lock (m_games)
				{
					m_games.Add(battleGame.Id, battleGame);
				}
				battleGame.Prepare();
				SendStartMessage(battleGame);
				return battleGame;
			}
			return null;
		}
		catch (Exception exception)
		{
			log.Error("Create battle game error:", exception);
			return null;
		}
	}

	public static void SendStartMessage(BattleGame game)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(2);
		if (CanUseBuff(game.GameType))
		{
			foreach (Player allFightPlayer in game.GetAllFightPlayers())
			{
				(allFightPlayer.PlayerDetail as ProxyPlayer).m_antiAddictionRate = 1.0;
				GSPacketIn pkg = SendBufferList(allFightPlayer, (allFightPlayer.PlayerDetail as ProxyPlayer).Buffers);
				game.SendToAll(pkg);
			}
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("StartMessage.free"));
		}
		else if (game.RoomType == eRoomType.FightFootballTime)
		{
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("Ghép cặp thành công, quyết chiến bắt đầu."));
		}
		else
		{
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("StartMessage.free"));
			log.Warn(LanguageMgr.GetTranslation("CanUseBuff = false,  code:-121212"));
		}
		game.SendToAll(gSPacketIn, null);
		Console.WriteLine($"Create {game.RoomType} with {game.PlayerCount} player, createID {game.Id}");
	}

	public static bool CanUseBuff(eGameType gameType)
	{
		if (gameType <= eGameType.ALL)
		{
			if (gameType != eGameType.Free && gameType != eGameType.ALL)
			{
				return false;
			}
		}
		else if (gameType != eGameType.Encounter && gameType != eGameType.BattleGame)
		{
			return false;
		}
		return true;
	}

	public static GSPacketIn SendBufferList(Player player, List<BufferInfo> infos)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(186, player.Id);
		gSPacketIn.WriteInt(infos.Count);
		foreach (BufferInfo info in infos)
		{
			gSPacketIn.WriteInt(info.Type);
			gSPacketIn.WriteBoolean(info.IsExist);
			gSPacketIn.WriteDateTime(info.BeginDate);
			gSPacketIn.WriteInt(info.ValidDate);
			gSPacketIn.WriteInt(info.Value);
		}
		return gSPacketIn;
	}
}
