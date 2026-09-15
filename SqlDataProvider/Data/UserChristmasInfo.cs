using System;

namespace SqlDataProvider.Data
{
public class UserChristmasInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _exp;

	private int _awardState;

	private int _count;

	private int _packsNumber;

	private int _lastPacks;

	private int _dayPacks;

	private DateTime _gameBeginTime;

	private DateTime _gameEndTime;

	private bool _isEnter;

	private int _AvailTime;

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

	public int exp
	{
		get
		{
			return _exp;
		}
		set
		{
			_exp = value;
			_isDirty = true;
		}
	}

	public int awardState
	{
		get
		{
			return _awardState;
		}
		set
		{
			_awardState = value;
			_isDirty = true;
		}
	}

	public int count
	{
		get
		{
			return _count;
		}
		set
		{
			_count = value;
			_isDirty = true;
		}
	}

	public int packsNumber
	{
		get
		{
			return _packsNumber;
		}
		set
		{
			_packsNumber = value;
			_isDirty = true;
		}
	}

	public int lastPacks
	{
		get
		{
			return _lastPacks;
		}
		set
		{
			_lastPacks = value;
			_isDirty = true;
		}
	}

	public int dayPacks
	{
		get
		{
			return _dayPacks;
		}
		set
		{
			_dayPacks = value;
			_isDirty = true;
		}
	}

	public DateTime gameBeginTime
	{
		get
		{
			return _gameBeginTime;
		}
		set
		{
			_gameBeginTime = value;
			_isDirty = true;
		}
	}

	public DateTime gameEndTime
	{
		get
		{
			return _gameEndTime;
		}
		set
		{
			_gameEndTime = value;
			_isDirty = true;
		}
	}

	public bool isEnter
	{
		get
		{
			return _isEnter;
		}
		set
		{
			_isEnter = value;
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
}

}
