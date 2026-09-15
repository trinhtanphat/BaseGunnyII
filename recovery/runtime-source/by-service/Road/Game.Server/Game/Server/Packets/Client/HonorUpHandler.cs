using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(96, "场景用户离开")]
public class HonorUpHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadByte();
		bool flag = packet.ReadBoolean();
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 0;
		}
		switch (num)
		{
		case 1:
			if (!flag && client.Player.PlayerCharacter.totemId > 0)
			{
				client.Player.Toemview = false;
			}
			break;
		case 2:
		{
			int iD = client.Player.PlayerCharacter.MaxBuyHonor + 1;
			TotemHonorTemplateInfo totemHonorTemplateInfo = TotemHonorMgr.FindTotemHonorTemplateInfo(iD);
			if (totemHonorTemplateInfo == null)
			{
				return 0;
			}
			int needMoney = totemHonorTemplateInfo.NeedMoney;
			int addHonor = totemHonorTemplateInfo.AddHonor;
			if (client.Player.MoneyDirect(needMoney))
			{
				client.Player.AddHonor(addHonor);
				client.Player.AddMaxHonor(1);
				client.Player.AddExpVip(needMoney);
			}
			break;
		}
		}
		client.Player.Out.SendUpdateUpCount(client.Player.PlayerCharacter);
		return 0;
	}
}
