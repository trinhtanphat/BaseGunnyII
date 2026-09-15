using System;
using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class PlayerInfo : DataObject
{
	private int _agility;

	private int _antiAddiction;

	private DateTime _antiDate;

	private int _attack;

	private string _checkCode;

	private int _checkCount;

	private DateTime _checkDate;

	private int _checkError;

	private string _colors;

	private int _consortiaID;

	private string _consortiaName;

	private bool _consortiaRename;

	private int _dayLoginCount;

	private int _defence;

	private int _escape;

	private DateTime? _expendDate;

	private int _fightPower;

	private int _giftGp;

	private int _giftLevel;

	private int _GiftToken;

	private int _gold;

	private int _gp;

	private int _grade;

	private int _hide;

	private PlayerInfoHistory _history;

	private TexpInfo _texp;

	private int _id;

	private int _hp;

	private int _inviter;

	private bool _isConsortia;

	private bool _isCreatedMarryRoom;

	private int _IsFirst;

	private bool _isGotRing;

	private bool _isLocked = true;

	private bool _isMarried;

	private byte _typeVIP;

	private bool _canTakeVipReward;

	private DateTime _LastAuncherAward;

	private DateTime _LastAward;

	private DateTime _LastVIPPackTime;

	private DateTime _LastWeekly;

	private int _LastWeeklyVersion;

	private int _luck;

	private int _marryInfoID;

	private int _money;

	private string _nickName;

	private int _nimbus;

	private int _offer;

	private string _PasswordTwo;

	private byte[] _QuestSite;

	private bool _rename;

	private int _repute;

	private int _richesOffer;

	private int _richesRob;

	private int _selfMarryRoomID;

	private bool _sex;

	private string _skin;

	private int _spouseID;

	private string _spouseName;

	private int _state;

	private string _style;

	private Dictionary<string, object> _tempInfo = new Dictionary<string, object>();

	private int _total;

	private string _userName;

	private int _VIPExp;

	private DateTime _VIPExpireDay;

	private int _VIPLevel;

	private int _VIPOfflineDays;

	private int _VIPOnlineDays;

	private int _win;

	private int m_AchievementPoint;

	private int m_AddDayAchievementPoint;

	private DateTime m_AddGPLastDate;

	private int m_AddWeekAchievementPoint;

	private int m_AlreadyGetBox;

	private int m_AnswerSite;

	private int m_BanChat;

	private DateTime m_BanChatEndDate;

	private DateTime m_BoxGetDate;

	private int m_BoxProgression;

	private int m_ChatCount;

	private int m_FailedPasswordAttemptCount;

	private string m_fightlabPermission;

	private int m_gameActiveHide;

	private string m_gameActiveStyle;

	private int m_getBoxLevel;

	private bool m_IsInSpaPubGoldToday;

	private bool m_IsInSpaPubMoneyToday;

	private bool m_IsOpenGift;

	private DateTime m_lastDate;

	private DateTime m_VIPlastDate;

	private DateTime m_LastSpaDate;

	private int m_OnlineTime;

	private string m_PasswordQuest1;

	private string m_PasswordQuest2;

	private string m_pvePermission;

	private string m_Rank;

	private int m_SpaPubGoldRoomLimit;

	private int m_SpaPubMoneyRoomLimit;

	private int _maxBuyHonor;

	private string _password;

	private int _medal;

	private bool _isOldPlayer;

	private string _weaklessGuildProgressStr;

	private byte[] _weaklessGuildProgress;

	private int _leagueMoney;

	private bool _isOpenKingBless;

	private int _VIPNextLevelDaysNeeded;

	private int _cardSoul;

	private int _score;

	private int _optionOnOff;

	private bool _isOldPlayerHasValidEquitAtLogin;

	private int _badLuckNumber;

	private int _lastLuckNum;

	private int _luckyNum;

	private DateTime _lastLuckyNumDate;

	private int _uesedFinishTime;

	private int _totemId;

	private int _damageScores;

	private int _petScore;

	private int _myHonor;

	private string _honor;

	private int _hardCurrency;

	private bool _isShowConsortia;

	private DateTime _timeBox;

	private bool _isFistGetPet;

	private int m_myScore;

	private DateTime m_LastRefreshPet;

	private int m_receiebox;

	private int m_receieGrade;

	private int m_needGetBoxTime;

	private DateTime m_LastGetEgg;

	private int _necklaceExp;

	private int _necklaceExpAdd;

	private int _GetSoulCount;

	private int _isFirstDivorce;

	public int DameAddPlus;

	public int GuardAddPlus;

	public int AttackAddPlus;

	public int AgiAddPlus;

	public int DefendAddPlus;

	public int LuckAddPlus;

	public int HpAddPlus;

	public TexpInfo Texp
	{
		get
		{
			return _texp;
		}
		set
		{
			_texp = value;
			_isDirty = true;
		}
	}

	public int MaxBuyHonor
	{
		get
		{
			return _maxBuyHonor;
		}
		set
		{
			_maxBuyHonor = value;
			_isDirty = true;
		}
	}

	public string Password
	{
		get
		{
			return _password;
		}
		set
		{
			_password = value;
			_isDirty = true;
		}
	}

	public int medal
	{
		get
		{
			return _medal;
		}
		set
		{
			_medal = value;
			_isDirty = true;
		}
	}

	public int hp
	{
		get
		{
			return _hp;
		}
		set
		{
			_hp = value;
			_isDirty = true;
		}
	}

	public bool IsOldPlayer
	{
		get
		{
			return _isOldPlayer;
		}
		set
		{
			_isOldPlayer = value;
			_isDirty = true;
		}
	}

	public string WeaklessGuildProgressStr
	{
		get
		{
			return _weaklessGuildProgressStr;
		}
		set
		{
			_weaklessGuildProgressStr = value;
			_isDirty = true;
		}
	}

	public int AchievementPoint
	{
		get
		{
			return m_AchievementPoint;
		}
		set
		{
			m_AchievementPoint = value;
		}
	}

	public int AddDayAchievementPoint
	{
		get
		{
			return m_AddDayAchievementPoint;
		}
		set
		{
			m_AddDayAchievementPoint = value;
		}
	}

	public int AddDayGiftGp { get; set; }

	public int AddDayGP { get; set; }

	public int AddDayOffer { get; set; }

	public DateTime AddGPLastDate
	{
		get
		{
			return m_AddGPLastDate;
		}
		set
		{
			m_AddGPLastDate = value;
		}
	}

	public int AddWeekAchievementPoint
	{
		get
		{
			return m_AddWeekAchievementPoint;
		}
		set
		{
			m_AddWeekAchievementPoint = value;
		}
	}

	public int AddWeekGiftGp { get; set; }

	public int AddWeekGP { get; set; }

	public int AddWeekOffer { get; set; }

	public int Agility
	{
		get
		{
			return _agility;
		}
		set
		{
			_agility = value;
			_isDirty = true;
		}
	}

	public int AlreadyGetBox
	{
		get
		{
			return m_AlreadyGetBox;
		}
		set
		{
			m_AlreadyGetBox = value;
		}
	}

	public int AnswerSite
	{
		get
		{
			return m_AnswerSite;
		}
		set
		{
			m_AnswerSite = value;
		}
	}

	public int AntiAddiction
	{
		get
		{
			TimeSpan timeSpan = DateTime.Now - _antiDate;
			return _antiAddiction + (int)timeSpan.TotalMinutes;
		}
		set
		{
			_antiAddiction = value;
			_antiDate = DateTime.Now;
		}
	}

	public DateTime AntiDate
	{
		get
		{
			return _antiDate;
		}
		set
		{
			_antiDate = value;
		}
	}

	public int Attack
	{
		get
		{
			return _attack;
		}
		set
		{
			_attack = value;
			_isDirty = true;
		}
	}

	public int BanChat
	{
		get
		{
			return m_BanChat;
		}
		set
		{
			m_BanChat = value;
		}
	}

	public DateTime BanChatEndDate
	{
		get
		{
			return m_BanChatEndDate;
		}
		set
		{
			m_BanChatEndDate = value;
		}
	}

	public DateTime BoxGetDate
	{
		get
		{
			return m_BoxGetDate;
		}
		set
		{
			m_BoxGetDate = value;
		}
	}

	public int BoxProgression
	{
		get
		{
			return m_BoxProgression;
		}
		set
		{
			m_BoxProgression = value;
		}
	}

	public string ChairmanName { get; set; }

	public int ChatCount
	{
		get
		{
			return m_ChatCount;
		}
		set
		{
			m_ChatCount = value;
		}
	}

	public string CheckCode
	{
		get
		{
			return _checkCode;
		}
		set
		{
			_checkDate = DateTime.Now;
			_checkCode = value;
			string.IsNullOrEmpty(_checkCode);
		}
	}

	public int CheckCount
	{
		get
		{
			return _checkCount;
		}
		set
		{
			_checkCount = value;
			_isDirty = true;
		}
	}

	public DateTime CheckDate => _checkDate;

	public int CheckError
	{
		get
		{
			return _checkError;
		}
		set
		{
			_checkError = value;
		}
	}

	public string Colors
	{
		get
		{
			return _colors;
		}
		set
		{
			_colors = value;
			_isDirty = true;
		}
	}

	public int ConsortiaGiftGp { get; set; }

	public int ConsortiaHonor { get; set; }

	public int ConsortiaID
	{
		get
		{
			return _consortiaID;
		}
		set
		{
			if (_consortiaID == 0 || value == 0)
			{
				_richesRob = 0;
				_richesOffer = 0;
			}
			_consortiaID = value;
		}
	}

	public int ConsortiaLevel { get; set; }

	public string ConsortiaName
	{
		get
		{
			return _consortiaName;
		}
		set
		{
			_consortiaName = value;
		}
	}

	public int _badgeID { get; set; }

	public int badgeID
	{
		get
		{
			return _badgeID;
		}
		set
		{
			_badgeID = value;
			_isDirty = true;
		}
	}

	public bool ConsortiaRename
	{
		get
		{
			return _consortiaRename;
		}
		set
		{
			if (_consortiaRename != value)
			{
				_consortiaRename = value;
				_isDirty = true;
			}
		}
	}

	public int ConsortiaRepute { get; set; }

	public int ConsortiaRiches { get; set; }

	public int DayLoginCount
	{
		get
		{
			return _dayLoginCount;
		}
		set
		{
			_dayLoginCount = value;
			_isDirty = true;
		}
	}

	public int Defence
	{
		get
		{
			return _defence;
		}
		set
		{
			_defence = value;
			_isDirty = true;
		}
	}

	public int DutyLevel { get; set; }

	public string DutyName { get; set; }

	public int Escape
	{
		get
		{
			return _escape;
		}
		set
		{
			_escape = value;
			_isDirty = true;
		}
	}

	public DateTime? ExpendDate
	{
		get
		{
			return _expendDate;
		}
		set
		{
			_expendDate = value;
			_isDirty = true;
		}
	}

	public int FailedPasswordAttemptCount
	{
		get
		{
			return m_FailedPasswordAttemptCount;
		}
		set
		{
			m_FailedPasswordAttemptCount = value;
		}
	}

	public string FightLabPermission
	{
		get
		{
			return m_fightlabPermission;
		}
		set
		{
			m_fightlabPermission = value;
		}
	}

	public int FightPower
	{
		get
		{
			return _fightPower;
		}
		set
		{
			if (_fightPower != value)
			{
				_fightPower = value;
				_isDirty = true;
			}
		}
	}

	public int GameActiveHide
	{
		get
		{
			return m_gameActiveHide;
		}
		set
		{
			m_gameActiveHide = value;
		}
	}

	public string GameActiveStyle
	{
		get
		{
			return m_gameActiveStyle;
		}
		set
		{
			m_gameActiveStyle = value;
		}
	}

	public int GetBoxLevel
	{
		get
		{
			return m_getBoxLevel;
		}
		set
		{
			m_getBoxLevel = value;
		}
	}

	public int GiftGp
	{
		get
		{
			return _giftGp;
		}
		set
		{
			_giftGp = value;
			_isDirty = true;
		}
	}

	public int GiftLevel
	{
		get
		{
			return _giftLevel;
		}
		set
		{
			_giftLevel = value;
			_isDirty = true;
		}
	}

	public int GiftToken
	{
		get
		{
			return _GiftToken;
		}
		set
		{
			_GiftToken = value;
		}
	}

	public int Gold
	{
		get
		{
			return _gold;
		}
		set
		{
			_gold = value;
			_isDirty = true;
		}
	}

	public int GP
	{
		get
		{
			return _gp;
		}
		set
		{
			_gp = value;
			_isDirty = true;
		}
	}

	public int Grade
	{
		get
		{
			return _grade;
		}
		set
		{
			_grade = value;
			_isDirty = true;
		}
	}

	public bool HasBagPassword => !string.IsNullOrEmpty(_PasswordTwo);

	public int Hide
	{
		get
		{
			return _hide;
		}
		set
		{
			_hide = value;
			_isDirty = true;
		}
	}

	public PlayerInfoHistory History
	{
		get
		{
			return _history;
		}
		set
		{
			_history = value;
		}
	}

	public byte[] weaklessGuildProgress
	{
		get
		{
			return _weaklessGuildProgress;
		}
		set
		{
			_weaklessGuildProgress = value;
			_isDirty = true;
		}
	}

	public int ID
	{
		get
		{
			return _id;
		}
		set
		{
			_id = value;
			_isDirty = true;
		}
	}

	public int Inviter
	{
		get
		{
			return _inviter;
		}
		set
		{
			_inviter = value;
		}
	}

	public bool IsBanChat { get; set; }

	public bool IsConsortia
	{
		get
		{
			return _isConsortia;
		}
		set
		{
			_isConsortia = value;
		}
	}

	public bool IsCreatedMarryRoom
	{
		get
		{
			return _isCreatedMarryRoom;
		}
		set
		{
			if (_isCreatedMarryRoom != value)
			{
				_isCreatedMarryRoom = value;
				_isDirty = true;
			}
		}
	}

	public int IsFirst
	{
		get
		{
			return _IsFirst;
		}
		set
		{
			_IsFirst = value;
		}
	}

	public bool IsGotRing
	{
		get
		{
			return _isGotRing;
		}
		set
		{
			if (_isGotRing != value)
			{
				_isGotRing = value;
				_isDirty = true;
			}
		}
	}

	public bool IsInSpaPubGoldToday
	{
		get
		{
			return m_IsInSpaPubGoldToday;
		}
		set
		{
			m_IsInSpaPubGoldToday = value;
		}
	}

	public bool IsInSpaPubMoneyToday
	{
		get
		{
			return m_IsInSpaPubMoneyToday;
		}
		set
		{
			m_IsInSpaPubMoneyToday = value;
		}
	}

	public bool IsLocked
	{
		get
		{
			return _isLocked;
		}
		set
		{
			_isLocked = value;
		}
	}

	public bool IsMarried
	{
		get
		{
			return _isMarried;
		}
		set
		{
			_isMarried = value;
			_isDirty = true;
		}
	}

	public bool IsOpenGift
	{
		get
		{
			return m_IsOpenGift;
		}
		set
		{
			m_IsOpenGift = value;
		}
	}

	public byte typeVIP
	{
		get
		{
			return _typeVIP;
		}
		set
		{
			if (_typeVIP != value)
			{
				_typeVIP = value;
				_isDirty = true;
			}
		}
	}

	public bool CanTakeVipReward
	{
		get
		{
			return _canTakeVipReward;
		}
		set
		{
			_canTakeVipReward = value;
			_isDirty = true;
		}
	}

	public DateTime LastAuncherAward
	{
		get
		{
			return _LastAuncherAward;
		}
		set
		{
			_LastAuncherAward = value;
		}
	}

	public DateTime LastAward
	{
		get
		{
			return _LastAward;
		}
		set
		{
			_LastAward = value;
		}
	}

	public DateTime LastDate
	{
		get
		{
			return m_lastDate;
		}
		set
		{
			m_lastDate = value;
		}
	}

	public DateTime VIPLastDate
	{
		get
		{
			return m_VIPlastDate;
		}
		set
		{
			m_VIPlastDate = value;
		}
	}

	public DateTime LastSpaDate
	{
		get
		{
			return m_LastSpaDate;
		}
		set
		{
			m_LastSpaDate = value;
		}
	}

	public DateTime LastVIPPackTime
	{
		get
		{
			return _LastVIPPackTime;
		}
		set
		{
			_LastVIPPackTime = value;
			_isDirty = true;
		}
	}

	public DateTime LastWeekly
	{
		get
		{
			return _LastWeekly;
		}
		set
		{
			_LastWeekly = value;
		}
	}

	public int LastWeeklyVersion
	{
		get
		{
			return _LastWeeklyVersion;
		}
		set
		{
			_LastWeeklyVersion = value;
		}
	}

	public int Luck
	{
		get
		{
			return _luck;
		}
		set
		{
			_luck = value;
			_isDirty = true;
		}
	}

	public int MarryInfoID
	{
		get
		{
			return _marryInfoID;
		}
		set
		{
			if (_marryInfoID != value)
			{
				_marryInfoID = value;
				_isDirty = true;
			}
		}
	}

	public int Money
	{
		get
		{
			return _money;
		}
		set
		{
			_money = value;
			_isDirty = true;
		}
	}

	public string NickName
	{
		get
		{
			return _nickName;
		}
		set
		{
			_nickName = value;
			_isDirty = true;
		}
	}

	public int Nimbus
	{
		get
		{
			return _nimbus;
		}
		set
		{
			if (_nimbus != value)
			{
				_nimbus = value;
				_isDirty = true;
			}
		}
	}

	public int Offer
	{
		get
		{
			return _offer;
		}
		set
		{
			_offer = value;
			_isDirty = true;
		}
	}

	public int OnlineTime
	{
		get
		{
			return m_OnlineTime;
		}
		set
		{
			m_OnlineTime = value;
		}
	}

	public string PasswordQuest1
	{
		get
		{
			return m_PasswordQuest1;
		}
		set
		{
			m_PasswordQuest1 = value;
		}
	}

	public string PasswordQuest2
	{
		get
		{
			return m_PasswordQuest2;
		}
		set
		{
			m_PasswordQuest2 = value;
		}
	}

	public string PasswordTwo
	{
		get
		{
			return _PasswordTwo;
		}
		set
		{
			_PasswordTwo = value;
			_isDirty = true;
		}
	}

	public string PvePermission
	{
		get
		{
			return m_pvePermission;
		}
		set
		{
			m_pvePermission = value;
		}
	}

	public byte[] QuestSite
	{
		get
		{
			return _QuestSite;
		}
		set
		{
			_QuestSite = value;
		}
	}

	public string Rank
	{
		get
		{
			return m_Rank;
		}
		set
		{
			m_Rank = value;
		}
	}

	public bool Rename
	{
		get
		{
			return _rename;
		}
		set
		{
			if (_rename != value)
			{
				_rename = value;
				_isDirty = true;
			}
		}
	}

	public int Repute
	{
		get
		{
			return _repute;
		}
		set
		{
			_repute = value;
			_isDirty = true;
		}
	}

	public int ReputeOffer { get; set; }

	public int Riches => RichesRob + RichesOffer;

	public int LeagueMoney
	{
		get
		{
			return _leagueMoney;
		}
		set
		{
			_leagueMoney = value;
			_isDirty = true;
		}
	}

	public int RichesOffer
	{
		get
		{
			return _richesOffer;
		}
		set
		{
			_richesOffer = value;
			_isDirty = true;
		}
	}

	public int RichesRob
	{
		get
		{
			return _richesRob;
		}
		set
		{
			_richesRob = value;
			_isDirty = true;
		}
	}

	public int Right { get; set; }

	public int SelfMarryRoomID
	{
		get
		{
			return _selfMarryRoomID;
		}
		set
		{
			if (_selfMarryRoomID != value)
			{
				_selfMarryRoomID = value;
				_isDirty = true;
			}
		}
	}

	public bool isOpenKingBless
	{
		get
		{
			return _isOpenKingBless;
		}
		set
		{
			_isOpenKingBless = value;
			_isDirty = true;
		}
	}

	public bool Sex
	{
		get
		{
			return _sex;
		}
		set
		{
			_sex = value;
			_isDirty = true;
		}
	}

	public int ShopLevel { get; set; }

	public string Skin
	{
		get
		{
			return _skin;
		}
		set
		{
			_skin = value;
			_isDirty = true;
		}
	}

	public int SmithLevel { get; set; }

	public int SpaPubGoldRoomLimit
	{
		get
		{
			return m_SpaPubGoldRoomLimit;
		}
		set
		{
			m_SpaPubGoldRoomLimit = value;
		}
	}

	public int SpaPubMoneyRoomLimit
	{
		get
		{
			return m_SpaPubMoneyRoomLimit;
		}
		set
		{
			m_SpaPubMoneyRoomLimit = value;
		}
	}

	public int SpouseID
	{
		get
		{
			return _spouseID;
		}
		set
		{
			if (_spouseID != value)
			{
				_spouseID = value;
				_isDirty = true;
			}
		}
	}

	public string SpouseName
	{
		get
		{
			return _spouseName;
		}
		set
		{
			if (_spouseName != value)
			{
				_spouseName = value;
				_isDirty = true;
			}
		}
	}

	public int State
	{
		get
		{
			return _state;
		}
		set
		{
			_state = value;
			_isDirty = true;
		}
	}

	public int StoreLevel { get; set; }

	public int SkillLevel { get; set; }

	public string Style
	{
		get
		{
			return _style;
		}
		set
		{
			_style = value;
			_isDirty = true;
		}
	}

	public Dictionary<string, object> TempInfo => _tempInfo;

	public int Total
	{
		get
		{
			return _total;
		}
		set
		{
			_total = value;
			_isDirty = true;
		}
	}

	public string UserName
	{
		get
		{
			return _userName;
		}
		set
		{
			_userName = value;
			_isDirty = true;
		}
	}

	public int VIPExp
	{
		get
		{
			return _VIPExp;
		}
		set
		{
			if (_VIPExp != value)
			{
				_VIPExp = value;
				_isDirty = true;
			}
		}
	}

	public DateTime VIPExpireDay
	{
		get
		{
			return _VIPExpireDay;
		}
		set
		{
			_VIPExpireDay = value;
			_isDirty = true;
		}
	}

	public int VIPLevel
	{
		get
		{
			return _VIPLevel;
		}
		set
		{
			if (_VIPLevel != value)
			{
				_VIPLevel = value;
				_isDirty = true;
			}
		}
	}

	public int VIPNextLevelDaysNeeded
	{
		get
		{
			return _VIPNextLevelDaysNeeded;
		}
		set
		{
			_VIPNextLevelDaysNeeded = value;
			_isDirty = true;
		}
	}

	public int VIPOfflineDays
	{
		get
		{
			return _VIPOfflineDays;
		}
		set
		{
			_VIPOfflineDays = value;
		}
	}

	public int VIPOnlineDays
	{
		get
		{
			return _VIPOnlineDays;
		}
		set
		{
			_VIPOnlineDays = value;
		}
	}

	public int CardSoul
	{
		get
		{
			return _cardSoul;
		}
		set
		{
			_cardSoul = value;
		}
	}

	public int Score
	{
		get
		{
			return _score;
		}
		set
		{
			_score = value;
		}
	}

	public int OptionOnOff
	{
		get
		{
			return _optionOnOff;
		}
		set
		{
			_optionOnOff = value;
		}
	}

	public bool isOldPlayerHasValidEquitAtLogin
	{
		get
		{
			return _isOldPlayerHasValidEquitAtLogin;
		}
		set
		{
			_isOldPlayerHasValidEquitAtLogin = value;
		}
	}

	public int badLuckNumber
	{
		get
		{
			return _badLuckNumber;
		}
		set
		{
			_badLuckNumber = value;
		}
	}

	public int lastLuckNum
	{
		get
		{
			return _lastLuckNum;
		}
		set
		{
			_lastLuckNum = value;
		}
	}

	public int luckyNum
	{
		get
		{
			return _luckyNum;
		}
		set
		{
			_luckyNum = value;
		}
	}

	public DateTime lastLuckyNumDate
	{
		get
		{
			return _lastLuckyNumDate;
		}
		set
		{
			_lastLuckyNumDate = value;
		}
	}

	public int uesedFinishTime
	{
		get
		{
			return _uesedFinishTime;
		}
		set
		{
			_uesedFinishTime = value;
		}
	}

	public int totemId
	{
		get
		{
			return _totemId;
		}
		set
		{
			_totemId = value;
		}
	}

	public int damageScores
	{
		get
		{
			return _damageScores;
		}
		set
		{
			_damageScores = value;
		}
	}

	public int petScore
	{
		get
		{
			return _petScore;
		}
		set
		{
			_petScore = value;
		}
	}

	public int myHonor
	{
		get
		{
			return _myHonor;
		}
		set
		{
			_myHonor = value;
		}
	}

	public string Honor
	{
		get
		{
			return _honor;
		}
		set
		{
			_honor = value;
		}
	}

	public int hardCurrency
	{
		get
		{
			return _hardCurrency;
		}
		set
		{
			_hardCurrency = value;
		}
	}

	public bool IsShowConsortia
	{
		get
		{
			return _isShowConsortia;
		}
		set
		{
			_isShowConsortia = value;
		}
	}

	public DateTime TimeBox
	{
		get
		{
			return _timeBox;
		}
		set
		{
			_timeBox = value;
		}
	}

	public bool IsFistGetPet
	{
		get
		{
			return _isFistGetPet;
		}
		set
		{
			_isFistGetPet = value;
		}
	}

	public int Win
	{
		get
		{
			return _win;
		}
		set
		{
			_win = value;
			_isDirty = true;
		}
	}

	public int myScore
	{
		get
		{
			return m_myScore;
		}
		set
		{
			m_myScore = value;
		}
	}

	public DateTime LastRefreshPet
	{
		get
		{
			return m_LastRefreshPet;
		}
		set
		{
			m_LastRefreshPet = value;
		}
	}

	public int receiebox
	{
		get
		{
			return m_receiebox;
		}
		set
		{
			m_receiebox = value;
		}
	}

	public int receieGrade
	{
		get
		{
			return m_receieGrade;
		}
		set
		{
			m_receieGrade = value;
		}
	}

	public int needGetBoxTime
	{
		get
		{
			return m_needGetBoxTime;
		}
		set
		{
			m_needGetBoxTime = value;
		}
	}

	public DateTime LastGetEgg
	{
		get
		{
			return m_LastGetEgg;
		}
		set
		{
			m_LastGetEgg = value;
		}
	}

	public int necklaceExp
	{
		get
		{
			return _necklaceExp;
		}
		set
		{
			_necklaceExp = value;
		}
	}

	public int necklaceExpAdd
	{
		get
		{
			return _necklaceExpAdd;
		}
		set
		{
			_necklaceExpAdd = value;
		}
	}

	public int GetSoulCount
	{
		get
		{
			return _GetSoulCount;
		}
		set
		{
			_GetSoulCount = value;
		}
	}

	public int isFirstDivorce
	{
		get
		{
			return _isFirstDivorce;
		}
		set
		{
			_isFirstDivorce = value;
		}
	}

	public bool bit(int param1)
	{
		param1--;
		int num = param1 / 8;
		int num2 = param1 % 8;
		int num3 = _weaklessGuildProgress[num] & (1 << num2);
		return num3 != 0;
	}

	public bool IsWeakGuildFinish(int id)
	{
		if (id < 1)
		{
			return false;
		}
		return bit(id);
	}

	public void openFunction(Step step)
	{
		int num = (int)(step - 1);
		int num2 = num / 8;
		int num3 = num % 8;
		byte[] array = weaklessGuildProgress;
		if (array.Length > 0)
		{
			array[num2] = (byte)(array[num2] | (1 << num3));
			weaklessGuildProgress = array;
		}
	}

	public void CheckLevelFunction()
	{
		int grade = Grade;
		if (grade > 1)
		{
			openFunction(Step.POP_WELCOME);
			openFunction(Step.CHANNEL_OPEN);
		}
		if (grade > 2)
		{
			openFunction(Step.SHOP_OPEN);
			openFunction(Step.STORE_OPEN);
			openFunction(Step.ENERGY);
			openFunction(Step.MAIL_OPEN);
			openFunction(Step.SIGN_OPEN);
		}
		if (grade > 3)
		{
			openFunction(Step.HP_PROP_OPEN);
		}
		if (grade > 4)
		{
			openFunction(Step.MOVE);
			openFunction(Step.CIVIL_OPEN);
			openFunction(Step.IM_OPEN);
			openFunction(Step.PLAY_ONE_GLOW);
		}
		if (grade > 5)
		{
			openFunction(Step.BEAT_ROBOT);
			openFunction(Step.MASTER_ROOM_OPEN);
			openFunction(Step.POP_ANGLE);
		}
		if (grade > 6)
		{
			openFunction(Step.POP_THREE_FOUR_FIVE);
			openFunction(Step.CONSORTIA_OPEN);
			openFunction(Step.SPAWN_SMALL_BOGU);
			openFunction(Step.PLANE_PROP_OPEN);
		}
		if (grade > 7)
		{
			openFunction(Step.CONSORTIA_SHOW);
			openFunction(Step.DUNGEON_OPEN);
			openFunction(Step.POP_WIN_II);
		}
		if (grade > 8)
		{
			openFunction(Step.DUNGEON_SHOW);
			openFunction(Step.BEAT_LIVING_LEFT);
		}
		if (grade > 9)
		{
			openFunction(Step.CHURCH_OPEN);
		}
		if (grade > 11)
		{
			openFunction(Step.CHURCH_SHOW);
			openFunction(Step.TOFF_LIST_OPEN);
		}
		if (grade > 12)
		{
			openFunction(Step.TOFF_LIST_SHOW);
			openFunction(Step.HOT_WELL_OPEN);
		}
		if (grade > 13)
		{
			openFunction(Step.HOT_WELL_SHOW);
			openFunction(Step.AUCTION_OPEN);
		}
		if (grade > 14)
		{
			openFunction(Step.AUCTION_SHOW);
			openFunction(Step.CAMPAIGN_LAB_OPEN);
		}
	}

	public void ClearConsortia()
	{
		ConsortiaID = 0;
		ConsortiaName = "";
		RichesOffer = 0;
		ConsortiaRepute = 0;
		ConsortiaLevel = 0;
		StoreLevel = 0;
		ShopLevel = 0;
		SmithLevel = 0;
		ConsortiaHonor = 0;
		RichesOffer = 0;
		RichesRob = 0;
		DutyLevel = 0;
		DutyName = "";
		Right = 0;
		AddDayGP = 0;
		AddWeekGP = 0;
		AddDayOffer = 0;
		AddWeekOffer = 0;
		ConsortiaRiches = 0;
	}

	public bool IsLastVIPPackTime()
	{
		return _LastVIPPackTime < DateTime.Now.Date;
	}

	public bool IsVIPExpire()
	{
		return _VIPExpireDay.Date < DateTime.Now.Date;
	}

	public bool IsValidadteTimeBox()
	{
		return _timeBox.Date < DateTime.Now.Date;
	}
}
