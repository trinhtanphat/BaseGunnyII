using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(152, "场景用户离开")]
public class FightFootballTimeTakeoutHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		client.Player.Card = TreasureAwardMgr.CreateFightFootballTimeAward();
		GSPacketIn gSPacketIn = new GSPacketIn(152, client.Player.PlayerCharacter.ID);
		if (num < 9 && num >= 0 && client.Player.takeoutCount > 0)
		{
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(client.Player.Card[num].templateID);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(client.Player.Card[num].count);
			client.Player.TakeFootballCard(client.Player.Card[num]);
		}
		else
		{
			client.Player.ShowAllFootballCard();
			gSPacketIn.WriteInt(2);
			gSPacketIn.WriteInt(client.Player.canTakeOut);
			CardInfo[] cardsTakeOut = client.Player.CardsTakeOut;
			foreach (CardInfo cardInfo in cardsTakeOut)
			{
				if (cardInfo.IsTake)
				{
					gSPacketIn.WriteInt(cardInfo.templateID);
					gSPacketIn.WriteInt(cardInfo.place);
					gSPacketIn.WriteInt(cardInfo.count);
				}
			}
			gSPacketIn.WriteInt(client.Player.Card.Count - client.Player.canTakeOut);
			CardInfo[] cardsTakeOut2 = client.Player.CardsTakeOut;
			foreach (CardInfo cardInfo2 in cardsTakeOut2)
			{
				if (!cardInfo2.IsTake)
				{
					gSPacketIn.WriteInt(cardInfo2.templateID);
					gSPacketIn.WriteInt(cardInfo2.place);
					gSPacketIn.WriteInt(cardInfo2.count);
				}
			}
			client.Player.RemoveFightFootballStyle();
		}
		client.Out.SendTCP(gSPacketIn);
		return 0;
	}
}
