using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{
public class Labyrinth40006 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss boss2 = null;

	private PhysicalObj[] m_leftWall = null;

	private PhysicalObj[] m_rightWall = null;

	private int kill = 0;

	private int turn = 0;

	private int npcID = 1310;

	private int m_state = 40006;

	private int bossID = 40006;

	private int bossID2 = 40007;

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1870)
		{
			return 3;
		}
		if (score > 1825)
		{
			return 2;
		}
		if (score > 1780)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[3] { bossID, bossID2, npcID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/bomb/blastOut/blastOut61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_left");
		base.Game.AddLoadingFile(2, "image/map/1076/objects/1076MapAsset.swf", "com.mapobject.asset.WaveAsset_01_right");
		base.Game.SetMap(1228);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		boss = base.Game.CreateBoss(bossID, 890, 590, -1, 1, "");
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		boss.AddDelay(10);
		turn = base.Game.TurnIndex;
	}

	public override void OnNewTurnStarted()
	{
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!boss.IsLiving && m_state == bossID)
		{
			m_state++;
		}
		if (m_state == bossID2 && boss2 == null)
		{
			boss2 = base.Game.CreateBoss(m_state, boss.X, boss.Y, boss.Direction, 2, "");
			base.Game.RemoveLiving(boss.Id);
			if (boss2.Direction == 1)
			{
				boss2.SetRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
			}
			boss2.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
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
			boss2.AddDelay(num - 2000);
			turn = base.Game.TurnIndex;
		}
		if (boss2 != null && !boss2.IsLiving)
		{
			if (base.Game.CanEnterGate)
			{
				return true;
			}
			kill++;
			base.Game.CanShowBigBox = true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return kill;
	}

	public override void OnGameOverMovie()
	{
		base.OnGameOverMovie();
		if (boss2 != null && !boss2.IsLiving)
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

	public override void OnGameOver()
	{
		base.OnGameOver();
	}
}
}
