using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(31, "场景用户离开")]
public class GoodsExchangeHandler : IPacketHandler
{
	private int GetGoodsAward(int index)
	{
		return index switch
		{
			1 => 3,
			2 => 5,
			3 => 7,
			_ => 1,
		};
	}

	private List<ActiveConvertItemInfo> GetActiveConvertItem(ActiveBussiness db, int id, int index, int lengh)
	{
		ActiveConvertItemInfo[] singleActiveConvertItems = db.GetSingleActiveConvertItems(id);
		List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
		ActiveConvertItemInfo[] array = singleActiveConvertItems;
		foreach (ActiveConvertItemInfo activeConvertItemInfo in array)
		{
			if (activeConvertItemInfo.ItemType == GetGoodsAward(index))
			{
				for (int j = 0; j < lengh; j++)
				{
					list.Add(activeConvertItemInfo);
				}
			}
		}
		return list;
	}

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		using (ActiveBussiness activeBussiness = new ActiveBussiness())
		{
			int num = packet.ReadInt();
			packet.ReadInt();
			int num2 = packet.ReadInt();
			int i = 0;
			ActiveInfo singleActives = activeBussiness.GetSingleActives(num);
			int num3 = Convert.ToInt32(singleActives.GoodsExchangeNum);
			ItemInfo item = null;
			PlayerInventory playerInventory = null;
			for (; i < num2; i++)
			{
				int templateId = packet.ReadInt();
				packet.ReadInt();
				int bageType = packet.ReadInt();
				playerInventory = client.Player.GetInventory((eBageType)bageType);
				item = playerInventory.GetItemByTemplateID(0, templateId);
				int itemCount = playerInventory.GetItemCount(templateId);
				if (itemCount < num3 || itemCount < 0)
				{
					client.Out.SendMessage(eMessageType.Normal, "vật phẩm không đủ!");
					break;
				}
			}
			int index = packet.ReadInt();
			StringBuilder stringBuilder = new StringBuilder();
			List<ActiveConvertItemInfo> activeConvertItem = GetActiveConvertItem(activeBussiness, num, index, num2);
			foreach (ActiveConvertItemInfo item2 in activeConvertItem)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item2.TemplateID);
				if (itemTemplateInfo == null)
				{
					client.Out.SendMessage(eMessageType.Normal, "Lổi phần thưởng liên hệ admin!");
					break;
				}
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
				itemInfo.IsBinds = item2.IsBind;
				client.Player.AddTemplate(itemInfo, itemInfo.Template.BagType, item2.ItemCount, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView);
				stringBuilder.Append(itemTemplateInfo.Name + " x" + item2.ItemCount + ". ");
			}
			playerInventory.RemoveCountFromStack(item, num3 * num2);
			if (stringBuilder.Length > 0)
			{
				client.Out.SendMessage(eMessageType.Normal, stringBuilder.ToString());
			}
		}
		return 0;
	}
}
