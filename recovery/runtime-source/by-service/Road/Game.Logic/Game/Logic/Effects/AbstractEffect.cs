using System;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public abstract class AbstractEffect
{
	private eEffectType m_type;

	protected Living m_living;

	protected Random rand;

	public bool IsTrigger;

	public eEffectType Type => m_type;

	public int TypeValue => (int)m_type;

	public AbstractEffect(eEffectType type)
	{
		rand = new Random();
		m_type = type;
	}

	public virtual bool Start(Living living)
	{
		m_living = living;
		return m_living.EffectList.Add(this);
	}

	public virtual bool Stop()
	{
		return m_living != null && m_living.EffectList.Remove(this);
	}

	public virtual void OnAttached(Living living)
	{
	}

	public virtual void OnRemoved(Living living)
	{
	}
}
