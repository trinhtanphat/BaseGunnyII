using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(134, "场景用户离开")]
public class DiceHandler : IPacketHandler
{
	private ThreadSafeRandom threadSafeRandom = new ThreadSafeRandom();

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		byte b = packet.ReadByte();
		int iD = client.Player.PlayerCharacter.ID;
		switch (b)
		{
		case 10:
			client.Player.Dice.ReceiveData();
			client.Player.Out.SendDiceReceiveData(client.Player.Dice, iD);
			break;
		case 11:
			receiveResult(client.Player, packet);
			break;
		case 12:
		{
			int refreshPrice = client.Player.Dice.refreshPrice;
			if (client.Player.MoneyDirect(refreshPrice))
			{
				client.Player.Dice.ReceiveData();
				client.Player.Out.SendDiceReceiveData(client.Player.Dice, iD);
			}
			break;
		}
		}
		return 0;
	}

	private void receiveResult(GamePlayer player, GSPacketIn packet)
	{
		if (player.PlayerCharacter.lastLuckNum >= player.Dice.Integral[player.Dice.MAX_LEVEL - 1])
		{
			player.SendMessage("Bạn đã nhận hết phần thưởng tích lũy hôm nay.");
			return;
		}
		int num = packet.ReadInt();
		packet.ReadInt();
		int num2;
		int value;
		switch (num)
		{
		case 1:
			num2 = threadSafeRandom.Next(2, 13);
			value = player.Dice.doubleDicePrice;
			break;
		case 2:
			num2 = threadSafeRandom.Next(4, 7);
			value = player.Dice.bigDicePrice;
			break;
		case 3:
			num2 = threadSafeRandom.Next(1, 4);
			value = player.Dice.smallDicePrice;
			break;
		default:
			num2 = threadSafeRandom.Next(1, 7);
			value = player.Dice.commonDicePrice;
			break;
		}
		if (player.MoneyDirect(value))
		{
			GSPacketIn gSPacketIn = new GSPacketIn(134);
			gSPacketIn.WriteByte(4);
			gSPacketIn.WriteInt(player.Dice.CurrentPosition);
			gSPacketIn.WriteInt(num2);
			player.Dice.CurrentPosition += num2;
			if (player.Dice.CurrentPosition > 18)
			{
				player.Dice.CurrentPosition -= 19;
			}
			ItemInfo itemInfo = player.Dice.rewardItem[player.Dice.CurrentPosition];
			ItemInfo itemInfo2 = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(itemInfo.TemplateID), itemInfo.Count, 103);
			if (!player.AddTemplate(itemInfo2, itemInfo2.Template.BagType, itemInfo.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipTypeView))
			{
				player.SendItemToMail(itemInfo2, itemInfo2.Template.Name.ToString(), "Phần thưởng Xí Ngầu.", eMailType.OpenUpArk);
			}
			player.Dice.rewardName = itemInfo2.Template.Name;
			int num3 = threadSafeRandom.Next(2, 13);
			player.Dice.LuckIntegral += num3;
			player.PlayerCharacter.lastLuckNum += num3;
			int luckIntegralLevel = player.Dice.LuckIntegralLevel;
			if (player.Dice.LuckIntegral >= player.Dice.Integral[luckIntegralLevel + 1])
			{
				player.Dice.LuckIntegralLevel++;
				player.PlayerCharacter.luckyNum++;
				player.Dice.GetLevelAward();
			}
			if (player.Dice.LuckIntegralLevel > 3)
			{
				player.Dice.LuckIntegralLevel = 3;
				player.PlayerCharacter.luckyNum = 3;
				num3 = player.Dice.Integral[player.Dice.MAX_LEVEL - 1];
				player.Dice.LuckIntegral = num3;
				player.PlayerCharacter.lastLuckNum = num3;
			}
			gSPacketIn.WriteInt(player.Dice.LuckIntegral);
			gSPacketIn.WriteInt(player.Dice.LuckIntegralLevel);
			gSPacketIn.WriteInt(player.Dice.freeCount);
			gSPacketIn.WriteString(player.Dice.rewardName);
			player.Out.SendTCP(gSPacketIn);
		}
	}
}
