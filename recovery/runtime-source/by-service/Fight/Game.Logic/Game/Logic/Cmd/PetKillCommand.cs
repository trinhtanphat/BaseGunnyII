using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic.Cmd;

[GameCommand(144, "使用道具")]
public class PetKillCommand : ICommandHandler
{
	public void HandleCommand(BaseGame game, Player player, GSPacketIn packet)
	{
		if (game.GameState == eGameState.Playing && !player.GetSealState())
		{
			int skillID = packet.ReadInt();
			player.PetUseKill(skillID);
		}
	}
}
