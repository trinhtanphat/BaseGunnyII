using Bussiness;
using Game.Base.Packets;
using Game.Server.GameUtils;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(188, "场景用户离开")]
public class UseConsortiaReworkNameHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		byte bageType = packet.ReadByte();
		int slot = packet.ReadInt();
		string newNickName = packet.ReadString();
		string text = "";
		if (client.Player.PlayerCharacter.ConsortiaID == 0)
		{
			return 0;
		}
		PlayerInventory inventory = client.Player.GetInventory((eBageType)bageType);
		ItemInfo itemAt = inventory.GetItemAt(slot);
		if (itemAt.Count < 1)
		{
			client.Player.SendMessage("Vật phẩm không tồn tại");
			return 0;
		}
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaInfo consortiaSingle = consortiaBussiness.GetConsortiaSingle(num);
			if (consortiaSingle == null)
			{
				client.Player.SendMessage("Guild Không tồn tại.");
				return 0;
			}
			if (client.Player.PlayerCharacter.ID != consortiaSingle.ChairmanID)
			{
				client.Player.SendMessage("Chủ Giuld mới có thể đổi tên.");
				return 0;
			}
			if (consortiaBussiness.RenameConsortia(num, client.Player.PlayerCharacter.NickName, newNickName))
			{
				inventory.RemoveCountFromStack(itemAt, 1);
			}
			else
			{
				text = "Tên Guild đã được sử dụng.";
			}
		}
		if (text != "")
		{
			client.Player.SendMessage(text);
		}
		return 0;
	}
}
