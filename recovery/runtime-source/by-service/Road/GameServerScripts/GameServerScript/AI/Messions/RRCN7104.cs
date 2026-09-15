using System.Collections.Generic;
using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class RRCN7104 : AMissionControl
{
	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private PhysicalObj m_eggs = null;

	private PhysicalObj m_out = null;

	private SimpleBoss cage = null;

	private SimpleBoss boss = null;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	private int m_kill = 0;

	private int turn = 0;

	private int bossID1 = 7131;

	private int bossID2 = 7132;

	private int npcID2 = 7133;

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
		base.Game.AddLoadingFile(1, "bombs/83.swf", "tank.resource.bombs.Bomb83");
		base.Game.AddLoadingFile(1, "bombs/84.swf", "tank.resource.bombs.Bomb84");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.choudanbenbenAsset");
		base.Game.AddLoadingFile(2, "image/game/living/living177.swf", "game.living.Living177");
		base.Game.AddLoadingFile(2, "image/game/effect/7/choud.swf", "asset.game.seven.choud");
		base.Game.AddLoadingFile(2, "image/game/effect/7/jinqucd.swf", "asset.game.seven.jinqucd");
		base.Game.AddLoadingFile(2, "image/game/effect/7/du.swf", "asset.game.seven.du");
		int[] npcIds = new int[3] { bossID1, npcID2, bossID2 };
		base.Game.LoadResources(npcIds);
		int[] npcIds2 = new int[1] { bossID1 };
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.SetMap(1164);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(300, 595, "font", "game.asset.living.choudanbenbenAsset", "out", 1, 1);
		m_eggs = base.Game.CreatePhysicalObj(2070, 633, "eggs", "game.living.Living178", "in", 1, 0);
		cage = base.Game.CreateBoss(bossID2, 1920, 920, -1, 0, "stand");
		cage.SetRelateDemagemRect(cage.NpcInfo.X, cage.NpcInfo.Y, cage.NpcInfo.Width, cage.NpcInfo.Height);
		boss = base.Game.CreateBoss(bossID1, 181, 875, 1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		boss.Say(LanguageMgr.GetTranslation("Định giải cứu gà con à ? Đừng có mơ ...."), 0, 4000);
		m_moive.PlayMovie("in", 9000, 0);
		m_front.PlayMovie("in", 9000, 0);
		m_moive.PlayMovie("out", 13000, 0);
		m_front.PlayMovie("out", 13400, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (base.Game.TurnIndex > 1)
		{
			cage.AddDelay(-200);
		}
		int num = 0;
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.Delay < num)
			{
				num = item.Delay;
			}
		}
		cage.AddDelay(num + 200);
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
		if (base.Game.TurnIndex == 1)
		{
			cage.PlayMovie("standB", 1000, 0);
			cage.Say(LanguageMgr.GetTranslation("Chúng mình không muốn bị lây bệnh, cứu cứu...."), 0, 1000);
		}
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (boss != null && !boss.IsLiving)
		{
			cage.PlayMovie("out", 1000, 0);
			cage.SetRelateDemagemRect(-144, cage.NpcInfo.Y, cage.NpcInfo.Width, cage.NpcInfo.Height);
			int num = 0;
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			foreach (Player item in allFightPlayers)
			{
				if (item.Delay < num)
				{
					num = item.Delay;
				}
			}
			cage.AddDelay(num - 200);
		}
		if (cage != null && !cage.IsLiving)
		{
			List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
			base.Game.RemoveLiving(cage.Id);
			m_out = base.Game.CreatePhysicalObj(1920, 947, "movie", "game.living.Living177", "die", 1, 0);
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
		if (cage != null && !cage.IsLiving)
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
