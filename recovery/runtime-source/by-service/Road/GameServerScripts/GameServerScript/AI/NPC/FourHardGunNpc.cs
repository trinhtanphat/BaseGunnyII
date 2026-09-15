using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class FourHardGunNpc : ABrain
{
	public int attackingTurn = 1;

	private int npcID = 4203;

	private static string[] AllAttackChat = new string[4] { "看我的绝技！", "这招酷吧，<br/>想学不？", "消失吧！！！<br/>卑微的灰尘！", "你们会为此付出代价的！ " };

	private static string[] ShootChat = new string[5] { "你是在给我挠痒痒吗？", "我可不会像刚才那个废物一样被你打败！", "哎哟，你打的我好疼啊，<br/>哈哈哈哈！", "啧啧啧，就这样的攻击力！", "看到我是你们的荣幸！" };

	private static string[] CallChat = new string[1] { "来啊，<br/>让他们尝尝炸弹的厉害！" };

	private static string[] AngryChat = new string[1] { "是你们逼我使出绝招的！" };

	private static string[] KillAttackChat = new string[1] { "你来找死吗？" };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 400 && allFightPlayer.X < 1600)
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
			KillAttack(400, 1600);
		}
		else if (!flag)
		{
			((PVEGame)base.Game).SendGameObjectFocus(1, "door", 1000, 0);
			if (attackingTurn == 1)
			{
				BestA();
			}
			else if (attackingTurn == 2)
			{
				BestC();
			}
			else
			{
				BestB();
				attackingTurn = 0;
			}
			attackingTurn++;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void BestB()
	{
		base.Body.PlayMovie("beatB", 0, 3000);
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (SimpleNpc item in list)
		{
			if (item is SimpleNpc)
			{
				list.Add(item as SimpleNpc);
				item.AddEffect(new ContinueReduceBloodEffect(2, 500, item), 0);
			}
		}
	}

	public void BestA()
	{
		base.Body.PlayMovie("beatA", 1000, 0);
	}

	public void BestC()
	{
		base.Body.PlayMovie("beatC", 1000, 0);
		int num = base.Game.Random.Next(400, 1680);
		base.Body.CallFuction(CreateChild, 2500);
	}

	public void CreateChild()
	{
		int x = base.Game.Random.Next(470, 880);
		((SimpleBoss)base.Body).CreateChild(npcID, x, 700, 2, 400, -1);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("beatB", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}
}
