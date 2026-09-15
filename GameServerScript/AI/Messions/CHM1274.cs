using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace GameServerScript.AI.Messions
{

public class CHM1274 : AMissionControl
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
		for (int i = 0; i < 54; i++)
		{
			List<ItemInfo> info = null;
			if (i > 9)
			{
				DropInventory.SpecialDrop(base.Game.MissionInfo.Id, 1, ref info);
			}
			else
			{
				DropInventory.SpecialDrop(base.Game.MissionInfo.Id, 2, ref info);
			}
			if (info != null)
			{
				foreach (ItemInfo item in info)
				{
					list.Add(item);
				}
			}
			else
			{
				list.Add(null);
			}
		}
		base.Game.CreateBox(455, 88, "2", list[0]);
		base.Game.CreateBox(555, 88, "2", list[1]);
		base.Game.CreateBox(655, 88, "2", list[2]);
		base.Game.CreateBox(755, 88, "2", list[3]);
		base.Game.CreateBox(855, 88, "2", list[4]);
		base.Game.CreateBox(955, 88, "2", list[5]);
		base.Game.CreateBox(1055, 88, "2", list[6]);
		base.Game.CreateBox(1155, 88, "2", list[7]);
		base.Game.CreateBox(1255, 88, "2", list[8]);
		base.Game.CreateBox(450, 184, "1", list[9]);
		base.Game.CreateBox(450, 259, "1", list[10]);
		base.Game.CreateBox(450, 335, "1", list[11]);
		base.Game.CreateBox(450, 420, "1", list[12]);
		base.Game.CreateBox(450, 504, "1", list[13]);
		base.Game.CreateBox(550, 184, "1", list[14]);
		base.Game.CreateBox(550, 259, "1", list[15]);
		base.Game.CreateBox(550, 335, "1", list[16]);
		base.Game.CreateBox(550, 420, "1", list[17]);
		base.Game.CreateBox(550, 504, "1", list[18]);
		base.Game.CreateBox(650, 184, "1", list[19]);
		base.Game.CreateBox(650, 259, "1", list[20]);
		base.Game.CreateBox(650, 335, "1", list[21]);
		base.Game.CreateBox(650, 420, "1", list[22]);
		base.Game.CreateBox(650, 504, "1", list[23]);
		base.Game.CreateBox(750, 184, "1", list[24]);
		base.Game.CreateBox(750, 259, "1", list[25]);
		base.Game.CreateBox(750, 335, "1", list[26]);
		base.Game.CreateBox(750, 420, "1", list[27]);
		base.Game.CreateBox(750, 504, "1", list[28]);
		base.Game.CreateBox(850, 184, "1", list[29]);
		base.Game.CreateBox(850, 259, "1", list[30]);
		base.Game.CreateBox(850, 335, "1", list[31]);
		base.Game.CreateBox(850, 420, "1", list[32]);
		base.Game.CreateBox(850, 504, "1", list[33]);
		base.Game.CreateBox(950, 184, "1", list[34]);
		base.Game.CreateBox(950, 259, "1", list[35]);
		base.Game.CreateBox(950, 335, "1", list[36]);
		base.Game.CreateBox(950, 420, "1", list[37]);
		base.Game.CreateBox(950, 504, "1", list[38]);
		base.Game.CreateBox(1050, 184, "1", list[39]);
		base.Game.CreateBox(1050, 259, "1", list[40]);
		base.Game.CreateBox(1050, 335, "1", list[41]);
		base.Game.CreateBox(1050, 420, "1", list[42]);
		base.Game.CreateBox(1050, 504, "1", list[43]);
		base.Game.CreateBox(1150, 184, "1", list[44]);
		base.Game.CreateBox(1150, 259, "1", list[45]);
		base.Game.CreateBox(1150, 335, "1", list[46]);
		base.Game.CreateBox(1150, 420, "1", list[47]);
		base.Game.CreateBox(1150, 504, "1", list[48]);
		base.Game.CreateBox(1250, 189, "1", list[49]);
		base.Game.CreateBox(1250, 259, "1", list[50]);
		base.Game.CreateBox(1250, 335, "1", list[51]);
		base.Game.CreateBox(1250, 420, "1", list[52]);
		base.Game.CreateBox(1250, 504, "1", list[53]);
		base.Game.BossCardCount = 1;
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
