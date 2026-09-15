using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(44, "购买物品")]
public class UserBuyItemHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
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
		int useableScore = 0;
		StringBuilder stringBuilder = new StringBuilder();
		eMessageType type = eMessageType.Normal;
		string translateId = "UserBuyItemHandler.Success";
		GSPacketIn gSPacketIn = new GSPacketIn(44, client.Player.PlayerCharacter.ID);
		List<bool> list = new List<bool>();
		List<int> list2 = new List<int>();
		StringBuilder stringBuilder2 = new StringBuilder();
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		bool isBinds = false;
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
		int num = packet.ReadInt();
		for (int i = 0; i < num; i++)
		{
			packet.ReadInt();
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			string text = packet.ReadString();
			bool item = packet.ReadBoolean();
			string text2 = packet.ReadString();
			int item2 = packet.ReadInt();
			packet.ReadBoolean();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(num2);
			bool flag = false;
			if (shopItemInfoById != null && (ShopMgr.IsOnShop(num2) || ShopMgr.IsSpecialItem(shopItemInfoById.TemplateID)))
			{
				flag = true;
				if (shopItemInfoById.ShopID == 99 || shopItemInfoById.ShopID == 98 || shopItemInfoById.ShopID == 95)
				{
					flag = false;
				}
			}
			if (!ShopMgr.CanBuy(shopItemInfoById.ShopID, consortiaInfo?.ShopLevel ?? 1, ref isBinds, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.Riches))
			{
				continue;
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
			if (shopItemInfoById.BuyType == 0)
			{
				if (1 == num3)
				{
					itemInfo.ValidDate = shopItemInfoById.AUnit;
				}
				if (2 == num3)
				{
					itemInfo.ValidDate = shopItemInfoById.BUnit;
				}
				if (3 == num3)
				{
					itemInfo.ValidDate = shopItemInfoById.CUnit;
				}
			}
			else
			{
				if (1 == num3)
				{
					itemInfo.Count = shopItemInfoById.AUnit;
				}
				if (2 == num3)
				{
					itemInfo.Count = shopItemInfoById.BUnit;
				}
				if (3 == num3)
				{
					itemInfo.Count = shopItemInfoById.CUnit;
				}
			}
			if (num3 > 3 || num3 < 0)
			{
				flag = false;
			}
			if (itemInfo == null && shopItemInfoById == null)
			{
				continue;
			}
			itemInfo.Color = ((text == null) ? "" : text);
			itemInfo.Skin = ((text2 == null) ? "" : text2);
			if (isBinds)
			{
				itemInfo.IsBinds = true;
			}
			else
			{
				itemInfo.IsBinds = Convert.ToBoolean(shopItemInfoById.IsBind);
			}
			stringBuilder2.Append(num3);
			stringBuilder2.Append(",");
			if (flag)
			{
				if (!dictionary.ContainsKey(itemInfo.TemplateID))
				{
					dictionary.Add(itemInfo.TemplateID, itemInfo);
				}
				else
				{
					dictionary[itemInfo.TemplateID].Count += itemInfo.Count;
				}
			}
			list.Add(item);
			list2.Add(item2);
			ShopMgr.SetItemType(shopItemInfoById, num3, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
		}
		int num4 = packet.ReadInt();
		if (dictionary.Count == 0)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Hệ thống chưa bán vật phẩm này."));
			return 1;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		bool flag2 = false;
		int num5 = gold + money + offer + gifttoken + medal + damageScore + petScore + hardCurrency + LeagueMoney;
		if (GameProperties.IsActiveMoney)
		{
			foreach (ItemInfo value in dictionary.Values)
			{
				if (MustActiveMoney(value.TemplateID) && client.Player.Actives.Info.ActiveMoney < num5)
				{
					client.Out.SendMessage(eMessageType.Normal, $"Xu năng động không đủ mua {value.Template.Name}, Hiện tại bạn có {client.Player.Actives.Info.ActiveMoney} Xu năng động. ");
					return 0;
				}
				client.Player.RemoveActiveMoney(num5);
			}
		}
		if (iTemplateID > 0 && num5 == 0)
		{
			int itemCount = client.Player.GetItemCount(iTemplateID);
			if (itemCount <= 0 || itemCount < iCount)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
			}
			else
			{
				flag2 = client.Player.RemoveTemplate(iTemplateID, iCount);
			}
		}
		else if (gold + money + offer + gifttoken + medal + hardCurrency + damageScore + petScore + LeagueMoney > 0)
		{
			if (money > client.Player.PlayerCharacter.Money)
			{
				translateId = "UserBuyItemHandler.NoMoney";
			}
			else if (gold > client.Player.PlayerCharacter.Gold)
			{
				translateId = "UserBuyItemHandler.NoGold";
			}
			else if (offer > client.Player.PlayerCharacter.Offer)
			{
				translateId = "UserBuyItemHandler.NoOffer";
			}
			else if (gifttoken > client.Player.PlayerCharacter.GiftToken)
			{
				translateId = "UserBuyItemHandler.GiftToken";
			}
			else if (medal > client.Player.PlayerCharacter.medal)
			{
				translateId = "UserBuyItemHandler.Medal";
			}
			else if (damageScore > client.Player.PlayerCharacter.damageScores)
			{
				translateId = "UserBuyItemHandler.FailByPermission";
			}
			else if (petScore > client.Player.PlayerCharacter.petScore)
			{
				translateId = "UserBuyItemHandler.FailByPermission";
			}
			else if (hardCurrency > client.Player.PlayerCharacter.hardCurrency)
			{
				translateId = "UserBuyItemHandler.FailByPermission";
			}
			else if (LeagueMoney > client.Player.PlayerCharacter.LeagueMoney)
			{
				translateId = "UserBuyItemHandler.FailByPermission";
			}
			else if (useableScore == 0)
			{
				type = eMessageType.ERROR;
				client.Player.RemoveMoney(money);
				client.Player.RemoveGold(gold);
				client.Player.RemoveOffer(offer);
				client.Player.RemoveGiftToken(gifttoken);
				client.Player.RemoveMedal(medal);
				client.Player.RemoveDamageScores(damageScore);
				client.Player.RemovePetScore(petScore);
				client.Player.RemoveHardCurrency(hardCurrency);
				client.Player.RemoveLeagueMoney(LeagueMoney);
				if (!GameProperties.IsDDTMoneyActive)
				{
					client.Player.AddExpVip(money);
				}
				flag2 = true;
			}
		}
		if (flag2)
		{
			string text3 = "";
			foreach (ItemInfo value2 in dictionary.Values)
			{
				text3 += ((text3 == "") ? value2.TemplateID.ToString() : ("," + value2.TemplateID));
				switch (num4)
				{
				case 1:
				case 2:
					if (!AddItemsToStoreBag(client, value2))
					{
						client.Player.AddTemplate(value2);
					}
					continue;
				}
				new List<ItemInfo>();
				if (value2.Template.MaxCount == 1)
				{
					for (int j = 0; j < value2.Count; j++)
					{
						ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(value2.Template, value2);
						itemInfo2.Count = 1;
						client.Player.AddTemplate(itemInfo2);
					}
				}
				else
				{
					client.Player.AddTemplate(value2);
				}
			}
			client.Player.OnPaid(money, gold, offer, gifttoken, medal, stringBuilder.ToString());
		}
		else
		{
			type = eMessageType.ERROR;
			translateId = "UserBuyItemHandler.FailByPermission";
		}
		client.Player.SaveBagIntoDatabase();
		client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId));
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(3);
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}

	private bool MustActiveMoney(int templateID)
	{
		return templateID == 201192;
	}

	private bool AddItemsToStoreBag(GameClient client, ItemInfo item)
	{
		int num = 2;
		if (item.TemplateID == 11018 || item.TemplateID == 11025)
		{
			num = 0;
		}
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(num);
		if (itemAt != null && itemAt.Count < itemAt.Template.MaxCount && itemAt.CanStackedTo(item))
		{
			return client.Player.StoreBag.AddTemplateAt(item, item.Count, num);
		}
		if (itemAt == null)
		{
			return client.Player.StoreBag.AddItemTo(item, num);
		}
		return client.Player.AddTemplate(item, (eBageType)item.GetBagType, item.Count, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView);
	}
}
