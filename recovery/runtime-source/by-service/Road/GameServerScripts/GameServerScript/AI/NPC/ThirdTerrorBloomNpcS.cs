using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirdTerrorBloomNpcS : ABrain
{
	private Player m_target = null;

	private int m_targetDis = 0;

	private int attackingTurn = 1;

	private int IsEixt = 0;

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
		bool flag = false;
		int num = 0;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		m_target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		m_targetDis = (int)m_target.Distance(base.Body.X, base.Body.Y);
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
		else
		{
			if (flag)
			{
				return;
			}
			if (attackingTurn == 1)
			{
				if (m_targetDis < 100)
				{
					List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
					foreach (Player item in allLivingPlayers)
					{
						item.AddBlood(700);
					}
					base.Body.PlayMovie("renew", 700, 400);
				}
				else
				{
					MoveToPlayer(m_target);
				}
			}
			else if (attackingTurn == 2)
			{
				if (m_targetDis < 100)
				{
					List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
					foreach (Player item2 in allLivingPlayers)
					{
						item2.AddBlood(700);
					}
					base.Body.PlayMovie("renew", 700, 400);
				}
				else
				{
					MoveToPlayer(m_target);
				}
			}
			else if (attackingTurn == 3)
			{
				if (m_targetDis < 100)
				{
					List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
					foreach (Player item3 in allLivingPlayers)
					{
						item3.AddBlood(700);
					}
					base.Body.PlayMovie("renew", 700, 400);
				}
				else
				{
					MoveToPlayer(m_target);
				}
			}
			else if (attackingTurn == 4)
			{
				if (m_targetDis < 100)
				{
					List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
					foreach (Player item4 in allLivingPlayers)
					{
						item4.AddBlood(700);
					}
					base.Body.PlayMovie("renew", 700, 400);
				}
				else
				{
					MoveToPlayer(m_target);
				}
			}
			else if (attackingTurn == 5)
			{
				base.Body.PlayMovie("die", 700, 400);
				base.Body.CallFuction(MoveTo, 1500);
			}
			else
			{
				attackingTurn = 1;
			}
			attackingTurn++;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void KillAttack(int fx, int mx)
	{
	}

	public void MoveToPlayer(Player player)
	{
		base.Body.Say("Đến gần tui sẻ hồi máu cho!", 0, 2000);
	}

	public void MoveTo()
	{
		if (IsEixt == 1)
		{
			base.Body.JumpToSpeed(478, 560, "born", 0, 0, 36, null);
			IsEixt = 0;
		}
		else
		{
			base.Body.JumpToSpeed(1000, 560, "born", 0, 0, 36, null);
			IsEixt = 1;
		}
	}

	public void Beat()
	{
		if (m_targetDis >= 100)
		{
			return;
		}
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			item.AddBlood(700);
		}
		base.Body.PlayMovie("renew", 700, 400);
	}
}
