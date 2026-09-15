using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class ZYSimpleNpc60005 : ABrain
{
	private int attackingTurn = 1;

	private int npcID = 1311;

	private static string[] AllAttackChat = new string[4] { "Xem tuyệt chiêu nè!", "Di chuyển mát mẻ!<br/>Bạn muốn tìm hiểu không?", "Chụi không nỗi!", "Bạn sẽ trả giá cho việc này! " };

	private static string[] CallChat = new string[1] { "Nào, <br/>cho thử sức mạnh của lựu đạn!" };

	private static string[] AngryChat = new string[1] { "Bạn buộc tôi để lừa!" };

	private static string[] KillAttackChat = new string[1] { "Bạn đến chết?" };

	private static string[] SealChat = new string[1] { "Chầu Diêm Vương!" };

	private static string[] KillPlayerChat = new string[2] { "Địa ngục là điểm đến duy nhất của bạn!", "Quá dễ bị tổn thương." };

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

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
		base.OnStartAttacking();
		bool flag = false;
		int num = 0;
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		foreach (Player allFightPlayer in base.Game.GetAllFightPlayers())
		{
			if (allFightPlayer.IsLiving && allFightPlayer.X > 740 && allFightPlayer.X < 1040)
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
			KillAttack(620, 1160);
			return;
		}
		if (attackingTurn == 1)
		{
			Healing();
			HalfAttack();
		}
		else if (attackingTurn == 2)
		{
			Healing();
			Summon();
		}
		else if (attackingTurn == 3)
		{
			Healing();
			Seal();
		}
		else
		{
			Healing();
			attackingTurn = 0;
		}
		attackingTurn++;
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
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		base.Body.Seal(player, 1, 3000);
	}

	public void KillAttack(int fx, int tx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatB", 2500, 0);
		base.Body.RangeAttacking(fx, tx, "cry", 3300, null);
	}

	public void Healing()
	{
		base.Body.SyncAtTime = true;
		base.Body.AddBlood(5000);
		base.Body.Say("Haha, tôi là đầy sức mạnh!", 1, 0);
	}

	public void CreateChild()
	{
		((SimpleBoss)base.Body).CreateChild(npcID, 680, 680, 405, 6, -1);
	}
}
