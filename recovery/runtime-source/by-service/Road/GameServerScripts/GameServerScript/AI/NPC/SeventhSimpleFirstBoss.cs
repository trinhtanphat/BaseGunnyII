using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class SeventhSimpleFirstBoss : ABrain
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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 1344)
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
			KillAttack(1344, base.Game.Map.Info.ForegroundWidth + 1);
		}
		else if (m_attackTurn == 0)
		{
			Summon(0);
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			Shield();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			Summon(1);
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			Shield();
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			Summon(2);
			m_attackTurn++;
		}
		else
		{
			Shield();
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

	public void Summon(int type)
	{
		base.Body.PlayMovie("Ato", 100, 0);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-56, -122, 124, 129);
		switch (type)
		{
		case 1:
			base.Body.CallFuction(PersonalAttackDame, 2500);
			break;
		case 2:
			base.Body.CallFuction(AllAttack, 2500);
			break;
		default:
			base.Body.CallFuction(PersonalAttack, 2500);
			break;
		}
	}

	private void AllAttack()
	{
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(0, base.Body.X, "cry", 6000, null);
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

	private void PersonalAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 0.8f;
			if (base.Body.ShootPoint(player.X, player.Y, 84, 1200, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	public void Shield()
	{
		base.Body.State = 1;
		base.Body.PlayMovie("toA", 2700, 0);
		((SimpleBoss)base.Body).SetRelateDemagemRect(0, 0, 124, 129);
	}

	private void PersonalAttackDame()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			base.Body.CurrentDamagePlus = 1f;
			int num = base.Game.Random.Next(player.X, player.X);
			if (base.Body.ShootPoint(player.X, player.Y, 84, 1200, 10000, 1, 3f, 2650))
			{
				base.Body.PlayMovie("beat", 1700, 0);
			}
		}
	}
}
