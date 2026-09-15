using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class NormalQueenAntAi : ABrain
{
	private int m_attackTurn = 0;

	private int npcID = 2104;

	private int isSay = 0;

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
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg1"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg2"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg3")
	};

	private static string[] ShootChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg4"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg5")
	};

	private static string[] KillPlayerChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg7")
	};

	private static string[] CallChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg8"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg9")
	};

	private static string[] JumpChat = new string[3]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg10"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg11"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg12")
	};

	private static string[] KillAttackChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg13"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg14")
	};

	private static string[] ShootedChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg15"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg16")
	};

	private static string[] DiedChat = new string[1] { LanguageMgr.GetTranslation("GameServerScript.AI.NPC.NormalQueenAntAi.msg17") };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1169 && allFightPlayer.X < base.Game.Map.Info.ForegroundWidth + 1)
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
			KillAttack(1169, base.Game.Map.Info.ForegroundWidth + 1);
		}
		else if (m_attackTurn == 0)
		{
			if (((PVEGame)base.Game).GetLivedLivings().Count == 9)
			{
				PersonalAttack();
			}
			else
			{
				Summon();
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
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 0.8f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(670, 880);
			if (base.Body.ShootPoint(player.X, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	private void Summon()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 600);
		base.Body.PlayMovie("call", 1700, 2000);
		base.Body.CallFuction(Call, 2000);
	}

	private void Call()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, brithPoint, 9, 3, 9, -1);
	}

	public void OnShootedSay(int delay)
	{
		int num = base.Game.Random.Next(0, ShootedChat.Length);
		if (isSay == 0 && base.Body.IsLiving)
		{
			base.Body.Say(ShootedChat[num], 1, delay, 0);
			isSay = 1;
		}
		if (!base.Body.IsLiving)
		{
			num = base.Game.Random.Next(0, DiedChat.Length);
			base.Body.Say(DiedChat[num], 1, delay - 800, 2000);
		}
	}
}
