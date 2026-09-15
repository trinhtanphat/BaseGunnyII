using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public class PlayerRank
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	protected GamePlayer m_player;

	private List<UserRankInfo> m_rank;

	private UserRankInfo m_currentRank;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public List<UserRankInfo> Ranks
	{
		get
		{
			return m_rank;
		}
		set
		{
			m_rank = value;
		}
	}

	public UserRankInfo CurrentRank
	{
		get
		{
			return m_currentRank;
		}
		set
		{
			m_currentRank = value;
		}
	}

	public PlayerRank(GamePlayer player, bool saveTodb)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_rank = new List<UserRankInfo>();
		m_currentRank = GetRank(m_player.PlayerCharacter.Honor);
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		List<UserRankInfo> singleUserRank = playerBussiness.GetSingleUserRank(Player.PlayerCharacter.ID);
		if (singleUserRank.Count == 0)
		{
			CreateRank(Player.PlayerCharacter.ID);
			return;
		}
		foreach (UserRankInfo item in singleUserRank)
		{
			if (item.IsValidRank())
			{
				AddRank(item);
			}
			else
			{
				RemoveRank(item);
			}
		}
	}

	public void AddRank(UserRankInfo info)
	{
		lock (m_rank)
		{
			m_rank.Add(info);
		}
	}

	public void RemoveRank(UserRankInfo item)
	{
		item.IsExit = false;
		AddRank(item);
	}

	public List<UserRankInfo> GetRank()
	{
		List<UserRankInfo> list = new List<UserRankInfo>();
		foreach (UserRankInfo item in m_rank)
		{
			if (item.IsExit)
			{
				list.Add(item);
			}
		}
		return list;
	}

	public UserRankInfo GetRank(string honor)
	{
		foreach (UserRankInfo item in m_rank)
		{
			if (item.UserRank == honor)
			{
				return item;
			}
		}
		return null;
	}

	public bool IsRank(string honor)
	{
		foreach (UserRankInfo item in m_rank)
		{
			if (item.UserRank == honor)
			{
				return true;
			}
		}
		return false;
	}

	public void CreateRank(int UserID)
	{
		new List<UserRankInfo>();
		AddRank(new UserRankInfo
		{
			ID = 0,
			UserID = UserID,
			UserRank = "Bé tập chơi",
			Attack = 0,
			Defence = 0,
			Luck = 0,
			Agility = 0,
			HP = 0,
			Damage = 0,
			Guard = 0,
			BeginDate = DateTime.Now,
			Validate = 0,
			IsExit = true
		});
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
			for (int i = 0; i < m_rank.Count; i++)
			{
				UserRankInfo userRankInfo = m_rank[i];
				if (userRankInfo != null && userRankInfo.IsDirty)
				{
					if (userRankInfo.ID > 0)
					{
						playerBussiness.UpdateUserRank(userRankInfo);
					}
					else
					{
						playerBussiness.AddUserRank(userRankInfo);
					}
				}
			}
		}
	}
}
