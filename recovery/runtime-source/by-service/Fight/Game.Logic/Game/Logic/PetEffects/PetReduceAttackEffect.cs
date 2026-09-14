using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetReduceAttackEffect : AbstractPetEffect
{
	private int m_count;

	private int m_added = 100;

	public PetReduceAttackEffect(int count)
		: base(ePetEffectType.PetReduceAttackEffect)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceAttackEffect) is PetReduceAttackEffect petReduceAttackEffect)
		{
			petReduceAttackEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.Attack -= m_added;
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.Attack += m_added;
		living.BeginSelfTurn -= player_BeginFitting;
	}

	private void player_BeginFitting(Living player)
	{
		m_count--;
		if (m_count <= 0)
		{
			Stop();
		}
	}
}
