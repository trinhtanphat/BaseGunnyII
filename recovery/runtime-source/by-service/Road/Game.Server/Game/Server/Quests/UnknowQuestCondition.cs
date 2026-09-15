using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class UnknowQuestCondition : BaseCondition
{
	public UnknowQuestCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.UnknowQuestConditionEvent += player_UnknowQuestCondition;
	}

	private void player_UnknowQuestCondition()
	{
		base.Value = 0;
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.UnknowQuestConditionEvent -= player_UnknowQuestCondition;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value <= 0;
	}
}
