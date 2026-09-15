using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetAddLuckEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	private int m_value;

	public PetAddLuckEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetAddLuckEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
		switch (skillId)
		{
		case 86:
			m_value = 500;
			break;
		case 87:
			m_value = 800;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetAddLuckEquipEffect) is PetAddLuckEquipEffect petAddLuckEquipEffect)
		{
			petAddLuckEquipEffect.m_probability = ((m_probability > petAddLuckEquipEffect.m_probability) ? m_probability : petAddLuckEquipEffect.m_probability);
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
		if (rand.Next(10000) < m_probability && living.PetEffects.PetSkillStase == m_currentId && living.PetEffects.AddLuckValue < m_value)
		{
			(living as Player).Lucky += m_value;
			living.PetEffects.AddLuckValue += m_value;
		}
	}
}
