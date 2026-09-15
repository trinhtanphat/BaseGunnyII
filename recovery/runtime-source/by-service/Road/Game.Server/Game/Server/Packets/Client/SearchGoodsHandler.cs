using System;
using Bussiness;
using Game.Base.Packets;

namespace Game.Server.Packets.Client;

[PacketHandler(98, "客户端日记")]
public class SearchGoodsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(98);
		switch (b)
		{
		case 0:
		{
			gSPacketIn.WriteByte(16);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(GameProperties.SearchGoodsFreeCount);
			gSPacketIn.WriteInt(1);
			for (int i = 0; i < 1; i++)
			{
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(7024);
			}
			client.Out.SendTCP(gSPacketIn);
			break;
		}
		case 1:
			packet.ReadBoolean();
			gSPacketIn.WriteByte(17);
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteInt(3);
			client.Out.SendTCP(gSPacketIn);
			break;
		default:
			Console.WriteLine("SearchGoodsPackageType." + (SearchGoodsPackageType)b);
			break;
		}
		return 0;
	}
}
