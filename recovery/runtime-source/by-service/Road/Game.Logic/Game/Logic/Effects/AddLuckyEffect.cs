using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddLuckyEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	private int m_added;

	public AddLuckyEffect(int count, int probability)
		: base(eEffectType.AddLuckyEffect)
	{
		m_count = count;
		m_probability = probability;
		m_added = 0;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddLuckyEffect) is AddLuckyEffect addLuckyEffect)
		{
			addLuckyEffect.m_probability = ((m_probability > addLuckyEffect.m_probability) ? m_probability : addLuckyEffect.m_probability);
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
			player.Lucky -= m_added;
			m_added = 0;
			IsTrigger = false;
			if (rand.Next(100) < m_probability && player.AttackGemLimit == 0)
			{
				player.AttackGemLimit = 4;
				player.FlyingPartical = 65;
				IsTrigger = true;
				player.Lucky += m_count;
				player.EffectTrigger = true;
				m_added = m_count;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddLuckyEffect.msg"), 9, 0, 1000));
			}
		}
	}
}
