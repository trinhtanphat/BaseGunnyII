using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetRemovePlusDameEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	public PetRemovePlusDameEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetRemovePlusDameEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetRemovePlusDameEquipEffect) is PetRemovePlusDameEquipEffect petRemovePlusDameEquipEffect)
		{
			petRemovePlusDameEquipEffect.m_probability = ((m_probability > petRemovePlusDameEquipEffect.m_probability) ? m_probability : petRemovePlusDameEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.AfterKilledByLiving += player_AfterKilledByLiving;
		player.BeforeTakeDamage += player_BeforeTakeDamage;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.AfterKilledByLiving -= player_AfterKilledByLiving;
		player.BeforeTakeDamage -= player_BeforeTakeDamage;
	}

	private void player_AfterKilledByLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		ReduceDame(living);
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		ReduceDame(living);
	}

	private void ReduceDame(Living living)
	{
		if (living.PetEffects.AddDameValue > 0)
		{
			(living as Player).BaseDamage -= living.PetEffects.AddDameValue;
			living.PetEffects.AddDameValue = 0;
		}
	}
}
