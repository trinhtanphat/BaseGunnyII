using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ListPmFireworks : ABrain
{
	private int m_attackTurn = 0;

	private int m_turn = 0;

	private PhysicalObj m_wallLeft = null;

	private PhysicalObj m_wallRight = null;

	private int IsEixt = 0;

	private static string[] AllAttackChat = new string[3] { "要地震喽！！<br/>各位请扶好哦", "把你武器震下来！", "看你们能还经得起几下！！" };

	private static string[] ShootChat = new string[3] { "让你知道什么叫百发百中！", "送你一个球~你可要接好啦", "你们这群无知的低等庶民" };

	private static string[] ShootedChat = new string[2] { "哎呀~~你们为什么要攻击我？<br/>我在干什么？", "噢~~好痛!我为什么要战斗？<br/>我必须战斗…" };

	private static string[] KillPlayerChat = new string[3] { "马迪亚斯不要再控制我！", "这就是挑战我的下场！", "不！！这不是我的意愿… " };

	private static string[] AddBooldChat = new string[3] { "扭啊扭~<br/>扭啊扭~~", "哈利路亚~<br/>路亚路亚~~", "呀呀呀，<br/>好舒服啊！" };

	private static string[] KillAttackChat = new string[1] { "君临天下！！" };

	private static string[] FrostChat = new string[3] { "来尝尝这个吧", "让你冷静一下", "你们激怒了我" };

	private static string[] WallChat = new string[2] { "神啊，赐予我力量吧！", "绝望吧，看我的水晶防护墙！" };

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
			KillAttack(0, 0);
		}
		else if (m_attackTurn == 0)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			ToA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			BeatA();
			m_attackTurn++;
		}
		else if (m_attackTurn == 4)
		{
			ToA();
			m_attackTurn++;
		}
		else
		{
			BeatA();
			m_attackTurn = 0;
		}
	}

	private void KillAttack(int fx, int tx)
	{
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		if (m_turn == 0)
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say(KillAttackChat[num], 1, 13000);
			base.Body.PlayMovie("beat1", 15000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 17000, null);
			m_turn++;
		}
		else
		{
			base.Body.CurrentDamagePlus = 10f;
			base.Body.Say(KillAttackChat[num], 1, 0);
			base.Body.PlayMovie("beat1", 2000, 0);
			base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
		}
	}

	private void ToA()
	{
		base.Body.PlayMovie("toA", 1000, 3000);
	}

	private void BeatA()
	{
		base.Body.PlayMovie("beatA", 1000, 9000);
		base.Body.RangeAttacking(base.Body.X + 9000, base.Body.X - 9000, "cry", 3000, null);
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
}
