using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class TVS12001 : AMissionControl
{
	private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

	private SimpleNpc npc;

	private bool result = false;

	private int preKillNum = 0;

	public int turnCount;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 900)
		{
			return 3;
		}
		if (score > 825)
		{
			return 2;
		}
		return (score > 725) ? 1 : 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[4] { 12001, 12002, 12003, 12004 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1207);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		for (int i = 0; i < 4; i++)
		{
			if (i < 1)
			{
				SomeNpc.Add(base.Game.CreateNpc(12001, 1360, 700, -1, 1));
			}
			else if (i < 3)
			{
				SomeNpc.Add(base.Game.CreateNpc(12001, 1410, 700, -1, 1));
			}
			else
			{
				SomeNpc.Add(base.Game.CreateNpc(12002, 1250, 700, -1, 1));
			}
		}
		npc = base.Game.CreateNpc(12004, 700, 700, -1, 0);
		npc.FallFrom(npc.X, npc.Y, "", 0, 0, 1200, null);
		npc.SetRelateDemagemRect(-42, -200, 84, 194);
		turnCount = 0;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		if (base.Game.TurnIndex > 1 && base.Game.CurrentPlayer.Delay > base.Game.PveGameDelay)
		{
			for (int i = 0; i < 4; i++)
			{
				if (turnCount < 12)
				{
					turnCount++;
					if (i < 1)
					{
						SomeNpc.Add(base.Game.CreateNpc(12001, 1260, 700, -1, 1));
					}
					else if (i < 3)
					{
						SomeNpc.Add(base.Game.CreateNpc(12001, 1350, 700, -1, 1));
					}
					else
					{
						SomeNpc.Add(base.Game.CreateNpc(12002, 1400, 700, -1, 1));
					}
				}
			}
		}
		if (base.Game.TurnIndex == 2)
		{
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (base.Game.TurnIndex > 199)
		{
			return true;
		}
		result = false;
		foreach (SimpleNpc item in SomeNpc)
		{
			if (item.IsLiving)
			{
				result = true;
			}
		}
		return (!result && SomeNpc.Count == 16) || !npc.IsLiving;
	}

	public override int UpdateUIData()
	{
		preKillNum = base.Game.TotalKillCount;
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		if (!result && SomeNpc.Count == 16)
		{
			foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
			{
				allFightPlayer.CanGetProp = true;
			}
			base.Game.IsWin = true;
		}
		if (!npc.IsLiving)
		{
			base.Game.IsWin = false;
		}
	}
}
}
