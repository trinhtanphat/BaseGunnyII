using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetStopMovingEffect : AbstractPetEffect
{
	private int m_count;

	private int m_added;

	public PetStopMovingEffect(int count)
		: base(ePetEffectType.PetStopMovingEffect)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetStopMovingEffect) is PetStopMovingEffect petStopMovingEffect)
		{
			petStopMovingEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.SpeedMultY(1);
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.SpeedMultY(0);
		living.BeginSelfTurn -= player_BeginFitting;
	}

	private void player_BeginFitting(Living living)
	{
		m_count--;
		if (m_count <= 0)
		{
			Stop();
		}
	}
}
