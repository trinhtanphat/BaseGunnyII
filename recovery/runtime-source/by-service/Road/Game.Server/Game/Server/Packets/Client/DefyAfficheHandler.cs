using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(123, "场景用户离开")]
public class DefyAfficheHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		packet.ReadBoolean();
		string str = packet.ReadString();
		int num = 200;
		if (client.Player.PlayerCharacter.Money >= num)
		{
			client.Player.RemoveMoney(num);
			GSPacketIn gSPacketIn = new GSPacketIn(123);
			gSPacketIn.WriteString(str);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
			client.Player.LastChatTime = DateTime.Now;
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				gSPacketIn.ClientID = gamePlayer.PlayerCharacter.ID;
				gamePlayer.Out.SendTCP(gSPacketIn);
			}
		}
		else
		{
			client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Xu không đủ!"));
		}
		return 0;
	}
}
