using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(108, "选取")]
public class GameTakeTempItemsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		string message = string.Empty;
		int num = packet.ReadInt();
		if (num != -1)
		{
			ItemInfo itemAt = client.Player.TempBag.GetItemAt(num);
			GetItem(client.Player, itemAt, ref message);
		}
		else
		{
			List<ItemInfo> items = client.Player.TempBag.GetItems();
			foreach (ItemInfo item in items)
			{
				if (!GetItem(client.Player, item, ref message))
				{
					break;
				}
			}
		}
		if (!string.IsNullOrEmpty(message))
		{
			client.Out.SendMessage(eMessageType.ERROR, message);
		}
		return 0;
	}

	private bool GetItem(GamePlayer player, ItemInfo item, ref string message)
	{
		if (item == null)
		{
			return false;
		}
		PlayerInventory itemInventory = player.GetItemInventory(item.Template);
		if (itemInventory.AddItem(item))
		{
			player.TempBag.RemoveItem(item);
			item.IsExist = true;
			return true;
		}
		itemInventory.UpdateChangedPlaces();
		message = LanguageMgr.GetTranslation(item.GetBagName()) + LanguageMgr.GetTranslation("GameTakeTempItemsHandler.Msg");
		return false;
	}
}
