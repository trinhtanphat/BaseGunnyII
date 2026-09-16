using System;

namespace SqlDataProvider.Data;

public class ItemInfo : DataObject
{
	private ItemTemplateInfo _template;

	private ItemTemplateInfo _goldEquip;

	private int _itemID;

	private int _userID;

	private int _bagType;

	private int _templateId;

	private int _place;

	private int _count;

	private bool _isJudage;

	private string _color;

	private bool _isExist;

	private int _strengthenLevel;

	private int _strengthenExp;

	private int _attackCompose;

	private int _defendCompose;

	private int _luckCompose;

	private int _agilityCompose;

	private bool _isBinds;

	private bool _isUsed;

	private string _skin;

	private DateTime _beginDate;

	private int _validDate;

	private DateTime _removeDate;

	private int _removeType;

	private int _hole1;

	private int _hole2;

	private int _hole3;

	private int _hole4;

	private int _hole5;

	private int _hole6;

	private int _strengthenTimes;

	private int _hole5Level;

	private int _hole6Level;

	private int _hole5Exp;

	private int _hole6Exp;

	private bool _isGold;

	private int _goldValidDate;

	private DateTime _goldBeginTime;

	private string _latentEnergyCurStr;

	private string _latentEnergyNewStr;

	private DateTime _latentEnergyEndTime;

	private bool _isTips;

	private bool _isLogs;

	private int _beadExp;

	private int _beadLevel;

	private bool _beadIsLock;

	private bool _isShowBind;

	private int _Damage;

	private int _Guard;

	private int _Blood;

	private int _Bless;

	private DateTime _advanceDate;

	public ItemTemplateInfo Template => _template;

	public ItemTemplateInfo GoldEquip
	{
		get
		{
			return _goldEquip;
		}
		set
		{
			_goldEquip = value;
			_isDirty = true;
		}
	}

