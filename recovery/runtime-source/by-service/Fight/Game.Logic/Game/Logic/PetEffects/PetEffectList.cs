using System;
using System.Collections;
using System.Reflection;
using Game.Logic.Phy.Object;
using log4net;

namespace Game.Logic.PetEffects;

public class PetEffectList
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected ArrayList m_effects;

	protected readonly Living m_owner;

	protected volatile sbyte m_changesCount;

	protected int m_immunity;

	public ArrayList List => m_effects;

	public PetEffectList(Living owner, int immunity)
	{
		m_owner = owner;
		m_effects = new ArrayList(5);
		m_immunity = immunity;
	}

	public bool CanAddEffect(int id)
	{
		return id > 350 || id < 0 || ((1 << id - 1) & m_immunity) == 0;
	}

	public virtual bool Add(AbstractPetEffect effect)
	{
		if (CanAddEffect(effect.TypeValue))
		{
			lock (m_effects)
			{
				m_effects.Add(effect);
			}
			effect.OnAttached(m_owner);
			OnEffectsChanged(effect);
			return true;
		}
		return false;
	}

	public virtual bool Remove(AbstractPetEffect effect)
	{
		int num = -1;
		lock (m_effects)
		{
			num = m_effects.IndexOf(effect);
			if (num < 0)
			{
				return false;
			}
			m_effects.RemoveAt(num);
		}
		if (num != -1)
		{
			effect.OnRemoved(m_owner);
			OnEffectsChanged(effect);
			return true;
		}
		return false;
	}

	public virtual void OnEffectsChanged(AbstractPetEffect changedEffect)
	{
		if (m_changesCount <= 0)
		{
			UpdateChangedEffects();
		}
	}

	public void BeginChanges()
	{
		m_changesCount++;
	}

	public virtual void CommitChanges()
	{
		if (--m_changesCount < 0)
		{
			if (log.IsWarnEnabled)
			{
				log.Warn("changes count is less than zero, forgot BeginChanges()?\n" + Environment.StackTrace);
			}
			m_changesCount = 0;
		}
		if (m_changesCount == 0)
		{
			UpdateChangedEffects();
		}
	}

	protected virtual void UpdateChangedEffects()
	{
	}

	public virtual AbstractPetEffect GetOfType(ePetEffectType effectType)
	{
		lock (m_effects)
		{
			foreach (AbstractPetEffect effect in m_effects)
			{
				if (effect.Type == effectType)
				{
					return effect;
				}
			}
		}
		return null;
	}

	public virtual IList GetAllOfType(Type effectType)
	{
		ArrayList arrayList = new ArrayList();
		lock (m_effects)
		{
			foreach (AbstractPetEffect effect in m_effects)
			{
				if (effect.GetType().Equals(effectType))
				{
					arrayList.Add(effect);
				}
			}
		}
		return arrayList;
	}

	public void StopEffect(Type effectType)
	{
		IList allOfType = GetAllOfType(effectType);
		BeginChanges();
		foreach (AbstractPetEffect item in allOfType)
		{
			item.Stop();
		}
		CommitChanges();
	}

	public void StopAllEffect()
	{
		if (m_effects.Count > 0)
		{
			AbstractPetEffect[] array = new AbstractPetEffect[m_effects.Count];
			m_effects.CopyTo(array);
			AbstractPetEffect[] array2 = array;
			foreach (AbstractPetEffect abstractPetEffect in array2)
			{
				abstractPetEffect.Stop();
			}
			m_effects.Clear();
		}
	}
}
