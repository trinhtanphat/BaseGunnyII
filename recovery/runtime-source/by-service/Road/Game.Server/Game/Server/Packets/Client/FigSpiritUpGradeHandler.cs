using System;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(209, "场景用户离开")]
public class FigSpiritUpGradeHandler : IPacketHandler
{
	private static readonly string[] places = new string[3] { "0", "1", "2" };

	private static readonly int[] exps = new int[6] { 0, 600, 5220, 15840, 39020, 89580 };

	private int getNeedExp(int _curExp, int _curLv)
	{
		int num = exps[_curLv + 1];
		return num - _curExp;
	}

	private bool getMax(string[] SpiritIdValue)
	{
		int num = 0;
		if (SpiritIdValue[0].Split(',')[0] == "5")
		{
			num = 1;
		}
		if (SpiritIdValue[1].Split(',')[0] == "5")
		{
			num = 2;
		}
		if (SpiritIdValue[2].Split(',')[0] == "5")
		{
			num = 3;
		}
		return num == 3;
	}

	private int[] getOldLv(string[] curLvs)
	{
		int[] array = new int[curLvs.Length];
		for (int i = 0; i < curLvs.Length; i++)
		{
			array[i] = Convert.ToInt32(curLvs[i].Split(',')[0]);
		}
		return array;
	}

	private int[] getOldExp(string[] curLvs)
	{
		int[] array = new int[curLvs.Length];
		for (int i = 0; i < curLvs.Length; i++)
		{
			array[i] = Convert.ToInt32(curLvs[i].Split(',')[1]);
		}
		return array;
	}

	private bool canUpLv(int exp, int _curLv)
	{
		return (exp >= exps[1] && _curLv == 0) || (exp >= exps[2] && _curLv == 1) || (exp >= exps[3] && _curLv == 2) || (exp >= exps[4] && _curLv == 3) || (exp >= exps[5] && _curLv == 4);
	}

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		if (client.Player.PlayerCharacter.Grade < 30)
		{
			client.Out.SendMessage(eMessageType.Normal, "Hack Lv àh bạn trẻ!");
			return 0;
		}
		packet.ReadByte();
		int num = packet.ReadInt();
		packet.ReadInt();
		packet.ReadInt();
		int templateId = packet.ReadInt();
		int figSpiritId = packet.ReadInt();
		int place = packet.ReadInt();
		packet.ReadInt();
		packet.ReadInt();
		ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, templateId);
		int itemCount = client.Player.PropBag.GetItemCount(templateId);
		UserGemStone gemStone = client.Player.GetGemStone(place);
		string[] array = gemStone.FigSpiritIdValue.Split('|');
		int iD = client.Player.PlayerCharacter.ID;
		bool flag = false;
		bool max = getMax(array);
		bool isFall = true;
		int num2 = 1;
		int dir = 0;
		int[] oldExp = getOldExp(array);
		int[] oldLv = getOldLv(array);
		if (itemCount <= 0)
		{
			client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, max, isFall, 0, dir);
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Chiến hồn không đủ!"));
			return 0;
		}
		if (!max && itemByTemplateID != null)
		{
			if (num == 0)
			{
				int property = itemByTemplateID.Template.Property2;
				for (int i = 0; i < places.Length; i++)
				{
					if (oldLv[i] < 5)
					{
						oldExp[i] += property;
						flag = canUpLv(oldExp[i], oldLv[i]);
						if (flag)
						{
							oldLv[i]++;
							oldExp[i] = 0;
						}
					}
				}
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
			}
			if (num == 1)
			{
				int num3 = 1;
				for (int j = 0; j < places.Length; j++)
				{
					num3 = getNeedExp(oldExp[j], oldLv[j]) / itemByTemplateID.Template.Property2;
					if (itemCount < num3)
					{
						num3 = itemCount;
					}
					int num4 = itemByTemplateID.Template.Property2 * num3;
					if (oldLv[j] < 5)
					{
						oldExp[j] += num4;
						flag = canUpLv(oldExp[j], oldLv[j]);
						if (flag)
						{
							oldLv[j]++;
							oldExp[j] = 0;
						}
					}
				}
				client.Player.PropBag.RemoveTemplate(templateId, num3);
			}
		}
		if (flag)
		{
			isFall = false;
			dir = 1;
			client.Player.MainBag.UpdatePlayerProperties();
		}
		string text = oldLv[0] + "," + oldExp[0] + "," + places[0];
		for (int k = 1; k < places.Length; k++)
		{
			object obj = text;
			text = string.Concat(obj, "|", oldLv[k], ",", oldExp[k], ",", places[k]);
		}
		gemStone.FigSpiritId = figSpiritId;
		gemStone.FigSpiritIdValue = text;
		client.Player.UpdateGemStone(place, gemStone);
		client.Player.OnUserToemGemstoneEvent();
		client.Player.Out.SendPlayerFigSpiritUp(iD, gemStone, flag, max, isFall, num2, dir);
		return 0;
	}
}
