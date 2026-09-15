using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public abstract class AbstractPetEffect
{
	private ePetEffectType m_type;

	protected Living m_living;

	protected Random rand;

	public bool IsTrigger;

	public ePetEffectType Type => m_type;

	public int TypeValue => (int)m_type;

	public AbstractPetEffect(ePetEffectType type)
	{
		rand = new Random();
		m_type = type;
	}

	public virtual bool Start(Living living)
	{
		m_living = living;
		return m_living.PetEffectList.Add(this);
	}

	public virtual bool Stop()
	{
		return m_living != null && m_living.PetEffectList.Remove(this);
	}

	public virtual void OnAttached(Living living)
	{
	}

	public virtual void OnRemoved(Living living)
	{
	}
}
