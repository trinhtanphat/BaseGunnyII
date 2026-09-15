using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirdSimpleKingThird : ABrain
{
	private int attackingTurn = 1;

	private int orchinIndex = 1;

	private int currentCount = 0;

	private int Dander = 0;

	private int npcID = 3003;

	private int npcID2 = 3112;

	private int npcID1 = 3013;

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

	private static string[] AllAttackChat = new string[4] { "Tiếng gầm của mảnh hổ !!...", "这招酷吧，<br/>想学不？", "消失吧！！！<br/>卑微的灰尘！", "你们会为此付出代价的！ " };

	private static string[] ShootChat = new string[5] { "你是在给我挠痒痒吗？", "我可不会像刚才那个废物一样被你打败！", "哎哟，你打的我好疼啊，<br/>哈哈哈哈！", "啧啧啧，就这样的攻击力！", "看到我是你们的荣幸！" };

	private static string[] CallChat = new string[1] { "Vũ điệu<br/>săn bắn...." };

	private static string[] AngryChat = new string[1] { "是你们逼我使出绝招的！" };

	private static string[] KillAttackChat = new string[1] { "Muốn xem lợi hại của cổ họng của ta!" };

	private static string[] SealChat = new string[1] { "异次元放逐！" };

	private static string[] KillPlayerChat = new string[2] { "灭亡是你唯一的归宿！", "太不堪一击了！" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		bool flag = false;
		int num = 0;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 592 && allFightPlayer.X < 872)
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
			KillAttack(592, 872);
		}
		else if (!flag)
		{
			if (attackingTurn == 1)
			{
				HalfAttack();
			}
			else if (attackingTurn == 2)
			{
				PersonalAttack();
			}
			else if (attackingTurn == 3)
			{
				Summon();
			}
			else if (attackingTurn == 4)
			{
				PersonalAttackDame();
			}
			else
			{
				SummonNpc();
				attackingTurn = 1;
			}
			attackingTurn++;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
		base.Game.RemoveLiving(npcID);
	}

	public void HalfAttack()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		int num = base.Game.Random.Next(0, SealChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.RangeAttacking(base.Body.X - 2000, base.Body.Y + 2000, "cry", 3000, null);
	}

	private void PersonalAttackDame()
	{
		Player player = base.Game.FindRandomPlayer();
		Player player2 = base.Game.FindRandomPlayer();
		Player player3 = base.Game.FindRandomPlayer();
		if (player.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 800);
		}
		else
		{
			base.Body.ChangeDirection(-1, 800);
		}
		if (player != null)
		{
			int num = base.Game.Random.Next(player.X, player.X);
			if (base.Body.ShootPoint(player.X, player.Y, 55, 1000, 10000, 1, 1.5f, 2550))
			{
				base.Body.PlayMovie("beatB", 1700, 0);
			}
		}
	}

	private void PersonalAttack()
	{
		int x = base.Game.Random.Next(700, 800);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 3, NextAttack);
	}

	private void NextAttack()
	{
		Player player = base.Game.FindRandomPlayer();
		Player player2 = base.Game.FindRandomPlayer();
		Player player3 = base.Game.FindRandomPlayer();
		if (player.X > base.Body.Y)
		{
			base.Body.ChangeDirection(1, 800);
		}
		else
		{
			base.Body.ChangeDirection(-1, 800);
		}
		if (player != null)
		{
			int num = base.Game.Random.Next(player.X, player.X);
			if (base.Body.ShootPoint(player.X, player.Y, 54, 1000, 10000, 1, 1.5f, 2550))
			{
				base.Body.PlayMovie("beatA", 1700, 0);
			}
		}
	}

	public void Summon()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 0);
		base.Body.PlayMovie("callA", 100, 0);
		base.Body.CallFuction(CreateChild2, 2500);
	}

	public void SummonNpc()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 0);
		base.Body.PlayMovie("callB", 100, 0);
		base.Body.CallFuction(CreateChild, 2500);
	}

	public void KillAttack(int fx, int mx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 520, 395, 50, 6, -1);
	}

	public void CreateChild2()
	{
		int x = base.Game.Random.Next(100, 400);
		int disToSecond = base.Game.Random.Next(150, 300);
		((SimpleBoss)base.Body).CreateChild(npcID2, x, 395, disToSecond, 6, -1);
	}
}
}
