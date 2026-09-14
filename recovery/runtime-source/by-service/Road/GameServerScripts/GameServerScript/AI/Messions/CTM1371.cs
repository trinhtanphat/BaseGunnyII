using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class CTM1371 : AMissionControl
{
	private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

	private int redTotalCount = 0;

	private int dieRedCount = 0;

	private int redNpcID = 1301;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 600)
		{
			return 3;
		}
		if (score > 520)
		{
			return 2;
		}
		if (score > 450)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[1] { redNpcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1072);
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
			redTotalCount++;
			if (i < 1)
			{
				SomeNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, 1, 1));
			}
			else if (i < 3)
			{
				SomeNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, 1, 1));
			}
			else
			{
				SomeNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, 1, 1));
			}
		}
		redTotalCount++;
		SomeNpc.Add(base.Game.CreateNpc(redNpcID, 1467, 495, 1, 1));
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay)
		{
			return;
		}
		for (int i = 0; i < 4; i++)
		{
			if (redTotalCount < 15)
			{
				redTotalCount++;
				if (i < 1)
				{
					SomeNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, 1, 1));
				}
				else if (i < 3)
				{
					SomeNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, 1, 1));
				}
				else
				{
					SomeNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, 1, 1));
				}
			}
		}
		if (redTotalCount < 15)
		{
			redTotalCount++;
			SomeNpc.Add(base.Game.CreateNpc(redNpcID, 1467, 495, 1, 1));
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		bool flag = true;
		base.CanGameOver();
		dieRedCount = 0;
		foreach (SimpleNpc item in SomeNpc)
		{
			if (item.IsLiving)
			{
				flag = false;
			}
			else
			{
				dieRedCount++;
			}
		}
		if (flag && dieRedCount == 15)
		{
			base.Game.IsWin = true;
			return true;
		}
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		return base.Game.TotalKillCount;
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
		List<LoadingFileInfo> list = new List<LoadingFileInfo>();
		list.Add(new LoadingFileInfo(2, "image/map/2", ""));
		base.Game.SendLoadResource(list);
	}
}
