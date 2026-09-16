using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirteenSimpleBrotherNpc : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive;

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
		if (m_attackTurn == 0)
		{
			CallBuff();
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

	private void PersonalAttack()
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
		base.Body.RangeAttacking(920, 1370, "cry", 1000, list);
		base.Body.PlayMovie("call", 1000, 1000);
		base.Body.CallFuction(Remove, 1000);
	}

	public void CallBuff()
	{
		base.Body.JumpTo(base.Body.X, base.Body.Y - 300, "walk", 2000, -1);
		base.Body.PlayMovie("call", 1000, 1000);
		base.Body.CallFuction(CreateMovie, 1000);
	}

	public void Remove()
	{
		if (m_moive != null)
		{
			m_moive.PlayMovie("beatA", 1000, 1000);
		}
	}

	public void CreateMovie()
	{
		m_moive = ((PVEGame)base.Game).CreatePhysicalObj(1146, 566, "moiveA", "asset.game.ten.jitan", "born", 1, 0);
	}
}
}
