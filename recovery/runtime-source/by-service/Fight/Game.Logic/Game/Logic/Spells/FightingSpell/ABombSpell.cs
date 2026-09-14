using Game.Logic.Actions;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.FightingSpell;

[SpellAttibute(10)]
public class ABombSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		player.SetBall(4);
		game.AddAction(new FightAchievementAction(player, 2, player.Direction, 1200));
	}
}
