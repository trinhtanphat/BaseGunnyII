using Bussiness;
using Game.Logic.AI;
using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC
{

public class ThirdSimpleBloomNpc : ABrain
{
	private Player m_target = null;

	private int m_maxBlood;

	private int m_blood;

	private int living;

	private int m_team;

	private int Team;

	private int m_targetDis = 0;

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
		m_target = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		m_maxBlood = 49999;
		m_blood = 40000;
		base.Body.Say(LanguageMgr.GetTranslation("Hồi máu cho tôi, tôi sẻ dẫn các cậu ra khỏi đây !"), 0, 200, 0);
		if (m_blood > m_maxBlood)
		{
			m_blood = m_maxBlood;
			base.Body.PlayMovie("grow", 100, 0);
			base.Body.Die(1000);
		}
		else
		{
			Beat();
		}
	}

	public override void OnStopAttacking()
	{
		base.OnStopAttacking();
	}

	public void MoveToPlayer(Player player)
	{
		int num = base.Game.Random.Next(((SimpleNpc)base.Body).NpcInfo.MoveMin, ((SimpleNpc)base.Body).NpcInfo.MoveMax);
		if (player.X > base.Body.X)
		{
			base.Body.MoveTo(base.Body.X + num, base.Body.Y, "walk", 2000, "", 3, Beat);
		}
		else
		{
			base.Body.MoveTo(base.Body.X - num, base.Body.Y, "walk", 2000, "", 3, Beat);
		}
	}

	public void Beat()
	{
	}
}
}
