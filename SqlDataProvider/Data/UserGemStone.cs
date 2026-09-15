namespace SqlDataProvider.Data
{
public class UserGemStone : DataObject
{
	private int _ID;

	private int _userID;

	private int _figSpiritId;

	private string _figSpiritIdValue;

	private int _equipPlace;

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

	public int FigSpiritId
	{
		get
		{
			return _figSpiritId;
		}
		set
		{
			_figSpiritId = value;
			_isDirty = true;
		}
	}

	public string FigSpiritIdValue
	{
		get
		{
			return _figSpiritIdValue;
		}
		set
		{
			_figSpiritIdValue = value;
			_isDirty = true;
		}
	}

	public int EquipPlace
	{
		get
		{
			return _equipPlace;
		}
		set
		{
			_equipPlace = value;
			_isDirty = true;
		}
	}
}

}
