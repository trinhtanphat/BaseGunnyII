namespace SqlDataProvider.Data
{
public class PyramidInfo : DataObject
{
	private int _ID;

	private int _userID;

	private int _currentLayer;

	private int _maxLayer;

	private int _totalPoint;

	private int _turnPoint;

	private int _pointRatio;

	private int _currentFreeCount;

	private bool _isPyramidStart;

	private string _LayerItems;

	private int _currentReviveCount;

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

	public int currentLayer
	{
		get
		{
			return _currentLayer;
		}
		set
		{
			_currentLayer = value;
			_isDirty = true;
		}
	}

	public int maxLayer
	{
		get
		{
			return _maxLayer;
		}
		set
		{
			_maxLayer = value;
			_isDirty = true;
		}
	}

	public int totalPoint
	{
		get
		{
			return _totalPoint;
		}
		set
		{
			_totalPoint = value;
			_isDirty = true;
		}
	}

	public int turnPoint
	{
		get
		{
			return _turnPoint;
		}
		set
		{
			_turnPoint = value;
			_isDirty = true;
		}
	}

	public int pointRatio
	{
		get
		{
			return _pointRatio;
		}
		set
		{
			_pointRatio = value;
			_isDirty = true;
		}
	}

	public int currentFreeCount
	{
		get
		{
			return _currentFreeCount;
		}
		set
		{
			_currentFreeCount = value;
			_isDirty = true;
		}
	}

	public bool isPyramidStart
	{
		get
		{
			return _isPyramidStart;
		}
		set
		{
			_isPyramidStart = value;
			_isDirty = true;
		}
	}

	public string LayerItems
	{
		get
		{
			return _LayerItems;
		}
		set
		{
			_LayerItems = value;
			_isDirty = true;
		}
	}

	public int currentReviveCount
	{
		get
		{
			return _currentReviveCount;
		}
		set
		{
			_currentReviveCount = value;
			_isDirty = true;
		}
	}
}

}
