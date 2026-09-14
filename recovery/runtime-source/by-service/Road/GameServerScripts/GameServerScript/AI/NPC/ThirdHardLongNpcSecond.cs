using System;
using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirdHardLongNpcSecond : ABrain
{
	private int m_attackTurn = 0;

	private int isSay = 0;

	private static string[] AllAttackChat = new string[1] { LanguageMgr.GetTranslation("Ddtank Vn là số 1") };

	private static string[] ShootChat = new string[1] { LanguageMgr.GetTranslation("Anh em tiến lên !") };

	private static string[] KillPlayerChat = new string[1] { LanguageMgr.GetTranslation("Anh em tiến lên !") };

	private static string[] CallChat = new string[1] { LanguageMgr.GetTranslation("Ai giết được chúng sẻ được ban thưởng !") };

	private static string[] JumpChat = new string[1] { LanguageMgr.GetTranslation("Ai giết được chúng sẻ được ban thưởng !") };

	private static string[] KillAttackChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg13"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg14")
	};

	private static string[] ShootedChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg15"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg16")
	};

	private static string[] DiedChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg17") };

	private static Random random = new Random();

	private static string[] listChat = new string[13]
	{
		"Để tôn vinh! Để giành chiến thắng! !", "Tổ chức cướp vũ khí của họ, không run sợ!", "Super Ddtank muôn năm !", "Kẻ thù ở phía trước, sẵn sàng chiến đấu!", "Cảm thấy hành vi của nhà vua và bất thường hơn ...", "Để Boo Goo chiến thắng! ! Brothers phí!", "Nhanh chóng để tiêu diệt kẻ thù!", "Sức mạnh số 1 !", "Với một sửa chữa nhanh chóng!", "Vây quanh kẻ thù và tiêu diệt chúng.",
		"Quân tiếp viện! Quân tiếp viện! Chúng tôi cần thêm quân tiếp viện! !", "Hy sinh bản thân, sẽ không cho phép bạn có được đi với.", "Đừng đánh giá thấp sức mạnh của Boo Goo, nếu không bạn sẽ phải trả cho việc này."
	};

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		isSay = 0;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
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
			if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
			{
				PersonalAttack();
			}
			else
			{
				PersonalAttack();
			}
			m_attackTurn++;
		}
		else
		{
			PersonalAttack();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.CurrentDamagePlus = 10f;
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void PersonalAttack()
	{
		int x = base.Game.Random.Next(50, 200);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 3, NextAttack);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 0.8f;
			if (player.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 50);
			}
			else
			{
				base.Body.ChangeDirection(-1, 50);
			}
			int num = base.Game.Random.Next(player.X, player.X);
			if (base.Body.ShootPoint(1100, player.Y, 58, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		base.Body.Say(KillPlayerChat[num], 1, 0, 2000);
	}

	public override void OnDiedSay()
	{
	}

	private void CreateChild()
	{
	}

	public override void OnShootedSay()
	{
		int num = base.Game.Random.Next(0, ShootedChat.Length);
		if (isSay == 0 && base.Body.IsLiving)
		{
			base.Body.Say(ShootedChat[num], 1, 900, 0);
			isSay = 1;
		}
		if (!base.Body.IsLiving)
		{
			num = base.Game.Random.Next(0, DiedChat.Length);
			base.Body.Say(DiedChat[num], 1, 100, 2000);
		}
	}

	public static string GetOneChat()
	{
		int num = random.Next(0, listChat.Length);
		return listChat[num];
	}

	public static void LivingSay(List<Living> livings)
	{
		if (livings == null || livings.Count == 0)
		{
			return;
		}
		int num = 0;
		int count = livings.Count;
		foreach (Living living in livings)
		{
			living.IsSay = false;
		}
		num = ((count <= 5) ? random.Next(0, 2) : ((count <= 5 || count > 10) ? random.Next(1, 4) : random.Next(1, 3)));
		if (num <= 0)
		{
			return;
		}
		int[] array = new int[num];
		int num2 = 0;
		while (num2 < num)
		{
			int index = random.Next(0, count);
			if (!livings[index].IsSay)
			{
				livings[index].IsSay = true;
				int delay = random.Next(0, 5000);
				livings[index].Say(GetOneChat(), 0, delay);
				num2++;
			}
		}
	}
}
