using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class DCN4101 : AMissionControl
{
	private SimpleBoss m_boss = null;

	private SimpleNpc npc;

	private int kill = 0;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int npcID2 = 4101;

	private int npcID = 4103;

	private int bossID = 4104;

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
		int[] npcIds = new int[3] { bossID, npcID, npcID2 };
		int[] npcIds2 = new int[0];
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(2, "image/game/effect/4/Gate.swf", "game.asset.Gate");
		base.Game.SetMap(1142);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_boss = base.Game.CreateBoss(bossID, 1520, 350, -1, 1, "NoBlood");
		npc = base.Game.CreateNpc(npcID2, 340, 750, 1, 0);
		npc.FallFrom(npc.X, npc.Y, "", 0, 0, 2000);
		base.Game.CreatePhysicalObj(1500, 250, "door", "", "start", 1, 0);
		base.Game.SendGameObjectFocus(1, "door", 2000, 3000);
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
		if (npc != null && !npc.IsLiving)
		{
			npc = base.Game.CreateNpc(npcID2, 340, 750, 1, 0);
			npc.FallFrom(npc.X, npc.Y, "", 0, 0, 2000);
		}
	}

	public override bool CanGameOver()
	{
		if (npc.FindCount != 3)
		{
			return false;
		}
		kill++;
		return true;
	}

	public override int UpdateUIData()
	{
		base.UpdateUIData();
		return kill;
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (npc.FindCount == 3)
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
