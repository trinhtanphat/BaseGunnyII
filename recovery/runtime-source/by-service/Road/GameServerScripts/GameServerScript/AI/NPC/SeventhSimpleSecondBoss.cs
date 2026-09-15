using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class SeventhSimpleSecondBoss : ABrain
{
	private int m_attackTurn = 0;

	private bool IsEixt = false;

	protected Player m_targer;

	private PhysicalObj moive;

	private PhysicalObj m_effect = null;

	private static string[] AllAttackChat = new string[1] { "Cận thận cái đầu !!" };

	private static string[] ShootChat = new string[2] { "Thịt đè người !", "Cảm nhận sức mạnh của ta !" };

	private static string[] KillAttackChat = new string[1] { "Đến nộp mạng à ?? Sức mạnh tối cao!!..." };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 0 && allFightPlayer.X < 350)
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
			KillAttack(0, 350);
		}
		else if (m_attackTurn == 0)
		{
			if (IsEixt)
			{
				base.Game.RemovePhysicalObj(m_effect, sendToClient: true);
				IsEixt = false;
			}
			AttackingB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			AttackingA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			AttackingC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			AttackingD();
			m_attackTurn++;
		}
		else
		{
			AttackingC();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 0);
		base.Body.PlayMovie("skill", 1900, 0);
		base.Body.RangeAttacking(0, 350, "cry", 4000, null);
		base.Body.CallFuction(GoKillAttack, 4000);
	}

	private void GoKillAttack()
	{
		base.Body.CurrentDamagePlus *= 10f;
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		m_effect = ((PVEGame)base.Game).CreatePhysicalObj(m_targer.X, m_targer.Y, "skill", "asset.game.seven.jinqucd", "1", 1, 0);
	}

	private void AttackingB()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.MoveTo(player.X - 150, player.Y, "run", 1000, "", 18, NextAttackB);
		}
	}

	private void NextAttackB()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("beatB", 500, 0);
		base.Body.RangeAttacking(base.Body.X, base.Body.X + 170, "cry", 2500, null);
		base.Body.CallFuction(Comback, 3000);
	}

	private void Comback()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.MoveTo(181, base.Body.Y, "run", 1000, "", 18, ChangeDirection);
	}

	private void ChangeDirection()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
	}

	private void AttackingA()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.MoveTo(player.X - 150, player.Y, "run", 1000, "", 18, NextAttackA);
		}
	}

	private void NextAttackA()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("beatA", 500, 0);
		base.Body.RangeAttacking(base.Body.X, base.Body.X + 170, "cry", 2500, null);
		base.Body.CallFuction(Comback, 3000);
	}

	private void AttackingC()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			int x = player.X;
			if (base.Body.ShootPoint(x, player.Y, 83, 1000, 10000, 1, 1f, 3300))
			{
				base.Body.PlayMovie("beatC", 1500, 0);
			}
		}
	}

	private void AttackingD()
	{
		base.Body.MoveTo(1477, base.Body.Y, "run", 1000, "", 18, PersonalAttack);
	}

	private void PersonalAttack()
	{
		base.Body.Direction = -base.Body.Direction;
		base.Body.PlayMovie("beatD", 500, 8100);
		base.Body.CallFuction(GoMovie, 6100);
	}

	private void GoMovie()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			int x = player.X;
			base.Body.RangeAttacking(x - 200, x + 200, "cry", 1000, null);
			moive = ((PVEGame)base.Game).Createlayer(x, player.Y, "moive", "asset.game.seven.choud", "out", 1, 0);
			base.Body.CallFuction(GoAttacking, 2000);
		}
	}

	private void GoAttacking()
	{
		Player player = base.Game.FindRandomPlayer();
		if (!IsEixt && player != null)
		{
			m_effect = ((PVEGame)base.Game).CreatePhysicalObj(player.X, player.Y, "effect", "asset.game.seven.du", "1", 1, 0);
			IsEixt = true;
			List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
			foreach (Player item in allLivingPlayers)
			{
				int num = 140;
				if (item.X > player.X - num && item.X < player.X + num)
				{
					item.AddEffect(new ContinueReduceBlood(2, 200, item), 0);
				}
			}
		}
		base.Body.CallFuction(Comback, 1000);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
