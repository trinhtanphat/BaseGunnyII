using System.Reflection;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Achievements;

public class BaseCondition
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected AchievementConditionInfo m_info;

	private int m_value;

	private BaseAchievement m_quest;

	public AchievementConditionInfo Info => m_info;

	public int Value
	{
		get
		{
			return m_value;
		}
		set
		{
			if (m_value != value)
			{
				m_value = value;
				m_quest.Update();
			}
		}
	}

	public BaseCondition(BaseAchievement quest, AchievementConditionInfo info, int value)
	{
		m_quest = quest;
		m_info = info;
		m_value = value;
	}

	public virtual void Reset(GamePlayer player)
	{
		m_value = m_info.Condiction_Para2;
	}

	public virtual void AddTrigger(GamePlayer player)
	{
	}

	public virtual void RemoveTrigger(GamePlayer player)
	{
	}

	public virtual bool IsCompleted(GamePlayer player)
	{
		return false;
	}

	public virtual bool Finish(GamePlayer player)
	{
		return true;
	}

	public virtual bool CancelFinish(GamePlayer player)
	{
		return true;
	}
}
