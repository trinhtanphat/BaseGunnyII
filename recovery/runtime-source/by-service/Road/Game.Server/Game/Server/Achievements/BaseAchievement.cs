using System.Collections.Generic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Achievements;

public class BaseAchievement
{
	private AchievementInfo m_info;

	private AchievementDataInfo m_data;

	private List<BaseCondition> m_list;

	private string m_rank;

	private GamePlayer m_player;

	public string Rank => m_rank;

	public AchievementInfo Info => m_info;

	public AchievementDataInfo Data => m_data;

	public BaseAchievement(AchievementInfo info, AchievementDataInfo data)
	{
		m_info = info;
		m_data = data;
		m_data.AchievementID = m_info.ID;
		m_rank = m_info.Title;
		m_list = new List<BaseCondition>();
	}

	public void AddToPlayer(GamePlayer player)
	{
		m_player = player;
		if (!m_data.IsComplete)
		{
			AddTrigger(player);
		}
	}

	private void AddTrigger(GamePlayer player)
	{
		foreach (BaseCondition item in m_list)
		{
			item.AddTrigger(player);
		}
	}

	public void SaveData()
	{
		foreach (BaseCondition item in m_list)
		{
		}
	}

	public void Update()
	{
		SaveData();
		if (m_data.IsDirty && m_player != null)
		{
			m_player.AchievementInventory.Update(this);
		}
	}
}
