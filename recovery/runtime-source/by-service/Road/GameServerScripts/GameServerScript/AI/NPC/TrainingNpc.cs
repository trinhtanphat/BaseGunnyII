using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class TrainingNpc : ABrain
{
	private static int direction = 1;

	private int dis = 0;

	private int mtX = 0;

	public override void OnCreated()
	{
		base.OnCreated();
	}

	public override void OnStartAttacking()
	{
		base.OnStartAttacking();
		dis = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
		if (direction == 1)
		{
			mtX = base.Body.X + dis;
			if (mtX > 800)
			{
				base.Body.MoveTo(800, base.Body.Y, "walk", 100, "", 3);
				direction = -direction;
			}
			else
			{
				base.Body.MoveTo(mtX, base.Body.Y, "walk", 100, "", 3);
			}
		}
		else
		{
			mtX = base.Body.X - dis;
			if (mtX < 100)
			{
				base.Body.MoveTo(100, base.Body.Y, "walk", 100, "", 3);
				direction = -direction;
			}
			else
			{
				base.Body.MoveTo(mtX, base.Body.Y, "walk", 100, "", 3);
			}
		}
	}

	public override void OnBeginNewTurn()
	{
		base.OnBeginNewTurn();
	}

	public void NextMove()
	{
		direction = -direction;
		if (direction == 1)
		{
			mtX = base.Body.X + dis;
			base.Body.MoveTo(mtX, base.Body.Y, "walk", 100, "", 3);
		}
		else
		{
			mtX = base.Body.X - dis;
			base.Body.MoveTo(mtX, base.Body.Y, "walk", 100, "", 3);
		}
	}
}
