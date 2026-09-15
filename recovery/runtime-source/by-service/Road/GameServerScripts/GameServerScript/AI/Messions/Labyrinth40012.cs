using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class Labyrinth40012 : AMissionControl
{
	private SimpleBoss boss1 = null;

	private SimpleBoss boss2 = null;

	private int kill = 0;

	private int bossID1 = 40015;

	private int bossID2 = 40016;

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
		int[] npcIds = new int[2] { bossID1, bossID2 };
		int[] npcIds2 = new int[2] { bossID1, bossID2 };
		base.Game.AddLoadingFile(2, "image/game/effect/4/feather.swf", "asset.game.4.feather");
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1236);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		boss1 = base.Game.CreateBoss(bossID2, 1200, 900, -1, 1, "");
		boss1.FallFromTo(boss1.X, boss1.Y, null, 0, 0, 2000, null);
		boss1.SetRelateDemagemRect(boss1.NpcInfo.X, boss1.NpcInfo.Y, boss1.NpcInfo.Width, boss1.NpcInfo.Height);
		boss2 = base.Game.CreateBoss(bossID1, 1389, 620, -1, 0, "", livingConfig);
		boss2.SetRelateDemagemRect(boss2.NpcInfo.X, boss2.NpcInfo.Y, boss2.NpcInfo.Width, boss2.NpcInfo.Height);
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
		if (boss1 != null && !boss1.IsLiving && boss2 != null && !boss2.IsLiving)
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
		if (boss1 != null && !boss1.IsLiving && boss2 != null && !boss2.IsLiving)
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
