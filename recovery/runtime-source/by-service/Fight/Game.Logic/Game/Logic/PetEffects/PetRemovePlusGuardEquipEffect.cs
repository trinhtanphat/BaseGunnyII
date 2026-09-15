using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetRemovePlusGuardEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	public PetRemovePlusGuardEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetRemovePlusGuardEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetRemovePlusGuardEquipEffect) is PetRemovePlusGuardEquipEffect petRemovePlusGuardEquipEffect)
		{
			petRemovePlusGuardEquipEffect.m_probability = ((m_probability > petRemovePlusGuardEquipEffect.m_probability) ? m_probability : petRemovePlusGuardEquipEffect.m_probability);
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
		player.PlayerBeginMoving -= player_WhenMoving;
	}

	private void player_WhenMoving(Player player)
	{
		if (player.PetEffects.AddGuardValue > 0)
		{
			player.BaseGuard -= player.PetEffects.AddGuardValue;
			player.PetEffects.AddGuardValue = 0;
		}
	}
}
