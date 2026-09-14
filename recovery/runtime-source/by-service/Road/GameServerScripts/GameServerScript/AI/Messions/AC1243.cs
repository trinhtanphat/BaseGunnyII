using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class AC1243 : AMissionControl
{
	private SimpleBoss boss = null;

	private int bossID = 1243;

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
		int[] npcIds = new int[1] { bossID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/5/guang.swf", "asset.game.4.guang");
		base.Game.AddLoadingFile(2, "image/game/effect/5/tang.swf", "asset.game.4.tang");
		base.Game.AddLoadingFile(2, "image/game/effect/5/ruodian.swf", "asset.game.4.ruodian");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.xieyanjulongAsset");
		base.Game.SetMap(1243);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		livingConfig.IsWorldBoss = true;
		boss = base.Game.CreateBoss(bossID, 1170, 370, -1, 0, "", livingConfig);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
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
		if (boss != null && !boss.IsLiving)
		{
			kill++;
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
		if (boss != null && !boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdatePveResult("worldboss", item.TotalDameLiving, base.Game.IsWin);
		}
	}
}
