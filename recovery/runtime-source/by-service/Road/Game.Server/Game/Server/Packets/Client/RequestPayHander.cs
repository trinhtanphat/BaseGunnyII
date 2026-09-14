using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(207, "场景用户离开")]
public class RequestPayHander : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int iD = client.Player.PlayerCharacter.ID;
		return 0;
	}
}
