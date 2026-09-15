using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(126, "场景用户离开")]
public class QuickBuyGoldBoxHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		packet.ReadBoolean();
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(1123301);
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
		int value = num * shopItemInfoById.AValue1;
		if (client.Player.MoneyDirect(value))
		{
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
			List<ItemInfo> itemInfos = new List<ItemInfo>();
			ItemBoxMgr.CreateItemBox(itemTemplateInfo.TemplateID, itemInfos, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge);
			int num2 = num * gold;
			client.Player.AddGold(num2);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bạn nhận được " + num2 + " vàng."));
		}
		return 0;
	}
}
