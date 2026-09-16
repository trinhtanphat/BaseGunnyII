using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class CSM1077 : AMissionControl
{
	private SimpleBoss m_king = null;

	private int m_kill = 0;

	private int IsSay = 0;

	private int bossID = 1007;

	private int npcID = 1004;

	private static string[] KillChat = new string[2] { "灭亡是你唯一的归宿！", "太不堪一击了！" };

	private static string[] ShootedChat = new string[3] { "哎哟～你打的我好疼啊！<br/>啊哈哈哈哈！", "你们就只有这点本事？！", "哼～有点意思了" };

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
		int[] npcIds = new int[2] { bossID, npcID };
		base.Game.LoadResources(npcIds);
		base.Game.LoadNpcGameOverResources(npcIds);
		base.Game.SetMap(1076);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
		m_king = base.Game.CreateBoss(bossID, 750, 510, -1, 0, "");
		m_king.SetRelateDemagemRect(-41, -187, 83, 140);
		m_king.Say("你们知道的太多了，我不能让你们继续活着！", 0, 3000);
		m_king.AddDelay(16);
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		IsSay = 0;
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
}
