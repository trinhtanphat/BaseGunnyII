using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FourHardCattleBoss : ABrain
{
	private int m_attackTurn = 0;

	private int Dander = 0;

	private PhysicalObj m_moive;

	private List<SimpleNpc> m_child = new List<SimpleNpc>();

	private int npcID = 4207;

	private static string[] AllAttackChat = new string[3] { "你们这是自寻死路！", "你惹毛我了!", "超级无敌大地震……<br/>震……震…… " };

	private static string[] ShootChat = new string[2] { "砸你家玻璃。", "看哥打的可比你们准多了" };

	private static string[] KillPlayerChat = new string[2] { "送你回老家！", "就凭你还妄想能够打败我？" };

	private static string[] CallChat = new string[2] { "卫兵！ <br/>卫兵！！ ", "啵咕们！！<br/>给我些帮助！" };

	private static string[] ShootedChat = new string[2] { "哎呦！很痛…", "我还顶的住…" };

	private static string[] JumpChat = new string[3] { "为了你们的胜利，<br/>向我开炮！", "你再往前半步我就把你给杀了！", "高！<br/>实在是高！" };

	private static string[] KillAttackChat = new string[1] { "超级肉弹！！" };

	public List<SimpleNpc> Child => m_child;

	public int CurrentLivingNpcNum
	{
		get
		{
			int num = 0;
			foreach (SimpleNpc item in Child)
			{
				if (!item.IsLiving)
				{
					num++;
				}
			}
			return Child.Count - num;
		}
	}

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		if (base.Body.Direction == -1)
		{
			base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}
		else
		{
			base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 0 && allFightPlayer.X < 0)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(0, 0);
		}
		else if (m_attackTurn == 0)
		{
			if (CurrentLivingNpcNum > 1)
			{
				base.Body.AddBlood(15000);
			}
			AllAttack2();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			PersonalAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			Jump();
			m_attackTurn++;
		}
		else
		{
			Physicallyinjured();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void Star()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y - 150, "moive", "game.crazytank.assetmap.Buff_powup", "", 1, 0);
		base.Body.CallFuction(CreateChild, 2000);
	}

	private void Physicallboss()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y - 150, "moive", "game.crazytank.assetmap.Buff_powup", "", 1, 0);
	}

	public void CreateChild()
	{
	}

	private void Physicallyinjured()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("AtoB", 1000, 0);
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void AllAttack2()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatB", 2000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void Healing()
	{
		base.Body.PlayMovie("beatC", 500, 0);
		base.Body.AddBlood(5000);
	}

	public void Jump()
	{
		base.Body.PlayMovie("jump", 1000, 6000);
		Player player = base.Game.FindRandomPlayer();
		base.Body.JumpToSpeed(player.X, base.Body.Y - 1000, "", 2500, 1, 10, Jump2);
	}

	public void Jump2()
	{
		base.Body.PlayMovie("fall", 0, 0);
		base.Body.RangeAttacking(base.Body.X - 2000, base.Body.X + 2000, "cry", 0, null);
	}

	private void PersonalAttack()
	{
		base.Body.MoveTo(base.Body.X - 100, base.Body.Y, "walk", 1000, "", 6, AllAttack);
	}

	public void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3300, null);
	}
}
