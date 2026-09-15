using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ConsortiaScorpionBoss : ABrain
{
	private int m_attackTurn = 0;

	public int currentCount = 0;

	public int Dander = 0;

	private PhysicalObj moive;

	private Player target = null;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 857 && allFightPlayer.X < 1440)
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
		}
		else if (m_attackTurn == 0)
		{
			Moving();
			m_attackTurn++;
		}
		else
		{
			AllAttack();
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
		base.Body.PlayMovie("beatA", 1000, 0);
		target = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 308f;
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 3300, null);
		base.Body.CallFuction(CreateEffect, 3300);
		base.Body.CallFuction(Out, 4600);
	}

	private void AllAttack()
	{
		ChangeDirection(3);
		base.Body.PlayMovie("beatA", 1000, 0);
		target = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 3.8f;
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 3300, null);
		base.Body.CallFuction(CreateEffect, 3300);
		base.Body.CallFuction(Out, 4600);
	}

	public void CreateEffect()
	{
		if (target != null)
		{
			if (target.X < 1000)
			{
				moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "effect", "asset.game.eight.xiezi", "beatA", 1, 0);
			}
			else
			{
				moive = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "effect", "asset.game.eight.xiezi", "beatB", 1, 0);
			}
		}
	}

	private void Out()
	{
		((PVEGame)base.Game).SendGameFocus(base.Body, 1000, 2000);
		base.Body.PlayMovie("in", 1000, 0);
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
	}

	private void Moving()
	{
		ChangeDirection(3);
		int x = base.Game.Random.Next(990, 1300);
		int direction = base.Body.Direction;
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", ((SimpleBoss)base.Body).NpcInfo.speed);
		base.Body.ChangeDirection(base.Game.FindlivingbyDir(base.Body), 3000);
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
}
