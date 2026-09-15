using System;

namespace SqlDataProvider.Data;

public class UserLabyrinthInfo : DataObject
{
	private int _userID;

	private int _myProgress;

	private int _myRanking;

	private bool _completeChallenge;

	private bool _isDoubleAward;

	private int _currentFloor;

	private int _accumulateExp;

	private int _remainTime;

	private int _currentRemainTime;

	private int _cleanOutAllTime;

	private int _cleanOutGold;

	private bool _tryAgainComplete;

	private bool _isInGame;

	private bool _isCleanOut;

	private bool _serverMultiplyingPower;

	private DateTime _lastDate;

	private string _processAward;

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

	public int myProgress
	{
		get
		{
			return _myProgress;
		}
		set
		{
			_myProgress = value;
			_isDirty = true;
		}
	}

	public int myRanking
	{
		get
		{
			return _myRanking;
		}
		set
		{
			_myRanking = value;
			_isDirty = true;
		}
	}

	public bool completeChallenge
	{
		get
		{
			return _completeChallenge;
		}
		set
		{
			_completeChallenge = value;
			_isDirty = true;
		}
	}

	public bool isDoubleAward
	{
		get
		{
			return _isDoubleAward;
		}
		set
		{
			_isDoubleAward = value;
			_isDirty = true;
		}
	}

	public int currentFloor
	{
		get
		{
			return _currentFloor;
		}
		set
		{
			_currentFloor = value;
			_isDirty = true;
		}
	}

	public int accumulateExp
	{
		get
		{
			return _accumulateExp;
		}
		set
		{
			_accumulateExp = value;
			_isDirty = true;
		}
	}

	public int remainTime
	{
		get
		{
			return _remainTime;
		}
		set
		{
			_remainTime = value;
			_isDirty = true;
		}
	}

	public int currentRemainTime
	{
		get
		{
			return _currentRemainTime;
		}
		set
		{
			_currentRemainTime = value;
			_isDirty = true;
		}
	}

	public int cleanOutAllTime
	{
		get
		{
			return _cleanOutAllTime;
		}
		set
		{
			_cleanOutAllTime = value;
			_isDirty = true;
		}
	}

	public int cleanOutGold
	{
		get
		{
			return _cleanOutGold;
		}
		set
		{
			_cleanOutGold = value;
			_isDirty = true;
		}
	}

	public bool tryAgainComplete
	{
		get
		{
			return _tryAgainComplete;
		}
		set
		{
			_tryAgainComplete = value;
			_isDirty = true;
		}
	}

	public bool isInGame
	{
		get
		{
			return _isInGame;
		}
		set
		{
			_isInGame = value;
			_isDirty = true;
		}
	}

	public bool isCleanOut
	{
		get
		{
			return _isCleanOut;
		}
		set
		{
			_isCleanOut = value;
			_isDirty = true;
		}
	}

	public bool serverMultiplyingPower
	{
		get
		{
			return _serverMultiplyingPower;
		}
		set
		{
			_serverMultiplyingPower = value;
			_isDirty = true;
		}
	}

	public DateTime LastDate
	{
		get
		{
			return _lastDate;
		}
		set
		{
			_lastDate = value;
			_isDirty = true;
		}
	}

	public string ProcessAward
	{
		get
		{
			return _processAward;
		}
		set
		{
			_processAward = value;
			_isDirty = true;
		}
	}

	public bool isValidDate()
	{
		return _lastDate.Date < DateTime.Now.Date;
	}
}
