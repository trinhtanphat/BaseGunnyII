using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Buffer;
using Game.Server.Managers;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(145, "场景用户离开")]
public class ActiveSystemHandler : IPacketHandler
{
	public static ThreadSafeRandom random = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		GSPacketIn gSPacketIn = new GSPacketIn(145, client.Player.PlayerCharacter.ID);
		BaseChristmasRoom christmasRoom = RoomMgr.ChristmasRoom;
		UserChristmasInfo christmas = client.Player.Actives.Christmas;
		bool flag = false;
		if (b <= 10)
		{
			flag = client.Player.Actives.LoadPyramid();
		}
		PyramidInfo pyramid = client.Player.Actives.Pyramid;
		string title = "Event Noel";
		switch (b)
		{
		case 1:
			if (flag && pyramid != null)
			{
				gSPacketIn.WriteByte(1);
				gSPacketIn.WriteInt(pyramid.currentLayer);
				gSPacketIn.WriteInt(pyramid.maxLayer);
				gSPacketIn.WriteInt(pyramid.totalPoint);
				gSPacketIn.WriteInt(pyramid.turnPoint);
				gSPacketIn.WriteInt(pyramid.pointRatio);
				gSPacketIn.WriteInt(pyramid.currentFreeCount);
				gSPacketIn.WriteInt(pyramid.currentReviveCount);
				gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
				if (pyramid.isPyramidStart)
				{
					string[] lists = pyramid.LayerItems.Split('|');
					int[] array8 = new int[7] { 8, 7, 6, 5, 4, 3, 2 };
					gSPacketIn.WriteInt(array8.Length);
					for (int k = 1; k <= array8.Length; k++)
					{
						string[] layerItems = GetLayerItems(lists, k);
						gSPacketIn.WriteInt(k);
						gSPacketIn.WriteInt(layerItems.Length);
						for (int l = 0; l < layerItems.Length; l++)
						{
							int val5 = int.Parse(layerItems[l].Split('-')[1]);
							int val6 = int.Parse(layerItems[l].Split('-')[2]);
							gSPacketIn.WriteInt(val5);
							gSPacketIn.WriteInt(val6);
						}
					}
				}
				client.Player.SendTCP(gSPacketIn);
				return 0;
			}
			client.Player.SendMessage("Tải dử liệu thất bại. Vui lòng thử lại sau.");
			return 0;
		case 2:
		{
			bool flag6 = (pyramid.isPyramidStart = packet.ReadBoolean());
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteBoolean(flag6);
			if (!flag6)
			{
				pyramid.totalPoint += pyramid.totalPoint * pyramid.pointRatio / 100;
				pyramid.totalPoint += pyramid.turnPoint;
				pyramid.turnPoint = 0;
				pyramid.pointRatio = 0;
				pyramid.currentLayer = 1;
				pyramid.currentReviveCount = 0;
				pyramid.LayerItems = "";
				gSPacketIn.WriteInt(pyramid.totalPoint);
				gSPacketIn.WriteInt(pyramid.turnPoint);
				gSPacketIn.WriteInt(pyramid.pointRatio);
				gSPacketIn.WriteInt(pyramid.currentLayer);
			}
			client.Player.SendTCP(gSPacketIn);
			return 0;
		}
		case 3:
		{
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			if (pyramid.currentFreeCount < client.Player.Actives.PyramidConfig.freeCount)
			{
				pyramid.currentFreeCount++;
			}
			else
			{
				int turnCardPrice = client.Player.Actives.PyramidConfig.turnCardPrice;
				if (!client.Player.MoneyDirect(turnCardPrice))
				{
					return 1;
				}
			}
			bool flag3 = true;
			string msg;
			if (num2 < 8)
			{
				List<ItemInfo> pyramidAward = ActiveSystemMgr.GetPyramidAward(num2);
				int index = random.Next(pyramidAward.Count);
				ItemInfo itemInfo = pyramidAward[index];
				int templateID = itemInfo.TemplateID;
				bool val = templateID == 201083;
				bool flag4 = templateID == 201082;
				msg = $"Bạn nhận được {itemInfo.Template.Name} x{itemInfo.Count}.";
				if (flag4)
				{
					msg = "Thật may bạn được lên tầng tiếp theo.";
					pyramid.currentLayer++;
					if (pyramid.currentLayer > pyramid.maxLayer)
					{
						pyramid.maxLayer++;
					}
					flag3 = false;
				}
				pyramid.totalPoint += 10;
				switch (templateID)
				{
				case 201077:
					pyramid.pointRatio += 5;
					msg = "Thật may bạn nhận thêm 5% điểm tích lũy.";
					flag3 = false;
					break;
				case 201078:
					pyramid.pointRatio += 10;
					msg = "Thật may bạn nhận thêm 10% điểm tích lũy.";
					flag3 = false;
					break;
				case 201079:
					pyramid.turnPoint += 10;
					msg = "Thật may bạn nhận thêm 10 điểm tích lũy.";
					flag3 = false;
					break;
				case 201080:
					pyramid.turnPoint += 30;
					msg = "Thật may bạn nhận thêm 30 điểm tích lũy.";
					flag3 = false;
					break;
				case 201081:
					pyramid.turnPoint += 50;
					msg = "Thật may bạn nhận thêm 40 điểm tích lũy.";
					flag3 = false;
					break;
				}
				if (flag3)
				{
					client.Player.AddTemplate(itemInfo);
				}
				string text = $"{num2}-{templateID}-{num3}";
				if (pyramid.LayerItems == "")
				{
					pyramid.LayerItems = text;
				}
				else
				{
					PyramidInfo pyramidInfo = pyramid;
					pyramidInfo.LayerItems = pyramidInfo.LayerItems + "|" + text;
				}
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(templateID);
				gSPacketIn.WriteInt(num3);
				gSPacketIn.WriteBoolean(val);
				gSPacketIn.WriteBoolean(flag4);
				gSPacketIn.WriteInt(pyramid.currentLayer);
				gSPacketIn.WriteInt(pyramid.maxLayer);
				gSPacketIn.WriteInt(pyramid.totalPoint);
				gSPacketIn.WriteInt(pyramid.turnPoint);
				gSPacketIn.WriteInt(pyramid.pointRatio);
				gSPacketIn.WriteInt(pyramid.currentFreeCount);
				client.Player.SendTCP(gSPacketIn);
			}
			else
			{
				int num4 = random.Next(49, 501);
				pyramid.turnPoint += num4;
				msg = $"Thật may bạn nhận thêm {num4} điểm tích lũy.";
				pyramid.isPyramidStart = false;
				pyramid.currentLayer = 1;
				pyramid.currentReviveCount = 0;
				pyramid.totalPoint += pyramid.totalPoint * pyramid.pointRatio / 100;
				pyramid.totalPoint += pyramid.turnPoint;
				pyramid.turnPoint = 0;
				pyramid.pointRatio = 0;
				gSPacketIn.WriteByte(2);
				gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
				gSPacketIn.WriteInt(pyramid.totalPoint);
				gSPacketIn.WriteInt(pyramid.turnPoint);
				gSPacketIn.WriteInt(pyramid.pointRatio);
				gSPacketIn.WriteInt(pyramid.currentLayer);
				client.Player.SendTCP(gSPacketIn);
			}
			client.Player.SendMessage(msg);
			return 0;
		}
		case 4:
		{
			if (!packet.ReadBoolean())
			{
				pyramid.isPyramidStart = false;
				pyramid.currentLayer = 1;
				pyramid.currentReviveCount = 0;
				pyramid.totalPoint += pyramid.totalPoint * pyramid.pointRatio / 100;
				pyramid.totalPoint += pyramid.turnPoint;
				pyramid.turnPoint = 0;
				pyramid.pointRatio = 0;
				pyramid.LayerItems = "";
				gSPacketIn.WriteByte(4);
				gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
				gSPacketIn.WriteInt(pyramid.currentLayer);
				gSPacketIn.WriteInt(pyramid.totalPoint);
				gSPacketIn.WriteInt(pyramid.turnPoint);
				gSPacketIn.WriteInt(pyramid.pointRatio);
				gSPacketIn.WriteInt(pyramid.currentReviveCount);
				client.Player.SendTCP(gSPacketIn);
				return 0;
			}
			int value = client.Player.Actives.PyramidConfig.revivePrice[pyramid.currentReviveCount];
			if (!client.Player.MoneyDirect(value))
			{
				return 1;
			}
			pyramid.currentReviveCount++;
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteBoolean(pyramid.isPyramidStart);
			gSPacketIn.WriteInt(pyramid.currentLayer);
			gSPacketIn.WriteInt(pyramid.totalPoint);
			gSPacketIn.WriteInt(pyramid.turnPoint);
			gSPacketIn.WriteInt(pyramid.pointRatio);
			gSPacketIn.WriteInt(pyramid.currentReviveCount);
			client.Player.SendTCP(gSPacketIn);
			return 0;
		}
		case 8:
			gSPacketIn.WriteByte(8);
			gSPacketIn.WriteInt(1);
			return 0;
		case 17:
			switch (packet.ReadByte())
			{
			case 2:
			{
				int x = packet.ReadInt();
				int y = packet.ReadInt();
				client.Player.X = x;
				client.Player.Y = y;
				if (client.Player.CurrentRoom != null)
				{
					client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
				}
				christmasRoom.AddMoreMonters();
				christmasRoom.SetMonterDie(client.Player.PlayerCharacter.ID);
				if (!client.Player.Actives.AvailTime())
				{
					client.Player.SendMessage("Hết thời gian, bạn phải trả phí để tiếp tục.");
					return 0;
				}
				gSPacketIn.WriteByte(22);
				gSPacketIn.WriteByte(0);
				gSPacketIn.WriteInt(christmasRoom.Monters.Count);
				foreach (MonterInfo value2 in christmasRoom.Monters.Values)
				{
					gSPacketIn.WriteInt(value2.ID);
					gSPacketIn.WriteInt(value2.type);
					gSPacketIn.WriteInt(value2.state);
					gSPacketIn.WriteInt(value2.MonsterPos.X);
					gSPacketIn.WriteInt(value2.MonsterPos.Y);
				}
				client.Out.SendTCP(gSPacketIn);
				christmasRoom.ViewOtherPlayerRoom(client.Player);
				return 0;
			}
			case 0:
			{
				client.Player.X = christmasRoom.DefaultPosX;
				client.Player.Y = christmasRoom.DefaultPosY;
				christmasRoom.AddPlayer(client.Player);
				int christmasMinute = GameProperties.ChristmasMinute;
				if (!christmas.isEnter)
				{
					christmas.gameBeginTime = DateTime.Now;
					christmas.gameEndTime = DateTime.Now.AddMinutes(christmasMinute);
					christmas.isEnter = true;
					christmas.AvailTime = christmasMinute;
				}
				else
				{
					christmasMinute = christmas.AvailTime;
					christmas.gameBeginTime = DateTime.Now;
					christmas.gameEndTime = DateTime.Now.AddMinutes(christmasMinute);
				}
				bool val4 = client.Player.Actives.AvailTime();
				gSPacketIn.WriteByte(17);
				gSPacketIn.WriteBoolean(val4);
				gSPacketIn.WriteDateTime(christmas.gameBeginTime);
				gSPacketIn.WriteDateTime(christmas.gameEndTime);
				gSPacketIn.WriteInt(christmas.count);
				client.Out.SendTCP(gSPacketIn);
				return 0;
			}
			case 1:
				christmasRoom.RemovePlayer(client.Player);
				return 0;
			default:
				return 0;
			}
		case 21:
		{
			int num8 = packet.ReadInt();
			int num9 = packet.ReadInt();
			string str = packet.ReadString();
			client.Player.X = num8;
			client.Player.Y = num9;
			gSPacketIn.WriteByte(21);
			gSPacketIn.WriteInt(client.Player.PlayerId);
			gSPacketIn.WriteInt(num8);
			gSPacketIn.WriteInt(num9);
			gSPacketIn.WriteString(str);
			christmasRoom.SendToALL(gSPacketIn);
			return 0;
		}
		case 22:
		{
			int num11 = packet.ReadInt();
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			if (!client.Player.Actives.AvailTime())
			{
				client.Player.SendMessage("Hết thời gian, bạn phải trả phí để tiếp tục.");
				return 0;
			}
			if (christmasRoom.SetFightMonter(num11, client.Player.PlayerCharacter.ID) && christmasRoom.Monters.ContainsKey(num11))
			{
				MonterInfo monterInfo = christmasRoom.Monters[num11];
				gSPacketIn.WriteByte(22);
				gSPacketIn.WriteByte(3);
				gSPacketIn.WriteInt(num11);
				gSPacketIn.WriteInt(monterInfo.state);
				christmasRoom.SendToALL(gSPacketIn);
				RoomMgr.CreateRoom(client.Player, "Christmas", "Christmas", eRoomType.Christmas, 3);
				return 0;
			}
			return 0;
		}
		case 24:
			gSPacketIn.WriteByte(24);
			gSPacketIn.WriteInt(christmas.count);
			gSPacketIn.WriteInt(christmas.exp);
			gSPacketIn.WriteInt(christmas.awardState);
			gSPacketIn.WriteInt(christmas.packsNumber);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		case 25:
		{
			int templateId = 201144;
			int num5 = packet.ReadInt();
			bool flag5 = packet.ReadBoolean();
			int itemCount = client.Player.GetItemCount(templateId);
			if (num5 > itemCount)
			{
				client.Player.SendMessage("Số lượng ma, thao tác thất bại.");
				return 0;
			}
			if (num5 > 5)
			{
				num5 = 5;
			}
			bool val2 = false;
			int num6 = num5;
			int val3 = 0;
			int num7 = 10;
			if (flag5 && client.Player.MoneyDirect(GameProperties.ChristmasBuildSnowmanDoubleMoney))
			{
				num6 = num5 * 2;
			}
			christmas.exp += num6;
			if (christmas.exp >= num7)
			{
				christmas.exp -= num7;
				val2 = true;
				christmas.count++;
				val3 = 1;
			}
			client.Player.RemoveTemplate(templateId, num5);
			gSPacketIn.WriteByte(25);
			gSPacketIn.WriteBoolean(val2);
			gSPacketIn.WriteInt(christmas.count);
			gSPacketIn.WriteInt(christmas.exp);
			gSPacketIn.WriteInt(num6);
			gSPacketIn.WriteInt(val3);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		case 26:
		{
			int num12 = packet.ReadInt();
			if (christmas.packsNumber >= GameProperties.ChristmasGiftsMaxNum - 1)
			{
				client.Player.SendMessage("Số lần nhận thưởng đã hết.");
				return 0;
			}
			string[] array5 = GameProperties.ChristmasGifts.Split('|');
			string text2 = "";
			int num13 = array5.Length;
			string[] array6 = array5;
			foreach (string text3 in array6)
			{
				if (text3.Split(',')[0] == num12.ToString())
				{
					text2 = text3;
					break;
				}
			}
			if (!(text2 != ""))
			{
				client.Player.SendMessage("Không đủ điều kiện, thao tác thất bại.");
				return 0;
			}
			int num14 = int.Parse(text2.Split(',')[1]);
			if (christmas.packsNumber >= num13 - 2)
			{
				int num15 = int.Parse(array5[num13 - 2].Split(',')[1]);
				num14 = num15 + num14 * (christmas.packsNumber + 1);
			}
			if (num14 <= christmas.count)
			{
				christmas.packsNumber++;
				christmas.awardState |= 1 << christmas.packsNumber;
				client.Player.SendMessage("Nhận thưởng thành công.");
				client.Player.SendItemToMail(num12, "", title);
				if (christmas.packsNumber == num13 - 2 && christmas.count < christmas.lastPacks)
				{
					christmas.count += int.Parse(array5[num13 - 1].Split(',')[1]) * christmas.packsNumber;
				}
				gSPacketIn.WriteByte(26);
				gSPacketIn.WriteInt(christmas.awardState);
				gSPacketIn.WriteInt(christmas.packsNumber);
				gSPacketIn.WriteInt(num12);
				client.Out.SendTCP(gSPacketIn);
				return 0;
			}
			client.Player.SendMessage("Không đủ người tuyết, thao tác thất bại.");
			return 0;
		}
		case 27:
		{
			byte b2 = packet.ReadByte();
			int[] array4 = new int[2] { 201146, 201147 };
			int num10 = random.Next(array4.Length);
			if (b2 == 1 && christmas.dayPacks < 2)
			{
				christmas.dayPacks++;
				client.Player.SendMessage("Nhận thưởng thành công.");
				client.Player.SendItemToMail(array4[num10], "", title);
				return 0;
			}
			if (christmas.count < 3)
			{
				client.Player.SendMessage("Tích lũy 3 người tuyết để nhận quà");
				return 0;
			}
			gSPacketIn.WriteByte(27);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteInt(christmas.dayPacks);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		case 29:
		{
			int christmasBuyTimeMoney = GameProperties.ChristmasBuyTimeMoney;
			if (!client.Player.MoneyDirect(christmasBuyTimeMoney))
			{
				return 1;
			}
			int christmasBuyMinute = GameProperties.ChristmasBuyMinute;
			client.Player.Actives.AddTime(christmasBuyMinute);
			client.Player.SendMessage("Thao tác thành công!");
			return 0;
		}
		case 33:
		{
			client.Player.Actives.YearMonterValidate();
			gSPacketIn.WriteByte(33);
			gSPacketIn.WriteInt(client.Player.Actives.Info.ChallengeNum);
			gSPacketIn.WriteInt(client.Player.Actives.Info.BuyBuffNum);
			gSPacketIn.WriteInt(GameProperties.YearMonsterBuffMoney);
			gSPacketIn.WriteInt(client.Player.Actives.Info.DamageNum);
			string[] array = GameProperties.YearMonsterBoxInfo.Split('|');
			gSPacketIn.WriteInt(array.Length);
			for (int i = 0; i < array.Length; i++)
			{
				string[] array2 = array[i].Split(',');
				string[] array3 = client.Player.Actives.Info.BoxState.Split('-');
				gSPacketIn.WriteInt(int.Parse(array2[0]));
				gSPacketIn.WriteInt(int.Parse(array2[1]) * 10000);
				gSPacketIn.WriteInt(int.Parse(array3[i]));
			}
			client.Out.SendTCP(gSPacketIn);
			return 0;
		}
		case 34:
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			if (client.Player.Actives.Info.ChallengeNum > 0)
			{
				client.Player.Actives.Info.ChallengeNum--;
				RoomMgr.CreateCatchBeastRoom(client.Player);
				return 0;
			}
			return 0;
		case 35:
		{
			packet.ReadBoolean();
			if (!client.Player.MoneyDirect(GameProperties.YearMonsterBuffMoney))
			{
				return 0;
			}
			if (client.Player.Actives.Info.BuyBuffNum > 0)
			{
				client.Player.Actives.Info.BuyBuffNum--;
			}
			gSPacketIn.WriteByte(35);
			gSPacketIn.WriteInt(client.Player.Actives.Info.BuyBuffNum);
			client.Out.SendTCP(gSPacketIn);
			client.Player.SendMessage("Thao tác thành công.");
			BufferList.CreatePayBuffer(400, 30000, 1)?.Start(client.Player);
			AbstractBuffer abstractBuffer = BufferList.CreatePayBuffer(406, 30000, 1);
			if (abstractBuffer != null)
			{
				abstractBuffer.Start(client.Player);
				return 0;
			}
			return 0;
		}
		case 36:
		{
			int num16 = packet.ReadInt();
			string[] array7 = GameProperties.YearMonsterBoxInfo.Split('|');
			bool flag8 = CanGetGift(client.Player.Actives.Info.DamageNum, num16, array7);
			if (flag8)
			{
				int dateId = int.Parse(array7[num16].Split(',')[0]);
				gSPacketIn.WriteByte(36);
				gSPacketIn.WriteBoolean(flag8);
				gSPacketIn.WriteInt(num16);
				client.Out.SendTCP(gSPacketIn);
				client.Player.Actives.SetYearMonterBoxState(num16);
				List<ItemInfo> list = new List<ItemInfo>();
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
				ItemBoxMgr.CreateItemBox(dateId, list, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge);
				StringBuilder stringBuilder = new StringBuilder();
				foreach (ItemInfo item in list)
				{
					stringBuilder.Append(item.Template.Name + " x" + item.Count + ", ");
				}
				client.Out.SendMessage(eMessageType.Normal, stringBuilder.ToString());
				client.Player.AddTemplate(list);
				return 0;
			}
			return 0;
		}
		case 38:
		{
			client.Player.Actives.StopLightriddleTimer();
			LanternriddlesInfo lanternriddlesInfo = ActiveSystemMgr.EnterLanternriddles(client.Player.PlayerCharacter.ID);
			client.Player.Actives.SendLightriddleQuestion(lanternriddlesInfo);
			if (lanternriddlesInfo.CanNextQuest)
			{
				client.Player.Actives.BeginLightriddleTimer();
				return 0;
			}
			return 0;
		}
		case 40:
		{
			packet.ReadInt();
			packet.ReadInt();
			int option = packet.ReadInt();
			ActiveSystemMgr.LanternriddlesAnswer(client.Player.PlayerCharacter.ID, option);
			return 0;
		}
		case 41:
		{
			packet.ReadInt();
			packet.ReadInt();
			int num = packet.ReadInt();
			packet.ReadBoolean();
			LanternriddlesInfo lanternriddles = ActiveSystemMgr.GetLanternriddles(client.Player.PlayerCharacter.ID);
			if (lanternriddles == null)
			{
				client.Player.SendMessage("Dữ liệu server lổi.");
				return 0;
			}
			bool flag2 = false;
			if (num == 0)
			{
				if (lanternriddles.HitFreeCount > 0)
				{
					lanternriddles.HitFreeCount--;
					lanternriddles.IsHint = true;
					flag2 = true;
				}
				else
				{
					int hitPrice = lanternriddles.HitPrice;
					if (client.Player.ActiveMoneyEnable(hitPrice))
					{
						lanternriddles.IsHint = true;
						flag2 = true;
					}
				}
			}
			else if (lanternriddles.DoubleFreeCount > 0)
			{
				lanternriddles.DoubleFreeCount--;
				lanternriddles.IsDouble = true;
				flag2 = true;
			}
			else
			{
				int hitPrice = lanternriddles.DoublePrice;
				if (client.Player.ActiveMoneyEnable(hitPrice))
				{
					lanternriddles.IsDouble = true;
					flag2 = true;
				}
			}
			if (flag2)
			{
				gSPacketIn.WriteByte(41);
				gSPacketIn.WriteBoolean(flag2);
				client.Out.SendTCP(gSPacketIn);
				client.Player.SendMessage("Thao tác thành công.");
				return 0;
			}
			return 0;
		}
		case 42:
			GameServer.Instance.LoginServer.SendLightriddleRank(client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ID);
			return 0;
		case 49:
			gSPacketIn.WriteByte(49);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
			gSPacketIn.WriteBoolean(client.Player.PlayerCharacter.typeVIP == 1);
			gSPacketIn.WriteBoolean(client.Player.PlayerCharacter.Sex);
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteByte((byte)client.Player.PlayerCharacter.Grade);
			gSPacketIn.WriteByte(32);
			gSPacketIn.WriteByte(16);
			gSPacketIn.WriteByte(8);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteByte(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteDateTime(DateTime.Now.AddDays(7.0));
			gSPacketIn.WriteInt(1);
			client.Out.SendTCP(gSPacketIn);
			return 0;
		default:
			Console.WriteLine("activeSystem_cmd: " + b);
			return 0;
		}
	}

	private bool CanGetGift(int damageNum, int id, string[] YearMonsterBoxInfo)
	{
		if (id > 4 || id < 0)
		{
			return false;
		}
		int num = int.Parse(YearMonsterBoxInfo[id].Split(',')[1]) * 10000;
		return num <= damageNum;
	}

	private string[] GetLayerItems(string[] lists, int layer)
	{
		List<string> list = new List<string>();
		foreach (string text in lists)
		{
			string text2 = text.Split('-')[0];
			if (text2 == layer.ToString())
			{
				list.Add(text);
			}
		}
		return list.ToArray();
	}
}
