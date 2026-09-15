using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class ReduceStrengthEffect : AbstractEffect
{
	private int m_count;

	private int m_reduce;

	public ReduceStrengthEffect(int count, int reduce)
		: base(eEffectType.ReduceStrengthEffect)
	{
		m_count = count;
		m_reduce = reduce;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.ReduceStrengthEffect) is ReduceStrengthEffect reduceStrengthEffect)
		{
			reduceStrengthEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginFitting;
		living.Game.SendPlayerPicture(living, 1, state: true);
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		living.Game.SendPlayerPicture(living, 1, state: false);
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (living is Player)
		{
			(living as Player).Energy -= m_reduce;
		}
		if (m_count < 0)
		{
			Stop();
		}
	}
}
