using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(61, "物品转移")]
public class ItemTransferHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(61, client.Player.PlayerCharacter.ID);
		new StringBuilder();
		int num = 40000;
		bool tranHole = packet.ReadBoolean();
		bool tranHoleFivSix = packet.ReadBoolean();
		ItemInfo itemZero = client.Player.StoreBag.GetItemAt(0);
		ItemInfo itemOne = client.Player.StoreBag.GetItemAt(1);
		if (itemZero != null && itemOne != null && itemZero.Template.CategoryID == itemOne.Template.CategoryID && itemOne.Count == 1 && itemZero.Count == 1)
		{
			if (client.Player.PlayerCharacter.Gold < num)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nogold"));
				return 1;
			}
			client.Player.RemoveGold(num);
			StrengthenMgr.InheritTransferProperty(ref itemZero, ref itemOne, tranHole, tranHoleFivSix);
			if (StrengthenMgr.TransferCondition(itemOne, itemZero))
			{
				int num2 = OrginWeaponID(itemZero);
				int num3 = OrginWeaponID(itemOne);
				if (num2 > 0)
				{
					itemZero.TemplateID = num2;
					num2 = GainWeaponID(itemZero);
				}
				if (num3 > 0)
				{
					itemOne.TemplateID = num3;
					num3 = GainWeaponID(itemOne);
				}
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(num2);
				ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(num3);
				if (itemTemplateInfo != null && itemTemplateInfo2 != null)
				{
					ItemInfo itemInfo = ItemInfo.CloneFromTemplate(itemTemplateInfo, itemZero);
					if (itemInfo.IsGold)
					{
						GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipNewTemplate(itemTemplateInfo.TemplateID);
						if (goldEquipTemplateLoadInfo != null)
						{
							ItemTemplateInfo itemTemplateInfo3 = ItemMgr.FindItemTemplate(goldEquipTemplateLoadInfo.NewTemplateId);
							if (itemTemplateInfo3 != null)
							{
								itemInfo.GoldEquip = itemTemplateInfo3;
							}
						}
					}
					itemZero = itemInfo;
					ItemInfo itemInfo2 = ItemInfo.CloneFromTemplate(itemTemplateInfo2, itemOne);
					if (itemInfo2.IsGold)
					{
						GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo2 = GoldEquipMgr.FindGoldEquipNewTemplate(itemTemplateInfo2.TemplateID);
						if (goldEquipTemplateLoadInfo2 != null)
						{
							ItemTemplateInfo itemTemplateInfo4 = ItemMgr.FindItemTemplate(goldEquipTemplateLoadInfo2.NewTemplateId);
							if (itemTemplateInfo4 != null)
							{
								itemInfo2.GoldEquip = itemTemplateInfo4;
							}
						}
					}
					itemOne = itemInfo2;
				}
				client.Player.StoreBag.ClearBag();
				client.Player.StoreBag.AddItemTo(itemZero, 0);
				client.Player.StoreBag.AddItemTo(itemOne, 1);
			}
			else
			{
				client.Player.StoreBag.UpdateItem(itemZero);
				client.Player.StoreBag.UpdateItem(itemOne);
			}
			gSPacketIn.WriteByte(0);
			client.Out.SendTCP(gSPacketIn);
		}
		else
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("itemtransferhandler.nocondition"));
		}
		return 0;
	}

	private int OrginWeaponID(ItemInfo item)
	{
		StrengthenGoodsInfo strengthenGoodsInfo = StrengthenMgr.FindTransferInfo(item.TemplateID);
		if (strengthenGoodsInfo == null)
		{
			GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipOldTemplate(item.TemplateID);
			if (goldEquipTemplateLoadInfo == null)
			{
				return 0;
			}
			strengthenGoodsInfo = StrengthenMgr.FindTransferInfo(goldEquipTemplateLoadInfo.OldTemplateId);
		}
		return strengthenGoodsInfo.OrginEquip;
	}

	private int GainWeaponID(ItemInfo item)
	{
		return item.StrengthenLevel switch
		{
			10 => StrengthenMgr.FindTransferInfo(10, item.TemplateID)?.GainEquip ?? (-1),
			11 => StrengthenMgr.FindTransferInfo(11, item.TemplateID)?.GainEquip ?? (-1),
			12 => StrengthenMgr.FindTransferInfo(12, item.TemplateID)?.GainEquip ?? (-1),
			13 => StrengthenMgr.FindTransferInfo(13, item.TemplateID)?.GainEquip ?? (-1),
			14 => StrengthenMgr.FindTransferInfo(14, item.TemplateID)?.GainEquip ?? (-1),
			15 => StrengthenMgr.FindTransferInfo(15, item.TemplateID)?.GainEquip ?? (-1),
			_ => StrengthenMgr.FindTransferInfo(item.TemplateID)?.OrginEquip ?? (-1),
		};
	}
}
