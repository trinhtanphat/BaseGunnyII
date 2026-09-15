using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(196, "场景用户离开")]
public class CardResetHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		UsersCardInfo itemByPlace = client.Player.CardBag.GetItemByPlace(0, num2);
		if (itemByPlace == null)
		{
			client.Out.SendMessage(eMessageType.Normal, "Xảy ra lổi, chuyển kênh và thử lại.");
			return 0;
		}
		if (client.Player.PlayerCharacter.CardSoul <= 50)
		{
			client.Out.SendMessage(eMessageType.Normal, "Thẻ hồn không đủ.");
			return 0;
		}
		List<int> list = new List<int>();
		string message = "Tẩy điểm thành công!";
		int minValue = 1;
		int maxValue = 10;
		if (itemByPlace.CardType == 2)
		{
			minValue = 5;
			maxValue = 31;
		}
		if (itemByPlace.CardType == 1)
		{
			minValue = 15;
			maxValue = 51;
		}
		if (itemByPlace.CardType == 4)
		{
			minValue = 25;
			maxValue = 81;
		}
		switch (num)
		{
		case 0:
		{
			for (int i = 0; i < 4; i++)
			{
				int num3 = random.Next(minValue, maxValue);
				list.Add(num3);
				switch (i)
				{
				case 0:
					itemByPlace.Attack = num3;
					break;
				case 1:
					itemByPlace.Defence = num3;
					break;
				case 2:
					itemByPlace.Agility = num3;
					break;
				case 3:
					itemByPlace.Luck = num3;
					break;
				}
			}
			client.Player.CardBag.UpdateTempCard(itemByPlace);
			client.Player.RemoveCardSoul(50);
			client.Player.Out.SendPlayerCardReset(client.Player.PlayerCharacter, list);
			break;
		}
		case 1:
			message = "Cập nhật thay đổi thành công!";
			client.Player.CardBag.UpdateCard();
			if (num2 < 5)
			{
				client.Player.MainBag.UpdatePlayerProperties();
			}
			client.Player.CardBag.SaveToDatabase();
			break;
		}
		client.Out.SendMessage(eMessageType.Normal, message);
		return 0;
	}
}
