using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(183, "卡片使用")]
public class CardUseHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int num2 = packet.ReadInt();
		ItemInfo itemInfo = null;
		ShopItemInfo shopItemInfo = new ShopItemInfo();
		List<ItemInfo> list = new List<ItemInfo>();
		if (DateTime.Compare(client.Player.LastOpenCard.AddSeconds(1.0), DateTime.Now) > 0)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Quá nhiều thao tác!"));
			return 0;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		if (num == -1 && num2 == -1)
		{
			int num3 = packet.ReadInt();
			int templatID = packet.ReadInt();
			packet.ReadInt();
			int num4 = 0;
			int value = 0;
			for (int i = 0; i < num3; i++)
			{
				shopItemInfo = ShopMgr.FindShopbyTemplateID(templatID);
				if (shopItemInfo != null)
				{
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfo.TemplateID);
					itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
					value = shopItemInfo.AValue1;
					itemInfo.ValidDate = shopItemInfo.AUnit;
				}
				if (itemInfo != null)
				{
					if (num4 <= client.Player.PlayerCharacter.Gold && client.Player.MoneyDirect(value))
					{
						client.Player.RemoveGold(num4);
						list.Add(itemInfo);
					}
				}
				else
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail"));
				}
			}
		}
		else
		{
			PlayerInventory inventory = client.Player.GetInventory((eBageType)num);
			itemInfo = inventory.GetItemAt(num2);
			if (itemInfo != null)
			{
				list.Add(itemInfo);
			}
			string translateId = "CardUseHandler.Success";
			if (list.Count > 0)
			{
				string translateId2 = string.Empty;
				foreach (ItemInfo item in list)
				{
					if (item.Template.Property1 == 13 || item.Template.Property1 == 11 || item.Template.Property1 == 12 || item.Template.Property1 == 26)
					{
						AbstractBuffer abstractBuffer = BufferList.CreateBuffer(item.Template, item.ValidDate);
						if (abstractBuffer != null)
						{
							abstractBuffer.Start(client.Player);
							if (num2 != -1 && num != -1)
							{
								inventory = client.Player.GetInventory((eBageType)num);
								inventory.RemoveCountFromStack(item, 1);
							}
						}
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
						continue;
					}
					if (item.Template.Property5 == 3)
					{
						if (item.ValidDate != 30)
						{
							item.ValidDate = item.Template.Property5 * 10;
						}
						AbstractBuffer abstractBuffer2 = BufferList.CreateBufferMinutes(item.Template, item.ValidDate);
						if (abstractBuffer2 != null)
						{
							abstractBuffer2.Start(client.Player);
							if (num2 != -1 && num != -1)
							{
								inventory = client.Player.GetInventory((eBageType)num);
								inventory.RemoveCountFromStack(item, 1);
							}
						}
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId));
						continue;
					}
					if (item.IsValidItem() && item.Template.Property1 == 21)
					{
						if (item.TemplateID == 201145)
						{
							int min = item.Template.Property2 * itemInfo.Count;
							client.Player.Actives.AddTime(min);
							translateId2 = "TimerDanUser.Success";
						}
						else
						{
							int num5 = item.Template.Property2 * itemInfo.Count;
							if (client.Player.Level == LevelMgr.MaxLevel)
							{
								int num6 = num5 / 100;
								if (num6 > 0)
								{
									client.Player.AddOffer(num6);
									translateId2 = $"Max level khinh nghiệm quy đổi thành {num6} công trạng";
								}
							}
							else
							{
								client.Player.AddGP(num5);
								translateId2 = "GPDanUser.Success";
							}
						}
						if (item.Template.CanDelete)
						{
							client.Player.RemoveAt((eBageType)num, num2);
						}
					}
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation(translateId2, itemInfo.Template.Property2 * itemInfo.Count));
				}
			}
			else
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("CardUseHandler.Fail"));
			}
		}
		client.Player.LastOpenCard = DateTime.Now;
		return 0;
	}
}
