using System;
using System.Collections.Generic;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class SimpleNpcAi : ABrain
{
	protected Player m_targer;

	private static Random random = new Random();

	private static string[] listChat = new string[13]
	{
		"为了荣誉！为了胜利！！", "握紧手中的武器，不要发抖呀～", "为了国王而战！", "敌人就在眼前，大家做好战斗准备！", "感觉最近国王的行为举止越来越反常......", "为了啵咕的胜利！！兄弟们冲啊！", "快消灭敌人！", "大家一起上,人多力量大！", "大家一起速战速决！", "包围敌人，歼灭他们。",
		"增援！增援！我们需要更多的增援！！", "就算牺牲自己，也不会让你们轻易得逞。", "不要轻视啵咕的力量，否则你会为此付出代价。"
	};

	public override void OnBeginSelfTurn()
	{
		base.OnBeginSelfTurn();
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		if (m_body.IsSay)
		{
			string oneChat = GetOneChat();
			int delay = base.Game.Random.Next(0, 5000);
			m_body.Say(oneChat, 0, delay);
		}
	}

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		Beating();
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void MoveToPlayer(Player player)
	{
		int num = (int)player.Distance(base.Body.X, base.Body.Y);
		int num2 = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
		if (num <= 97)
		{
			return;
		}
		num = ((num <= ((SimpleNpc)base.Body).NpcInfo.MoveMax) ? (num - 90) : num2);
		if (player.Y < 420 && player.X < 210)
		{
			if (base.Body.Y > 420)
			{
				if (base.Body.X - num < 50)
				{
					base.Body.MoveTo(25, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, Jump);
				}
				else
				{
					base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
				}
			}
			else if (player.X > base.Body.X)
			{
				base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
			}
			else
			{
				base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
			}
		}
		else if (base.Body.Y < 420)
		{
			if (base.Body.X + num > 200)
			{
				base.Body.MoveTo(200, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, Fall);
			}
		}
		else if (player.X > base.Body.X)
		{
			base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
		}
		else
		{
			base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, MoveBeat);
		}
	}

	public void MoveBeat()
	{
		base.Body.Beat(m_targer, "beatA", 100, 0, 0, 1, 1);
	}

	public void FallBeat()
	{
		base.Body.Beat(m_targer, "beatA", 100, 0, 2000, 1, 1);
	}

	public void Jump()
	{
		base.Body.Direction = 1;
		base.Body.JumpTo(base.Body.X, base.Body.Y - 240, "Jump", 0, 2, 3, Beating);
	}

	public void Beating()
	{
		if (m_targer != null && !base.Body.Beat(m_targer, "beatA", 100, 0, 0, 1, 1))
		{
			MoveToPlayer(m_targer);
		}
	}

	public void Fall()
	{
		base.Body.FallFrom(base.Body.X, base.Body.Y + 240, null, 0, 0, 12, Beating);
	}

	public static string GetOneChat()
	{
		int num = random.Next(0, listChat.Length);
		return listChat[num];
	}

	public static void LivingSay(List<Living> livings)
	{
		if (livings == null || livings.Count == 0)
		{
			return;
		}
		int num = 0;
		int count = livings.Count;
		foreach (Living living in livings)
		{
			living.IsSay = false;
		}
		num = ((count <= 5) ? random.Next(0, 2) : ((count <= 5 || count > 10) ? random.Next(1, 4) : random.Next(1, 3)));
		if (num <= 0)
		{
			return;
		}
		int[] array = new int[num];
		int num2 = 0;
		while (num2 < num)
		{
			int index = random.Next(0, count);
			if (!livings[index].IsSay)
			{
				livings[index].IsSay = true;
				int delay = random.Next(0, 5000);
				livings[index].Say(GetOneChat(), 0, delay);
				num2++;
			}
		}
	}
}
