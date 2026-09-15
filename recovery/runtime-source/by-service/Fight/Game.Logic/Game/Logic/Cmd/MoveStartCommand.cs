using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(9, "开始移动")]
public class MoveStartCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (!player.IsAttacking)
		{
			return;
		}
		byte b = packet.ReadByte();
		int x = packet.ReadInt();
		int num = packet.ReadInt();
		byte dir = packet.ReadByte();
		bool flag = packet.ReadBoolean();
		short num2 = packet.ReadShort();
		int turnIndex = game.TurnIndex;
		game.SendPlayerMove(player, b, x, num, dir);
		switch (b)
		{
		case 0:
		case 1:
			player.SetXY(x, num);
			player.StartMoving();
			if (player.Y - num > 1 || player.IsLiving != flag)
			{
				game.SendPlayerMove(player, 3, player.X, player.Y, dir, flag);
			}
			break;
		}
	}
}
