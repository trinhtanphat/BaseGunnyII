using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client
{

[PacketHandler(73, "大喇叭")]
public class CBugleHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int templateId = 11100;
		int clientId = packet.ReadInt();
		ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
		if (DateTime.Compare(client.Player.LastChatTime.AddSeconds(15.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Quá nhiều thao tác!"));
			return 1;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(73, clientId);
		if (itemByTemplateID != null)
		{
			packet.ReadString();
			string str = packet.ReadString();
			client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(client.Player.PlayerCharacter.ID);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteString(str);
			gSPacketIn.WriteString("Zone No.1");
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
			client.Player.LastChatTime = DateTime.Now;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				gSPacketIn.ClientID = gamePlayer.PlayerCharacter.ID;
				gamePlayer.Out.SendTCP(gSPacketIn);
			}
		}
		return 0;
	}
}
}
