using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(26, "打开物品")]
public class LotteryOpenBoxHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		new ProduceBussiness();
		if (client.Lottery != -1)
		{
			client.Out.SendMessage(eMessageType.Normal, "Rương đang hoạt động!");
			return 1;
		}
		int num = packet.ReadByte();
		int num2 = packet.ReadInt();
		int num3 = packet.ReadInt();
		List<ItemInfo> list = new List<ItemInfo>();
		PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
		string name = client.Player.PlayerCharacter.NickName;
		if (inventory.FindFirstEmptySlot() == -1)
		{
			client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể mở thêm!");
			return 1;
		}
		int num4 = 11456;
		ItemInfo itemByTemplateID = client.Player.GetItemByTemplateID(num4);
		ItemInfo itemByTemplateID2 = client.Player.GetItemByTemplateID(num3);
		if (itemByTemplateID2 == null || itemByTemplateID2.Count < 1)
		{
			Console.WriteLine("eBageType.{0} slot {1} templateID {2}", (eBageType)num, num2, num3);
			return 1;
		}
		if (itemByTemplateID2 == null)
		{
			list = client.Player.CaddyBag.GetItems();
			client.Out.SendTCP(caddygetaward(list, name));
			return 0;
		}
		if (itemByTemplateID2.Count < 1)
		{
			list = client.Player.CaddyBag.GetItems();
			client.Out.SendTCP(caddygetaward(list, name));
			return 0;
		}
		List<ItemInfo> list2 = new List<ItemInfo>();
		StringBuilder stringBuilder = new StringBuilder();
		int point = 0;
		int gold = 0;
		int giftToken = 0;
		int medal = 0;
		int exp = 0;
		int honor = 0;
		int hardCurrency = 0;
		int leagueMoney = 0;
		int useableScore = 0;
		int prestge = 0;
		if (!ItemBoxMgr.CreateItemBox(itemByTemplateID2.TemplateID, list2, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge))
		{
			client.Player.SendMessage("Sảy ra lổi hảy thử lại sau.");
			return 0;
		}
		if (point != 0)
		{
			stringBuilder.Append(point + LanguageMgr.GetTranslation("OpenUpArkHandler.Money"));
			client.Player.AddMoney(point);
		}
		if (gold != 0)
		{
			stringBuilder.Append(gold + LanguageMgr.GetTranslation("OpenUpArkHandler.Gold"));
			client.Player.AddGold(gold);
		}
		if (giftToken != 0)
		{
			stringBuilder.Append(giftToken + LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken"));
			client.Player.AddGiftToken(giftToken);
		}
		if (medal != 0)
		{
			stringBuilder.Append(medal + LanguageMgr.GetTranslation("OpenUpArkHandler.Medal"));
			client.Player.AddMedal(medal);
		}
		if (exp != 0)
		{
			stringBuilder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.Exp"));
			client.Player.AddGP(exp);
		}
		if (honor != 0)
		{
			stringBuilder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.honor"));
			client.Player.AddHonor(honor);
		}
		if (hardCurrency != 0)
		{
			stringBuilder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.hardCurrency"));
			client.Player.AddHardCurrency(hardCurrency);
		}
		if (leagueMoney != 0)
		{
			stringBuilder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.leagueMoney"));
			client.Player.AddLeagueMoney(leagueMoney);
		}
		if (useableScore != 0)
		{
			stringBuilder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.useableScore"));
			client.Player.AddGP(useableScore);
		}
		if (prestge != 0)
		{
			stringBuilder.Append(exp + LanguageMgr.GetTranslation("OpenUpArkHandler.prestge"));
			client.Player.AddGP(prestge);
		}
		int index = ThreadSafeRandom.NextStatic(list2.Count);
		ItemInfo itemInfo = list2[index];
		if (itemInfo != null)
		{
			switch (num3)
			{
			default:
				name = itemInfo.Template.Name;
				break;
			case 112047:
			case 112100:
			case 112101:
				if (itemByTemplateID.Count < 4)
				{
					client.Player.Disconnect();
					return 1;
				}
				client.Player.RemoveTemplate(num4, 4);
				break;
			}
			inventory.AddItem(itemInfo);
			stringBuilder.Append(itemInfo.Template.Name);
			client.Player.RemoveTemplate(num3, 1);
		}
		list = client.Player.CaddyBag.GetItems();
		client.Out.SendTCP(caddygetaward(list, name));
		client.Lottery = -1;
		if (stringBuilder != null)
		{
			client.Out.SendMessage(eMessageType.Normal, "Bạn nhận được " + stringBuilder.ToString());
		}
		return 1;
	}

	private GSPacketIn caddygetaward(List<ItemInfo> list, string name)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(245);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteInt(list.Count);
		foreach (ItemInfo item in list)
		{
			gSPacketIn.WriteString(name);
			gSPacketIn.WriteInt(item.TemplateID);
			gSPacketIn.WriteInt(item.Count);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteBoolean(val: false);
		}
		return gSPacketIn;
	}
}
