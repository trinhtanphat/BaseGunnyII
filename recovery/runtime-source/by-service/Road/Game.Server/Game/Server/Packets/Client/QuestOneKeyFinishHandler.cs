using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(86, "任务完成")]
public class QuestOneKeyFinishHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		packet.ReadInt();
		client.Player.SendMessage("Thao tác thất bại.");
		return 0;
	}
}
