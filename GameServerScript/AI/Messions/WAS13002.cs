using System.Drawing;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class WAS13002 : AMissionControl
{
	private SimpleBoss m_king = null;

	private SimpleBoss bossL = null;

	private SimpleBoss bossR = null;

	private int bossID = 13005;

	private int npcID = 13003;

	private int npcID2 = 13004;

	private int npcID3 = 3312;

	private int kill = 0;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private PhysicalObj front;

	private Point[] brithPoint = new Point[2]
	{
		new Point(709, 590),
		new Point(1504, 590)
	};

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
		int[] npcIds = new int[4] { bossID, npcID, npcID2, npcID3 };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/55.swf", "tank.resource.bombs.Bomb55");
		base.Game.AddLoadingFile(2, "image/game/effect/10/jitan.swf", "asset.game.ten.jitan");
		base.Game.AddLoadingFile(2, "image/game/effect/10/zhuzi.swf", "asset.game.ten.zhuzi");
		base.Game.AddLoadingFile(2, "image/game/effect/10/tuteng.swf", "asset.game.ten.pilao");
		base.Game.AddLoadingFile(2, "image/game/effect/10/tuteng.swf", "asset.game.ten.jiaodu");
		base.Game.AddLoadingFile(2, "image/game/effect/10/tuteng.swf", "asset.game.ten.baozha");
		base.Game.AddLoadingFile(2, "image/game/living/living035.swf", "game.living.Living035");
		base.Game.AddLoadingFile(2, "image/game/living/living126.swf", "game.living.Living126");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanLeaderAsset");
		base.Game.SetMap(1215);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "13002";
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(950, 750, "font", "game.asset.living.ClanLeaderAsset", "out", 1, 1);
		front = base.Game.CreatePhysicalObj(609, 1023, "font", "game.living.Living035", "", 1, 0);
		front = base.Game.CreatePhysicalObj(1604, 1023, "font", "game.living.Living035", "", 1, 0);
		m_king = base.Game.CreateBoss(bossID, 1100, 1000, -1, 1, "");
		m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
		m_king.Say(LanguageMgr.GetTranslation("Sự dận dữ của thần linh sẻ tiêu diệt các ngươi !"), 0, 200, 0);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6100, 0);
		m_moive.PlayMovie("out", 10000, 1000);
		m_front.PlayMovie("out", 9900, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (bossL != null)
		{
		}
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
