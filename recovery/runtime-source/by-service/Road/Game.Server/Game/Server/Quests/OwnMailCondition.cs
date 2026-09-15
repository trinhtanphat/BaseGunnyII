using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class OwnMailCondition : BaseCondition
{
	public OwnMailCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
	}

	private void player_OwnMail(int templateID, int count)
	{
		if (templateID == m_info.Para1 && base.Value > 0)
		{
			base.Value -= count;
		}
	}

	public override void RemoveTrigger(GamePlayer player)
	{
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value <= 0;
	}
}
