using Bussiness;
using Game.Logic.Actions;
using Game.Logic.Phy.Object;

namespace Game.Logic.Effects;

public class AddTurnEquipEffect : BasePlayerEffect
{
	private int m_count;

	private int m_probability;

	private int m_templateID;

	public AddTurnEquipEffect(int count, int probability, int templateID)
		: base(eEffectType.AddTurnEquipEffect)
	{
		m_count = count;
		m_probability = probability;
		m_templateID = templateID;
	}

	public override bool Start(Living living)
	{
		if (living.EffectList.GetOfType(eEffectType.AddTurnEquipEffect) is AddTurnEquipEffect addTurnEquipEffect)
		{
			addTurnEquipEffect.m_probability = ((m_probability > addTurnEquipEffect.m_probability) ? m_probability : addTurnEquipEffect.m_probability);
			return true;
		}
		return base.Start(living);
	}

	protected override void OnAttachedToPlayer(Player player)
	{
		player.PlayerShoot += ChangeProperty;
		player.BeginNextTurn += player_BeginSelfTurn;
	}

	protected override void OnRemovedFromPlayer(Player player)
	{
		player.PlayerShoot -= ChangeProperty;
		player.BeginNextTurn -= player_BeginSelfTurn;
	}

	public void player_BeginSelfTurn(Living living)
	{
		if (IsTrigger && living is Player)
		{
			int energy = 0;
			switch (m_templateID)
			{
			case 311129:
				energy = 145;
				break;
			case 311112:
				energy = 130;
				break;
			case 311312:
				energy = 190;
				break;
			case 311229:
				energy = 175;
				break;
			case 311212:
				energy = 160;
				break;
			case 311412:
				energy = 220;
				break;
			case 311329:
				energy = 205;
				break;
			case 311529:
				energy = 265;
				break;
			case 311512:
				energy = 260;
				break;
			case 311429:
				energy = 245;
				break;
			}
			(living as Player).Delay += (living as Player).Delay * m_count / 100;
			(living as Player).Energy = energy;
			IsTrigger = false;
		}
	}

	private void ChangeProperty(Player player)
	{
		if (!player.CurrentBall.IsSpecial() && rand.Next(100) < m_probability && player.AttackGemLimit == 0)
		{
			player.AttackGemLimit = 4;
			player.Delay = player.DefaultDelay;
			IsTrigger = true;
			player.EffectTrigger = true;
			player.Game.SendEquipEffect(player, LanguageMgr.GetTranslation("AttackEffect.Success"));
			player.Game.AddAction(new LivingSayAction(player, LanguageMgr.GetTranslation("AddTurnEquipEffect.msg"), 9, 0, 1000));
		}
	}
}
