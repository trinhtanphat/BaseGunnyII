using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirteenSimpleNioBoss : ABrain
{
	private int m_attackTurn = 0;

	private static string[] AllAttackChat = new string[3] { "Địa chấn ! <br/> Thật đáng sợ", "Đặt hết vũ khí xuống !", "Nhìn bạn cũng có thể có thể chịu được một số ít!" };

	private static string[] ShootChat = new string[3] { "Cảm nhận sức mạnh của ta !", "Gửi cho cậu nhưng viên kheo đau khổ", "Cho các ngươi nén mùi lợi hại " };

	private static string[] ShootedChat = new string[2] { "哎呀~~你们为什么要攻击我？<br/>我在干什么？", "噢~~好痛!我为什么要战斗？<br/>我必须战斗…" };

	private static string[] AddBooldChat = new string[3] { "Xoay xoay ~ <br/> xoay ah xoay ~ ~", "Hallelujah ~ <br/> Luyaluya ~ ~", "Kì diệu quá! Đã đem đến cho ta sức mạnh siêu phàm !" };

	private static string[] KillAttackChat = new string[1] { "君临天下！！" };

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
			NextAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			Healing();
			m_attackTurn++;
		}
		else
		{
			AllAttack();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.PlayMovie("beatA", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 0);
		base.Body.PlayMovie("beat", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 4000, base.Body.X + 4000, "cry", 3000, null);
	}

	private void Healing()
	{
		int num = base.Game.Random.Next(0, AddBooldChat.Length);
		base.Body.Say(AddBooldChat[num], 1, 0);
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(7500);
		base.Body.PlayMovie("renew", 1000, 4500);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 800);
		}
		else
		{
			base.Body.ChangeDirection(-1, 800);
		}
		base.Body.CurrentDamagePlus = 0.8f;
		int num = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num], 1, 0);
		if (player != null)
		{
			int x = base.Game.Random.Next(player.X - 30, player.X + 30);
			if (base.Body.ShootPoint(x, player.Y, 61, 1400, 10000, 1, 1.5f, 2300))
			{
				base.Body.PlayMovie("beat2", 1500, 0);
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
