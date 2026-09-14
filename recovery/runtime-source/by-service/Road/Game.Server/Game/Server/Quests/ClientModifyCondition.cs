using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class ClientModifyCondition : BaseCondition
{
	public ClientModifyCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void Reset(GamePlayer player)
	{
		base.Value = m_info.Para2;
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Para2;
	}
}
