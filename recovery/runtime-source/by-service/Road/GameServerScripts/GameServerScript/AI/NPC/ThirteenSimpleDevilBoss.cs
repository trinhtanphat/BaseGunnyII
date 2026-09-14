using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirteenSimpleDevilBoss : ABrain
{
	private int m_attackTurn = 0;

	private int npcID2 = 5323;

	private PhysicalObj moive;

	private PhysicalObj front;

	private PhysicalObj wallLeft = null;

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
		else if (base.Body.State != 1)
		{
			if (m_attackTurn == 0)
			{
				CallBell();
				m_attackTurn++;
			}
			else if (m_attackTurn == 1)
			{
				BeatE();
				m_attackTurn++;
			}
			else
			{
				BlackAttack();
				m_attackTurn = 0;
			}
		}
	}

	private void CallBell()
	{
		base.Body.PlayMovie("beatB", 3300, 0);
	}

	private void BlackAttack()
	{
		base.Body.PlayMovie("beatA", 0, 3000);
		moive = ((PVEGame)base.Game).Createlayer(base.Body.X, base.Body.Y, "top", "asset.game.4.heip", "out", 2, 1);
		base.Body.CallFuction(AllAttack, 2500);
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 2.5f;
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 500, null);
		base.Body.CallFuction(RemoveMove, 0);
	}

	private void RemoveMove()
	{
		if (moive != null)
		{
			base.Game.RemovePhysicalObj(moive, sendToClient: true);
			moive = null;
		}
		if (front != null)
		{
			base.Game.RemovePhysicalObj(front, sendToClient: true);
			front = null;
		}
	}

	private void BeatE()
	{
		Player player = base.Game.FindRandomPlayer();
		int x = player.X;
		int y = player.Y - 95;
		base.Body.MoveTo(x, y, "fly", 1000, "", 16, PersonalAttack);
	}

	private void PersonalAttack()
	{
		base.Body.CurrentDamagePlus = 5.5f;
		base.Body.PlayMovie("beatE", 3500, 0);
		Player player = base.Game.FindRandomPlayer();
		base.Body.RangeAttacking(player.X - 50, player.X + 50, "cry", 5000, null);
		base.Body.CallFuction(Run, 5000);
	}

	private void Run()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.MoveTo(850, 770, "fly", 1000, "", 16, ChangeDirection);
	}

	private void ChangeDirection()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
	}

	private void CallHopquaidi()
	{
		base.Body.CurrentDamagePlus = 0.8f;
		int num = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num], 1, 0);
		int num2 = base.Game.Random.Next(400, 1300);
		Player player = base.Game.FindRandomPlayer();
		int x = player.X;
		int y = player.Y - 150;
		base.Body.MoveTo(x, y, "fly", 3500, "", 16, CallHopquaidi2);
		base.Body.PlayMovie("beatD", 3300, 2000);
	}

	private void BeatDame()
	{
		Player player = base.Game.FindRandomPlayer();
		int x = player.X;
		int y = player.Y - 150;
		base.Body.MoveTo(x, y, "fly", 3500, "", 16, GoBeatDame);
	}

	private void GoBeatDame()
	{
		int num = base.Game.Random.Next(0, AddBooldChat.Length);
		base.Body.Say(AddBooldChat[num], 1, 0);
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.SyncAtTime = true;
		base.Body.PlayMovie("beatE", 3300, 5000);
		base.Body.RangeAttacking(base.Body.X - 100, base.Body.X + 100, "cry", 5000, null);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.PlayMovie("beat", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void CallHopquaidi2()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		int x = base.Game.Random.Next(700, 1300);
		base.Body.SetXY(base.Body.X, 600);
		((SimpleBoss)base.Body).CreateChild(npcID2, x, 900, 1000, 1, -1);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
