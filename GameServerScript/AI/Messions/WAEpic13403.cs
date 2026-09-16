using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class WAEpic13403 : AMissionControl
{
	private SimpleBoss m_kingf = null;

	private SimpleBoss m_king = null;

	private int bossID = 13406;

	private int bossID2 = 13407;

	private int npcID = 5322;

	private int npcID2 = 5323;

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
		int[] npcIds = new int[4] { bossID2, bossID, npcID, npcID2 };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/5/heip.swf", "asset.game.4.heip");
		base.Game.AddLoadingFile(2, "image/game/effect/10/chengtuo.swf", "asset.game.ten.chengtuo");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.dadangAsset");
		base.Game.SetMap(1216);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "13003";
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(950, 650, "font", "game.asset.living.dadangAsset", "out", 1, 1);
		m_king = base.Game.CreateBoss(bossID2, 1390, 1000, -1, 1, "");
		m_king.CallFuction(CreateDevil, 3500);
		m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6100, 0);
		m_moive.PlayMovie("out", 10000, 1000);
		m_front.PlayMovie("out", 9900, 0);
	}

	private void CreateDevil()
	{
		m_kingf = base.Game.CreateBoss(bossID, 1445, 650, -1, 0, "");
		m_kingf.SetRelateDemagemRect(m_kingf.NpcInfo.X, m_kingf.NpcInfo.Y, m_kingf.NpcInfo.Width, m_kingf.NpcInfo.Height);
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
		if (m_king != null && !m_king.IsLiving)
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
