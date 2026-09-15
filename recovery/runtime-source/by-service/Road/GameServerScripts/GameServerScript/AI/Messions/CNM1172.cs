using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class CNM1172 : AMissionControl
{
	private List<SimpleNpc> redNpc = new List<SimpleNpc>();

	private List<SimpleNpc> blueNpc = new List<SimpleNpc>();

	private int redCount = 0;

	private int blueCount = 0;

	private int redTotalCount = 0;

	private int blueTotalCount = 0;

	private int dieRedCount = 0;

	private int dieBlueCount = 0;

	private int redNpcID = 1101;

	private int blueNpcID = 1102;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 930)
		{
			return 3;
		}
		if (score > 850)
		{
			return 2;
		}
		if (score > 775)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[2] { redNpcID, blueNpcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1072);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "1172";
		for (int i = 0; i < 4; i++)
		{
			redTotalCount++;
			if (i < 1)
			{
				redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, 1, 1));
			}
			else if (i < 3)
			{
				redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, 1, 1));
			}
			else
			{
				redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, 1, 1));
			}
		}
		blueTotalCount++;
		blueNpc.Add(base.Game.CreateNpc(blueNpcID, 1467, 495, 1, 1));
	}

	public override void OnNewTurnStarted()
	{
		redCount = redTotalCount - dieRedCount;
		blueCount = blueTotalCount - dieBlueCount;
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.PveGameDelay = 0;
		}
		if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay || (blueCount == 1 && redCount == 4))
		{
			return;
		}
		if (redTotalCount < 4 && blueTotalCount < 1)
		{
			for (int i = 0; i < 4; i++)
			{
				redTotalCount++;
				if (i < 1)
				{
					redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, 1, 1));
				}
				else if (i < 3)
				{
					redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, 1, 1));
				}
				else
				{
					redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, 1, 1));
				}
			}
			blueTotalCount++;
			blueNpc.Add(base.Game.CreateNpc(blueNpcID, 1467, 495, 1, 1));
		}
		else
		{
			if (redCount >= 4)
			{
				return;
			}
			if (4 - redCount >= 1)
			{
				for (int i = 0; i < 4; i++)
				{
					if (redTotalCount < 12 && redCount != 4)
					{
						redTotalCount++;
						if (i < 1)
						{
							redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, 1, 1));
						}
						else if (i < 3)
						{
							redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, 1, 1));
						}
						else
						{
							redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, 1, 1));
						}
					}
				}
			}
			else if (4 - redCount > 0)
			{
				for (int i = 0; i < 4 - redCount; i++)
				{
					if (redTotalCount < 12 && redCount != 4)
					{
						redTotalCount++;
						if (i < 1)
						{
							redNpc.Add(base.Game.CreateNpc(redNpcID, 900 + (i + 1) * 100, 505, 1, 1));
						}
						else if (i < 3)
						{
							redNpc.Add(base.Game.CreateNpc(redNpcID, 920 + (i + 1) * 100, 505, 1, 1));
						}
						else
						{
							redNpc.Add(base.Game.CreateNpc(redNpcID, 1000 + (i + 1) * 100, 515, 1, 1));
						}
					}
				}
			}
			if (blueCount < 1 && blueTotalCount < 3)
			{
				blueTotalCount++;
				blueNpc.Add(base.Game.CreateNpc(blueNpcID, 1467, 495, 1, 1));
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		bool flag = true;
		dieRedCount = 0;
		dieBlueCount = 0;
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		foreach (SimpleNpc item in redNpc)
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
		foreach (SimpleNpc item2 in blueNpc)
		{
			if (item2.IsLiving)
			{
				flag = false;
			}
			else
			{
				dieBlueCount++;
			}
		}
		if (flag && redTotalCount + blueTotalCount == 15)
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
		base.UpdateUIData();
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
	}
}
