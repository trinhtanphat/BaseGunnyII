using System;
using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(81, "游戏创建")]
public class EnterFarmHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		if (client.Player.PlayerCharacter.Grade < 25)
		{
			client.Out.SendMessage(eMessageType.Normal, "Cấp chưa đủ không thể vào!");
			return 0;
		}
		switch (b)
		{
		case 1:
		{
			int num9 = packet.ReadInt();
			if (num9 != client.Player.PlayerCharacter.ID)
			{
				client.Player.Farm.EnterFriendFarm(num9);
				break;
			}
			client.Player.Farm.EnterFarm();
			if (client.Player.PlayerCharacter.IsFistGetPet)
			{
				client.Player.PetBag.ClearAdoptPets();
				List<UsersPetinfo> list2 = PetMgr.CreateFirstAdoptList(num9, client.Player.Level);
				foreach (UsersPetinfo item2 in list2)
				{
					client.Player.PetBag.AddAdoptPetTo(item2, item2.Place);
				}
				client.Player.RemoveFistGetPet();
			}
			else
			{
				if (!(client.Player.PlayerCharacter.LastRefreshPet.Date < DateTime.Now.Date))
				{
					break;
				}
				client.Player.PetBag.ClearAdoptPets();
				List<UsersPetinfo> list3 = PetMgr.CreateAdoptList(num9, client.Player.Level);
				foreach (UsersPetinfo item3 in list3)
				{
					client.Player.PetBag.AddAdoptPetTo(item3, item3.Place);
				}
				client.Player.RemoveLastRefreshPet();
			}
			break;
		}
		case 2:
		{
			packet.ReadByte();
			int num8 = packet.ReadInt();
			int fieldId3 = packet.ReadInt();
			if (client.Player.Farm.GrowField(fieldId3, num8))
			{
				client.Player.FarmBag.RemoveTemplate(num8, 1);
				client.Player.OnSeedFoodPetEvent();
			}
			break;
		}
		case 4:
		{
			int num5 = packet.ReadInt();
			int fieldId = packet.ReadInt();
			string message2 = "Thu hoạch thất bại!";
			if (num5 != client.Player.PlayerCharacter.ID)
			{
				message2 = ((!client.Player.Farm.GainFriendFields(num5, fieldId)) ? "Không thể chộm nửa." : "Thao tác thành công.");
			}
			else if (client.Player.Farm.GainField(fieldId))
			{
				message2 = "Thu hoạch thành công!";
			}
			client.Player.Out.SendMessage(eMessageType.Normal, message2);
			break;
		}
		case 6:
		{
			string message3 = "Mở rộng thành công!";
			List<int> list = new List<int>();
			int num6 = packet.ReadInt();
			for (int i = 0; i < num6; i++)
			{
				int item = packet.ReadInt();
				list.Add(item);
			}
			int num7 = packet.ReadInt();
			PlayerFarm farm = client.Player.Farm;
			int value = ((farm.payFieldTimeToMonth() != num7) ? (num6 * farm.payFieldMoneyToWeek()) : (num6 * farm.payFieldMoneyToMonth()));
			if (client.Player.MoneyDirect(value))
			{
				farm.PayField(list, num7);
				client.Out.SendMessage(eMessageType.Normal, message3);
			}
			break;
		}
		case 7:
		{
			int fieldId2 = packet.ReadInt();
			client.Player.Farm.killCropField(fieldId2);
			break;
		}
		case 9:
		{
			string message = "Kích hoạt trợ thủ thất bại!";
			bool flag = packet.ReadBoolean();
			int num = packet.ReadInt();
			int seedTime = packet.ReadInt();
			int num2 = packet.ReadInt();
			int getCount = packet.ReadInt();
			int num3 = packet.ReadInt();
			int num4 = packet.ReadInt();
			bool flag2 = false;
			if (flag)
			{
				if (client.Player.MoneyDirect(num4) && num3 == -1)
				{
					flag2 = true;
				}
				else if (client.Player.PlayerCharacter.GiftToken < num4 || num3 != -2)
				{
					message = ((num3 != -1) ? "Xu khóa không đủ!" : "Xu không đủ!");
				}
				else
				{
					client.Player.RemoveGiftToken(num4);
					flag2 = true;
				}
			}
			else
			{
				message = "Hủy trợ thủ thành công!";
				client.Player.Farm.CropHelperSwitchField(isStopFarmHelper: true);
			}
			if (flag2)
			{
				message = "Kích hoạt trợ thủ thành công!";
				client.Player.Farm.HelperSwitchField(flag, num, seedTime, num2, getCount);
				client.Player.FarmBag.RemoveTemplate(num, num2);
			}
			client.Out.SendMessage(eMessageType.Normal, message);
			break;
		}
		case 16:
			client.Player.Farm.ExitFarm();
			break;
		default:
			Console.WriteLine("FarmPackageType." + (FarmPackageType)b);
			break;
		}
		client.Player.Farm.SaveToDatabase();
		return 0;
	}
}
