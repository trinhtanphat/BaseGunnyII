using System.Collections.Generic;
using System.Drawing;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class WorldSoccerCaptain : ABrain
{
	private int m_attackTurn = 0;

	private int isSay = 0;

	private PhysicalObj moive;

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1225 && allFightPlayer.X < 1571)
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
			KillAttack(0, base.Game.Map.Info.ForegroundWidth + 1);
		}
		else if (m_attackTurn == 0)
		{
			AttackA(0, base.Game.Map.Info.ForegroundWidth + 1);
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			AttackB(0, base.Game.Map.Info.ForegroundWidth + 1);
			m_attackTurn++;
		}
		else
		{
			AttackC(0, base.Game.Map.Info.ForegroundWidth + 1);
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
		base.Body.CurrentDamagePlus = 100f;
		base.Body.PlayMovie("beatD", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void AttackA(int fx, int tx)
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 10f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
			base.Body.PlayMovie("beatA", 1700, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
		}
	}

	private void AttackB(int fx, int tx)
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 15f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
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
			moive = ((PVEGame)base.Game).Createlayer(item.X, item.Y, "moive", "asset.game.zero.294b", "out", 1, 0);
		}
	}

	private void AttackC(int fx, int tx)
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 20f;
			int num = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num], 1, 0);
			int num2 = base.Game.Random.Next(0, 1200);
			base.Body.PlayMovie("beatC", 1700, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
		}
	}

	public override void OnKillPlayerSay()
	{
		base.OnKillPlayerSay();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		base.Body.Say(KillPlayerChat[num], 1, 0, 2000);
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
}
