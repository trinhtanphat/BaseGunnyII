using System;

namespace SqlDataProvider.Data;

public class UserTreasureInfo : DataObject
{
	private int _ID;

	private int _userID;

	private string _NickName;

	private int _logoinDays;

	private int _treasure;

	private int _treasureAdd;

	private int _friendHelpTimes;

	private bool _isEndTreasure;

	private bool _isBeginTreasure;

	private DateTime _lastLoginDay;

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

	public string NickName
	{
		get
		{
			return _NickName;
		}
		set
		{
			_NickName = value;
			_isDirty = true;
		}
	}

	public int logoinDays
	{
		get
		{
			return _logoinDays;
		}
		set
		{
			_logoinDays = value;
			_isDirty = true;
		}
	}

	public int treasure
	{
		get
		{
			return _treasure;
		}
		set
		{
			_treasure = value;
			_isDirty = true;
		}
	}

	public int treasureAdd
	{
		get
		{
			return _treasureAdd;
		}
		set
		{
			_treasureAdd = value;
			_isDirty = true;
		}
	}

	public int friendHelpTimes
	{
		get
		{
			return _friendHelpTimes;
		}
		set
		{
			_friendHelpTimes = value;
			_isDirty = true;
		}
	}

	public bool isEndTreasure
	{
		get
		{
			return _isEndTreasure;
		}
		set
		{
			_isEndTreasure = value;
			_isDirty = true;
		}
	}

	public bool isBeginTreasure
	{
		get
		{
			return _isBeginTreasure;
		}
		set
		{
			_isBeginTreasure = value;
			_isDirty = true;
		}
	}

	public DateTime LastLoginDay
	{
		get
		{
			return _lastLoginDay;
		}
		set
		{
			_lastLoginDay = value;
			_isDirty = true;
		}
	}

	public bool isValidDate()
	{
		return _lastLoginDay.Date < DateTime.Now.Date;
	}
}
