using System.Collections.Generic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Spells.NormalSpell;

[SpellAttibute(1)]
public class AddLifeSpell : ISpellHandler
{
	public void Execute(BaseGame game, Player player, ItemTemplateInfo item)
	{
		switch (item.Property2)
		{
		case 0:
			if (player.IsLiving)
			{
				int num = item.Property3;
				if (player.FightBuffers.ConsortionAddSpellCount > 0)
				{
					num += player.FightBuffers.ConsortionAddSpellCount;
				}
				player.AddBlood(num);
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
						item2.AddBlood(item.Property3);
					}
				}
				break;
			}
		}
		}
	}
}
