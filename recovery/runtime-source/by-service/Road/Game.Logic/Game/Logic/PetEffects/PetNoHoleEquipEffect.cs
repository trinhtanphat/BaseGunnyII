using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetNoHoleEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	public PetNoHoleEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.NoHoleEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.NoHoleEquipEffect) is PetNoHoleEquipEffect petNoHoleEquipEffect)
		{
			petNoHoleEquipEffect.m_probability = ((m_probability > petNoHoleEquipEffect.m_probability) ? m_probability : petNoHoleEquipEffect.m_probability);
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
		if (rand.Next(10000) < m_probability && living.PetEffects.PetSkillStase == m_currentId)
		{
			living.PetEffectTrigger = true;
			new PetNoHoleEffect(m_count).Start(living);
		}
	}
}
