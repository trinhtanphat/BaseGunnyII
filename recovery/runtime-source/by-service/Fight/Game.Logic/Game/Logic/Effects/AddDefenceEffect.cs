using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddDefenceEffect : BasePlayerEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_added;

	public AddDefenceEffect(int count, int probability, int type)
		: base(eEffectType.AddDefenceEffect)
	{
		m_count = count;
		m_probability = probability;
		m_added = 0;
		m_type = type;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddDefenceEffect) is AddDefenceEffect addDefenceEffect)
		{
			addDefenceEffect.m_probability = ((m_probability > addDefenceEffect.m_probability) ? m_probability : addDefenceEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginAttacked += ChangeProperty;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginAttacked -= ChangeProperty;
	}

	public void ChangeProperty(Living living)
	{
		living.Defence -= m_added;
		m_added = 0;
		IsTrigger = false;
		if (rand.Next(100) < m_probability && living.DefendActiveGem == m_type)
		{
			IsTrigger = true;
			living.Defence += m_count;
			m_added = m_count;
			living.EffectTrigger = true;
			living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success"));
			living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AddDefenceEffect.msg"), 9, 1000, 1000));
		}
	}
}
