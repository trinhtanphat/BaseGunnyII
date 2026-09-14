using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class UsingItemCondition : BaseCondition
{
	public UsingItemCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.AfterUsingItem += player_ItemProperty;
	}

	private void player_ItemProperty(int templateID)
	{
		if (templateID == m_info.Para1 && base.Value < m_info.Para2)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.AfterUsingItem -= player_ItemProperty;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Para2;
	}
}
