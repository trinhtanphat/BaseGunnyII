using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ThirteenHardThirdBoss : ABrain
{
	public int attackingTurn = 0;

	public int orchinIndex = 1;

	public int currentCount = 0;

	public int Dander = 0;

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

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
			return;
		}
		base.Body.DoAction = -1;
		if (attackingTurn == 0)
		{
			attackingTurn++;
		}
		else if (attackingTurn == 1)
		{
			Jump();
			attackingTurn++;
		}
		else
		{
			Run();
			attackingTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void Jump()
	{
		base.Body.PlayMovie("jump", 1000, 6000);
		Player player = base.Game.FindRandomPlayer();
		base.Body.JumpToSpeed(player.X, base.Body.Y - 1000, "", 2500, 1, 10, fall);
	}

	public void fall()
	{
		base.Body.CurrentDamagePlus = 3f;
		base.Body.PlayMovie("fall", 0, 0);
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 0, null);
	}

	private void StandC()
	{
		base.Body.PlayMovie("standC", 0, 0);
		base.Body.DoAction = 5;
	}

	private void Run()
	{
		base.Body.CurrentDamagePlus = 5f;
		int x = base.Game.Random.Next(1800, 1800);
		base.Body.MoveTo(x, base.Body.Y, "walk", 1000, "", 25, JumpBack);
		base.Body.RangeAttacking(0, base.Game.Map.Info.ForegroundWidth + 1, "cry", 1000, null);
	}

	public void JumpBack()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.PlayMovie("jump", 1000, 6000);
		Player player = base.Game.FindRandomPlayer();
		base.Body.JumpToSpeed(player.X, base.Body.Y - 1000, "", 2500, 1, 10, FallBack);
	}

	public void FallBack()
	{
		base.Body.PlayMovie("fall", 0, 0);
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
	}

	private void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		base.Body.Say(KillAttackChat[num], 1, 1000);
		base.Body.PlayMovie("beat", 3000, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 4000, null);
	}
}
