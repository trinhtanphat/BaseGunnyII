using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic.Cmd;

[GameCommand(32, "使用道具")]
public class PropUseCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (game.GameState != eGameState.Playing || player.GetSealState())
		{
			return;
		}
		int bag = packet.ReadByte();
		int place = packet.ReadInt();
		int num = packet.ReadInt();
		ItemTemplateInfo item = ItemMgr.FindItemTemplate(num);
		if (!player.CanUseItem(item))
		{
			return;
		}
		if (player.PlayerDetail.UsePropItem(game, bag, place, num, player.IsLiving))
		{
			if (!player.UseItem(item))
			{
				BaseGame.log.Error("Using prop error");
			}
			return;
		}
		player.UseItem(item);
		switch (num)
		{
		case 10004:
			if (player.Prop < num * 2)
			{
				player.Prop += num;
			}
			break;
		case 10001:
			if (player.Prop < num * 2)
			{
				player.Prop += num;
			}
			break;
		}
	}
}
