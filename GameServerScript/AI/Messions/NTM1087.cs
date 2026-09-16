using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class NTM1087 : AMissionControl
{
	private int mapId = 2012;

	private SimpleBoss m_boss;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int bossID = 23003;

	private int redNpcID = 23001;

	private int blueNpcID = 23002;

	private List<SimpleNpc> simpleNpcList = new List<SimpleNpc>();

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
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
		base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
		int[] npcIds = new int[3] { redNpcID, blueNpcID, bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(mapId);
	}

	public override void OnStartGame()
	{
		CreateNpc();
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
		foreach (SimpleNpc simpleNpc in simpleNpcList)
		{
			if (simpleNpc.IsLiving)
			{
				return false;
			}
		}
		if (m_boss != null && !m_boss.IsLiving)
		{
			return true;
		}
		if (m_boss == null)
		{
			CreateBoss();
		}
		return false;
	}

	public void CreateBoss()
	{
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(200, 470, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID, 260, 560, 1, 1, "");
		m_boss.FallFrom(260, 620, "fall", 0, 2, 1000);
		m_boss.SetRelateDemagemRect(m_boss.NpcInfo.X, m_boss.NpcInfo.Y, m_boss.NpcInfo.Width, m_boss.NpcInfo.Height);
		m_boss.Say("Loài người kia，đến được đây quả nhiên có chút bản lĩnh！", 0, 3000);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6000, 0);
		m_moive.PlayMovie("out", 9000, 0);
		m_front.PlayMovie("out", 9000, 0);
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (base.Game.GetLivedLivings().Count == 0)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	private void CreateNpc()
	{
		int[,] array = new int[5, 2]
		{
			{ 260, 620 },
			{ 312, 625 },
			{ 350, 621 },
			{ 285, 620 },
			{ 331, 625 }
		};
		for (int i = 0; i <= 2; i++)
		{
			simpleNpcList.Add(base.Game.CreateNpc(redNpcID, array[i, 0], array[i, 1], 1, 1));
		}
		for (int i = 3; i <= 4; i++)
		{
			simpleNpcList.Add(base.Game.CreateNpc(blueNpcID, array[i, 0], array[i, 1], 1, 1));
		}
	}
}
}
