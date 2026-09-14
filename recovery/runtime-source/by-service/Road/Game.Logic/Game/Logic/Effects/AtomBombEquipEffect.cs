using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AtomBombEquipEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	public AtomBombEquipEffect(int count, int probability)
		: base(eEffectType.AtomBomb)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AtomBomb) is AtomBombEquipEffect atomBombEquipEffect)
		{
			atomBombEquipEffect.m_probability = ((m_probability > atomBombEquipEffect.m_probability) ? m_probability : atomBombEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += ChangeProperty;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= ChangeProperty;
	}

	private void ChangeProperty(Player player)
	{
		if (!player.CurrentBall.IsSpecial() && rand.Next(100) < m_probability && player.AttackGemLimit == 0)
		{
			player.SetBall(4);
			player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
			player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AtomBombEquipEffect.msg"), 9, 0, 1000));
		}
	}
}
