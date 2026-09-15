using System;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(133, "场景用户离开")]
public class LatentEnergyHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		int num2 = packet.ReadInt();
		int place = packet.ReadInt();
		ItemInfo itemAt = client.Player.GetItemAt((eBageType)num2, place);
		if (!itemAt.CanLatentEnergy())
		{
			client.Out.SendMessage(eMessageType.Normal, "Vật phẩm không đúng, thao tác thất bại.");
			return 0;
		}
		PlayerInventory inventory = client.Player.GetInventory((eBageType)num2);
		string msg = "Kích hoạt tiềm năng thành công!";
		GSPacketIn gSPacketIn = new GSPacketIn(133, client.Player.PlayerCharacter.ID);
		if (num == 1)
		{
			int num3 = packet.ReadInt();
			int place2 = packet.ReadInt();
			ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)num3, place2);
			if (itemAt2 == null || itemAt2.Count < 1)
			{
				client.Out.SendMessage(eMessageType.Normal, "Vật phẩm không đủ.");
				return 0;
			}
			int num4 = int.Parse(itemAt.latentEnergyCurStr.Split(',')[0]);
			ItemTemplateInfo template = itemAt2.Template;
			if (itemAt.IsValidLatentEnergy() || num4 >= template.Property3 - 5 || num4 <= template.Property2 - 5)
			{
				itemAt.ResetLatentEnergy();
			}
			string text = random.Next(template.Property2, template.Property3).ToString();
			for (int i = 1; i < 4; i++)
			{
				text = text + "," + random.Next(template.Property2, template.Property3);
			}
			if (itemAt.latentEnergyCurStr.Split(',')[0] == "0")
			{
				itemAt.latentEnergyCurStr = text;
			}
			itemAt.latentEnergyNewStr = text;
			itemAt.latentEnergyEndTime = DateTime.Now.AddDays(7.0);
			PlayerInventory inventory2 = client.Player.GetInventory((eBageType)num3);
			inventory2.RemoveCountFromStack(itemAt2, 1);
		}
		else
		{
			itemAt.latentEnergyCurStr = itemAt.latentEnergyNewStr;
			msg = "Cập nhật tiềm năng thành công!";
		}
		gSPacketIn.WriteInt(itemAt.Place);
		gSPacketIn.WriteString(itemAt.latentEnergyCurStr);
		gSPacketIn.WriteString(itemAt.latentEnergyNewStr);
		gSPacketIn.WriteDateTime(itemAt.latentEnergyEndTime);
		itemAt.IsBinds = true;
		inventory.UpdateItem(itemAt);
		client.Player.MainBag.UpdatePlayerProperties();
		client.Out.SendTCP(gSPacketIn);
		client.Player.SendMessage(msg);
		return 0;
	}
}
