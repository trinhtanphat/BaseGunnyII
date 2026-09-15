using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetPlusGuardEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	private int m_value;

	public PetPlusGuardEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetPlusGuardEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
		switch (skillId)
		{
		case 56:
			m_value = 200;
			break;
		case 57:
			m_value = 500;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPlusGuardEquipEffect) is PetPlusGuardEquipEffect petPlusGuardEquipEffect)
		{
			petPlusGuardEquipEffect.m_probability = ((m_probability > petPlusGuardEquipEffect.m_probability) ? m_probability : petPlusGuardEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerBuffSkillPet += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerBuffSkillPet -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living)
	{
		if (rand.Next(10000) < m_probability && living.PetEffects.AddGuardValue < m_value)
		{
			(living as Player).BaseGuard += m_value;
			living.PetEffects.AddGuardValue += m_value;
		}
	}
}
