using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(232, "打开物品")]
public class CaddyClearAllHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		PlayerInventory caddyBag = client.Player.CaddyBag;
		int num = 1;
		int num2 = 0;
		int num3 = 0;
		string text = "";
		string text2 = "";
		for (int i = 0; i < caddyBag.Capalility; i++)
		{
			ItemInfo itemAt = caddyBag.GetItemAt(i);
			if (itemAt != null)
			{
				if (itemAt.Template.ReclaimType == 1)
				{
					num2 += num * itemAt.Template.ReclaimValue;
				}
				if (itemAt.Template.ReclaimType == 2)
				{
					num3 += num * itemAt.Template.ReclaimValue;
				}
				caddyBag.RemoveItem(itemAt);
			}
		}
		if (num2 > 0)
		{
			text = LanguageMgr.GetTranslation("ItemReclaimHandler.Success2", num2);
		}
		if (num3 > 0)
		{
			text2 = LanguageMgr.GetTranslation("ItemReclaimHandler.Success1", num3);
		}
		client.Player.BeginChanges();
		client.Player.AddGold(num2);
		client.Player.AddGiftToken(num3);
		client.Player.CommitChanges();
		client.Out.SendMessage(eMessageType.Normal, text + " " + text2);
		return 1;
	}
}
