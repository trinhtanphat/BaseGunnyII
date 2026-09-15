using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(57, "购买物品")]
public class UserPresentGoodsHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int gold = 0;
		int money = 0;
		int offer = 0;
		int gifttoken = 0;
		int medal = 0;
		int damageScore = 0;
		int petScore = 0;
		int iTemplateID = 0;
		int iCount = 0;
		int hardCurrency = 0;
		int LeagueMoney = 0;
		StringBuilder stringBuilder = new StringBuilder();
		eMessageType eMessageType2 = eMessageType.Normal;
		string translateId = "UserPresentGoodsHandler.Success";
		string text = packet.ReadString();
		string text2 = packet.ReadString();
		int num = packet.ReadInt();
		if (client.Player.PlayerCharacter.NickName == text2)
		{
			client.Out.SendMessage(eMessageType.Normal, $"Không thể tặng cho chính mình.");
			return 0;
		}
		List<ItemInfo> list = new List<ItemInfo>();
		StringBuilder stringBuilder2 = new StringBuilder();
		GamePlayer clientByPlayerNickName = WorldMgr.GetClientByPlayerNickName(text2);
		PlayerInfo playerInfo;
		if (clientByPlayerNickName == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			playerInfo = playerBussiness.GetUserSingleByNickName(text2);
		}
		else
		{
			playerInfo = clientByPlayerNickName.PlayerCharacter;
		}
		bool flag = false;
		bool flag2 = false;
		for (int i = 0; i < num; i++)
		{
			int num2 = packet.ReadInt();
			int num3 = packet.ReadInt();
			string text3 = packet.ReadString();
			string text4 = packet.ReadString();
			packet.ReadInt();
			ShopItemInfo shopItemInfoById = ShopMgr.GetShopItemInfoById(num2);
			if (shopItemInfoById != null && ShopMgr.IsOnShop(num2) && shopItemInfoById.ShopID == 1)
			{
				flag2 = true;
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(shopItemInfoById.TemplateID);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 102);
			if (shopItemInfoById.BuyType == 0)
			{
				if (1 == num3)
				{
					itemInfo.ValidDate = shopItemInfoById.AUnit;
				}
				if (2 == num3)
				{
					itemInfo.ValidDate = shopItemInfoById.BUnit;
				}
				if (3 == num3)
				{
					itemInfo.ValidDate = shopItemInfoById.CUnit;
				}
			}
			else
			{
				if (1 == num3)
				{
					itemInfo.Count = shopItemInfoById.AUnit;
				}
				if (2 == num3)
				{
					itemInfo.Count = shopItemInfoById.BUnit;
				}
				if (3 == num3)
				{
					itemInfo.Count = shopItemInfoById.CUnit;
				}
			}
			if (itemInfo != null || shopItemInfoById != null)
			{
				itemInfo.Color = ((text3 == null) ? "" : text3);
				itemInfo.Skin = ((text4 == null) ? "" : text4);
				if (flag)
				{
					itemInfo.IsBinds = true;
				}
				else
				{
					itemInfo.IsBinds = Convert.ToBoolean(shopItemInfoById.IsBind);
				}
				stringBuilder2.Append(num3);
				stringBuilder2.Append(",");
				if (flag2)
				{
					list.Add(itemInfo);
				}
				ShopMgr.SetItemType(shopItemInfoById, num3, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref medal);
			}
		}
		if (list.Count == 0)
		{
			client.Disconnect();
			return 1;
		}
		if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
		{
			client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
			return 1;
		}
		if (!client.Player.ActiveMoneyEnable(money))
		{
			return 0;
		}
		if (gold <= client.Player.PlayerCharacter.Gold && money <= client.Player.PlayerCharacter.Money && gifttoken <= client.Player.PlayerCharacter.GiftToken && medal <= client.Player.PlayerCharacter.medal)
		{
			client.Player.RemoveGold(gold);
			client.Player.RemoveOffer(offer);
			client.Player.RemoveGiftToken(gifttoken);
			client.Player.RemoveMedal(medal);
			string text5 = "";
			int num4 = 0;
			MailInfo mailInfo = new MailInfo();
			StringBuilder stringBuilder3 = new StringBuilder();
			stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));
			for (int j = 0; j < list.Count; j++)
			{
				text5 += ((text5 == "") ? list[j].TemplateID.ToString() : ("," + list[j].TemplateID));
				using PlayerBussiness playerBussiness2 = new PlayerBussiness();
				list[j].UserID = 0;
				playerBussiness2.AddGoods(list[j]);
				num4++;
				stringBuilder3.Append(num4);
				stringBuilder3.Append("、");
				stringBuilder3.Append(list[j].Template.Name);
				stringBuilder3.Append("x");
				stringBuilder3.Append(list[j].Count);
				stringBuilder3.Append(";");
				switch (num4)
				{
				case 1:
					mailInfo.Annex1 = list[j].ItemID.ToString();
					mailInfo.Annex1Name = list[j].Template.Name;
					break;
				case 2:
					mailInfo.Annex2 = list[j].ItemID.ToString();
					mailInfo.Annex2Name = list[j].Template.Name;
					break;
				case 3:
					mailInfo.Annex3 = list[j].ItemID.ToString();
					mailInfo.Annex3Name = list[j].Template.Name;
					break;
				case 4:
					mailInfo.Annex4 = list[j].ItemID.ToString();
					mailInfo.Annex4Name = list[j].Template.Name;
					break;
				case 5:
					mailInfo.Annex5 = list[j].ItemID.ToString();
					mailInfo.Annex5Name = list[j].Template.Name;
					break;
				}
				if (num4 == 5)
				{
					num4 = 0;
					mailInfo.AnnexRemark = stringBuilder3.ToString();
					stringBuilder3.Remove(0, stringBuilder3.Length);
					stringBuilder3.Append(LanguageMgr.GetTranslation("GoodsPresentHandler.AnnexRemark"));
					mailInfo.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title") + mailInfo.Annex1Name + "] " + text;
					mailInfo.Gold = 0;
					mailInfo.Money = 0;
					mailInfo.Receiver = playerInfo.NickName;
					mailInfo.ReceiverID = playerInfo.ID;
					mailInfo.Sender = client.Player.PlayerCharacter.NickName;
					mailInfo.SenderID = client.Player.PlayerCharacter.ID;
					mailInfo.Title = mailInfo.Content;
					mailInfo.Type = 8;
					playerBussiness2.SendMail(mailInfo);
					eMessageType2 = eMessageType.ERROR;
					mailInfo.Revert();
				}
			}
			if (num4 > 0)
			{
				using PlayerBussiness playerBussiness3 = new PlayerBussiness();
				mailInfo.AnnexRemark = stringBuilder3.ToString();
				mailInfo.Content = LanguageMgr.GetTranslation("UserBuyItemHandler.Title") + mailInfo.Annex1Name + "] " + text;
				mailInfo.Gold = 0;
				mailInfo.Money = 0;
				mailInfo.Receiver = playerInfo.NickName;
				mailInfo.ReceiverID = playerInfo.ID;
				mailInfo.Sender = client.Player.PlayerCharacter.NickName;
				mailInfo.SenderID = client.Player.PlayerCharacter.ID;
				mailInfo.Title = mailInfo.Content;
				mailInfo.Type = 8;
				playerBussiness3.SendMail(mailInfo);
				eMessageType2 = eMessageType.ERROR;
			}
			if (eMessageType2 == eMessageType.ERROR)
			{
				clientByPlayerNickName?.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
			client.Player.OnPaid(money, gold, offer, gifttoken, medal, stringBuilder.ToString());
		}
		else
		{
			if (gold > client.Player.PlayerCharacter.Gold)
			{
				translateId = "UserBuyItemHandler.NoGold";
			}
			if (money > client.Player.PlayerCharacter.Money)
			{
				translateId = "UserBuyItemHandler.NoMoney";
			}
			if (offer > client.Player.PlayerCharacter.Offer)
			{
				translateId = "UserBuyItemHandler.NoOffer";
			}
			if (gifttoken > client.Player.PlayerCharacter.GiftToken)
			{
				translateId = "UserBuyItemHandler.GiftToken";
			}
			if (medal > client.Player.PlayerCharacter.medal)
			{
				translateId = "UserBuyItemHandler.Medal";
			}
			eMessageType2 = eMessageType.ERROR;
		}
		client.Out.SendMessage(eMessageType2, LanguageMgr.GetTranslation(translateId));
		return 0;
	}
}
