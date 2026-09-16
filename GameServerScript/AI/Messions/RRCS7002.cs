using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class RRCS7002 : AMissionControl
{
	private List<SimpleBoss> someBoss = new List<SimpleBoss>();

	private int bossID = 7011;

	private int bossID2 = 7011;

	private int bossID3 = 7011;

	private int kill = 0;

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
		int[] npcIds = new int[3] { bossID, bossID2, bossID3 };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
		base.Game.AddLoadingFile(2, "image/game/effect/7/cao.swf", "asset.game.seven.cao");
		base.Game.SetMap(1162);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "7002";
		someBoss.Add(base.Game.CreateBoss(bossID, 1565, 787, -1, 1, "standA"));
		someBoss.Add(base.Game.CreateBoss(bossID, 1583, 495, -1, 1, "standA"));
		someBoss.Add(base.Game.CreateBoss(bossID, 1643, 236, -1, 1, "standA"));
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		bool flag = true;
		base.CanGameOver();
		kill = 0;
		foreach (SimpleBoss item in someBoss)
		{
			if (item.IsLiving)
			{
				flag = false;
			}
			else
			{
				kill++;
			}
		}
		if (flag && kill == base.Game.MissionInfo.TotalCount)
		{
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
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}
}
