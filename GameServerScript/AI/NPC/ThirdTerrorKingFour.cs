using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirdTerrorKingFour : ABrain
{
	private int attackingTurn = 1;

	private int npcID = 3303;

	private int IsEixt = 0;

	private PhysicalObj m_kingMoive;

	public List<SimpleNpc> orchins = new List<SimpleNpc>();

	private static string[] AllAttackChat = new string[4] { "看我的绝技！", "这招酷吧，<br/>想学不？", "消失吧！！！<br/>卑微的灰尘！", "你们会为此付出代价的！ " };

	private static string[] ShootChat = new string[5] { "Lửa địa ngục...", "我可不会像刚才那个废物一样被你打败！", "哎哟，你打的我好疼啊，<br/>哈哈哈哈！", "啧啧啧，就这样的攻击力！", "看到我是你们的荣幸！" };

	private static string[] CallChat = new string[1] { "来啊，<br/>让他们尝尝炸弹的厉害！" };

	private static string[] AngryChat = new string[1] { "是你们逼我使出绝招的！" };

	private static string[] KillAttackChat = new string[1] { "你来找死吗？" };

	private static string[] SealChat = new string[1] { "Chạy đường nào đây?" };

	private static string[] KillPlayerChat = new string[1] { "Lửa bất diệt cháy bừng lên đi!" };

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
			if (allFightPlayer.IsLiving && allFightPlayer.X > 572 && allFightPlayer.X < 872)
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
			KillAttack(572, 872);
		}
		else if (!flag)
		{
			if (attackingTurn == 1)
			{
				Summon();
			}
			else if (attackingTurn == 2)
			{
				PersonalAttackDame();
			}
			else if (attackingTurn == 3)
			{
				HalfAttack();
			}
			else if (attackingTurn == 4)
			{
				MovePlayer();
			}
			else if (attackingTurn == 5)
			{
				PersonalNpc();
			}
			else
			{
				HalfAttack();
				attackingTurn = 1;
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
		base.Body.RangeAttacking(base.Body.X - 2000, base.Body.Y + 2000, "cry", 3000, null);
	}

	private void PersonalAttackDame()
	{
		Player player = base.Game.FindRandomPlayer();
		int num = base.Game.Random.Next(0, ShootChat.Length);
		((SimpleBoss)base.Body).Say(ShootChat[num], 1, 500);
		if (base.Body.X > player.X)
		{
			if (player.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 50);
			}
			else
			{
				base.Body.ChangeDirection(-1, 50);
			}
			int num2 = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 2.3f, 2600))
			{
				base.Body.PlayMovie("aim", 1000, 0);
				base.Body.PlayMovie("beatA", 1500, 0);
			}
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 2.3f, 4600))
			{
				base.Body.PlayMovie("aim", 3000, 0);
				base.Body.PlayMovie("beatA", 4500, 0);
			}
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 2.3f, 6600))
			{
				base.Body.PlayMovie("aim", 5000, 0);
				base.Body.PlayMovie("beatA", 6500, 0);
			}
		}
		else
		{
			if (player.X > base.Body.Y)
			{
				base.Body.ChangeDirection(1, 50);
			}
			else
			{
				base.Body.ChangeDirection(-1, 50);
			}
			int num2 = base.Game.Random.Next(player.X - 10, player.X + 10);
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 1f, 2600))
			{
				base.Body.PlayMovie("aim", 1000, 0);
				base.Body.PlayMovie("beatA", 1500, 0);
			}
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 1f, 4600))
			{
				base.Body.PlayMovie("aim", 3000, 0);
				base.Body.PlayMovie("beatA", 4500, 0);
			}
			if (base.Body.ShootPoint(player.X, player.Y, 53, 1000, 10000, 3, 1f, 6600))
			{
				base.Body.PlayMovie("aim", 5000, 0);
				base.Body.PlayMovie("beatA", 6500, 0);
			}
		}
	}

	private void PersonalNpc()
	{
		int num = base.Game.Random.Next(0, CallChat.Length);
		((SimpleBoss)base.Body).Say(CallChat[num], 1, 500);
		if (IsEixt == 1)
		{
			base.Body.ChangeDirection(1, 50);
			if (base.Body.ShootPoint(1000, 560, 53, 1000, 10000, 1, 2f, 2550))
			{
				base.Body.PlayMovie("aim", 1700, 0);
			}
			base.Body.PlayMovie("beatA", 2000, 0);
			IsEixt = 0;
		}
		else
		{
			base.Body.ChangeDirection(-1, 50);
			if (base.Body.ShootPoint(478, 550, 53, 1000, 10000, 1, 2f, 2550))
			{
				base.Body.PlayMovie("aim", 1700, 0);
			}
			base.Body.PlayMovie("beatA", 2000, 0);
			IsEixt = 1;
		}
	}

	public void Summon()
	{
		if (base.Body.State == 1)
		{
			base.Body.PlayMovie("beatC", 2500, 0);
			return;
		}
		base.Body.PlayMovie("beatC", 0, 2000);
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		Player player = base.Game.FindRandomPlayer();
		int num = base.Game.Random.Next(0, KillPlayerChat.Length);
		((SimpleBoss)base.Body).Say(KillPlayerChat[num], 1, 500);
		foreach (Player item in allLivingPlayers)
		{
			item.AddEffect(new ContinueReduceBloodEffect(2, 500, item), 0);
		}
		base.Body.CallFuction(In, 1300);
	}

	public void In()
	{
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		Player player = base.Game.FindRandomPlayer();
		foreach (Player item in allLivingPlayers)
		{
			m_kingMoive = ((PVEGame)base.Game).Createlayer(item.X, item.Y - 100, "moive", "asset.game.4.flame", "out", 1, 0);
		}
		base.Body.CallFuction(Remove, 1000);
	}

	public void Remove()
	{
		if (m_kingMoive != null)
		{
			base.Game.RemovePhysicalObj(m_kingMoive, sendToClient: true);
			m_kingMoive = null;
		}
	}

	public void MovePlayer()
	{
		if (base.Body.State == 1)
		{
			base.Body.PlayMovie("beatC", 2500, 0);
			return;
		}
		base.Body.PlayMovie("beatC", 0, 2000);
		List<Player> allLivingPlayers = base.Game.GetAllLivingPlayers();
		Player player = base.Game.FindRandomPlayer();
		int num = base.Game.Random.Next(0, SealChat.Length);
		((SimpleBoss)base.Body).Say(SealChat[num], 1, 500);
		foreach (Player item in allLivingPlayers)
		{
			int x = base.Game.Random.Next(200, 1550);
			item.JumpToSpeed(x, 400, "", 0, 0, 36, null);
		}
	}

	public void KillAttack(int fx, int mx)
	{
		base.Body.CurrentDamagePlus = 10f;
		int num = base.Game.Random.Next(0, KillAttackChat.Length);
		((SimpleBoss)base.Body).Say(KillAttackChat[num], 1, 500);
		base.Body.PlayMovie("beatC", 2500, 0);
		base.Body.RangeAttacking(fx, mx, "cry", 3300, null);
	}
}
}
