using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class FatalEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	private int m_saycount;

	public FatalEffect(int count, int probability)
		: base(eEffectType.FatalEffect)
	{
		m_count = count;
		m_probability = probability;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.FatalEffect) is FatalEffect fatalEffect)
		{
			fatalEffect.m_probability = ((m_probability > fatalEffect.m_probability) ? m_probability : fatalEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += ChangeProperty;
		player.TakePlayerDamage += player_BeforeTakeDamage;
		player.BeginNextTurn += player_BeginNextTurn;
		player.AfterPlayerShooted += player_AfterPlayerShooted;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= ChangeProperty;
		player.TakePlayerDamage -= player_BeforeTakeDamage;
		player.BeginNextTurn -= player_BeginNextTurn;
		player.AfterPlayerShooted -= player_AfterPlayerShooted;
	}

	private void player_AfterPlayerShooted(Player player)
	{
		IsTrigger = false;
		player.ControlBall = false;
		player.EffectTrigger = false;
	}

	private void player_BeginNextTurn(Living living)
	{
		m_saycount = 0;
	}

	private void player_BeforeTakeDamage(Living living, Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (IsTrigger && living is Player)
		{
			damageAmount = damageAmount * (100 - m_count) / 100;
		}
	}

	private void ChangeProperty(Player player)
	{
		if (player.CurrentBall.IsSpecial())
		{
			return;
		}
		m_saycount++;
		IsTrigger = false;
		if (rand.Next(100) < m_probability && player.AttackGemLimit == 0)
		{
			player.AttackGemLimit = 4;
			player.ShootMovieDelay = 50;
			IsTrigger = true;
			if (player.CurrentBall.ID != 3)
			{
				player.ControlBall = true;
			}
			if (m_saycount == 1)
			{
				player.EffectTrigger = true;
				player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
				player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("FatalEffect.msg"), 9, 0, 1000));
			}
		}
	}
}
