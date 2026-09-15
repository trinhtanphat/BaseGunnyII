using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Game.Base.Packets;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(129, "公会聊天")]
public class ConsortiaHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		bool flag = false;
		string msg = "Packet Error!";
		WorldMgr.GetAllPlayers();
		switch (num)
		{
		case 0:
		{
			if (client.Player.PlayerCharacter.ConsortiaID != 0)
			{
				return 0;
			}
			int num16 = packet.ReadInt();
			msg = "ConsortiaApplyLoginHandler.ADD_Failed";
			using (ConsortiaBussiness consortiaBussiness25 = new ConsortiaBussiness())
			{
				if (consortiaBussiness25.AddConsortiaApplyUsers(new ConsortiaApplyUserInfo
				{
					ApplyDate = DateTime.Now,
					ConsortiaID = num16,
					ConsortiaName = "",
					IsExist = true,
					Remark = "",
					UserID = client.Player.PlayerCharacter.ID,
					UserName = client.Player.PlayerCharacter.NickName
				}, ref msg))
				{
					msg = ((num16 != 0) ? "ConsortiaApplyLoginHandler.ADD_Success" : "ConsortiaApplyLoginHandler.DELETE_Success");
					flag = true;
				}
				else
				{
					client.Player.SendMessage("db.AddConsortia Error ");
				}
			}
			client.Out.sendConsortiaTryIn(num16, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 1:
		{
			if (client.Player.PlayerCharacter.ConsortiaID != 0)
			{
				return 0;
			}
			ConsortiaLevelInfo consortiaLevelInfo = ConsortiaExtraMgr.FindConsortiaLevelInfo(1);
			string text4 = packet.ReadString();
			int num14 = 0;
			int needGold = consortiaLevelInfo.NeedGold;
			int num15 = 5;
			msg = "ConsortiaCreateHandler.Failed";
			ConsortiaDutyInfo dutyInfo = new ConsortiaDutyInfo();
			if (!string.IsNullOrEmpty(text4) && client.Player.PlayerCharacter.Gold >= needGold && client.Player.PlayerCharacter.Grade >= num15)
			{
				using ConsortiaBussiness consortiaBussiness14 = new ConsortiaBussiness();
				ConsortiaInfo consortiaInfo5 = new ConsortiaInfo();
				consortiaInfo5.BuildDate = DateTime.Now;
				consortiaInfo5.CelebCount = 0;
				consortiaInfo5.ChairmanID = client.Player.PlayerCharacter.ID;
				consortiaInfo5.ChairmanName = client.Player.PlayerCharacter.NickName;
				consortiaInfo5.ConsortiaName = text4;
				consortiaInfo5.CreatorID = consortiaInfo5.ChairmanID;
				consortiaInfo5.CreatorName = consortiaInfo5.ChairmanName;
				consortiaInfo5.Description = "";
				consortiaInfo5.Honor = 0;
				consortiaInfo5.IP = "";
				consortiaInfo5.IsExist = true;
				consortiaInfo5.Level = consortiaLevelInfo.Level;
				consortiaInfo5.MaxCount = consortiaLevelInfo.Count;
				consortiaInfo5.Riches = consortiaLevelInfo.Riches;
				consortiaInfo5.Placard = "";
				consortiaInfo5.Port = 0;
				consortiaInfo5.Repute = 0;
				consortiaInfo5.Count = 1;
				if (consortiaBussiness14.AddConsortia(consortiaInfo5, ref msg, ref dutyInfo))
				{
					client.Player.PlayerCharacter.ConsortiaID = consortiaInfo5.ConsortiaID;
					client.Player.PlayerCharacter.ConsortiaName = consortiaInfo5.ConsortiaName;
					client.Player.PlayerCharacter.DutyLevel = dutyInfo.Level;
					client.Player.PlayerCharacter.DutyName = dutyInfo.DutyName;
					client.Player.PlayerCharacter.Right = dutyInfo.Right;
					client.Player.PlayerCharacter.ConsortiaLevel = consortiaLevelInfo.Level;
					client.Player.RemoveGold(needGold);
					msg = "ConsortiaCreateHandler.Success";
					flag = true;
					num14 = consortiaInfo5.ConsortiaID;
					GameServer.Instance.LoginServer.SendConsortiaCreate(num14, client.Player.PlayerCharacter.Offer, consortiaInfo5.ConsortiaName);
				}
				else
				{
					client.Player.SendMessage("db.AddConsortia Error ");
				}
			}
			client.Out.SendConsortiaCreate(text4, flag, num14, text4, LanguageMgr.GetTranslation(msg), dutyInfo.Level, dutyInfo.DutyName, dutyInfo.Right, client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 3:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num13 = packet.ReadInt();
			string nickName = "";
			msg = ((num13 == client.Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitFailed" : "ConsortiaUserDeleteHandler.KickFailed");
			using (ConsortiaBussiness consortiaBussiness13 = new ConsortiaBussiness())
			{
				if (consortiaBussiness13.DeleteConsortiaUser(client.Player.PlayerCharacter.ID, num13, client.Player.PlayerCharacter.ConsortiaID, ref msg, ref nickName))
				{
					msg = ((num13 == client.Player.PlayerCharacter.ID) ? "ConsortiaUserDeleteHandler.ExitSuccess" : "ConsortiaUserDeleteHandler.KickSuccess");
					int consortiaID3 = client.Player.PlayerCharacter.ConsortiaID;
					if (num13 == client.Player.PlayerCharacter.ID)
					{
						client.Player.ClearConsortia();
						client.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Receiver);
					}
					GameServer.Instance.LoginServer.SendConsortiaUserDelete(num13, consortiaID3, num13 != client.Player.PlayerCharacter.ID, nickName, client.Player.PlayerCharacter.NickName);
					flag = true;
				}
			}
			client.Out.sendConsortiaOut(num13, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 4:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num18 = packet.ReadInt();
			msg = "ConsortiaApplyLoginPassHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness27 = new ConsortiaBussiness())
			{
				int consortiaRepute2 = 0;
				ConsortiaUserInfo consortiaUserInfo4 = new ConsortiaUserInfo();
				if (consortiaBussiness27.PassConsortiaApplyUsers(num18, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, client.Player.PlayerCharacter.ConsortiaID, ref msg, consortiaUserInfo4, ref consortiaRepute2))
				{
					msg = "ConsortiaApplyLoginPassHandler.Success";
					flag = true;
					if (consortiaUserInfo4.UserID != 0)
					{
						consortiaUserInfo4.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
						consortiaUserInfo4.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
						GameServer.Instance.LoginServer.SendConsortiaUserPass(client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, consortiaUserInfo4, isInvite: false, consortiaRepute2, consortiaUserInfo4.LoginName, client.Player.PlayerCharacter.FightPower, client.Player.PlayerCharacter.Offer);
					}
				}
			}
			client.Out.sendConsortiaTryInPass(num18, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 5:
		{
			int num5 = packet.ReadInt();
			msg = "ConsortiaApplyAllyDeleteHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness6 = new ConsortiaBussiness())
			{
				if (consortiaBussiness6.DeleteConsortiaApplyUsers(num5, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref msg))
				{
					msg = ((client.Player.PlayerCharacter.ID == 0) ? "ConsortiaApplyAllyDeleteHandler.Success" : "ConsortiaApplyAllyDeleteHandler.Success2");
					flag = true;
				}
			}
			client.Out.sendConsortiaTryInDel(num5, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 6:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num17 = packet.ReadInt();
			if (client.Player.PlayerCharacter.HasBagPassword && client.Player.PlayerCharacter.IsLocked)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Bag.Locked"));
				return 1;
			}
			if (num17 < 1 || !client.Player.MoneyDirect(num17))
			{
				return 1;
			}
			msg = "ConsortiaRichesOfferHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness26 = new ConsortiaBussiness())
			{
				int riches5 = num17 / 2;
				if (consortiaBussiness26.ConsortiaRichAdd(client.Player.PlayerCharacter.ConsortiaID, ref riches5, 5, client.Player.PlayerCharacter.NickName))
				{
					flag = true;
					client.Player.AddRichesOffer(riches5);
					msg = "ConsortiaRichesOfferHandler.Successed";
					GameServer.Instance.LoginServer.SendConsortiaRichesOffer(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, riches5);
				}
			}
			client.Out.SendConsortiaRichesOffer(num17, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 7:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 1;
			}
			bool state = packet.ReadBoolean();
			msg = "CONSORTIA_APPLY_STATE.Failed";
			using (ConsortiaBussiness consortiaBussiness15 = new ConsortiaBussiness())
			{
				if (consortiaBussiness15.UpdateConsotiaApplyState(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, state, ref msg))
				{
					msg = "CONSORTIA_APPLY_STATE.Success";
					flag = true;
				}
			}
			client.Out.sendConsortiaApplyStatusOut(state, flag, client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 9:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int dutyID2 = packet.ReadInt();
			msg = "ConsortiaDutyDeleteHandler.Failed";
			using ConsortiaBussiness consortiaBussiness17 = new ConsortiaBussiness();
			if (consortiaBussiness17.DeleteConsortiaDuty(dutyID2, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.ConsortiaID, ref msg))
			{
				msg = "ConsortiaDutyDeleteHandler.Success";
				flag = true;
			}
			return 0;
		}
		case 12:
		{
			if (client.Player.PlayerCharacter.ConsortiaID != 0)
			{
				return 0;
			}
			int num2 = packet.ReadInt();
			int consortiaID = 0;
			string consortiaName = "";
			msg = "ConsortiaInvitePassHandler.Failed";
			int tempID = 0;
			string tempName = "";
			using (ConsortiaBussiness consortiaBussiness2 = new ConsortiaBussiness())
			{
				int consortiaRepute = 0;
				ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
				if (consortiaBussiness2.PassConsortiaInviteUsers(num2, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, ref consortiaID, ref consortiaName, ref msg, consortiaUserInfo, ref tempID, ref tempName, ref consortiaRepute))
				{
					client.Player.PlayerCharacter.ConsortiaID = consortiaID;
					client.Player.PlayerCharacter.ConsortiaName = consortiaName;
					client.Player.PlayerCharacter.DutyLevel = consortiaUserInfo.Level;
					client.Player.PlayerCharacter.DutyName = consortiaUserInfo.DutyName;
					client.Player.PlayerCharacter.Right = consortiaUserInfo.Right;
					ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(consortiaID);
					if (consortiaInfo != null)
					{
						client.Player.PlayerCharacter.ConsortiaLevel = consortiaInfo.Level;
					}
					msg = "ConsortiaInvitePassHandler.Success";
					flag = true;
					consortiaUserInfo.UserID = client.Player.PlayerCharacter.ID;
					consortiaUserInfo.UserName = client.Player.PlayerCharacter.NickName;
					consortiaUserInfo.Grade = client.Player.PlayerCharacter.Grade;
					consortiaUserInfo.Offer = client.Player.PlayerCharacter.Offer;
					consortiaUserInfo.RichesOffer = client.Player.PlayerCharacter.RichesOffer;
					consortiaUserInfo.RichesRob = client.Player.PlayerCharacter.RichesRob;
					consortiaUserInfo.Win = client.Player.PlayerCharacter.Win;
					consortiaUserInfo.Total = client.Player.PlayerCharacter.Total;
					consortiaUserInfo.Escape = client.Player.PlayerCharacter.Escape;
					consortiaUserInfo.ConsortiaID = consortiaID;
					consortiaUserInfo.ConsortiaName = consortiaName;
					GameServer.Instance.LoginServer.SendConsortiaUserPass(tempID, tempName, consortiaUserInfo, isInvite: true, consortiaRepute, client.Player.PlayerCharacter.UserName, client.Player.PlayerCharacter.FightPower, client.Player.PlayerCharacter.Offer);
				}
			}
			client.Out.sendConsortiaInvitePass(num2, flag, consortiaID, consortiaName, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 13:
		{
			int num19 = packet.ReadInt();
			msg = "ConsortiaInviteDeleteHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness29 = new ConsortiaBussiness())
			{
				if (consortiaBussiness29.DeleteConsortiaInviteUsers(num19, client.Player.PlayerCharacter.ID))
				{
					msg = "ConsortiaInviteDeleteHandler.Success";
					flag = true;
				}
			}
			client.Out.sendConsortiaInviteDel(num19, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 14:
		{
			string text5 = packet.ReadString();
			if (Encoding.Default.GetByteCount(text5) > 300)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDescriptionUpdateHandler.Long"));
				return 1;
			}
			msg = "ConsortiaDescriptionUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness24 = new ConsortiaBussiness())
			{
				if (consortiaBussiness24.UpdateConsortiaDescription(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, text5, ref msg))
				{
					msg = "ConsortiaDescriptionUpdateHandler.Success";
					flag = true;
				}
			}
			client.Out.sendConsortiaUpdateDescription(text5, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 15:
		{
			string text3 = packet.ReadString();
			if (Encoding.Default.GetByteCount(text3) > 300)
			{
				client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaPlacardUpdateHandler.Long"));
				return 1;
			}
			msg = "ConsortiaPlacardUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness8 = new ConsortiaBussiness())
			{
				if (consortiaBussiness8.UpdateConsortiaPlacard(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, text3, ref msg))
				{
					msg = "ConsortiaPlacardUpdateHandler.Success";
					flag = true;
				}
			}
			client.Out.sendConsortiaUpdatePlacard(text3, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 18:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int num3 = packet.ReadInt();
			bool flag2 = packet.ReadBoolean();
			msg = "ConsortiaUserGradeUpdateHandler.Failed";
			using (ConsortiaBussiness consortiaBussiness3 = new ConsortiaBussiness())
			{
				string tempUserName = "";
				ConsortiaDutyInfo info = new ConsortiaDutyInfo();
				if (consortiaBussiness3.UpdateConsortiaUserGrade(num3, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, flag2, ref msg, ref info, ref tempUserName))
				{
					msg = "ConsortiaUserGradeUpdateHandler.Success";
					flag = true;
					GameServer.Instance.LoginServer.SendConsortiaDuty(info, flag2 ? 6 : 7, client.Player.PlayerCharacter.ConsortiaID, num3, tempUserName, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName);
				}
			}
			client.Out.SendConsortiaMemberGrade(num3, flag2, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 19:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			string text2 = packet.ReadString();
			msg = "ConsortiaChangeChairmanHandler.Failed";
			if (string.IsNullOrEmpty(text2))
			{
				msg = "ConsortiaChangeChairmanHandler.NoName";
			}
			else if (text2 == client.Player.PlayerCharacter.NickName)
			{
				msg = "ConsortiaChangeChairmanHandler.Self";
			}
			else
			{
				using ConsortiaBussiness consortiaBussiness5 = new ConsortiaBussiness();
				string tempUserName2 = "";
				int tempUserID = 0;
				ConsortiaDutyInfo info2 = new ConsortiaDutyInfo();
				if (consortiaBussiness5.UpdateConsortiaChairman(text2, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg, ref info2, ref tempUserID, ref tempUserName2))
				{
					ConsortiaDutyInfo consortiaDutyInfo = new ConsortiaDutyInfo();
					consortiaDutyInfo.Level = client.Player.PlayerCharacter.DutyLevel;
					consortiaDutyInfo.DutyName = client.Player.PlayerCharacter.DutyName;
					consortiaDutyInfo.Right = client.Player.PlayerCharacter.Right;
					msg = "ConsortiaChangeChairmanHandler.Success1";
					flag = true;
					GameServer.Instance.LoginServer.SendConsortiaDuty(consortiaDutyInfo, 9, client.Player.PlayerCharacter.ConsortiaID, tempUserID, tempUserName2, 0, "");
					GameServer.Instance.LoginServer.SendConsortiaDuty(info2, 8, client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, client.Player.PlayerCharacter.NickName, 0, "");
				}
			}
			string translation = LanguageMgr.GetTranslation(msg);
			if (msg == "ConsortiaChangeChairmanHandler.Success1")
			{
				translation = translation + text2 + LanguageMgr.GetTranslation("ConsortiaChangeChairmanHandler.Success2");
			}
			client.Out.sendConsortiaChangeChairman(text2, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 20:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			if (client.Player.PlayerCharacter.IsBanChat)
			{
				client.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("ConsortiaChatHandler.IsBanChat"));
				return 1;
			}
			packet.ClientID = client.Player.PlayerCharacter.ID;
			packet.ReadByte();
			packet.ReadString();
			packet.ReadString();
			packet.WriteInt(client.Player.PlayerCharacter.ConsortiaID);
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				if (gamePlayer.PlayerCharacter.ConsortiaID == client.Player.PlayerCharacter.ConsortiaID)
				{
					gamePlayer.Out.SendTCP(packet);
				}
			}
			GameServer.Instance.LoginServer.SendPacket(packet);
			return 0;
		}
		case 21:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			byte b2 = packet.ReadByte();
			byte level = 0;
			switch (b2)
			{
			case 1:
			{
				msg = "ConsortiaUpGradeHandler.Failed";
				using (ConsortiaBussiness consortiaBussiness19 = new ConsortiaBussiness())
				{
					ConsortiaInfo consortiaSingle = consortiaBussiness19.GetConsortiaSingle(client.Player.PlayerCharacter.ConsortiaID);
					if (consortiaSingle == null)
					{
						msg = "ConsortiaUpGradeHandler.NoConsortia";
						break;
					}
					ConsortiaLevelInfo consortiaLevelInfo = ConsortiaExtraMgr.FindConsortiaLevelInfo(consortiaSingle.Level + 1);
					if (consortiaLevelInfo == null)
					{
						msg = "ConsortiaUpGradeHandler.NoUpGrade";
						break;
					}
					if (consortiaLevelInfo.NeedGold > client.Player.PlayerCharacter.Gold)
					{
						msg = "ConsortiaUpGradeHandler.NoGold";
						break;
					}
					using ConsortiaBussiness consortiaBussiness20 = new ConsortiaBussiness();
					if (consortiaBussiness20.UpGradeConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
					{
						consortiaSingle.Level++;
						client.Player.RemoveGold(consortiaLevelInfo.NeedGold);
						GameServer.Instance.LoginServer.SendConsortiaUpGrade(consortiaSingle);
						msg = "ConsortiaUpGradeHandler.Success";
						flag = true;
						level = (byte)consortiaSingle.Level;
					}
				}
				break;
			}
			case 2:
			{
				msg = "ConsortiaStoreUpGradeHandler.Failed";
				ConsortiaInfo consortiaInfo9 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaInfo9 == null)
				{
					msg = "ConsortiaStoreUpGradeHandler.NoConsortia";
					break;
				}
				using (ConsortiaBussiness consortiaBussiness23 = new ConsortiaBussiness())
				{
					if (consortiaBussiness23.UpGradeStoreConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
					{
						consortiaInfo9.StoreLevel++;
						GameServer.Instance.LoginServer.SendConsortiaStoreUpGrade(consortiaInfo9);
						msg = "ConsortiaStoreUpGradeHandler.Success";
						flag = true;
						level = (byte)consortiaInfo9.StoreLevel;
					}
				}
				break;
			}
			case 3:
			{
				msg = "ConsortiaShopUpGradeHandler.Failed";
				ConsortiaInfo consortiaInfo7 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaInfo7 == null)
				{
					msg = "ConsortiaShopUpGradeHandler.NoConsortia";
					break;
				}
				using (ConsortiaBussiness consortiaBussiness21 = new ConsortiaBussiness())
				{
					if (consortiaBussiness21.UpGradeShopConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
					{
						consortiaInfo7.ShopLevel++;
						GameServer.Instance.LoginServer.SendConsortiaShopUpGrade(consortiaInfo7);
						msg = "ConsortiaShopUpGradeHandler.Success";
						flag = true;
						level = (byte)consortiaInfo7.ShopLevel;
					}
				}
				break;
			}
			case 4:
			{
				msg = "ConsortiaSmithUpGradeHandler.Failed";
				ConsortiaInfo consortiaInfo8 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaInfo8 == null)
				{
					msg = "ConsortiaSmithUpGradeHandler.NoConsortia";
					break;
				}
				using (ConsortiaBussiness consortiaBussiness22 = new ConsortiaBussiness())
				{
					if (consortiaBussiness22.UpGradeSmithConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
					{
						consortiaInfo8.SmithLevel++;
						GameServer.Instance.LoginServer.SendConsortiaSmithUpGrade(consortiaInfo8);
						msg = "ConsortiaSmithUpGradeHandler.Success";
						flag = true;
						level = (byte)consortiaInfo8.SmithLevel;
					}
				}
				break;
			}
			case 5:
			{
				msg = "ConsortiaBufferUpGradeHandler.Failed";
				ConsortiaInfo consortiaInfo6 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
				if (consortiaInfo6 == null)
				{
					msg = "ConsortiaUpGradeHandler.NoConsortia";
					break;
				}
				using (ConsortiaBussiness consortiaBussiness18 = new ConsortiaBussiness())
				{
					if (consortiaBussiness18.UpGradeSkillConsortia(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, ref msg))
					{
						consortiaInfo6.SkillLevel++;
						GameServer.Instance.LoginServer.SendConsortiaKillUpGrade(consortiaInfo6);
						msg = "ConsortiaBufferUpGradeHandler.Success";
						flag = true;
						level = (byte)consortiaInfo6.SkillLevel;
					}
				}
				break;
			}
			}
			client.Out.SendConsortiaLevelUp(b2, level, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 24:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int item = 0;
			int item2 = 0;
			int item3 = 0;
			int item4 = 0;
			int item5 = 0;
			int item6 = 0;
			int item7 = 0;
			msg = "ConsortiaEquipControlHandler.Fail";
			ConsortiaEquipControlInfo consortiaEquipControlInfo = new ConsortiaEquipControlInfo();
			consortiaEquipControlInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
			using (ConsortiaBussiness consortiaBussiness16 = new ConsortiaBussiness())
			{
				for (int j = 0; j < 5; j++)
				{
					consortiaEquipControlInfo.Riches = packet.ReadInt();
					consortiaEquipControlInfo.Type = 1;
					consortiaEquipControlInfo.Level = j + 1;
					consortiaBussiness16.AddAndUpdateConsortiaEuqipControl(consortiaEquipControlInfo, client.Player.PlayerCharacter.ID, ref msg);
					switch (j)
					{
					case 0:
						item = consortiaEquipControlInfo.Riches;
						break;
					case 1:
						item2 = consortiaEquipControlInfo.Riches;
						break;
					case 2:
						item3 = consortiaEquipControlInfo.Riches;
						break;
					case 3:
						item4 = consortiaEquipControlInfo.Riches;
						break;
					case 4:
						item5 = consortiaEquipControlInfo.Riches;
						break;
					}
				}
				consortiaEquipControlInfo.Riches = packet.ReadInt();
				consortiaEquipControlInfo.Type = 2;
				consortiaEquipControlInfo.Level = 0;
				item6 = consortiaEquipControlInfo.Riches;
				consortiaBussiness16.AddAndUpdateConsortiaEuqipControl(consortiaEquipControlInfo, client.Player.PlayerCharacter.ID, ref msg);
				consortiaEquipControlInfo.Riches = packet.ReadInt();
				consortiaEquipControlInfo.Type = 3;
				consortiaEquipControlInfo.Level = 0;
				item7 = consortiaEquipControlInfo.Riches;
				consortiaBussiness16.AddAndUpdateConsortiaEuqipControl(consortiaEquipControlInfo, client.Player.PlayerCharacter.ID, ref msg);
				msg = "ConsortiaEquipControlHandler.Success";
				flag = true;
			}
			List<int> list = new List<int>();
			list.Add(item);
			list.Add(item2);
			list.Add(item3);
			list.Add(item4);
			list.Add(item5);
			list.Add(item6);
			list.Add(item7);
			List<int> riches4 = list;
			client.Out.sendConsortiaEquipConstrol(flag, riches4, client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 26:
		{
			packet.ReadBoolean();
			int id = packet.ReadInt();
			int num7 = packet.ReadInt();
			int num8 = packet.ReadInt();
			if (num7 < 0)
			{
				num7 = 1;
			}
			ConsortiaBuffTempInfo consortiaBuffTempInfo = ConsortiaExtraMgr.FindConsortiaBuffInfo(id);
			if (consortiaBuffTempInfo == null)
			{
				client.Out.SendMessage(eMessageType.Normal, "Dữ liệu server lổi.");
				return 0;
			}
			int num9 = 0;
			bool flag3 = false;
			int num10 = num8;
			if (num10 == 1)
			{
				num9 = num7 * consortiaBuffTempInfo.riches;
				if (client.Player.PlayerCharacter.Offer >= num9)
				{
					flag3 = true;
				}
			}
			else
			{
				num9 = num7 * consortiaBuffTempInfo.metal;
				if (client.Player.PlayerCharacter.GiftToken >= num9)
				{
					flag3 = true;
				}
			}
			int validMinutes = 1440 * num7;
			ConsortiaInfo consortiaInfo3 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			if (consortiaInfo3 == null || !flag3)
			{
				client.Out.SendMessage(eMessageType.Normal, "Điều kiện không đủ, thao tác thất bại.");
				return 0;
			}
			if (consortiaBuffTempInfo.level <= consortiaInfo3.Level)
			{
				switch (consortiaBuffTempInfo.group)
				{
				case 1:
					BufferList.CreatePayBuffer(101, consortiaBuffTempInfo.value, validMinutes, id)?.Start(client.Player);
					break;
				case 3:
					BufferList.CreatePayBuffer(103, consortiaBuffTempInfo.value, validMinutes, id)?.Start(client.Player);
					break;
				case 6:
					BufferList.CreatePayBuffer(106, consortiaBuffTempInfo.value, validMinutes, id)?.Start(client.Player);
					break;
				case 8:
					client.Out.SendMessage(eMessageType.Normal, "Skill này chưa hổ trợ.");
					return 0;
				case 11:
					BufferList.CreatePayBuffer(111, consortiaBuffTempInfo.value, validMinutes, id)?.Start(client.Player);
					break;
				case 12:
					BufferList.CreatePayBuffer(112, consortiaBuffTempInfo.value, validMinutes, id)?.Start(client.Player);
					break;
				default:
				{
					using (PlayerBussiness playerBussiness2 = new PlayerBussiness())
					{
						ConsortiaUserInfo[] allMemberByConsortia2 = playerBussiness2.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
						AbstractBuffer abstractBuffer = null;
						switch (consortiaBuffTempInfo.group)
						{
						case 2:
							abstractBuffer = BufferList.CreatePayBuffer(102, consortiaBuffTempInfo.value, validMinutes, id);
							break;
						case 4:
							abstractBuffer = BufferList.CreatePayBuffer(104, consortiaBuffTempInfo.value, validMinutes, id);
							break;
						case 5:
							abstractBuffer = BufferList.CreatePayBuffer(105, consortiaBuffTempInfo.value, validMinutes, id);
							break;
						case 7:
							abstractBuffer = BufferList.CreatePayBuffer(107, consortiaBuffTempInfo.value, validMinutes, id);
							break;
						case 9:
							abstractBuffer = BufferList.CreatePayBuffer(109, consortiaBuffTempInfo.value, validMinutes, id);
							break;
						case 10:
							abstractBuffer = BufferList.CreatePayBuffer(110, consortiaBuffTempInfo.value, validMinutes, id);
							break;
						}
						ConsortiaUserInfo[] array = allMemberByConsortia2;
						foreach (ConsortiaUserInfo consortiaUserInfo3 in array)
						{
							GamePlayer playerById2 = WorldMgr.GetPlayerById(consortiaUserInfo3.UserID);
							if (playerById2 != null)
							{
								abstractBuffer?.Start(playerById2);
								if (playerById2 != client.Player)
								{
									playerById2.Out.SendMessage(eMessageType.Normal, "Guild cập nhật kỹ năng Guild, thành công!");
								}
							}
						}
						if (abstractBuffer != null)
						{
							ConsortiaBufferInfo consortiaBufferInfo = playerBussiness2.GetUserConsortiaBufferSingle(consortiaBuffTempInfo.id);
							if (consortiaBufferInfo == null)
							{
								consortiaBufferInfo = new ConsortiaBufferInfo();
								consortiaBufferInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
								consortiaBufferInfo.IsOpen = true;
								consortiaBufferInfo.BufferID = consortiaBuffTempInfo.id;
								consortiaBufferInfo.Type = abstractBuffer.Info.Type;
								consortiaBufferInfo.Value = abstractBuffer.Info.Value;
								consortiaBufferInfo.ValidDate = abstractBuffer.Info.ValidDate;
								consortiaBufferInfo.BeginDate = abstractBuffer.Info.BeginDate;
							}
							else
							{
								consortiaBufferInfo.BufferID = consortiaBuffTempInfo.id;
								consortiaBufferInfo.Value = abstractBuffer.Info.Value;
								consortiaBufferInfo.ValidDate += abstractBuffer.Info.ValidDate;
							}
							playerBussiness2.SaveConsortiaBuffer(consortiaBufferInfo);
						}
					}
					break;
				}
				}
				if (num8 == 1)
				{
					if (consortiaBuffTempInfo.type == 1)
					{
						using ConsortiaBussiness consortiaBussiness9 = new ConsortiaBussiness();
						int riches = num9;
						consortiaBussiness9.ConsortiaRichRemove(client.Player.PlayerCharacter.ConsortiaID, ref riches);
					}
					else
					{
						client.Player.RemoveOffer(num9);
					}
				}
				else
				{
					client.Player.RemoveGiftToken(num9);
				}
				client.Out.SendMessage(eMessageType.Normal, "Thao tác thành công.");
				return 0;
			}
			client.Out.SendMessage(eMessageType.Normal, "Level Guild chưa đủ.");
			return 0;
		}
		case 28:
		{
			int num4 = packet.ReadInt();
			msg = "BuyBadgeHandler.Fail";
			int validDate = 30;
			string badgeBuyTime = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss");
			ConsortiaInfo consortiaInfo2 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			using (ConsortiaBussiness consortiaBussiness4 = new ConsortiaBussiness())
			{
				consortiaInfo2.BadgeID = num4;
				consortiaInfo2.ValidDate = validDate;
				consortiaInfo2.BadgeBuyTime = badgeBuyTime;
				if (consortiaBussiness4.BuyBadge(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, consortiaInfo2, ref msg))
				{
					msg = "BuyBadgeHandler.Success";
					flag = true;
				}
			}
			if (flag)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				ConsortiaUserInfo[] allMemberByConsortia = playerBussiness.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
				ConsortiaUserInfo[] array = allMemberByConsortia;
				foreach (ConsortiaUserInfo consortiaUserInfo2 in array)
				{
					GamePlayer playerById = WorldMgr.GetPlayerById(consortiaUserInfo2.UserID);
					if (playerById != null && playerById.PlayerId != client.Player.PlayerCharacter.ID)
					{
						playerById.UpdateBadgeId(num4);
						playerById.SendMessage("Guild của bạn đã thay đổi huy hiệu mới!");
						playerById.UpdateProperties();
					}
				}
			}
			client.Player.SendMessage(msg);
			client.Out.sendBuyBadge(num4, validDate, flag, badgeBuyTime, client.Player.PlayerCharacter.ID);
			client.Player.UpdateBadgeId(num4);
			client.Player.UpdateProperties();
			return 0;
		}
		case 29:
		{
			string title = packet.ReadString();
			string content = packet.ReadString();
			msg = "ConsortiaRichiUpdateHandler.Failed";
			ConsortiaInfo consortiaInfo10 = ConsortiaMgr.FindConsortiaInfo(client.Player.PlayerCharacter.ConsortiaID);
			using (PlayerBussiness playerBussiness3 = new PlayerBussiness())
			{
				ConsortiaUserInfo[] allMemberByConsortia3 = playerBussiness3.GetAllMemberByConsortia(client.Player.PlayerCharacter.ConsortiaID);
				MailInfo mailInfo = new MailInfo();
				ConsortiaUserInfo[] array = allMemberByConsortia3;
				foreach (ConsortiaUserInfo consortiaUserInfo5 in array)
				{
					mailInfo.SenderID = client.Player.PlayerCharacter.ID;
					mailInfo.Sender = "Chủ Guild " + consortiaInfo10.ConsortiaName;
					mailInfo.ReceiverID = consortiaUserInfo5.UserID;
					mailInfo.Receiver = consortiaUserInfo5.UserName;
					mailInfo.Title = title;
					mailInfo.Content = content;
					mailInfo.Type = 59;
					if (consortiaUserInfo5.UserID != client.Player.PlayerCharacter.ID && playerBussiness3.SendMail(mailInfo))
					{
						msg = "ConsortiaRichiUpdateHandler.Success";
						flag = true;
						if (consortiaUserInfo5.State != 0)
						{
							client.Player.Out.SendMailResponse(consortiaUserInfo5.UserID, eMailRespose.Receiver);
						}
						client.Player.Out.SendMailResponse(client.Player.PlayerCharacter.ID, eMailRespose.Send);
					}
					if (!flag)
					{
						client.Player.SendMessage("SendMail Error!");
						break;
					}
				}
			}
			if (flag)
			{
				using ConsortiaBussiness consortiaBussiness28 = new ConsortiaBussiness();
				consortiaBussiness28.UpdateConsortiaRiches(client.Player.PlayerCharacter.ConsortiaID, client.Player.PlayerCharacter.ID, 1000, ref msg);
			}
			client.Out.SendConsortiaMail(flag, client.Player.PlayerCharacter.ID);
			return 0;
		}
		case 30:
		{
			byte b = packet.ReadByte();
			int num11 = packet.ReadInt();
			int consortiaID2 = client.Player.PlayerCharacter.ConsortiaID;
			ConsortiaInfo consortiaInfo4 = ConsortiaBossMgr.GetConsortiaById(consortiaID2);
			if (consortiaInfo4 == null || consortiaInfo4.LastOpenBoss.Date < DateTime.Now.Date)
			{
				using ConsortiaBussiness consortiaBussiness10 = new ConsortiaBussiness();
				consortiaInfo4 = consortiaBussiness10.GetConsortiaSingle(consortiaID2);
			}
			if (consortiaInfo4 == null)
			{
				client.Player.SendMessage("Không tìm thấy Guild, thao tác thất bại!");
				return 0;
			}
			if (consortiaInfo4.Level < 3)
			{
				client.Player.SendMessage("Guild cấp 3 trở lên mới có thể triệu hồi boss.");
				return 0;
			}
			int num12 = ConsortiaMgr.FindConsortiaBossBossMaxLevel(0, consortiaInfo4);
			ConsortiaBossConfigInfo consortiaBossConfigInfo = ConsortiaMgr.FindConsortiaBossConfig(num12);
			if (consortiaBossConfigInfo == null || (consortiaBossConfigInfo.Level != num11 && num11 > 1))
			{
				client.Player.SendMessage("Dữ liệu server lổi, thao tác thất bại! ");
				return 0;
			}
			switch (b)
			{
			case 0:
			{
				if (consortiaInfo4.Riches < consortiaBossConfigInfo.CostRich)
				{
					client.Player.SendMessage("Tài sản Guild không đủ, tạo boss thất bại!");
					return 0;
				}
				consortiaInfo4.bossState = 1;
				consortiaInfo4.endTime = DateTime.Now.AddMinutes(20.0);
				consortiaInfo4.LastOpenBoss = DateTime.Now;
				using (ConsortiaBussiness consortiaBussiness12 = new ConsortiaBussiness())
				{
					int riches3 = consortiaBossConfigInfo.CostRich;
					if (consortiaBussiness12.ConsortiaRichRemove(client.Player.PlayerCharacter.ConsortiaID, ref riches3))
					{
						consortiaInfo4.Riches = riches3;
					}
				}
				ConsortiaBossMgr.CreateBoss(consortiaInfo4, consortiaBossConfigInfo.NpcID);
				return 0;
			}
			case 2:
			{
				if (consortiaInfo4.Riches < consortiaBossConfigInfo.ProlongRich)
				{
					client.Player.SendMessage("Tài sản Guild không đủ, thêm thời gian thất bại!");
					return 0;
				}
				using (ConsortiaBussiness consortiaBussiness11 = new ConsortiaBussiness())
				{
					int riches2 = consortiaBossConfigInfo.ProlongRich;
					if (consortiaBussiness11.ConsortiaRichRemove(client.Player.PlayerCharacter.ConsortiaID, ref riches2))
					{
						consortiaInfo4.Riches = riches2;
					}
				}
				ConsortiaBossMgr.ExtendAvailable(consortiaID2, consortiaInfo4.Riches);
				return 0;
			}
			default:
				if (ConsortiaBossMgr.GetConsortiaExit(consortiaID2))
				{
					ConsortiaBossMgr.reload(consortiaID2);
					return 0;
				}
				consortiaInfo4.bossState = 0;
				consortiaInfo4.endTime = DateTime.Now;
				consortiaInfo4.extendAvailableNum = 3;
				consortiaInfo4.callBossLevel = num12;
				ConsortiaBossMgr.AddConsortia(consortiaInfo4);
				return 0;
			}
		}
		case 10:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			int dutyID = packet.ReadInt();
			int num6 = packet.ReadByte();
			msg = "ConsortiaDutyUpdateHandler.Failed";
			using ConsortiaBussiness consortiaBussiness7 = new ConsortiaBussiness();
			ConsortiaDutyInfo consortiaDutyInfo2 = new ConsortiaDutyInfo();
			consortiaDutyInfo2.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
			consortiaDutyInfo2.DutyID = dutyID;
			consortiaDutyInfo2.IsExist = true;
			consortiaDutyInfo2.DutyName = "";
			switch (num6)
			{
			case 1:
				return 1;
			case 2:
				consortiaDutyInfo2.DutyName = packet.ReadString();
				if (string.IsNullOrEmpty(consortiaDutyInfo2.DutyName) || Encoding.Default.GetByteCount(consortiaDutyInfo2.DutyName) > 10)
				{
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("ConsortiaDutyUpdateHandler.Long"));
					return 1;
				}
				consortiaDutyInfo2.Right = packet.ReadInt();
				break;
			}
			if (consortiaBussiness7.UpdateConsortiaDuty(consortiaDutyInfo2, client.Player.PlayerCharacter.ID, num6, ref msg))
			{
				dutyID = consortiaDutyInfo2.DutyID;
				msg = "ConsortiaDutyUpdateHandler.Success";
				flag = true;
				GameServer.Instance.LoginServer.SendConsortiaDuty(consortiaDutyInfo2, num6, client.Player.PlayerCharacter.ConsortiaID);
			}
			return 0;
		}
		case 11:
		{
			if (client.Player.PlayerCharacter.ConsortiaID == 0)
			{
				return 0;
			}
			string text = packet.ReadString();
			msg = "ConsortiaInviteAddHandler.Failed";
			if (string.IsNullOrEmpty(text))
			{
				return 0;
			}
			using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
			{
				ConsortiaInviteUserInfo consortiaInviteUserInfo = new ConsortiaInviteUserInfo();
				consortiaInviteUserInfo.ConsortiaID = client.Player.PlayerCharacter.ConsortiaID;
				consortiaInviteUserInfo.ConsortiaName = client.Player.PlayerCharacter.ConsortiaName;
				consortiaInviteUserInfo.InviteDate = DateTime.Now;
				consortiaInviteUserInfo.InviteID = client.Player.PlayerCharacter.ID;
				consortiaInviteUserInfo.InviteName = client.Player.PlayerCharacter.NickName;
				consortiaInviteUserInfo.IsExist = true;
				consortiaInviteUserInfo.Remark = "";
				consortiaInviteUserInfo.UserID = 0;
				consortiaInviteUserInfo.UserName = text;
				if (consortiaBussiness.AddConsortiaInviteUsers(consortiaInviteUserInfo, ref msg))
				{
					msg = "ConsortiaInviteAddHandler.Success";
					flag = true;
					GameServer.Instance.LoginServer.SendConsortiaInvite(consortiaInviteUserInfo.ID, consortiaInviteUserInfo.UserID, consortiaInviteUserInfo.UserName, consortiaInviteUserInfo.InviteID, consortiaInviteUserInfo.InviteName, consortiaInviteUserInfo.ConsortiaName, consortiaInviteUserInfo.ConsortiaID);
				}
			}
			client.Out.SendConsortiaInvite(text, flag, LanguageMgr.GetTranslation(msg), client.Player.PlayerCharacter.ID);
			return 0;
		}
		default:
			Console.WriteLine("ConsortiaPackageType." + (ConsortiaPackageType)num);
			return 0;
		}
	}
}