	public int ItemID
	{
		get
		{
			return _itemID;
		}
		set
		{
			_itemID = value;
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

	public int BagType
	{
		get
		{
			return _bagType;
		}
		set
		{
			_bagType = value;
			_isDirty = true;
		}
	}

	public int TemplateID
	{
		get
		{
			if (IsGold && GoldEquip != null)
			{
				return GoldEquip.TemplateID;
			}
			return _templateId;
		}
		set
		{
			_templateId = value;
			_isDirty = true;
		}
	}

	public int Place
	{
		get
		{
			return _place;
		}
		set
		{
			_place = value;
			_isDirty = true;
		}
	}

	public int Count
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

	public bool IsJudge
	{
		get
		{
			return _isJudage;
		}
		set
		{
			_isJudage = value;
			_isDirty = true;
		}
	}

	public string Color
	{
		get
		{
			return _color;
		}
		set
		{
			_color = value;
			_isDirty = true;
		}
	}

	public bool IsExist
	{
		get
		{
			return _isExist;
		}
		set
		{
			_isExist = value;
			_isDirty = true;
		}
	}

	public int StrengthenLevel
	{
		get
		{
			return _strengthenLevel;
		}
		set
		{
			_strengthenLevel = value;
			_isDirty = true;
		}
	}

	public int StrengthenExp
	{
		get
		{
			return _strengthenExp;
		}
		set
		{
			_strengthenExp = value;
			_isDirty = true;
		}
	}

	public int AttackCompose
	{
		get
		{
			return _attackCompose;
		}
		set
		{
			_attackCompose = value;
			_isDirty = true;
		}
	}

	public int DefendCompose
	{
		get
		{
			return _defendCompose;
		}
		set
		{
			_defendCompose = value;
			_isDirty = true;
		}
	}

	public int LuckCompose
	{
		get
		{
			return _luckCompose;
		}
		set
		{
			_luckCompose = value;
			_isDirty = true;
		}
	}

	public int AgilityCompose
	{
		get
		{
			return _agilityCompose;
		}
		set
		{
			_agilityCompose = value;
			_isDirty = true;
		}
	}

	public bool IsBinds
	{
		get
		{
			return _isBinds;
		}
		set
		{
			_isBinds = value;
			_isDirty = true;
		}
	}

	public bool IsUsed
	{
		get
		{
			return _isUsed;
		}
		set
		{
			if (_isUsed != value)
			{
				_isUsed = value;
				_isDirty = true;
			}
		}
	}

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

	public DateTime BeginDate
	{
		get
		{
			return _beginDate;
		}
		set
		{
			_beginDate = value;
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
			_validDate = ((value > 999) ? 365 : value);
			_isDirty = true;
		}
	}

	public DateTime RemoveDate
	{
		get
		{
			return _removeDate;
		}
		set
		{
			_removeDate = value;
			_isDirty = true;
		}
	}

	public int RemoveType
	{
		get
		{
			return _removeType;
		}
		set
		{
			_removeType = value;
			_removeDate = DateTime.Now;
			_isDirty = true;
		}
	}

	public int Hole1
	{
		get
		{
			return _hole1;
		}
		set
		{
			_hole1 = value;
			_isDirty = true;
		}
	}

	public int Hole2
	{
		get
		{
			return _hole2;
		}
		set
		{
			_hole2 = value;
			_isDirty = true;
		}
	}

	public int Hole3
	{
		get
		{
			return _hole3;
		}
		set
		{
			_hole3 = value;
			_isDirty = true;
		}
	}

	public int Hole4
	{
		get
		{
			return _hole4;
		}
		set
		{
			_hole4 = value;
			_isDirty = true;
		}
	}

	public int Hole5
	{
		get
		{
			return _hole5;
		}
		set
		{
			_hole5 = value;
			_isDirty = true;
		}
	}

	public int Hole6
	{
		get
		{
			return _hole6;
		}
		set
		{
			_hole6 = value;
			_isDirty = true;
		}
	}

	public int StrengthenTimes
	{
		get
		{
			return _strengthenTimes;
		}
		set
		{
			_strengthenTimes = value;
			_isDirty = true;
		}
	}

	public int Hole5Level
	{
		get
		{
			return _hole5Level;
		}
		set
		{
			_hole5Level = value;
			_isDirty = true;
		}
	}

	public int Hole6Level
	{
		get
		{
			return _hole6Level;
		}
		set
		{
			_hole6Level = value;
			_isDirty = true;
		}
	}

	public int Hole5Exp
	{
		get
		{
			return _hole5Exp;
		}
		set
		{
			_hole5Exp = value;
			_isDirty = true;
		}
	}

	public int Hole6Exp
	{
		get
		{
			return _hole6Exp;
		}
		set
		{
			_hole6Exp = value;
			_isDirty = true;
		}
	}

	public bool IsGold => IsValidGoldItem();

	public int goldValidDate
	{
		get
		{
			return _goldValidDate;
		}
		set
		{
			_goldValidDate = value;
			_isDirty = true;
		}
	}

	public DateTime goldBeginTime
	{
		get
		{
			return _goldBeginTime;
		}
		set
		{
			_goldBeginTime = value;
			_isDirty = true;
		}
	}

	public string latentEnergyCurStr
	{
		get
		{
			return _latentEnergyCurStr;
		}
		set
		{
			_latentEnergyCurStr = value;
			_isDirty = true;
		}
	}

	public string latentEnergyNewStr
	{
		get
		{
			return _latentEnergyNewStr;
		}
		set
		{
			_latentEnergyNewStr = value;
			_isDirty = true;
		}
	}

	public DateTime latentEnergyEndTime
	{
		get
		{
			return _latentEnergyEndTime;
		}
		set
		{
			_latentEnergyEndTime = value;
			_isDirty = true;
		}
	}

	public string Pic
	{
		get
		{
			if (IsGold && GoldEquip != null)
			{
				return GoldEquip.Pic;
			}
			return _template.Pic;
		}
	}

	public int RefineryLevel
	{
		get
		{
			if (IsGold && GoldEquip != null)
			{
				return GoldEquip.RefineryLevel;
			}
			return _template.RefineryLevel;
		}
	}

	public int Attack
	{
		get
		{
			int attack = _template.Attack;
			if (!IsGold || GoldEquip == null)
			{
				return _attackCompose + attack;
			}
			if (GoldEquip.Attack <= attack)
			{
				return attack;
			}
			return GoldEquip.Attack;
		}
	}

	public int Defence
	{
		get
		{
			int defence = _template.Defence;
			if (!IsGold || GoldEquip == null)
			{
				return _defendCompose + defence;
			}
			if (GoldEquip.Defence <= defence)
			{
				return defence;
			}
			return GoldEquip.Defence;
		}
	}

	public int Agility
	{
		get
		{
			int agility = _template.Agility;
			if (!IsGold || GoldEquip == null)
			{
				return _agilityCompose + agility;
			}
			if (GoldEquip.Agility <= agility)
			{
				return agility;
			}
			return GoldEquip.Agility;
		}
	}

	public int Luck
	{
		get
		{
			int luck = _template.Luck;
			if (!IsGold || GoldEquip == null)
			{
				return _luckCompose + luck;
			}
			if (GoldEquip.Luck <= luck)
			{
				return luck;
			}
			return GoldEquip.Luck;
		}
	}

	public bool IsTips
	{
		get
		{
			return _isTips;
		}
		set
		{
			_isTips = value;
		}
	}

	public bool IsLogs
	{
		get
		{
			return _isLogs;
		}
		set
		{
			_isLogs = value;
		}
	}

	public int beadExp
	{
		get
		{
			return _beadExp;
		}
		set
		{
			_beadExp = value;
			_isDirty = true;
		}
	}

	public int beadLevel
	{
		get
		{
			return _beadLevel;
		}
		set
		{
			_beadLevel = value;
			_isDirty = true;
		}
	}

	public bool beadIsLock
	{
		get
		{
			return _beadIsLock;
		}
		set
		{
			_beadIsLock = value;
			_isDirty = true;
		}
	}

	public bool isShowBind
	{
		get
		{
			return _isShowBind;
		}
		set
		{
			_isShowBind = value;
			_isDirty = true;
		}
	}

	public int Damage
	{
		get
		{
			return _Damage;
		}
		set
		{
			_Damage = value;
			_isDirty = true;
		}
	}

	public int Guard
	{
		get
		{
			return _Guard;
		}
		set
		{
			_Guard = value;
			_isDirty = true;
		}
	}

	public int Blood
	{
		get
		{
			return _Blood;
		}
		set
		{
			_Blood = value;
			_isDirty = true;
		}
	}

	public int Bless
	{
		get
		{
			return _Bless;
		}
		set
		{
			_Bless = value;
			_isDirty = true;
		}
	}

	public DateTime AdvanceDate
	{
		get
		{
			return _advanceDate;
		}
		set
		{
			_advanceDate = value;
			_isDirty = true;
		}
	}

	public int GetBagType => (int)_template.BagType;

	public ItemInfo(ItemTemplateInfo temp)
	{
		_template = temp;
	}

	public bool IsAdvanceDate()
	{
		return _advanceDate.Date < DateTime.Now.Date;
	}

	public ItemInfo Clone()
	{
		ItemInfo itemInfo = new ItemInfo(_template);
		itemInfo._userID = _userID;
		itemInfo._validDate = _validDate;
		itemInfo._templateId = _templateId;
		itemInfo._strengthenLevel = _strengthenLevel;
		itemInfo._strengthenExp = _strengthenExp;
		itemInfo._luckCompose = _luckCompose;
		itemInfo._itemID = 0;
		itemInfo._isJudage = _isJudage;
		itemInfo._isExist = _isExist;
		itemInfo._isBinds = _isBinds;
		itemInfo._isUsed = _isUsed;
		itemInfo._defendCompose = _defendCompose;
		itemInfo._count = _count;
		itemInfo._color = _color;
		itemInfo.Skin = _skin;
		itemInfo._beginDate = _beginDate;
		itemInfo._attackCompose = _attackCompose;
		itemInfo._agilityCompose = _agilityCompose;
		itemInfo._bagType = _bagType;
		itemInfo._isDirty = true;
		itemInfo._removeDate = _removeDate;
		itemInfo._removeType = _removeType;
		itemInfo._hole1 = _hole1;
		itemInfo._hole2 = _hole2;
		itemInfo._hole3 = _hole3;
		itemInfo._hole4 = _hole4;
		itemInfo._hole5 = _hole5;
		itemInfo._hole6 = _hole6;
		itemInfo._hole5Exp = _hole5Exp;
		itemInfo._hole5Level = _hole5Level;
		itemInfo._hole6Exp = _hole6Exp;
		itemInfo._hole6Level = _hole6Level;
		itemInfo._isGold = _isGold;
		itemInfo._goldBeginTime = _goldBeginTime;
		itemInfo._goldValidDate = _goldValidDate;
		itemInfo._latentEnergyCurStr = _latentEnergyCurStr;
		itemInfo._latentEnergyNewStr = _latentEnergyNewStr;
		itemInfo._latentEnergyEndTime = _latentEnergyEndTime;
		itemInfo._beadExp = _beadExp;
		itemInfo._beadLevel = _beadLevel;
		itemInfo._beadIsLock = _beadIsLock;
		itemInfo._isShowBind = _isShowBind;
		itemInfo._Damage = _Damage;
		itemInfo._Guard = _Guard;
		itemInfo._Bless = _Bless;
		itemInfo._Blood = _Blood;
		itemInfo.AdvanceDate = _advanceDate;
		return itemInfo;
	}

	public void Copy(ItemInfo item)
	{
		_userID = item.UserID;
		_validDate = item.ValidDate;
		_templateId = item.TemplateID;
		_strengthenLevel = item.StrengthenLevel;
		_strengthenExp = item.StrengthenExp;
		_luckCompose = item.LuckCompose;
		_itemID = 0;
		_isJudage = item.IsJudge;
		_isExist = item.IsExist;
		_isBinds = item.IsBinds;
		_isUsed = item.IsUsed;
		_defendCompose = item.DefendCompose;
		_count = item.Count;
		_color = item.Color;
		_skin = item.Skin;
		_beginDate = item.BeginDate;
		_attackCompose = item.AttackCompose;
		_agilityCompose = item.AgilityCompose;
		_bagType = item.BagType;
		_isDirty = item.IsDirty;
		_removeDate = item.RemoveDate;
		_removeType = item.RemoveType;
		_hole1 = item.Hole1;
		_hole2 = item.Hole2;
		_hole3 = item.Hole3;
		_hole4 = item.Hole4;
		_hole5 = item.Hole5;
		_hole6 = item.Hole6;
		_hole5Exp = item.Hole5Exp;
		_hole5Level = item.Hole5Level;
		_hole6Exp = item.Hole6Exp;
		_hole6Level = item.Hole6Level;
		_isGold = item.IsGold;
		_goldBeginTime = item.goldBeginTime;
		_goldValidDate = item.goldValidDate;
		_strengthenExp = item.StrengthenExp;
		_latentEnergyCurStr = item.latentEnergyCurStr;
		_latentEnergyNewStr = item._latentEnergyNewStr;
		_latentEnergyEndTime = item._latentEnergyEndTime;
		_beadExp = item.beadExp;
		_beadLevel = item.beadLevel;
		_beadIsLock = item.beadIsLock;
		_isShowBind = item.isShowBind;
		_Damage = item.Damage;
		_Guard = item.Guard;
		_Bless = item.Bless;
		_Blood = item.Blood;
		_advanceDate = item.AdvanceDate;
	}

	public bool IsValidItem()
	{
		return _validDate == 0 || !_isUsed || DateTime.Compare(_beginDate.AddDays(_validDate), DateTime.Now) > 0;
	}

	public bool IsValidGoldItem()
	{
		return _goldValidDate > 0 && DateTime.Compare(_goldBeginTime.AddDays(_goldValidDate), DateTime.Now) > 0;
	}

	public bool IsValidLatentEnergy()
	{
		return _latentEnergyEndTime.Date < DateTime.Now.Date;
	}

	public void ResetLatentEnergy()
	{
		_latentEnergyCurStr = "0,0,0,0";
		_latentEnergyNewStr = "0,0,0,0";
	}

	public bool CanLatentEnergy()
	{
		switch (Template.CategoryID)
		{
		case 5:
			return false;
		case 14:
			return false;
		default:
			return false;
		case 2:
		case 3:
		case 4:
		case 6:
		case 13:
		case 15:
			return true;
		}
	}

	public bool CanStackedTo(ItemInfo to)
	{
		return _templateId == to.TemplateID && Template.MaxCount > 1 && _isBinds == to.IsBinds && _isUsed == to._isUsed && (ValidDate == 0 || (BeginDate.Date == to.BeginDate.Date && ValidDate == ValidDate));
	}

	public int eqType()
	{
		return _template.CategoryID switch
		{
			51 => 1,
			52 => 2,
			_ => 0,
		};
	}

	public bool IsProp()
	{
		int categoryID = _template.CategoryID;
		if (categoryID <= 18)
		{
			if (categoryID != 11 && categoryID != 18)
			{
				return false;
			}
		}
		else
		{
			switch (categoryID)
			{
			case 33:
				return false;
			default:
				return false;
			case 32:
			case 34:
			case 35:
			case 40:
				break;
			}
		}
		return true;
	}

	public bool CanEquip()
	{
		return _template.CategoryID < 10 || (_template.CategoryID >= 13 && _template.CategoryID <= 16);
	}

	public string GetBagName()
	{
		switch (_template.CategoryID)
		{
		case 10:
		case 11:
			return "Game.Server.GameObjects.Prop";
		case 12:
			return "Game.Server.GameObjects.Task";
		default:
			return "Game.Server.GameObjects.Equip";
		}
	}

	public static ItemInfo CreateFromTemplate(ItemTemplateInfo goods, int count, int type)
	{
		if (goods == null)
		{
			return null;
		}
		ItemInfo itemInfo = new ItemInfo(goods);
		itemInfo.AgilityCompose = 0;
		itemInfo.AttackCompose = 0;
		itemInfo.BeginDate = DateTime.Now;
		itemInfo.Color = "";
		itemInfo.Skin = "";
		itemInfo.DefendCompose = 0;
		itemInfo.IsUsed = false;
		itemInfo.IsDirty = false;
		itemInfo.IsExist = true;
		itemInfo.IsJudge = true;
		itemInfo.LuckCompose = 0;
		itemInfo.StrengthenLevel = 0;
		itemInfo.TemplateID = goods.TemplateID;
		itemInfo.ValidDate = 0;
		itemInfo.Count = count;
		itemInfo.IsBinds = goods.BindType == 1;
		itemInfo._removeDate = DateTime.Now;
		itemInfo._removeType = type;
		itemInfo.Hole1 = -1;
		itemInfo.Hole2 = -1;
		itemInfo.Hole3 = -1;
		itemInfo.Hole4 = -1;
		itemInfo.Hole5 = -1;
		itemInfo.Hole6 = -1;
		itemInfo.Hole5Exp = 0;
		itemInfo.Hole5Level = 0;
		itemInfo.Hole6Exp = 0;
		itemInfo.Hole6Level = 0;
		itemInfo.goldValidDate = 0;
		itemInfo.goldBeginTime = DateTime.Now;
		itemInfo.StrengthenExp = 0;
		itemInfo.latentEnergyCurStr = "0,0,0,0";
		itemInfo.latentEnergyNewStr = "0,0,0,0";
		itemInfo.latentEnergyEndTime = DateTime.Now;
		itemInfo.beadExp = 0;
		itemInfo.beadLevel = 0;
		itemInfo.beadIsLock = false;
		itemInfo.isShowBind = false;
		itemInfo.Damage = 0;
		itemInfo.Guard = 0;
		itemInfo.Bless = 0;
		itemInfo.Blood = 0;
		itemInfo.AdvanceDate = DateTime.Now;
		return itemInfo;
	}

	public static ItemInfo CloneFromTemplate(ItemTemplateInfo goods, ItemInfo item)
	{
		if (goods == null)
		{
			return null;
		}
		ItemInfo item2 = new ItemInfo(goods);
		item2.AgilityCompose = item.AgilityCompose;
		item2.AttackCompose = item.AttackCompose;
		item2.BeginDate = item.BeginDate;
		item2.Color = item.Color;
		item2.Skin = item.Skin;
		item2.DefendCompose = item.DefendCompose;
		item2.IsBinds = item.IsBinds;
		item2.Place = item.Place;
		item2.BagType = item.BagType;
		item2.IsUsed = item.IsUsed;
		item2.IsDirty = item.IsDirty;
		item2.IsExist = item.IsExist;
		item2.IsJudge = item.IsJudge;
		item2.LuckCompose = item.LuckCompose;
		item2.StrengthenExp = item.StrengthenExp;
		item2.StrengthenLevel = item.StrengthenLevel;
		item2.TemplateID = goods.TemplateID;
		item2.ValidDate = item.ValidDate;
		item2._template = goods;
		item2.Count = item.Count;
		item2._removeDate = item._removeDate;
		item2._removeType = item._removeType;
		item2.Hole1 = item.Hole1;
		item2.Hole2 = item.Hole2;
		item2.Hole3 = item.Hole3;
		item2.Hole4 = item.Hole4;
		item2.Hole5 = item.Hole5;
		item2.Hole6 = item.Hole6;
		item2.Hole5Level = item.Hole5Level;
		item2.Hole5Exp = item.Hole5Exp;
		item2.Hole6Level = item.Hole6Level;
		item2.Hole6Exp = item.Hole6Exp;
		item2.goldBeginTime = item.goldBeginTime;
		item2.goldValidDate = item.goldValidDate;
		item2.latentEnergyEndTime = item.latentEnergyEndTime;
		item2.latentEnergyCurStr = item.latentEnergyCurStr;
		item2.latentEnergyNewStr = item.latentEnergyNewStr;
		item2.AdvanceDate = item.AdvanceDate;
		OpenHole(ref item2);
		return item2;
	}

	public bool IsBead()
	{
		return _template.Property1 == 31 && _template.CategoryID == 11;
	}

	public bool IsEquipPet()
	{
		return _template.CategoryID == 50 || _template.CategoryID == 51 || _template.CategoryID == 52;
	}

	public bool IsCard()
	{
		int categoryID = _template.CategoryID;
		if (categoryID != 11)
		{
			return categoryID == 18;
		}
		return _template.TemplateID == 112108 || _template.TemplateID == 112150;
	}

	public static void FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken, ref int medal)
	{
		switch (info.TemplateID)
		{
		case -100:
			gold += info.Count;
			break;
		case -200:
			money += info.Count;
			break;
		case -300:
			giftToken += info.Count;
			break;
		case 11408:
			medal += info.Count;
			break;
		}
	}

	public static void OpenHole(ref ItemInfo item)
	{
		string[] array = item.Template.Hole.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(',');
			if (item.StrengthenLevel < Convert.ToInt32(array2[0]) || Convert.ToInt32(array2[1]) == -1)
			{
				continue;
			}
			switch (i)
			{
			case 0:
				if (item.Hole1 < 0)
				{
					item.Hole1 = 0;
				}
				break;
			case 1:
				if (item.Hole2 < 0)
				{
					item.Hole2 = 0;
				}
				break;
			case 2:
				if (item.Hole3 < 0)
				{
					item.Hole3 = 0;
				}
				break;
			case 3:
				if (item.Hole4 < 0)
				{
					item.Hole4 = 0;
				}
				break;
			case 4:
				if (item.Hole5 < 0)
				{
					item.Hole5 = 0;
				}
				break;
			case 5:
				if (item.Hole6 < 0)
				{
					item.Hole6 = 0;
				}
				break;
			}
		}
	}
}
