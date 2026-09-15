using System;

namespace SqlDataProvider.Data;

public class PetEquipDataInfo : DataObject
{
	private ItemTemplateInfo _template;

	private int _ID;

	private int _userID;

	private int _petID;

	private int _eqtemplateID;

	private int _eqType;

	private DateTime _startTime;

	private int _validDate;

	private bool _isExit;

	public ItemTemplateInfo Template => _template;

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

	public int PetID
	{
		get
		{
			return _petID;
		}
		set
		{
			_petID = value;
			_isDirty = true;
		}
	}

	public int eqTemplateID
	{
		get
		{
			return _eqtemplateID;
		}
		set
		{
			_eqtemplateID = value;
			_isDirty = true;
		}
	}

	public int eqType
	{
		get
		{
			return _eqType;
		}
		set
		{
			_eqType = value;
			_isDirty = true;
		}
	}

	public DateTime startTime
	{
		get
		{
			return _startTime;
		}
		set
		{
			_startTime = value;
			_isDirty = true;
		}
	}

	public int ValidDate
	{
		get
		{
			return _validDate;
		}
		set
		{
			_validDate = value;
			_isDirty = true;
		}
	}

	public bool IsExit
	{
		get
		{
			return _isExit;
		}
		set
		{
			_isExit = value;
			_isDirty = true;
		}
	}

	public PetEquipDataInfo(ItemTemplateInfo temp)
	{
		_template = temp;
	}

	public PetEquipDataInfo addTempalte(ItemTemplateInfo Template)
	{
		PetEquipDataInfo petEquipDataInfo = new PetEquipDataInfo(Template);
		petEquipDataInfo._ID = _ID;
		petEquipDataInfo._userID = _userID;
		petEquipDataInfo._petID = _petID;
		petEquipDataInfo._eqType = _eqType;
		petEquipDataInfo._eqtemplateID = _eqtemplateID;
		petEquipDataInfo._validDate = _validDate;
		petEquipDataInfo._startTime = _startTime;
		petEquipDataInfo._isExit = _isExit;
		return petEquipDataInfo;
	}

	public bool IsValidate()
	{
		return _validDate == 0 || DateTime.Compare(_startTime.AddDays(_validDate), DateTime.Now) > 0;
	}
}
