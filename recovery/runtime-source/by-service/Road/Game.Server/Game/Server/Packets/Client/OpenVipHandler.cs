using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;

namespace Game.Server.Packets.Client;

[PacketHandler(92, "场景用户离开")]
public class OpenVipHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		string text = packet.ReadString();
		int num = packet.ReadInt();
		int value = 569;
		int num2 = 569;
		int num3 = 1707;
		int num4 = 3000;
		string message = "Kích hoạt VIP thành công!";
		int num5 = num;
		switch (num5)
		{
		case 180:
			value = num4;
			break;
		case 90:
			value = num3;
			break;
		case 30:
			value = num2;
			break;
		}
		GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
		if (client.Player.MoneyDirect(value))
		{
			DateTime ExpireDayOut = DateTime.Now;
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerBussiness.VIPRenewal(text, num, ref ExpireDayOut);
			if (clientByPlayerNickName == null)
			{
				message = "Tiếp phí VIP cho " + text + " thàng công!";
			}
			else if (client.Player.PlayerCharacter.NickName == text)
			{
				if (client.Player.PlayerCharacter.typeVIP == 0)
				{
					client.Player.OpenVIP(num5, ExpireDayOut);
				}
				else
				{
					client.Player.ContinousVIP(num5, ExpireDayOut);
					message = "Gia hạn VIP thành công!";
				}
				client.Out.SendOpenVIP(client.Player.PlayerCharacter);
			}
			else
			{
				string message2;
				if (clientByPlayerNickName.PlayerCharacter.typeVIP == 0)
				{
					clientByPlayerNickName.OpenVIP(num5, ExpireDayOut);
					message = "Kích hoạt VIP cho " + text + " thàng công!";
					message2 = client.Player.PlayerCharacter.NickName + ", tiếp phí VIP cho bạn thàng công!";
				}
				else
				{
					clientByPlayerNickName.ContinousVIP(num5, ExpireDayOut);
					message = "Gia hạn VIP cho " + text + " thàng công!";
					message2 = client.Player.PlayerCharacter.NickName + ", gia hạn VIP cho bạn thàng công!";
				}
				clientByPlayerNickName.Out.SendOpenVIP(clientByPlayerNickName.PlayerCharacter);
				clientByPlayerNickName.Out.SendMessage(eMessageType.Normal, message2);
			}
			client.Player.AddExpVip(value);
			client.Out.SendMessage(eMessageType.Normal, message);
			return 0;
		}
		client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.Money"));
		return 0;
	}
}
