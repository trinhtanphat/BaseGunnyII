using System;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(62, "续费")]
public class UserItemContineueHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		new StringBuilder();
		int num = packet.ReadInt();
		string translateId = "UserItemContineueHandler.Success";
		for (int i = 0; i < num; i++)
		{
			eBageType eBageType2 = (eBageType)packet.ReadByte();
			int num2 = packet.ReadInt();
			int iD = packet.ReadInt();
			int num3 = packet.ReadByte();
			bool flag = packet.ReadBoolean();
			packet.ReadInt();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			ItemInfo itemAt = client.Player.GetItemAt(eBageType2, num2);
			if (eBageType2 != eBageType.MainBag || num2 < 31 || itemAt == null || shopItemInfoById == null || shopItemInfoById.TemplateID != itemAt.TemplateID)
			{
				continue;
			}
			if (itemAt.ValidDate != 0)
			{
				int gold = 0;
				int money = 0;
				int offer = 0;
				int gifttoken = 0;
				int medal = 0;
				int damageScore = 0;
				int petScore = 0;
				int iTemplateID = 0;
				int iCount = 0;
				int hardCurrency = 0;
				int LeagueMoney = 0;
				int validDate = itemAt.ValidDate;
				DateTime beginDate = itemAt.BeginDate;
				int count = itemAt.Count;
				bool flag2 = itemAt.IsValidItem();
				if (!ShopMgr.SetItemType(shopItemInfoById, num3, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref medal))
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Vật phẩm không thể tiếp phí."));
					return 0;
				}
				if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money && offer <= client.Player.PlayerCharacter.Offer && gifttoken <= client.Player.PlayerCharacter.GiftToken)
				{
					int num4 = 3;
					if (1 == num3)
					{
						num4 = shopItemInfoById.AUnit;
					}
					if (2 == num3)
					{
						num4 = shopItemInfoById.BUnit;
					}
					if (3 == num3)
					{
						num4 = shopItemInfoById.CUnit;
					}
					if (num4 > 365 || itemAt.ValidDate > 365)
					{
						flag2 = false;
					}
					if (!flag2)
					{
						itemAt.ValidDate = num4;
						itemAt.BeginDate = DateTime.Now;
						client.Player.RemoveMoney(money);
						client.Player.RemoveGold(gold);
						client.Player.RemoveOffer(offer);
						client.Player.RemoveGiftToken(gifttoken);
					}
					else
					{
						translateId = $"{itemAt.Template.Name} chưa hết hạn";
					}
				}
				else
				{
					itemAt.ValidDate = validDate;
					itemAt.Count = count;
					translateId = "UserItemContineueHandler.NoMoney";
				}
			}
			if (eBageType2 == eBageType.MainBag)
			{
				if (flag)
				{
					int toSlot = client.Player.MainBag.FindItemEpuipSlot(itemAt.Template);
					client.Player.MainBag.MoveItem(num2, toSlot, itemAt.Count);
				}
				else
				{
					client.Player.MainBag.UpdateItem(itemAt);
				}
			}
		}
		client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
		return 0;
	}
}
