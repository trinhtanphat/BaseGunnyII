using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetAddDefendEffect : AbstractPetEffect
{
	private int m_count;

	private int m_added;

	public PetAddDefendEffect(int count, int skilId)
		: base(ePetEffectType.AddDefenceEffect)
	{
		m_count = count;
		switch (skilId)
		{
		case 37:
			m_added = 300;
			break;
		case 38:
			m_added = 400;
			break;
		case 39:
			m_added = 500;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.AddDefenceEffect) is PetAddDefendEffect petAddDefendEffect)
		{
			petAddDefendEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.Defence += m_added;
		living.BeginSelfTurn += player_BeginFitting;
	}

	public override void OnRemoved(Living living)
	{
		living.Defence -= m_added;
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
