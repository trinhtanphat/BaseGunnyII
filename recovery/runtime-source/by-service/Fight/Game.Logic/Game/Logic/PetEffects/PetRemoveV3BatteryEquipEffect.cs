using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetRemoveV3BatteryEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	public PetRemoveV3BatteryEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetRemoveV3BatteryEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetRemoveV3BatteryEquipEffect) is PetRemoveV3BatteryEquipEffect petRemoveV3BatteryEquipEffect)
		{
			petRemoveV3BatteryEquipEffect.m_probability = ((m_probability > petRemoveV3BatteryEquipEffect.m_probability) ? m_probability : petRemoveV3BatteryEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBeginMoving += player_WhenMoving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBeginMoving += player_WhenMoving;
	}

	private void player_WhenMoving(Living living)
	{
		if (living.PetEffects.AddAttackValue > 0 && living.PetEffects.AddLuckValue > 0 && living.PetEffects.ReduceDefendValue > 0)
		{
			living.IsNoHole = false;
			living.Game.SendPetBuff(living, 1083, isActive: false);
			(living as Player).Attack -= living.PetEffects.AddAttackValue;
			(living as Player).Lucky -= living.PetEffects.AddLuckValue;
			(living as Player).Defence += living.PetEffects.ReduceDefendValue;
			living.PetEffects.AddAttackValue = 0;
			living.PetEffects.AddLuckValue = 0;
			living.PetEffects.ReduceDefendValue = 0;
		}
	}
}
