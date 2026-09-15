using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirdNormalBloodNpc : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_king = null;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
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
			Healing();
			m_attackTurn++;
		}
		else
		{
			GoHealing();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		base.Body.PlayMovie("beat", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void Healing()
	{
	}

	private void GoHealing()
	{
		base.Body.SyncAtTime = true;
		base.Body.PlayMovie("die", 1000, 4500);
		base.Body.Die(0);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
}
