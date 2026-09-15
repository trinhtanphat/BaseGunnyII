using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Packets.Client;

[PacketHandler(53, "场景用户离开")]
public class GetTimeBoxHandler : IPacketHandler
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		int receiebox = packet.ReadInt();
		packet.ReadInt();
		packet.ReadInt();
		bool result = false;
		List<ItemInfo> list = new List<ItemInfo>();
		int iD = client.Player.PlayerCharacter.ID;
		int receiebox2 = client.Player.PlayerCharacter.receiebox;
		string message = "Nhận rương thời gian thành công!";
		switch (num)
		{
		case 0:
			client.Player.UpdateTimeBox(receiebox, 20, 0);
			client.Out.SendGetBoxTime(iD, receiebox2, result);
			break;
		case 1:
		{
			result = true;
			LoadUserBoxInfo loadUserBoxInfo = ItemMgr.FindItemBoxTemplate(receiebox2);
			if (loadUserBoxInfo == null)
			{
				log.Warn("receiebox not found id: " + receiebox2);
				return 0;
			}
			list = ItemBoxMgr.GetAllItemBoxAward(loadUserBoxInfo.TemplateID);
			foreach (ItemInfo item in list)
			{
				if (!client.Player.AddTemplate(item, item.Template.BagType, item.Count, eItemNotice.NoneTypeView, eItemNotice.NoneTypeView))
				{
					using (PlayerBussiness playerBussiness = new PlayerBussiness())
					{
						item.UserID = 0;
						playerBussiness.AddGoods(item);
						MailInfo mailInfo = new MailInfo();
						mailInfo.Annex1 = item.ItemID.ToString();
						mailInfo.Content = "Phần thưởng từ rương thời gian.";
						mailInfo.Gold = 0;
						mailInfo.Money = 0;
						mailInfo.Receiver = client.Player.PlayerCharacter.NickName;
						mailInfo.ReceiverID = client.Player.PlayerCharacter.ID;
						mailInfo.Sender = mailInfo.Receiver;
						mailInfo.SenderID = mailInfo.ReceiverID;
						mailInfo.Title = "Mở rương thời gian!";
						mailInfo.Type = 12;
						playerBussiness.SendMail(mailInfo);
						message = "Túi đã đầy, vật phẩm đã được chuyển vào thư!";
					}
					client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
			}
			client.Out.SendGetBoxTime(iD, receiebox2, result);
			client.Out.SendMessage(eMessageType.Normal, message);
			break;
		}
		}
		return 0;
	}
}
