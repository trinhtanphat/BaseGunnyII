using System.Collections.Generic;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class CNM1176 : AMissionControl
{
	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

	private SimpleBoss m_king = null;

	private SimpleBoss m_secondKing = null;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private int m_kill = 0;

	private int m_state = 1105;

	private int turn = 0;

	private int firstBossID = 1105;

	private int secondBossID = 1106;

	private int npcID = 1109;

	private static string[] KillChat = new string[3] { "马迪亚斯不要再控制我！", "这就是挑战我的下场！", "不！！这不是我的意愿… " };

	private static string[] ShootedChat = new string[2] { "哎呀~~你们为什么要攻击我？<br/>我在干什么？", "噢~~好痛!我为什么要战斗？<br/>我必须战斗…" };

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
		base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
		int[] npcIds = new int[3] { npcID, firstBossID, secondBossID };
		base.Game.LoadResources(npcIds);
		int[] npcIds2 = new int[2] { firstBossID, npcID };
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1076);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_kingFront = base.Game.Createlayer(610, 380, "font", "game.asset.living.boguoKingAsset", "out", 1, 0);
		m_king = base.Game.CreateBoss(m_state, 890, 590, -1, 0, "");
		m_king.FallFrom(m_king.X, m_king.Y, "fall", 0, 2, 1000);
		m_king.SetRelateDemagemRect(-21, -79, 72, 51);
		m_king.AddDelay(10);
		m_king.Say("你们这些低等的庶民，竟敢来到我的王国放肆！", 0, 3000);
		m_kingMoive.PlayMovie("in", 9000, 0);
		m_kingFront.PlayMovie("in", 9000, 0);
		m_kingMoive.PlayMovie("out", 13000, 0);
		m_kingFront.PlayMovie("out", 13400, 0);
		turn = base.Game.TurnIndex;
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
		base.CanGameOver();
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!m_king.IsLiving && m_state == firstBossID)
		{
			m_state++;
		}
		if (m_state == secondBossID && m_secondKing == null)
		{
			m_secondKing = base.Game.CreateBoss(m_state, m_king.X, m_king.Y, m_king.Direction, 1, "");
			base.Game.RemoveLiving(m_king.Id);
			if (m_secondKing.Direction == -1)
			{
				m_secondKing.SetRectBomb(24, -159, 66, 38);
				m_secondKing.SetRelateDemagemRect(-21, -79, 72, 51);
			}
			else
			{
				m_secondKing.SetRectBomb(-90, -159, 66, 38);
				m_secondKing.SetRelateDemagemRect(-21, -79, 72, 51);
			}
			m_secondKing.Say(LanguageMgr.GetTranslation("GameServerScript.AI.Messions.CHM1376.msg3"), 0, 3000);
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			int delay = base.Game.FindRandomPlayer().Delay;
			foreach (Player item in allFightPlayers)
			{
				if (item.Delay < delay)
				{
					delay = item.Delay;
				}
			}
			m_secondKing.AddDelay(delay - 2000);
			turn = base.Game.TurnIndex;
		}
		if (m_secondKing != null && !m_secondKing.IsLiving)
		{
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
	}

	public override void DoOther()
	{
		base.DoOther();
		if (m_king != null && m_king != null)
		{
			if (m_king.IsLiving)
			{
				int num = base.Game.Random.Next(0, KillChat.Length);
				m_king.Say(KillChat[num], 0, 0);
			}
			else
			{
				int num = base.Game.Random.Next(0, KillChat.Length);
				m_king.Say(KillChat[num], 0, 0);
			}
		}
	}
}
