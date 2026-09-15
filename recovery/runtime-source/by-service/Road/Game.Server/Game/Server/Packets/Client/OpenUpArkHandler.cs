using System.Collections.Generic;
using System.Linq;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameUtils;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Packets.Client;

[PacketHandler(63, "打开物品")]
public class OpenUpArkHandler : IPacketHandler
{
	public static readonly ILog log = LogManager.GetLogger("FlashErrorLogger");

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int bageType = packet.ReadByte();
		int slot = packet.ReadInt();
		int num = packet.ReadInt();
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		string text = "";
		if (itemAt != null && itemAt.IsValidItem() && itemAt.Template.CategoryID == 11 && itemAt.Template.Property1 == 6 && client.Player.PlayerCharacter.Grade >= itemAt.Template.NeedLevel)
		{
			if (num < 1 || num > itemAt.Count)
			{
				num = itemAt.Count;
			}
			int num2 = 0;
			string text2 = "";
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			if (!inventory.RemoveCountFromStack(itemAt, num))
			{
				return 0;
			}
			Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
			stringBuilder2.Append(LanguageMgr.GetTranslation("OpenUpArkHandler.Start"));
			for (int i = 0; i < num; i++)
			{
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
				List<ItemInfo> list = new List<ItemInfo>();
				ItemBoxMgr.CreateItemBox(itemAt.TemplateID, list, ref gold, ref point, ref giftToken, ref medal, ref exp, ref honor, ref hardCurrency, ref leagueMoney, ref useableScore, ref prestge);
				if (point != 0)
				{
					num2 += point;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.Money");
					client.Player.AddMoney(point);
				}
				if (gold != 0)
				{
					num2 += gold;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.Gold");
					client.Player.AddGold(gold);
				}
				if (giftToken != 0)
				{
					num2 += giftToken;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.GiftToken");
					client.Player.AddGiftToken(giftToken);
				}
				if (medal != 0)
				{
					num2 += medal;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.Medal");
					client.Player.AddMedal(medal);
				}
				if (exp != 0)
				{
					num2 += exp;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.Exp");
					client.Player.AddGP(exp);
				}
				if (honor != 0)
				{
					num2 += honor;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.honor");
					client.Player.AddHonor(honor);
				}
				if (hardCurrency != 0)
				{
					num2 += hardCurrency;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.hardCurrency");
					client.Player.AddHardCurrency(hardCurrency);
				}
				if (leagueMoney != 0)
				{
					num2 += leagueMoney;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.leagueMoney");
					client.Player.AddLeagueMoney(leagueMoney);
				}
				if (useableScore != 0)
				{
					num2 += useableScore;
					if (client.Player.Level == LevelMgr.MaxLevel)
					{
						int num3 = num2 / 100;
						if (num3 > 0)
						{
							client.Player.AddOffer(num3);
							text2 = $"Max level khinh nghiệm quy đổi thành {num3} công trạng";
						}
					}
					else
					{
						text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.useableScore");
						client.Player.AddGP(useableScore);
					}
				}
				if (prestge != 0)
				{
					num2 += prestge;
					text2 = LanguageMgr.GetTranslation("OpenUpArkHandler.prestge");
					client.Player.AddGP(prestge);
				}
				foreach (ItemInfo item in list)
				{
					if (!dictionary.Keys.Contains(item.TemplateID))
					{
						dictionary.Add(item.TemplateID, item);
					}
					else
					{
						dictionary[item.TemplateID].Count += item.Count;
					}
				}
			}
			string name = itemAt.Template.Name;
			if (num2 > 0)
			{
				stringBuilder2.Append(num2 + text2);
			}
			if (stringBuilder.Length > 0)
			{
				stringBuilder.Remove(stringBuilder.Length - 1, 1);
				string[] array = stringBuilder.ToString().Split(',');
				for (int j = 0; j < array.Length; j++)
				{
					int num4 = 1;
					for (int k = j + 1; k < array.Length; k++)
					{
						if (array[j].Contains(array[k]) && array[k].Length == array[j].Length)
						{
							num4++;
							array[k] = k.ToString();
						}
					}
					if (num4 > 1)
					{
						array[j] = array[j].Remove(array[j].Length - 1, 1);
						string[] array3;
						string[] array2 = (array3 = array);
						int num5 = j;
						nint num6 = num5;
						array2[num5] = array3[num6] + num4;
					}
					if (array[j] != j.ToString())
					{
						string[] array3;
						string[] array4 = (array3 = array);
						int num7 = j;
						nint num6 = num7;
						array4[num7] = array3[num6] + ",";
						stringBuilder2.Append(array[j]);
					}
				}
			}
			stringBuilder2.Remove(stringBuilder2.Length - 1, 1);
			stringBuilder2.Append(".");
			if (num2 > 0)
			{
				client.Out.SendMessage(eMessageType.Normal, text + stringBuilder2.ToString());
			}
			else
			{
				GSPacketIn gSPacketIn = new GSPacketIn(63, client.Player.PlayerCharacter.ID);
				gSPacketIn.WriteString(name);
				gSPacketIn.WriteByte((byte)dictionary.Count);
				foreach (ItemInfo value in dictionary.Values)
				{
					gSPacketIn.WriteInt(value.TemplateID);
					gSPacketIn.WriteInt(value.Count);
					gSPacketIn.WriteBoolean(value.IsBinds);
					gSPacketIn.WriteInt(value.ValidDate);
					gSPacketIn.WriteInt(value.StrengthenLevel);
					gSPacketIn.WriteInt(value.AttackCompose);
					gSPacketIn.WriteInt(value.DefendCompose);
					gSPacketIn.WriteInt(value.AgilityCompose);
					gSPacketIn.WriteInt(value.LuckCompose);
					stringBuilder.Append(value.Template.Name + "x" + value.Count + ",");
					client.Player.AddTemplate(value);
				}
				client.Player.SendTCP(gSPacketIn);
			}
		}
		return 1;
	}
}
