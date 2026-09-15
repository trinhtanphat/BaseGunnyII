using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetPlusDameEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	private int m_value;

	public PetPlusDameEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetPlusDameEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
		switch (skillId)
		{
		case 69:
			m_value = 15;
			break;
		case 70:
			m_value = 30;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPlusDameEquipEffect) is PetPlusDameEquipEffect petPlusDameEquipEffect)
		{
			petPlusDameEquipEffect.m_probability = ((m_probability > petPlusDameEquipEffect.m_probability) ? m_probability : petPlusDameEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeginSelfTurn += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeginSelfTurn -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living)
	{
		if (rand.Next(10000) < m_probability && living.PetEffects.AddDameValue < m_value * 5)
		{
			(living as Player).BaseDamage += m_value;
			living.PetEffects.AddDameValue += m_value;
		}
	}
}
