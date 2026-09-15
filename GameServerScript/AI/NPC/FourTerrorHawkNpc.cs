using System.Collections.Generic;
using Game.Logic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class FourTerrorHawkNpc : ABrain
{
	private int m_attackTurn = 0;

	private int Dander = 0;

	private List<SimpleNpc> Children = new List<SimpleNpc>();

	private PhysicalObj m_moive;

	private static string[] AllAttackChat = new string[3] { "你们这是自寻死路！", "你惹毛我了!", "超级无敌大地震……<br/>震……震…… " };

	private static string[] ShootChat = new string[2] { "砸你家玻璃。", "看哥打的可比你们准多了" };

	private static string[] KillPlayerChat = new string[2] { "送你回老家！", "就凭你还妄想能够打败我？" };

	private static string[] CallChat = new string[2] { "卫兵！ <br/>卫兵！！ ", "啵咕们！！<br/>给我些帮助！" };

	private static string[] ShootedChat = new string[2] { "哎呦！很痛…", "我还顶的住…" };

	private static string[] JumpChat = new string[3] { "为了你们的胜利，<br/>向我开炮！", "你再往前半步我就把你给杀了！", "高！<br/>实在是高！" };

	private static string[] KillAttackChat = new string[1] { "超级肉弹！！" };

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		base.Body.CurrentDamagePlus = 1f;
		base.Body.CurrentShootMinus = 1f;
		base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		if (base.Body.Direction == -1)
		{
			base.Body.SetRect(((SimpleBoss)base.Body).NpcInfo.X, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}
		else
		{
			base.Body.SetRect(-((SimpleBoss)base.Body).NpcInfo.X - ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Y, ((SimpleBoss)base.Body).NpcInfo.Width, ((SimpleBoss)base.Body).NpcInfo.Height);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		if (m_attackTurn == 0)
		{
			WalkA();
			Angger();
			m_attackTurn++;
		}
		else if (m_attackTurn == 1)
		{
			WalkA();
			Angger();
			m_attackTurn++;
		}
		else if (m_attackTurn == 2)
		{
			CallBoss();
			m_attackTurn++;
		}
		else if (m_attackTurn == 3)
		{
			CallBoss();
			m_attackTurn++;
		}
		else
		{
			WalkA();
			Angger();
			m_attackTurn = 0;
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	private void WalkA()
	{
		int x = base.Game.Random.Next(200, 1389);
		base.Body.MoveTo(x, base.Body.Y, "fly", 1200, "", 12, AllAttack);
	}

	private void AllAttack()
	{
		base.Body.PlayMovie("beatA", 2000, 2000);
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.CallFuction(CreateFeather, 3300);
	}

	private void AllAttack2()
	{
		base.Body.PlayMovie("beatA", 2000, 2000);
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		base.Body.CallFuction(CreateFeather, 3300);
	}

	private void CallBoss()
	{
		base.Body.Direction = base.Game.FindlivingbyDir(base.Body);
		int num = base.Game.Random.Next(0, AllAttackChat.Length);
		base.Body.Say(AllAttackChat[num], 1, 1000);
		base.Body.PlayMovie("cry", 2000, 0);
	}

	public void CreateFeather()
	{
		base.Body.CurrentDamagePlus = 0.5f;
		base.Body.RangeAttacking(base.Body.X - 1000, base.Body.X + 1000, "cry", 1000, null);
		Player player = base.Game.FindRandomPlayer();
		m_moive = ((PVEGame)base.Game).Createlayer(player.X, player.Y, "moive", "asset.game.4.feather", "out", 1, 0);
		((SimpleBoss)base.Body).SetRelateDemagemRect(-41, -100, 50, 70);
	}

	public void Angger()
	{
		base.Body.State = 1;
		Dander += 100;
		((SimpleBoss)base.Body).SetDander(Dander);
		if (base.Body.Direction == -1)
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(8, -252, 74, 50);
		}
		else
		{
			((SimpleBoss)base.Body).SetRelateDemagemRect(-8, -252, 74, 50);
		}
	}
}
}
