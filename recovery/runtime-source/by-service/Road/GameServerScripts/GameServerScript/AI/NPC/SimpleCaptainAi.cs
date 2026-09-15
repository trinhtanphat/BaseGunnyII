using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class SimpleCaptainAi : ABrain
{
	private int m_attackTurn = 0;

	public int currentCount = 0;

	public int Dander = 0;

	private int npcID = 1009;

	public List<SimpleNpc> Children = new List<SimpleNpc>();

	private static string[] AllAttackChat = new string[3] { "你们这是自寻死路！", "你惹毛我了!", "超级无敌大地震……<br/>震……震…… " };

	private static string[] ShootChat = new string[2] { "砸你家玻璃。", "看哥打的可比你们准多了" };

	private static string[] KillPlayerChat = new string[2] { "送你回老家！", "就凭你还妄想能够打败我？" };

	private static string[] CallChat = new string[2] { "卫兵！ <br/>卫兵！！ ", "啵咕们！！<br/>给我些帮助！" };

	private static string[] ShootedChat = new string[2] { "哎呦！很痛…", "我还顶的住…" };

	private static string[] JumpChat = new string[3] { "为了你们的胜利，<br/>向我开炮！", "你再往前半步我就把你给杀了！", "高！<br/>实在是高！" };

	private static string[] KillAttackChat = new string[1] { "超级肉弹！！" };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 480 && allFightPlayer.X < 1000)
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
			KillAttack(480, 1000);
		}
		else if (m_attackTurn == 0)
		{
			AllAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			PersonalAttack();
			m_attackTurn++;
		}
		else
		{
			Summon();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		ChangeDirection(3);
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.CurrentDamagePlus = 10f;
		base.Body.PlayMovie("beat2", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void AllAttack()
	{
		ChangeDirection(3);
		base.Body.CurrentDamagePlus = 0.5f;
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 0);
		base.Body.FallFrom(base.Body.X, 509, null, 1000, 1, 12);
		base.Body.PlayMovie("beat2", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 4000, null);
	}

	private void PersonalAttack()
	{
		ChangeDirection(3);
		int num = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num], 1, 0);
		int x = base.Game.Random.Next(670, 880);
		int direction = base.Body.Direction;
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", ((SimpleBoss)base.Body).NpcInfo.speed, NextAttack);
		base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 9000);
	}

	private void Summon()
	{
		ChangeDirection(3);
		base.Body.JumpTo(base.Body.X, base.Body.Y - 300, "Jump", 1000, 1);
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 3300);
		base.Body.PlayMovie("call", 3500, 0);
		base.Body.CallFuction(CreateChild, 4000);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.SetRect(0, 0, 0, 0);
		if (player.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 500);
		}
		else
		{
			base.Body.ChangeDirection(-1, 500);
		}
		base.Body.CurrentDamagePlus = 0.8f;
		if (player != null)
		{
			int x = base.Game.Random.Next(player.X - 50, player.X + 50);
			if (base.Body.ShootPoint(x, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 1f, 2200))
			{
				base.Body.PlayMovie("beat", 1700, 0);
			}
		}
	}

	private void ChangeDirection(int count)
	{
		int direction = base.Body.Direction;
		for (int i = 0; i < count; i++)
		{
			base.Body.ChangeDirection(-direction, i * 200 + 100);
			base.Body.ChangeDirection(direction, (i + 1) * 100 + i * 200);
		}
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 520, 530, 430, 6, 1);
	}
}
