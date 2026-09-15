using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class FourTerrorFireNpc : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive = null;

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
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
			m_attackTurn = 0;
		}
	}

	private void Move()
	{
		base.Body.MoveTo(base.Game.Random.Next(225, 1115), base.Game.Random.Next(113, 354), "fly", 500, "", 6, null);
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
}
