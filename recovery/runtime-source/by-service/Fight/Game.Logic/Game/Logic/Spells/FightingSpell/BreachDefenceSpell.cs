using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(8)]
public class BreachDefenceSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		player.IgnoreArmor = true;
		game.AddAction(new FightAchievementAction(player, 6, player.Direction, 1200));
	}
}
