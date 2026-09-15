using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class ETN3103 : AMissionControl
{
	private SimpleBoss boss = null;

	private SimpleBoss m_boss = null;

	private SimpleBoss m_king = null;

	private int m_kill = 0;

	private int IsSay = 0;

	private int bossID = 3108;

	private int bossID2 = 3109;

	private int npcID = 3106;

	private int npcID2 = 3110;

	private int npcID3 = 3111;

	private PhysicalObj m_moive;

	private PhysicalObj m_front = null;

	private static string[] KillChat = new string[3] { "A~đau quá!", "Ngươi dám đối mặt?", "Đánh nữa ta đánh trả đó!" };

	private static string[] ShootedChat = new string[2] { "Ah ~ ~ Tại sao bạn tấn công?<br/>Tôi đang làm?", "Oh ~ ~ nó thực sự đau khổ! Tại sao tôi phải chiến đấu?<br/>Tôi phải chiến đấu ..." };

	public override int CalculateScoreGrade(int score)
	{
		base.CalculateScoreGrade(score);
		if (score > 1330)
		{
			return 3;
		}
		if (score > 1150)
		{
			return 2;
		}
		if (score > 970)
		{
			return 1;
		}
		return 0;
	}

	public override void OnPrepareNewSession()
	{
		base.OnPrepareNewSession();
		int[] npcIds = new int[4] { bossID, bossID2, npcID, npcID2 };
		int[] npcIds2 = new int[2] { bossID, bossID2 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds2);
		base.Game.AddLoadingFile(1, "bombs/54.swf", "tank.resource.bombs.Bomb54");
		base.Game.AddLoadingFile(1, "bombs/58.swf", "tank.resource.bombs.Bomb58");
		base.Game.AddLoadingFile(2, "image/game/effect/3/buff.swf", "asset.game.4.buff");
		base.Game.AddLoadingFile(2, "image/game/effect/3/dici.swf", "asset.game.4.dici");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.ClanBrotherAsset");
		base.Game.AddLoadingFile(2, "image/game/living/living117.swf", "living117_fla.walk_6");
		base.Game.SetMap(1124);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		base.Game.IsBossWar = "3103";
		m_moive = base.Game.Createlayer(0, 0, "moive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_front = base.Game.Createlayer(650, 400, "front", "game.asset.living.ClanBrotherAsset", "out", 1, 0);
		m_king = base.Game.CreateBoss(bossID, 1360, 357, -1, 1, "");
		m_king.FallFrom(m_king.X, m_king.Y, "", 0, 0, 1000, null);
		m_king.SetRelateDemagemRect(m_king.NpcInfo.X, m_king.NpcInfo.Y, m_king.NpcInfo.Width, m_king.NpcInfo.Height);
		boss = base.Game.CreateBoss(bossID2, 255, 357, 1, 1, "");
		boss.FallFrom(boss.X, boss.Y, "", 0, 0, 1000, null);
		boss.SetRelateDemagemRect(boss.NpcInfo.X, boss.NpcInfo.Y, boss.NpcInfo.Width, boss.NpcInfo.Height);
		m_moive.PlayMovie("in", 5000, 0);
		m_front.PlayMovie("in", 5000, 0);
		m_moive.PlayMovie("out", 9000, 0);
		m_front.PlayMovie("out", 9400, 0);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
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

	public override bool CanGameOver()
	{
		if (m_king != null && !m_king.IsLiving && boss != null && !boss.IsLiving)
		{
			m_kill++;
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
		bool flag = true;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving)
			{
				flag = false;
			}
		}
		if (m_king != null && !m_king.IsLiving && boss != null && !boss.IsLiving && !flag)
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
		if (m_king != null)
		{
			if (m_king.IsLiving)
			{
				int num = base.Game.Random.Next(0, KillChat.Length);
				m_king.Say(KillChat[num], 0, 0);
				boss.Say(KillChat[num], 0, 0);
			}
			else
			{
				int num = base.Game.Random.Next(0, KillChat.Length);
				m_king.Say(KillChat[num], 0, 0);
				boss.Say(KillChat[num], 0, 0);
			}
		}
	}

	public override void OnShooted()
	{
		if (IsSay == 0)
		{
			if (m_king.IsLiving)
			{
				int num = base.Game.Random.Next(0, ShootedChat.Length);
				m_king.Say(ShootedChat[num], 0, 1500);
				boss.Say(ShootedChat[num], 0, 1500);
			}
			else
			{
				int num = base.Game.Random.Next(0, ShootedChat.Length);
				m_king.Say(ShootedChat[num], 0, 1500);
				boss.Say(ShootedChat[num], 0, 1500);
			}
			IsSay = 1;
		}
	}
}
