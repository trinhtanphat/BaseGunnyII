using System.Collections.Generic;
using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(210, "物品炼化")]
public class ItemRefineryHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(210, client.Player.PlayerCharacter.ID);
		bool flag = false;
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		List<ItemInfo> list = new List<ItemInfo>();
		new List<ItemInfo>();
		List<eBageType> list2 = new List<eBageType>();
		StringBuilder stringBuilder = new StringBuilder();
		int defaultprobability = 25;
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.locked"));
			return 1;
		}
		for (int i = 0; i < num2; i++)
		{
			eBageType eBageType2 = (eBageType)packet.ReadInt();
			int place = packet.ReadInt();
			ItemInfo itemAt = client.Player.GetItemAt(eBageType2, place);
			if (itemAt != null)
			{
				if (list.Contains(itemAt))
				{
					client.Out.SendMessage(eMessageType.Normal, "Bad Input");
					return 1;
				}
				if (itemAt.IsBinds)
				{
					flag = true;
				}
				stringBuilder.Append(itemAt.ItemID + ":" + itemAt.TemplateID + ",");
				list.Add(itemAt);
				list2.Add(eBageType2);
			}
		}
		eBageType bagType = (eBageType)packet.ReadInt();
		int place2 = packet.ReadInt();
		ItemInfo itemAt2 = client.Player.GetItemAt(bagType, place2);
		if (itemAt2 != null)
		{
			stringBuilder.Append(itemAt2.ItemID + ":" + itemAt2.TemplateID + ",");
		}
		eBageType bagType2 = (eBageType)packet.ReadInt();
		int place3 = packet.ReadInt();
		ItemInfo itemAt3 = client.Player.GetItemAt(bagType2, place3);
		bool flag2 = itemAt3 != null;
		if (num2 != 4 || itemAt2 == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemRefineryHandler.ItemNotEnough"));
			return 1;
		}
		bool result = false;
		bool IsFormula = false;
		if (num == 0)
		{
			ItemTemplateInfo itemTemplateInfo = RefineryMgr.Refinery(client.Player, list, itemAt2, flag2, num, ref result, ref defaultprobability, ref IsFormula);
			if (itemTemplateInfo != null)
			{
				client.Out.SendRefineryPreview(client.Player, itemTemplateInfo.TemplateID, flag, itemAt2);
			}
			return 0;
		}
		int num3 = 10000;
		if (client.Player.PlayerCharacter.Gold > num3)
		{
			client.Player.RemoveGold(num3);
			ItemTemplateInfo itemTemplateInfo2 = RefineryMgr.Refinery(client.Player, list, itemAt2, flag2, num, ref result, ref defaultprobability, ref IsFormula);
			if (itemTemplateInfo2 != null && IsFormula && result)
			{
				stringBuilder.Append("Success");
				if (true)
				{
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo2, 1, 114);
					if (itemInfo != null)
					{
						client.Player.OnItemMelt(itemAt2.Template.CategoryID);
						itemInfo.IsBinds = flag;
						AbstractInventory itemInventory = client.Player.GetItemInventory(itemTemplateInfo2);
						if (!itemInventory.AddItem(itemInfo, itemInventory.BeginSlot))
						{
							stringBuilder.Append("NoPlace");
							client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(itemInfo.GetBagName()) + LanguageMgr.GetTranslation("ItemFusionHandler.NoPlace"));
						}
						gSPacketIn.WriteByte(0);
						itemAt2.Count--;
						client.Player.UpdateItem(itemAt2);
					}
				}
				else
				{
					stringBuilder.Append("false");
				}
			}
			else
			{
				gSPacketIn.WriteByte(1);
			}
			if (flag2)
			{
				itemAt3.Count--;
				client.Player.UpdateItem(itemAt3);
			}
			for (int j = 0; j < list.Count; j++)
			{
				client.Player.UpdateItem(list[j]);
				if (list[j].Count <= 0)
				{
					client.Player.RemoveItem(list[j]);
				}
			}
			client.Player.RemoveItem(list[list.Count - 1]);
			client.Player.Out.SendTCP(gSPacketIn);
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemRefineryHandler.NoGold"));
		}
		return 1;
	}
}
