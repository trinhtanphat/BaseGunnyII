using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(222, "场景用户离开")]
public class EquipRetrieveHandler : IPacketHandler
{
	private Random rnd = new Random();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int user = 0;
		PlayerInventory inventory = client.Player.GetInventory(eBageType.Store);
		int num = 0;
		bool isBinds = false;
		for (int i = 1; i < 5; i++)
		{
			ItemInfo itemAt = inventory.GetItemAt(i);
			if (itemAt != null)
			{
				inventory.RemoveItemAt(i);
			}
			if (itemAt.IsBinds)
			{
				isBinds = true;
			}
			num += itemAt.Template.Quality;
		}
		int num2 = num;
		if (num2 <= 12)
		{
			if (num2 == 8 || num2 == 12)
			{
				goto IL_00b6;
			}
		}
		else if (num2 == 15 || num2 == 20)
		{
			goto IL_00b6;
		}
		goto IL_00b8;
		IL_00b6:
		user = num;
		goto IL_00b8;
		IL_00b8:
		List<ItemInfo> info = null;
		DropInventory.RetrieveDrop(user, ref info);
		int index = rnd.Next(info.Count);
		int templateID = info[index].TemplateID;
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateID), 1, 105);
		itemInfo.IsBinds = isBinds;
		itemInfo.BeginDate = DateTime.Now;
		if (itemInfo.Template.CategoryID != 11)
		{
			itemInfo.ValidDate = 30;
			itemInfo.IsBinds = true;
		}
		itemInfo.IsBinds = true;
		inventory.AddItemTo(itemInfo, 0);
		return 1;
	}
}
