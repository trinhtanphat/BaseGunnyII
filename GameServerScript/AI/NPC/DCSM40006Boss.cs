using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class DCSM40006Boss : ABrain
{
	private int m_attackTurn = 0;

	private int m_turn = 0;

	private PhysicalObj m_wallLeft = null;

	private PhysicalObj m_wallRight = null;

	private int IsEixt = 0;

	private int npcID = 1310;

	private static string[] AllAttackChat = new string[3] { "Trận động đất, bản thân mình! ! <br/> bạn vui lòng Ay giúp đỡ", "Hạ vũ khí xuống!", "Xem nếu bạn có thể đủ khả năng, một số ít!！" };

	private static string[] ShootChat = new string[3] { "Cho bạn biết những gì một cú sút vết nứt!", "Gửi cho bạn một quả bóng - bạn phải chọn Vâng", "Nhóm của bạn của những người dân thường ngu dốt và thấp" };

	private static string[] ShootedChat = new string[2] { "Ah ~ ~ Tại sao bạn tấn công? <br/> tôi đang làm gì?", "Oh ~ ~ nó thực sự đau khổ! Tại sao tôi phải chiến đấu? <br/> tôi phải chiến đấu ..." };

	private static string[] KillPlayerChat = new string[3] { "Mathias không kiểm soát tôi!", "Đây là thách thức số phận của tôi!", "Không! !Đây không phải là ý chí của tôi ..." };

	private static string[] AddBooldChat = new string[3] { "Xoắn ah xoay ~ <br/>xoắn ah xoay ~ ~ ~", "~ Hallelujah <br/>Luyaluya ~ ~ ~", "Yeah Yeah Yeah, <br/> để thoải mái!" };

	private static string[] KillAttackChat = new string[1] { "Con rồng trong thế giới! !" };

	private static string[] FrostChat = new string[3] { "Hương vị này", "Hãy để bạn bình tĩnh", "Bạn đã giận dữ với tôi." };

	private static string[] WallChat = new string[2] { "Chúa, cho tôi sức mạnh!", "Tuyệt vọng, xem tường thủy tinh của tôi!" };

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
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 620 && allFightPlayer.X < 1160)
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
			KillAttack(620, 1160);
		}
		else if (m_attackTurn == 0)
		{
			AllAttack();
			if (IsEixt == 1)
			{
				m_wallLeft.CanPenetrate = true;
				m_wallRight.CanPenetrate = true;
				base.Game.RemovePhysicalObj(m_wallLeft, sendToClient: true);
				base.Game.RemovePhysicalObj(m_wallRight, sendToClient: true);
				IsEixt = 0;
			}
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			ProtectingWall();
			m_attackTurn++;
		}
		else
		{
			CallBaby();
			m_attackTurn = 0;
		}
	}

	private void CallBaby()
	{
		base.Body.PlayMovie("renew", 3500, 0);
		base.Body.CallFuction(CreateChild, 6000);
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		if (m_turn == 0)
		{
			int num = base.Game.Random.Next(0, AllAttackChat.Length);
			base.Body.Say(AllAttackChat[num], 1, 13000);
			base.Body.PlayMovie("beat1", 15000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 17000, null);
			m_turn++;
		}
		else
		{
			int num = base.Game.Random.Next(0, AllAttackChat.Length);
			base.Body.Say(AllAttackChat[num], 1, 0);
			base.Body.PlayMovie("beat1", 1000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
		}
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		if (m_turn == 0)
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say(KillAttackChat[num], 1, 13000);
			base.Body.PlayMovie("beat1", 15000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 17000, null);
			m_turn++;
		}
		else
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say(KillAttackChat[num], 1, 0);
			base.Body.PlayMovie("beat1", 2000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
		}
	}

	private void ProtectingWall()
	{
		if (IsEixt == 0)
		{
			m_wallLeft = ((PVEGame)base.Game).CreatePhysicalObj(base.Body.X - 65, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_left", "1", 1, 0);
			m_wallRight = ((PVEGame)base.Game).CreatePhysicalObj(base.Body.X + 65, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_right", "1", 1, 0);
			m_wallLeft.SetRect(-165, -169, 43, 330);
			m_wallRight.SetRect(128, -165, 41, 330);
			IsEixt = 1;
		}
		int num = base.Game.Random.Next(0, WallChat.Length);
		base.Body.Say(WallChat[num], 1, 0);
	}

	public void CreateChild()
	{
		base.Body.PlayMovie("renew", 100, 2000);
		((SimpleBoss)base.Body).CreateChild(npcID, 520, 530, 400, 6, -1);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
}
