using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FourNormalWolfNpc : ABrain
{
	private int m_attackTurn = 0;

	private int m_run = 0;

	protected Player m_targer;

	private int Dander = 0;

	private List<SimpleNpc> Children = new List<SimpleNpc>();

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
		if (m_attackTurn == 0)
		{
			WalkA();
			Angger();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			WalkA();
			Angger();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			Jump();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			WalkB();
			m_attackTurn++;
		}
		else
		{
			WalkB();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void Jump()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.JumpToSpeed(player.X, player.Y, "jump", 1000, 1, 1000, Fall);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -100, 83, 70);
	}

	public void Fall()
	{
		base.Body.PlayMovie("fall", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1000, null);
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.SetRelateDemagemRect(-41, -100, 83, 70);
	}

	private void WalkA()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		int num = base.Game.Random.Next(100, 170);
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		}
		if (base.Body.X < 400)
		{
			base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walkA", 1000, "", 4);
		}
		else
		{
			base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walkA", 1000, "", 4);
		}
		base.Body.CallFuction(AllAttack, 2600);
	}

	private void AllAttack()
	{
		ChangeDirection(1);
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatA", 0, 0);
		ChangeDirection(3);
	}

	private void AllAttack2()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatB", 0, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 1000, null);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -100, 83, 70);
	}

	private void WalkB()
	{
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (m_run == 0)
		{
			Beat(m_targer);
			m_run = 1;
		}
		else
		{
			Beat2(m_targer);
			m_run = 0;
		}
	}

	private void Beat(Player player)
	{
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		int num = (int)player.Distance(base.Body.X, base.Body.Y);
		if (num > 200)
		{
			if (player.X > base.Body.X)
			{
				base.Body.MoveTo(base.Body.X + num - 150, base.Body.Y, "walkB", 0, "", 12);
			}
			else
			{
				base.Body.MoveTo(base.Body.X - num + 150, base.Body.Y, "walkB", 0, "", 12);
			}
		}
		if (num < 200)
		{
			base.Body.PlayMovie("beatB", 2000, 0);
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.CallFuction(RangeAttacking, 2100);
		}
		else
		{
			base.Body.PlayMovie("beatB", num * 3 + 200, 0);
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.CallFuction(RangeAttacking, num * 4);
		}
	}

	private void Beat2(Player player)
	{
		int num = (int)player.Distance(base.Body.X, base.Body.Y);
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (num > 200)
		{
			if (player.X > base.Body.X)
			{
				base.Body.MoveTo(base.Body.X + num - 150, base.Body.Y, "walkB", 0, "", 12);
			}
			else
			{
				base.Body.MoveTo(base.Body.X - num + 150, base.Body.Y, "walkB", 0, "", 12);
			}
		}
		if (num < 200)
		{
			base.Body.PlayMovie("beatB", 2000, 0);
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.CallFuction(RangeAttacking, 2100);
		}
		else
		{
			base.Body.PlayMovie("beatB", num * 3 + 200, 0);
			base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
			base.Body.CallFuction(RangeAttacking, num * 4);
		}
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(base.Body.X - 200, base.Body.X + 200, "", 0, null);
	}

	public void Angger()
	{
		base.Body.State = 1;
		Dander += 100;
		((SimpleBoss)base.Body).SetDander(Dander);
		if (base.Body.Direction == -1)
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
		}
		else
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(-8, -252, 74, 50);
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
}
