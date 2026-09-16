using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirteenHardBrynBoss : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_npc;

	private PhysicalObj m_moive;

	private int isSay = 0;

	private static string[] AllAttackChat = new string[3]
	{
		LanguageMgr.GetTranslation("Sư tử rống..."),
		LanguageMgr.GetTranslation("Sức mạnh của chúa rừng !"),
		LanguageMgr.GetTranslation("Sự đau đớn tột độ !")
	};

	private static string[] ShootChat = new string[2]
	{
		LanguageMgr.GetTranslation("Nén đá dấu tay ...!"),
		LanguageMgr.GetTranslation("Vũ khí của thần bộ lạc !")
	};

	private static string[] KillPlayerChat = new string[2]
	{
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg6"),
		LanguageMgr.GetTranslation("GameServerScript.AI.NPC.SimpleQueenAntAi.msg7")
	};

	private static string[] CallChat = new string[2]
	{
		LanguageMgr.GetTranslation("Vật tổ ..."),
		LanguageMgr.GetTranslation("Kill !")
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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 950 && allFightPlayer.X < 1250 && allFightPlayer.Y > 666)
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
			KillAttack(950, 1250);
		}
		else if (m_attackTurn == 0)
		{
			SummonA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			CreateMovie();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			PersonalAttack();
			m_attackTurn++;
		}
		else
		{
			PersonalAttack2();
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
		base.Body.CurrentDamagePlus = 20f;
		base.Body.PlayMovie("beatC", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void PersonalAttack()
	{
		m_moive.PlayMovie("beatA", 1000, 1000);
		base.Body.CallFuction(CallEffectB, 1000);
	}

	private void CallEffectB()
	{
		bool flag = false;
		Player[] allPlayers = base.Game.GetAllPlayers();
		List<Player> list = new List<Player>();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player.X > 990 && player.X < 1315 && player.Y < 606)
			{
				flag = true;
				list.Add(player);
				break;
			}
		}
		if (flag)
		{
			array = allPlayers;
			foreach (Player player2 in array)
			{
				player2.AddEffect(new DamageEffect(2), 0);
				player2.AddEffect(new GuardEffect(2), 0);
			}
		}
		base.Body.RangeAttacking(base.Body.X - 1500, base.Body.X + 1500, "cry", 1500, list);
	}

	private void PersonalAttack2()
	{
		int num = base.Game.Random.Next(0, ShootChat.Length);
		base.Body.Say(ShootChat[num], 1, 0);
		base.Body.CallFuction(DoudbleAttack, 2600);
		base.Body.CallFuction(DoudbleAttack, 6500);
	}

	private void DoudbleAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.CurrentDamagePlus = 1.8f;
		if (player != null)
		{
			int num = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, 55, 1000, 10000, 1, 1.5f, 2550))
			{
				base.Body.PlayMovie("beatB", 1700, 0);
			}
		}
	}

	private void SummonA()
	{
		base.Body.PlayMovie("callA", 3500, 0);
		base.Body.RangeAttacking(base.Body.X - 1500, base.Body.X + 1500, "cry", 5500, null);
		base.Body.CallFuction(GoMovie, 5500);
		base.Body.CallFuction(MovingPlayer, 6500);
	}

	private void MovingPlayer()
	{
		Player[] allPlayers = base.Game.GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			int x = base.Game.Random.Next(900, 1350);
			player.StartSpeedMult(x, player.Y);
		}
	}

	private void GoMovie()
	{
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			((PVEGame)base.Game).Createlayer(item.X, item.Y, "boom", "game.living.Living126", "beatA", 1, 0);
		}
	}

	public void CreateMovie()
	{
		m_moive = ((PVEGame)base.Game).CreatePhysicalObj(1146, 566, "moiveA", "asset.game.ten.jitan", "born", 1, 0);
		base.Body.CallFuction(SummonB, 1000);
	}

	private void SummonB()
	{
		base.Body.PlayMovie("callA", 3500, 0);
		base.Body.RangeAttacking(base.Body.X - 1500, base.Body.X + 1500, "cry", 5500, null);
		base.Body.CallFuction(GoMovie, 5500);
		base.Body.CallFuction(CallEffectA, 6500);
	}

	private void CallEffectA()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		int num = 0;
		foreach (Player item in allLivingPlayers)
		{
			if (num == 0)
			{
				item.AddEffect(new ReduceStrengthEffect(3, 110), 0);
			}
			if (num == 1)
			{
				item.AddEffect(new LockDirectionEffect(3), 0);
			}
			num++;
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
}
