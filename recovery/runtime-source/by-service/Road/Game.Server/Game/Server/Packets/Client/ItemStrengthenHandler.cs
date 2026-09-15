using System;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(59, "物品强化")]
public class ItemStrengthenHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		StringBuilder stringBuilder = new StringBuilder();
		bool flag = false;
		bool flag2 = packet.ReadBoolean();
		bool flag3 = packet.ReadBoolean();
		GSPacketIn gSPacketIn = new GSPacketIn(59, client.Player.PlayerCharacter.ID);
		ItemInfo itemAt = client.Player.StoreBag.GetItemAt(0);
		ItemInfo itemInfo = client.Player.StoreBag.GetItemAt(1);
		if (itemAt.Count <= 0 || itemAt == null || itemInfo == null)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Đặt đá tăng cấp và trang bị cần tăng cấp vào!"));
			return 0;
		}
		int num = 1;
		string Property = null;
		string text = "";
		using (ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness())
		{
			itemRecordBussiness.PropertyString(itemInfo, ref Property);
		}
		if (itemInfo != null && itemInfo.Template.CanStrengthen && itemInfo.Template.CategoryID < 18 && itemInfo.Count == 1)
		{
			flag = flag || itemInfo.IsBinds;
			stringBuilder.Append(itemInfo.ItemID + ":" + itemInfo.TemplateID + ",");
			double num2 = 0.0;
			double num3 = 0.0;
			double num4 = 0.0;
			int strengthenExp = itemInfo.StrengthenExp;
			int strengthenLevel = itemInfo.StrengthenLevel;
			if (strengthenLevel >= 12)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Level đã đạt cấp độ cao nhất, không thể cường hóa!"));
				return 0;
			}
			bool flag4 = false;
			ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			if (flag2)
			{
				ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness();
				ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(client.Player.PlayerCharacter.ConsortiaID, 0, 2);
				if (consortiaInfo == null)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemStrengthenHandler.Fail"));
				}
				else
				{
					if (client.Player.PlayerCharacter.Riches < consortiaEuqipRiches.Riches)
					{
						client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemStrengthenHandler.FailbyPermission"));
						return 1;
					}
					flag4 = true;
				}
			}
			if (itemAt != null && itemAt.Template.CategoryID == 11 && (itemAt.Template.Property1 == 2 || itemAt.Template.Property1 == 35))
			{
				flag = flag || itemAt.IsBinds;
				string text2 = text;
				text = text2 + "," + itemAt.ItemID + ":" + itemAt.Template.Name;
				int num5 = ((itemAt.Template.Property2 < 10) ? 10 : itemAt.Template.Property2);
				if (flag4)
				{
					int smithLevel = consortiaInfo.SmithLevel;
					double num6 = GameProperties.ConsortiaStrengExp(smithLevel - 1);
					num3 = num6 * (double)num5 / 100.0;
				}
				if (client.Player.PlayerCharacter.VIPExpireDay.Date >= DateTime.Now.Date)
				{
					int vIPLevel = client.Player.PlayerCharacter.VIPLevel;
					double num7 = GameProperties.VIPStrengthenExp(vIPLevel - 1);
					num4 = num7 * (double)num5 / 100.0;
				}
				num5 += (int)num3 + (int)num4;
				if (flag3)
				{
					int needExp = StrengthenMgr.getNeedExp(strengthenExp, strengthenLevel);
					num = CalculatorCount(needExp, num5);
					if (num > itemAt.Count)
					{
						num = itemAt.Count;
					}
					num2 += (double)(num5 * num);
				}
				else
				{
					num2 += (double)num5;
				}
			}
			stringBuilder.Append("true");
			bool flag5 = false;
			int num8 = (int)num2 + strengthenExp;
			if (StrengthenMgr.canUpLv(num8, strengthenLevel))
			{
				itemInfo.IsBinds = flag;
				itemInfo.StrengthenLevel++;
				itemInfo.StrengthenExp = num8 - StrengthenMgr.FindStrengthenExpInfo(strengthenLevel + 1).Exp;
				gSPacketIn.WriteByte(1);
				flag5 = true;
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
				itemInfo.StrengthenExp = num8;
			}
			client.Player.StoreBag.RemoveCountFromStack(itemAt, num);
			client.Player.StoreBag.UpdateItem(itemInfo);
			client.Player.OnItemStrengthen(itemInfo.Template.CategoryID, itemInfo.StrengthenLevel);
			gSPacketIn.WriteBoolean(val: false);
			client.Out.SendTCP(gSPacketIn);
			if (flag5 && itemInfo.StrengthenLevel > 9 && itemInfo.ItemID > 0)
			{
				string translation = LanguageMgr.GetTranslation("ItemStrengthenHandler.congratulation", client.Player.PlayerCharacter.NickName, "@", itemInfo.StrengthenLevel);
				WorldMgr.SendSysNotice(eMessageType.ChatNormal, translation, itemInfo.ItemID, itemInfo.TemplateID, "");
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

	private int CalculatorCount(int Exp, int step)
	{
		int num = 1;
		for (int i = step; i < Exp; i += step)
		{
			num++;
		}
		return num;
	}
}
