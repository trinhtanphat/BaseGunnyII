using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(68, "添加好友")]
public class PetHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		string msg = "Xu không đủ!";
		int num = -1;
		PetInventory petBag = client.Player.PetBag;
		int num2 = ((client.Player.Level > 60) ? 60 : client.Player.Level);
		switch (b)
		{
		case 1:
			UpdatePetHandle(client, packet.ReadInt());
			break;
		case 2:
		{
			int place = packet.ReadInt();
			int bagType = packet.ReadInt();
			int iD3 = client.Player.PlayerCharacter.ID;
			int num4 = petBag.FindFirstEmptySlot();
			if (client.Player.PlayerCharacter.Grade < 25)
			{
				client.Player.SendMessage("Level chưa đủ, không thể mở.");
				return 0;
			}
			if (num4 == -1)
			{
				client.Player.SendMessage("Số lượng pet đã đạt giới hạn!");
				break;
			}
			ItemInfo itemAt = client.Player.GetItemAt((eBageType)bagType, place);
			PetTemplateInfo petTemplateInfo = PetMgr.FindPetTemplate(itemAt.Template.Property5);
			if (petTemplateInfo == null)
			{
				client.Player.SendMessage("Dữ liệu server lổi.");
				return 0;
			}
			UsersPetinfo usersPetinfo2 = PetMgr.CreatePet(petTemplateInfo, iD3, num4, num2);
			usersPetinfo2.IsExit = true;
			petBag.AddPetTo(usersPetinfo2, num4);
			client.Player.RemoveItem(itemAt);
			if (petTemplateInfo.StarLevel > 3)
			{
				string msg2 = $"Người chơi [{client.Player.PlayerCharacter.NickName}] thật ghê gớm mở trứng được {petTemplateInfo.Name} {petTemplateInfo.StarLevel} sao.";
				GSPacketIn packet2 = WorldMgr.SendSysNotice(msg2);
				GameServer.Instance.LoginServer.SendPacket(packet2);
			}
			else
			{
				client.Player.SendMessage($"Bạn nhận được {petTemplateInfo.Name} {petTemplateInfo.StarLevel} sao");
			}
			petBag.SaveToDatabase(saveAdopt: false);
			break;
		}
		case 4:
		{
			int place = packet.ReadInt();
			int bagType = packet.ReadInt();
			num = packet.ReadInt();
			bool flag4 = false;
			ItemInfo itemAt3 = client.Player.GetItemAt((eBageType)bagType, place);
			if (itemAt3 == null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Xảy ra lổi, chuyển kênh và thử lại.");
				return 0;
			}
			int num8 = Convert.ToInt32(PetMgr.FindConfig("MaxHunger").Value);
			UsersPetinfo petAt4 = petBag.GetPetAt(num);
			int num9 = itemAt3.Count;
			int property = itemAt3.Template.Property2;
			int property2 = itemAt3.Template.Property1;
			int num10 = num9 * property2;
			int num11 = num10 + petAt4.Hunger;
			int num12 = num9 * property;
			msg = "";
			if (itemAt3.TemplateID == 334100)
			{
				num12 = itemAt3.DefendCompose;
			}
			if (petAt4.Level < num2)
			{
				num12 += petAt4.GP;
				int level = petAt4.Level;
				int level2 = PetMgr.GetLevel(num12, num2);
				int gP = PetMgr.GetGP(level2 + 1, num2);
				int gP2 = PetMgr.GetGP(num2, num2);
				int num13 = num12;
				if (num12 > gP2)
				{
					num12 -= gP2;
					if (num12 >= property && property != 0)
					{
						num9 -= (int)Math.Ceiling((double)num12 / (double)property);
					}
				}
				petAt4.GP = ((num13 >= gP2) ? gP2 : num13);
				petAt4.Level = level2;
				petAt4.MaxGP = ((gP == 0) ? gP2 : gP);
				petAt4.Hunger = ((num11 > num8) ? num8 : num11);
				flag4 = petBag.UpGracePet(petAt4, num, isUpdateProp: true, level, level2, num2, ref msg);
				if (itemAt3.TemplateID == 334100)
				{
					client.Player.StoreBag.RemoveItem(itemAt3);
				}
				else
				{
					client.Player.StoreBag.RemoveCountFromStack(itemAt3, num9);
					client.Player.OnUsingItem(itemAt3.TemplateID);
				}
			}
			else if (petAt4.Hunger < num8)
			{
				petAt4.Hunger = num11;
				client.Player.StoreBag.RemoveCountFromStack(itemAt3, num9);
				flag4 = petBag.UpGracePet(petAt4, num, isUpdateProp: false, 0, 0, num2, ref msg);
				msg = "Ðộ vui vẻ tang thêm " + num10;
			}
			else
			{
				msg = "Ðộ vui vui đã đạt mức tối da";
			}
			if (flag4)
			{
				petBag.SaveToDatabase(saveAdopt: false);
			}
			if (!string.IsNullOrEmpty(msg))
			{
				client.Player.SendMessage(msg);
			}
			break;
		}
		case 5:
		{
			bool flag3 = packet.ReadBoolean();
			int num7 = Convert.ToInt32(PetMgr.FindConfig("AdoptRefereshCost").Value);
			int templateId = Convert.ToInt32(PetMgr.FindConfig("FreeRefereshID").Value);
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
			if (flag3)
			{
				if (!client.Player.MoneyDirect(num7))
				{
					return 0;
				}
				if (itemByTemplateID != null)
				{
					client.Player.PropBag.RemoveTemplate(templateId, 1);
				}
				else
				{
					client.Player.AddPetScore(num7 / 100);
				}
				List<UsersPetinfo> list = PetMgr.CreateAdoptList(client.Player.PlayerCharacter.ID, num2);
				client.Player.PetBag.ClearAdoptPets();
				foreach (UsersPetinfo item in list)
				{
					client.Player.PetBag.AddAdoptPetTo(item, item.Place);
				}
			}
			client.Player.Out.SendRefreshPet(client.Player, client.Player.PetBag.GetAdoptPet(), null, flag3);
			break;
		}
		case 6:
		{
			num = packet.ReadInt();
			int num6 = petBag.FindFirstEmptySlot();
			if (num6 == -1)
			{
				client.Out.SendRefreshPet(client.Player, petBag.GetAdoptPet(), null, refreshBtn: false);
				client.Player.SendMessage("Số lượng pet đã đạt giới hạn!");
				break;
			}
			if (num < 0)
			{
				client.Out.SendRefreshPet(client.Player, petBag.GetAdoptPet(), null, refreshBtn: false);
				client.Player.SendMessage("Không tìm thấy pet này!");
				return 0;
			}
			UsersPetinfo adoptPetAt = petBag.GetAdoptPetAt(num);
			using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
			{
				if (adoptPetAt.ID > 0)
				{
					playerBussiness2.RemoveUserAdoptPet(adoptPetAt.ID);
					adoptPetAt.ID = 0;
				}
			}
			petBag.RemoveAdoptPet(adoptPetAt);
			if (petBag.AddPetTo(adoptPetAt, num6))
			{
				PetTemplateInfo petTemplateInfo2 = PetMgr.FindPetTemplate(adoptPetAt.TemplateID);
				if (petTemplateInfo2.StarLevel > 3)
				{
					string msg3 = $"Người chơi [{client.Player.PlayerCharacter.NickName}] may mắn bắt được {petTemplateInfo2.Name} {petTemplateInfo2.StarLevel} sao.";
					GSPacketIn packet3 = WorldMgr.SendSysNotice(msg3);
					GameServer.Instance.LoginServer.SendPacket(packet3);
				}
				client.Player.OnAdoptPetEvent();
			}
			petBag.SaveToDatabase(saveAdopt: false);
			break;
		}
		case 7:
		{
			num = packet.ReadInt();
			int killId = packet.ReadInt();
			int killindex = packet.ReadInt();
			if (!petBag.EquipSkillPet(num, killId, killindex))
			{
				client.Player.SendMessage("Skill này đã trang bị!");
			}
			break;
		}
		case 8:
		{
			num = packet.ReadInt();
			UsersPetinfo petAt5 = petBag.GetPetAt(num);
			if (petBag.RemovePet(petAt5))
			{
				using PlayerBussiness playerBussiness3 = new PlayerBussiness();
				playerBussiness3.UpdateUserAdoptPet(petAt5.ID);
			}
			client.Player.SendMessage("Thả pet thành công!");
			petBag.SaveToDatabase(saveAdopt: false);
			break;
		}
		case 9:
		{
			num = packet.ReadInt();
			string name = packet.ReadString();
			int value2 = Convert.ToInt32(PetMgr.FindConfig("ChangeNameCost").Value);
			if (client.Player.MoneyDirect(value2))
			{
				if (petBag.RenamePet(num, name))
				{
					msg = "Đổi tên thành công!";
				}
				client.Player.SendMessage(msg);
			}
			break;
		}
		case 17:
		{
			num = packet.ReadInt();
			bool isEquip = packet.ReadBoolean();
			UsersPetinfo petAt3 = petBag.GetPetAt(num);
			if (petAt3 == null)
			{
				return 0;
			}
			if (petAt3.Level > num2 && !petAt3.IsEquip)
			{
				client.Player.SendMessage("Level pet quá cao, không thể sử dụng!");
				return 0;
			}
			if (petBag.EquipPet(num, isEquip))
			{
				client.Player.MainBag.UpdatePlayerProperties();
			}
			else
			{
				client.Player.SendMessage("Độ no bằng không, không thể chiến đấu!");
			}
			break;
		}
		case 18:
		{
			num = packet.ReadInt();
			int value = Convert.ToInt32(PetMgr.FindConfig("RecycleCost").Value);
			if (client.Player.MoneyDirect(value))
			{
				UsersPetinfo petAt2 = client.Player.PetBag.GetPetAt(num);
				if (petAt2 == null)
				{
					return 0;
				}
				UsersPetinfo usersPetinfo = new UsersPetinfo();
				int iD = petAt2.ID;
				using (PlayerBussiness playerBussiness = new PlayerBussiness())
				{
					usersPetinfo = playerBussiness.GetAdoptPetSingle(iD);
				}
				if (usersPetinfo == null)
				{
					client.Player.SendMessage("Phục hồi thất bại!");
					return 0;
				}
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(334100);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
				itemInfo.IsBinds = true;
				itemInfo.DefendCompose = petAt2.GP;
				itemInfo.AgilityCompose = petAt2.MaxGP;
				if (!client.Player.PropBag.AddTemplate(itemInfo, 1))
				{
					client.Player.SendItemToMail(itemInfo, LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), LanguageMgr.GetTranslation("UserChangeItemPlaceHandler.full"), eMailType.ItemOverdue);
					client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				petAt2.Blood = usersPetinfo.Blood;
				petAt2.Attack = usersPetinfo.Attack;
				petAt2.Defence = usersPetinfo.Defence;
				petAt2.Agility = usersPetinfo.Agility;
				petAt2.Luck = usersPetinfo.Luck;
				int iD2 = client.Player.PlayerCharacter.ID;
				int templateID = usersPetinfo.TemplateID;
				petAt2.TemplateID = templateID;
				petAt2.Skill = usersPetinfo.Skill;
				petAt2.SkillEquip = usersPetinfo.SkillEquip;
				petAt2.GP = 0;
				petAt2.Level = 1;
				petAt2.MaxGP = 55;
				List<PetEquipDataInfo> equip = petAt2.GetEquip();
				client.Player.PetBag.MoveEqAllToBag(equip);
				petAt2.EquipList = client.Player.PetBag.RemoveEq(equip);
				if (client.Player.PetBag.UpGracePet(petAt2, num, isUpdateProp: false, 0, 0, num2, ref msg))
				{
					client.Player.SendMessage("Phục hồi thành công!");
				}
			}
			petBag.SaveToDatabase(saveAdopt: false);
			break;
		}
		case 19:
		{
			bool flag = packet.ReadBoolean();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				return 0;
			}
			UserFarmInfo currentFarm = client.Player.Farm.CurrentFarm;
			int buyExpRemainNum = currentFarm.buyExpRemainNum;
			PetExpItemPriceInfo petExpItemPriceInfo = PetMgr.FindPetExpItemPrice(RealMoney(buyExpRemainNum));
			if (petExpItemPriceInfo == null || buyExpRemainNum < 1)
			{
				return 0;
			}
			bool flag2 = false;
			int money = petExpItemPriceInfo.Money;
			if (flag)
			{
				if (client.Player.MoneyDirect(money))
				{
					client.Player.AddExpVip(money);
					flag2 = true;
				}
			}
			else if (money <= client.Player.PlayerCharacter.GiftToken)
			{
				client.Player.RemoveGiftToken(money);
				flag2 = true;
			}
			if (!flag2)
			{
				if (GameProperties.IsDDTMoneyActive)
				{
					client.Player.SendMessage("Xu khóa không đủ. Thao tác thất bại.");
				}
				else
				{
					client.Player.SendMessage("Xu không đủ. Thao tác thất bại.");
				}
				return 0;
			}
			ItemTemplateInfo goods2 = ItemMgr.FindItemTemplate(334102);
			ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(goods2, petExpItemPriceInfo.ItemCount, 102);
			itemInfo2.IsBinds = true;
			client.Player.AddTemplate(itemInfo2, itemInfo2.Template.BagType, petExpItemPriceInfo.ItemCount, eItemNotice.NoneTypeView, eItemNotice.GoodsTipTypeView);
			currentFarm.buyExpRemainNum--;
			GSPacketIn gSPacketIn = new GSPacketIn(68);
			gSPacketIn.WriteByte(19);
			gSPacketIn.WriteInt(currentFarm.buyExpRemainNum);
			client.SendTCP(gSPacketIn);
			client.Player.Farm.UpdateFarm(currentFarm);
			break;
		}
		case 20:
		{
			int bagType2 = packet.ReadInt();
			num = packet.ReadInt();
			int num5 = packet.ReadInt();
			if (petBag.GetPetAt(num5) == null)
			{
				return 0;
			}
			ItemInfo itemAt2 = client.Player.GetItemAt((eBageType)bagType2, num);
			if (itemAt2 != null && itemAt2.IsEquipPet())
			{
				if (petBag.MoveEqFromBag(num5, itemAt2.eqType(), itemAt2))
				{
					client.Player.RemoveAt((eBageType)bagType2, num);
					client.Player.MainBag.UpdatePlayerProperties();
				}
				else
				{
					client.Player.SendMessage("Cấp không đủ, trang bị thất bại!");
				}
			}
			else
			{
				client.Player.RemoveAt((eBageType)bagType2, num);
			}
			break;
		}
		case 21:
		{
			int num3 = packet.ReadInt();
			num = packet.ReadInt();
			UsersPetinfo petAt = petBag.GetPetAt(num3);
			if (petAt == null)
			{
				return 0;
			}
			if (num > petAt.EquipList.Count)
			{
				return 0;
			}
			PetEquipDataInfo petEquipDataInfo = petBag.GetPetAt(num3).EquipList[num];
			if (petEquipDataInfo == null)
			{
				return 0;
			}
			petBag.MoveEqToBag(petEquipDataInfo);
			petEquipDataInfo.eqTemplateID = -1;
			petEquipDataInfo.ValidDate = 7;
			petEquipDataInfo = petEquipDataInfo.addTempalte(null);
			petBag.UpdateQPet(num3, num, petEquipDataInfo);
			client.Player.MainBag.UpdatePlayerProperties();
			break;
		}
		}
		return 0;
	}

	private void UpdatePetHandle(GameClient client, int ID)
	{
		GamePlayer playerById = WorldMgr.GetPlayerById(ID);
		PlayerInfo playerInfo;
		UsersPetinfo[] array;
		if (playerById != null)
		{
			playerInfo = playerById.PlayerCharacter;
			array = playerById.PetBag.GetPets();
		}
		else
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerInfo = playerBussiness.GetUserSingleByUserID(ID);
			array = playerBussiness.GetUserPetSingles(ID);
			PetEquipDataInfo[] eqPetSingles = playerBussiness.GetEqPetSingles(ID);
			for (int i = 0; i < array.Length; i++)
			{
				array[i].EquipList = GetPetEquip(array[i].ID, eqPetSingles);
			}
		}
		if (array != null && playerInfo != null)
		{
			client.Out.SendPetInfo(playerInfo, array);
		}
	}

	private List<PetEquipDataInfo> GetPetEquip(int petID, PetEquipDataInfo[] eqs)
	{
		List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
		foreach (PetEquipDataInfo petEquipDataInfo in eqs)
		{
			if (petID == petEquipDataInfo.PetID)
			{
				list.Add(petEquipDataInfo);
			}
		}
		return list;
	}

	private int RealMoney(int timebuy)
	{
		return timebuy switch
		{
			1 => 20,
			2 => 19,
			3 => 18,
			4 => 17,
			5 => 16,
			6 => 15,
			7 => 14,
			8 => 13,
			9 => 12,
			10 => 11,
			11 => 10,
			12 => 9,
			13 => 8,
			14 => 7,
			15 => 6,
			16 => 5,
			17 => 4,
			18 => 3,
			19 => 2,
			_ => 1,
		};
	}
}
