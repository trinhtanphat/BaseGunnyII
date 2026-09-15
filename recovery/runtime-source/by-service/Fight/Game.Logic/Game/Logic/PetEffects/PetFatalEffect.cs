using Game.Logic.Phy.Object;

namespace Game.Logic.PetEffects;

public class PetFatalEffect : BasePetEffect
{
	private int m_type;

	private int m_delay;

	private int m_count;

	private int m_probability;

	private int m_currentId;

	public PetFatalEffect(int count, int probability, int type, int skillId, int delay)
		: base(ePetEffectType.FatalEffect)
	{
		m_count = count;
		m_probability = ((probability == -1) ? 10000 : probability);
		m_type = type;
		m_delay = delay;
		m_currentId = skillId;
	}

	public override bool Start(Living living)
	{
		if (living.PetEffectList.GetOfType(ePetEffectType.FatalEffect) is PetFatalEffect petFatalEffect)
		{
			petFatalEffect.m_probability = ((m_probability > petFatalEffect.m_probability) ? m_probability : petFatalEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += ChangeProperty;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= ChangeProperty;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterPlayerShooted(Player player)
	{
		IsTrigger = false;
		player.ControlBall = false;
		player.EffectTrigger = false;
	}

	public void ChangeProperty(Living living)
	{
		IsTrigger = false;
		if (rand.Next(10000) < m_probability && living.PetEffects.PetSkillStase == m_currentId)
		{
			IsTrigger = true;
			living.PetEffectTrigger = true;
			living.PetEffects.PetDelay = m_delay;
			if (living.PetEffects.IsPetUseSkill)
			{
				living.PetEffects.IsPetUseSkill = false;
				(living as Player).ControlBall = true;
			}
		}
	}
}
