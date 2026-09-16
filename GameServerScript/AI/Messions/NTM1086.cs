using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class NTM1086 : AMissionControl
{
	private int mapId = 1015;

	private SimpleBoss m_boss;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int bossID = 22001;

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
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.qiheibojueAsset");
		base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout86.swf", "bullet86");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet86.swf", "bullet86");
		int[] npcIds = new int[1] { bossID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(mapId);
	}

	public override void OnStartGame()
	{
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(680, 330, "font", "game.asset.living.qiheibojueAsset", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID, 750, 420, -1, 1, "");
		m_boss.FallFrom(750, 520, "fall", 0, 2, 1000);
		m_boss.SetRelateDemagemRect(m_boss.NpcInfo.X, m_boss.NpcInfo.Y, m_boss.NpcInfo.Width, m_boss.NpcInfo.Height);
		m_boss.Say("Đến đúng lúc lắm，ta đang chán đây！", 0, 4000);
		m_moive.PlayMovie("in", 6000, 0);
		m_front.PlayMovie("in", 6000, 0);
		m_moive.PlayMovie("out", 9000, 0);
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex > 1)
		{
			if (m_moive != null)
			{
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
		if (m_boss != null && !m_boss.IsLiving)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return base.Game.TotalKillCount;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
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
