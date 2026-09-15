using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirdNormalKingFirst : ABrain
{
	private int m_attackTurn = 0;

	private SimpleBoss m_boss = null;

	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingMoive2;

	private PhysicalObj m_kingMoive3;

	private PhysicalObj m_kingMoive4;

	private PhysicalObj m_wallLeft = null;

	private int npcID = 3106;

	private int npcID2 = 3112;

	private static string[] AllAttackChat = new string[3] { "Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ", "Hạ vũ khí xuống!", "Xem nếu bạn có thể đủ khả năng, một số ít!！" };

	private static string[] CallChat = new string[2] { "Vệ binh! <br/> bảo vệ! ! ", "Boo đệm! ! <br/> cung cấp cho tôi một số trợ giúp!" };

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
			In();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			Summon2();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			Jump();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			Jump2();
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
		base.Body.Say(KillAttackChat[num], 0, 1000);
		base.Body.PlayMovie("beat", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void In()
	{
		base.Body.PlayMovie("castA", 3000, 0);
		base.Body.Say("Kiên cố!", 0, 0);
		base.Body.CallFuction(Star, 4000);
	}

	private void Star()
	{
		base.Body.CurrentDamagePlus = 1f;
		Player player = base.Game.FindRandomPlayer();
		m_kingMoive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "1", "asset.game.4.dici", "out", 1, 0);
		Player player2 = base.Game.FindRandomPlayer();
		m_kingMoive2 = ((PVEGame)base.Game).Createlayer(player2.X, player2.Y, "2", "asset.game.4.dici", "out", 1, 0);
		Player player3 = base.Game.FindRandomPlayer();
		m_kingMoive3 = ((PVEGame)base.Game).Createlayer(player3.X, player3.Y, "3", "asset.game.4.dici", "out", 1, 0);
		Player player4 = base.Game.FindRandomPlayer();
		m_kingMoive4 = ((PVEGame)base.Game).Createlayer(player4.X, player4.Y, "4", "asset.game.4.dici", "out", 1, 0);
		base.Body.RangeAttacking(player.X + 10, player.X - 10, "cry", 3000, null);
		base.Body.RangeAttacking(player2.X + 10, player2.X - 10, "cry", 3000, null);
		base.Body.RangeAttacking(player3.X + 10, player3.X - 10, "cry", 3000, null);
		base.Body.RangeAttacking(player4.X + 10, player4.X - 10, "cry", 3000, null);
		base.Body.CallFuction(Remove, 800);
	}

	public void Remove()
	{
		if (m_kingMoive != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
			m_kingMoive = null;
		}
		if (m_kingMoive2 != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive2, sendToClient: true);
			m_kingMoive2 = null;
		}
		if (m_kingMoive3 != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive3, sendToClient: true);
			m_kingMoive3 = null;
		}
		if (m_kingMoive4 != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive4, sendToClient: true);
			m_kingMoive4 = null;
		}
	}

	private void Jump()
	{
		base.Body.JumpTo(base.Body.X, base.Body.Y - 130, "", 1000, 1, 12, NextAttack);
	}

	private void Jump2()
	{
		base.Body.PlayMovie("walk", 100, 1000);
		base.Body.FallFromTo(base.Body.X, base.Body.Y + 260, "", 1000, 0, 25, NextAttack);
	}

	public void Healing()
	{
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(5000);
		base.Body.PlayMovie("castA", 100, 400);
		base.Body.Say("Hồi phục sức mạnh", 0, 0);
	}

	private void Summon2()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 0, 3300);
		base.Body.PlayMovie("call", 3500, 0);
		base.Body.CallFuction(CreateChild2, 4000);
	}

	public void CreateChild2()
	{
		((SimpleBoss)base.Body).CreateBoss(npcID, 1352, 227, -1, 430, 1, "");
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
