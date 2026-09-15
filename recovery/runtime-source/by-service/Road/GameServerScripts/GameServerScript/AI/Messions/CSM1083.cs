using System.Collections.Generic;
using Bussiness.Managers;
using Game.Logic.AI;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace GameServerScript.AI.Messions;

public class CSM1083 : AMissionControl
{
	private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

	private PhysicalObj Tip = null;

	private bool result = false;

	private int killCount = 0;

	private int preKillNum = 0;

	private bool canPlayMovie = false;

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
		base.Game.AddLoadingFile(2, "image/map/1086/object/Asset.swf", "com.map.trainer.TankTrainerAssetII");
		int[] npcIds = new int[2] { 1, 2 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1086);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		for (int i = 0; i < 4; i++)
		{
			SomeNpc.Add(base.Game.CreateNpc(201, (i + 1) * 100, 500, 1, 1));
		}
		SomeNpc.Add(base.Game.CreateNpc(202, 500, 500, 1, 1));
		Tip = base.Game.CreateTip(390, 120, "firstFront", "com.map.trainer.TankTrainerAssetII", "Empty", 1, 0);
	}

	public override void OnNewTurnStarted()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		List<ItemInfo> list2 = new List<ItemInfo>();
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			foreach (SimpleNpc livedLiving in base.Game.GetLivedLivings())
			{
				if (livedLiving.Distance(allFightPlayer.X, allFightPlayer.Y) <= 100.0)
				{
					canPlayMovie = true;
				}
			}
		}
		if (base.Game.TurnIndex > 1 && base.Game.CurrentPlayer.Delay > base.Game.PveGameDelay)
		{
			for (int i = 0; i < 5; i++)
			{
				if (SomeNpc.Count >= 15)
				{
					break;
				}
				SomeNpc.Add(base.Game.CreateNpc(201, (i + 1) * 100, 500, 1, 1));
			}
		}
		if (base.Game.CurrentPlayer.Delay >= base.Game.PveGameDelay)
		{
			return;
		}
		if (Tip.CurrentAction == "Empty")
		{
			Tip.PlayMovie("tip1", 0, 3000);
		}
		if (preKillNum < base.Game.TotalKillCount && killCount < 2)
		{
			killCount++;
		}
		if (killCount == 2)
		{
			Tip.PlayMovie("tip2", 0, 2000);
		}
		if (canPlayMovie)
		{
			Tip.PlayMovie("tip3", 0, 2000);
		}
		list.Add(ItemMgr.FindItemTemplate(10001));
		list.Add(ItemMgr.FindItemTemplate(10003));
		list.Add(ItemMgr.FindItemTemplate(10018));
		foreach (ItemTemplateInfo item in list)
		{
			list2.Add(ItemInfo.CreateFromTemplate(item, 1, 101));
		}
		foreach (Player allFightPlayer2 in base.Game.GetAllFightPlayers())
		{
			allFightPlayer2.CanGetProp = false;
			allFightPlayer2.PlayerDetail.ClearFightBag();
			foreach (ItemInfo item2 in list2)
			{
				allFightPlayer2.PlayerDetail.AddTemplate(item2, eBageType.FightBag, item2.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
			}
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
