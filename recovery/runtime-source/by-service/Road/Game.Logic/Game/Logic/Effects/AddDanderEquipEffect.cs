using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddDanderEquipEffect : BasePlayerEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	public AddDanderEquipEffect(int count, int probability, int type)
		: base(eEffectType.AddDander)
	{
		m_count = count;
		m_probability = probability;
		m_type = type;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddDander) is AddDanderEquipEffect addDanderEquipEffect)
		{
			m_probability = ((m_probability > addDanderEquipEffect.m_probability) ? m_probability : addDanderEquipEffect.m_probability);
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

	private void ChangeProperty(Living living)
	{
		IsTrigger = false;
		if (rand.Next(100) < m_probability && living.DefendActiveGem == m_type)
		{
			IsTrigger = true;
			if (living is Player)
			{
				(living as Player).AddDander(m_count);
			}
			living.EffectTrigger = true;
			living.Game.SendEquipEffect(living, LanguageMgr.GetTranslation("DefenceEffect.Success"));
			living.Game.AddAction(new LivingSayAction(living, LanguageMgr.GetTranslation("AddDanderEquipEffect.msg"), 9, 0, 1000));
		}
	}
}
