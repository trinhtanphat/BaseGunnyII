using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirteenTerrorBrotherNpc : ABrain
{
	private int m_attackTurn = 0;

	private int isSay = 0;

	private int IsEixt = 0;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private PhysicalObj wallLeft = null;

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
			Jump();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			JumpPersonalAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			FallSummon();
			m_attackTurn++;
		}
		else
		{
			Healing();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void JumpPersonalAttack()
	{
		base.Body.PlayMovie("walk", 0, 500);
		base.Body.JumpTo(base.Body.X, base.Body.Y - 150, "", 0, 1, PersonalAttack);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -107, 83, 100);
	}

	private void PersonalAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		if (player != null)
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -107, 83, 100);
			int num = base.Game.Random.Next(player.X - 10, player.Y + 20);
			if (base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 1, 3f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	public void Jump()
	{
		base.Body.PlayMovie("walk", 700, 0);
		base.Body.JumpTo(base.Body.X, base.Body.Y - 150, "", 1000, 1, CreateChild);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -107, 83, 100);
	}

	public void FallSummon()
	{
		base.Body.PlayMovie("walk", 700, 0);
		base.Body.FallFrom(base.Body.X, base.Body.Y + 150, "", 1000, 0, 50, Summon);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -107, 83, 100);
	}

	public void Summon()
	{
		base.Body.PlayMovie("call", 100, 0);
		wallLeft = ((PVEGame)base.Game).CreatePhysicalObj(1146, 566, "moive", "asset.game.ten.jitan", "beatA", 1, 0);
		base.Body.CallFuction(Remove, 1000);
	}

	public void Remove()
	{
		if (m_moive != null)
		{
			base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
			m_moive = null;
		}
		if (m_front != null)
		{
			base.Game.RemovePhysicalObj(m_front, sendToClient: true);
			m_front = null;
		}
	}

	public void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3300, null);
	}

	public void Healing()
	{
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(5000);
		base.Body.PlayMovie("castA", 100, 0);
		base.Body.Say("Hồi phục sức mạnh", 1, 0);
	}

	public void CreateChild()
	{
		base.Body.PlayMovie("call", 100, 0);
		m_moive = ((PVEGame)base.Game).Createlayer(1146, 566, "moive", "asset.game.ten.jitan", "out", 1, 0);
		m_front = ((PVEGame)base.Game).Createlayer(1146, 566, "font", "asset.game.ten.jitan", "out", 1, 0);
	}
}
}
