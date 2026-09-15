using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Bussiness.Protocol;
using Center.Server.Managers;
using Center.Server.Statics;
using Game.Base;
using Game.Base.Events;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;
using log4net.Config;

namespace Center.Server;

public class CenterServer : BaseServer
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private CenterServerConfig _config;

	private string Edition = "5498628";

	private Random rand = new Random();

	private bool _aSSState;

	private bool _dailyAwardState;

	private Timer m_loginLapseTimer;

	private Timer m_saveDBTimer;

	private Timer m_saveRecordTimer;

	private Timer m_scanAuction;

	private Timer m_scanMail;

	private Timer m_scanConsortia;

	private Timer m_sytemNotice;

	private Timer m_worldEvent;

	private Timer m_consortiaboss;

	private readonly int awardTime = 20;

	private static CenterServer _instance;

	public bool ASSState
	{
		get
		{
			return _aSSState;
		}
		set
		{
			_aSSState = value;
		}
	}

	public bool DailyAwardState
	{
		get
		{
			return _dailyAwardState;
		}
		set
		{
			_dailyAwardState = value;
		}
	}

	public static CenterServer Instance => _instance;

	protected override BaseClient GetNewClient()
	{
		return new ServerClient(this);
	}

	public override bool Start()
	{
		try
		{
			Thread.CurrentThread.Priority = ThreadPriority.Normal;
			AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;
			GameProperties.Refresh();
			if (!InitComponent(RecompileScripts(), "Recompile Scripts"))
			{
				return false;
			}
			if (!InitComponent(StartScriptComponents(), "Script components"))
			{
				return false;
			}
			if (!InitComponent(GameProperties.EDITION == Edition, "Check Server Edition:" + Edition))
			{
				return false;
			}
			if (!InitComponent(InitSocket(IPAddress.Parse(_config.Ip), _config.Port), "InitSocket Port:" + _config.Port))
			{
				return false;
			}
			if (!InitComponent(CenterService.Start(), "Center Service"))
			{
				return false;
			}
			if (!InitComponent(ServerMgr.Start(), "Load serverlist"))
			{
				return false;
			}
			if (!InitComponent(ConsortiaExtraMgr.Init(), "Init ConsortiaLevelMgr"))
			{
				return false;
			}
			if (!InitComponent(MacroDropMgr.Init(), "Init MacroDropMgr"))
			{
				return false;
			}
			if (!InitComponent(WorldEventMgr.Init(), "WorldEventMgr Init"))
			{
				return false;
			}
			if (!InitComponent(LanguageMgr.Setup(""), "LanguageMgr Init"))
			{
				return false;
			}
			if (!InitComponent(WorldMgr.Start(), "WorldMgr Init"))
			{
				return false;
			}
			if (!InitComponent(InitGlobalTimers(), "Init Global Timers"))
			{
				return false;
			}
			GameEventMgr.Notify(ScriptEvent.Loaded);
			MacroDropMgr.Start();
			if (!InitComponent(base.Start(), "base.Start()"))
			{
				return false;
			}
			GameEventMgr.Notify(GameServerEvent.Started, this);
			GC.Collect(GC.MaxGeneration);
			log.Info("GameServer is now open for connections!");
			GameProperties.Save();
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Failed to start the server", exception);
			return false;
		}
	}

	private void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
	{
		log.Fatal("Unhandled exception!\n" + e.ExceptionObject.ToString());
		if (e.IsTerminating)
		{
			LogManager.Shutdown();
		}
	}

	protected bool InitComponent(bool componentInitState, string text)
	{
		log.Info(text + ": " + componentInitState);
		if (!componentInitState)
		{
			Stop();
		}
		return componentInitState;
	}

	public bool RecompileScripts()
	{
		string path = _config.RootDirectory + Path.DirectorySeparatorChar + "scripts";
		if (!Directory.Exists(path))
		{
			Directory.CreateDirectory(path);
		}
		string[] asm_names = _config.ScriptAssemblies.Split(',');
		return ScriptMgr.CompileScripts(compileVB: false, path, _config.ScriptCompilationTarget, asm_names);
	}

	protected bool StartScriptComponents()
	{
		try
		{
			ScriptMgr.InsertAssembly(typeof(CenterServer).Assembly);
			ScriptMgr.InsertAssembly(typeof(BaseServer).Assembly);
			Assembly[] scripts = ScriptMgr.Scripts;
			Assembly[] array = scripts;
			foreach (Assembly asm in array)
			{
				GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStartedEventAttribute), GameServerEvent.Started);
				GameEventMgr.RegisterGlobalEvents(asm, typeof(GameServerStoppedEventAttribute), GameServerEvent.Stopped);
				GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptLoadedEventAttribute), ScriptEvent.Loaded);
				GameEventMgr.RegisterGlobalEvents(asm, typeof(ScriptUnloadedEventAttribute), ScriptEvent.Unloaded);
			}
			log.Info("Registering global event handlers: true");
			return true;
		}
		catch (Exception exception)
		{
			log.Error("StartScriptComponents", exception);
			return false;
		}
	}

	public bool InitGlobalTimers()
	{
		int num = _config.SaveIntervalInterval * 60 * 1000;
		if (m_saveDBTimer == null)
		{
			m_saveDBTimer = new Timer(SaveTimerProc, null, num, num);
		}
		else
		{
			m_saveDBTimer.Change(num, num);
		}
		num = _config.SystemNoticeInterval * 60 * 1000;
		if (m_sytemNotice == null)
		{
			m_sytemNotice = new Timer(SystemNoticeTimerProc, null, num, num);
		}
		else
		{
			m_sytemNotice.Change(num, num);
		}
		num = _config.LoginLapseInterval * 60 * 1000;
		if (m_loginLapseTimer == null)
		{
			m_loginLapseTimer = new Timer(LoginLapseTimerProc, null, num, num);
		}
		else
		{
			m_loginLapseTimer.Change(num, num);
		}
		num = _config.SaveRecordInterval * 60 * 1000;
		if (m_saveRecordTimer == null)
		{
			m_saveRecordTimer = new Timer(SaveRecordProc, null, num, num);
		}
		else
		{
			m_saveRecordTimer.Change(num, num);
		}
		num = _config.ScanAuctionInterval * 60 * 1000;
		if (m_scanAuction == null)
		{
			m_scanAuction = new Timer(ScanAuctionProc, null, num, num);
		}
		else
		{
			m_scanAuction.Change(num, num);
		}
		num = _config.ScanMailInterval * 60 * 1000;
		if (m_scanMail == null)
		{
			m_scanMail = new Timer(ScanMailProc, null, num, num);
		}
		else
		{
			m_scanMail.Change(num, num);
		}
		num = _config.ScanConsortiaInterval * 60 * 1000;
		if (m_scanConsortia == null)
		{
			m_scanConsortia = new Timer(ScanConsortiaProc, null, num, num);
		}
		else
		{
			m_scanConsortia.Change(num, num);
		}
		num = 60000;
		if (m_worldEvent == null)
		{
			m_worldEvent = new Timer(ScanWorldEventProc, null, num, num);
		}
		else
		{
			m_worldEvent.Change(num, num);
		}
		num = 60000;
		if (m_consortiaboss == null)
		{
			m_consortiaboss = new Timer(ScanConsortiabossProc, null, num, num);
		}
		else
		{
			m_consortiaboss.Change(num, num);
		}
		return true;
	}

	public void DisposeGlobalTimers()
	{
		if (m_saveDBTimer != null)
		{
			m_saveDBTimer.Dispose();
		}
		if (m_loginLapseTimer != null)
		{
			m_loginLapseTimer.Dispose();
		}
		if (m_saveRecordTimer != null)
		{
			m_saveRecordTimer.Dispose();
		}
		if (m_scanAuction != null)
		{
			m_scanAuction.Dispose();
		}
		if (m_scanMail != null)
		{
			m_scanMail.Dispose();
		}
		if (m_scanConsortia != null)
		{
			m_scanConsortia.Dispose();
		}
		if (m_worldEvent != null)
		{
			m_worldEvent.Dispose();
		}
		if (m_sytemNotice != null)
		{
			m_sytemNotice.Dispose();
		}
		if (m_consortiaboss != null)
		{
			m_consortiaboss.Dispose();
		}
	}

	protected void SystemNoticeTimerProc(object state)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("System Notice ...");
				log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			List<string> notceList = WorldMgr.NotceList;
			if (notceList.Count > 0)
			{
				int index = rand.Next(notceList.Count);
				Instance.SendSystemNotice(notceList[index]);
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("System Notice complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SystemNoticeTimerProc", exception);
			}
		}
	}

	protected void SaveTimerProc(object state)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving database...");
				log.Debug("Save ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			ServerMgr.SaveToDatabase();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Saving database complete!");
				log.Info("Saved all databases " + tickCount + "ms");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SaveTimerProc", exception);
			}
		}
	}

	protected void LoginLapseTimerProc(object sender)
	{
		try
		{
			Player[] allPlayer = LoginMgr.GetAllPlayer();
			long ticks = DateTime.Now.Ticks;
			long num = (long)_config.LoginLapseInterval * 10L * 1000;
			Player[] array = allPlayer;
			foreach (Player player in array)
			{
				if (player.State == ePlayerState.NotLogin)
				{
					if (player.LastTime + num < ticks)
					{
						LoginMgr.RemovePlayer(player.Id);
					}
				}
				else
				{
					player.LastTime = ticks;
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("LoginLapseTimer callback", exception);
			}
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
				log.WarnFormat("Saved all Record  in {0} ms!", tickCount);
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

	protected void ScanAuctionProc(object sender)
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
			string noticeUserID = "";
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.ScanAuction(ref noticeUserID);
			}
			string[] array = noticeUserID.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					GSPacketIn gSPacketIn = new GSPacketIn(117);
					gSPacketIn.WriteInt(int.Parse(text));
					gSPacketIn.WriteInt(1);
					SendToALL(gSPacketIn);
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan Auction complete!");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Scan all Auction  in {0} ms", tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ScanAuctionProc", exception);
			}
		}
	}

	protected void ScanMailProc(object sender)
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
			string noticeUserID = "";
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				playerBussiness.ScanMail(ref noticeUserID);
			}
			string[] array = noticeUserID.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					GSPacketIn gSPacketIn = new GSPacketIn(117);
					gSPacketIn.WriteInt(int.Parse(text));
					gSPacketIn.WriteInt(1);
					SendToALL(gSPacketIn);
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan Mail complete!");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Scan all Mail in {0} ms", tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ScanMailProc", exception);
			}
		}
	}

	protected void ScanConsortiabossProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan Consortiaboss...");
				log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			ConsortiaBossMgr.UpdateTime();
			ConsortiaBossMgr.TimeCheckingAward++;
			if (ConsortiaBossMgr.TimeCheckingAward > 5)
			{
				List<int> allConsortiaGetAward = ConsortiaBossMgr.GetAllConsortiaGetAward();
				GSPacketIn gSPacketIn = new GSPacketIn(185);
				gSPacketIn.WriteInt(allConsortiaGetAward.Count);
				foreach (int item in allConsortiaGetAward)
				{
					gSPacketIn.WriteInt(item);
				}
				SendToALL(gSPacketIn);
				ConsortiaBossMgr.TimeCheckingAward = 0;
				log.Info("Scan Consortiaboss award complete!");
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan Consortiaboss complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ScanConsortiabossProc", exception);
			}
		}
	}

	protected void ScanWorldEventProc(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan  WorldEvent ...");
				log.Debug("Scan ThreadId=" + Thread.CurrentThread.ManagedThreadId);
			}
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			SendUpdateWorldEvent();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan WorldEvent complete!");
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Scan WorldEvent Proc", exception);
			}
		}
	}

	protected void ScanConsortiaProc(object sender)
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
			string noticeID = "";
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				consortiaBussiness.ScanConsortia(ref noticeID);
			}
			string[] array = noticeID.Split(',');
			string[] array2 = array;
			foreach (string text in array2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					GSPacketIn gSPacketIn = new GSPacketIn(128);
					gSPacketIn.WriteByte(2);
					gSPacketIn.WriteInt(int.Parse(text));
					SendToALL(gSPacketIn);
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			if (log.IsInfoEnabled)
			{
				log.Info("Scan Consortia complete!");
			}
			if (tickCount > 120000)
			{
				log.WarnFormat("Scan all Consortia in {0} ms", tickCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ScanConsortiaProc", exception);
			}
		}
	}

	public override void Stop()
	{
		DisposeGlobalTimers();
		SaveTimerProc(null);
		SaveRecordProc(null);
		CenterService.Stop();
		base.Stop();
	}

	public new ServerClient[] GetAllClients()
	{
		ServerClient[] array = null;
		lock (_clients.SyncRoot)
		{
			array = new ServerClient[_clients.Count];
			_clients.Keys.CopyTo(array, 0);
		}
		return array;
	}

	public void SendToALL(GSPacketIn pkg)
	{
		SendToALL(pkg, null);
	}

	public void SendToALL(GSPacketIn pkg, ServerClient except)
	{
		ServerClient[] allClients = GetAllClients();
		if (allClients == null)
		{
			return;
		}
		ServerClient[] array = allClients;
		foreach (ServerClient serverClient in array)
		{
			if (serverClient != except)
			{
				serverClient.SendTCP(pkg);
			}
		}
	}

	public void SendConsortiaDelete(int consortiaID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(128);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(consortiaID);
		SendToALL(gSPacketIn);
	}

	public void SendSystemNotice(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString(msg);
		SendToALL(gSPacketIn, null);
	}

	public bool SendAAS(bool state)
	{
		if (StaticFunction.UpdateConfig("Center.Service.exe.config", "AAS", state.ToString()))
		{
			ASSState = state;
			GSPacketIn gSPacketIn = new GSPacketIn(7);
			gSPacketIn.WriteBoolean(state);
			SendToALL(gSPacketIn);
			return true;
		}
		return false;
	}

	public bool SendConfigState(int type, bool state)
	{
		string empty = string.Empty;
		switch (type)
		{
		case 1:
			empty = "AAS";
			break;
		case 2:
			empty = "DailyAwardState";
			break;
		default:
			return false;
		}
		if (StaticFunction.UpdateConfig("Center.Service.exe.config", empty, state.ToString()))
		{
			switch (type)
			{
			case 1:
				ASSState = state;
				break;
			case 2:
				DailyAwardState = state;
				break;
			}
			SendConfigState();
			return true;
		}
		return false;
	}

	public bool AvailTime(DateTime startTime, int min)
	{
		int num = min - (int)(DateTime.Now - startTime).TotalMinutes;
		return num > 0;
	}

	public void SendLeagueOpenClose(bool value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87);
		gSPacketIn.WriteBoolean(value);
		SendToALL(gSPacketIn);
	}

	public void SendBattleGoundOpenClose(bool value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(88);
		gSPacketIn.WriteBoolean(value);
		SendToALL(gSPacketIn);
	}

	public void SendFightFootballTime(bool value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(89);
		gSPacketIn.WriteBoolean(value);
		SendToALL(gSPacketIn);
	}

	public void SendUpdateWorldEvent()
	{
		int hour = DateTime.Now.Hour;
		if ((hour >= 9 && hour < 10) || (hour >= 18 && hour < 19))
		{
			if (!WorldMgr.IsBattleGoundOpen)
			{
				SendBattleGoundOpenClose(value: true);
				WorldMgr.IsBattleGoundOpen = true;
				WorldMgr.BattleGoundOpenTime = DateTime.Now;
				SendSystemNotice("Đấu trường đã mở. Nhanh chóng tham gia nào các Gunner.");
			}
		}
		else if (WorldMgr.IsBattleGoundOpen)
		{
			SendBattleGoundOpenClose(value: false);
			WorldMgr.IsBattleGoundOpen = false;
			SendSystemNotice("Đấu trường hôm nay kết thúc.");
		}
		if (hour >= 20 && hour < 22)
		{
			if (!WorldMgr.IsLeagueOpen)
			{
				SendLeagueOpenClose(value: true);
				WorldMgr.IsLeagueOpen = true;
				WorldMgr.LeagueOpenTime = DateTime.Now;
				SendSystemNotice("Chiến thần đã mở. Nhanh chóng tham gia nào các Gunner.");
			}
		}
		else if (WorldMgr.IsLeagueOpen)
		{
			SendLeagueOpenClose(value: false);
			WorldMgr.IsLeagueOpen = false;
			SendSystemNotice("Chiến thần hôm nay kết thúc.");
		}
		string[] array = GameProperties.FightFootballTime.Split('|');
		if (WorldMgr.IsFightFootballTime)
		{
			int min = int.Parse(array[1]);
			if (!AvailTime(WorldMgr.FightFootballTime, min))
			{
				SendFightFootballTime(value: false);
				WorldMgr.IsFightFootballTime = false;
				SendSystemNotice("Quyết chiến hôm nay kết thúc.");
			}
		}
		else if (hour.ToString() == array[0] && !WorldMgr.IsFightFootballTime)
		{
			SendFightFootballTime(value: true);
			WorldMgr.IsFightFootballTime = true;
			WorldMgr.FightFootballTime = DateTime.Now;
			SendSystemNotice("Quyết chiến đã mở. Nhanh chóng tham gia nào các Gunner.");
		}
		if (!WorldMgr.worldOpen)
		{
			if (hour.ToString() == "12" || hour.ToString() == "18")
			{
				WorldMgr.WorldBossClearRank();
				SendRoomClose(1);
			}
			if (hour >= 13 && hour < 14)
			{
				WorldMgr.SetupWorldBoss(0);
				SendPrivateInfo();
				SendSystemNotice($"Boss thế giới {WorldMgr.name[0]} đã mở hãy tận hưởng.");
			}
			else if (hour >= 19 && hour < 20)
			{
				WorldMgr.SetupWorldBoss(3);
				SendPrivateInfo();
				SendSystemNotice($"Boss thế giới {WorldMgr.name[3]} đã mở hãy tận hưởng.");
			}
			else if ((hour >= 14 && hour < 16) || (hour >= 20 && hour < 22))
			{
				SendPrivateInfo();
			}
		}
		if (WorldMgr.worldOpen)
		{
			int minute = DateTime.Now.Minute;
			if (hour.ToString() == "14" || hour.ToString() == "20")
			{
				WorldMgr.WorldBossFightOver();
				SendWorldBossFightOver();
			}
			if ((hour.ToString() == "14" || hour.ToString() == "20") && minute == 5)
			{
				WorldMgr.WorldBossRoomClose();
				WorldMgr.WorldBossClose();
				SendRoomClose(0);
				SendUpdateRank(type: true);
			}
			if ((hour.ToString() == "14" || hour.ToString() == "20") && minute < 5)
			{
				int currentPVE_ID = WorldMgr.currentPVE_ID;
				SendSystemNotice($"Boss thế giới {WorldMgr.name[currentPVE_ID]} đã kết thúc. Phòng sẽ đóng sau {5 - minute} phút.");
			}
			WorldMgr.UpdateFightTime();
		}
		int hour2 = DateTime.Now.Hour;
		if (hour2 > awardTime && WorldMgr.CanSendLightriddleAward)
		{
			WorldMgr.SendLightriddleTopEightAward();
		}
		if (hour2 > awardTime && WorldMgr.CanSendLuckyStarAward)
		{
			WorldMgr.SendLuckyStarTopTenAward();
		}
		if (hour2 > 1 && hour2 < awardTime)
		{
			if (!WorldMgr.CanSendLightriddleAward)
			{
				WorldMgr.CanSendLightriddleAward = true;
				WorldMgr.ResetLightriddleRank();
			}
			if (!WorldMgr.CanSendLuckyStarAward)
			{
				WorldMgr.CanSendLuckyStarAward = true;
				WorldMgr.ResetLuckStar();
			}
		}
		WorldMgr.SaveLuckyStarRewardRecord();
	}

	public void SendPrivateInfo()
	{
		int currentPVE_ID = WorldMgr.currentPVE_ID;
		GSPacketIn gSPacketIn = new GSPacketIn(80);
		gSPacketIn.WriteLong(WorldMgr.MAX_BLOOD);
		gSPacketIn.WriteLong(WorldMgr.current_blood);
		gSPacketIn.WriteString(WorldMgr.name[currentPVE_ID]);
		gSPacketIn.WriteString(WorldMgr.bossResourceId[currentPVE_ID]);
		gSPacketIn.WriteInt(WorldMgr.Pve_Id[currentPVE_ID]);
		gSPacketIn.WriteBoolean(WorldMgr.fightOver);
		gSPacketIn.WriteBoolean(WorldMgr.roomClose);
		gSPacketIn.WriteDateTime(WorldMgr.begin_time);
		gSPacketIn.WriteDateTime(WorldMgr.end_time);
		gSPacketIn.WriteInt(WorldMgr.fight_time);
		gSPacketIn.WriteBoolean(WorldMgr.worldOpen);
		SendToALL(gSPacketIn);
	}

	public void SendUpdateRank(bool type)
	{
		List<RankingPersonInfo> list = WorldMgr.SelectTopTen();
		if (list.Count == 0)
		{
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(81);
		gSPacketIn.WriteBoolean(type);
		gSPacketIn.WriteInt(list.Count);
		foreach (RankingPersonInfo item in list)
		{
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteString(item.Name);
			gSPacketIn.WriteInt(item.Damage);
		}
		SendToALL(gSPacketIn);
	}

	public void SendPrivateInfo(string name)
	{
		if (WorldMgr.CheckName(name))
		{
			GSPacketIn gSPacketIn = new GSPacketIn(85);
			RankingPersonInfo singleRank = WorldMgr.GetSingleRank(name);
			gSPacketIn.WriteString(name);
			gSPacketIn.WriteInt(singleRank.Damage);
			gSPacketIn.WriteInt(singleRank.Honor);
			SendToALL(gSPacketIn);
		}
	}

	public void SendWorldBossFightOver()
	{
		GSPacketIn pkg = new GSPacketIn(82);
		SendToALL(pkg);
	}

	public void SendUpdateWorldBlood()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(79);
		gSPacketIn.WriteLong(WorldMgr.MAX_BLOOD);
		gSPacketIn.WriteLong(WorldMgr.current_blood);
		SendToALL(gSPacketIn);
	}

	public void SendRoomClose(byte type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(83);
		gSPacketIn.WriteByte(type);
		SendToALL(gSPacketIn);
	}

	public void SendConfigState()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(8);
		gSPacketIn.WriteBoolean(ASSState);
		gSPacketIn.WriteBoolean(DailyAwardState);
		SendToALL(gSPacketIn);
	}

	public int RateUpdate(int serverId)
	{
		ServerClient[] allClients = GetAllClients();
		if (allClients != null)
		{
			ServerClient[] array = allClients;
			foreach (ServerClient serverClient in array)
			{
				if (serverClient.Info.ID == serverId)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(177);
					gSPacketIn.WriteInt(serverId);
					serverClient.SendTCP(gSPacketIn);
					return 0;
				}
			}
		}
		return 1;
	}

	public int NoticeServerUpdate(int serverId, int type)
	{
		ServerClient[] allClients = GetAllClients();
		if (allClients != null)
		{
			ServerClient[] array = allClients;
			foreach (ServerClient serverClient in array)
			{
				if (serverClient.Info.ID == serverId)
				{
					GSPacketIn gSPacketIn = new GSPacketIn(11);
					gSPacketIn.WriteInt(type);
					serverClient.SendTCP(gSPacketIn);
					return 0;
				}
			}
		}
		return 1;
	}

	public bool SendReload(eReloadType type)
	{
		return SendReload(type.ToString());
	}

	public bool SendReload(string str)
	{
		try
		{
			eReloadType eReloadType2 = (eReloadType)Enum.Parse(typeof(eReloadType), str, ignoreCase: true);
			eReloadType eReloadType3 = eReloadType2;
			if (eReloadType3 == eReloadType.server)
			{
				_config.Refresh();
				InitGlobalTimers();
				LoadConfig();
				ServerMgr.ReLoadServerList();
				SendConfigState();
			}
			GSPacketIn gSPacketIn = new GSPacketIn(11);
			gSPacketIn.WriteInt((int)eReloadType2);
			SendToALL(gSPacketIn, null);
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Order is not Exist!", exception);
		}
		return false;
	}

	public void SendShutdown()
	{
		GSPacketIn pkg = new GSPacketIn(15);
		SendToALL(pkg);
	}

	public CenterServer(CenterServerConfig config)
	{
		_config = config;
		LoadConfig();
	}

	public void LoadConfig()
	{
		_aSSState = bool.Parse(ConfigurationManager.AppSettings["AAS"]);
		_dailyAwardState = bool.Parse(ConfigurationManager.AppSettings["DailyAwardState"]);
	}

	public static void CreateInstance(CenterServerConfig config)
	{
		if (Instance == null)
		{
			FileInfo fileInfo = new FileInfo(config.LogConfigFile);
			if (!fileInfo.Exists)
			{
				ResourceUtil.ExtractResource(fileInfo.Name, fileInfo.FullName, Assembly.GetAssembly(typeof(CenterServer)));
			}
			XmlConfigurator.ConfigureAndWatch(fileInfo);
			_instance = new CenterServer(config);
		}
	}
}
