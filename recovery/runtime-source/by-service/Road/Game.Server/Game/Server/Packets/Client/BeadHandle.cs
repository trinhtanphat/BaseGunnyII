using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(121, "物品镶嵌")]
public class BeadHandle : IPacketHandler
{
	public static ThreadSafeRandom randomExp = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		PlayerInventory inventory = client.Player.GetInventory(eBageType.BeadBag);
		string text = "";
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		switch (b)
		{
		case 1:
		{
			int slot = packet.ReadInt();
			int num9 = packet.ReadInt();
			int needLv = 10;
			if (num9 == -1)
			{
				num9 = inventory.FindFirstEmptySlot();
			}
			if (num9 <= 12 && num9 >= 4 && !canEquip(num9, client.Player.PlayerCharacter.Grade, ref needLv))
			{
				client.Out.SendMessage(eMessageType.Normal, $"Cấp {needLv} mở");
				return 0;
			}
			ItemInfo itemAt3 = inventory.GetItemAt(slot);
			ItemInfo itemAt4 = inventory.GetItemAt(num9);
			if (itemAt3 == null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Sảy ra lổi, chuyển kênh và thử lại!");
				return 0;
			}
			if (slot <= 18 && slot >= 13)
			{
				bool flag3 = false;
				int drillLevel = client.Player.GetDrillLevel(slot);
				if (!JudgeLevel(itemAt3.Hole1, drillLevel))
				{
					flag3 = true;
				}
				if (itemAt4 != null && !JudgeLevel(itemAt4.Hole1, drillLevel))
				{
					flag3 = true;
				}
				if (flag3)
				{
					inventory.UpdateChangedPlaces();
					client.Out.SendMessage(eMessageType.Normal, "Cấp châu báu và cấp lỗ không khớp.");
					return 0;
				}
			}
			if (num9 > 30 && itemAt4 != null)
			{
				RuneTemplateInfo runeTemplateInfo3 = RuneMgr.FindRuneByTemplateID(itemAt4.TemplateID);
				bool flag4 = true;
				if (slot == 1 && !runeTemplateInfo3.IsAttack())
				{
					flag4 = false;
				}
				if ((slot == 2 || slot == 3) && !runeTemplateInfo3.IsDefend())
				{
					flag4 = false;
				}
				if (slot > 3 && slot < 30 && !runeTemplateInfo3.IsProp())
				{
					flag4 = false;
				}
				if (!flag4)
				{
					inventory.UpdateChangedPlaces();
					client.Out.SendMessage(eMessageType.Normal, "Trang bị không phù hợp.");
					return 0;
				}
			}
			if (!inventory.MoveItem(slot, num9, 1))
			{
				client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể di chuyển!");
			}
			client.Player.MainBag.UpdatePlayerProperties();
			return 0;
		}
		case 2:
		{
			ItemInfo itemAt2 = inventory.GetItemAt(31);
			if (itemAt2 == null)
			{
				client.Player.SendMessage("Châu báu không tồn tại. Thao tác thất bại.");
				return 0;
			}
			List<int> list2 = new List<int>();
			int num4 = 0;
			int num5 = packet.ReadInt();
			for (int i = 0; i < num5; i++)
			{
				int num6 = packet.ReadInt();
				ItemInfo itemAt = inventory.GetItemAt(num6);
				if (itemAt != null && !itemAt.IsUsed)
				{
					list2.Add(num6);
					num4 += itemAt.Hole2;
				}
			}
			if (num4 == 0)
			{
				client.Player.SendMessage("Châu báu nâng cấp không đủ. Thao tác thất bại.");
				return 0;
			}
			int hole = itemAt2.Hole2;
			int hole2 = itemAt2.Hole1;
			int num7 = num4 + hole;
			int runeLevel = RuneMgr.GetRuneLevel(num7);
			int num8 = RuneMgr.MaxExp();
			if (num7 > num8)
			{
				num4 = num8;
			}
			itemAt2.Hole2 += num4;
			itemAt2.Hole1 = runeLevel;
			RuneTemplateInfo runeTemplateInfo2 = RuneMgr.FindRuneTemplateID(itemAt2.TemplateID, runeLevel);
			if (runeTemplateInfo2 == null)
			{
				client.Player.SendMessage("Dữ liệu server lổi. Thao tác thất bại.");
				return 0;
			}
			if (runeTemplateInfo2.TemplateID != itemAt2.TemplateID)
			{
				ItemInfo itemInfo2 = new ItemInfo(ItemMgr.FindItemTemplate(runeTemplateInfo2.TemplateID));
				itemAt2.TemplateID = runeTemplateInfo2.TemplateID;
				itemInfo2.Copy(itemAt2);
				inventory.RemoveItemAt(31);
				inventory.AddItemTo(itemInfo2, 31);
			}
			itemAt2.IsBinds = true;
			inventory.UpdateItem(itemAt2);
			inventory.RemoveAllItem(list2);
			inventory.SaveToDatabase();
			return 0;
		}
		case 4:
		{
			int slot = packet.ReadInt();
			ItemInfo itemAt = inventory.GetItemAt(slot);
			if (itemAt == null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Xảy ra lổi, chuyển kênh và thử lại.");
				return 0;
			}
			if (itemAt.IsUsed)
			{
				itemAt.IsUsed = false;
			}
			else
			{
				itemAt.IsUsed = true;
			}
			inventory.UpdateItem(itemAt);
			return 0;
		}
		case 5:
		{
			int num2 = packet.ReadInt();
			int templateId = packet.ReadInt();
			bool flag2 = false;
			PlayerInventory inventory2 = client.Player.GetInventory(eBageType.PropBag);
			inventory2.GetItemByTemplateID(0, templateId);
			int itemCount = inventory2.GetItemCount(templateId);
			if (itemCount <= 0)
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("Mủi khoan không đủ!"));
			}
			else
			{
				int num3 = randomExp.Next(2, 6);
				text = LanguageMgr.GetTranslation("OpenHoleHandler.GetExp", num3);
				UserDrillInfo userDrillInfo = client.Player.UserDrills[num2];
				userDrillInfo.HoleExp += num3;
				if ((userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(0) && userDrillInfo.HoleLv == 0) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(1) && userDrillInfo.HoleLv == 1) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(2) && userDrillInfo.HoleLv == 2) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(3) && userDrillInfo.HoleLv == 3) || (userDrillInfo.HoleExp >= GameProperties.HoleLevelUpExp(4) && userDrillInfo.HoleLv == 4))
				{
					userDrillInfo.HoleLv++;
					userDrillInfo.HoleExp = 0;
					flag2 = true;
				}
				client.Player.UpdateDrill(num2, userDrillInfo);
			}
			if (text != "")
			{
				client.Out.SendMessage(eMessageType.Normal, text);
			}
			client.Player.Out.SendPlayerDrill(client.Player.PlayerCharacter.ID, client.Player.UserDrills);
			inventory2.RemoveTemplate(templateId, 1);
			if (!flag2)
			{
				return 0;
			}
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			foreach (UserDrillInfo value2 in client.Player.UserDrills.Values)
			{
				playerBussiness.UpdateUserDrillInfo(value2);
			}
			return 0;
		}
		default:
			return 0;
		case 3:
		{
			string[] array = GameProperties.OpenRunePackageMoney.Split('|');
			int num = packet.ReadInt();
			packet.ReadBoolean();
			bool flag = true;
			if (DateTime.Compare(client.Player.LastOpenPack.AddSeconds(1.0), DateTime.Now) > 0)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Quá nhiều thao tác!"));
				flag = false;
			}
			int value = Convert.ToInt32(array[num]);
			if (!client.Player.MoneyDirect(value))
			{
				flag = false;
			}
			if (inventory.FindFirstEmptySlot() == -1)
			{
				client.Out.SendMessage(eMessageType.Normal, "Rương đã đầy không thể mở thêm!");
				flag = false;
			}
			if (flag)
			{
				List<ItemInfo> list = client.Player.beadIndex switch
				{
					1 => RuneMgr.OpenPackageLv2(),
					2 => RuneMgr.OpenPackageLv3(),
					3 => RuneMgr.OpenPackageLv4(),
					_ => RuneMgr.OpenPackageLv1(),
				};
				if (list == null)
				{
					client.Player.SendMessage("Sảy ra lổi, chuyển kênh và thử lại.");
					return 0;
				}
				int index = ThreadSafeRandom.NextStatic(list.Count);
				ItemInfo itemInfo = list[index];
				itemInfo.Count = 1;
				inventory.AddItem(itemInfo);
				RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(itemInfo.TemplateID);
				client.Player.SendMessage($"Bạn nhận được {runeTemplateInfo.Name} lv{runeTemplateInfo.BaseLevel}.");
				int rand = NextBeadIndex(client, client.Player.beadIndex);
				BeadIndexUpdate(client, client.Player.beadIndex);
				client.Out.SendRuneOpenPackage(client.Player, rand);
			}
			client.Player.LastOpenPack = DateTime.Now;
			return 0;
		}
		}
	}

	private static bool JudgeLevel(int beadLv, int drillLv)
	{
		switch (drillLv)
		{
		case 1:
			if (beadLv >= 1 && beadLv <= 4)
			{
				return true;
			}
			break;
		case 2:
			if (beadLv >= 1 && beadLv <= 8)
			{
				return true;
			}
			break;
		case 3:
			if (beadLv >= 1 && beadLv <= 12)
			{
				return true;
			}
			break;
		case 4:
			if (beadLv >= 1 && beadLv <= 16)
			{
				return true;
			}
			break;
		case 5:
			return true;
		}
		return false;
	}

	private static bool canEquip(int place, int grade, ref int needLv)
	{
		bool result = true;
		switch (place)
		{
		case 6:
			needLv = 15;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		case 7:
			needLv = 18;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		case 8:
			needLv = 21;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		case 9:
			needLv = 24;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		case 10:
			needLv = 27;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		case 11:
			needLv = 30;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		case 12:
			needLv = 33;
			if (grade < needLv)
			{
				result = false;
			}
			break;
		}
		return result;
	}

	private static int NextBeadIndex(GameClient client, int index)
	{
		if (client.Player.beadRequestBtn1 == 10 && index == 0)
		{
			client.Player.beadIndex = 1;
			return 1;
		}
		if (client.Player.beadRequestBtn2 == 5 && index == 1)
		{
			client.Player.beadIndex = 2;
			return 2;
		}
		if (client.Player.beadRequestBtn3 == 5 && index == 2)
		{
			client.Player.beadIndex = 3;
			return 4;
		}
		client.Player.beadIndex = 0;
		return 0;
	}

	private void BeadIndexUpdate(GameClient client, int index)
	{
		if (index == 0)
		{
			if (client.Player.beadRequestBtn1 > 10)
			{
				client.Player.beadRequestBtn1 = 0;
			}
			client.Player.beadRequestBtn1++;
		}
		if (index == 1)
		{
			client.Player.beadRequestBtn2++;
			client.Player.beadRequestBtn1 = 0;
		}
		if (index == 2)
		{
			client.Player.beadRequestBtn3++;
			client.Player.beadRequestBtn2 = 0;
		}
		if (index == 3)
		{
			client.Player.beadRequestBtn3 = 0;
		}
	}
}
