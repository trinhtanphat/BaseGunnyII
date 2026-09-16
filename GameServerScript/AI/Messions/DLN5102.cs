using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class DLN5102 : AMissionControl
{
	private SimpleBoss m_boss;

	private int m_kill = 0;

	private SimpleBoss boss;

	private SimpleBoss king;

	private PhysicalObj m_moive;

	private PhysicalObj m_moive1;

	private PhysicalObj m_moive2;

	private PhysicalObj m_moive3;

	private PhysicalObj m_front;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private PhysicalObj m_wallRight = null;

	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	private PhysicalObj m_NPC;

	private PhysicalObj n_NPC;

	private PhysicalObj npc = null;

	private PhysicalObj npc2 = null;

	private int IsEixt = 0;

	private int IsEixt2 = 0;

	private int bossID = 5114;

	private int bossID2 = 5113;

	private int bossID3 = 5112;

	private int npcID = 5116;

	private int npcID2 = 5117;

	private int npcID3 = 5111;

	private static string[] KillChat = new string[2] { "送你回老家！", "就凭你还妄想能够打败我？" };

	private static string[] ShootedChat = new string[2] { "哎呦！很痛…", "我还顶的住…" };

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
		base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
		base.Game.AddLoadingFile(2, "image/game/effect/5/mubiao.swf", "asset.game.4.mubiao");
		base.Game.AddLoadingFile(2, "image/game/effect/5/xiaopao.swf", "asset.game.4.xiaopao");
		base.Game.AddLoadingFile(2, "image/game/effect/5/zao.swf", "asset.game.4.zao");
		base.Game.AddLoadingFile(2, "image/game/living/living144.swf", "game.living.Living144");
		base.Game.AddLoadingFile(2, "image/game/living/living152.swf", "game.living.Living152");
		base.Game.AddLoadingFile(2, "image/game/living/living154.swf", "game.living.Living154");
		base.Game.AddLoadingFile(2, "image/game/living/living147.swf", "game.living.Living147");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.gebulinzhihuiguanAsset");
		int[] npcIds = new int[6] { bossID, bossID2, bossID3, npcID, npcID2, npcID3 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1152);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(1100, 395, "font", "game.asset.living.gebulinzhihuiguanAsset", "out", 1, 0);
		m_wallRight = base.Game.CreatePhysicalObj(1460, 580, "wallLeft", "asset.game.4.zao", "1", 1, 0);
		m_wallRight.SetRect(-75, -159, 100, 130);
		m_boss = base.Game.CreateBoss(bossID, 1480, 610, -1, 1, "");
		king = base.Game.CreateBoss(bossID2, 1617, 544, -1, 1, "");
		boss = base.Game.CreateBoss(bossID3, 1300, 650, -1, 1, "");
		boss.FallFrom(1300, 650, "", 0, 0, 1000);
		m_NPC = base.Game.Createlayer(1550, 650, "NPC", "game.living.Living154", "stand", 1, 0);
		n_NPC = base.Game.Createlayer(1367, 845, "NPC", "game.living.Living147", "stand", 1, 0);
		king.SetRelateDemagemRect(-34, -35, 50, 40);
		boss.SetRelateDemagemRect(-34, -35, 50, 40);
		m_boss.SetRelateDemagemRect(-34, -35, 50, 40);
		m_moive.PlayMovie("in", 3000, 0);
		m_front.PlayMovie("in", 3000, 0);
		m_moive.PlayMovie("out", 4000, 0);
		m_front.PlayMovie("out", 4000, 0);
		m_moive1 = base.Game.Createlayer(1617, 530, "moive", "asset.game.4.mubiao", "out", 1, 0);
		m_moive2 = base.Game.Createlayer(1300, 635, "moive", "asset.game.4.mubiao", "out", 1, 0);
		m_moive3 = base.Game.Createlayer(1480, 595, "moive", "asset.game.4.mubiao", "out", 1, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (boss == null || boss.IsLiving || king == null || king.IsLiving)
		{
			return;
		}
		m_leftWall = base.Game.FindPhysicalObjByName("wallLeft", CanPenetrate: false);
		m_rightWall = base.Game.FindPhysicalObjByName("wallRight", CanPenetrate: false);
		m_wallRight.SetRect(0, 0, 0, 0);
		PhysicalObj[] leftWall = m_leftWall;
		foreach (PhysicalObj physicalObj in leftWall)
		{
			if (physicalObj != null)
			{
				base.Game.RemovePhysicalObj(physicalObj, sendToClient: true);
			}
		}
		leftWall = m_rightWall;
		foreach (PhysicalObj physicalObj in leftWall)
		{
			if (physicalObj != null)
			{
				base.Game.RemovePhysicalObj(physicalObj, sendToClient: true);
			}
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
			if (m_moive1 != null)
			{
				base.Game.RemovePhysicalObj(m_moive1, sendToClient: true);
				m_moive1 = null;
			}
			if (m_moive2 != null)
			{
				base.Game.RemovePhysicalObj(m_moive2, sendToClient: true);
				m_moive2 = null;
			}
			if (m_moive3 != null)
			{
				base.Game.RemovePhysicalObj(m_moive3, sendToClient: true);
				m_moive3 = null;
			}
			if (m_front != null)
			{
				base.Game.RemovePhysicalObj(m_front, sendToClient: true);
				m_front = null;
			}
			if (m_NPC != null)
			{
				base.Game.RemovePhysicalObj(m_NPC, sendToClient: true);
				m_NPC = null;
			}
			if (n_NPC != null)
			{
				base.Game.RemovePhysicalObj(n_NPC, sendToClient: true);
				n_NPC = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (m_boss != null && !m_boss.IsLiving)
		{
			m_kill++;
			return true;
		}
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
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
		if (m_boss != null && !m_boss.IsLiving)
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

	private void OnDie()
	{
		if (!king.IsLiving && IsEixt == 0)
		{
			npc = base.Game.Createlayer(king.X, king.Y, "", "game.living.Living144", "standB", 1, 0);
			IsEixt = 1;
		}
		if (!boss.IsLiving && IsEixt2 == 0)
		{
			npc2 = base.Game.Createlayer(boss.X, boss.Y, "", "game.living.Living152", "standB", 1, 0);
			IsEixt2 = 1;
		}
	}

	public override void OnShooted()
	{
		if (!king.IsLiving)
		{
			king.CallFuction(OnDie, 8000);
		}
		if (!boss.IsLiving)
		{
			boss.CallFuction(OnDie, 8000);
		}
	}
}
}
