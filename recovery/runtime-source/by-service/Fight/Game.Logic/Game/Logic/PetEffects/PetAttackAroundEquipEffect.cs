using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetAttackAroundEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	private int m_value;

	public PetAttackAroundEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetPlusAllTwoMpEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
		switch (skillId)
		{
		case 78:
			m_value = 200;
			break;
		case 79:
			m_value = 400;
			break;
		}
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPlusAllTwoMpEquipEffect) is PetAttackAroundEquipEffect petAttackAroundEquipEffect)
		{
			petAttackAroundEquipEffect.m_probability = ((m_probability > petAttackAroundEquipEffect.m_probability) ? m_probability : petAttackAroundEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += player_AfterKilledByLiving;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= player_AfterKilledByLiving;
	}

	private void player_AfterKilledByLiving(Living living)
	{
		if (rand.Next(10000) >= m_probability)
		{
			return;
		}
		List<Living> list = living.Game.Map.FindAllNearestEnemy(living.X, living.Y, 110.0, living);
		foreach (Living item in list)
		{
			item.AddBlood(-m_value, 1);
			if (item.Blood <= 0)
			{
				item.Die();
			}
		}
	}
}
