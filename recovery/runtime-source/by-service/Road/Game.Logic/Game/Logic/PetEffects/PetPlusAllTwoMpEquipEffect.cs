using System.Collections.Generic;
using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetPlusAllTwoMpEquipEffect : BasePetEffect
{
	private int m_type;

	private int m_count;

	private int m_probability;

	private int m_delay;

	private int m_currentId;

	public PetPlusAllTwoMpEquipEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.PetPlusAllTwoMpEquipEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.PetPlusAllTwoMpEquipEffect) is PetPlusAllTwoMpEquipEffect petPlusAllTwoMpEquipEffect)
		{
			petPlusAllTwoMpEquipEffect.m_probability = ((m_probability > petPlusAllTwoMpEquipEffect.m_probability) ? m_probability : petPlusAllTwoMpEquipEffect.m_probability);
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
		if (rand.Next(10000) >= m_probability)
		{
			return;
		}
		List<Player> allTeamPlayers = living.Game.GetAllTeamPlayers(living);
		foreach (Player item in allTeamPlayers)
		{
			item.AddPetMP(2);
		}
	}
}
