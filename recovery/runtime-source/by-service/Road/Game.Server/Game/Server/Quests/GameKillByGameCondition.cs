using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class GameKillByGameCondition : BaseCondition
{
	public GameKillByGameCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
	}

	public override void AddTrigger(GamePlayer player)
	{
		player.AfterKillingLiving += player_AfterKillingLiving;
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.AfterKillingLiving -= player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
	{
		if (isLiving || type != 1)
		{
			return;
		}
		switch (game.GameType)
		{
		case eGameType.Free:
			if ((m_info.Para1 == 0 || m_info.Para1 == -1) && base.Value > m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.Guild:
			if ((m_info.Para1 == 1 || m_info.Para1 == -1) && base.Value > m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.ALL:
			if ((m_info.Para1 == 4 || m_info.Para1 == -1) && base.Value > m_info.Para2)
			{
				base.Value++;
			}
			break;
		case eGameType.Dungeon:
			if ((m_info.Para1 == 7 || m_info.Para1 == -1) && base.Value > m_info.Para2)
			{
				base.Value++;
			}
			break;
		}
		if (base.Value > m_info.Para2)
		{
			base.Value = m_info.Para2;
		}
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value >= m_info.Para2;
	}
}
