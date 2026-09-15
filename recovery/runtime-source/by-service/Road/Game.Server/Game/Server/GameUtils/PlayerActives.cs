using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public class PlayerActives
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected Timer _christmasTimer;

	protected Timer _labyrinthTimer;

	protected Timer _lightriddleTimer;

	protected GamePlayer m_player;

	private PyramidConfigInfo m_pyramidConfig;

	private PyramidInfo m_pyramid;

	private UserChristmasInfo m_christmas;

	private ActiveSystemInfo m_activeInfo;

	private NewChickenBoxItemInfo[] m_ChickenBoxRewards;

	private List<NewChickenBoxItemInfo> m_RemoveChickenBoxRewards;

	private int m_flushPrice;

	private int[] m_eagleEyePrice;

	private int[] m_openCardPrice;

	private int m_freeFlushTime;

	private ThreadSafeRandom rand = new ThreadSafeRandom();

	private bool m_saveToDb;

	private readonly int defaultCoins = 1000;

	private readonly int flushCoins = 15;

	private readonly int ChikenBoxCount = 18;

	private readonly int LuckyStartBoxCount = 14;

	public readonly int coinTemplateID = 201193;

	private int m_labyrinthCountDown = GameProperties.WarriorFamRaidTimeRemain;

	private int m_freeRefreshBoxCount;

	private int m_freeEyeCount;

	private int m_freeOpenCardCount;

	private DateTime m_luckyBegindate;

	private DateTime m_luckyEnddate;

	private int m_minUseNum;

	private NewChickenBoxItemInfo[] m_LuckyStartRewards;

	private NewChickenBoxItemInfo m_award;

	private int _lightriddleColdown = 15;

	public GamePlayer Player => m_player;

	public PyramidConfigInfo PyramidConfig
	{
		get
		{
			return m_pyramidConfig;
		}
		set
		{
			m_pyramidConfig = value;
		}
	}

	public PyramidInfo Pyramid
	{
		get
		{
			return m_pyramid;
		}
		set
		{
			m_pyramid = value;
		}
	}

	public UserChristmasInfo Christmas
	{
		get
		{
			return m_christmas;
		}
		set
		{
			m_christmas = value;
		}
	}

	public ActiveSystemInfo Info
	{
		get
		{
			return m_activeInfo;
		}
		set
		{
			m_activeInfo = value;
		}
	}

	public NewChickenBoxItemInfo[] ChickenBoxRewards
	{
		get
		{
			return m_ChickenBoxRewards;
		}
		set
		{
			m_ChickenBoxRewards = value;
		}
	}

	public int flushPrice
	{
		get
		{
			return m_flushPrice;
		}
		set
		{
			m_flushPrice = value;
		}
	}

	public int[] eagleEyePrice
	{
		get
		{
			return m_eagleEyePrice;
		}
		set
		{
			m_eagleEyePrice = value;
		}
	}

	public int[] openCardPrice
	{
		get
		{
			return m_openCardPrice;
		}
		set
		{
			m_openCardPrice = value;
		}
	}

	public int freeFlushTime
	{
		get
		{
			return m_freeFlushTime;
		}
		set
		{
			m_freeFlushTime = value;
		}
	}

	public int freeRefreshBoxCount
	{
		get
		{
			return m_freeRefreshBoxCount;
		}
		set
		{
			m_freeRefreshBoxCount = value;
		}
	}

	public int freeEyeCount
	{
		get
		{
			return m_freeEyeCount;
		}
		set
		{
			m_freeEyeCount = value;
		}
	}

	public int freeOpenCardCount
	{
		get
		{
			return m_freeOpenCardCount;
		}
		set
		{
			m_freeOpenCardCount = value;
		}
	}

	public DateTime LuckyBegindate
	{
		get
		{
			return m_luckyBegindate;
		}
		set
		{
			m_luckyBegindate = value;
		}
	}

	public DateTime LuckyEnddate
	{
		get
		{
			return m_luckyEnddate;
		}
		set
		{
			m_luckyEnddate = value;
		}
	}

	public int minUseNum
	{
		get
		{
			return m_minUseNum;
		}
		set
		{
			m_minUseNum = value;
		}
	}

	public NewChickenBoxItemInfo Award
	{
		get
		{
			return m_award;
		}
		set
		{
			m_award = value;
		}
	}

	public PlayerActives(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_eagleEyePrice = GameProperties.ConvertStringArrayToIntArray("NewChickenEagleEyePrice");
		m_openCardPrice = GameProperties.ConvertStringArrayToIntArray("NewChickenOpenCardPrice");
		m_flushPrice = GameProperties.NewChickenFlushPrice;
		m_freeFlushTime = 120;
		m_RemoveChickenBoxRewards = new List<NewChickenBoxItemInfo>();
		m_freeEyeCount = 0;
		m_freeOpenCardCount = 0;
		m_freeRefreshBoxCount = 0;
		SetupPyramidConfig();
		SetupLuckyStart();
	}

	private void SetupPyramidConfig()
	{
		lock (m_lock)
		{
			m_pyramidConfig = new PyramidConfigInfo();
			m_pyramidConfig.isOpen = IsPyramidOpen();
			m_pyramidConfig.isScoreExchange = !IsPyramidOpen();
			m_pyramidConfig.beginTime = Convert.ToDateTime(GameProperties.PyramidBeginTime);
			m_pyramidConfig.endTime = Convert.ToDateTime(GameProperties.PyramidEndTime);
			m_pyramidConfig.freeCount = 3;
			m_pyramidConfig.revivePrice = GameProperties.ConvertStringArrayToIntArray("PyramidRevivePrice");
			m_pyramidConfig.turnCardPrice = GameProperties.PyramydTurnCardPrice;
		}
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		if (IsChristmasOpen())
		{
			m_christmas = playerBussiness.GetSingleUserChristmas(Player.PlayerCharacter.ID);
			if (m_christmas == null)
			{
				CreateChristmasInfo(Player.PlayerCharacter.ID);
			}
		}
		m_activeInfo = playerBussiness.GetSingleActiveSystem(Player.PlayerCharacter.ID);
		if (m_activeInfo == null)
		{
			CreateActiveSystemInfo(Player.PlayerCharacter.ID, Player.PlayerCharacter.NickName);
		}
	}

	public bool IsChristmasOpen()
	{
		Convert.ToDateTime(GameProperties.ChristmasBeginDate);
		DateTime dateTime = Convert.ToDateTime(GameProperties.ChristmasEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsChickenBoxOpen()
	{
		Convert.ToDateTime(GameProperties.NewChickenBeginTime);
		DateTime dateTime = Convert.ToDateTime(GameProperties.NewChickenEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsLuckStarActivityOpen()
	{
		Convert.ToDateTime(GameProperties.LuckStarActivityBeginDate);
		DateTime dateTime = Convert.ToDateTime(GameProperties.LuckStarActivityEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsPyramidOpen()
	{
		Convert.ToDateTime(GameProperties.PyramidBeginTime);
		DateTime dateTime = Convert.ToDateTime(GameProperties.PyramidEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsDiceOpen()
	{
		Convert.ToDateTime(GameProperties.DiceBeginTime);
		DateTime dateTime = Convert.ToDateTime(GameProperties.DiceEndTime);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsDragonBoatOpen()
	{
		Convert.ToDateTime(GameProperties.DragonBoatBeginDate);
		DateTime dateTime = Convert.ToDateTime(GameProperties.DragonBoatEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public bool IsYearMonsterOpen()
	{
		Convert.ToDateTime(GameProperties.YearMonsterBeginDate);
		DateTime dateTime = Convert.ToDateTime(GameProperties.YearMonsterEndDate);
		return DateTime.Now.Date < dateTime.Date;
	}

	public NewChickenBoxItemInfo GetAward(int pos)
	{
		NewChickenBoxItemInfo[] chickenBoxRewards = m_ChickenBoxRewards;
		foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
		{
			if (newChickenBoxItemInfo.Position == pos && !newChickenBoxItemInfo.IsSelected)
			{
				return newChickenBoxItemInfo;
			}
		}
		return null;
	}

	public NewChickenBoxItemInfo ViewAward(int pos)
	{
		NewChickenBoxItemInfo[] chickenBoxRewards = m_ChickenBoxRewards;
		foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
		{
			if (newChickenBoxItemInfo.Position == pos && !newChickenBoxItemInfo.IsSeeded)
			{
				return newChickenBoxItemInfo;
			}
		}
		return null;
	}

	public bool UpdateChickenBoxAward(NewChickenBoxItemInfo box)
	{
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			if (m_ChickenBoxRewards[i].Position == box.Position)
			{
				m_ChickenBoxRewards[i] = box;
				return true;
			}
		}
		return false;
	}

	public void LoadChickenBox()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		m_ChickenBoxRewards = playerBussiness.GetSingleNewChickenBox(Player.PlayerCharacter.ID);
		if (m_ChickenBoxRewards.Length == 0)
		{
			PayFlushView();
		}
	}

	public void EnterChickenBox()
	{
		if (m_ChickenBoxRewards == null)
		{
			LoadChickenBox();
		}
	}

	public void RandomPosition()
	{
		List<int> list = new List<int>();
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			list.Add(m_ChickenBoxRewards[i].Position);
		}
		rand.Shuffer(m_ChickenBoxRewards);
		for (int j = 0; j < list.Count; j++)
		{
			m_ChickenBoxRewards[j].Position = list[j];
		}
	}

	public void PayFlushView()
	{
		Info.lastFlushTime = DateTime.Now;
		Info.isShowAll = true;
		Info.canOpenCounts = 5;
		Info.canEagleEyeCounts = 3;
		RemoveChickenBoxRewards();
		m_ChickenBoxRewards = TreasureAwardMgr.CreateChickenBoxAward(ChikenBoxCount);
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			m_ChickenBoxRewards[i].UserID = Player.PlayerCharacter.ID;
		}
	}

	public void RemoveChickenBoxRewards()
	{
		for (int i = 0; i < m_ChickenBoxRewards.Length; i++)
		{
			NewChickenBoxItemInfo newChickenBoxItemInfo = m_ChickenBoxRewards[i];
			if (newChickenBoxItemInfo != null && newChickenBoxItemInfo.ID > 0)
			{
				newChickenBoxItemInfo.Position = -1;
				m_RemoveChickenBoxRewards.Add(newChickenBoxItemInfo);
			}
		}
	}

	public bool IsFreeFlushTime()
	{
		DateTime lastFlushTime = Info.lastFlushTime;
		DateTime dateTime = lastFlushTime.AddMinutes(freeFlushTime);
		TimeSpan timeSpan = DateTime.Now - Info.lastFlushTime;
		double num = (dateTime - lastFlushTime).TotalMinutes - timeSpan.TotalMinutes;
		return num > 0.0;
	}

	public bool LoadPyramid()
	{
		if (m_pyramid == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			m_pyramid = playerBussiness.GetSinglePyramid(Player.PlayerCharacter.ID);
			if (m_pyramid == null)
			{
				CreatePyramidInfo();
			}
		}
		return true;
	}

	public void CreatePyramidInfo()
	{
		lock (m_lock)
		{
			m_pyramid = new PyramidInfo();
			m_pyramid.ID = 0;
			m_pyramid.UserID = Player.PlayerCharacter.ID;
			m_pyramid.currentLayer = 1;
			m_pyramid.maxLayer = 1;
			m_pyramid.totalPoint = 0;
			m_pyramid.turnPoint = 0;
			m_pyramid.pointRatio = 0;
			m_pyramid.currentFreeCount = 0;
			m_pyramid.currentReviveCount = 0;
			m_pyramid.isPyramidStart = false;
			m_pyramid.LayerItems = "";
		}
	}

	public void ResetChristmas()
	{
		lock (m_lock)
		{
			if (m_christmas != null)
			{
				m_christmas.dayPacks = 0;
				m_christmas.AvailTime = 0;
				m_christmas.isEnter = false;
				m_activeInfo.dayScore = 0;
			}
		}
	}

	public void ResetDragonBoat()
	{
		lock (m_lock)
		{
			m_activeInfo.useableScore = 0;
			m_activeInfo.totalScore = 0;
			m_activeInfo.dayScore = 0;
			m_activeInfo.CanGetGift = true;
		}
	}

	public void SendDragonBoatAward()
	{
		if (DateTime.Now.DayOfWeek != DayOfWeek.Sunday || !IsDragonBoatOpen())
		{
			return;
		}
		int dragonBoatMinScore = GameProperties.DragonBoatMinScore;
		int dragonBoatAreaMinScore = GameProperties.DragonBoatAreaMinScore;
		List<ActiveSystemInfo> list = ActiveSystemMgr.SelectTopTenCurrenServer(dragonBoatMinScore);
		int num = 0;
		List<ItemInfo> list2 = new List<ItemInfo>();
		string text = "Phần thưởng Thuyền rồng hạng {0}";
		foreach (ActiveSystemInfo item in list)
		{
			if (item.UserID == Player.PlayerCharacter.ID && m_activeInfo.CanGetGift)
			{
				int myRank = item.myRank;
				if (myRank <= 10)
				{
					text = string.Format(text, myRank);
					list2 = CommunalActiveMgr.GetAwardInfos(1, myRank);
					WorldEventMgr.SendItemsToMail(list2, item.UserID, Player.PlayerCharacter.NickName, text);
					num++;
				}
				break;
			}
		}
		list = ActiveSystemMgr.SelectTopTenAllServer(dragonBoatAreaMinScore);
		foreach (ActiveSystemInfo item2 in list)
		{
			if (item2.UserID == Player.PlayerCharacter.ID && m_activeInfo.CanGetGift)
			{
				int myRank = item2.myRank;
				if (myRank <= 10)
				{
					text = string.Format(text, myRank);
					text += " liên server";
					list2 = CommunalActiveMgr.GetAwardInfos(2, myRank);
					WorldEventMgr.SendItemsToMail(list2, item2.UserID, Player.PlayerCharacter.NickName, text);
					num++;
				}
				break;
			}
		}
		if (num > 0)
		{
			m_activeInfo.CanGetGift = false;
		}
	}

	public void BeginChristmasTimer()
	{
		int num = 60000;
		if (_christmasTimer == null)
		{
			_christmasTimer = new Timer(ChristmasTimeCheck, null, num, num);
		}
		else
		{
			_christmasTimer.Change(num, num);
		}
	}

	protected void ChristmasTimeCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateChristmasTime();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("ChristmasTimeCheck: " + ex);
		}
	}

	public void StopChristmasTimer()
	{
		if (_christmasTimer != null)
		{
			_christmasTimer.Dispose();
			_christmasTimer = null;
		}
	}

	public void UpdateChristmasTime()
	{
		DateTime gameBeginTime = Christmas.gameBeginTime;
		DateTime gameEndTime = Christmas.gameEndTime;
		TimeSpan timeSpan = DateTime.Now - gameBeginTime;
		double num = (gameEndTime - gameBeginTime).TotalMinutes - timeSpan.TotalMinutes;
		lock (m_christmas)
		{
			m_christmas.AvailTime = (((int)num >= 0) ? ((int)num) : 0);
		}
	}

	public void AddTime(int min)
	{
		lock (m_christmas)
		{
			m_christmas.AvailTime += min;
		}
	}

	private void BeginLabyrinthTimer()
	{
		int num = 1000;
		if (_labyrinthTimer == null)
		{
			_labyrinthTimer = new Timer(LabyrinthCheck, null, num, num);
		}
		else
		{
			_labyrinthTimer.Change(num, num);
		}
	}

	protected void LabyrinthCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			UpdateLabyrinthTime();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("LabyrinthCheck: " + ex);
		}
	}

	public void StopLabyrinthTimer()
	{
		if (_labyrinthTimer != null)
		{
			_labyrinthTimer.Dispose();
			_labyrinthTimer = null;
		}
	}

	public void UpdateLabyrinthTime()
	{
		UserLabyrinthInfo labyrinth = Player.Labyrinth;
		labyrinth.isCleanOut = true;
		labyrinth.isInGame = true;
		if (labyrinth.remainTime > 0 && labyrinth.currentRemainTime > 0)
		{
			labyrinth.remainTime--;
			labyrinth.currentRemainTime--;
			m_labyrinthCountDown--;
		}
		if (m_labyrinthCountDown == 0)
		{
			GetLabyrinthAward();
			m_labyrinthCountDown = 120;
			labyrinth.currentFloor++;
			if (labyrinth.currentFloor > labyrinth.myProgress)
			{
				labyrinth.currentFloor = labyrinth.myProgress;
				StopLabyrinthTimer();
			}
		}
		Player.Out.SendLabyrinthUpdataInfo(Player.PlayerId, labyrinth);
	}

	public void CleantOutLabyrinth()
	{
		BeginLabyrinthTimer();
	}

	private void GetLabyrinthAward()
	{
		int currentFloor = m_player.Labyrinth.currentFloor;
		currentFloor--;
		int[] array = m_player.CreateExps();
		int num = array[currentFloor];
		string text = m_player.labyrinthGolds[currentFloor];
		int num2 = int.Parse(text.Split('|')[0]);
		int num3 = int.Parse(text.Split('|')[1]);
		ItemInfo itemByTemplateID = m_player.PropBag.GetItemByTemplateID(0, 11916);
		if (itemByTemplateID == null || !m_player.RemoveTemplate(11916, 1))
		{
			m_player.Labyrinth.isDoubleAward = false;
		}
		if (m_player.Labyrinth.isDoubleAward)
		{
			int num4 = 2;
			num *= num4;
			num2 *= num4;
			num3 *= num4;
		}
		m_player.Labyrinth.accumulateExp += num;
		List<ItemInfo> list = new List<ItemInfo>();
		if (CanGetBigAward())
		{
			list = m_player.CopyDrop(2, 40002);
			m_player.AddTemplate(list, num2, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
			m_player.AddHardCurrency(num3);
		}
		m_player.AddGP(num);
		PlusCleantOutInfo(m_player.Labyrinth.currentFloor, num, num3, list);
	}

	private bool CanGetBigAward()
	{
		bool result = false;
		for (int i = 0; i <= m_player.Labyrinth.myProgress; i += 2)
		{
			if (i == m_player.Labyrinth.currentFloor)
			{
				result = true;
				break;
			}
		}
		return result;
	}

	private void PlusCleantOutInfo(int FamRaidLevel, int exp, int HardCurrency, List<ItemInfo> lists)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(131, m_player.PlayerId);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(FamRaidLevel);
		gSPacketIn.WriteInt(exp);
		gSPacketIn.WriteInt(lists.Count);
		foreach (ItemInfo list in lists)
		{
			gSPacketIn.WriteInt(list.TemplateID);
			gSPacketIn.WriteInt(list.Count);
		}
		gSPacketIn.WriteInt(HardCurrency);
		m_player.SendTCP(gSPacketIn);
	}

	public void StopCleantOutLabyrinth()
	{
		UserLabyrinthInfo labyrinth = Player.Labyrinth;
		labyrinth.isCleanOut = false;
		Player.Out.SendLabyrinthUpdataInfo(Player.PlayerId, labyrinth);
		StopLabyrinthTimer();
	}

	public void SpeededUpCleantOutLabyrinth()
	{
		UserLabyrinthInfo labyrinth = Player.Labyrinth;
		labyrinth.isCleanOut = false;
		labyrinth.isInGame = false;
		labyrinth.completeChallenge = false;
		labyrinth.remainTime = 0;
		labyrinth.currentRemainTime = 0;
		labyrinth.cleanOutAllTime = 0;
		for (int i = labyrinth.currentFloor; i <= labyrinth.myProgress; i++)
		{
			GetLabyrinthAward();
			labyrinth.currentFloor++;
		}
		labyrinth.currentFloor = labyrinth.myProgress;
		Player.Out.SendLabyrinthUpdataInfo(Player.PlayerId, labyrinth);
		StopLabyrinthTimer();
	}

	public bool AvailTime()
	{
		DateTime gameBeginTime = Christmas.gameBeginTime;
		DateTime gameEndTime = Christmas.gameEndTime;
		TimeSpan timeSpan = DateTime.Now - gameBeginTime;
		double num = (gameEndTime - gameBeginTime).TotalMinutes - timeSpan.TotalMinutes;
		return num > 0.0;
	}

	public void CreateActiveSystemInfo(int UserID, string name)
	{
		lock (m_lock)
		{
			m_activeInfo = new ActiveSystemInfo();
			m_activeInfo.ID = 0;
			m_activeInfo.UserID = UserID;
			m_activeInfo.useableScore = 0;
			m_activeInfo.totalScore = 0;
			m_activeInfo.AvailTime = 0;
			m_activeInfo.NickName = name;
			m_activeInfo.dayScore = 0;
			m_activeInfo.CanGetGift = true;
			m_activeInfo.canOpenCounts = 5;
			m_activeInfo.canEagleEyeCounts = 3;
			m_activeInfo.lastFlushTime = DateTime.Now;
			m_activeInfo.isShowAll = true;
			m_activeInfo.ActiveMoney = 0;
			m_activeInfo.activityTanabataNum = 0;
			m_activeInfo.LuckystarCoins = defaultCoins;
			m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
			m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
			m_activeInfo.lastEnterYearMonter = DateTime.Now;
			m_activeInfo.DamageNum = 0;
			CreateYearMonterBoxState();
		}
	}

	public void YearMonterValidate()
	{
		lock (m_lock)
		{
			if (m_activeInfo.lastEnterYearMonter.Date < DateTime.Now.Date)
			{
				m_activeInfo.ChallengeNum = GameProperties.YearMonsterFightNum;
				m_activeInfo.BuyBuffNum = GameProperties.YearMonsterFightNum;
				m_activeInfo.lastEnterYearMonter = DateTime.Now;
				m_activeInfo.DamageNum = 0;
				CreateYearMonterBoxState();
			}
		}
	}

	public void CreateYearMonterBoxState()
	{
		string[] array = GameProperties.YearMonsterBoxInfo.Split('|');
		int num = array.Length;
		string[] array2 = new string[num];
		for (int i = 0; i < num; i++)
		{
			int num2 = int.Parse(array[i].Split(',')[1]) * 10000;
			if (num2 <= m_activeInfo.DamageNum)
			{
				array2[i] = "2";
			}
			else
			{
				array2[i] = "1";
			}
		}
		m_activeInfo.BoxState = string.Join("-", array2);
	}

	public void SetYearMonterBoxState(int id)
	{
		string[] array = m_activeInfo.BoxState.Split('-');
		int num = array.Length;
		string[] array2 = new string[num];
		for (int i = 0; i < num; i++)
		{
			if (i == id)
			{
				array2[i] = "3";
			}
			else
			{
				array2[i] = array[i];
			}
		}
		m_activeInfo.BoxState = string.Join("-", array2);
	}

	public void CreateChristmasInfo(int UserID)
	{
		lock (m_lock)
		{
			m_christmas = new UserChristmasInfo();
			m_christmas.ID = 0;
			m_christmas.UserID = UserID;
			m_christmas.count = 0;
			m_christmas.exp = 0;
			m_christmas.awardState = 0;
			m_christmas.lastPacks = 1100;
			m_christmas.packsNumber = -1;
			m_christmas.gameBeginTime = DateTime.Now;
			m_christmas.gameEndTime = DateTime.Now.AddMinutes(60.0);
			m_christmas.isEnter = false;
			m_christmas.dayPacks = 0;
			m_christmas.AvailTime = 0;
		}
	}

	public virtual void SaveToDatabase()
	{
		if (m_saveToDb)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			lock (m_lock)
			{
				if (m_pyramid != null && m_pyramid.IsDirty)
				{
					if (m_pyramid.ID > 0)
					{
						playerBussiness.UpdatePyramid(m_pyramid);
					}
					else
					{
						playerBussiness.AddPyramid(m_pyramid);
					}
				}
				if (m_christmas != null && m_christmas.IsDirty)
				{
					if (m_christmas.ID > 0)
					{
						playerBussiness.UpdateUserChristmas(m_christmas);
					}
					else
					{
						playerBussiness.AddUserChristmas(m_christmas);
					}
				}
				if (m_activeInfo != null && m_activeInfo.IsDirty)
				{
					if (m_activeInfo.ID > 0)
					{
						playerBussiness.UpdateActiveSystem(m_activeInfo);
					}
					else
					{
						playerBussiness.AddActiveSystem(m_activeInfo);
					}
				}
				if (m_ChickenBoxRewards != null)
				{
					NewChickenBoxItemInfo[] chickenBoxRewards = m_ChickenBoxRewards;
					foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
					{
						if (newChickenBoxItemInfo != null && newChickenBoxItemInfo.IsDirty)
						{
							if (newChickenBoxItemInfo.ID > 0)
							{
								playerBussiness.UpdateNewChickenBox(newChickenBoxItemInfo);
							}
							else
							{
								playerBussiness.AddNewChickenBox(newChickenBoxItemInfo);
							}
						}
					}
				}
				if (m_RemoveChickenBoxRewards.Count > 0)
				{
					foreach (NewChickenBoxItemInfo removeChickenBoxReward in m_RemoveChickenBoxRewards)
					{
						playerBussiness.UpdateNewChickenBox(removeChickenBoxReward);
					}
				}
			}
		}
		LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(m_player.PlayerCharacter.ID);
		GameServer.Instance.LoginServer.SendLightriddleInfo(lanternriddles);
	}

	public void SendChickenBoxItemList()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteDateTime(Info.lastFlushTime);
		gSPacketIn.WriteInt(freeFlushTime);
		gSPacketIn.WriteInt(freeRefreshBoxCount);
		gSPacketIn.WriteInt(freeEyeCount);
		gSPacketIn.WriteInt(freeOpenCardCount);
		gSPacketIn.WriteBoolean(Info.isShowAll);
		gSPacketIn.WriteInt(ChickenBoxRewards.Length);
		NewChickenBoxItemInfo[] chickenBoxRewards = ChickenBoxRewards;
		foreach (NewChickenBoxItemInfo newChickenBoxItemInfo in chickenBoxRewards)
		{
			gSPacketIn.WriteInt(newChickenBoxItemInfo.TemplateID);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.StrengthenLevel);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.Count);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.ValidDate);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.AttackCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.DefendCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.AgilityCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.LuckCompose);
			gSPacketIn.WriteInt(newChickenBoxItemInfo.Position);
			gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSelected);
			gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsSeeded);
			gSPacketIn.WriteBoolean(newChickenBoxItemInfo.IsBinds);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendEvent()
	{
		if (IsChickenBoxOpen())
		{
			m_player.Out.SendChickenBoxOpen(m_player.PlayerId, flushPrice, openCardPrice, eagleEyePrice);
		}
		if (IsLuckStarActivityOpen())
		{
			m_player.Out.SendLuckStarOpen(m_player.PlayerId);
		}
	}

	public GSPacketIn SendLuckStarClose()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(26);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	private void SetupLuckyStart()
	{
		m_luckyBegindate = DateTime.Parse(GameProperties.LuckStarActivityBeginDate);
		m_luckyEnddate = DateTime.Parse(GameProperties.LuckStarActivityEndDate);
		m_minUseNum = GameProperties.MinUseNum;
	}

	public void CreateLuckyStartAward()
	{
		m_LuckyStartRewards = TreasureAwardMgr.CreateChickenBoxAward(LuckyStartBoxCount);
		NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
		newChickenBoxItemInfo.TemplateID = coinTemplateID;
		newChickenBoxItemInfo.StrengthenLevel = 0;
		newChickenBoxItemInfo.Count = 1;
		newChickenBoxItemInfo.IsBinds = true;
		newChickenBoxItemInfo.Quality = 1;
		m_LuckyStartRewards[0] = newChickenBoxItemInfo;
		rand.Shuffer(m_LuckyStartRewards);
	}

	public void ChangeLuckyStartAwardPlace()
	{
		rand.Shuffer(m_LuckyStartRewards);
	}

	public void SendLuckStarAllGoodsInfo()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(21);
		gSPacketIn.WriteInt(m_activeInfo.LuckystarCoins);
		gSPacketIn.WriteDateTime(LuckyBegindate);
		gSPacketIn.WriteDateTime(LuckyEnddate);
		gSPacketIn.WriteInt(minUseNum);
		int num = m_LuckyStartRewards.Length;
		int i = 0;
		gSPacketIn.WriteInt(num);
		for (; i < num; i++)
		{
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].TemplateID);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].StrengthenLevel);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].Count);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].ValidDate);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].AttackCompose);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].DefendCompose);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].AgilityCompose);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].LuckCompose);
			gSPacketIn.WriteBoolean(m_LuckyStartRewards[i].IsBinds);
			gSPacketIn.WriteInt(m_LuckyStartRewards[i].Quality);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendLuckStarRewardRecord()
	{
		List<LuckStarRewardRecordInfo> recordList = ActiveSystemMgr.RecordList;
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(22);
		gSPacketIn.WriteInt(recordList.Count);
		foreach (LuckStarRewardRecordInfo item in recordList)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteString(item.nickName);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public void SendUpdateReward()
	{
		if (Award.TemplateID != coinTemplateID)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
			gSPacketIn.WriteInt(24);
			gSPacketIn.WriteInt(Award.TemplateID);
			gSPacketIn.WriteInt(Award.Count);
			gSPacketIn.WriteString(m_player.PlayerCharacter.NickName);
			m_player.SendTCP(gSPacketIn);
		}
	}

	private void GetAward()
	{
		int num = rand.Next(m_LuckyStartRewards.Length);
		m_award = m_LuckyStartRewards[num];
	}

	public void SendLuckStarTurnGoodsInfo()
	{
		GetAward();
		m_activeInfo.LuckystarCoins += flushCoins;
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(23);
		gSPacketIn.WriteInt(m_activeInfo.LuckystarCoins);
		gSPacketIn.WriteInt(Award.TemplateID);
		gSPacketIn.WriteInt(Award.StrengthenLevel);
		gSPacketIn.WriteInt(Award.Count);
		gSPacketIn.WriteInt(Award.ValidDate);
		gSPacketIn.WriteInt(Award.AttackCompose);
		gSPacketIn.WriteInt(Award.DefendCompose);
		gSPacketIn.WriteInt(Award.AgilityCompose);
		gSPacketIn.WriteInt(Award.LuckCompose);
		gSPacketIn.WriteBoolean(Award.IsBinds);
		m_player.SendTCP(gSPacketIn);
		if (Award.TemplateID == coinTemplateID)
		{
			if (GameProperties.IsActiveMoney)
			{
				m_player.AddActiveMoney(m_activeInfo.LuckystarCoins);
			}
			else
			{
				m_player.AddMoney(m_activeInfo.LuckystarCoins);
			}
			m_activeInfo.LuckystarCoins = defaultCoins;
		}
		ActiveSystemMgr.UpdateLuckStarRewardRecord(m_player.PlayerCharacter.ID, m_player.PlayerCharacter.NickName, Award.TemplateID, Award.Count, m_player.PlayerCharacter.typeVIP);
	}

	public void SendLuckStarRewardRank()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, m_player.PlayerId);
		gSPacketIn.WriteInt(27);
		List<LuckyStartToptenAwardInfo> luckyStartToptenAward = WorldEventMgr.GetLuckyStartToptenAward();
		gSPacketIn.WriteInt(luckyStartToptenAward.Count);
		foreach (LuckyStartToptenAwardInfo item in luckyStartToptenAward)
		{
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.StrengthenLevel);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteInt(item.Validate);
			gSPacketIn.WriteInt(item.AttackCompose);
			gSPacketIn.WriteInt(item.DefendCompose);
			gSPacketIn.WriteInt(item.AgilityCompose);
			gSPacketIn.WriteInt(item.LuckCompose);
			gSPacketIn.WriteBoolean(item.IsBinds);
			gSPacketIn.WriteInt(item.Type);
		}
		m_player.SendTCP(gSPacketIn);
	}

	public GSPacketIn SendLightriddleRank(int myRank, List<RankingLightriddleInfo> list)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(42);
		gSPacketIn.WriteInt(myRank);
		gSPacketIn.WriteInt(list.Count);
		foreach (RankingLightriddleInfo item in list)
		{
			gSPacketIn.WriteInt(item.Rank);
			gSPacketIn.WriteString(item.NickName);
			gSPacketIn.WriteByte((byte)item.TypeVIP);
			gSPacketIn.WriteInt(item.Integer);
			List<LuckyStartToptenAwardInfo> lanternriddlesAwardByRank = WorldEventMgr.GetLanternriddlesAwardByRank(item.Rank);
			gSPacketIn.WriteInt(lanternriddlesAwardByRank.Count);
			foreach (LuckyStartToptenAwardInfo item2 in lanternriddlesAwardByRank)
			{
				gSPacketIn.WriteInt(item2.TemplateID);
				gSPacketIn.WriteInt(item2.Count);
				gSPacketIn.WriteBoolean(item2.IsBinds);
				gSPacketIn.WriteInt(item2.Validate);
			}
		}
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLightriddleQuestion(LanternriddlesInfo Lanternriddles)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(38);
		gSPacketIn.WriteInt(Lanternriddles.QuestionIndex);
		gSPacketIn.WriteInt(Lanternriddles.GetQuestionID);
		gSPacketIn.WriteInt(Lanternriddles.QuestionView);
		gSPacketIn.WriteDateTime(Lanternriddles.EndDate);
		gSPacketIn.WriteInt(Lanternriddles.DoubleFreeCount);
		gSPacketIn.WriteInt(Lanternriddles.DoublePrice);
		gSPacketIn.WriteInt(Lanternriddles.HitFreeCount);
		gSPacketIn.WriteInt(Lanternriddles.HitPrice);
		gSPacketIn.WriteInt(Lanternriddles.MyInteger);
		gSPacketIn.WriteInt(Lanternriddles.QuestionNum);
		gSPacketIn.WriteInt(Lanternriddles.Option);
		gSPacketIn.WriteBoolean(Lanternriddles.IsHint);
		gSPacketIn.WriteBoolean(Lanternriddles.IsDouble);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public GSPacketIn SendLightriddleAnswerResult(bool Iscorrect, int option, string award)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(145, m_player.PlayerId);
		gSPacketIn.WriteByte(39);
		gSPacketIn.WriteBoolean(Iscorrect);
		gSPacketIn.WriteBoolean(Iscorrect);
		gSPacketIn.WriteInt(option);
		gSPacketIn.WriteString(award);
		m_player.SendTCP(gSPacketIn);
		return gSPacketIn;
	}

	public void BeginLightriddleTimer()
	{
		int num = 1000;
		if (_lightriddleTimer == null)
		{
			_lightriddleTimer = new Timer(LightriddleCheck, null, num, num);
		}
		else
		{
			_lightriddleTimer.Change(num, num);
		}
	}

	protected void LightriddleCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			if (_lightriddleColdown > 0)
			{
				_lightriddleColdown--;
				if (_lightriddleColdown == 1)
				{
					LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(m_player.PlayerId);
					if (lanternriddles == null)
					{
						StopLightriddleTimer();
						return;
					}
					LightriddleQuestInfo getCurrentQuestion = lanternriddles.GetCurrentQuestion;
					string text = "5 Chiến Hồn Đơn, 10.000 EXP và 29 điểm tích lũy";
					string text2 = "1 Chiến Hồn Đơn và 1.000 EXP.";
					string award = "Hệ thống Nguyên Tiêu lổi.";
					int gp = 1000;
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(100100);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 105);
					bool iscorrect = false;
					if (getCurrentQuestion != null)
					{
						if (lanternriddles.IsHint)
						{
							lanternriddles.Option = getCurrentQuestion.OptionTrue;
						}
						if (lanternriddles.Option == getCurrentQuestion.OptionTrue)
						{
							itemInfo.Count = 5;
							gp = 10000;
							lanternriddles.MyInteger += 29;
							lanternriddles.QuestionNum++;
							if (lanternriddles.IsDouble)
							{
								text = "5 Chiến Hồn Đơn, 10.000 EXP và 58 điểm tích lũy";
								lanternriddles.MyInteger += 29;
							}
							award = text;
							iscorrect = true;
						}
						else
						{
							itemInfo.Count = 1;
							award = text2;
						}
					}
					if (lanternriddles.Option > 0)
					{
						m_player.AddGP(gp);
						itemInfo.IsBinds = true;
						m_player.AddTemplate(itemInfo);
						SendLightriddleAnswerResult(iscorrect, lanternriddles.Option, award);
						GameServer.Instance.LoginServer.SendLightriddleUpateRank(lanternriddles.MyInteger, m_player.PlayerCharacter);
					}
					GameServer.Instance.LoginServer.SendLightriddleInfo(lanternriddles);
				}
			}
			else
			{
				LanternriddlesInfo lanternriddles2 = ActiveSystemMgr.GetLanternriddles(m_player.PlayerId);
				if (lanternriddles2 == null)
				{
					StopLightriddleTimer();
					return;
				}
				if (lanternriddles2.CanNextQuest)
				{
					lanternriddles2.QuestionIndex++;
					lanternriddles2.Option = -1;
					lanternriddles2.IsHint = false;
					lanternriddles2.IsDouble = false;
					lanternriddles2.EndDate = ActiveSystemMgr.EndDate;
					SendLightriddleQuestion(lanternriddles2);
					_lightriddleColdown = 15;
					GameServer.Instance.LoginServer.SendLightriddleRank(m_player.PlayerCharacter.NickName, m_player.PlayerCharacter.ID);
				}
				else
				{
					lanternriddles2.QuestionIndex = lanternriddles2.QuestionView;
					lanternriddles2.IsHint = true;
					lanternriddles2.IsDouble = true;
					lanternriddles2.EndDate = DateTime.Now;
					SendLightriddleQuestion(lanternriddles2);
					StopLightriddleTimer();
				}
				GameServer.Instance.LoginServer.SendLightriddleInfo(lanternriddles2);
			}
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("LabyrinthCheck: " + ex);
		}
	}

	public void StopLightriddleTimer()
	{
		if (_lightriddleTimer != null)
		{
			_lightriddleColdown = 15;
			_lightriddleTimer.Dispose();
			_lightriddleTimer = null;
		}
	}
}
