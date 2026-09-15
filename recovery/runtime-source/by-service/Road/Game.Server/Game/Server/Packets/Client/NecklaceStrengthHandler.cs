using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(95, "客户端日记")]
public class NecklaceStrengthHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		int slot = packet.ReadInt();
		int num2 = packet.ReadInt();
		ItemInfo itemAt = client.Player.PropBag.GetItemAt(slot);
		if (itemAt.Count < num2)
		{
			num2 = itemAt.Count;
		}
		int num3 = num;
		if (num3 == 2)
		{
			int lv = 12;
			int necklaceExpAdd = client.Player.PlayerCharacter.necklaceExpAdd;
			int necklaceExp = client.Player.PlayerCharacter.necklaceExp;
			int necklaceMaxExp = StrengthenMgr.GetNecklaceMaxExp(lv);
			if (necklaceExp <= necklaceMaxExp)
			{
				if (itemAt != null && itemAt.TemplateID == 11160 && itemAt.Count > 0 && num2 > 0)
				{
					int property = itemAt.Template.Property2;
					necklaceExp += property * num2;
					int num4 = StrengthenMgr.GetNecklacePlus(necklaceExp, necklaceExpAdd);
					if (necklaceExp >= necklaceMaxExp)
					{
						num4 = StrengthenMgr.GetNecklaceMaxPlus(lv);
						num2 -= (necklaceExp - necklaceMaxExp) / property;
						client.Player.PlayerCharacter.necklaceExp = necklaceMaxExp + property;
					}
					else
					{
						client.Player.PlayerCharacter.necklaceExp = necklaceExp;
					}
					if (num4 > necklaceExpAdd)
					{
						client.Player.PlayerCharacter.necklaceExpAdd = num4;
						client.Player.MainBag.UpdatePlayerProperties();
					}
					client.Player.RemoveTemplate(itemAt.TemplateID, num2);
				}
			}
			else
			{
				client.Player.SendMessage("Đã đạt cấp tối đa!");
			}
		}
		client.Player.Out.SendNecklaceStrength(client.Player.PlayerCharacter);
		return 0;
	}
}
