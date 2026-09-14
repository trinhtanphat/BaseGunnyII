using System;

namespace SqlDataProvider.Data;

public class UserFarmInfo : DataObject
{
	private UserFieldInfo _field;

	private int _ID;

	private int _farmID;

	private string _payFieldMoney;

	private string _payAutoMoney;

	private DateTime _autoPayTime;

	private int _autoValidDate;

	private int _vipLimitLevel;

	private string _farmerName;

	private int _gainFieldId;

	private int _matureId;

	private int _killCropId;

	private int _isAutoId;

	private bool _isFarmHelper;

	private int _buyExpRemainNum;

	private bool _isArrange;

	public UserFieldInfo Field
	{
		get
		{
			return _field;
		}
		set
		{
			_field = value;
			_isDirty = true;
		}
	}

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

	public int FarmID
	{
		get
		{
			return _farmID;
		}
		set
		{
			_farmID = value;
			_isDirty = true;
		}
	}

	public string PayFieldMoney
	{
		get
		{
			return _payFieldMoney;
		}
		set
		{
			_payFieldMoney = value;
			_isDirty = true;
		}
	}

	public string PayAutoMoney
	{
		get
		{
			return _payAutoMoney;
		}
		set
		{
			_payAutoMoney = value;
			_isDirty = true;
		}
	}

	public DateTime AutoPayTime
	{
		get
		{
			return _autoPayTime;
		}
		set
		{
			_autoPayTime = value;
			_isDirty = true;
		}
	}

	public int AutoValidDate
	{
		get
		{
			return _autoValidDate;
		}
		set
		{
			_autoValidDate = value;
			_isDirty = true;
		}
	}

	public int VipLimitLevel
	{
		get
		{
			return _vipLimitLevel;
		}
		set
		{
			_vipLimitLevel = value;
			_isDirty = true;
		}
	}

	public string FarmerName
	{
		get
		{
			return _farmerName;
		}
		set
		{
			_farmerName = value;
			_isDirty = true;
		}
	}

	public int GainFieldId
	{
		get
		{
			return _gainFieldId;
		}
		set
		{
			_gainFieldId = value;
			_isDirty = true;
		}
	}

	public int MatureId
	{
		get
		{
			return _matureId;
		}
		set
		{
			_matureId = value;
			_isDirty = true;
		}
	}

	public int KillCropId
	{
		get
		{
			return _killCropId;
		}
		set
		{
			_killCropId = value;
			_isDirty = true;
		}
	}

	public int isAutoId
	{
		get
		{
			return _isAutoId;
		}
		set
		{
			_isAutoId = value;
			_isDirty = true;
		}
	}

	public bool isFarmHelper
	{
		get
		{
			return _isFarmHelper;
		}
		set
		{
			_isFarmHelper = value;
			_isDirty = true;
		}
	}

	public int buyExpRemainNum
	{
		get
		{
			return _buyExpRemainNum;
		}
		set
		{
			_buyExpRemainNum = value;
			_isDirty = true;
		}
	}

	public bool isArrange
	{
		get
		{
			return _isArrange;
		}
		set
		{
			_isArrange = value;
			_isDirty = true;
		}
	}
}
