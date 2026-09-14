using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class CHM1273 : AMissionControl
{
	private SimpleBoss m_boss;

	private PhysicalObj m_moive;

	private PhysicalObj m_front;

	private int IsSay = 0;

	private int bossID = 1203;

	private int npcID = 1209;

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
		base.Game.AddLoadingFile(2, "image/bomb/blastout/blastout61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/bomb/bullet/bullet61.swf", "bullet61");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.boguoLeaderAsset");
		int[] npcIds = new int[2] { bossID, npcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1073);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "1273";
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(680, 330, "font", "game.asset.living.boguoLeaderAsset", "out", 1, 0);
		m_boss = base.Game.CreateBoss(bossID, 770, -1500, -1, 4, "");
		m_boss.FallFrom(770, 301, "fall", 0, 2, 1000);
		m_boss.SetRelateDemagemRect(34, -35, 11, 18);
		m_boss.AddDelay(10);
		m_boss.Say("你们胆敢闯入我的地盘，准备受死吧！", 0, 6000);
		m_boss.PlayMovie("call", 5900, 0);
		m_moive.PlayMovie("in", 9000, 0);
		m_boss.PlayMovie("weakness", 10000, 5000);
		m_front.PlayMovie("in", 9000, 0);
		m_moive.PlayMovie("out", 15000, 0);
		base.Game.BossCardCount = 1;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.AfterKilledByLiving += OnKillPlayer;
		}
		IsSay = 0;
	}

	public void OnKillPlayer(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (m_boss != null)
		{
			int num = base.Game.Random.Next(0, KillChat.Length);
			m_boss.Say(KillChat[num], 0, 0);
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
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		if (!m_boss.IsLiving)
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
		if (!m_boss.IsLiving)
		{
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

	public override void DoOther()
	{
		base.DoOther();
		int num = base.Game.Random.Next(0, KillChat.Length);
		if (m_boss != null)
		{
			m_boss.Say(KillChat[num], 0, 0);
		}
	}

	public override void OnShooted()
	{
		if (m_boss != null && m_boss.IsLiving && IsSay == 0)
		{
			int num = base.Game.Random.Next(0, ShootedChat.Length);
			m_boss.Say(ShootedChat[num], 0, 1500);
			IsSay = 1;
		}
	}
}
