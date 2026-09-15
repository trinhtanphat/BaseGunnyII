using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(405, "场景用户离开")]
public class WonderfulActivityInitHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int val = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(405, client.Player.PlayerCharacter.ID);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteString("Event ủn ỉn 1");
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteInt(13);
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteString("Event ủn ỉn 2");
		gSPacketIn.WriteInt(33);
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}
}
