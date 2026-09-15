using System.Collections.Generic;
using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Quests;

public class TurnPropertyCondition : BaseCondition
{
	private BaseQuest m_quest;

	private GamePlayer m_player;

	public TurnPropertyCondition(BaseQuest quest, QuestConditionInfo info, int value)
		: base(quest, info, value)
	{
		m_quest = quest;
	}

	public override void AddTrigger(GamePlayer player)
	{
		m_player = player;
		player.GameKillDrop += QuestDropItem;
		base.AddTrigger(player);
	}

	public override bool IsCompleted(GamePlayer player)
	{
		bool result = false;
		if (player.GetItemCount(m_info.Para1) >= m_info.Para2)
		{
			base.Value = m_info.Para2;
			result = true;
		}
		return result;
	}

	public override bool Finish(GamePlayer player)
	{
		return player.RemoveTemplate(m_info.Para1, m_info.Para2);
	}

	public override void RemoveTrigger(GamePlayer player)
	{
		player.GameKillDrop -= QuestDropItem;
		base.RemoveTrigger(player);
	}

	public override bool CancelFinish(GamePlayer player)
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(m_info.Para1);
		if (itemTemplateInfo != null)
		{
			ItemInfo cloneItem = ItemInfo.CreateFromTemplate(itemTemplateInfo, m_info.Para2, 117);
			return player.AddTemplate(cloneItem, eBageType.TempBag, m_info.Para2, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView);
		}
		return false;
	}

	private void QuestDropItem(AbstractGame game, int copyId, int npcId, bool playResult)
	{
		if (m_player.GetItemCount(m_info.Para1) >= m_info.Para2)
		{
			return;
		}
		List<ItemInfo> info = null;
		int gold = 0;
		int money = 0;
		int giftToken = 0;
		int medal = 0;
		int honor = 0;
		int hardCurrency = 0;
		int token = 0;
		int dragonToken = 0;
		if (game is PVEGame)
		{
			DropInventory.PvEQuestsDrop(npcId, ref info);
		}
		if (game is PVPGame)
		{
			DropInventory.PvPQuestsDrop(game.RoomType, playResult, ref info);
		}
		if (info == null)
		{
			return;
		}
		foreach (ItemInfo item in info)
		{
			ShopMgr.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken);
			if (item != null)
			{
				m_player.TempBag.AddTemplate(item, item.Count);
			}
		}
		m_player.AddGold(gold);
		m_player.AddGiftToken(giftToken);
		m_player.AddMoney(money);
		m_player.AddMedal(medal);
	}
}
