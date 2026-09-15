using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class NewGearCondition : BaseCondition
{
	public NewGearCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.NewGearEvent += player_NewGear;
	}

	private void player_NewGear(int CategoryID)
	{
		if ((CategoryID == m_info.Para1 || CategoryID == m_info.Para2) && base.Value < m_info.Para2)
		{
			base.Value++;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.NewGearEvent -= player_NewGear;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Para2;
	}
}
