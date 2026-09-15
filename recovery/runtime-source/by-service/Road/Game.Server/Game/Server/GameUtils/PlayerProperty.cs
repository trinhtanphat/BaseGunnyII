using System.Collections.Generic;
using System.Reflection;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public class PlayerProperty
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected GamePlayer m_player;

	private Dictionary<string, Dictionary<string, int>> m_playerProperty;

	private Dictionary<string, Dictionary<string, int>> m_otherPlayerProperty;

	protected int m_loading;

	protected int m_totalDamage;

	protected int m_totalArmor;

	public GamePlayer Player => m_player;

	public Dictionary<string, Dictionary<string, int>> Current
	{
		get
		{
			return m_playerProperty;
		}
		set
		{
			m_playerProperty = value;
		}
	}

	public Dictionary<string, Dictionary<string, int>> OtherPlayerProperty
	{
		get
		{
			return m_playerProperty;
		}
		set
		{
			m_playerProperty = value;
		}
	}

	public int Loading
	{
		get
		{
			return m_loading;
		}
		set
		{
			m_loading = value;
		}
	}

	public int totalDamage
	{
		get
		{
			return m_totalDamage;
		}
		set
		{
			m_totalDamage = value;
		}
	}

	public int totalArmor
	{
		get
		{
			return m_totalArmor;
		}
		set
		{
			m_totalArmor = value;
		}
	}

	public PlayerProperty(GamePlayer player)
	{
		m_player = player;
		m_playerProperty = new Dictionary<string, Dictionary<string, int>>();
		m_otherPlayerProperty = new Dictionary<string, Dictionary<string, int>>();
		m_loading = 0;
		m_totalDamage = 0;
		m_totalArmor = 0;
		CreateProp(isSelf: true, "Texp", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Card", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Pet", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Suit", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Gem", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Bead", 0, 0, 0, 0, 0);
		CreateBaseProp(isSelf: true, 0.0, 0.0, 0.0, 0.0);
		CreateProp(isSelf: false, "Texp", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Card", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Pet", 0, 0, 0, 0, 0);
		CreateProp(isSelf: true, "Suit", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Gem", 0, 0, 0, 0, 0);
		CreateProp(isSelf: false, "Bead", 0, 0, 0, 0, 0);
		CreateBaseProp(isSelf: false, 0.0, 0.0, 0.0, 0.0);
	}

	public void AddProp(string key, Dictionary<string, int> propAdd)
	{
		if (!m_playerProperty.ContainsKey(key))
		{
			m_playerProperty.Add(key, propAdd);
		}
		else
		{
			m_playerProperty[key] = propAdd;
		}
	}

	public void AddOtherProp(string key, Dictionary<string, int> propAdd)
	{
		if (!m_playerProperty.ContainsKey(key))
		{
			m_otherPlayerProperty.Add(key, propAdd);
		}
		else
		{
			m_otherPlayerProperty[key] = propAdd;
		}
	}

	public void ViewCurrent()
	{
		if (m_player.ShowPP)
		{
			m_player.Out.SendUpdatePlayerProperty(m_player.PlayerCharacter, this);
		}
	}

	public void ViewOther(PlayerInfo player)
	{
		m_player.Out.SendUpdatePlayerProperty(player, this);
	}

	public void CreateBaseProp(bool isSelf, double beaddefence, double beadattack, double suitdefence, double suitattack)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("Bead", (int)beadattack);
		dictionary.Add("Suit", (int)suitattack);
		Dictionary<string, int> dictionary2 = new Dictionary<string, int>();
		dictionary2.Add("Bead", (int)beaddefence);
		dictionary2.Add("Suit", (int)suitdefence);
		if (isSelf)
		{
			AddProp("Damage", dictionary);
			AddProp("Armor", dictionary2);
		}
		else
		{
			AddOtherProp("Damage", dictionary);
			AddOtherProp("Armor", dictionary2);
		}
	}

	public void UpadateBaseProp(bool isSelf, string mainKey, string subKey, double value)
	{
		if (isSelf)
		{
			if (m_playerProperty.ContainsKey(mainKey) && m_playerProperty[mainKey].ContainsKey(subKey))
			{
				m_playerProperty[mainKey][subKey] = (int)value;
			}
		}
		else if (m_otherPlayerProperty.ContainsKey(mainKey) && m_otherPlayerProperty[mainKey].ContainsKey(subKey))
		{
			m_otherPlayerProperty[mainKey][subKey] = (int)value;
		}
	}

	public void CreateProp(bool isSelf, string skey, int attack, int defence, int agility, int lucky, int hp)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("Attack", attack);
		dictionary.Add("Defence", defence);
		dictionary.Add("Agility", agility);
		dictionary.Add("Luck", lucky);
		dictionary.Add("HP", hp);
		if (isSelf)
		{
			AddProp(skey, dictionary);
		}
		else
		{
			AddOtherProp(skey, dictionary);
		}
	}
}
