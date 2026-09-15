using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class SimpleBossFor40066 : ABrain
{
	private int m_attackTurn = 0;

	public int currentCount = 0;

	public int Dander = 0;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 670)
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
		}
		if (m_attackTurn == 0)
		{
			MoveBeatA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			AttackB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			AttackC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			AttackD();
			m_attackTurn++;
		}
		else
		{
			AttackE();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void MoveBeatA()
	{
		base.Body.MoveTo(base.Game.Random.Next(641, 1110), 781, "walk", 500, "", 12, AttackA);
	}

	private void AttackA()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.CallFuction(RangeAttacking, 2000);
	}

	private void AttackB()
	{
		base.Body.CurrentDamagePlus = 0.8f;
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.CallFuction(RangeAttacking, 4000);
	}

	private void AttackC()
	{
		base.Body.CurrentDamagePlus = 1.1f;
		base.Body.PlayMovie("beatC", 1000, 0);
		base.Body.CallFuction(RangeAttacking, 3500);
	}

	private void AttackD()
	{
		base.Body.CurrentDamagePlus = 1.1f;
		base.Body.PlayMovie("beatD", 1000, 0);
		base.Body.CallFuction(RangeAttacking, 3500);
	}

	private void AttackE()
	{
		base.Body.CurrentDamagePlus = 1.1f;
		base.Body.PlayMovie("beatE", 1000, 0);
		base.Body.CallFuction(RangeAttacking, 3500);
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}
}
}
