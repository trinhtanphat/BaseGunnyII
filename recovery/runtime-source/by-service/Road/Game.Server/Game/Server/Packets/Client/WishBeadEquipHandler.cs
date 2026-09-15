using System;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(106, "场景用户离开")]
public class WishBeadEquipHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int slot = packet.ReadInt();
		int bageType = packet.ReadInt();
		int templateId = packet.ReadInt();
		int place = packet.ReadInt();
		int bagType = packet.ReadInt();
		int num = packet.ReadInt();
		GSPacketIn gSPacketIn = new GSPacketIn(106, client.Player.PlayerCharacter.ID);
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)bagType, place);
		if (itemAt2 == null || itemAt == null)
		{
			client.Out.SendMessage(eMessageType.Normal, "Lổi vật phẩm.");
			gSPacketIn.WriteInt(5);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		if (itemAt2.Count < 1 || itemAt2.TemplateID != num)
		{
			client.Out.SendMessage(eMessageType.Normal, "Châu báu chúc phúc không đủ.");
			gSPacketIn.WriteInt(5);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		if (!CanWishBeat(itemAt2.TemplateID, itemAt.Template.CategoryID))
		{
			gSPacketIn.WriteInt(5);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		double num2 = 5.0;
		GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipNewTemplate(templateId);
		itemAt.IsBinds = itemAt2.IsBinds;
		if (goldEquipTemplateLoadInfo == null && itemAt.Template.CategoryID == 7)
		{
			gSPacketIn.WriteInt(5);
		}
		else if (itemAt.StrengthenLevel > GameProperties.WishBeadLimitLv && GameProperties.IsWishBeadLimit)
		{
			gSPacketIn.WriteInt(5);
		}
		else if (!itemAt.IsGold)
		{
			if (num2 > (double)random.Next(100))
			{
				itemAt.goldBeginTime = DateTime.Now;
				itemAt.goldValidDate = 30;
				itemAt.IsBinds = true;
				if (goldEquipTemplateLoadInfo != null && itemAt.Template.CategoryID == 7)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(goldEquipTemplateLoadInfo.NewTemplateId);
					if (itemTemplateInfo != null)
					{
						itemAt.GoldEquip = itemTemplateInfo;
					}
				}
				inventory.UpdateItem(itemAt);
				gSPacketIn.WriteInt(0);
				inventory.SaveToDatabase();
			}
			else
			{
				gSPacketIn.WriteInt(1);
			}
			client.Player.RemoveTemplate(num, 1);
		}
		else
		{
			gSPacketIn.WriteInt(6);
		}
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}

	private bool CanWishBeat(int beatID, int CategoryID)
	{
		return (beatID == 11560 && CategoryID == 7) || (beatID == 11561 && CategoryID == 5) || (beatID == 11562 && CategoryID == 1);
	}
}
