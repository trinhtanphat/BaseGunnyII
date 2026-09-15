using System.Collections.Generic;
using Game.Logic.Effects;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell;

[SpellAttibute(3)]
public class HideSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		switch (item.Property2)
		{
		case 0:
			if (player.IsLiving)
			{
				new HideEffect(item.Property3).Start(player);
			}
			break;
		case 1:
		{
			List<Player> allFightPlayers = player.Game.GetAllFightPlayers();
			{
				foreach (Player item2 in allFightPlayers)
				{
					if (item2.IsLiving && item2.Team == player.Team)
					{
						new HideEffect(item.Property3).Start(item2);
					}
				}
				break;
			}
		}
		}
	}
}
