using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(138, "物品强化")]
public class ItemAdvanceHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		packet.ReadBoolean();
		packet.ReadBoolean();
		GSPacketIn gSPacketIn = new GSPacketIn(138, client.Player.PlayerCharacter.ID);
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
		ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(1);
		int strengthenLevel = itemInfo.StrengthenLevel;
		if (itemAt.Count <= 0 || itemAt == null || itemInfo == null)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Đặt đá tăng cấp và trang bị cần tăng cấp vào!"));
			return 0;
		}
		if (strengthenLevel >= 15)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Level đã đạt cấp độ cao nhất, không thể thăng cấp!"));
			return 0;
		}
		int count = 1;
		string text = "";
		if (itemInfo != null && itemInfo.Template.CanStrengthen && itemInfo.Template.CategoryID < 18 && itemInfo.Count == 1)
		{
			flag = flag || itemInfo.IsBinds;
			stringBuilder.Append(itemInfo.ItemID + ":" + itemInfo.TemplateID + ",");
			int num = 0;
			if (itemAt != null && itemAt.Template.CategoryID == 11 && (itemAt.Template.Property1 == 2 || itemAt.Template.Property1 == 45))
			{
				flag = flag || itemAt.IsBinds;
				string text2 = text;
				text = text2 + "," + itemAt.ItemID + ":" + itemAt.Template.Name;
				num = ((itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2);
			}
			stringBuilder.Append("true");
			bool flag2 = false;
			int num2 = random.Next(20000);
			double num3 = itemInfo.StrengthenExp / strengthenLevel;
			if (num3 > (double)num2)
			{
				itemInfo.IsBinds = flag;
				itemInfo.StrengthenLevel++;
				itemInfo.StrengthenExp = 0;
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(num);
				flag2 = true;
				StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindStrengthenGoodsInfo(itemInfo.StrengthenLevel, itemInfo.TemplateID);
				if (strengthenGoodsInfo != null && itemInfo.Template.CategoryID == 7 && strengthenGoodsInfo.GainEquip > itemInfo.TemplateID)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(strengthenGoodsInfo.GainEquip);
					if (itemTemplateInfo != null)
					{
						ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemInfo);
						client.Player.StoreBag.RemoveItemAt(1);
						client.Player.StoreBag.AddItemTo(itemInfo2, 1);
						itemInfo = itemInfo2;
					}
				}
			}
			else
			{
				itemInfo.StrengthenExp += num;
				gSPacketIn.WriteByte(1);
				gSPacketIn.WriteInt(num);
			}
			client.Player.StoreBag.RemoveCountFromStack(itemAt, count);
			client.Player.StoreBag.UpdateItem(itemInfo);
			client.Out.SendTCP(gSPacketIn);
			if (flag2 && itemInfo.ItemID > 0)
			{
				string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation2", client.Player.PlayerCharacter.NickName, "@", itemInfo.StrengthenLevel - 12);
				GSPacketIn packet2 = WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemInfo.ItemID, itemInfo.TemplateID, "");
				GameServer.Instance.LoginServer.SendPacket(packet2);
			}
			stringBuilder.Append(itemInfo.StrengthenLevel);
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Content1") + itemAt.Template.Name + LanguageMgr.GetTranslation("ItemStrengthenHandler.Content2"));
		}
		if (itemInfo.Place < 31)
		{
			client.Player.MainBag.UpdatePlayerProperties();
		}
		return 0;
	}
}
