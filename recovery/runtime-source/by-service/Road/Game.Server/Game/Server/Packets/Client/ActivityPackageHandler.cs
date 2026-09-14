using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(84, "场景用户离开")]
public class ActivityPackageHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int iD = client.Player.PlayerCharacter.ID;
		ActiveSystemInfo info = client.Player.Actives.Info;
		if (num == 1)
		{
			int num2 = num;
			if (num2 == 1)
			{
				int availTime = info.AvailTime;
				if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
					return 0;
				}
				int promotePackagePrice = GameProperties.PromotePackagePrice;
				if (client.Player.MoneyDirect(promotePackagePrice))
				{
					string translation = LanguageMgr.GetTranslation("UserBuyItemHandler.Success");
					if (CanGetAward(client.Player.PlayerCharacter.Grade))
					{
						info.AvailTime++;
						client.Player.Out.SendGrowthPackageOpen(client.Player.PlayerCharacter.ID, info.AvailTime);
						int num3 = (client.Player.PlayerCharacter.Sex ? 1 : 2);
						List<ActivitySystemItemInfo> list = ActiveSystemMgr.FindGrowthPackage(availTime);
						List<ItemInfo> list2 = new List<ItemInfo>();
						foreach (ActivitySystemItemInfo item in list)
						{
							ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.TemplateID);
							if (itemTemplateInfo != null && (itemTemplateInfo.NeedSex == 0 || itemTemplateInfo.NeedSex == num3))
							{
								ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, item.Count, 102);
								itemInfo.Count = item.Count;
								itemInfo.IsBinds = item.IsBinds;
								itemInfo.ValidDate = item.ValidDate;
								itemInfo.StrengthenLevel = item.StrengthenLevel;
								itemInfo.AttackCompose = item.AttackCompose;
								itemInfo.DefendCompose = item.DefendCompose;
								itemInfo.AgilityCompose = item.AgilityCompose;
								itemInfo.LuckCompose = item.LuckCompose;
								list2.Add(itemInfo);
							}
						}
						string text = "";
						foreach (ItemInfo item2 in list2)
						{
							text += ((text == "") ? item2.TemplateID.ToString() : ("," + item2.TemplateID));
							client.Player.AddTemplate(item2);
						}
						client.Out.SendMessage(eMessageType.Normal, translation);
					}
					else
					{
						client.Player.Out.SendGrowthPackageOpen(client.Player.PlayerCharacter.ID, info.AvailTime);
						client.Out.SendMessage(eMessageType.Normal, "Cấp độ chưa đủ, thao tác thất bại.");
					}
				}
			}
		}
		return 0;
	}

	private bool CanGetAward(int grace)
	{
		int[] array = new int[9] { 10, 20, 30, 40, 45, 50, 55, 60, 65 };
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] >= grace)
			{
				return true;
			}
		}
		return false;
	}
}
