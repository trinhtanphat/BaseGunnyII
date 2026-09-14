using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FiveNormalFourBoss : ABrain
{
	private int m_attackTurn = 0;

	private int npcID = 5132;

	private int isSay = 0;

	private int m_maxBlood;

	private int m_blood;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1500 && allFightPlayer.X < base.Game.Map.Info.ForegroundWidth + 1)
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
			KillAttack(1500, base.Game.Map.Info.ForegroundWidth + 1);
		}
		else if (m_attackTurn == 0)
		{
			BeatE();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			AllAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			AllAttack2();
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			Dame();
			m_attackTurn++;
		}
		else if (m_attackTurn == 5)
		{
			Summon();
			m_attackTurn++;
		}
		else if (m_attackTurn == 6)
		{
			AtoB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 7)
		{
			AtoB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 8)
		{
			Born();
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

	private void Born()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("born", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void BeatE()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatE", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void AllAttack()
	{
		base.Body.PlayMovie("beatA", 1000, 3000);
	}

	private void AllAttack2()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatB", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
	}

	private void Dame()
	{
		base.Body.PlayMovie("beatD", 1000, 3000);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			item.MoveTo(item.X - 400, base.Body.Y, "run", 0, "", 3);
			m_moive = ((PVEGame)base.Game).Createlayer(item.X, item.Y, "moive", "asset.game.4.tang", "out", 1, 0);
		}
	}

	private void PersonalAttack()
	{
		base.Body.PlayMovie("beatC", 3000, 5000);
		base.Body.CallFuction(OnPersonalAttack, 3000);
	}

	private void OnPersonalAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			int num = base.Game.Random.Next(player.Y + 10, player.Y + 10);
			if (base.Body.Shoot(0, player.X, player.Y, 66, 66, 1, 2550))
			{
				m_moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.4.guang", "out", 1, 0);
			}
		}
	}

	private void Summon()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.PlayMovie("beatE", 1000, 0);
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
		base.Body.CallFuction(Call, 4000);
	}

	private void AtoB()
	{
		base.Body.PlayMovie("AtoB", 1700, 2000);
	}

	public void Call()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 1000, 530, 430, 1, -1);
	}
}
