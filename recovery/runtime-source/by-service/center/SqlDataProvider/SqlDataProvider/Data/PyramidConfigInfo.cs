using System;

namespace SqlDataProvider.Data;

public class PyramidConfigInfo
{
	private int _userID;

	private bool _isOpen;

	private bool _isScoreExchange;

	private DateTime _beginTime;

	private DateTime _endTime;

	private int _freeCount;

	private int _turnCardPrice;

	private int[] _revivePrice;

	public int UserID
	{
		get
		{
			return _userID;
		}
		set
		{
			_userID = value;
		}
	}

	public bool isOpen
	{
		get
		{
			return _isOpen;
		}
		set
		{
			_isOpen = value;
		}
	}

	public bool isScoreExchange
	{
		get
		{
			return _isScoreExchange;
		}
		set
		{
			_isScoreExchange = value;
		}
	}

	public DateTime beginTime
	{
		get
		{
			return _beginTime;
		}
		set
		{
			_beginTime = value;
		}
	}

	public DateTime endTime
	{
		get
		{
			return _endTime;
		}
		set
		{
			_endTime = value;
		}
	}

	public int freeCount
	{
		get
		{
			return _freeCount;
		}
		set
		{
			_freeCount = value;
		}
	}

	public int turnCardPrice
	{
		get
		{
			return _turnCardPrice;
		}
		set
		{
			_turnCardPrice = value;
		}
	}

	public int[] revivePrice
	{
		get
		{
			return _revivePrice;
		}
		set
		{
			_revivePrice = value;
		}
	}
}
