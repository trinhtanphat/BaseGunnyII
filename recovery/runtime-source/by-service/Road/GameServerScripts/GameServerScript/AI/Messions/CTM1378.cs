using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class CTM1378 : AMissionControl
{
	private SimpleBoss m_king = null;

	private int m_kill = 0;

	private int bossID = 1308;

	private int npcID = 1311;

	private PhysicalObj m_kingMoive;

	private PhysicalObj m_front;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1330)
		{
			return 3;
		}
		if (score > 1150)
		{
			return 2;
		}
		if (score > 970)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[2] { npcID, bossID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ZhenBombKingAsset");
		base.Game.SetMap(1084);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_king = base.Game.CreateBoss(bossID, 888, 590, -1, 0, "");
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(710, 380, "font", "game.asset.living.ZhenBombKingAsset", "out", 1, 1);
		m_king.FallFrom(888, 590, "fall", 0, 2, 1000);
		m_king.SetRelateDemagemRect(-41, -187, 83, 140);
		m_kingMoive.PlayMovie("in", 1000, 0);
		m_front.PlayMovie("in", 2000, 2000);
		m_king.AddDelay(16);
		base.Game.BossCardCount = 1;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (m_king.State == 0)
		{
			m_king.SetRelateDemagemRect(-41, -187, 83, 140);
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (m_kingMoive != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
			m_kingMoive = null;
		}
		if (m_front != null)
		{
			base.Game.RemovePhysicalObj(m_front, sendToClient: true);
			m_front = null;
		}
	}

	public override bool CanGameOver()
	{
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!m_king.IsLiving)
		{
			m_kill++;
			m_king.PlayMovie("die", 0, 200);
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return m_kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		bool flag = true;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving)
			{
				flag = false;
			}
		}
		if (!m_king.IsLiving && !flag)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}
