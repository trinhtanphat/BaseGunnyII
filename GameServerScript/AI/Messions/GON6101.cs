using System.Drawing;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.Messions
{

public class GON6101 : AMissionControl
{
	private int turn = 0;

	private SimpleBoss m_boss;

	private PhysicalObj m_kingMoive;

	private PhysicalObj m_kingFront;

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
		base.Game.AddLoadingFile(2, "image/game/thing/bossborn6.swf", "game.asset.living.GuizeAsset");
		base.Game.AddLoadingFile(2, "image/game/effect/6/ball.swf", "asset.game.six.ball");
		base.Game.AddLoadingFile(2, "image/game/effect/6/jifenpai.swf", "asset.game.six.fenshu");
		base.Game.SetMap(1165);
	}

	public override void OnStartGame()
	{
		base.OnStartGame();
	}

	private void CreatBall()
	{
		int[] array = new int[28]
		{
			1199, 973, 842, 705, 971, 1110, 1240, 799, 662, 556,
			572, 731, 926, 1106, 587, 1305, 775, 941, 1127, 675,
			889, 1147, 462, 493, 846, 537, 771, 1009
		};
		int[] array2 = new int[28]
		{
			812, 776, 718, 765, 617, 648, 574, 596, 624, 702,
			496, 472, 495, 476, 345, 374, 332, 338, 313, 245,
			196, 198, 585, 411, 860, 228, 127, 111
		};
		string[] array3 = new string[16]
		{
			"s1", "s2", "s3", "s4", "s5", "double", "s1", "s2", "s3", "s4",
			"s5", "s1", "s2", "s3", "s4", "s5"
		};
		string[] array4 = new string[15]
		{
			"s-1", "s-2", "s-3", "s-4", "s-5", "s-1", "s-2", "s-3", "s-4", "s-5",
			"s-1", "s-2", "s-3", "s-4", "s-5"
		};
		Point[] array5 = new Point[5]
		{
			new Point(1199, 812),
			new Point(973, 776),
			new Point(842, 718),
			new Point(705, 765),
			new Point(971, 617)
		};
		base.Game.Shuffer(array);
		base.Game.Shuffer(array2);
		base.Game.Shuffer(array5);
		for (int i = 0; i < array5.Length; i++)
		{
			int num = base.Game.Random.Next(array4.Length);
			if (i == 3 || i == 7 || i == 13)
			{
				base.Game.CreateBall(array5[i].X, array5[i].Y, array4[num]);
				continue;
			}
			num = base.Game.Random.Next(array3.Length);
			base.Game.CreateBall(array5[i].X, array5[i].Y, array3[num]);
		}
	}

	public override void OnNewTurnStarted()
	{
		base.OnNewTurnStarted();
		base.Game.ClearBall();
		CreatBall();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
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
		}
	}

	public override bool CanGameOver()
	{
		base.CanGameOver();
		return m_boss != null && !m_boss.IsLiving;
	}

	public override int UpdateUIData()
	{
		return base.UpdateUIData();
	}

	public override void OnGameOver()
	{
		base.OnGameOver();
		if (m_boss != null && !m_boss.IsLiving)
		{
			base.Game.IsWin = true;
		}
	}
}
}
