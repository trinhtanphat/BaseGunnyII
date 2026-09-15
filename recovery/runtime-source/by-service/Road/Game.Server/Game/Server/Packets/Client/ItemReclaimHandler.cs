using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(127, "物品比较")]
public class ItemReclaimHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		eBageType bageType = (eBageType)packet.ReadByte();
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory(bageType);
		if (inventory != null && inventory.GetItemAt(num) != null)
		{
			if (inventory.GetItemAt(num).Count <= num2)
			{
				num2 = inventory.GetItemAt(num).Count;
			}
			ItemTemplateInfo template = inventory.GetItemAt(num).Template;
			int num3 = num2 * template.ReclaimValue;
			if (template.ReclaimType == 2)
			{
				client.Player.AddGiftToken(num3);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success1", num3));
			}
			if (template.ReclaimType == 1)
			{
				client.Player.AddGold(num3);
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.Success2", num3));
			}
			inventory.RemoveItemAt(num);
			return 0;
		}
		client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemReclaimHandler.NoSuccess"));
		return 1;
	}
}
