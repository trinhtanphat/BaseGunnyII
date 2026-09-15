using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class FourNormalBlowArmsNpc : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive = null;

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
		if (m_attackTurn == 0)
		{
			MoveToGate();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			MoveToGate();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			MoveToGate();
			m_attackTurn++;
		}
		else
		{
			MoveToExit();
			m_attackTurn = 0;
		}
	}

	private void MoveToGate()
	{
		base.Body.MoveTo(base.Body.X + base.Game.Random.Next(250, 300), base.Body.Y, "walk", 2000, "", 4, CanDie);
	}

	private void MoveToExit()
	{
		base.Body.MoveTo(1415, base.Body.Y, "walk", 2000, "", 4, BeatA);
	}

	private void CanDie()
	{
		if (base.Body.Blood <= 50)
		{
			base.Body.PlayMovie("die", 100, 0);
			base.Body.Die(1000);
		}
		else
		{
			base.Body.AddEffect(new ContinueReduceBloodEffect(2, base.Game.Random.Next(789, 1021), base.Body), 0);
			base.Body.PlayMovie("standB", 100, 0);
		}
	}

	private void BeatA()
	{
		base.Body.PlayMovie("beatA", 100, 0);
		if (base.Body.FindCount == 0)
		{
			base.Body.CallFuction(CryA, 2900);
		}
		else if (base.Body.FindCount == 1)
		{
			base.Body.CallFuction(CryB, 2900);
		}
		else
		{
			base.Body.CallFuction(CryC, 2900);
		}
		base.Body.Die(3000);
	}

	private void CryA()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(1590, 750, "moive", "game.asset.Gate", "cryA", 1, 0);
		base.Body.FindCount = 3;
	}

	private void CryB()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(1590, 750, "moive", "game.asset.Gate", "cryB", 1, 0);
		base.Body.FindCount = 2;
	}

	private void CryC()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(1590, 750, "moive", "game.asset.Gate", "cryC", 1, 0);
		base.Body.FindCount = 3;
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
}
