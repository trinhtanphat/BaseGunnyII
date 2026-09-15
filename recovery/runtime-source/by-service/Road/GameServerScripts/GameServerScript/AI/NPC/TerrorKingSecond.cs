using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class TerrorKingSecond : ABrain
{
	private int m_attackTurn = 0;

	private int m_turn = 0;

	private PhysicalObj m_wallLeft = null;

	private PhysicalObj m_wallRight = null;

	private int IsEixt = 0;

	private int npcID = 1310;

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
		else if (m_attackTurn == 0)
		{
			AllAttack();
			if (IsEixt == 1)
			{
				m_wallLeft.CanPenetrate = true;
				m_wallRight.CanPenetrate = true;
				base.Game.RemovePhysicalObj(m_wallLeft, sendToClient: true);
				base.Game.RemovePhysicalObj(m_wallRight, sendToClient: true);
				IsEixt = 0;
			}
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			FrostAttack();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			ProtectingWall();
			m_attackTurn++;
		}
		else
		{
			CriticalStrikes();
			m_attackTurn = 0;
		}
	}

	private void CriticalStrikes()
	{
		Player frostPlayerRadom = base.Game.GetFrostPlayerRadom();
		List<Player> allFightPlayers = base.Game.GetAllFightPlayers();
		List<Player> list = new List<Player>();
		foreach (Player item in allFightPlayers)
		{
			if (!item.IsFrost)
			{
				list.Add(item);
			}
		}
		((SimpleBoss)base.Body).CurrentDamagePlus = 30f;
		if (list.Count != allFightPlayers.Count)
		{
			if (list.Count != 0)
			{
				base.Body.PlayMovie("beat1", 0, 0);
				base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "beat1", 1500, list);
			}
			else
			{
				base.Body.PlayMovie("beat1", 0, 0);
				base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "beat1", 1500, null);
			}
		}
		else
		{
			base.Body.Say("小的们给我上，好好教训敌人！", 1, 3300);
			base.Body.PlayMovie("renew", 3500, 0);
			base.Body.CallFuction(CreateChild, 6000);
		}
	}

	private void FrostAttack()
	{
		int x = base.Game.Random.Next(840, 900);
		base.Body.MoveTo(x, base.Body.Y, "walk", 0, "", ((SimpleBoss)base.Body).NpcInfo.speed, NextAttack);
	}

	private void AllAttack()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		if (m_turn == 0)
		{
			int num = base.Game.Random.Next(0, AllAttackChat.Length);
			base.Body.Say(AllAttackChat[num], 1, 13000);
			base.Body.PlayMovie("beat1", 15000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 17000, null);
			m_turn++;
		}
		else
		{
			int num = base.Game.Random.Next(0, AllAttackChat.Length);
			base.Body.Say(AllAttackChat[num], 1, 0);
			base.Body.PlayMovie("beat1", 1000, 0);
			base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 3000, null);
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

	private void ProtectingWall()
	{
		if (IsEixt == 0)
		{
			m_wallLeft = ((PVEGame)base.Game).CreatePhysicalObj(base.Body.X - 65, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_left", "1", 1, 0);
			m_wallRight = ((PVEGame)base.Game).CreatePhysicalObj(base.Body.X + 65, 620, "wallLeft", "com.mapobject.asset.WaveAsset_01_right", "1", 1, 0);
			m_wallLeft.SetRect(-165, -169, 43, 330);
			m_wallRight.SetRect(128, -165, 41, 330);
			IsEixt = 1;
		}
		int num = base.Game.Random.Next(0, WallChat.Length);
		base.Body.Say(WallChat[num], 1, 0);
	}

	public void CreateChild()
	{
		base.Body.PlayMovie("renew", 100, 2000);
		((SimpleBoss)base.Body).CreateChild(npcID, 520, 530, 400, 6, 1);
	}

	private void NextAttack()
	{
		int num = base.Game.Random.Next(1, 2);
		for (int i = 0; i < num; i++)
		{
			Player player = base.Game.FindRandomPlayer();
			int num2 = base.Game.Random.Next(0, ShootChat.Length);
			base.Body.Say(ShootChat[num2], 1, 0);
			if (player.X > base.Body.X)
			{
				base.Body.ChangeDirection(1, 500);
			}
			else
			{
				base.Body.ChangeDirection(-1, 500);
			}
			if (player != null && !player.IsFrost && base.Body.ShootPoint(player.X, player.Y, ((SimpleBoss)base.Body).NpcInfo.CurrentBallId, 1000, 10000, 1, 1.5f, 2000))
			{
				base.Body.PlayMovie("beat2", 1500, 0);
			}
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}
}
