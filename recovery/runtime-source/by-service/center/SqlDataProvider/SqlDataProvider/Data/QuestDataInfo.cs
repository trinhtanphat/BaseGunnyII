using System;

namespace SqlDataProvider.Data;

public class QuestDataInfo : DataObject
{
	private int _userID;

	private int _questID;

	private int _questLevel;

	private int _condition1;

	private int _condition2;

	private int _condition3;

	private int _condition4;

	private int _condition5;

	private int _condition6;

	private int _condition7;

	private int _condition8;

	private bool _isComplete;

	private DateTime _completeDate;

	private bool _isExist;

	private int _repeatFinish;

	private int _randDobule;

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

	public int QuestID
	{
		get
		{
			return _questID;
		}
		set
		{
			_questID = value;
			_isDirty = true;
		}
	}

	public int QuestLevel
	{
		get
		{
			return _questLevel;
		}
		set
		{
			_questLevel = value;
			_isDirty = true;
		}
	}

	public int Condition1
	{
		get
		{
			return _condition1;
		}
		set
		{
			_condition1 = value;
			_isDirty = true;
		}
	}

	public int Condition2
	{
		get
		{
			return _condition2;
		}
		set
		{
			_condition2 = value;
			_isDirty = true;
		}
	}

	public int Condition3
	{
		get
		{
			return _condition3;
		}
		set
		{
			_condition3 = value;
			_isDirty = true;
		}
	}

	public int Condition4
	{
		get
		{
			return _condition4;
		}
		set
		{
			_condition4 = value;
			_isDirty = true;
		}
	}

	public int Condition5
	{
		get
		{
			return _condition5;
		}
		set
		{
			_condition5 = value;
			_isDirty = true;
		}
	}

	public int Condition6
	{
		get
		{
			return _condition6;
		}
		set
		{
			_condition6 = value;
			_isDirty = true;
		}
	}

	public int Condition7
	{
		get
		{
			return _condition7;
		}
		set
		{
			_condition7 = value;
			_isDirty = true;
		}
	}

	public int Condition8
	{
		get
		{
			return _condition8;
		}
		set
		{
			_condition8 = value;
			_isDirty = true;
		}
	}

	public bool IsComplete
	{
		get
		{
			return _isComplete;
		}
		set
		{
			_isComplete = value;
			_isDirty = true;
		}
	}

	public DateTime CompletedDate
	{
		get
		{
			return _completeDate;
		}
		set
		{
			_completeDate = value;
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

	public int RepeatFinish
	{
		get
		{
			return _repeatFinish;
		}
		set
		{
			_repeatFinish = value;
			_isDirty = true;
		}
	}

	public int RandDobule
	{
		get
		{
			return _randDobule;
		}
		set
		{
			_randDobule = value;
			_isDirty = true;
		}
	}

	public int GetConditionValue(int index)
	{
		return index switch
		{
			0 => Condition1,
			1 => Condition2,
			2 => Condition3,
			3 => Condition4,
			4 => Condition5,
			5 => Condition6,
			6 => Condition7,
			7 => Condition8,
			_ => throw new Exception("Quest condition index out of range."),
		};
	}

	public void SaveConditionValue(int index, int value)
	{
		switch (index)
		{
		case 0:
			Condition1 = value;
			break;
		case 1:
			Condition2 = value;
			break;
		case 2:
			Condition3 = value;
			break;
		case 3:
			Condition4 = value;
			break;
		case 4:
			Condition5 = value;
			break;
		case 5:
			Condition6 = value;
			break;
		case 6:
			Condition7 = value;
			break;
		case 7:
			Condition8 = value;
			break;
		default:
			throw new Exception("Quest condition index out of range.");
		}
	}

	public int[] setProgressConcoat()
	{
		return new int[4] { Condition5, Condition6, Condition7, Condition8 };
	}
}
