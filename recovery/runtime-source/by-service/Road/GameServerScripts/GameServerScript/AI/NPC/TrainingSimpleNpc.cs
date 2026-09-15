using Game.Logic.Phy.Object;

namespace GameServerScript.AI.NPC;

public class TrainingSimpleNpc : SimpleNpcAi
{
	public override void OnStartAttacking()
	{
		m_body.CurrentDamagePlus = 1f;
		m_body.CurrentShootMinus = 1f;
		m_targer = base.Game.FindNearestPlayer(base.Body.X, base.Body.Y);
		if (m_targer != null)
		{
			if (m_targer.Blood > 200)
			{
				Beating();
			}
			else
			{
				Beat();
			}
		}
	}

	public void BeatCallBack()
	{
		int blood = m_targer.Blood;
		int demageAmount = blood / 10;
		base.Body.Beat(m_targer, "beat", demageAmount, 0, 0, 1, 1);
	}

	private void Beat()
	{
		int blood = m_targer.Blood;
		int demageAmount = blood / 10;
		if (m_targer != null && !base.Body.Beat(m_targer, "beat", demageAmount, 0, 0, 1, 1))
		{
			int num = base.Game.Random.Next(80, 150);
			if (base.Body.X - m_targer.X > num)
			{
				base.Body.MoveTo(base.Body.X - num, m_targer.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, BeatCallBack);
			}
			else
			{
				base.Body.MoveTo(base.Body.X + num, m_targer.Y, "walk", 1200, "", ((SimpleNpc)base.Body).NpcInfo.speed, BeatCallBack);
			}
		}
	}
}
