using System;
using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Packets.Client;

[PacketHandler(216, "防沉迷系统开关")]
internal class CardDataHander : IPacketHandler
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		PlayerInfo playerCharacter = client.Player.PlayerCharacter;
		CardInventory cardBag = client.Player.CardBag;
		List<ItemInfo> list = new List<ItemInfo>();
		switch (num)
		{
		case 0:
		{
			int num9 = packet.ReadInt();
			int num10 = packet.ReadInt();
			UsersCardInfo itemAt2 = cardBag.GetItemAt(num9);
			if (itemAt2 == null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Không tìm thấy thẻ này, thử lại sau.");
				return 0;
			}
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(itemAt2.TemplateID);
			if (itemTemplateInfo.Property8 == 0 && num10 == 0)
			{
				return 0;
			}
			if (itemTemplateInfo.Property8 == 1 && num10 > 0 && num10 <= 4)
			{
				return 0;
			}
			if (num9 == num10)
			{
				cardBag.MoveCard(num9, num10);
				client.Player.MainBag.UpdatePlayerProperties();
				break;
			}
			string message2;
			if (cardBag.FindEquipCard(itemAt2.TemplateID))
			{
				message2 = "Thẻ này đã trang bị!";
			}
			else
			{
				cardBag.MoveCard(num9, num10);
				client.Player.MainBag.UpdatePlayerProperties();
				message2 = "Trang bị thành công!";
			}
			client.Out.SendMessage(eMessageType.Normal, message2);
			break;
		}
		case 1:
		case 4:
		{
			int slot = packet.ReadInt();
			int num4 = packet.ReadInt();
			ItemInfo itemAt = client.Player.MainBag.GetItemAt(slot);
			if (num == 4)
			{
				itemAt = client.Player.PropBag.GetItemAt(slot);
			}
			if (itemAt == null)
			{
				return 0;
			}
			if (itemAt.Count < num4 || num4 < 1)
			{
				return 0;
			}
			if (!itemAt.IsCard())
			{
				client.Out.SendMessage(eMessageType.Normal, "Hộp thẻ bài lổi!");
				return 0;
			}
			int point = 0;
			int gold = 0;
			int giftToken = 0;
			int medal = 0;
			int exp = 0;
			int honor = 0;
			int hardCurrency = 0;
			int leagueMoney = 0;
			int useableScore = 0;
			int prestge = 0;
			bool flag = false;
			int num5 = 0;
			int num6 = 0;
			for (int i = 0; i < num4; i++)
			{
				if (ItemBoxMgr.CreateItemBox(itemAt.TemplateID, list, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge))
				{
					int index = random.Next(list.Count);
					ItemInfo itemInfo = list[index];
					flag = TakeCard(client, itemInfo.Template.Property5);
					if (flag)
					{
						num5++;
					}
				}
				if (!flag)
				{
					num6 = ((itemAt.TemplateID != 20150) ? (num6 + random.Next(5, 25)) : (num6 + random.Next(50, 500)));
				}
			}
			client.Player.RemoveTemplate(itemAt.TemplateID, num4);
			client.Player.AddCardSoul(num6);
			client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, isSoul: true, num6);
			string message = $"Bạn nhận được {num6} điểm thẻ hồn,{num5} Thẻ bài.";
			client.Out.SendMessage(eMessageType.Normal, message);
			break;
		}
		case 2:
		{
			int num4 = packet.ReadInt();
			List<UsersCardInfo> cards = cardBag.GetCards(5, cardBag.Capalility);
			if (num4 == cards.Count)
			{
				cardBag.BeginChanges();
				try
				{
					UsersCardInfo[] rawSpaces = cardBag.GetRawSpaces();
					cardBag.ClearBag();
					for (int j = 0; j < num4; j++)
					{
						int num7 = packet.ReadInt();
						int num8 = packet.ReadInt();
						UsersCardInfo card = rawSpaces[num7];
						if (!cardBag.AddCardTo(card, num8))
						{
							log.Warn($"move card error: old place:{num7} new place:{num8}");
						}
					}
				}
				catch (Exception ex)
				{
					log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, ex.Message);
				}
				finally
				{
					cardBag.CommitChanges();
				}
			}
			else
			{
				log.Warn($"Count: {num4} recoverCards.Count:{cards.Count} ");
			}
			break;
		}
		case 5:
		{
			int value = 50;
			if (!client.Player.MoneyDirect(value))
			{
				break;
			}
			if (client.Player.PlayerCharacter.GetSoulCount > 0)
			{
				int num2 = 50;
				int num3 = random.Next(1000);
				if (num3 < 100)
				{
					num2 = 120;
				}
				client.Player.AddCardSoul(num2);
				client.Player.PlayerCharacter.GetSoulCount--;
				client.Player.Out.SendPlayerCardSoul(client.Player.PlayerCharacter, isSoul: true, num2);
				client.Out.SendMessage(eMessageType.Normal, $"Bạn nhận được {num2} điểm thẻ hồn.");
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, "Số lần sử dụng thẻ hồn hôm nay đã hết.");
			}
			break;
		}
		}
		return 0;
	}

	private bool TakeCard(GameClient client, int templateId)
	{
		bool result = false;
		int place = client.Player.CardBag.FindFirstEmptySlot(5);
		CardTemplateInfo card = CardMgr.GetCard(templateId);
		if (card != null)
		{
			int num = client.Player.CardBag.FindPlaceByTamplateId(5, templateId);
			UsersCardInfo usersCardInfo;
			if (num == -1)
			{
				usersCardInfo = new UsersCardInfo();
				usersCardInfo.CardType = card.CardType;
				usersCardInfo.UserID = client.Player.PlayerCharacter.ID;
				usersCardInfo.Place = place;
				usersCardInfo.TemplateID = card.CardID;
				usersCardInfo.isFirstGet = true;
				usersCardInfo.Attack = 0;
				usersCardInfo.Agility = 0;
				usersCardInfo.Defence = 0;
				usersCardInfo.Luck = 0;
				usersCardInfo.Damage = 0;
				usersCardInfo.Guard = 0;
			}
			else
			{
				usersCardInfo = client.Player.CardBag.GetItemAt(num);
				if (canUpdate(usersCardInfo.CardType, card.CardType))
				{
					usersCardInfo.isFirstGet = true;
				}
				else
				{
					usersCardInfo = null;
				}
			}
			if (usersCardInfo != null)
			{
				result = ((num != -1) ? client.Player.CardBag.UpdateCardType(usersCardInfo.TemplateID, card.CardType) : client.Player.CardBag.AddCardTo(usersCardInfo, place));
				client.Out.SendGetCard(client.Player.PlayerCharacter, usersCardInfo);
			}
		}
		return result;
	}

	private string cardQuality(int Type)
	{
		return Type switch
		{
			2 => "Bạc",
			1 => "Vàng",
			4 => "Bạch kim",
			_ => "Đồng",
		};
	}

	private bool canUpdate(int oldtype, int newType)
	{
		return (newType == 2 && (oldtype == 0 || oldtype == 3)) || (newType == 1 && (oldtype == 2 || oldtype == 0 || oldtype == 3)) || (newType == 4 && (oldtype == 1 || oldtype == 2 || oldtype == 0 || oldtype == 3));
	}
}
