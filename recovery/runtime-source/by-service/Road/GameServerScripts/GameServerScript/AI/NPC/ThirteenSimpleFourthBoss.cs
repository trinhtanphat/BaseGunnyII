using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirteenSimpleFourthBoss : ABrain
{
	public int attackingTurn = 0;

	private Player target = null;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private static string[] AllAttackChat = new string[1] { "Bạn sẽ trả giá cho việc này ! " };

	private static string[] ShootChat = new string[3] { "Tôi không muốn chỉ là lãng phí khi bạn đánh bại!", "Oh, bạn chơi tốt đấy, <br/>ha ha ha ha!", "Xem tôi là danh dự của bạn!" };

	private static string[] CallChat = new string[1] { "Các, <br/>Boom con của ta !" };

	private static string[] AngryChat = new string[1] { "Sức mạnh cuối cùng !" };

	private static string[] KillAttackChat = new string[1] { "I want kill you ?" };

	private static string[] SealChat = new string[1] { "Hãy đón nhận cái nón đến tột cùng !" };

	private static string[] KillPlayerChat = new string[2] { "Địa ngục là điểm đến duy nhất của ngươi !", "Quá dễ để ta tiêu diệt." };

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
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X < 158)
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
			KillAttack(0, 300);
		}
		else if (attackingTurn == 0)
		{
			CallDeadZone();
			attackingTurn++;
		}
		else if (attackingTurn == 1)
		{
			AttackInDeadZone();
			attackingTurn++;
		}
		else if (attackingTurn == 2)
		{
			PersonalActack();
			attackingTurn++;
		}
		else
		{
			AllAttack();
			attackingTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void CallDeadZone()
	{
		base.Body.PlayMovie("beatA", 2000, 0);
		base.Body.CallFuction(CreateMovie, 4000);
	}

	public void CreateMovie()
	{
		m_moive = ((PVEGame)base.Game).Createlayer(800, base.Body.Y, "moive", "asset.game.ten.tedabiaoji", "out", 1, 0);
	}

	public void CreateEffect()
	{
		if (target != null)
		{
			m_front = ((PVEGame)base.Game).Createlayer(target.X, target.Y, "effect", "asset.game.ten.qunbao", "out", 1, 0);
		}
	}

	public void AllAttack()
	{
		base.Body.PlayMovie("beatA", 1000, 1000);
		base.Body.RangeAttacking(base.Body.X - 1500, base.Body.X + 1500, "cry", 4000, null);
		base.Body.CallFuction(MultiMovie, 4000);
		base.Body.CallFuction(Out, 5000);
	}

	private void MultiMovie()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			m_moive = ((PVEGame)base.Game).Createlayer(item.X, item.Y, "boom", "asset.game.ten.qunbao", "out", 1, 0);
		}
	}

	public void Out()
	{
		if (m_moive != null)
		{
			base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
			m_moive = null;
		}
		if (m_front != null)
		{
			base.Game.RemovePhysicalObj(m_front, sendToClient: true);
			m_front = null;
		}
	}

	public void AttackInDeadZone()
	{
		int num = base.Game.Random.Next(0, AngryChat.Length);
		base.Body.Say(AngryChat[num], 1, 0);
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.CurrentDamagePlus = 10f;
		base.Body.RangeAttacking(base.Body.X, 1400, "cry", 4000, null);
		base.Body.CallFuction(Out, 5000);
	}

	public void PersonalActack()
	{
		base.Body.PlayMovie("beatA", 1000, 1000);
		target = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 2f;
		base.Body.RangeAttacking(target.X - 20, target.X + 20, "cry", 2000, null);
		base.Body.CallFuction(CreateEffect, 2000);
		base.Body.CallFuction(Out, 3000);
	}

	public void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 100f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3300, null);
	}
}
