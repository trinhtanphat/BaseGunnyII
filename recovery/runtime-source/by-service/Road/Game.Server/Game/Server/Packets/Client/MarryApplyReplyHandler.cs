using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(250, "求婚答复")]
internal class MarryApplyReplyHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		bool flag = packet.ReadBoolean();
		int userID = packet.ReadInt();
		int answerId = packet.ReadInt();
		if (flag && client.Player.PlayerCharacter.IsMarried)
		{
			client.Player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg2"));
		}
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(userID);
			if (!flag)
			{
				SendGoodManCard(userSingleByUserID.NickName, userSingleByUserID.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ID, playerBussiness);
				GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
			}
			if (userSingleByUserID == null || userSingleByUserID.Sex == client.Player.PlayerCharacter.Sex)
			{
				return 1;
			}
			if (userSingleByUserID.IsMarried)
			{
				client.Player.Out.SendMessage(eMessageType.ChatNormal, LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg3"));
			}
			MarryApplyInfo marryApplyInfo = new MarryApplyInfo();
			marryApplyInfo.UserID = userID;
			marryApplyInfo.ApplyUserID = client.Player.PlayerCharacter.ID;
			marryApplyInfo.ApplyUserName = client.Player.PlayerCharacter.NickName;
			marryApplyInfo.ApplyType = 2;
			marryApplyInfo.LoveProclamation = "";
			marryApplyInfo.ApplyResult = flag;
			int id = 0;
			if (playerBussiness.SavePlayerMarryNotice(marryApplyInfo, answerId, ref id))
			{
				if (flag)
				{
					client.Player.Out.SendMarryApplyReply(client.Player, userSingleByUserID.ID, userSingleByUserID.NickName, flag, isApplicant: false, id);
					client.Player.LoadMarryProp();
				}
				GameServer.Instance.LoginServer.SendUpdatePlayerMarriedStates(userSingleByUserID.ID);
				return 0;
			}
		}
		return 1;
	}

	public void SendSYSMessages(GamePlayer player, PlayerInfo spouse)
	{
		string text = (player.PlayerCharacter.Sex ? player.PlayerCharacter.NickName : spouse.NickName);
		string text2 = (player.PlayerCharacter.Sex ? spouse.NickName : player.PlayerCharacter.NickName);
		string translation = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Msg1", text, text2);
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteString(translation);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
		GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.Out.SendTCP(gSPacketIn);
		}
	}

	public void SendGoodManCard(string receiverName, int receiverID, string senderName, int senderID, PlayerBussiness db)
	{
		ItemTemplateInfo goods = ItemMgr.FindItemTemplate(11105);
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 112);
		itemInfo.IsBinds = true;
		itemInfo.UserID = 0;
		db.AddGoods(itemInfo);
		db.SendMail(new MailInfo
		{
			Annex1 = itemInfo.ItemID.ToString(),
			Content = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Content"),
			Gold = 0,
			IsExist = true,
			Money = 0,
			Receiver = receiverName,
			ReceiverID = receiverID,
			Sender = senderName,
			SenderID = senderID,
			Title = LanguageMgr.GetTranslation("MarryApplyReplyHandler.Title"),
			Type = 14
		});
	}
}
