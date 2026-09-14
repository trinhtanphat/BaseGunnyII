using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Packets.Client;

[PacketHandler(124, "物品比较")]
public class MoveGoodsAllHandler : IPacketHandler
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int num = packet.ReadInt();
		int bageType = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		List<ItemInfo> items = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
		if (num == items.Count)
		{
			inventory.BeginChanges();
			try
			{
				ItemInfo[] rawSpaces = inventory.GetRawSpaces();
				inventory.ClearBag();
				for (int i = 0; i < num; i++)
				{
					int num2 = packet.ReadInt();
					int num3 = packet.ReadInt();
					ItemInfo item = rawSpaces[num2];
					if (!inventory.AddItemTo(item, num3))
					{
						log.Warn($"move item error: old place:{num2} new place:{num3}");
					}
				}
				if (flag)
				{
					items = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
					List<int> list = new List<int>();
					for (int j = 0; j < items.Count; j++)
					{
						if (list.Contains(j))
						{
							continue;
						}
						for (int num4 = items.Count - 1; num4 > j; num4--)
						{
							if (!list.Contains(num4) && items[j].TemplateID == items[num4].TemplateID && items[j].CanStackedTo(items[num4]))
							{
								inventory.MoveItem(items[num4].Place, items[j].Place, items[num4].Count);
								list.Add(num4);
							}
						}
					}
					items = inventory.GetItems(inventory.BeginSlot, inventory.Capalility);
					if (inventory.FindFirstEmptySlot() != -1)
					{
						for (int k = 1; inventory.FindFirstEmptySlot() < items[items.Count - k].Place; k++)
						{
							inventory.MoveItem(items[items.Count - k].Place, inventory.FindFirstEmptySlot(), items[items.Count - k].Count);
						}
					}
				}
			}
			catch (Exception ex)
			{
				log.ErrorFormat("Arrage bag errror,user id:{0}   msg:{1}", client.Player.PlayerId, ex.Message);
			}
			finally
			{
				inventory.CommitChanges();
			}
		}
		return 0;
	}
}
