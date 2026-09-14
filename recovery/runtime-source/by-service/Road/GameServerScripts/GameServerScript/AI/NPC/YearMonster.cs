using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class YearMonster : ABrain
{
	private int m_attackTurn = 0;

	public int currentCount = 0;

	public int Dander = 0;

	private PhysicalObj moive;

	private Player target;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1000)
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
			KillAttack(0, base.Game.Map.Info.ForegroundWidth + 1);
			return;
		}
		if (m_attackTurn == 0)
		{
			AttackA();
			m_attackTurn++;
			return;
		}
		if (m_attackTurn == 1)
		{
			AttackB();
			m_attackTurn++;
			return;
		}
		if (m_attackTurn == 2)
		{
			AttackC();
			m_attackTurn++;
			return;
		}
		AttackD();
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			if (target.X < 400)
			{
				m_attackTurn = 0;
			}
			else
			{
				m_attackTurn = 1;
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1000f;
		base.Body.PlayMovie("beatE", 2000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
	}

	private void AttackA()
	{
		base.Body.CurrentDamagePlus = 1.5f;
		base.Body.PlayMovie("beatA", 1000, 0);
		base.Body.CallFuction(MovingPlayer, 3000);
	}

	private void MovingPlayer()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			player.StartSpeedMult(player.X - 200, player.Y);
		}
		base.Body.CallFuction(RangeAttacking, 1000);
	}

	private void AttackB()
	{
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.CallFuction(GoShootB, 4000);
	}

	private void AttackC()
	{
		base.Body.CurrentDamagePlus = 3.1f;
		base.Body.PlayMovie("beatC", 3000, 0);
		base.Body.CallFuction(RangeAttacking, 4500);
	}

	private void AttackD()
	{
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.CallFuction(GoShootD, 4000);
	}

	private void GoShootB()
	{
		base.Body.CurrentDamagePlus = 2.5f;
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			((PVEGame)base.Game).SendGameFocus(target, 0, 1000);
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.fifteen.305b", "out", 1, 1);
			base.Body.CallFuction(GoOutB, 2000);
			base.Body.CallFuction(RangeAttacking, 1000);
		}
	}

	private void GoOutB()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	private void GoShootD()
	{
		base.Body.CurrentDamagePlus = 7.5f;
		target = base.Game.FindRandomPlayer();
		if (target != null)
		{
			((PVEGame)base.Game).SendGameFocus(target, 0, 1000);
			moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "moive", "asset.game.fifteen.305d", "out", 1, 1);
			base.Body.CallFuction(GoOutD, 2000);
			base.Body.CallFuction(RangeAttacking, 1000);
		}
	}

	private void GoOutD()
	{
		((PVEGame)base.Game).SendGameFocus(base.Body, 0, 1000);
		base.Body.PlayMovie("born", 1000, 0);
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	private void RangeAttacking()
	{
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}
}
