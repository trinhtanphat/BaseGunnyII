using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class DLN5101 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss m_boss = null;

	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	private int bossID2 = 5102;

	private int bossID = 5101;

	private int kill = 0;

	private PhysicalObj moive;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

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
		int[] npcIds = new int[2] { bossID, bossID2 };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/5/minigun.swf", "asset.game.4.minigun");
		base.Game.AddLoadingFile(2, "image/game/effect/5/jinqud.swf", "asset.game.4.jinqud");
		base.Game.AddLoadingFile(2, "image/game/effect/5/zap.swf", "asset.game.4.zap");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.gebulinzhihuiguanAsset");
		base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
		base.Game.AddLoadingFile(1, "bombs/72.swf", "tank.resource.bombs.Bomb72");
		base.Game.SetMap(1151);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(1131, 650, "font", "game.asset.living.gebulinzhihuiguanAsset", "out", 1, 1);
		moive = base.Game.Createlayer(1567, 810, "moive", "asset.game.4.jinqud", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID2, 190, 365, 1, 1, "");
		boss = base.Game.CreateBoss(bossID, 1477, 768, -1, 0, "");
		boss.SetRelateDemagemRect(-21, -79, 120, 80);
		m_boss.SetRelateDemagemRect(-42, -200, 84, 104);
		boss.PlayMovie("in", 0, 2000);
		m_boss.PlayMovie("in", 4000, 2000);
		m_boss.PlayMovie("in", 0, 2000);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6100, 0);
		m_moive.PlayMovie("out", 10000, 1000);
		m_front.PlayMovie("out", 9900, 0);
	}

	public override void OnNewTurnStarted()
	{
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > 1)
		{
			if (m_moive != null)
			{
				base.Game.RemovePhysicalObj(m_moive, sendToClient: true);
				m_moive = null;
			}
			if (m_front != null)
			{
				base.Game.RemovePhysicalObj(m_front, sendToClient: true);
				m_front = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		if (boss != null && !boss.IsLiving)
		{
			m_boss = base.Game.CreateBoss(bossID2, 185, 370, 1, 10, "cryA");
			m_boss.PlayMovie("in", 4000, 4000);
			boss.SetXY(1477, 758);
			boss.PlayMovie("fallingA", 100, 1000);
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
		if (!boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	public override void OnShooted()
	{
		if (boss.IsLiving)
		{
			if (boss.Y == 659)
			{
				boss.SetXY(1477, 659);
				boss.SetXY(1477, 759);
				boss.CallFuction(kill1, 100);
				boss.FallCount = 1;
			}
			else if (boss.Y == 559)
			{
				boss.SetXY(1477, 559);
				boss.SetXY(1477, 759);
				boss.CallFuction(kill2, 100);
				boss.FallCount = 2;
			}
			else if (boss.Y == 459)
			{
				boss.SetXY(1477, 459);
				boss.SetXY(1477, 759);
				boss.CallFuction(kill3, 100);
				boss.FallCount = 3;
			}
			else if (boss.Y == 359)
			{
				boss.SetXY(1477, 359);
				boss.SetXY(1477, 759);
				boss.CallFuction(kill4, 100);
				boss.FallCount = 4;
			}
			else if (boss.Y == 259)
			{
				boss.SetXY(1477, 259);
				boss.SetXY(1477, 759);
				boss.CallFuction(kill5, 100);
				boss.FallCount = 5;
			}
		}
	}

	private void kill1()
	{
		boss.SetXY(1477, 659);
	}

	private void kill2()
	{
		boss.SetXY(1477, 559);
	}

	private void kill3()
	{
		boss.SetXY(1477, 459);
	}

	private void kill4()
	{
		boss.SetXY(1477, 359);
	}

	private void kill5()
	{
		boss.SetXY(1477, 259);
	}
}
