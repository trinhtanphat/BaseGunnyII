using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace GameServerScript.AI.Messions
{

public class CSM3001 : AMissionControl
{
	private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

	private SimpleBoss boss = null;

	private PhysicalObj Tip = null;

	private bool result = false;

	private int killCount = 0;

	private int preKillNum = 0;

	private bool canPlayMovie = false;

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
		if (score > 725)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(1, "bombs/51.swf", "tank.resource.bombs.Bomb51");
		base.Game.AddLoadingFile(1, "bombs/17.swf", "tank.resource.bombs.Bomb17");
		base.Game.AddLoadingFile(1, "bombs/18.swf", "tank.resource.bombs.Bomb18");
		base.Game.AddLoadingFile(1, "bombs/19.swf", "tank.resource.bombs.Bomb19");
		base.Game.AddLoadingFile(1, "bombs/67.swf", "tank.resource.bombs.Bomb67");
		int[] npcIds = new int[4] { 3001, 3003, 3004, 3005 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1089);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(3005, 2000, 1200, -1, 1, "");
		boss.SetRelateDemagemRect(-42, -200, 84, 194);
		turnCount = 1;
	}

	public override void OnNewTurnStarted()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		List<ItemInfo> list2 = new List<ItemInfo>();
		if (base.Game.TurnIndex <= 1 || base.Game.CurrentPlayer.Delay <= base.Game.PveGameDelay)
		{
			return;
		}
		for (int i = 0; i < 3; i++)
		{
			if (SomeNpc.Count >= 7)
			{
				break;
			}
			if (turnCount % 2 == 0)
			{
				SomeNpc.Add(base.Game.CreateNpc(3003, (i + 1) * 50, boss.Y - 50, 1, 1));
			}
			else
			{
				SomeNpc.Add(base.Game.CreateNpc(3003, (i + 1) * 50 + 500, boss.Y - 50, 1, 1));
			}
			turnCount++;
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (base.Game.TurnIndex > 99)
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
		if (!result && SomeNpc.Count == 15)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		preKillNum = base.Game.TotalKillCount;
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		if (result)
		{
			return;
		}
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			allFightPlayer.CanGetProp = true;
		}
		base.Game.IsWin = true;
	}
}
}
