using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class Labyrinth40030 : AMissionControl
{
	private int bossID = 40041;

	private int bossID2 = 40042;

	private int bossID3 = 40038;

	private int npcIDl = 40048;

	private int npcIDr = 40047;

	private int npcID2 = 40049;

	private int npcID3 = 40050;

	private SimpleBoss boss;

	private SimpleBoss boss2;

	private SimpleBoss boss3;

	private int kill = 0;

	private int m_state = 40041;

	private int turn = 0;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1870)
		{
			return 3;
		}
		if (score > 1825)
		{
			return 2;
		}
		if (score > 1780)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[7] { bossID, bossID2, bossID3, npcIDl, npcIDr, npcID2, npcID3 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut51.swf", "bullet51");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet51.swf", "bullet51");
		base.Game.SetMap(1280);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		boss = base.Game.CreateBoss(bossID, 1316, 444, -1, 1, "", livingConfig);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
	}

	public override void OnNewTurnStarted()
	{
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!boss.IsLiving && m_state == bossID)
		{
			m_state = bossID2;
		}
		if (m_state == bossID2 && boss2 == null)
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			boss2 = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 2, "", livingConfig);
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
			turn = base.Game.TurnIndex;
		}
		if (m_state == bossID2 && !boss2.IsLiving && boss3 == null)
		{
			LivingConfig livingConfig = base.Game.BaseLivingConfig();
			livingConfig.IsFly = true;
			boss3 = base.Game.CreateBoss(bossID3, boss.X, boss.Y, boss.Direction, 2, "", livingConfig);
			base.Game.RemoveLiving(boss2.Id);
			if (boss3.Direction == 1)
			{
				boss3.SetRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			}
			boss3.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			Player player = base.Game.FindRandomPlayer();
			int num = 0;
			if (player != null)
			{
				num = player.Delay;
			}
			foreach (Player item2 in allFightPlayers)
			{
				if (item2.Delay < num)
				{
					num = item2.Delay;
				}
			}
			boss3.AddDelay(num - 2000);
			turn = base.Game.TurnIndex;
		}
		if (boss3 != null && !boss3.IsLiving)
		{
			if (base.Game.CanEnterGate)
			{
				return true;
			}
			kill++;
			base.Game.CanShowBigBox = true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return kill;
	}

	public override void OnGameOverMovie()
	{
		base.OnGameOverMovie();
		if (boss3 != null && !boss3.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
	}
}
