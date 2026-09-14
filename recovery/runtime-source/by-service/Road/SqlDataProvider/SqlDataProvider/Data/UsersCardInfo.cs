namespace SqlDataProvider.Data;

public class UsersCardInfo : DataObject
{
	private int _cardID;

	private int _cardType;

	private int _userID;

	private int _place;

	private int _type;

	private int _templateID;

	private bool _isFirstGet;

	private int _attack;

	private int _defence;

	private int _luck;

	private int _agility;

	private int _damage;

	private int _guard;

	private bool _isExit;

	private int _level;

	private int _cardGp;

	public int CardID
	{
		get
		{
			return _cardID;
		}
		set
		{
			_cardID = value;
			_isDirty = true;
		}
	}

	public int CardType
	{
		get
		{
			return _cardType;
		}
		set
		{
			_cardType = value;
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

	public int Type
	{
		get
		{
			return _type;
		}
		set
		{
			_type = value;
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

	public bool isFirstGet
	{
		get
		{
			return _isFirstGet;
		}
		set
		{
			_isFirstGet = value;
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

	public int CardGP
	{
		get
		{
			return _cardGp;
		}
		set
		{
			_cardGp = value;
			_isDirty = true;
		}
	}
}
