using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetNoHoleEffect : AbstractPetEffect
{
	private int m_count;

	public PetNoHoleEffect(int count)
		: base(ePetEffectType.NoHoleEquipEffect)
	{
		m_count = count;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.NoHoleEquipEffect) is PetNoHoleEffect petNoHoleEffect)
		{
			petNoHoleEffect.m_count = m_count;
			return true;
		}
		return base.Start(living);
	}

	public override void OnAttached(Living living)
	{
		living.IsNoHole = true;
		living.BeginSelfTurn += player_BeginFitting;
		living.Game.SendPlayerPicture(living, 5, state: true);
	}

	public override void OnRemoved(Living living)
	{
		living.BeginSelfTurn -= player_BeginFitting;
		living.IsNoHole = false;
		living.Game.SendPlayerPicture(living, 5, state: false);
	}

	private void player_BeginFitting(Living player)
	{
		m_count--;
		if (m_count <= 0)
		{
			Stop();
		}
	}
}
