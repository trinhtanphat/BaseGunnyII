using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FiveTerrorFirstBoss : ABrain
{
	private int m_attackTurn = 0;

	private PhysicalObj m_moive;

	private PhysicalObj m_wallRight = null;

	private static string[] AllAttackChat = new string[3] { "要地震喽！！<br/>各位请扶好哦", "把你武器震下来！", "看你们能还经得起几下！！" };

	private static string[] ShootChat = new string[3] { "让你知道什么叫百发百中！", "送你一个球~你可要接好啦", "你们这群无知的低等庶民" };

	private static string[] ShootedChat = new string[2] { "哎呀~~你们为什么要攻击我？<br/>我在干什么？", "噢~~好痛!我为什么要战斗？<br/>我必须战斗…" };

	private static string[] AddBooldChat = new string[3] { "扭啊扭~<br/>扭啊扭~~", "哈利路亚~<br/>路亚路亚~~", "呀呀呀，<br/>好舒服啊！" };

	private static string[] KillAttackChat = new string[1] { "君临天下！！" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		bool flag = false;
		int num = 0;
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 0 && allFightPlayer.X < 0)
			{
				int num2 = (int)base.Body.Distance(allFightPlayer.X, allFightPlayer.Y);
				if (num2 > num)
				{
					num = num2;
				}
				flag = true;
			}
		}
		if (flag)
		{
			KillAttack(1400, 1600);
		}
		else if (m_attackTurn == 0)
		{
			BeatA();
			base.Body.SetXY(base.Body.X, 659);
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			base.Body.SetXY(base.Body.X, 559);
			BeatB();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			base.Body.SetXY(base.Body.X, 459);
			BeatC();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			base.Body.SetXY(base.Body.X, 359);
			BeatD();
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			if (base.Body.Y == 758)
			{
				m_attackTurn = 0;
				return;
			}
			BeatE();
			base.Body.SetXY(base.Body.X, 259);
			m_attackTurn++;
		}
		else if (m_attackTurn == 5)
		{
			if (base.Body.Y == 758)
			{
				m_attackTurn = 0;
				return;
			}
			BeatG();
			base.Body.SetXY(base.Body.X, 259);
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.CurrentDamagePlus = 10f;
		base.Body.PlayMovie("beat", 3000, 10000);
		base.Body.RangeAttacking(fx, tx, "cry", 5000, null);
	}

	private void BeatA()
	{
		base.Body.PlayMovie("beatA", 3000, 11000);
		base.Body.CallFuction(CallBeatA, 11000);
	}

	private void CallBeatA()
	{
		base.Body.PlayMovie("standA", 2000, 0);
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		int blood = base.Game.Random.Next(1000, 2510);
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ContinueReduceBloodEffect(2, blood, item), 0);
		}
	}

	private void BeatB()
	{
		base.Body.PlayMovie("beatB", 3000, 10000);
		Player player = base.Game.FindRandomPlayer();
		((SimpleBoss)base.Body).NpcInfo.FireY = 0;
		if (player != null)
		{
			base.Body.ShootPoint(player.X, player.Y, 56, 1000, 10000, 1, 2f, 10000);
		}
		base.Body.CallFuction(CallBeatB, 11000);
	}

	private void CallBeatB()
	{
		base.Body.PlayMovie("standB", 3000, 0);
	}

	private void BeatC()
	{
		base.Body.PlayMovie("beatC", 3000, 10000);
		base.Body.CallFuction(CallBeatC, 11000);
	}

	private void CallBeatC()
	{
		base.Body.PlayMovie("standC", 3000, 0);
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			int num = base.Game.Random.Next(200, 510);
			m_wallRight = ((PVEGame)base.Game).CreatePhysicalObj(item.X, item.Y, "wallLeft", "asset.game.4.zap", "1", 1, 1);
			item.AddEffect(new ReduceStrengthEffect(2, 5), 0);
			item.AddBlood(-num, 1);
		}
		List<Player> list = new List<Player>();
		foreach (Player item2 in allFightPlayers)
		{
			if (!item2.IsFrost)
			{
				list.Add(item2);
			}
		}
	}

	private void BeatD()
	{
		base.Body.PlayMovie("beatD", 3000, 10000);
		base.Body.CallFuction(CallBeatD, 11000);
	}

	private void CallBeatD()
	{
		base.Body.PlayMovie("standD", 3000, 0);
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			int num = base.Game.Random.Next(100, 515);
			m_moive = ((PVEGame)base.Game).Createlayer(allFightPlayer.X, allFightPlayer.Y, "moive", "asset.game.4.minigun", "", 1, 1);
			allFightPlayer.AddBlood(-num, 1);
			allFightPlayer.AddBlood(-num, 1);
			allFightPlayer.AddBlood(-num, 1);
			allFightPlayer.AddBlood(-num, 1);
			allFightPlayer.AddBlood(-num, 1);
			allFightPlayer.AddBlood(-num, 1);
		}
	}

	private void BeatE()
	{
		base.Body.PlayMovie("DtoE", 3000, 10000);
		base.Body.CallFuction(BeatG, 10000);
	}

	private void CallBeatE()
	{
		base.Body.PlayMovie("standE", 3000, 0);
	}

	private void BeatG()
	{
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("beatE", 3000, 10000);
		((SimpleBoss)base.Body).NpcInfo.FireY = 20;
		base.Body.ShootPoint(player.X, player.Y, 72, 1000, 10000, 1, 1f, 5500);
		base.Body.CallFuction(CallBeatE, 10000);
	}
}
