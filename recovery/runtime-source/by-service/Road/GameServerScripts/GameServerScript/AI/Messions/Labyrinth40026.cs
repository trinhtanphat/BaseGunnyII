using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class Labyrinth40026 : AMissionControl
{
	private int InSet = 0;

	private int bossID = 40039;

	private int bossID2 = 40040;

	private int bossID3 = 40038;

	private int npcIDl = 40044;

	private int npcIDr = 40043;

	private int npcID2 = 40045;

	private int npcID3 = 40046;

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

	public void CreateBoss()
	{
		base.Game.ClearAllChild();
		InSet = 1;
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		boss2 = base.Game.CreateBoss(bossID2, 1316, 444, -1, 1, "", livingConfig);
		boss2.SetRelateDemagemRect(boss2.NpcInfo.X, boss2.NpcInfo.Y, boss2.NpcInfo.Width, boss2.NpcInfo.Height);
	}

	public void CreateKing()
	{
		base.Game.ClearAllChild();
		InSet = 2;
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		boss3 = base.Game.CreateBoss(bossID3, 1316, 444, -1, 1, "", livingConfig);
		boss3.SetRelateDemagemRect(boss3.NpcInfo.X, boss3.NpcInfo.Y, boss3.NpcInfo.Width, boss3.NpcInfo.Height);
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
		base.CanGameOver();
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (boss != null && !boss.IsLiving && InSet == 0)
		{
			CreateBoss();
		}
		if (boss2 != null && !boss2.IsLiving && InSet == 1)
		{
			CreateKing();
		}
		if (boss3 != null && !boss3.IsLiving && InSet == 2)
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
