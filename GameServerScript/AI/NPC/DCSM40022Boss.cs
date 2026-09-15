using System.Drawing;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class DCSM40022Boss : ABrain
{
	private int m_attackTurn = 0;

	private Point[] brithPoint = new Point[5]
	{
		new Point(979, 630),
		new Point(1013, 630),
		new Point(1052, 630),
		new Point(1088, 630),
		new Point(1142, 630)
	};

	private static string[] AllAttackChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg1"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg2"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg3")
	};

	private static string[] ShootChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg4"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg5")
	};

	private static string[] KillPlayerChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg7")
	};

	private static string[] CallChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg8"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg9")
	};

	private static string[] JumpChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg10"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg11"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg12")
	};

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
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1000)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (m_attackTurn == 0)
		{
			PersonalAttackC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			PersonalAttackE();
			m_attackTurn++;
		}
		else
		{
			KillAttack();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatF", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void KillAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			int num = base.Game.Random.Next(0, KillAttackChat.Length);
			base.Body.Say(KillAttackChat[num], 1, 1000);
			base.Body.CurrentDamagePlus = 15f;
			base.Body.PlayMovie("beatF", 3000, 0);
			base.Body.RangeAttacking(0, base.Body.X + 1000, "cry", 5000, null);
		}
	}

	private void PersonalAttackC()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 5f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
			base.Body.PlayMovie("beatC", 1700, 0);
			base.Body.RangeAttacking(0, base.Body.X + 1000, "cry", 4000, null);
		}
	}

	private void PersonalAttackE()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 10f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
			base.Body.PlayMovie("beatE", 1700, 0);
			base.Body.RangeAttacking(0, base.Body.X + 1000, "cry", 4000, null);
		}
	}
}
}
