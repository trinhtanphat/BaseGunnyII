using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FiveNormalFourNpc1 : ABrain
{
	private int m_attackTurn = 0;

	private int npcID2 = 5134;

	protected Living targer;

	private static string[] AllAttackChat = new string[3] { "Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ", "Hạ vũ khí xuống!", "Xem nếu bạn có thể đủ khả năng, một số ít!！" };

	private static string[] ShootChat = new string[3] { "Cho bạn biết những gì một cú sút vết nứt!", "Gửi cho bạn một quả bóng - bạn phải chọn Vâng", "Nhóm của bạn của những người dân thường ngu dốt và thấp" };

	private static string[] ShootedChat = new string[2] { "Ah ~ ~ Tại sao bạn tấn công? <br/> tôi đang làm gì?", "Oh ~ ~ nó thực sự đau khổ! Tại sao tôi phải chiến đấu? <br/> tôi phải chiến đấu ..." };

	private static string[] AddBooldChat = new string[3] { "Xoắn ah xoay ~ <br/>xoắn ah xoay ~ ~ ~", "~ Hallelujah <br/>Luyaluya ~ ~ ~", "Yeah Yeah Yeah, <br/> để thoải mái!" };

	private static string[] KillAttackChat = new string[1] { "Con rồng trong thế giới! !" };

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
		else if (m_attackTurn == 0)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			NextAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 5)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 6)
		{
			DameBoss();
			m_attackTurn++;
		}
		else if (m_attackTurn == 7)
		{
			DameBoss();
			m_attackTurn++;
		}
		else if (m_attackTurn == 8)
		{
			m_attackTurn++;
		}
		else
		{
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.PlayMovie("beat", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void DameBoss()
	{
		if (base.Body.Shoot(4, 1732, 380, 45, 7, 1, 15000))
		{
			base.Body.PlayMovie("beatC", 0, 0);
		}
		if (base.Body.Shoot(4, 1732, 380, 45, 7, 1, 15000))
		{
		}
		if (!base.Body.Shoot(4, 1732, 380, 45, 7, 1, 15000))
		{
		}
	}

	private void NextAttack()
	{
		if (base.Body.ShootPoint(920, base.Body.Y, 56, 1000, 10000, 1, 1f, 2300))
		{
			base.Body.PlayMovie("beat2", 1500, 0);
			base.Body.CallFuction(CreateChild, 4000);
		}
	}

	private void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID2, 1320, 700, 700, 1, -1);
	}
}
