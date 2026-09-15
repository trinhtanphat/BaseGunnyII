using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class SeventhNormalLongNpc : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj moive;

	private static string[] AllAttackChat = new string[1] { LanguageMgr.GetTranslation("Ddtank super là số 1") };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X < 870)
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
			KillAttack(0, base.Body.X + 100);
		}
		else if (m_attackTurn == 0)
		{
			AllAttack();
			m_attackTurn++;
		}
		else
		{
			Move();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void Move()
	{
		int x = base.Game.Random.Next(600, 750);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1200, "", 3);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 1f;
		base.Body.PlayMovie("beatB", 2000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3000, null);
	}

	private void AllAttack()
	{
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 6000, null);
		base.Body.CallFuction(GoMovie, 5000);
	}

	private void GoMovie()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			moive = ((PVEGame)base.Game).Createlayer(item.X, item.Y, "moive", "asset.game.seven.cao", "out", 1, 0);
			moive.PlayMovie("in", 1000, 0);
		}
	}
}
