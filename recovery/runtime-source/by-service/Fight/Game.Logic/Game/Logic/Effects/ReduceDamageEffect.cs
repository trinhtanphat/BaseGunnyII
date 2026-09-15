using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class ReduceDamageEffect : BasePlayerEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	public ReduceDamageEffect(int count, int probability, int type)
		: base(eEffectType.ReduceDamageEffect)
	{
		m_count = count;
		m_probability = probability;
		m_type = type;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.ReduceDamageEffect) is ReduceDamageEffect reduceDamageEffect)
		{
			reduceDamageEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		IsTrigger = false;
		if (rand.Next(100) < m_probability && living.DefendActiveGem == m_type)
		{
			IsTrigger = true;
			damageAmount -= m_count;
			if ((damageAmount -= m_count) <= 0)
			{
				damageAmount = 1;
			}
			living.EffectTrigger = true;
			living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success"));
			living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("ReduceDamageEffect.msg"), 9, 0, 1000));
		}
	}
}
