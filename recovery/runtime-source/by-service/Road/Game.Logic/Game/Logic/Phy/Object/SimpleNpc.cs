using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness.Managers;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic.Phy.Object;

public class SimpleNpc : Living
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private NpcInfo m_npcInfo;

	private ABrain m_ai;

	public int TotalCure;

	public NpcInfo NpcInfo => m_npcInfo;

	public SimpleNpc(int id, BaseGame game, NpcInfo npcInfo, int type, int direction)
		: base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
	{
		if (type == 0)
		{
			base.Type = eLivingType.SimpleNpc;
		}
		else
		{
			base.Type = eLivingType.SimpleNpc1;
		}
		m_npcInfo = npcInfo;
		m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
		if (m_ai == null)
		{
			log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
			m_ai = SimpleBrain.Simple;
		}
		m_ai.Game = m_game;
		m_ai.Body = this;
		try
		{
			m_ai.OnCreated();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc Created error:{1}", arg);
		}
	}

	public override void Reset()
	{
		Agility = m_npcInfo.Agility;
		Attack = m_npcInfo.Attack;
		BaseDamage = m_npcInfo.BaseDamage;
		BaseGuard = m_npcInfo.BaseGuard;
		Lucky = m_npcInfo.Lucky;
		Grade = m_npcInfo.Level;
		Experience = m_npcInfo.Experience;
		TotalCure = 0;
		SetRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		SetRelateDemagemRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		base.Reset();
	}

	public void GetDropItemInfo()
	{
		if (!(m_game.CurrentLiving is Player))
		{
			return;
		}
		Player player = m_game.CurrentLiving as Player;
		List<ItemInfo> info = null;
		int gold = 0;
		int money = 0;
		int giftToken = 0;
		int medal = 0;
		int honor = 0;
		int hardCurrency = 0;
		int token = 0;
		int dragonToken = 0;
		DropInventory.NPCDrop(m_npcInfo.DropId, ref info);
		if (info == null)
		{
			return;
		}
		foreach (ItemInfo item in info)
		{
			ShopMgr.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken);
			if (item != null)
			{
				if (item.Template.CategoryID == 10)
				{
					player.PlayerDetail.AddTemplate(item, eBageType.FightBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
				}
				else
				{
					player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
				}
			}
		}
		player.PlayerDetail.AddGold(gold);
		player.PlayerDetail.AddMoney(money);
		player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_Drop, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
		player.PlayerDetail.AddGiftToken(giftToken);
		player.PlayerDetail.AddMedal(medal);
	}

	public override void Die()
	{
		GetDropItemInfo();
		base.Die();
	}

	public override void Die(int delay)
	{
		GetDropItemInfo();
		base.Die(delay);
	}

	public override void PrepareNewTurn()
	{
		base.PrepareNewTurn();
		try
		{
			m_ai.OnBeginNewTurn();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc BeginNewTurn error:{1}", arg);
		}
	}

	public override void StartAttacking()
	{
		base.StartAttacking();
		try
		{
			m_ai.OnStartAttacking();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc StartAttacking error:{1}", arg);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		try
		{
			m_ai.Dispose();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleNpc Dispose error:{1}", arg);
		}
	}
}
