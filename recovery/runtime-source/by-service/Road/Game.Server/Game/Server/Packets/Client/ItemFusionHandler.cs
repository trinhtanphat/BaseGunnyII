using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Packets.Client;

[PacketHandler(78, "熔化")]
public class ItemFusionHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		int fusionId = packet.ReadInt();
		int num = packet.ReadInt();
		bool isBinds = true;
		StringBuilder stringBuilder = new StringBuilder();
		int validDate = 30;
		FusionInfo fusionInfo = FusionMgr.FindItemFusion(fusionId);
		if (fusionInfo == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 1;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(fusionInfo.Reward);
		if (itemTemplateInfo == null)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 1;
		}
		PlayerEquipInventory mainBag = client.Player.MainBag;
		if (itemTemplateInfo.BagType == eBageType.PropBag)
		{
			PlayerInventory propBag = client.Player.PropBag;
			validDate = 0;
		}
		int num2 = client.Player.MainBag.FindFirstEmptySlot();
		int num3 = client.Player.PropBag.FindFirstEmptySlot();
		if ((num2 == -1 && itemTemplateInfo.BagType == eBageType.MainBag) || (num3 == -1 && itemTemplateInfo.BagType == eBageType.PropBag))
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"));
			return 0;
		}
		int itemCount = client.Player.GetItemCount(fusionInfo.Item1);
		int num4 = itemCount / fusionInfo.Count1;
		if (num > num4)
		{
			num = num4;
		}
		int num5 = fusionInfo.Count1 * num;
		bool flag = true;
		if (num > 1)
		{
			if (itemCount < num5)
			{
				flag = false;
			}
		}
		else if (itemCount < fusionInfo.Count1)
		{
			flag = false;
		}
		if (!flag)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
			return 0;
		}
		int num6 = GameProperties.PRICE_FUSION_GOLD * fusionInfo.Count1 * num;
		if (client.Player.PlayerCharacter.Gold < num6)
		{
			client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("ItemFusionHandler.NoMoney"));
			return 0;
		}
		List<ItemInfo> list = new List<ItemInfo>();
		if (itemTemplateInfo != null)
		{
			client.Player.RemoveGold(num6);
			client.Player.RemoveTemplate(fusionInfo.Item1, num5);
			stringBuilder.Append(itemTemplateInfo.TemplateID + ",");
			ItemInfo itemInfo = null;
			for (int i = 0; i < num; i++)
			{
				if (random.Next(100000) <= fusionInfo.FusionRate)
				{
					ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 105);
					itemInfo2.IsBinds = isBinds;
					itemInfo2.ValidDate = validDate;
					if (itemTemplateInfo.MaxCount == 1)
					{
						list.Add(itemInfo2);
					}
					else if (itemInfo == null)
					{
						itemInfo = itemInfo2;
					}
				}
			}
			if (itemInfo != null)
			{
				itemInfo.Count = num;
				list.Add(itemInfo);
			}
			if (list.Count > 0)
			{
				client.Out.SendFusionResult(client.Player, result: true);
				client.Player.OnItemFusion(fusionInfo.FusionType);
				foreach (ItemInfo item in list)
				{
					client.Player.AddTemplate(item);
				}
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Succeed1", itemTemplateInfo.Name + " x" + num));
			}
			else
			{
				client.Out.SendFusionResult(client.Player, result: false);
				stringBuilder.Append("false");
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.Failed"));
			}
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ItemFusionHandler.NoCondition"));
		}
		return 0;
	}
}
