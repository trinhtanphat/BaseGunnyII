using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(160, "添加好友")]
public class IMHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		switch (packet.ReadByte())
		{
		case 160:
		{
			string text = packet.ReadString();
			int num2 = packet.ReadInt();
			if (num2 < 0 || num2 > 1)
			{
				return 1;
			}
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text);
			PlayerInfo playerInfo = ((clientByPlayerNickName == null) ? playerBussiness.GetUserSingleByNickName(text) : clientByPlayerNickName.PlayerCharacter);
			if (playerInfo != null)
			{
				if (!client.Player.Friends.ContainsKey(playerInfo.ID) || client.Player.Friends[playerInfo.ID] != num2)
				{
					if (playerBussiness.AddFriends(new FriendInfo
					{
						FriendID = playerInfo.ID,
						IsExist = true,
						Remark = "",
						UserID = client.Player.PlayerCharacter.ID,
						Relation = num2
					}))
					{
						client.Player.FriendsAdd(playerInfo.ID, num2);
						if (num2 != 1 && playerInfo.State != 0)
						{
							GSPacketIn gSPacketIn = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
							gSPacketIn.WriteByte(166);
							gSPacketIn.WriteInt(playerInfo.ID);
							gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
							gSPacketIn.WriteBoolean(val: false);
							if (clientByPlayerNickName != null)
							{
								clientByPlayerNickName.SendTCP(gSPacketIn);
							}
							else
							{
								GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
							}
						}
						client.Out.SendAddFriend(playerInfo, num2, state: true);
						client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Success2"));
					}
				}
				else
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("FriendAddHandler.Falied"));
				}
			}
			else
			{
				client.Out.SendMessage(eMessageType.ERROR, LanguageMgr.GetTranslation("FriendAddHandler.Success") + text);
			}
			return 1;
		}
		case 162:
		case 163:
		case 164:
			return 1;
		default:
			return 1;
		case 161:
		{
			int num4 = packet.ReadInt();
			using PlayerBussiness playerBussiness2 = new PlayerBussiness();
			if (playerBussiness2.DeleteFriends(client.Player.PlayerCharacter.ID, num4))
			{
				client.Player.FriendsRemove(num4);
				client.Out.SendFriendRemove(num4);
			}
			return 1;
		}
		case 165:
		{
			int num3 = packet.ReadInt();
			GSPacketIn gSPacketIn2 = new GSPacketIn(160, client.Player.PlayerCharacter.ID);
			gSPacketIn2.WriteByte(165);
			gSPacketIn2.WriteInt(num3);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.typeVIP);
			gSPacketIn2.WriteInt(client.Player.PlayerCharacter.VIPLevel);
			gSPacketIn2.WriteBoolean(val: false);
			GameServer.Instance.LoginServer.SendPacket(gSPacketIn2);
			WorldMgr.ChangePlayerState(client.Player.PlayerCharacter.ID, num3, client.Player.PlayerCharacter.ConsortiaID);
			break;
		}
		case 51:
		{
			int num = packet.ReadInt();
			string msg = packet.ReadString();
			packet.ReadBoolean();
			GamePlayer playerById = WorldMgr.GetPlayerById(num);
			if (playerById != null)
			{
				client.Player.Out.sendOneOnOneTalk(num, isAutoReply: false, client.Player.PlayerCharacter.NickName, msg, client.Player.PlayerCharacter.ID);
				playerById.Out.sendOneOnOneTalk(client.Player.PlayerCharacter.ID, isAutoReply: false, client.Player.PlayerCharacter.NickName, msg, num);
			}
			else
			{
				client.Player.Out.SendMessage(eMessageType.Normal, "Người chơi không online!");
			}
			break;
		}
		case 45:
			break;
		}
		return 1;
	}
}
