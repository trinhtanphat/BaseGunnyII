using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class ArmorPiercerEquipEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	public ArmorPiercerEquipEffect(int count, int probability)
		: base(eEffectType.ArmorPiercer)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.ArmorPiercer) is ArmorPiercerEquipEffect armorPiercerEquipEffect)
		{
			armorPiercerEquipEffect.m_probability = ((m_probability > armorPiercerEquipEffect.m_probability) ? m_probability : armorPiercerEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.BeforePlayerShoot += ChangeProperty;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.BeforePlayerShoot -= ChangeProperty;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterPlayerShooted(Player player)
	{
		player.FlyingPartical = 0;
	}

	private void ChangeProperty(Player player)
	{
		if (!player.CurrentBall.IsSpecial())
		{
			player.IgnoreArmor = false;
			if (rand.Next(100) < m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.FlyingPartical = 65;
				player.IgnoreArmor = true;
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("ArmorPiercerEquipEffect.msg"), 9, 0, 1000));
			}
		}
	}
}
