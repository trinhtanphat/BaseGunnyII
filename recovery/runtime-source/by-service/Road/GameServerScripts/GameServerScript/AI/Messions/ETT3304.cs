using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class ETT3304 : AMissionControl
{
	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

	private SimpleBoss m_king = null;

	private SimpleBoss king = null;

	private SimpleBoss m_secondKing = null;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private int IsSay = 0;

	private int m_kill = 0;

	private int m_state = 3316;

	private int turn = 0;

	private int firstBossID = 3316;

	private int secondBossID = 3317;

	private int npcID = 3303;

	private int npcID3 = 3318;

	private int npcID2 = 3112;

	private int npcID1 = 3313;

	private int direction;

	private static string[] ShootedChat = new string[4] { "Ta dận rồi nha!!", "Yếu, quá yếu...", "Ta né, ta né, hãy...", "Hỡi thần thánh, trợ giúp cho ta..." };

	private static string[] ShootedChatSecond = new string[4] { "Thân thể yếu ớt này mà các nguwoi không làm gì được!", "Yếu, quá yếu...", "Chọc dận ta à?...", "Dựa vào các nguwoi mà muốn cản nghi lễ của ta..." };

	private static string[] KillChat = new string[1] { "Ngươi muốn làm gì đây?<br/>A...." };

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
		base.Game.AddLoadingFile(1, "bombs/55.swf", "tank.resource.bombs.Bomb55");
		base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
		base.Game.AddLoadingFile(1, "bombs/53.swf", "tank.resource.bombs.Bomb53");
		base.Game.AddLoadingFile(2, "image/game/effect/3/flame.swf", "asset.game.4.flame");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanLeaderAsset");
		int[] npcIds = new int[6] { firstBossID, secondBossID, npcID, npcID2, npcID1, npcID3 };
		base.Game.LoadResources(npcIds);
		int[] npcIds2 = new int[1] { firstBossID };
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1126);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_kingFront = base.Game.Createlayer(700, 355, "font", "game.asset.living.ClanLeaderAsset", "out", 1, 1);
		m_king = base.Game.CreateBoss(m_state, 800, 400, -1, 1, "");
		m_king.FallFrom(800, 0, "fall", 0, 2, 1000, null);
		m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
		m_king.AddDelay(10);
		m_king.Say("Đến đây thôi, dám ngăn cản nghi lễ của ta, không muốn sống à!", 0, 4000);
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
		base.CanGameOver();
		if (!m_king.IsLiving && m_state == firstBossID)
		{
			m_state++;
		}
		if (m_state == secondBossID && m_secondKing == null)
		{
			king = base.Game.CreateBoss(npcID3, 478, 560, -1, 0, "");
			king.FallFrom(king.X, king.Y, "fall", 0, 0, 5000, null);
			m_secondKing = base.Game.CreateBoss(m_state, m_king.X, m_king.Y, m_king.Direction, 2, "");
			base.Game.RemoveLiving(m_king.Id);
			if (m_secondKing.Direction == 1)
			{
				m_secondKing.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
			}
			m_secondKing.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
			m_secondKing.Say(LanguageMgr.GetTranslation("Thể xác ốm yếu này, đưa ta mượn tạm xem!"), 0, 3000);
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
			m_secondKing.AddDelay(num - 1000);
			turn = base.Game.TurnIndex;
		}
		if (m_secondKing != null && !m_secondKing.IsLiving)
		{
			direction = m_secondKing.Direction;
			m_kill++;
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
		if (m_state == secondBossID && !m_secondKing.IsLiving)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
		List<LoadingFileInfo> list = new List<LoadingFileInfo>();
		list.Add(new LoadingFileInfo(2, "image/map/show7.jpg", ""));
		base.Game.SendLoadResource(list);
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

	public override void DoOther()
	{
		base.DoOther();
	}

	public override void OnShooted()
	{
	}
}
