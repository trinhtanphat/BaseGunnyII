using System.Collections.Generic;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class WorldBatKing : ABrain
{
	private int m_attackTurn = 0;

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
		if (base.Body.State == 10)
		{
			KillAttack(0, base.Game.Map.Info.ForegroundWidth + 1);
		}
		else if (m_attackTurn == 0)
		{
			AttackA();
			m_attackTurn++;
		}
		else
		{
			AttackB(0, base.Game.Map.Info.ForegroundWidth + 1);
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void AttackA()
	{
		int x = base.Game.Random.Next(173, 1439);
		int y = base.Game.Random.Next(150, 700);
		base.Body.MoveTo(x, y, "fly", 3000, "", 12, AllAttackA);
	}

	private void AllAttackA()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 10f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			base.Body.PlayMovie("beatA", 1700, 0);
			base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 4000, null);
		}
	}

	private void AttackB(int fx, int tx)
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 15f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			base.Body.PlayMovie("beatB", 1900, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
			base.Body.CallFuction(GoMovie, 4000);
		}
	}

	private void GoMovie()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
		}
		base.Body.CallFuction(ShowIn, 2000);
	}

	private void ShowIn()
	{
		base.Body.PlayMovie("in", 100, 0);
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		base.Body.Say(KillPlayerChat[num], 1, 0, 2000);
	}
}
