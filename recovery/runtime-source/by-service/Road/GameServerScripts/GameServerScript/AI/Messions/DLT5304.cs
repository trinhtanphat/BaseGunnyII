using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions;

public class DLT5304 : AMissionControl
{
	private SimpleBoss m_king = null;

	private SimpleBoss king = null;

	private SimpleBoss m_preKing = null;

	private PhysicalObj m_kingMoive;

	private PhysicalObj kingMoive;

	private PhysicalObj m_kingFront;

	private int turn = 0;

	private int m_kill = 0;

	private int IsSay = 0;

	private int bossID = 5331;

	private int bossID2 = 5333;

	private int npcID = 5332;

	private int npcID2 = 5334;

	private static string[] KillChat = new string[2] { "Địa ngục là điểm đến duy nhất của bạn!", "Quá dễ bị tổn thương." };

	private static string[] ShootedChat = new string[3] { "Oh ~ bạn chơi tốt một điều đau khổ!<br/>Ah ha ha ha ha!", "Bạn sẽ chỉ có khả năng này? !", "Có một chút có nghĩa là" };

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
		int[] npcIds = new int[4] { npcID, npcID2, bossID, bossID2 };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.AddLoadingFile(1, "bombs/56.swf", "tank.resource.bombs.Bomb56");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.BossBgAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/5/guang.swf", "asset.game.4.guang");
		base.Game.AddLoadingFile(2, "image/game/effect/5/tang.swf", "asset.game.4.tang");
		base.Game.AddLoadingFile(2, "image/game/effect/5/ruodian.swf", "asset.game.4.ruodian");
		base.Game.AddLoadingFile(2, "image/game/effect/5/jinqudan.swf", "asset.game.4.jinqudan");
		base.Game.AddLoadingFile(2, "image/game/thing/BossBornBgAsset.swf", "game.asset.living.xieyanjulongAsset");
		base.Game.AddLoadingFile(2, "image/game/living/living156.swf", "game.living.Living156");
		base.Game.SetMap(1154);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_kingMoive = base.Game.Createlayer(0, 0, "kingmoive", "game.asset.living.BossBgAsset", "out", 1, 0);
		m_kingFront = base.Game.Createlayer(1300, 280, "font", "game.asset.living.xieyanjulongAsset", "out", 1, 0);
		LivingConfig livingConfig = base.Game.BaseLivingConfig();
		livingConfig.IsFly = true;
		m_king = base.Game.CreateBoss(bossID, 1702, 470, -1, 0, "", livingConfig);
		king = base.Game.CreateBoss(bossID2, 200, 200, 1, 0, "");
		m_king.SetRelateDemagemRect(-100, -79, 172, 100);
		king.Say("Có ta ở đây đừng sợ!", 3000, 0);
		m_kingMoive.PlayMovie("in", 9000, 0);
		m_kingFront.PlayMovie("in", 9000, 0);
		m_kingMoive.PlayMovie("out", 10000, 0);
		m_kingFront.PlayMovie("out", 10400, 0);
		turn = base.Game.TurnIndex;
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		IsSay = 0;
		kingMoive = base.Game.Createlayer(1710, 480, "kingmoive", "asset.game.4.ruodian", "out", 1, 0);
		if (base.Game.TurnIndex > turn + 1)
		{
			if (m_kingMoive != null)
			{
				base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
				m_kingMoive = null;
			}
			if (m_kingFront != null)
			{
				base.Game.RemovePhysicalObj(m_kingFront, sendToClient: true);
				m_kingFront = null;
			}
			if (kingMoive != null)
			{
				base.Game.RemovePhysicalObj(kingMoive, sendToClient: true);
				kingMoive = null;
			}
		}
	}

	public override bool CanGameOver()
	{
		if (!m_king.IsLiving)
		{
			m_kill++;
			return true;
		}
		if (base.Game.TurnIndex > base.Game.MissionInfo.TotalTurn - 1)
		{
			return true;
		}
		return false;
	}

	public override int UpdateUIData()
	{
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
		if (!m_king.IsLiving && !flag)
		{
			base.Game.IsWin = true;
		}
		else
		{
			base.Game.IsWin = false;
		}
	}

	public override void DoOther()
	{
		base.DoOther();
		if (m_king != null)
		{
			int num = base.Game.Random.Next(0, KillChat.Length);
			m_king.Say(KillChat[num], 0, 0);
		}
	}

	public override void OnShooted()
	{
		if (m_king.IsLiving && IsSay == 0)
		{
			int num = base.Game.Random.Next(0, ShootedChat.Length);
			m_king.Say(ShootedChat[num], 0, 1500);
			IsSay = 1;
		}
	}
}
