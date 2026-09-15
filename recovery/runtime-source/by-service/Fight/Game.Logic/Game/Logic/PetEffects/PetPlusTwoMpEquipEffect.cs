using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetPlusTwoMpEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	public PetPlusTwoMpEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetPlusTwoMpEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPlusTwoMpEquipEffect) is PetPlusTwoMpEquipEffect petPlusTwoMpEquipEffect)
		{
			petPlusTwoMpEquipEffect.m_probability = ((m_probability > petPlusTwoMpEquipEffect.m_probability) ? m_probability : petPlusTwoMpEquipEffect.m_probability);
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
		if (rand.Next(10000) < m_probability)
		{
			(living as Player).AddPetMP(2);
		}
	}
}
