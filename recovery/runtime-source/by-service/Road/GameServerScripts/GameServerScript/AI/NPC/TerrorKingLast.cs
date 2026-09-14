using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class TerrorKingLast : ABrain
{
	private int attackingTurn = 1;

	private int Dander = 0;

	private int npcID = 1304;

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

	private static string[] AllAttackChat = new string[4] { "Nghiên cứu kỹ năng của tôi!", "Di chuyển mát mẻ!<br/>Bạn muốn tìm hiểu không?", "Chụi không nỗi!", "Bạn sẽ trả giá cho việc này! " };

	private static string[] CallChat = new string[1] { "Nào, <br/>cho thử sức mạnh của lựu đạn!" };

	private static string[] AngryChat = new string[1] { "Bạn buộc tôi để lừa!" };

	private static string[] KillAttackChat = new string[1] { "Bạn đến chết?" };

	private static string[] SealChat = new string[1] { "Chầu Diêm Vương!" };

	private static string[] KillPlayerChat = new string[2] { "Địa ngục là điểm đến duy nhất của bạn!", "Quá dễ bị tổn thương." };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 620 && allFightPlayer.X < 1160)
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
				base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
				Summon();
			}
			else if (attackingTurn == 3)
			{
				base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
				Seal();
			}
			else if (attackingTurn == 4)
			{
				base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
				Angger();
			}
			else
			{
				base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
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
		base.Body.PlayMovie("angry", 1000, 0);
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
			item.AddEffect(new ContinueReduceBloodEffect(2, 150, item), 0);
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
		((SimpleBoss)base.Body).CreateChild(npcID, 800, 665, 180, 6, -1);
	}
}
