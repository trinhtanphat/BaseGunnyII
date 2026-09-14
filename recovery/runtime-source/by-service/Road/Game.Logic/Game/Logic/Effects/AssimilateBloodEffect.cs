using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AssimilateBloodEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	public AssimilateBloodEffect(int count, int probability)
		: base(eEffectType.AssimilateBloodEffect)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AssimilateBloodEffect) is AssimilateBloodEffect assimilateBloodEffect)
		{
			assimilateBloodEffect.m_probability = ((m_probability > assimilateBloodEffect.m_probability) ? m_probability : assimilateBloodEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.TakePlayerDamage += player_BeforeTakeDamage;
		player.PlayerShoot += player_PlayerShoot;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.TakePlayerDamage -= player_BeforeTakeDamage;
		player.PlayerShoot -= player_PlayerShoot;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (living.IsLiving && IsTrigger)
		{
			living.SyncAtTime = true;
			living.AddBlood(damageAmount * m_count / 100);
			living.SyncAtTime = false;
		}
	}

	private void player_PlayerShoot(Player player)
	{
		if (!player.CurrentBall.IsSpecial())
		{
			IsTrigger = false;
			if (rand.Next(100) < m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				IsTrigger = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AssimilateBloodEffect.msg"), 9, 0, 1000));
			}
		}
	}
}
