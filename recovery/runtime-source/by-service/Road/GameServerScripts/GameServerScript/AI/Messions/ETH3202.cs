using System.Collections.Generic;
using Bussiness;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class ETH3202 : AMissionControl
{
	private List<SimpleNpc> someNpc = new List<SimpleNpc>();

	private SimpleBoss boss = null;

	private SimpleBoss m_king = null;

	protected int m_maxBlood;

	protected int m_blood;

	private int npcID = 3202;

	private int npcID1 = 3207;

	private int npcID2 = 3205;

	private int bossId = 3208;

	private int kill = 0;

	private SimpleBoss m_boss = null;

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
		int[] npcIds = new int[4] { npcID, npcID1, npcID2, bossId };
		int[] npcIds2 = new int[4] { npcID, npcID1, npcID2, bossId };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
		base.Game.SetMap(1123);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "3202";
		m_king = base.Game.CreateBoss(bossId, 100, 444, 1, 1, "");
		m_king.FallFrom(m_king.X, m_king.Y, "", 0, 0, 2000);
		m_king.PlayMovie("castA", 500, 0);
		m_king.CallFuction(CreateStarGame, 2500);
	}

	public void CreateStarGame()
	{
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsHelper = true;
		livingConfig.ReduceBloodStart = 3;
		boss = base.Game.CreateBoss(npcID1, 1100, 444, -1, 10, "", livingConfig);
		boss.FallFrom(boss.X, boss.Y, "", 0, 0, 1000, null);
		m_boss = base.Game.CreateBoss(npcID2, 300, 444, 1, 0, "");
		m_boss.FallFrom(m_boss.X, m_boss.Y, "", 0, 1, 1000, null);
		someNpc.Add(base.Game.CreateNpc(npcID, 450, 344, 1, 1));
		someNpc.Add(base.Game.CreateNpc(npcID, 400, 344, 1, 1));
		someNpc.Add(base.Game.CreateNpc(npcID, 350, 344, 1, 1));
		base.Game.SendGameFocus(boss, 500, 3000);
		boss.Say(LanguageMgr.GetTranslation("Hồi máu cho tôi, tôi sẻ dẫn các cậu ra khỏi đây !"), 0, 1500, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		if (m_boss != null && !m_boss.IsLiving)
		{
			m_boss = base.Game.CreateBoss(npcID2, 300, 444, 1, 1, "");
			m_boss.FallFrom(m_boss.X, m_boss.Y, "", 0, 0, 1000, null);
			someNpc.Add(base.Game.CreateNpc(npcID, 450, 344, 1, 1));
			someNpc.Add(base.Game.CreateNpc(npcID, 400, 344, 1, 1));
			someNpc.Add(base.Game.CreateNpc(npcID, 350, 344, 1, 1));
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		if (base.Game.TurnIndex == 1 && m_king != null && m_king.IsLiving)
		{
			m_king.PlayMovie("out", 0, 2000);
			m_king.CallFuction(CreateOutGame, 1200);
		}
	}

	public void CreateOutGame()
	{
		base.Game.RemoveLiving(m_king.Id);
		m_king.Die();
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		if (boss.Blood == boss.NpcInfo.Blood)
		{
			return true;
		}
		if (boss == null || boss.IsLiving)
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
		if (boss.Blood == boss.NpcInfo.Blood)
		{
			boss.PlayMovie("grow", 0, 1000);
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
		List<LoadingFileInfo> list = new List<LoadingFileInfo>();
		list.Add(new LoadingFileInfo(2, "image/map/show4.jpg", ""));
		base.Game.SendLoadResource(list);
	}
}
