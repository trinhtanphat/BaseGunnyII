using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class NTM1089 : AMissionControl
{
	private int mapId = 1129;

	private int dieCount = 0;

	private int[] birthX = new int[4] { 52, 115, 1155, 1106 };

	private int[] birthY = new int[4] { 388, 392, 399, 387 };

	private int npcID = 25001;

	private int bossID = 25002;

	private SimpleBoss m_boss;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

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
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.shikongAsset");
		int[] npcIds = new int[2] { npcID, bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(mapId);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		int y = birthX[0];
		someNpc.Add(base.Game.CreateNpc(npcID, 52, y, 1, -1));
		someNpc.Add(base.Game.CreateNpc(npcID, 100, y, 1, -1));
		someNpc.Add(base.Game.CreateNpc(npcID, 1120, y, 1, 1));
		someNpc.Add(base.Game.CreateNpc(npcID, 1155, y, 1, 1));
	}

	public void CreateBoss()
	{
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(200, 200, "font", "game.asset.living.shikongAsset", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID, 160, 330, 1, 1, "");
		m_boss.SetRelateDemagemRect(m_boss.NpcInfo.X, m_boss.NpcInfo.Y, m_boss.NpcInfo.Width, m_boss.NpcInfo.Height);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6000, 0);
		m_moive.PlayMovie("out", 9000, 0);
	}

	public override void OnNewTurnStarted()
	{
		if (base.Game.TurnIndex <= 1 || m_boss != null || base.Game.GetLivedLivings().Count >= 4)
		{
			return;
		}
		for (int i = 0; i < 4 - base.Game.GetLivedLivings().Count; i++)
		{
			if (someNpc.Count == 8)
			{
				break;
			}
			int num = base.Game.Random.Next(0, birthX.Length);
			int num2 = birthX[num];
			int direction = 1;
			if (num2 < 200)
			{
				direction = -1;
			}
			someNpc.Add(base.Game.CreateNpc(npcID, num2, birthY[0], 1, direction));
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public override bool CanGameOver()
	{
		bool flag = true;
		base.CanGameOver();
		dieCount = 0;
		foreach (SimpleNpc item in someNpc)
		{
			if (item.IsLiving)
			{
				flag = false;
			}
			else
			{
				dieCount++;
			}
		}
		if (flag && dieCount == 8 && m_boss == null)
		{
			CreateBoss();
		}
		if (m_boss != null && !m_boss.IsLiving)
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
		if (m_boss != null)
		{
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
}
