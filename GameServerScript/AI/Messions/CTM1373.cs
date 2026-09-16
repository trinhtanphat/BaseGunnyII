using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class CTM1373 : AMissionControl
{
	private SimpleBoss m_boss;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int bossID = 1303;

	private int npcID = 1309;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1540)
		{
			return 3;
		}
		if (score > 1410)
		{
			return 2;
		}
		if (score > 1285)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
		int[] npcIds = new int[2] { bossID, npcID };
		base.Game.LoadResources(npcIds);
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1073);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "1373";
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(680, 330, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 1);
		m_boss = base.Game.CreateBoss(bossID, 770, -1500, -1, 1, "");
		m_boss.FallFrom(770, 301, "fall", 0, 2, 1000);
		m_boss.SetRelateDemagemRect(34, -35, 11, 18);
		m_boss.AddDelay(10);
		m_boss.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.CHM1373.msg2"), 0, 6000);
		m_boss.PlayMovie("call", 5900, 0);
		m_moive.PlayMovie("in", 9000, 0);
		m_boss.PlayMovie("weakness", 10000, 5000);
		m_front.PlayMovie("in", 9000, 0);
		m_moive.PlayMovie("out", 15000, 0);
		base.Game.BossCardCount = 1;
		base.OnStartGame();
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
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
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		base.CanGameOver();
		if (!m_boss.IsLiving)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		if (m_boss == null)
		{
			return 0;
		}
		if (!m_boss.IsLiving)
		{
			return 1;
		}
		return base.UpdateUIData();
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (!m_boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}
}
}
