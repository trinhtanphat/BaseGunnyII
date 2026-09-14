using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class ActiveSystemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	private static Dictionary<int, ActiveSystemInfo> m_activeSystem = new Dictionary<int, ActiveSystemInfo>();

	private static Dictionary<int, ActivitySystemItemInfo> m_activySystemItem = new Dictionary<int, ActivitySystemItemInfo>();

	private static Dictionary<int, LanternriddlesInfo> m_lanternriddlesInfo = new Dictionary<int, LanternriddlesInfo>();

	private static List<LuckStarRewardRecordInfo> m_recordList = new List<LuckStarRewardRecordInfo>();

	protected static Timer m_scanRank;

	protected static Timer m_statusScanTimer;

	protected static Timer m_lanternriddlesScanTimer;

	private static bool m_sendOpenToClient;

	private static bool m_sendCloseToClient;

	private static bool m_lanternriddlesOpen;

	private static int m_periodType;

	private static int m_boatCompleteExp;

	private static int m_reduceToemUpGrace;

	private static bool m_IsReset;

	private static bool m_IsSendAward;

	private static bool m_IsBattleGoundOpen;

	private static bool m_IsLeagueOpen;

	private static bool m_IsFightFootballTime;

	private static bool m_x2Exp;

	private static bool m_x3Exp;

	private static int m_luckStarCountDown;

	public static List<LuckStarRewardRecordInfo> RecordList => m_recordList;

	public static bool LanternriddlesOpen => m_lanternriddlesOpen;

	public static int periodType => m_periodType;

	public static int boatCompleteExp => m_boatCompleteExp;

	public static int ReduceToemUpGrace => m_reduceToemUpGrace;

	public static bool IsBattleGoundOpen
	{
		get
		{
			return m_IsBattleGoundOpen;
		}
		set
		{
			m_IsBattleGoundOpen = value;
		}
	}

	public static bool IsLeagueOpen
	{
		get
		{
			return m_IsLeagueOpen;
		}
		set
		{
			m_IsLeagueOpen = value;
		}
	}

	public static bool IsFightFootballTime
	{
		get
		{
			return m_IsFightFootballTime;
		}
		set
		{
			m_IsFightFootballTime = value;
		}
	}

	public static DateTime EndDate => DateTime.Now.AddMilliseconds(GameProperties.LightRiddleAnswerTime * 1000);

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			rand = new ThreadSafeRandom();
			m_IsBattleGoundOpen = false;
			m_IsLeagueOpen = false;
			m_IsFightFootballTime = false;
			m_lanternriddlesOpen = false;
			m_sendOpenToClient = true;
			m_sendCloseToClient = true;
			m_luckStarCountDown = Math.Abs(60 - DateTime.Now.Minute - 30);
			Setup();
			return LoadSystermInfo();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ActiveSystemMgr", exception);
			}
			return false;
		}
	}

	public static void AddOrUpdateLanternriddles(int playerID, LanternriddlesInfo Lanternriddles)
	{
		Lanternriddles.QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
		if (!m_lanternriddlesInfo.ContainsKey(playerID))
		{
			m_lanternriddlesInfo.Add(playerID, Lanternriddles);
		}
		else
		{
			m_lanternriddlesInfo[playerID] = Lanternriddles;
		}
	}

	public static LanternriddlesInfo EnterLanternriddles(int playerID)
	{
		if (!m_lanternriddlesInfo.ContainsKey(playerID))
		{
			LanternriddlesInfo lanternriddlesInfo = new LanternriddlesInfo();
			lanternriddlesInfo.PlayerID = playerID;
			lanternriddlesInfo.QuestionIndex = 1;
			lanternriddlesInfo.QuestionView = 30;
			lanternriddlesInfo.DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
			lanternriddlesInfo.DoublePrice = GameProperties.LightRiddleComboMoney;
			lanternriddlesInfo.HitFreeCount = GameProperties.LightRiddleFreeHitNum;
			lanternriddlesInfo.HitPrice = GameProperties.LightRiddleHitMoney;
			lanternriddlesInfo.QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
			lanternriddlesInfo.MyInteger = 0;
			lanternriddlesInfo.QuestionNum = 0;
			lanternriddlesInfo.Option = -1;
			lanternriddlesInfo.IsHint = false;
			lanternriddlesInfo.IsDouble = false;
			lanternriddlesInfo.EndDate = EndDate;
			m_lanternriddlesInfo.Add(playerID, lanternriddlesInfo);
			return lanternriddlesInfo;
		}
		return GetLanternriddlesInfo(playerID);
	}

	public static LanternriddlesInfo GetLanternriddlesInfo(int playerID)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			if (m_lanternriddlesInfo[playerID].CanNextQuest)
			{
				m_lanternriddlesInfo[playerID].EndDate = EndDate;
				if (m_lanternriddlesInfo[playerID].QuestionIndex > 1)
				{
					m_lanternriddlesInfo[playerID].QuestionIndex++;
				}
			}
			if (m_lanternriddlesInfo[playerID].EndDate.Date < DateTime.Now.Date)
			{
				m_lanternriddlesInfo[playerID].QuestionIndex = 1;
				m_lanternriddlesInfo[playerID].QuestViews = LightriddleQuestMgr.Get30LightriddleQuest();
				m_lanternriddlesInfo[playerID].DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
				m_lanternriddlesInfo[playerID].DoublePrice = GameProperties.LightRiddleComboMoney;
				m_lanternriddlesInfo[playerID].HitFreeCount = GameProperties.LightRiddleFreeHitNum;
				m_lanternriddlesInfo[playerID].HitPrice = GameProperties.LightRiddleHitMoney;
				m_lanternriddlesInfo[playerID].MyInteger = 0;
				m_lanternriddlesInfo[playerID].QuestionNum = 0;
				m_lanternriddlesInfo[playerID].Option = -1;
				m_lanternriddlesInfo[playerID].IsHint = false;
				m_lanternriddlesInfo[playerID].IsDouble = false;
				m_lanternriddlesInfo[playerID].EndDate = EndDate;
			}
			return m_lanternriddlesInfo[playerID];
		}
		return CreateNullLanternriddlesInfo(playerID);
	}

	public static LanternriddlesInfo CreateNullLanternriddlesInfo(int playerID)
	{
		LanternriddlesInfo lanternriddlesInfo = new LanternriddlesInfo();
		lanternriddlesInfo.PlayerID = playerID;
		lanternriddlesInfo.QuestionIndex = 30;
		lanternriddlesInfo.QuestionView = 30;
		lanternriddlesInfo.DoubleFreeCount = GameProperties.LightRiddleFreeComboNum;
		lanternriddlesInfo.DoublePrice = GameProperties.LightRiddleComboMoney;
		lanternriddlesInfo.HitFreeCount = GameProperties.LightRiddleFreeHitNum;
		lanternriddlesInfo.HitPrice = GameProperties.LightRiddleHitMoney;
		lanternriddlesInfo.MyInteger = 0;
		lanternriddlesInfo.QuestionNum = 0;
		lanternriddlesInfo.Option = -1;
		lanternriddlesInfo.IsHint = true;
		lanternriddlesInfo.IsDouble = true;
		lanternriddlesInfo.EndDate = DateTime.Now;
		return lanternriddlesInfo;
	}

	public static LanternriddlesInfo GetLanternriddles(int playerID)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			return m_lanternriddlesInfo[playerID];
		}
		return null;
	}

	public static void LanternriddlesAnswer(int playerID, int option)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			m_lanternriddlesInfo[playerID].Option = option;
		}
	}

	public static void SendTCP(GSPacketIn pkg, int playerID)
	{
		WorldMgr.GetPlayerById(playerID)?.SendTCP(pkg);
	}

	private static bool CanOpenLanternriddles()
	{
		Convert.ToDateTime(GameProperties.LightRiddleBeginDate);
		DateTime dateTime = Convert.ToDateTime(GameProperties.LightRiddleEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public static void UpdateLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
	{
		AddRewardRecord(PlayerID, nickName, TemplateID, Count, isVip);
		GameServer.Instance.LoginServer.SendLuckStarRewardRecord(PlayerID, nickName, TemplateID, Count, isVip);
	}

	public static void AddRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
	{
		if (m_recordList.Count > 10)
		{
			m_recordList.Clear();
		}
		LuckStarRewardRecordInfo luckStarRewardRecordInfo = new LuckStarRewardRecordInfo();
		luckStarRewardRecordInfo.PlayerID = PlayerID;
		luckStarRewardRecordInfo.nickName = nickName;
		luckStarRewardRecordInfo.useStarNum = 1;
		luckStarRewardRecordInfo.TemplateID = TemplateID;
		luckStarRewardRecordInfo.Count = Count;
		luckStarRewardRecordInfo.isVip = isVip;
		m_recordList.Add(luckStarRewardRecordInfo);
	}

	public static void UpdateIsFightFootballTime(bool open)
	{
		m_IsFightFootballTime = open;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer?.Out.SendFightFootballTimeOpenClose(gamePlayer.PlayerCharacter.ID, open);
		}
	}

	public static void UpdateIsLeagueOpen(bool open)
	{
		m_IsLeagueOpen = open;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer != null)
			{
				if (open)
				{
					gamePlayer.Out.SendLeagueNotice(gamePlayer.PlayerCharacter.ID, gamePlayer.BattleData.MatchInfo.restCount, gamePlayer.BattleData.maxCount, 1);
				}
				else
				{
					gamePlayer.Out.SendLeagueNotice(gamePlayer.PlayerCharacter.ID, gamePlayer.BattleData.MatchInfo.restCount, gamePlayer.BattleData.maxCount, 2);
				}
			}
		}
	}

	public static void UpdateIsBattleGoundOpen(bool open)
	{
		m_IsBattleGoundOpen = open;
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer?.Out.SendBattleGoundOpen(gamePlayer.PlayerCharacter.ID);
		}
	}

	public static bool CanExchange()
	{
		return DateTime.Now.DayOfWeek == DayOfWeek.Sunday && m_periodType == 2;
	}

	public static void CheckPeriod()
	{
		if (DateTime.Now.DayOfWeek == DayOfWeek.Sunday)
		{
			bool flag = false;
			if (m_IsReset)
			{
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					playerBussiness.ResetCommunalActive(1, IsReset: false);
				}
				m_IsReset = false;
				flag = LoadSystermInfo();
			}
			if (flag)
			{
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				GamePlayer[] array = allPlayers;
				for (int i = 0; i < array.Length; i++)
				{
					array[i]?.Actives.SendDragonBoatAward();
				}
			}
			int gP = CommunalActiveMgr.GetGP(6);
			if (boatCompleteExp >= gP)
			{
				m_periodType = 2;
				m_reduceToemUpGrace = 40;
			}
			m_x3Exp = false;
		}
		else if (DateTime.Now.DayOfWeek == DayOfWeek.Monday)
		{
			if (!m_IsReset)
			{
				GamePlayer[] allPlayers2 = WorldMgr.GetAllPlayers();
				GamePlayer[] array2 = allPlayers2;
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j]?.Actives.ResetDragonBoat();
				}
				bool flag2 = false;
				using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
				{
					flag2 = playerBussiness2.ResetDragonBoat();
				}
				if (flag2)
				{
					LoadSystermInfo();
					m_IsReset = true;
				}
			}
			m_periodType = 1;
			m_reduceToemUpGrace = 0;
		}
		else if (DateTime.Now.DayOfWeek == DayOfWeek.Friday && !m_x2Exp)
		{
			GamePlayer[] allPlayers3 = WorldMgr.GetAllPlayers();
			GamePlayer[] array3 = allPlayers3;
			foreach (GamePlayer gamePlayer in array3)
			{
				if (gamePlayer != null)
				{
					gamePlayer.CanX2Exp = true;
				}
			}
			m_x2Exp = true;
		}
		else
		{
			if (DateTime.Now.DayOfWeek != DayOfWeek.Saturday || m_x3Exp)
			{
				return;
			}
			GamePlayer[] allPlayers4 = WorldMgr.GetAllPlayers();
			GamePlayer[] array4 = allPlayers4;
			foreach (GamePlayer gamePlayer2 in array4)
			{
				if (gamePlayer2 != null)
				{
					gamePlayer2.CanX3Exp = true;
				}
			}
			m_x3Exp = true;
			m_x2Exp = false;
		}
	}

	public static bool CanX2Exp()
	{
		int gP = CommunalActiveMgr.GetGP(5);
		return DateTime.Now.DayOfWeek == DayOfWeek.Friday && boatCompleteExp >= gP;
	}

	public static bool CanX3Exp()
	{
		int gP = CommunalActiveMgr.GetGP(6);
		return DateTime.Now.DayOfWeek == DayOfWeek.Saturday && boatCompleteExp >= gP;
	}

	public static void UpdateBoatExpFromDB(int exp)
	{
		m_boatCompleteExp = 0;
		m_boatCompleteExp += exp;
	}

	public static void UpdateBoatExp(int exp)
	{
		m_boatCompleteExp += exp;
	}

	public static int FindMyRank(int ID)
	{
		int dragonBoatMinScore = GameProperties.DragonBoatMinScore;
		if (m_activeSystem.ContainsKey(ID) && m_activeSystem[ID].totalScore >= dragonBoatMinScore)
		{
			return m_activeSystem[ID].myRank;
		}
		return -1;
	}

	public static int FindAreaMyRank(int ID)
	{
		int dragonBoatAreaMinScore = GameProperties.DragonBoatAreaMinScore;
		if (m_activeSystem.ContainsKey(ID) && m_activeSystem[ID].totalScore >= dragonBoatAreaMinScore)
		{
			return m_activeSystem[ID].myRank;
		}
		return -1;
	}

	public static List<ActiveSystemInfo> SelectTopTenCurrenServer(int condition)
	{
		List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
		IOrderedEnumerable<KeyValuePair<int, ActiveSystemInfo>> orderedEnumerable = from pair in m_activeSystem
			where pair.Value.totalScore >= condition
			orderby pair.Value.totalScore descending
			select pair;
		foreach (KeyValuePair<int, ActiveSystemInfo> item in orderedEnumerable)
		{
			if (list.Count == 10)
			{
				break;
			}
			list.Add(item.Value);
		}
		return list;
	}

	public static List<ActiveSystemInfo> SelectTopTenAllServer(int condition)
	{
		List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
		IOrderedEnumerable<KeyValuePair<int, ActiveSystemInfo>> orderedEnumerable = from pair in m_activeSystem
			where pair.Value.totalScore >= condition
			orderby pair.Value.totalScore descending
			select pair;
		foreach (KeyValuePair<int, ActiveSystemInfo> item in orderedEnumerable)
		{
			if (list.Count == 10)
			{
				break;
			}
			list.Add(item.Value);
		}
		return list;
	}

	public static void Setup()
	{
		try
		{
			m_periodType = 1;
			m_boatCompleteExp = 0;
			m_reduceToemUpGrace = 0;
			CommunalActiveInfo communalActiveInfo = CommunalActiveMgr.FindCommunalActive(1);
			if (communalActiveInfo != null)
			{
				m_IsSendAward = communalActiveInfo.IsSendAward;
				m_IsReset = communalActiveInfo.IsReset;
			}
			else
			{
				m_IsSendAward = true;
				m_IsReset = true;
			}
			CheckPeriod();
			BeginTimer();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ActiveSystemMgr Setup", exception);
			}
		}
	}

	private static bool LoadSystermInfo()
	{
		try
		{
			m_activeSystem = new Dictionary<int, ActiveSystemInfo>();
			m_activySystemItem = new Dictionary<int, ActivitySystemItemInfo>();
			long num = 0L;
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			ActiveSystemInfo[] allActiveSystemData = playerBussiness.GetAllActiveSystemData();
			ActiveSystemInfo[] array = allActiveSystemData;
			foreach (ActiveSystemInfo activeSystemInfo in array)
			{
				if (!m_activeSystem.ContainsKey(activeSystemInfo.UserID))
				{
					num += activeSystemInfo.totalScore;
					m_activeSystem.Add(activeSystemInfo.UserID, activeSystemInfo);
				}
			}
			ActivitySystemItemInfo[] allActivitySystemItem = playerBussiness.GetAllActivitySystemItem();
			ActivitySystemItemInfo[] array2 = allActivitySystemItem;
			foreach (ActivitySystemItemInfo activitySystemItemInfo in array2)
			{
				if (!m_activySystemItem.ContainsKey(activitySystemItemInfo.ID))
				{
					m_activySystemItem.Add(activitySystemItemInfo.ID, activitySystemItemInfo);
				}
			}
			if (num > int.MaxValue)
			{
				num = 2147483647L;
			}
			UpdateBoatExpFromDB((int)num);
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ActiveSystemMgr", exception);
			}
		}
		return false;
	}

	public static List<ItemInfo> GetPyramidAward(int layer)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<ActivitySystemItemInfo> list2 = new List<ActivitySystemItemInfo>();
		List<ActivitySystemItemInfo> source = FindActivitySystemItemByLayer(layer);
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(source.Select((ActivitySystemItemInfo s) => s.Random).Max());
		List<ActivitySystemItemInfo> list3 = source.Where((ActivitySystemItemInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				ActivitySystemItemInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (ActivitySystemItemInfo item2 in list2)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateID), item2.Count, 102);
			itemInfo.TemplateID = item2.TemplateID;
			itemInfo.IsBinds = item2.IsBinds;
			itemInfo.ValidDate = item2.ValidDate;
			itemInfo.Count = item2.Count;
			itemInfo.StrengthenLevel = item2.StrengthenLevel;
			itemInfo.AttackCompose = 0;
			itemInfo.DefendCompose = 0;
			itemInfo.AgilityCompose = 0;
			itemInfo.LuckCompose = 0;
			list.Add(itemInfo);
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindActivitySystemItemByLayer(int layer)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		if (m_activySystemItem != null)
		{
			foreach (ActivitySystemItemInfo value in m_activySystemItem.Values)
			{
				if (value.Quality == layer && value.ActivityType == 8)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public static List<ActivitySystemItemInfo> FindGrowthPackage(int layer)
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		if (m_activySystemItem != null)
		{
			foreach (ActivitySystemItemInfo value in m_activySystemItem.Values)
			{
				if (value.Quality == layer && value.ActivityType == 20)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}

	public static void BeginTimer()
	{
		int num = 1800000;
		if (m_scanRank == null)
		{
			m_scanRank = new Timer(TimeCheck, null, num, num);
		}
		else
		{
			m_scanRank.Change(num, num);
		}
		num = 60000;
		if (m_statusScanTimer == null)
		{
			m_statusScanTimer = new Timer(StatusScan, null, num, num);
		}
		else
		{
			m_statusScanTimer.Change(num, num);
		}
		num = 60000;
		if (m_lanternriddlesScanTimer == null)
		{
			m_lanternriddlesScanTimer = new Timer(LanternriddlesScan, null, num, num);
		}
		else
		{
			m_lanternriddlesScanTimer.Change(num, num);
		}
	}

	protected static void LanternriddlesScan(object sender)
	{
		try
		{
			log.Info("Begin Lanternriddles CheckPeriod....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			if (CanOpenLanternriddles())
			{
				int hour = DateTime.Now.Hour;
				DateTime dateTime = Convert.ToDateTime(GameProperties.LightRiddleBeginTime);
				DateTime dateTime2 = Convert.ToDateTime(GameProperties.LightRiddleEndTime);
				int hour2 = dateTime.Hour;
				int hour3 = dateTime2.Hour;
				if (hour >= hour2 && hour < hour3)
				{
					m_lanternriddlesOpen = true;
					if (m_sendOpenToClient)
					{
						LanternriddlesOpenClose();
						m_sendOpenToClient = false;
					}
				}
				else
				{
					m_lanternriddlesOpen = false;
					if (hour >= hour3 && m_sendCloseToClient)
					{
						LanternriddlesOpenClose();
						m_sendCloseToClient = false;
					}
				}
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End Lanternriddles CheckPeriod....");
		}
		catch (Exception exception)
		{
			log.Error("lanternriddlesScan ", exception);
		}
	}

	public static void LanternriddlesOpenClose()
	{
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer?.Out.SendLanternriddlesOpen(gamePlayer.PlayerId, m_lanternriddlesOpen);
		}
	}

	protected static void StatusScan(object sender)
	{
		try
		{
			log.Info("Begin ActiveSystem CheckPeriod....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			CheckPeriod();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End ActiveSystem CheckPeriod....");
		}
		catch (Exception exception)
		{
			log.Error("StatusScan ", exception);
		}
	}

	protected static void TimeCheck(object sender)
	{
		try
		{
			log.Info("Begin ActiveSystem TimeCheck....");
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			LoadSystermInfo();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
			log.Info("End ActiveSystem TimeCheck....");
		}
		catch (Exception exception)
		{
			log.Error("StatusScan ", exception);
		}
	}

	public static void StopAllTimer()
	{
		if (m_scanRank != null)
		{
			m_scanRank.Dispose();
			m_scanRank = null;
		}
		if (m_lanternriddlesScanTimer != null)
		{
			m_lanternriddlesScanTimer.Dispose();
			m_lanternriddlesScanTimer = null;
		}
		if (m_statusScanTimer != null)
		{
			m_statusScanTimer.Dispose();
			m_statusScanTimer = null;
		}
	}
}
