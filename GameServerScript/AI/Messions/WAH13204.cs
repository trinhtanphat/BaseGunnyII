using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class WAH13204 : AMissionControl
{
	private List<SimpleBoss> someBoss = new List<SimpleBoss>();

	private int bossID = 13208;

	private int bossID2 = 13209;

	private int npcID = 13212;

	private int kill = 0;

	private int m_kill = 0;

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
		int[] npcIds = new int[3] { bossID2, bossID, npcID };
		int[] npcIds2 = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/10/tedabiaoji.swf", "asset.game.ten.tedabiaoji");
		base.Game.AddLoadingFile(2, "image/game/effect/10/qunbao.swf", "asset.game.ten.qunbao");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.canbaoAsset");
		base.Game.SetMap(1217);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 1);
		m_front = base.Game.Createlayer(1000, 600, "font", "game.asset.living.canbaoAsset", "out", 1, 1);
		SimpleBoss simpleBoss = base.Game.CreateBoss(bossID, 100, 990, 1, 1, "");
		simpleBoss.SetRelateDemagemRect(simpleBoss.NpcInfo.X, simpleBoss.NpcInfo.Y, simpleBoss.NpcInfo.Width, simpleBoss.NpcInfo.Height);
		someBoss.Add(simpleBoss);
		simpleBoss = base.Game.CreateBoss(bossID2, 1890, 990, -1, 1, "");
		simpleBoss.SetRelateDemagemRect(simpleBoss.NpcInfo.X, simpleBoss.NpcInfo.Y, simpleBoss.NpcInfo.Width, simpleBoss.NpcInfo.Height);
		someBoss.Add(simpleBoss);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6100, 0);
		m_moive.PlayMovie("out", 10000, 1000);
		m_front.PlayMovie("out", 9900, 0);
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
		kill = 0;
		bool flag = true;
		base.CanGameOver();
		foreach (SimpleBoss item in someBoss)
		{
			if (item.IsLiving)
			{
				flag = false;
			}
			else
			{
				kill++;
			}
		}
		if (flag && kill == base.Game.MissionInfo.TotalCount)
		{
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
		if (base.Game.GetLivedLivings().Count == 0)
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
