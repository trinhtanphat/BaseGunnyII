using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FourNormalFireNpc : ABrain
{
	private int m_turn = 0;

	private int m_attackTurn = 0;

	private PhysicalObj m_moive = null;

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		if (m_attackTurn == 0)
		{
			Move();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			Move();
			m_attackTurn++;
		}
		else
		{
			Die();
			m_attackTurn = 0;
		}
	}

	private void Move()
	{
		base.Body.MoveTo(base.Game.Random.Next(300, 980), base.Game.Random.Next(300, 600), "fly", 500, "", 6, CreateChild);
	}

	public void Die()
	{
		base.Body.PlayMovie("cry", 1000, 0);
		base.Body.PlayMovie("die", 2000, 0);
		base.Body.Die(3000);
	}

	private void CreateChild()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y + 20, "moive", "game.living.Living141", "stand", 1, 0);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (m_moive != null)
		{
			base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
			m_moive = null;
		}
	}
}
