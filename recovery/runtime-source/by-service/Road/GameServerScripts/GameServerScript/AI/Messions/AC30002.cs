using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class AC30002 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss boss2 = null;

	private int m_state = 30002;

	private int bossID1 = 30002;

	private int bossID2 = 30003;

	private int kill = 0;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1750)
		{
			return 3;
		}
		if (score > 1675)
		{
			return 2;
		}
		if (score > 1600)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[1] { bossID1 };
		int[] npcIds2 = new int[1] { bossID1 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/0/294b.swf", "asset.game.zero.294b");
		base.Game.SetMap(1250);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		livingConfig.IsWorldBoss = true;
		boss = base.Game.CreateBoss(bossID1, 944, 581, -1, 0, "", livingConfig);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		boss.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.DCSM2002.msg1"), 0, 200, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > 1)
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
	}

	public override bool CanGameOver()
	{
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!boss.IsLiving && m_state == bossID1)
		{
			m_state++;
		}
		if (m_state == bossID2 && boss2 == null)
		{
			boss2 = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 0, "");
			base.Game.RemoveLiving(boss.Id);
			if (boss2.Direction == 1)
			{
				boss2.SetRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			}
			boss2.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			Player player = base.Game.FindRandomPlayer();
			int num = 0;
			if (player != null)
			{
				num = player.Delay;
			}
			foreach (Player item in allFightPlayers)
			{
				if (item.Delay < num)
				{
					num = item.Delay;
				}
			}
			boss2.AddDelay(num - 2000);
		}
		if (boss != null && !boss.IsLiving)
		{
			kill++;
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (boss != null && !boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
			base.Game.IsKillWorldBoss = false;
		}
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdatePveResult("worldboss", item.TotalDameLiving, base.Game.IsWin);
		}
	}
}
