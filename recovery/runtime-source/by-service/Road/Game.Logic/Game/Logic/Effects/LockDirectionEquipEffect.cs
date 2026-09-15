using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class LockDirectionEquipEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	public LockDirectionEquipEffect(int count, int probability)
		: base(eEffectType.LockDirectionEquipEffect)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.LockDirectionEquipEffect) is LockDirectionEquipEffect lockDirectionEquipEffect)
		{
			lockDirectionEquipEffect.m_probability = ((m_probability > lockDirectionEquipEffect.m_probability) ? m_probability : lockDirectionEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += ChangeProperty;
		player.AfterKillingLiving += player_AfterKillingLiving;
	}

	private void player_AfterKillingLiving(Living living, Living target, int damageAmount, int criticalAmount)
	{
		if (IsTrigger)
		{
			target.AddEffect(new LockDirectionEffect(2), 0);
		}
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= ChangeProperty;
		player.AfterKillingLiving -= player_AfterKillingLiving;
	}

	private void ChangeProperty(Player player)
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
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("LockDirectionEquipEffect.msg"), 9, 0, 1000));
			}
		}
	}
}
