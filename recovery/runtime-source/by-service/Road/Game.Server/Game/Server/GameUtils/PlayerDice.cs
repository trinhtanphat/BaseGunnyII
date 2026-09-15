using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerDice
{
	public int MAX_LEVEL = 5;

	public int refreshPrice = GameProperties.DiceRefreshPrice;

	public int commonDicePrice = GameProperties.CommonDicePrice;

	public int doubleDicePrice = GameProperties.DoubleDicePrice;

	public int bigDicePrice = GameProperties.BigDicePrice;

	public int smallDicePrice = GameProperties.SmallDicePrice;

	public int[] Integral = new int[5] { 100, 300, 700, 1500, 3100 };

	protected GamePlayer m_player;

	private int m_result;

	private int m_level;

	private int m_freeCount;

	private int m_CurrentPosition;

	private int m_LuckIntegral;

	private int m_LuckIntegralLevel;

	private bool m_UserFirstCell;

	private List<ItemInfo> m_rewardItem;

	private string m_rewardName;

	private Dictionary<int, List<ItemInfo>> m_LevelAward;

	public GamePlayer Player => m_player;

	public int result
	{
		get
		{
			return m_result;
		}
		set
		{
			m_result = value;
		}
	}

	public int level
	{
		get
		{
			return m_level;
		}
		set
		{
			m_level = value;
		}
	}

	public int freeCount
	{
		get
		{
			return m_freeCount;
		}
		set
		{
			m_freeCount = value;
		}
	}

	public int CurrentPosition
	{
		get
		{
			return m_CurrentPosition;
		}
		set
		{
			m_CurrentPosition = value;
		}
	}

	public int LuckIntegral
	{
		get
		{
			return m_LuckIntegral;
		}
		set
		{
			m_LuckIntegral = value;
		}
	}

	public int LuckIntegralLevel
	{
		get
		{
			return m_LuckIntegralLevel;
		}
		set
		{
			m_LuckIntegralLevel = value;
		}
	}

	public bool UserFirstCell
	{
		get
		{
			return m_UserFirstCell;
		}
		set
		{
			m_UserFirstCell = value;
		}
	}

	public List<ItemInfo> rewardItem
	{
		get
		{
			return m_rewardItem;
		}
		set
		{
			m_rewardItem = value;
		}
	}

	public string rewardName
	{
		get
		{
			return m_rewardName;
		}
		set
		{
			m_rewardName = value;
		}
	}

	public Dictionary<int, List<ItemInfo>> LevelAward
	{
		get
		{
			return m_LevelAward;
		}
		set
		{
			m_LevelAward = value;
		}
	}

	public PlayerDice(GamePlayer player)
	{
		m_player = player;
		m_CurrentPosition = -1;
		m_LuckIntegral = 0;
		m_LuckIntegralLevel = -1;
		m_UserFirstCell = false;
		m_freeCount = 0;
		m_result = 0;
		m_level = 0;
		m_rewardName = "";
		ReceiveLevelAward();
	}

	public void LoadFromDatabase()
	{
		if (Player.PlayerCharacter.lastLuckNum > 0)
		{
			m_LuckIntegral = Player.PlayerCharacter.lastLuckNum;
		}
		if (Player.PlayerCharacter.luckyNum > -1)
		{
			m_LuckIntegralLevel = Player.PlayerCharacter.luckyNum;
		}
	}

	public void ReceiveData()
	{
		m_rewardItem = TreasureAwardMgr.CreateDiceAward();
	}

	public void ReceiveLevelAward()
	{
		m_LevelAward = new Dictionary<int, List<ItemInfo>>();
		for (int i = 0; i < MAX_LEVEL; i++)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			list = i switch
			{
				1 => TreasureAwardMgr.CreateDiceLevelAward(2),
				2 => TreasureAwardMgr.CreateDiceLevelAward(3),
				3 => TreasureAwardMgr.CreateDiceLevelAward(4),
				4 => TreasureAwardMgr.CreateDiceLevelAward(5),
				_ => TreasureAwardMgr.CreateDiceLevelAward(1),
			};
			if (!m_LevelAward.ContainsKey(i))
			{
				m_LevelAward.Add(i, list);
			}
		}
	}

	public void GetLevelAward()
	{
		StringBuilder stringBuilder = new StringBuilder();
		IList<ItemInfo> list = m_LevelAward[m_LuckIntegralLevel];
		foreach (ItemInfo item in list)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateID), item.Count, 103);
			Player.AddTemplate(itemInfo);
			stringBuilder.Append(itemInfo.Template.Name + "x" + itemInfo.Count + "; ");
		}
		if (m_LuckIntegralLevel > 3)
		{
			Player.SendMessage("Bạn nhận được " + stringBuilder.ToString());
		}
	}
}
