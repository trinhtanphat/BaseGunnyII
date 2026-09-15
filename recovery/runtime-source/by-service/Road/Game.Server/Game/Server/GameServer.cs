using System;
using System.Collections;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Base.Events;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using Game.Server.Statics;
using SqlDataProvider.Data;
using log4net;
using log4net.Config;

namespace Game.Server;

public class GameServer : BaseServer
{
	private const int BUF_SIZE = 8192;

	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static readonly string Edition = "5498628";

	public static bool KeepRunning = false;

	private static GameServer m_instance = null;

	private bool m_isRunning;

	private GameServerConfig m_config;

	private LoginServerConnector _loginServer;

	private Queue m_packetBufPool;

	private bool m_debugMenory;

	private static int m_tryCount = 4;

	private static bool m_compiled = false;

	private Timer _shutdownTimer;

	private int _shutdownCount = 6;

	protected Timer m_saveDbTimer;

	protected Timer m_pingCheckTimer;

	protected Timer m_saveRecordTimer;

	protected Timer m_buffScanTimer;

	protected Timer m_qqTipScanTimer;

	protected Timer m_bagMailScanTimer;

	public static GameServer Instance => m_instance;

	public GameServerConfig Configuration => m_config;

	public LoginServerConnector LoginServer => _loginServer;

	public int PacketPoolSize => m_packetBufPool.Count;

