using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(156, "客户端日记")]
public class BuyTransnationalGoodsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int iD = packet.ReadInt();
		PyramidInfo pyramid = client.Player.Actives.Pyramid;
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
		eMessageType type = eMessageType.Normal;
		string translateId = "UserBuyItemHandler.Success";
		ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
		bool flag = false;
		if (shopItemInfoById != null && ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 98)
		{
			flag = true;
		}
		if (!flag)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
			return 1;
		}
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
		if (shopItemInfoById.BuyType == 0)
		{
			itemInfo.ValidDate = shopItemInfoById.AUnit;
		}
		else
		{
			itemInfo.Count = shopItemInfoById.AUnit;
		}
		itemInfo.IsBinds = true;
		if (!dictionary.Keys.Contains(itemInfo.TemplateID))
		{
			dictionary.Add(itemInfo.TemplateID, itemInfo);
		}
		else
		{
			dictionary[itemInfo.TemplateID].Count += itemInfo.Count;
		}
		ShopMgr.SetItemType(shopItemInfoById, 1, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
		if (dictionary.Values.Count == 0)
		{
			return 1;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		if (pyramid.totalPoint < damageScore)
		{
			client.Player.SendMessage("Tích lũy không đủ.");
			return 0;
		}
		pyramid.totalPoint -= damageScore;
		if (true)
		{
			string text = "";
			foreach (ItemInfo value in dictionary.Values)
			{
				text += ((text == "") ? value.TemplateID.ToString() : ("," + value.TemplateID));
				client.Player.AddTemplate(value);
			}
		}
		else
		{
			type = eMessageType.ERROR;
			translateId = "UserBuyItemHandler.FailByPermission";
		}
		client.Player.SaveBagIntoDatabase();
		client.Out.SendMessage(type, LanguageMgr.GetTranslation(translateId));
		GSPacketIn gSPacketIn = new GSPacketIn(145, client.Player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
		gSPacketIn.WriteInt(pyramid.totalPoint);
		gSPacketIn.WriteInt(pyramid.turnPoint);
		gSPacketIn.WriteInt(pyramid.pointRatio);
		gSPacketIn.WriteInt(pyramid.currentLayer);
		client.Player.SendTCP(gSPacketIn);
		return 0;
	}
}
