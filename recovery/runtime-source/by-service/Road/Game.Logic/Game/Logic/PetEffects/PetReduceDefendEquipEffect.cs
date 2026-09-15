using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetReduceDefendEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	private int m_value;

	public PetReduceDefendEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetReduceDefendEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
		switch (skillId)
		{
		case 86:
			m_value = 1000;
			break;
		case 87:
			m_value = 800;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetReduceDefendEquipEffect) is PetReduceDefendEquipEffect petReduceDefendEquipEffect)
		{
			petReduceDefendEquipEffect.m_probability = ((m_probability > petReduceDefendEquipEffect.m_probability) ? m_probability : petReduceDefendEquipEffect.m_probability);
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
		if (rand.Next(10000) < m_probability && living.PetEffects.PetSkillStase == m_currentId && living.PetEffects.ReduceDefendValue < m_value)
		{
			double defence = (living as Player).Defence;
			if ((living as Player).Defence < (double)m_value)
			{
				m_value -= m_value - (int)defence;
			}
			(living as Player).Defence -= m_value;
			living.PetEffects.ReduceDefendValue += m_value;
		}
	}
}
