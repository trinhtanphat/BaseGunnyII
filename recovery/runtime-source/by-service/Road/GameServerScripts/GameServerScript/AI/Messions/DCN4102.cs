using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class DCN4102 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss m_king = null;

	private int bossID = 4105;

	private int bossID2 = 4106;

	private int kill = 0;

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
		int[] npcIds2 = new int[2] { bossID, bossID2 };
		base.Game.AddLoadingFile(2, "image/game/effect/4/feather.swf", "asset.game.4.feather");
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1143);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(bossID2, 1380, 900, -1, 1, "");
		boss.FallFromTo(boss.X, boss.Y, null, 0, 0, 2000, null);
		boss.SetRelateDemagemRect(-41, -100, 83, 70);
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		m_king = base.Game.CreateBoss(bossID, 189, 520, -1, 0, "", livingConfig);
		m_king.SetRelateDemagemRect(-41, -100, 50, 70);
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
		if (m_king != null && !m_king.IsLiving)
		{
			kill++;
			return true;
		}
		if (boss == null || boss.IsLiving)
		{
			return false;
		}
		kill++;
		return true;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (m_king != null && !m_king.IsLiving)
		{
			boss.PlayMovie("die", 1000, 1000);
			base.Game.IsWin = true;
		}
		if (boss != null && !boss.IsLiving)
		{
			m_king.PlayMovie("die", 1000, 1000);
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	public override void OnShooted()
	{
		base.OnShooted();
	}
}
