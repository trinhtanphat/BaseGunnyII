using System.Collections.Generic;
using System.Linq;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(100, "客户端日记")]
public class DragonBoatHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(100);
		ActiveSystemInfo info = client.Player.Actives.Info;
		switch (b)
		{
		case 2:
		{
			byte b2 = packet.ReadByte();
			int num2 = packet.ReadInt();
			if (num2 <= 0)
			{
				return 0;
			}
			CommunalActiveInfo communalActiveInfo = CommunalActiveMgr.FindCommunalActive(1);
			if (communalActiveInfo != null && communalActiveInfo.LimitGrade > client.Player.PlayerCharacter.Grade)
			{
				client.Out.SendMessage(eMessageType.Normal, $"Cấp {communalActiveInfo.LimitGrade} mới được tham gia đóng thuyền.");
				return 0;
			}
			if (ActiveSystemMgr.periodType == 2)
			{
				client.Out.SendMessage(eMessageType.Normal, "Hoạt động đã kết thúc.");
				return 0;
			}
			if (info.useableScore >= int.MaxValue)
			{
				client.Out.SendMessage(eMessageType.Normal, "Điểm tích lũy đã đạt giới hạn. Thao tác thất bại.");
				return 0;
			}
			int dragonBoatProp = GameProperties.DragonBoatProp;
			int itemCount = client.Player.GetItemCount(dragonBoatProp);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(dragonBoatProp);
			string[] array = GameProperties.DragonBoatByMoney.Split(',');
			int num3 = int.Parse(array[1]);
			int num4 = int.Parse(array[0].Split(':')[0]);
			int num5 = num3 * num2;
			bool flag = false;
			byte b3 = b2;
			if (b3 == 1)
			{
				array = GameProperties.DragonBoatByProps.Split(',');
				num3 = int.Parse(array[1]);
				num4 = int.Parse(array[0].Split(':')[0]);
				num5 = num3 * num2;
				if (itemCount < num2)
				{
					if (itemTemplateInfo != null)
					{
						client.Player.SendMessage($"{itemTemplateInfo.Name} không đủ.");
					}
					return 0;
				}
				int count = num2 * num4;
				flag = client.Player.RemoveTemplate(dragonBoatProp, count);
			}
			else
			{
				int value = num2 * num4;
				if (client.Player.ActiveMoneyEnable(value))
				{
					flag = true;
				}
			}
			if (flag)
			{
				int templateId = 201031;
				int itemCount2 = client.Player.GetItemCount(templateId);
				ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(templateId);
				if (itemTemplateInfo2 != null && itemCount2 > 0)
				{
					num5 += itemCount2 * itemTemplateInfo2.Property1;
					client.Player.RemoveTemplate(templateId, itemCount2);
				}
				info.useableScore += num5;
				if (info.useableScore == int.MinValue)
				{
					info.useableScore = int.MaxValue;
				}
				if (info.dayScore <= GameProperties.DragonBoatMaxScore)
				{
					int num6 = num5 + info.dayScore;
					if (num6 > GameProperties.DragonBoatMaxScore)
					{
						num5 = GameProperties.DragonBoatMaxScore - info.dayScore;
					}
					ActiveSystemMgr.UpdateBoatExp(num5);
					info.totalScore += num5;
					if (info.totalScore == int.MinValue)
					{
						info.totalScore = int.MaxValue;
					}
				}
				client.Player.SendMessage("Thao tác thành công.");
			}
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(info.useableScore);
			gSPacketIn.WriteInt(info.totalScore);
			client.Player.SendTCP(gSPacketIn);
			GSPacketIn gSPacketIn4 = new GSPacketIn(100);
			gSPacketIn4.WriteByte(3);
			gSPacketIn4.WriteInt(ActiveSystemMgr.boatCompleteExp);
			client.Player.SendTCP(gSPacketIn4);
			break;
		}
		case 3:
		{
			gSPacketIn.WriteByte(3);
			gSPacketIn.WriteInt(ActiveSystemMgr.boatCompleteExp);
			client.Player.SendTCP(gSPacketIn);
			GSPacketIn gSPacketIn3 = new GSPacketIn(100);
			gSPacketIn3.WriteByte(2);
			gSPacketIn3.WriteInt(info.useableScore);
			gSPacketIn3.WriteInt(info.totalScore);
			client.Player.SendTCP(gSPacketIn3);
			break;
		}
		case 4:
		{
			int iD = packet.ReadInt();
			int num7 = packet.ReadInt();
			if (num7 > 999)
			{
				num7 = 999;
			}
			int gold = 0;
			int money = 0;
			int offer = 0;
			int gifttoken = 0;
			int medal = 0;
			int damageScore = 0;
			int petScore = 0;
			int iTemplateID = 0;
			int iCount = 0;
			int hardCurrency = 0;
			int LeagueMoney = 0;
			int useableScore = 0;
			eMessageType type = eMessageType.Normal;
			string translation = LanguageMgr.GetTranslation("UserBuyItemHandler.Success");
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(iD);
			bool flag2 = false;
			if (shopItemInfoById != null && ShopMgr.IsOnShop(shopItemInfoById.ID) && shopItemInfoById.ShopID == 95)
			{
				flag2 = true;
			}
			if (!flag2)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission"));
				return 1;
			}
			Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
			for (int i = 0; i < num7; i++)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				if (itemInfo != null || shopItemInfoById != null)
				{
					if (shopItemInfoById.BuyType == 0)
					{
						itemInfo.ValidDate = shopItemInfoById.AUnit;
					}
					else
					{
						itemInfo.Count = shopItemInfoById.AUnit;
					}
					itemInfo.IsBinds = true;
					if (!dictionary.Keys.Contains(itemInfo.TemplateID))
					{
						dictionary.Add(itemInfo.TemplateID, itemInfo);
					}
					else
					{
						dictionary[itemInfo.TemplateID].Count += itemInfo.Count;
					}
					ShopMgr.SetItemType(shopItemInfoById, 1, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
				}
			}
			if (dictionary.Count == 0)
			{
				return 1;
			}
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				return 1;
			}
			if (info.useableScore < useableScore)
			{
				client.Player.SendMessage("Tích lũy không đủ.");
				return 0;
			}
			info.useableScore -= useableScore;
			if (true)
			{
				string text = "";
				foreach (ItemInfo value2 in dictionary.Values)
				{
					text += ((text == "") ? value2.TemplateID.ToString() : ("," + value2.TemplateID));
					client.Player.AddTemplate(value2);
				}
			}
			else
			{
				type = eMessageType.ERROR;
				translation = LanguageMgr.GetTranslation("UserBuyItemHandler.FailByPermission");
			}
			client.Player.SaveBagIntoDatabase();
			client.Out.SendMessage(type, LanguageMgr.GetTranslation(translation));
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(info.useableScore);
			gSPacketIn.WriteInt(info.totalScore);
			client.Player.SendTCP(gSPacketIn);
			break;
		}
		case 16:
		{
			int dragonBoatMinScore = GameProperties.DragonBoatMinScore;
			int dragonBoatAreaMinScore = GameProperties.DragonBoatAreaMinScore;
			List<ActiveSystemInfo> list = ActiveSystemMgr.SelectTopTenCurrenServer(10);
			gSPacketIn.WriteByte(16);
			gSPacketIn.WriteInt(list.Count);
			int num = 1;
			foreach (ActiveSystemInfo item in list)
			{
				gSPacketIn.WriteInt(num);
				gSPacketIn.WriteInt(item.totalScore);
				gSPacketIn.WriteString(item.NickName);
				num++;
			}
			gSPacketIn.WriteInt(ActiveSystemMgr.FindMyRank(client.Player.PlayerCharacter.ID));
			gSPacketIn.WriteInt(dragonBoatMinScore);
			client.Player.SendTCP(gSPacketIn);
			list = ActiveSystemMgr.SelectTopTenAllServer(10);
			num = 1;
			GSPacketIn gSPacketIn2 = new GSPacketIn(100);
			gSPacketIn2.WriteByte(17);
			gSPacketIn2.WriteInt(list.Count);
			foreach (ActiveSystemInfo item2 in list)
			{
				gSPacketIn2.WriteInt(num);
				gSPacketIn2.WriteInt(item2.totalScore);
				gSPacketIn2.WriteString(item2.NickName);
				gSPacketIn2.WriteString("Zone No.1");
				num++;
			}
			gSPacketIn2.WriteInt(ActiveSystemMgr.FindAreaMyRank(client.Player.PlayerCharacter.ID));
			gSPacketIn2.WriteInt(dragonBoatAreaMinScore);
			client.Player.SendTCP(gSPacketIn2);
			break;
		}
		}
		return 0;
	}
}
