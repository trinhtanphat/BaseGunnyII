using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class NormalKingLast : ABrain
{
	private int attackingTurn = 1;

	private int Dander = 0;

	private int npcID = 1104;

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
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 390 && allFightPlayer.X < 1110)
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
			KillAttack(390, 1110);
		}
		else if (!flag)
		{
			if (attackingTurn == 1)
			{
				base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
				HalfAttack();
			}
			else if (attackingTurn == 2)
			{
				base.Body.Direction = -base.Body.Direction;
				Summon();
			}
			else if (attackingTurn == 3)
			{
				base.Body.Direction = -base.Body.Direction;
				Seal();
			}
			else if (attackingTurn == 4)
			{
				base.Body.Direction = -base.Body.Direction;
				Angger();
			}
			else
			{
				base.Body.Direction = -base.Body.Direction;
				GoOnAngger();
				attackingTurn = 0;
			}
			attackingTurn++;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void HalfAttack()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		int num = base.Game.Random.Next(0, SealChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		if (base.Body.Direction == 1)
		{
			base.Body.RangeAttacking(base.Body.X, base.Body.X + 1000, "cry", 3300, null);
		}
		else
		{
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X, "cry", 3300, null);
		}
	}

	public void Summon()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		base.Body.Say(CallChat[num], 1, 0);
		base.Body.PlayMovie("beatA", 100, 0);
		base.Body.CallFuction(CreateChild, 2500);
	}

	public void Seal()
	{
		int num = base.Game.Random.Next(0, SealChat.Length);
		((SimpleBoss)base.Body).Say(SealChat[num], 1, 0);
		Player player = base.Game.FindRandomPlayer();
		base.Body.PlayMovie("mantra", 2000, 2000);
		base.Body.Seal(player, 1, 3000);
	}

	public void Angger()
	{
		int num = base.Game.Random.Next(0, AngryChat.Length);
		base.Body.Say(AngryChat[num], 1, 0);
		base.Body.State = 1;
		Dander += 100;
		((SimpleBoss)base.Body).SetDander(Dander);
		if (base.Body.Direction == -1)
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
		}
		else
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(-82, -252, 74, 50);
		}
	}

	public void GoOnAngger()
	{
		if (base.Body.State == 1)
		{
			base.Body.CurrentDamagePlus = 1000f;
			base.Body.PlayMovie("beatC", 3500, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 5600, null);
			base.Body.Die(5600);
			return;
		}
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -187, 83, 140);
		base.Body.PlayMovie("mantra", 0, 2000);
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ContinueReduceBloodEffect(2, 50, item), 0);
		}
	}

	public void KillAttack(int fx, int mx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 520, 530, 400, 6, 1);
	}
}
