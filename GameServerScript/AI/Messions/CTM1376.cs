using System.Collections.Generic;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class CTM1376 : AMissionControl
{
	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

	private SimpleBoss m_king = null;

	private SimpleBoss m_secondKing = null;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private int m_kill = 0;

	private int m_state = 1305;

	private int turn = 0;

	private int firstBossID = 1305;

	private int secondBossID = 1306;

	private int npcID = 1310;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1150)
		{
			return 3;
		}
		if (score > 925)
		{
			return 2;
		}
		if (score > 700)
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
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
		int[] npcIds = new int[3] { firstBossID, secondBossID, npcID };
		base.Game.LoadResources(npcIds);
		int[] npcIds2 = new int[1] { firstBossID };
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1076);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "1376";
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_kingFront = base.Game.Createlayer(720, 495, "font", "game.asset.living.boguoKingAsset", "out", 1, 1);
		m_king = base.Game.CreateBoss(m_state, 890, 590, -1, 0, "");
		m_king.FallFrom(890, 590, "fall", 0, 2, 1000);
		m_king.SetRelateDemagemRect(-21, -87, 72, 59);
		m_king.AddDelay(10);
		m_king.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.CHM1376.msg2"), 0, 3000);
		m_kingMoive.PlayMovie("in", 9000, 0);
		m_kingFront.PlayMovie("in", 9000, 0);
		m_kingMoive.PlayMovie("out", 13000, 0);
		m_kingFront.PlayMovie("out", 13400, 0);
		turn = base.Game.TurnIndex;
		base.Game.BossCardCount = 1;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > turn + 1)
		{
			if (m_kingMoive != null)
			{
				base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
				m_kingMoive = null;
			}
			if (m_kingFront != null)
			{
				base.Game.RemovePhysicalObj(m_kingFront, sendToClient: true);
				m_kingFront = null;
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
		if (!m_king.IsLiving && m_state == firstBossID)
		{
			m_state++;
		}
		if (m_state == secondBossID && m_secondKing == null)
		{
			m_secondKing = base.Game.CreateBoss(m_state, m_king.X, m_king.Y, m_king.Direction, 2, "");
			base.Game.RemoveLiving(m_king.Id);
			if (m_secondKing.Direction == 1)
			{
				m_secondKing.SetRelateDemagemRect(-21, -79, 72, 51);
			}
			m_secondKing.SetRelateDemagemRect(-21, -79, 72, 51);
			m_secondKing.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.CHM1376.msg3"), 0, 3000);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			Player player = base.Game.FindRandomPlayer();
			int num = 0;
			if (player != null)
			{
				num = player.Delay;
			}
			foreach (Player item in allFightPlayers)
			{
				if (item.Delay < num)
				{
					num = item.Delay;
				}
			}
			m_secondKing.AddDelay(num - 2000);
			turn = base.Game.TurnIndex;
		}
		if (m_secondKing != null && !m_secondKing.IsLiving)
		{
			base.Game.RemoveLiving(m_secondKing.Id);
			PhysicalObj physicalObj = base.Game.CreatePhysicalObj(m_secondKing.X, m_secondKing.Y, "king", "game.living.Living005", "specialDie", 1, 0);
			if (physicalObj.CurrentAction == "specialDie")
			{
				m_kill++;
				return true;
			}
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
		if (m_state == secondBossID && !m_secondKing.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
		m_leftWall = base.Game.FindPhysicalObjByName("wallLeft");
		m_rightWall = base.Game.FindPhysicalObjByName("wallRight");
		for (int i = 0; i < m_leftWall.Length; i++)
		{
			base.Game.RemovePhysicalObj(m_leftWall[i], sendToClient: true);
		}
		for (int i = 0; i < m_rightWall.Length; i++)
		{
			base.Game.RemovePhysicalObj(m_rightWall[i], sendToClient: true);
		}
	}
}
}
