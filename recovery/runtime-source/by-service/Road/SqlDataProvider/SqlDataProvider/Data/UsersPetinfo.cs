using System.Collections.Generic;

namespace SqlDataProvider.Data;

public class UsersPetinfo : DataObject
{
	private List<PetEquipDataInfo> m_peEquip;

	private string _skillEquip;

	private string _skill;

	private int _ID;

	private int _petID;

	private int _templateID;

	private string _name;

	private int _userID;

	private int _attack;

	private int _defence;

	private int _luck;

	private int _agility;

	private int _blood;

	private int _damage;

	private int _guard;

	private int _attackGrow;

	private int _defenceGrow;

	private int _luckGrow;

	private int _agilityGrow;

	private int _bloodGrow;

	private int _damageGrow;

	private int _guardGrow;

	private int _level;

	private int _gp;

	private int _maxGP;

	private int _hunger;

	private int _mp;

	private bool _isEquip;

	private int _place;

	private bool _isExit;

	public List<PetEquipDataInfo> EquipList
	{
		get
		{
			return m_peEquip;
		}
		set
		{
			m_peEquip = value;
		}
	}

	public string SkillEquip
	{
		get
		{
			return _skillEquip;
		}
		set
		{
			_skillEquip = value;
			_isDirty = true;
		}
	}

	public string Skill
	{
		get
		{
			return _skill;
		}
		set
		{
			_skill = value;
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

	public int TemplateID
	{
		get
		{
			return _templateID;
		}
		set
		{
			_templateID = value;
			_isDirty = true;
		}
	}

	public string Name
	{
		get
		{
			return _name;
		}
		set
		{
			_name = value;
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

	public int Blood
	{
		get
		{
			return _blood;
		}
		set
		{
			_blood = value;
			_isDirty = true;
		}
	}

	public int Damage
	{
		get
		{
			return _damage;
		}
		set
		{
			_damage = value;
			_isDirty = true;
		}
	}

	public int Guard
	{
		get
		{
			return _guard;
		}
		set
		{
			_guard = value;
			_isDirty = true;
		}
	}

	public int AttackGrow
	{
		get
		{
			return _attackGrow;
		}
		set
		{
			_attackGrow = value;
			_isDirty = true;
		}
	}

	public int DefenceGrow
	{
		get
		{
			return _defenceGrow;
		}
		set
		{
			_defenceGrow = value;
			_isDirty = true;
		}
	}

	public int LuckGrow
	{
		get
		{
			return _luckGrow;
		}
		set
		{
			_luckGrow = value;
			_isDirty = true;
		}
	}

	public int AgilityGrow
	{
		get
		{
			return _agilityGrow;
		}
		set
		{
			_agilityGrow = value;
			_isDirty = true;
		}
	}

	public int BloodGrow
	{
		get
		{
			return _bloodGrow;
		}
		set
		{
			_bloodGrow = value;
			_isDirty = true;
		}
	}

	public int DamageGrow
	{
		get
		{
			return _damageGrow;
		}
		set
		{
			_damageGrow = value;
			_isDirty = true;
		}
	}

	public int GuardGrow
	{
		get
		{
			return _guardGrow;
		}
		set
		{
			_guardGrow = value;
			_isDirty = true;
		}
	}

	public int Level
	{
		get
		{
			return _level;
		}
		set
		{
			_level = value;
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

	public int MaxGP
	{
		get
		{
			return _maxGP;
		}
		set
		{
			_maxGP = value;
			_isDirty = true;
		}
	}

	public int Hunger
	{
		get
		{
			return _hunger;
		}
		set
		{
			_hunger = value;
			_isDirty = true;
		}
	}

	public int PetHappyStar => happyPercent();

	public int MP
	{
		get
		{
			return _mp;
		}
		set
		{
			_mp = value;
			_isDirty = true;
		}
	}

	public bool IsEquip
	{
		get
		{
			return _isEquip;
		}
		set
		{
			_isEquip = value;
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

	public int TotalAttack
	{
		get
		{
			int num = 0;
			int i = 0;
			if (m_peEquip != null)
			{
				for (; i < m_peEquip.Count; i++)
				{
					ItemTemplateInfo template = m_peEquip[i].Template;
					if (template != null)
					{
						num += template.Attack;
					}
				}
			}
			return _attack - ReduceProp(_attack) + num;
		}
	}

	public int TotalDefence
	{
		get
		{
			int num = 0;
			int i = 0;
			if (m_peEquip != null)
			{
				for (; i < m_peEquip.Count; i++)
				{
					ItemTemplateInfo template = m_peEquip[i].Template;
					if (template != null)
					{
						num += template.Defence;
					}
				}
			}
			return _defence - ReduceProp(_defence) + num;
		}
	}

	public int TotalLuck
	{
		get
		{
			int num = 0;
			int i = 0;
			if (m_peEquip != null)
			{
				for (; i < m_peEquip.Count; i++)
				{
					ItemTemplateInfo template = m_peEquip[i].Template;
					if (template != null)
					{
						num += template.Luck;
					}
				}
			}
			return _luck - ReduceProp(_luck) + num;
		}
	}

	public int TotalAgility
	{
		get
		{
			int num = 0;
			int i = 0;
			if (m_peEquip != null)
			{
				for (; i < m_peEquip.Count; i++)
				{
					ItemTemplateInfo template = m_peEquip[i].Template;
					if (template != null)
					{
						num += template.Agility;
					}
				}
			}
			return _agility - ReduceProp(_agility) + num;
		}
	}

	public int TotalBlood => _blood - ReduceProp(_blood);

	public int TotalDamage => _damage - ReduceProp(_damage);

	public int TotalGuard => _guard - ReduceProp(_guard);

	public List<PetEquipDataInfo> GetEquip()
	{
		List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
		if (m_peEquip == null)
		{
			return list;
		}
		int num = 0;
		for (int i = 0; i < m_peEquip.Count; i++)
		{
			if (m_peEquip[i].IsValidate())
			{
				list.Add(m_peEquip[i]);
				num++;
			}
			else
			{
				m_peEquip[i].eqTemplateID = -1;
				m_peEquip[i].ValidDate = 7;
			}
		}
		return list;
	}

	public List<string> GetSkill()
	{
		List<string> list = new List<string>();
		string[] array = _skill.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		return list;
	}

	public List<string> GetSkillEquip()
	{
		List<string> list = new List<string>();
		string[] array = _skillEquip.Split('|');
		for (int i = 0; i < array.Length; i++)
		{
			list.Add(array[i]);
		}
		return list;
	}

	private int happyPercent()
	{
		double num = (double)_hunger / 10000.0 * 100.0;
		int result = 0;
		if (num >= 80.0)
		{
			result = 3;
		}
		if (num < 80.0 && num >= 60.0)
		{
			result = 2;
		}
		if (num < 60.0 && num > 0.0)
		{
			result = 1;
		}
		return result;
	}

	private int ReduceProp(int val)
	{
		if (happyPercent() == 2)
		{
			return val * 20 / 100;
		}
		if (happyPercent() == 1)
		{
			return val * 40 / 100;
		}
		return 0;
	}

	public static PetType GetPetType(int Id)
	{
		switch (Id)
		{
		case 1:
			return PetType.FORZEN;
		case 3:
			return PetType.TRANFORM;
		case 5:
		case 59:
		case 64:
		case 97:
		case 98:
			return PetType.CURE;
		default:
			return PetType.Normal;
		}
	}
}
