using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AssimilateDamageEffect : BasePlayerEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	public AssimilateDamageEffect(int count, int probability, int type)
		: base(eEffectType.AssimilateDamageEffect)
	{
		m_count = count;
		m_probability = probability;
		m_type = type;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AssimilateDamageEffect) is AssimilateDamageEffect assimilateDamageEffect)
		{
			assimilateDamageEffect.m_probability = ((m_probability > assimilateDamageEffect.m_probability) ? m_probability : assimilateDamageEffect.m_probability);
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
			living.EffectTrigger = true;
			living.SyncAtTime = true;
			if (damageAmount > m_count)
			{
				living.AddBlood(m_count);
			}
			else
			{
				living.AddBlood(damageAmount);
			}
			living.SyncAtTime = false;
			damageAmount -= damageAmount;
			criticalAmount -= criticalAmount;
			living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AssimilateDamageEffect.msg"), 9, 0, 1000));
			living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success"));
		}
	}
}
