using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class Labyrinth40035 : AMissionControl
{
	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	private int kill = 0;

	private int npcIDs = 40056;

	private int npcIDs2 = 40057;

	private int[] birthX = new int[10] { 1110, 1090, 1060, 1040, 1020, 1000, 980, 960, 940, 920 };

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
		int[] npcIds = new int[2] { npcIDs, npcIDs2 };
		int[] npcIds2 = new int[2] { npcIDs, npcIDs2 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1314);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		for (int i = 0; i < birthX.Length; i++)
		{
			if (birthX[i] > 1000)
			{
				someNpc.Add(base.Game.CreateNpc(npcIDs, birthX[i], 534, 1, -1));
			}
			else
			{
				someNpc.Add(base.Game.CreateNpc(npcIDs2, birthX[i], 534, 1, -1));
			}
		}
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
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		kill = 0;
		foreach (SimpleNpc item in someNpc)
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
			base.Game.CreateGate(isEnter: true);
		}
		return flag;
	}

	public override int UpdateUIData()
	{
		return base.Game.TotalKillCount;
	}

	public override void OnGameOverMovie()
	{
		base.OnGameOverMovie();
		if (base.Game.GetLivedLivings().Count == 0)
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
