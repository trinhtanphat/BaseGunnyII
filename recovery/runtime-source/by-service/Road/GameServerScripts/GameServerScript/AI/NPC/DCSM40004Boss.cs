using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class DCSM40004Boss : ABrain
{
	private int m_attackTurn = 0;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 500 && allFightPlayer.X < 1050)
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
			KillAttack(500, 1050);
		}
		else if (m_attackTurn == 0)
		{
			AllAttack();
			m_attackTurn++;
		}
		else
		{
			PersonalAttack();
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
		base.Body.CurrentDamagePlus = 10f;
		base.Body.PlayMovie("beat2", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void AllAttack()
	{
		ChangeDirection(3);
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.FallFrom(base.Body.X, 509, null, 1000, 1, 12);
		base.Body.PlayMovie("beat2", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 4000, null);
	}

	private void PersonalAttack()
	{
		ChangeDirection(3);
		int x = base.Game.Random.Next(670, 880);
		int direction = base.Body.Direction;
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 3, NextAttack);
		base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 9000);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.SetRect(0, 0, 0, 0);
		base.Body.CurrentDamagePlus = 0.8f;
		if (player != null)
		{
			if (player.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 500);
			}
			else
			{
				base.Body.ChangeDirection(-1, 500);
			}
			int x = base.Game.Random.Next(player.X - 50, player.X + 50);
			if (base.Body.ShootPoint(x, player.Y, 61, 1000, 10000, 1, 1f, 2200))
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
}
