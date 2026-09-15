using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public class PlayerTreasure
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private List<TreasureDataInfo> m_TreasureData;

	private List<TreasureDataInfo> m_TreasureDig;

	private UserTreasureInfo m_Treasure;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public List<TreasureDataInfo> TreasureData
	{
		get
		{
			return m_TreasureData;
		}
		set
		{
			m_TreasureData = value;
		}
	}

	public List<TreasureDataInfo> TreasureDig
	{
		get
		{
			return m_TreasureDig;
		}
		set
		{
			m_TreasureDig = value;
		}
	}

	public UserTreasureInfo CurrentTreasure
	{
		get
		{
			return m_Treasure;
		}
		set
		{
			m_Treasure = value;
		}
	}

	public PlayerTreasure(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_TreasureData = new List<TreasureDataInfo>();
		m_TreasureDig = new List<TreasureDataInfo>();
		m_Treasure = new UserTreasureInfo();
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		m_Treasure = playerBussiness.GetSingleTreasure(Player.PlayerCharacter.ID);
		List<TreasureDataInfo> singleTreasureData = playerBussiness.GetSingleTreasureData(Player.PlayerCharacter.ID);
		if (m_Treasure == null)
		{
			CreateTreasure();
		}
		foreach (TreasureDataInfo item in singleTreasureData)
		{
			m_TreasureData.Add(item);
			if (item.pos > 0)
			{
				m_TreasureDig.Add(item);
			}
		}
	}

	public void CreateTreasure()
	{
		if (m_Treasure == null)
		{
			m_Treasure = new UserTreasureInfo();
		}
		lock (m_Treasure)
		{
			m_Treasure.ID = 0;
			m_Treasure.UserID = Player.PlayerCharacter.ID;
			m_Treasure.NickName = Player.PlayerCharacter.NickName;
			m_Treasure.treasure = 1;
			m_Treasure.treasureAdd = 0;
			m_Treasure.logoinDays = 1;
			m_Treasure.friendHelpTimes = 0;
			m_Treasure.isBeginTreasure = false;
			m_Treasure.isEndTreasure = false;
			m_Treasure.LastLoginDay = DateTime.Now;
		}
	}

	public void AddfriendHelpTimes()
	{
		lock (m_Treasure)
		{
			if (m_Treasure.friendHelpTimes < 5)
			{
				m_Treasure.friendHelpTimes++;
				if (m_Treasure.friendHelpTimes == 5 && m_Treasure.treasureAdd == 0)
				{
					m_Treasure.treasureAdd++;
				}
			}
		}
	}

	public void UpdateUserTreasure(UserTreasureInfo info)
	{
		lock (m_Treasure)
		{
			m_Treasure = info;
		}
	}

	public void Clear()
	{
		lock (m_TreasureDig)
		{
			m_TreasureDig = new List<TreasureDataInfo>();
		}
	}

	public void UpdateLoginDay()
	{
		int iD = Player.PlayerCharacter.ID;
		List<TreasureDataInfo> treasureData = m_TreasureData;
		if (treasureData.Count == 0)
		{
			treasureData = TreasureAwardMgr.CreateTreasureData(iD);
			AddTreasureData(treasureData);
		}
		else if (m_Treasure.isValidDate())
		{
			treasureData = TreasureAwardMgr.CreateTreasureData(iD);
			UpdateTreasureData(treasureData);
			Clear();
		}
		lock (m_Treasure)
		{
			if (m_Treasure.isValidDate())
			{
				if ((int)DateTime.Now.Subtract(m_Treasure.LastLoginDay).TotalDays > 1)
				{
					m_Treasure.logoinDays = 0;
				}
				m_Treasure.logoinDays++;
				if (m_Treasure.logoinDays > 3)
				{
					m_Treasure.treasure = 3;
				}
				else
				{
					m_Treasure.treasure = m_Treasure.logoinDays;
				}
				m_Treasure.treasureAdd = 0;
				m_Treasure.friendHelpTimes = 0;
				m_Treasure.isBeginTreasure = false;
				m_Treasure.isEndTreasure = false;
				m_Treasure.LastLoginDay = DateTime.Now;
			}
		}
	}

	public void AddTreasureData(List<TreasureDataInfo> datas)
	{
		lock (m_TreasureData)
		{
			foreach (TreasureDataInfo data in datas)
			{
				m_TreasureData.Add(data);
			}
		}
	}

	public void UpdateTreasureData(List<TreasureDataInfo> datas)
	{
		for (int i = 0; i < m_TreasureData.Count; i++)
		{
			datas[i].ID = m_TreasureData[i].ID;
		}
		lock (m_TreasureData)
		{
			m_TreasureData = datas;
		}
	}

	public void AddTreasureDig(TreasureDataInfo info, int index)
	{
		lock (m_TreasureDig)
		{
			m_TreasureDig.Add(info);
		}
		lock (m_TreasureData)
		{
			m_TreasureData[index].pos = info.pos;
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			if (m_Treasure != null && m_Treasure.IsDirty)
			{
				if (m_Treasure.ID > 0)
				{
					playerBussiness.UpdateUserTreasureInfo(m_Treasure);
				}
				else
				{
					playerBussiness.AddUserTreasureInfo(m_Treasure);
				}
			}
			for (int i = 0; i < m_TreasureData.Count; i++)
			{
				TreasureDataInfo treasureDataInfo = m_TreasureData[i];
				if (treasureDataInfo != null && treasureDataInfo.IsDirty)
				{
					if (treasureDataInfo.ID > 0)
					{
						playerBussiness.UpdateTreasureData(treasureDataInfo);
					}
					else
					{
						playerBussiness.AddTreasureData(treasureDataInfo);
					}
				}
			}
		}
	}
}
