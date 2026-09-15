using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class TVS12002 : AMissionControl
{
	private static string[] KillChat = new string[2] { "Không có gì phải sợ, đau đớn sẻ qua", "Kẻ tầm thường và thấp hèn." };

	private static string[] ShootedChat = new string[2] { "Chỉ được vậy thôi à ?…", "Sức mạnh cũng thường…" };

	private List<SimpleNpc> SomeNpc = new List<SimpleNpc>();

	private int IsSay = 0;

	private int bossID = 12008;

	private int bossID2 = 12006;

	private int npcID = 12007;

	private SimpleBoss m_boss;

	private SimpleBoss boss;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

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
		return (score > 725) ? 1 : 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		base.Game.AddLoadingFile(1, "bombs/174.swf", "tank.resource.bombs.Bomb174");
		base.Game.AddLoadingFile(2, "image/game/effect/9/biaoji.swf", "asset.game.nine.biaoji");
		base.Game.AddLoadingFile(2, "image/game/effect/9/dapao.swf", "asset.game.nine.dapao");
		base.Game.AddLoadingFile(2, "image/game/effect/5/xiaopao.swf", "asset.game.4.xiaopao");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ducaizheAsset");
		int[] npcIds = new int[3] { bossID, bossID2, npcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1208);
	}

	public static void msg(Living living, Living target, int damageAmount, int criticalAmount)
	{
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(1300, 730, "font", "game.asset.living.ducaizheAsset", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID, 1500, 800, -1, 1, "");
		SomeNpc.Add(base.Game.CreateNpc(bossID2, 150, 700, 1, 1));
		m_boss.FallFrom(m_boss.X, m_boss.Y, "", 0, 0, 1000, null);
		m_boss.SetRelateDemagemRect(-21, -79, 72, 91);
		m_boss.AddDelay(10);
		m_boss.Say("Nếu chiến thắng ta thì các ngươi sẻ tiếp cận được vòng xoáy thời gian!", 0, 6000);
		m_moive.PlayMovie("in", 9000, 0);
		m_front.PlayMovie("in", 9000, 0);
		m_moive.PlayMovie("out", 15000, 0);
		base.Game.BossCardCount = 1;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
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
		base.CanGameOver();
		return base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1 || !m_boss.IsLiving;
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
		bool flag = true;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving)
			{
				flag = false;
			}
		}
		if (!m_boss.IsLiving && !flag)
		{
			base.Game.IsWin = true;
		}
	}
}
