using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirdNormalKingSecond : ABrain
{
	private int m_attackTurn = 0;

	private int npcID = 3106;

	private int npcID2 = 3110;

	private PhysicalObj m_kingMoive;

	private static string[] AllAttackChat = new string[3] { "Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ", "Hạ vũ khí xuống!", "Xem nếu bạn có thể đủ khả năng, một số ít!！" };

	private static string[] CallChat = new string[2] { "Vệ binh! <br/> bảo vệ! ! ", "Boo đệm! ! <br/> cung cấp cho tôi một số trợ giúp!" };

	private static string[] ShootChat = new string[3] { "Nếm thử cái này!", "Gửi cho bạn một quả bóng - bạn phải chọn Vâng", "Nhóm của bạn của những người dân thường ngu dốt và thấp" };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 0 && allFightPlayer.X < 300)
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
		else if (m_attackTurn == 0)
		{
			Jump2();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			Star();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			Jump();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			Summon();
			m_attackTurn++;
		}
		else
		{
			Healing();
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

	private void Star()
	{
		m_kingMoive = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y - 10, "moive", "asset.game.4.buff", "out", 1, 1);
		base.Body.CallFuction(Remove, 1000);
	}

	public void Remove()
	{
		if (m_kingMoive != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
			m_kingMoive = null;
		}
	}

	private void Jump()
	{
		base.Body.JumpTo(base.Body.X, base.Body.Y - 130, "", 1000, 1, 12, NextAttack);
	}

	private void Jump2()
	{
		base.Body.PlayMovie("walk", 100, 1000);
		base.Body.FallFromTo(base.Body.X, base.Body.Y + 130, "", 1000, 0, 25, NextAttack);
	}

	public void Healing()
	{
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(5000);
		base.Body.PlayMovie("castA", 100, 0);
		base.Body.Say("Định đánh bại bọn ta à, không dể đâu!", 0, 0);
	}

	private void Summon()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 0, 3300);
		base.Body.PlayMovie("call", 3500, 0);
		base.Body.CallFuction(CreateChild, 4000);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID2, 827, 628, 430, 1, -1);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 0.8f;
		int num = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num], 0, 0);
		if (player != null)
		{
			int x = base.Game.Random.Next(player.X - 20, player.X + 20);
			if (base.Body.ShootPoint(x, player.Y, 54, 1000, 10000, 1, 1f, 2300))
			{
				base.Body.PlayMovie("beatA", 1500, 0);
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
