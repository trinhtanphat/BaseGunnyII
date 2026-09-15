using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class FiveNormalSecoundBoss : ABrain
{
	private Player m_target = null;

	private int m_targetDis = 0;

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		m_targetDis = (int)m_target.Distance(base.Body.X, base.Body.Y);
		if (m_targetDis < 50)
		{
			base.Body.PlayMovie("beat", 100, 0);
			base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1500, null);
		}
		else
		{
			MoveToPlayer(m_target);
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void MoveToPlayer(Player player)
	{
		int num = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
		if (player.X > base.Body.X)
		{
			base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 2000, "", 3, Beat);
		}
		else
		{
			base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 2000, "", 3, Beat);
		}
	}

	public void Beat()
	{
		if (m_targetDis < 50)
		{
			base.Body.PlayMovie("beatA", 100, 0);
			base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 1500, null);
		}
	}
}
}
