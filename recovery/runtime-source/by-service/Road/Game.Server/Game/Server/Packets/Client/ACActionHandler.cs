using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(35, "user ac action")]
[Obsolete("已经不用")]
public class ACActionHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		return 1;
	}
}
