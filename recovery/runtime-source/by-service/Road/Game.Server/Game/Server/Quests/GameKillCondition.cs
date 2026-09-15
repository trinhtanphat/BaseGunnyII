using System;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class GameKillCondition : BaseCondition
{
	public GameKillCondition(BaseQuest quest, QuestConditionInfo info, int value)
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
		Console.WriteLine("是否活" + isLiving + ":房间类型" + game.RoomType);
		if (isLiving || type != 1)
		{
			return;
		}
		switch (game.RoomType)
		{
		case eRoomType.Match:
			if ((m_info.Para1 == 0 || m_info.Para1 == -1) && base.Value > 0)
			{
				base.Value--;
			}
			break;
		case eRoomType.Freedom:
			if ((m_info.Para1 == 1 || m_info.Para1 == -1) && base.Value > 0)
			{
				base.Value--;
			}
			break;
		}
		if (base.Value < 0)
		{
			base.Value = 0;
		}
	}

	public override bool IsCompleted(GamePlayer player)
	{
		return base.Value <= 0;
	}
}
