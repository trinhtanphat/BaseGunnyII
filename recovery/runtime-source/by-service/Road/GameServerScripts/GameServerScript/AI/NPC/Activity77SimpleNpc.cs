using System;
using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class Activity77SimpleNpc : ABrain
{
	private Player m_target;

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
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		if (!ShootLowestBooldPlayer())
		{
			RandomShootPlayer();
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private bool ShootLowestBooldPlayer()
	{
		List<Player> list = new List<Player>();
		foreach (Player allLivingPlayer in base.Game.GetAllLivingPlayers())
		{
			if ((double)allLivingPlayer.Blood < (double)allLivingPlayer.MaxBlood * 0.2)
			{
				list.Add(allLivingPlayer);
			}
		}
		if (list.Count > 0)
		{
			int index = base.Game.Random.Next(0, list.Count);
			m_target = list[index];
			NpcAttack();
			return true;
		}
		return false;
	}

	private void RandomShootPlayer()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		int index = base.Game.Random.Next(0, allLivingPlayers.Count);
		m_target = allLivingPlayers[index];
		NpcAttack();
	}

	private void NpcAttack()
	{
		int num = 1;
		if (m_target.X > base.Body.X)
		{
			num = 1;
			base.Body.ChangeDirection(1, 0);
		}
		else
		{
			num = -1;
			base.Body.ChangeDirection(-1, 0);
		}
		int num2 = Math.Abs(m_target.X - base.Body.X);
		if (num2 < 300)
		{
			ShootAttack();
			return;
		}
		int num3 = base.Game.Random.Next(((SimpleBoss)base.Body).NpcInfo.MoveMin, ((SimpleBoss)base.Body).NpcInfo.MoveMax) * 3;
		if (num3 > num2)
		{
			num3 = num2 - 300;
		}
		num3 *= num;
		if (!base.Body.MoveTo(base.Body.X + num3, m_target.Y - 20, "walk", 0, "", ((SimpleBoss)base.Body).NpcInfo.speed, ShootAttack))
		{
			ShootAttack();
		}
	}

	private void ShootAttack()
	{
		int num = Math.Abs(m_target.X - base.Body.X);
		int num2 = 30;
		num2 = ((num < 200) ? 10 : ((num >= 500) ? 50 : 30));
		int x = base.Game.Random.Next(m_target.X - num2, m_target.X + num2);
		if (base.Body.ShootPoint(x, m_target.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 2f, 1700))
		{
			base.Body.PlayMovie("beat", 1700, 0);
		}
	}
}
