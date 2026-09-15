using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace GameServerScript.AI.Messions
{

public class CSM1074 : AMissionControl
{
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
		base.Game.SetMap(1074);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.TotalTurn = base.Game.PlayerCount * 3;
		base.Game.SendMissionInfo();
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < 24; i++)
		{
			List<ItemInfo> info = null;
			DropInventory.SpecialDrop(1074, 2, ref info);
			if (info == null)
			{
				continue;
			}
			foreach (ItemInfo item in info)
			{
				list.Add(item);
			}
		}
		base.Game.CreateBox(550, 68, "2", list[0]);
		base.Game.CreateBox(750, 68, "2", list[1]);
		base.Game.CreateBox(932, 68, "2", list[2]);
		base.Game.CreateBox(1104, 68, "2", list[3]);
		base.Game.CreateBox(451, 184, "1", list[4]);
		base.Game.CreateBox(451, 285, "1", list[5]);
		base.Game.CreateBox(451, 394, "1", list[6]);
		base.Game.CreateBox(451, 499, "1", list[7]);
		base.Game.CreateBox(643, 184, "1", list[8]);
		base.Game.CreateBox(643, 285, "1", list[9]);
		base.Game.CreateBox(643, 394, "1", list[10]);
		base.Game.CreateBox(643, 499, "1", list[11]);
		base.Game.CreateBox(830, 184, "1", list[12]);
		base.Game.CreateBox(830, 285, "1", list[13]);
		base.Game.CreateBox(830, 394, "1", list[14]);
		base.Game.CreateBox(830, 499, "1", list[15]);
		base.Game.CreateBox(1022, 184, "1", list[16]);
		base.Game.CreateBox(1022, 285, "1", list[17]);
		base.Game.CreateBox(1022, 394, "1", list[18]);
		base.Game.CreateBox(1022, 499, "1", list[19]);
		base.Game.CreateBox(1201, 184, "1", list[20]);
		base.Game.CreateBox(1201, 285, "1", list[21]);
		base.Game.CreateBox(1201, 394, "1", list[22]);
		base.Game.CreateBox(1201, 499, "1", list[23]);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		((Player)base.Game.CurrentLiving).Seal((Player)base.Game.CurrentLiving, 0, 0);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		((Player)base.Game.CurrentLiving).SetBall(3);
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		return base.Game.TurnIndex > base.Game.TotalTurn - 1;
	}

	public override int UpdateUIData()
	{
		return base.UpdateUIData();
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		base.Game.IsWin = true;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			((SealEffect)allFightPlayer.EffectList.GetOfType(eEffectType.SealEffect))?.Stop();
		}
		List<LoadingFileInfo> list = new List<LoadingFileInfo>();
		list.Add(new LoadingFileInfo(2, "image/map/show5.jpg", ""));
		base.Game.SendLoadResource(list);
	}
}
}
