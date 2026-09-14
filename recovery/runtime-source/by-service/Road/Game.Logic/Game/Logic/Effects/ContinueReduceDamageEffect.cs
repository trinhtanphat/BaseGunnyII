using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class ContinueReduceDamageEffect : AbstractEffect
{
	private int m_count;

	public ContinueReduceDamageEffect(int count)
		: base(eEffectType.ContinueReduceDamageEffect)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.ContinueReduceDamageEffect) is ContinueReduceDamageEffect continueReduceDamageEffect)
		{
			continueReduceDamageEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.BeginSelfTurn += player_BeginFitting;
		if (living is Player)
		{
			(living as Player).BaseDamage = (living as Player).BaseDamage * 5.0 / 100.0;
		}
		living.Game.SendPlayerPicture(living, 4, state: true);
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		(living as Player).BaseDamage = (living as Player).BaseDamage * 100.0 / 5.0;
		living.Game.SendPlayerPicture(living, 4, state: false);
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count < 0)
		{
			Stop();
		}
	}
}