	public static void CreateInstance(GameServerConfig config)
	{
		if (m_instance == null)
		{
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(GameServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			m_instance = new GameServer(config);
		}
	}

	protected GameServer(GameServerConfig config)
	{
		m_config = config;
		if (log.IsDebugEnabled)
		{
			log.Debug("Current directory is: " + Directory.GetCurrentDirectory());
			log.Debug("Gameserver root directory is: " + Configuration.RootDirectory);
			log.Debug("Changing directory to root directory");
		}
		Directory.SetCurrentDirectory(Configuration.RootDirectory);
	}

	private bool AllocatePacketBuffers()
	{
		int num = Configuration.MaxClientCount * 3;
		m_packetBufPool = new Queue(num);
		for (int i = 0; i < num; i++)
		{
			m_packetBufPool.Enqueue(new byte[8192]);
		}
		if (log.IsDebugEnabled)
		{
			log.DebugFormat("allocated packet buffers: {0}", num.ToString());
		}
		return true;
	}

	public byte[] AcquirePacketBuffer()
	{
		lock (m_packetBufPool.SyncRoot)
		{
			if (m_packetBufPool.Count > 0)
			{
				return (byte[])m_packetBufPool.Dequeue();
			}
		}
		log.Warn("packet buffer pool is empty!");
		return new byte[8192];
	}

	public void ReleasePacketBuffer(byte[] buf)
	{
		if (buf == null || GC.GetGeneration(buf) < GC.MaxGeneration)
		{
			return;
		}
		lock (m_packetBufPool.SyncRoot)
		{
			m_packetBufPool.Enqueue(buf);
		}
	}

	protected override BaseClient GetNewClient()
	{
		return new GameClient(this, AcquirePacketBuffer(), AcquirePacketBuffer());
	}

	public new GameClient[] GetAllClients()
	{
		GameClient[] array = null;
		lock (_clients.SyncRoot)
		{
			array = new GameClient[_clients.Count];
			_clients.Keys.CopyTo(array, 0);
		}
		return array;
	}

	public override bool Start()
	{
		if (m_isRunning)
		{
			return false;
		}
		bool result;
		try
		{
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			Thread.CurrentThread.Priority = ThreadPriority.Normal;
			GameProperties.Refresh();
			if (!InitComponent(RecompileScripts(), "Recompile Scripts"))
			{
				result = false;
			}
			else if (!InitComponent(StartScriptComponents(), "Script components"))
			{
				result = false;
			}
			else if (!InitComponent(GameProperties.EDITION == Edition, "Edition: " + Edition))
			{
				result = false;
			}
			else if (!InitComponent(InitSocket(IPAddress.Parse(Configuration.Ip), Configuration.Port), "InitSocket Port: " + Configuration.Port))
			{
				result = false;
			}
			else if (!InitComponent(AllocatePacketBuffers(), "AllocatePacketBuffers()"))
			{
				result = false;
			}
			else if (!InitComponent(LogMgr.Setup(Configuration.GAME_TYPE, Configuration.ServerID, Configuration.AreaID), "LogMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(WorldMgr.Init(), "WorldMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(MapMgr.Init(), "MapMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(ItemMgr.Init(), "ItemMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(ItemBoxMgr.Init(), "ItemBox Init"))
			{
				result = false;
			}
			else if (!InitComponent(BallMgr.Init(), "BallMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(ExerciseMgr.Init(), "ExerciseMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(LevelMgr.Init(), "levelMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(BallConfigMgr.Init(), "BallConfigMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(FusionMgr.Init(), "FusionMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(AwardMgr.Init(), "AwardMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(AchievementMgr.Init(), "AchievementMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(NPCInfoMgr.Init(), "NPCInfoMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(MissionInfoMgr.Init(), "MissionInfoMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(PveInfoMgr.Init(), "PveInfoMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(DropMgr.Init(), "Drop Init"))
			{
				result = false;
			}
			else if (!InitComponent(FightRateMgr.Init(), "FightRateMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(RefineryMgr.Init(), "RefineryMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(StrengthenMgr.Init(), "StrengthenMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(PropItemMgr.Init(), "PropItemMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(ShopMgr.Init(), "ShopMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(QuestMgr.Init(), "QuestMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(RoomMgr.Setup(Configuration.MaxRoomCount), "RoomMgr.Setup"))
			{
				result = false;
			}
			else if (!InitComponent(GameMgr.Setup(Configuration.ServerID, GameProperties.BOX_APPEAR_CONDITION), "GameMgr.Start()"))
			{
				result = false;
			}
			else if (!InitComponent(ConsortiaMgr.Init(), "ConsortiaMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(ConsortiaExtraMgr.Init(), "ConsortiaExtraMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(RateMgr.Init(Configuration), "ExperienceRateMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(WindMgr.Init(), "WindMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(CardMgr.Init(), "CardMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(PetMgr.Init(), "PetMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(GoldEquipMgr.Init(), "GoldEquipMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(RuneMgr.Init(), "RuneMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(TotemMgr.Init(), "TotemMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(TotemHonorMgr.Init(), "TotemHonorMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(TreasureAwardMgr.Init(), "TreasureAwardMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(FairBattleRewardMgr.Init(), "FairBattleRewardMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(FightSpiritTemplateMgr.Init(), "FightSpiritTemplateMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(MacroDropMgr.Init(), "MacroDropMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(MarryRoomMgr.Init(), "MarryRoomMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(RankMgr.Init(), "RankMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(CommunalActiveMgr.Init(), "CommunalActiveMgr Setup"))
			{
				result = false;
			}
			else if (!InitComponent(ActiveSystemMgr.Init(), "ActiveSystemMgr Setup"))
			{
				result = false;
			}
			else if (!InitComponent(QQTipsMgr.Init(), "QQTipsMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(LightriddleQuestMgr.Init(), "LightriddleQuestMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(WorldEventMgr.Init(), "WorldEventMgr Init"))
			{
				result = false;
			}
			else if (!InitComponent(BattleMgr.Setup(), "BattleMgr Setup"))
			{
				result = false;
			}
			else if (!InitComponent(InitGlobalTimer(), "Init Global Timers"))
			{
				result = false;
			}
			else if (!InitComponent(LogMgr.Setup(1, 4, 4), "LogMgr Setup"))
			{
				result = false;
			}
			else
			{
				GameEventMgr.Notify(ScriptEvent.Loaded);
				if (!InitComponent(InitLoginServer(), "Login To CenterServer"))
				{
					result = false;
				}
				else
				{
					RoomMgr.Start();
					GameMgr.Start();
					BattleMgr.Start();
					MacroDropMgr.Start();
					if (!InitComponent(base.Start(), "base.Start()"))
					{
						result = false;
					}
					else
					{
						GameEventMgr.Notify(GameServerEvent.Started, this);
						GC.Collect(GC.MaxGeneration);
						if (log.IsInfoEnabled)
						{
							log.Info("GameServer is now open for connections!");
						}
						m_isRunning = true;
						result = true;
					}
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Failed to start the server", exception);
			}
			result = false;
		}
		return result;
	}

	private bool InitLoginServer()
	{
		_loginServer = new LoginServerConnector(m_config.LoginServerIp, m_config.LoginServerPort, m_config.ServerID, m_config.ServerName, AcquirePacketBuffer(), AcquirePacketBuffer());
		_loginServer.Disconnected += loginServer_Disconnected;
		return _loginServer.Connect();
	}

	private void loginServer_Disconnected(BaseClient client)
	{
		bool isRunning = m_isRunning;
		Stop();
		if (isRunning && m_tryCount > 0)
		{
			m_tryCount--;
			log.Error("Center Server Disconnect! Stopping Server");
			log.ErrorFormat("Start the game server again after 1 second,and left try times:{0}", m_tryCount);
			Thread.Sleep(1000);
			if (Start())
			{
				log.Error("Restart the game server success!");
			}
		}
		else
		{
			if (m_tryCount == 0)
			{
				log.ErrorFormat("Restart the game server failed after {0} times.", 4);
				log.Error("Server Stopped!");
			}
			LogManager.Shutdown();
		}
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		try
		{
			log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
			if (e.IsTerminating)
			{
				Stop();
			}
		}
		catch
		{
			try
			{
				using FileStream stream = new FileStream("c:\\testme.log", FileMode.Append, FileAccess.Write);
				using StreamWriter streamWriter = new StreamWriter(stream, Encoding.UTF8);
				streamWriter.WriteLine(e.ExceptionObject);
			}
			catch
			{
			}
		}
	}

	public bool RecompileScripts()
	{
		if (!m_compiled)
		{
			string path = Configuration.RootDirectory + Path.DirectorySeparatorChar + "scripts";
			if (!Directory.Exists(path))
			{
				Directory.CreateDirectory(path);
			}
			string[] asm_names = Configuration.ScriptAssemblies.Split(',');
			m_compiled = ScriptMgr.CompileScripts(compileVB: false, path, Configuration.ScriptCompilationTarget, asm_names);
		}
		return m_compiled;
	}

	protected bool StartScriptComponents()
	{
		try
		{
			if (log.IsInfoEnabled)
			{
				log.Info("Server rules: true");
			}
			ScriptMgr.InsertAssembly(typeof(GameServer).Assembly);
			ScriptMgr.InsertAssembly(typeof(BaseGame).Assembly);
			ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
			ArrayList arrayList = new ArrayList(ScriptMgr.Scripts);
			foreach (Assembly item in arrayList)
			{
				GameEventMgr.RegisterGlobalEvents(item, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
				GameEventMgr.RegisterGlobalEvents(item, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
				GameEventMgr.RegisterGlobalEvents(item, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
				GameEventMgr.RegisterGlobalEvents(item, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
			}
			if (log.IsInfoEnabled)
			{
				log.Info("Registering global event handlers: true");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("StartScriptComponents", exception);
			}
			return false;
		}
		return true;
	}

	protected bool InitComponent(bool componentInitState, string text)
	{
		if (m_debugMenory)
		{
			log.Debug("Start Memory " + text + ": " + GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024);
		}
		if (log.IsInfoEnabled)
		{
			log.Info(text + ": " + componentInitState);
		}
		if (!componentInitState)
		{
			Stop();
		}
		if (m_debugMenory)
		{
			log.Debug("Finish Memory " + text + ": " + GC.GetTotalMemory(forceFullCollection: false) / 1024 / 1024);
		}
		return componentInitState;
	}

	public override void Stop()
	{
		if (m_isRunning)
		{
			m_isRunning = false;
			if (!MarryRoomMgr.UpdateBreakTimeWhereServerStop())
			{
				Console.WriteLine("Update BreakTime failed");
			}
			RoomMgr.Stop();
			GameMgr.Stop();
			WorldMgr.ScanBagMail();
			ActiveSystemMgr.StopAllTimer();
			if (_loginServer != null)
			{
				_loginServer.Disconnected -= loginServer_Disconnected;
				_loginServer.Disconnect();
			}
			if (m_pingCheckTimer != null)
			{
				m_pingCheckTimer.Change(-1, -1);
				m_pingCheckTimer.Dispose();
				m_pingCheckTimer = null;
			}
			if (m_saveDbTimer != null)
			{
				m_saveDbTimer.Change(-1, -1);
				m_saveDbTimer.Dispose();
				m_saveDbTimer = null;
			}
			if (m_saveRecordTimer != null)
			{
				m_saveRecordTimer.Change(-1, -1);
				m_saveRecordTimer.Dispose();
				m_saveRecordTimer = null;
			}
			if (m_buffScanTimer != null)
			{
				m_buffScanTimer.Change(-1, -1);
				m_buffScanTimer.Dispose();
				m_buffScanTimer = null;
			}
			if (m_qqTipScanTimer != null)
			{
				m_qqTipScanTimer.Change(-1, -1);
				m_qqTipScanTimer.Dispose();
				m_qqTipScanTimer = null;
			}
			if (m_bagMailScanTimer != null)
			{
				m_bagMailScanTimer.Change(-1, -1);
				m_bagMailScanTimer.Dispose();
				m_bagMailScanTimer = null;
			}
			base.Stop();
			Thread.CurrentThread.Priority = ThreadPriority.BelowNormal;
			log.Info("Server Stopped!");
			Console.WriteLine("Server Stopped!");
		}
	}

	public void Shutdown()
	{
		Instance.LoginServer.SendShutdown(isStoping: true);
		_shutdownTimer = new Timer(ShutDownCallBack, null, 0, 60000);
	}

	private void ShutDownCallBack(object state)
	{
		try
		{
			_shutdownCount--;
			Console.WriteLine($"Server will shutdown after {_shutdownCount} mins!");
			GameClient[] allClients = Instance.GetAllClients();
			GameClient[] array = allClients;
			foreach (GameClient gameClient in array)
			{
				if (gameClient.Out != null)
				{
					gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _shutdownCount, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
				}
			}
			if (_shutdownCount == 0)
			{
				Console.WriteLine("Server has stopped!");
				Instance.LoginServer.SendShutdown(isStoping: false);
				_shutdownTimer.Dispose();
				_shutdownTimer = null;
				Instance.Stop();
			}
		}
		catch (Exception message)
		{
			log.Error(message);
		}
	}

	public bool InitGlobalTimer()
	{
		int num = Configuration.PingCheckInterval * 60 * 1000;
		if (m_pingCheckTimer == null)
		{
			m_pingCheckTimer = new Timer(PingCheck, null, num, num);
		}
		else
		{
			m_pingCheckTimer.Change(num, num);
		}
		num = Configuration.SaveRecordInterval * 60 * 1000;
		if (m_saveRecordTimer != null)
		{
			m_saveRecordTimer.Change(num, num);
		}
		num = 60000;
		if (m_buffScanTimer == null)
		{
			m_buffScanTimer = new Timer(BuffScanTimerProc, null, num, num);
		}
		else
		{
			m_buffScanTimer.Change(num, num);
		}
		num = 300000;
		if (m_qqTipScanTimer == null)
		{
			m_qqTipScanTimer = new Timer(QQTipScanTimerProc, null, num, num);
		}
		else
		{
			m_qqTipScanTimer.Change(num, num);
		}
		num = 900000;
		if (m_bagMailScanTimer == null)
		{
			m_bagMailScanTimer = new Timer(BagMailScanTimerProc, null, num, num);
		}
		else
		{
			m_bagMailScanTimer.Change(num, num);
		}
		return true;
	}

	protected void PingCheck(object sender)
	{
		try
		{
			log.Info("Begin ping check....");
			long num = (long)Configuration.PingCheckInterval * 60L * 1000 * 1000 * 10;
			GameClient[] allClients = GetAllClients();
			if (allClients != null)
			{
				GameClient[] array = allClients;
				foreach (GameClient gameClient in array)
				{
					if (gameClient == null)
					{
						continue;
					}
					if (gameClient.IsConnected)
					{
						if (gameClient.Player != null)
						{
							gameClient.Out.SendPingTime(gameClient.Player);
							if (AntiAddictionMgr.ISASSon && AntiAddictionMgr.count == 0)
							{
								AntiAddictionMgr.count++;
							}
						}
						else if (gameClient.PingTime + num < DateTime.Now.Ticks)
						{
							gameClient.Disconnect();
						}
					}
					else
					{
						gameClient.Disconnect();
					}
				}
			}
			log.Info("End ping check....");
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PingCheck callback", exception);
			}
		}
		try
		{
			log.Info("Begin ping center check....");
			Instance.LoginServer.SendPingCenter();
			log.Info("End ping center check....");
		}
		catch (Exception exception2)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PingCheck center callback", exception2);
			}
		}
	}

	protected void SaveTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving database...");
				log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			int num = 0;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.SaveIntoDatabase();
				num++;
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving database complete!");
				log.Info("Saved all databases and " + num + " players in " + tickCount + "ms");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Saved all databases and {0} players in {1} ms", num, tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SaveTimerProc", exception);
			}
		}
		finally
		{
			GameEventMgr.Notify(GameServerEvent.WorldSave);
		}
	}

	protected void SaveRecordProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving Record...");
				log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			LogMgr.Save();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving Record complete!");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Saved all Record  in {0} ms", tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SaveRecordProc", exception);
			}
		}
	}

	protected void BuffScanTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Buff Scaning ...");
				log.Debug("BuffScan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			int num = 0;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				if (gamePlayer.BufferList != null)
				{
					gamePlayer.BufferList.Update();
					num++;
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Buff Scan complete!");
				log.Info("Buff all " + num + " players in " + tickCount + "ms");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Scan all Buff and {0} players in {1} ms", num, tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("BuffScanTimerProc", exception);
			}
		}
	}

	protected void QQTipScanTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("QQTips Scaning ...");
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			QQtipsMessagesInfo qQtipsMessages = QQTipsMgr.GetQQtipsMessages();
			GamePlayer[] allPlayersNoGame = WorldMgr.GetAllPlayersNoGame();
			GamePlayer[] array = allPlayersNoGame;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.Out.SendQQtips(gamePlayer.PlayerId, qQtipsMessages);
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("QQTips Scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("QQTipScanTimerProc", exception);
			}
		}
	}

	protected void BagMailScanTimerProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("BagMail Scaning ...");
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			WorldMgr.ScanBagMail();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("BagMail Scan complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("BagMailScanTimerProc", exception);
			}
		}
	}
}
