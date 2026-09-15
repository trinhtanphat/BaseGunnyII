using System;
using Game.Base.Packets;

namespace Game.Server.Packets.Client
{

[PacketHandler(404, "场景用户离开")]
public class RingStationHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int iD = client.Player.PlayerCharacter.ID;
		GSPacketIn gSPacketIn = new GSPacketIn(404, iD);
		switch (b)
		{
		case 6:
			gSPacketIn.WriteByte(6);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteDateTime(DateTime.Now);
			client.Player.SendTCP(gSPacketIn);
			break;
		default:
			Console.WriteLine("RingStationPackageType." + (RingStationPackageType)b);
			break;
		case 1:
		{
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(10);
			gSPacketIn.WriteInt(10);
			gSPacketIn.WriteInt(80000);
			gSPacketIn.WriteDateTime(DateTime.Now);
			gSPacketIn.WriteInt(100000);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString("Ủn ỉn chán nản vì không làm được lôi đài. Hu huuuu");
			gSPacketIn.WriteInt(300);
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
			gSPacketIn.WriteString("Ủn ỉn No.1");
			gSPacketIn.WriteInt(4);
			for (int i = 0; i < 4; i++)
			{
				gSPacketIn.WriteInt(iD);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.UserName);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
				gSPacketIn.WriteByte(client.Player.PlayerCharacter.typeVIP);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.VIPLevel);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Grade);
				gSPacketIn.WriteBoolean(client.Player.PlayerCharacter.Sex);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.Style);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.Colors);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.Skin);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.ConsortiaName);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Hide);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Offer);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Win);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Total);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Escape);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Repute);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.Nimbus);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.GP);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.FightPower);
				gSPacketIn.WriteInt(client.Player.PlayerCharacter.AchievementPoint);
				gSPacketIn.WriteInt(2 + i);
				if (client.Player.MainWeapon == null)
				{
					gSPacketIn.WriteInt(7008);
				}
				else
				{
					gSPacketIn.WriteInt(client.Player.MainWeapon.TemplateID);
				}
				gSPacketIn.WriteString("Ủn ỉn thách thức");
			}
			client.Player.SendTCP(gSPacketIn);
			break;
		}
		}
		return 0;
	}
}
}
