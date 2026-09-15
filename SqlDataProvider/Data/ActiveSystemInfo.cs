using System;

namespace SqlDataProvider.Data
{
public class ActiveSystemInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _myRank;

	private string _nickName;

	private int _totalScore;

	private int _useableScore;

	private int _dayScore;

	private bool _CanGetGift;

	private int _AvailTime;

	private int _canOpenCounts;

	private int _canEagleEyeCounts;

	private DateTime _lastFlushTime;

	private bool _isShowAll;

	private int _isBuy;

	private int _activeMoney;

	private int _activityTanabataNum;

	private int _challengeNum;

	private int _buyBuffNum;

	private int _damageNum;

	private int _luckystarCoins;

	private DateTime _lastEnterYearMonter;

	private string _boxState;

	public int ID
	{
		get
		{
			return _ID;
		}
		set
		{
			_ID = value;
			_isDirty = true;
		}
	}

	public int UserID
	{
		get
		{
			return _userID;
		}
		set
		{
			_userID = value;
			_isDirty = true;
		}
	}

	public int myRank
	{
		get
		{
			return _myRank;
		}
		set
		{
			_myRank = value;
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

	public int totalScore
	{
		get
		{
			return _totalScore;
		}
		set
		{
			_totalScore = value;
			_isDirty = true;
		}
	}

	public int useableScore
	{
		get
		{
			return _useableScore;
		}
		set
		{
			_useableScore = value;
			_isDirty = true;
		}
	}

	public int dayScore
	{
		get
		{
			return _dayScore;
		}
		set
		{
			_dayScore = value;
			_isDirty = true;
		}
	}

	public bool CanGetGift
	{
		get
		{
			return _CanGetGift;
		}
		set
		{
			_CanGetGift = value;
			_isDirty = true;
		}
	}

	public int AvailTime
	{
		get
		{
			return _AvailTime;
		}
		set
		{
			_AvailTime = value;
			_isDirty = true;
		}
	}

	public int canOpenCounts
	{
		get
		{
			return _canOpenCounts;
		}
		set
		{
			_canOpenCounts = value;
			_isDirty = true;
		}
	}

	public int canEagleEyeCounts
	{
		get
		{
			return _canEagleEyeCounts;
		}
		set
		{
			_canEagleEyeCounts = value;
			_isDirty = true;
		}
	}

	public DateTime lastFlushTime
	{
		get
		{
			return _lastFlushTime;
		}
		set
		{
			_lastFlushTime = value;
			_isDirty = true;
		}
	}

	public bool isShowAll
	{
		get
		{
			return _isShowAll;
		}
		set
		{
			_isShowAll = value;
			_isDirty = true;
		}
	}

	public int isBuy
	{
		get
		{
			return _isBuy;
		}
		set
		{
			_isBuy = value;
			_isDirty = true;
		}
	}

	public int ActiveMoney
	{
		get
		{
			return _activeMoney;
		}
		set
		{
			_activeMoney = value;
			_isDirty = true;
		}
	}

	public int activityTanabataNum
	{
		get
		{
			return _activityTanabataNum;
		}
		set
		{
			_activityTanabataNum = value;
			_isDirty = true;
		}
	}

	public int ChallengeNum
	{
		get
		{
			return _challengeNum;
		}
		set
		{
			_challengeNum = value;
			_isDirty = true;
		}
	}

	public int BuyBuffNum
	{
		get
		{
			return _buyBuffNum;
		}
		set
		{
			_buyBuffNum = value;
			_isDirty = true;
		}
	}

	public int DamageNum
	{
		get
		{
			return _damageNum;
		}
		set
		{
			_damageNum = value;
			_isDirty = true;
		}
	}

	public int LuckystarCoins
	{
		get
		{
			return _luckystarCoins;
		}
		set
		{
			_luckystarCoins = value;
			_isDirty = true;
		}
	}

	public DateTime lastEnterYearMonter
	{
		get
		{
			return _lastEnterYearMonter;
		}
		set
		{
			_lastEnterYearMonter = value;
			_isDirty = true;
		}
	}

	public string BoxState
	{
		get
		{
			return _boxState;
		}
		set
		{
			_boxState = value;
			_isDirty = true;
		}
	}
}

}
