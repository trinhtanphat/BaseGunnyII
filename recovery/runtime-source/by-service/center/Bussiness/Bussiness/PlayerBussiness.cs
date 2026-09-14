using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using Bussiness.CenterService;
using Bussiness.Managers;
using SqlDataProvider.Data;

namespace Bussiness;

public class PlayerBussiness : BaseBussiness
{
	public bool AddStore(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ItemID", item.ItemID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
			array[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
			array[8] = new SqlParameter("@Count", item.Count);
			array[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
			array[10] = new SqlParameter("@IsBinds", item.IsBinds);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@IsJudge", item.IsJudge);
			array[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
			result = db.RunProcedure("SP_Users_Items_Add", array);
			item.ItemID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpateStore(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ItemID", item.ItemID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
			array[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
			array[8] = new SqlParameter("@Count", item.Count);
			array[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
			array[10] = new SqlParameter("@IsBinds", item.IsBinds);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@IsJudge", item.IsJudge);
			array[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
			result = db.RunProcedure("SP_Users_Items_Add", array);
			item.ItemID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateBuyStore(int storeId)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@StoreID", storeId)
			};
			result = db.RunProcedure("SP_Update_Buy_Store", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Update_Buy_Store", exception);
			}
		}
		return result;
	}

	public ConsortiaUserInfo[] GetAllMemberByConsortia(int ConsortiaID)
	{
		List<ConsortiaUserInfo> list = new List<ConsortiaUserInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
			};
			array[0].Value = ConsortiaID;
			db.GetReader(ref ResultDataReader, "SP_Consortia_Users_All", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitConsortiaUserInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ConsortiaUserInfo InitConsortiaUserInfo(SqlDataReader dr)
	{
		ConsortiaUserInfo consortiaUserInfo = new ConsortiaUserInfo();
		consortiaUserInfo.ID = (int)dr["ID"];
		consortiaUserInfo.ConsortiaID = (int)dr["ConsortiaID"];
		consortiaUserInfo.DutyID = (int)dr["DutyID"];
		consortiaUserInfo.DutyName = dr["DutyName"].ToString();
		consortiaUserInfo.IsExist = (bool)dr["IsExist"];
		consortiaUserInfo.RatifierID = (int)dr["RatifierID"];
		consortiaUserInfo.RatifierName = dr["RatifierName"].ToString();
		consortiaUserInfo.Remark = dr["Remark"].ToString();
		consortiaUserInfo.UserID = (int)dr["UserID"];
		consortiaUserInfo.UserName = dr["UserName"].ToString();
		consortiaUserInfo.Grade = (int)dr["Grade"];
		consortiaUserInfo.GP = (int)dr["GP"];
		consortiaUserInfo.Repute = (int)dr["Repute"];
		consortiaUserInfo.State = (int)dr["State"];
		consortiaUserInfo.Right = (int)dr["Right"];
		consortiaUserInfo.Offer = (int)dr["Offer"];
		consortiaUserInfo.Colors = dr["Colors"].ToString();
		consortiaUserInfo.Style = dr["Style"].ToString();
		consortiaUserInfo.Hide = (int)dr["Hide"];
		consortiaUserInfo.Skin = ((dr["Skin"] == null) ? "" : consortiaUserInfo.Skin);
		consortiaUserInfo.Level = (int)dr["Level"];
		consortiaUserInfo.LastDate = (DateTime)dr["LastDate"];
		consortiaUserInfo.Sex = (bool)dr["Sex"];
		consortiaUserInfo.IsBanChat = (bool)dr["IsBanChat"];
		consortiaUserInfo.Win = (int)dr["Win"];
		consortiaUserInfo.Total = (int)dr["Total"];
		consortiaUserInfo.Escape = (int)dr["Escape"];
		consortiaUserInfo.RichesOffer = (int)dr["RichesOffer"];
		consortiaUserInfo.RichesRob = (int)dr["RichesRob"];
		consortiaUserInfo.LoginName = ((dr["LoginName"] == null) ? "" : dr["LoginName"].ToString());
		consortiaUserInfo.Nimbus = (int)dr["Nimbus"];
		consortiaUserInfo.FightPower = (int)dr["FightPower"];
		consortiaUserInfo.typeVIP = Convert.ToByte(dr["typeVIP"]);
		consortiaUserInfo.VIPLevel = (int)dr["VIPLevel"];
		return consortiaUserInfo;
	}

	public bool ActivePlayer(ref PlayerInfo player, string userName, string passWord, bool sex, int gold, int money, string IP, string site)
	{
		bool result = false;
		try
		{
			player = new PlayerInfo();
			player.Agility = 0;
			player.Attack = 0;
			player.Colors = ",,,,,,";
			player.Skin = "";
			player.ConsortiaID = 0;
			player.Defence = 0;
			player.Gold = 0;
			player.GP = 1;
			player.Grade = 1;
			player.ID = 0;
			player.Luck = 0;
			player.Money = 0;
			player.NickName = "";
			player.Sex = sex;
			player.State = 0;
			player.Style = ",,,,,,";
			player.Hide = 1111111111;
			SqlParameter[] array = new SqlParameter[21];
			array[0] = new SqlParameter("@UserID", SqlDbType.Int);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Attack", player.Attack);
			array[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
			array[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
			array[4] = new SqlParameter("@Defence", player.Defence);
			array[5] = new SqlParameter("@Gold", player.Gold);
			array[6] = new SqlParameter("@GP", player.GP);
			array[7] = new SqlParameter("@Grade", player.Grade);
			array[8] = new SqlParameter("@Luck", player.Luck);
			array[9] = new SqlParameter("@Money", player.Money);
			array[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
			array[11] = new SqlParameter("@Agility", player.Agility);
			array[12] = new SqlParameter("@State", player.State);
			array[13] = new SqlParameter("@UserName", userName);
			array[14] = new SqlParameter("@PassWord", passWord);
			array[15] = new SqlParameter("@Sex", sex);
			array[16] = new SqlParameter("@Hide", player.Hide);
			array[17] = new SqlParameter("@ActiveIP", IP);
			array[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[19].Direction = ParameterDirection.ReturnValue;
			array[20] = new SqlParameter("@Site", site);
			result = db.RunProcedure("SP_Users_Active", array);
			player.ID = (int)array[0].Value;
			result = (int)array[19].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RegisterPlayer(string userName, string passWord, string nickName, string bStyle, string gStyle, string armColor, string hairColor, string faceColor, string clothColor, string hatColor, int sex, ref string msg, int validDate)
	{
		bool result = false;
		try
		{
			string[] array = bStyle.Split(',');
			string[] array2 = gStyle.Split(',');
			SqlParameter[] array3 = new SqlParameter[21]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@PassWord", passWord),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@BArmID", int.Parse(array[0])),
				new SqlParameter("@BHairID", int.Parse(array[1])),
				new SqlParameter("@BFaceID", int.Parse(array[2])),
				new SqlParameter("@BClothID", int.Parse(array[3])),
				new SqlParameter("@BHatID", int.Parse(array[4])),
				new SqlParameter("@GArmID", int.Parse(array2[0])),
				new SqlParameter("@GHairID", int.Parse(array2[1])),
				new SqlParameter("@GFaceID", int.Parse(array2[2])),
				new SqlParameter("@GClothID", int.Parse(array2[3])),
				new SqlParameter("@GHatID", int.Parse(array2[4])),
				new SqlParameter("@ArmColor", armColor),
				new SqlParameter("@HairColor", hairColor),
				new SqlParameter("@FaceColor", faceColor),
				new SqlParameter("@ClothColor", clothColor),
				new SqlParameter("@HatColor", clothColor),
				new SqlParameter("@Sex", sex),
				new SqlParameter("@StyleDate", validDate),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array3[20].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RegisterNotValidate", array3);
			int num = (int)array3[20].Value;
			result = num == 0;
			switch (num)
			{
			case 2:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg2");
				break;
			case 3:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.RegisterPlayer.Msg3");
				break;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RenameNick(string userName, string nickName, string newNickName, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@NewNickName", newNickName),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RenameNick", array);
			int num = (int)array[3].Value;
			result = num == 0;
			switch (num)
			{
			case 4:
			case 5:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.RenameNick.Msg4");
				break;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("RenameNick", exception);
			}
		}
		return result;
	}

	public bool RenameNick(string userName, string nickName, string newNickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@NewNickName", newNickName),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Users_RenameNick2", array);
			int num = (int)array[3].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("RenameNick", exception);
			}
		}
		return result;
	}

	public bool DisableUser(string userName, bool isExit)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@IsExist", isExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Disable_User", array);
			if ((int)array[2].Value == 0)
			{
				result = true;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DisableUser", exception);
			}
		}
		return result;
	}

	public bool UpdatePassWord(int userID, string password)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Password", password)
			};
			result = db.RunProcedure("SP_Users_UpdatePassword", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdatePasswordInfo(int userID, string PasswordQuestion1, string PasswordAnswer1, string PasswordQuestion2, string PasswordAnswer2, int Count)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[6]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@PasswordQuestion1", PasswordQuestion1),
				new SqlParameter("@PasswordAnswer1", PasswordAnswer1),
				new SqlParameter("@PasswordQuestion2", PasswordQuestion2),
				new SqlParameter("@PasswordAnswer2", PasswordAnswer2),
				new SqlParameter("@FailedPasswordAttemptCount", Count)
			};
			result = db.RunProcedure("SP_Users_Password_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public void GetPasswordInfo(int userID, ref string PasswordQuestion1, ref string PasswordAnswer1, ref string PasswordQuestion2, ref string PasswordAnswer2, ref int Count)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", userID)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_PasswordInfo", sqlParameters);
			while (ResultDataReader.Read())
			{
				PasswordQuestion1 = ((ResultDataReader["PasswordQuestion1"] == null) ? "" : ResultDataReader["PasswordQuestion1"].ToString());
				PasswordAnswer1 = ((ResultDataReader["PasswordAnswer1"] == null) ? "" : ResultDataReader["PasswordAnswer1"].ToString());
				PasswordQuestion2 = ((ResultDataReader["PasswordQuestion2"] == null) ? "" : ResultDataReader["PasswordQuestion2"].ToString());
				PasswordAnswer2 = ((ResultDataReader["PasswordAnswer2"] == null) ? "" : ResultDataReader["PasswordAnswer2"].ToString());
				DateTime dateTime = (DateTime)ResultDataReader["LastFindDate"];
				if (dateTime == DateTime.Today)
				{
					Count = (int)ResultDataReader["FailedPasswordAttemptCount"];
				}
				else
				{
					Count = 5;
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
	}

	public bool UpdatePasswordTwo(int userID, string passwordTwo)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", userID),
				new SqlParameter("@PasswordTwo", passwordTwo)
			};
			result = db.RunProcedure("SP_Users_UpdatePasswordTwo", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public PlayerInfo[] GetUserLoginList(string userName)
	{
		List<PlayerInfo> list = new List<PlayerInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_LoginList", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitPlayerInfo(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PlayerInfo LoginGame(string username, ref int isFirst, ref bool isExist, ref bool isError, bool firstValidate, ref DateTime forbidDate, string nickname)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", ""),
				new SqlParameter("@FirstValidate", firstValidate),
				new SqlParameter("@Nickname", nickname)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_LoginWeb", sqlParameters);
			if (ResultDataReader.Read())
			{
				isFirst = (int)ResultDataReader["IsFirst"];
				isExist = (bool)ResultDataReader["IsExist"];
				forbidDate = (DateTime)ResultDataReader["ForbidDate"];
				if (isFirst > 1)
				{
					isFirst--;
				}
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			isError = true;
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public PlayerInfo LoginGame(string username, string password)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserName", username),
				new SqlParameter("@Password", password)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_Login", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public PlayerInfo ReLoadPlayer(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_Users_Reload", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool UpdatePlayer(PlayerInfo player)
	{
		bool flag = false;
		try
		{
			if (player.Grade < 1)
			{
				return flag;
			}
			SqlParameter[] array = new SqlParameter[81];
			array[0] = new SqlParameter("@UserID", player.ID);
			array[1] = new SqlParameter("@Attack", player.Attack);
			array[2] = new SqlParameter("@Colors", (player.Colors == null) ? "" : player.Colors);
			array[3] = new SqlParameter("@ConsortiaID", player.ConsortiaID);
			array[4] = new SqlParameter("@Defence", player.Defence);
			array[5] = new SqlParameter("@Gold", player.Gold);
			array[6] = new SqlParameter("@GP", player.GP);
			array[7] = new SqlParameter("@Grade", player.Grade);
			array[8] = new SqlParameter("@Luck", player.Luck);
			array[9] = new SqlParameter("@Money", player.Money);
			array[10] = new SqlParameter("@Style", (player.Style == null) ? "" : player.Style);
			array[11] = new SqlParameter("@Agility", player.Agility);
			array[12] = new SqlParameter("@State", player.State);
			array[13] = new SqlParameter("@Hide", player.Hide);
			array[14] = new SqlParameter("@ExpendDate", (!player.ExpendDate.HasValue) ? "" : player.ExpendDate.ToString());
			array[15] = new SqlParameter("@Win", player.Win);
			array[16] = new SqlParameter("@Total", player.Total);
			array[17] = new SqlParameter("@Escape", player.Escape);
			array[18] = new SqlParameter("@Skin", (player.Skin == null) ? "" : player.Skin);
			array[19] = new SqlParameter("@Offer", player.Offer);
			array[20] = new SqlParameter("@AntiAddiction", player.AntiAddiction);
			array[20].Direction = ParameterDirection.InputOutput;
			array[21] = new SqlParameter("@Result", SqlDbType.Int);
			array[21].Direction = ParameterDirection.ReturnValue;
			array[22] = new SqlParameter("@RichesOffer", player.RichesOffer);
			array[23] = new SqlParameter("@RichesRob", player.RichesRob);
			array[24] = new SqlParameter("@CheckCount", player.CheckCount);
			array[24].Direction = ParameterDirection.InputOutput;
			array[25] = new SqlParameter("@MarryInfoID", player.MarryInfoID);
			array[26] = new SqlParameter("@DayLoginCount", player.DayLoginCount);
			array[27] = new SqlParameter("@Nimbus", player.Nimbus);
			array[28] = new SqlParameter("@LastAward", player.LastAward);
			array[29] = new SqlParameter("@GiftToken", player.GiftToken);
			array[30] = new SqlParameter("@QuestSite", player.QuestSite);
			array[31] = new SqlParameter("@PvePermission", player.PvePermission);
			array[32] = new SqlParameter("@FightPower", player.FightPower);
			array[33] = new SqlParameter("@AnswerSite", player.AnswerSite);
			array[34] = new SqlParameter("@LastAuncherAward", player.LastAward);
			array[35] = new SqlParameter("@hp", player.hp);
			array[36] = new SqlParameter("@ChatCount", player.ChatCount);
			array[37] = new SqlParameter("@SpaPubGoldRoomLimit", player.SpaPubGoldRoomLimit);
			array[38] = new SqlParameter("@LastSpaDate", player.LastSpaDate);
			array[39] = new SqlParameter("@FightLabPermission", player.FightLabPermission);
			array[40] = new SqlParameter("@SpaPubMoneyRoomLimit", player.SpaPubMoneyRoomLimit);
			array[41] = new SqlParameter("@IsInSpaPubGoldToday", player.IsInSpaPubGoldToday);
			array[42] = new SqlParameter("@IsInSpaPubMoneyToday", player.IsInSpaPubMoneyToday);
			array[43] = new SqlParameter("@AchievementPoint", player.AchievementPoint);
			array[44] = new SqlParameter("@LastWeekly", player.LastWeekly);
			array[45] = new SqlParameter("@LastWeeklyVersion", player.LastWeeklyVersion);
			array[46] = new SqlParameter("@GiftGp", player.GiftGp);
			array[47] = new SqlParameter("@GiftLevel", player.GiftLevel);
			array[48] = new SqlParameter("@IsOpenGift", player.IsOpenGift);
			array[49] = new SqlParameter("@WeaklessGuildProgressStr", player.WeaklessGuildProgressStr);
			array[50] = new SqlParameter("@IsOldPlayer", player.IsOldPlayer);
			array[51] = new SqlParameter("@VIPLevel", player.VIPLevel);
			array[52] = new SqlParameter("@VIPExp", player.VIPExp);
			array[53] = new SqlParameter("@Score", player.Score);
			array[54] = new SqlParameter("@OptionOnOff", player.OptionOnOff);
			array[55] = new SqlParameter("@isOldPlayerHasValidEquitAtLogin", player.isOldPlayerHasValidEquitAtLogin);
			array[56] = new SqlParameter("@badLuckNumber", player.badLuckNumber);
			array[57] = new SqlParameter("@luckyNum", player.luckyNum);
			array[58] = new SqlParameter("@lastLuckyNumDate", player.lastLuckyNumDate.ToString());
			array[59] = new SqlParameter("@lastLuckNum", player.lastLuckNum);
			array[60] = new SqlParameter("@CardSoul", player.CardSoul);
			array[61] = new SqlParameter("@uesedFinishTime", player.uesedFinishTime);
			array[62] = new SqlParameter("@totemId", player.totemId);
			array[63] = new SqlParameter("@damageScores", player.damageScores);
			array[64] = new SqlParameter("@petScore", player.petScore);
			array[65] = new SqlParameter("@IsShowConsortia", player.IsShowConsortia);
			array[66] = new SqlParameter("@LastRefreshPet", player.LastRefreshPet.ToString());
			array[67] = new SqlParameter("@GetSoulCount", player.GetSoulCount);
			array[68] = new SqlParameter("@isFirstDivorce", player.isFirstDivorce);
			array[69] = new SqlParameter("@needGetBoxTime", player.needGetBoxTime);
			array[70] = new SqlParameter("@myScore", player.myScore);
			array[71] = new SqlParameter("@TimeBox", player.TimeBox.ToString());
			array[72] = new SqlParameter("@IsFistGetPet", player.IsFistGetPet);
			array[73] = new SqlParameter("@MaxBuyHonor", player.MaxBuyHonor);
			array[74] = new SqlParameter("@Medal", player.medal);
			array[75] = new SqlParameter("@myHonor", player.myHonor);
			array[76] = new SqlParameter("@LeagueMoney", player.LeagueMoney);
			array[77] = new SqlParameter("@Honor", player.Honor);
			array[78] = new SqlParameter("@necklaceExp", player.necklaceExp);
			array[79] = new SqlParameter("@necklaceExpAdd", player.necklaceExpAdd);
			array[80] = new SqlParameter("@hardCurrency", player.hardCurrency);
			db.RunProcedure("SP_Users_Update", array);
			flag = (int)array[21].Value == 0;
			if (flag)
			{
				player.AntiAddiction = (int)array[20].Value;
				player.CheckCount = (int)array[24].Value;
			}
			player.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return flag;
	}

	public bool UpdatePlayerMarry(PlayerInfo player)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@UserID", player.ID),
				new SqlParameter("@IsMarried", player.IsMarried),
				new SqlParameter("@SpouseID", player.SpouseID),
				new SqlParameter("@SpouseName", player.SpouseName),
				new SqlParameter("@IsCreatedMarryRoom", player.IsCreatedMarryRoom),
				new SqlParameter("@SelfMarryRoomID", player.SelfMarryRoomID),
				new SqlParameter("@IsGotRing", player.IsGotRing)
			};
			result = db.RunProcedure("SP_Users_Marry", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerMarry", exception);
			}
		}
		return result;
	}

	public bool UpdatePlayerLastAward(int id, int type)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@UserID", id),
				new SqlParameter("@Type", type)
			};
			result = db.RunProcedure("SP_Users_LastAward", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerAward", exception);
			}
		}
		return result;
	}

	public PlayerInfo GetUserSingleByUserID(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserID", array);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public PlayerLimitInfo GetUserLimitByUserName(string userName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_LimitByUserName", array);
			if (ResultDataReader.Read())
			{
				PlayerLimitInfo playerLimitInfo = new PlayerLimitInfo();
				playerLimitInfo.ID = (int)ResultDataReader["UserID"];
				playerLimitInfo.NickName = (string)ResultDataReader["NickName"];
				return playerLimitInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public PlayerInfo GetUserSingleByUserName(string userName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = userName;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByUserName", array);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public PlayerInfo GetUserSingleByNickName(string nickName)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = nickName;
			db.GetReader(ref ResultDataReader, "SP_Users_SingleByNickName", array);
			if (ResultDataReader.Read())
			{
				return InitPlayerInfo(ResultDataReader);
			}
		}
		catch
		{
			throw new Exception();
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public PlayerInfo InitPlayerInfo(SqlDataReader reader)
	{
		PlayerInfo playerInfo = new PlayerInfo();
		playerInfo.Password = (string)reader["Password"];
		playerInfo.IsConsortia = (bool)reader["IsConsortia"];
		playerInfo.Agility = (int)reader["Agility"];
		playerInfo.Attack = (int)reader["Attack"];
		playerInfo.hp = (int)reader["hp"];
		playerInfo.Colors = ((reader["Colors"] == null) ? "" : reader["Colors"].ToString());
		playerInfo.ConsortiaID = (int)reader["ConsortiaID"];
		playerInfo.Defence = (int)reader["Defence"];
		playerInfo.Gold = (int)reader["Gold"];
		playerInfo.GP = (int)reader["GP"];
		playerInfo.Grade = (int)reader["Grade"];
		playerInfo.ID = (int)reader["UserID"];
		playerInfo.Luck = (int)reader["Luck"];
		playerInfo.Money = (int)reader["Money"];
		playerInfo.NickName = (((string)reader["NickName"] == null) ? "" : ((string)reader["NickName"]));
		playerInfo.Sex = (bool)reader["Sex"];
		playerInfo.State = (int)reader["State"];
		playerInfo.Style = ((reader["Style"] == null) ? "" : reader["Style"].ToString());
		playerInfo.Hide = (int)reader["Hide"];
		playerInfo.Repute = (int)reader["Repute"];
		playerInfo.UserName = ((reader["UserName"] == null) ? "" : reader["UserName"].ToString());
		playerInfo.ConsortiaName = ((reader["ConsortiaName"] == null) ? "" : reader["ConsortiaName"].ToString());
		playerInfo.Offer = (int)reader["Offer"];
		playerInfo.Win = (int)reader["Win"];
		playerInfo.Total = (int)reader["Total"];
		playerInfo.Escape = (int)reader["Escape"];
		playerInfo.Skin = ((reader["Skin"] == null) ? "" : reader["Skin"].ToString());
		playerInfo.IsBanChat = (bool)reader["IsBanChat"];
		playerInfo.ReputeOffer = (int)reader["ReputeOffer"];
		playerInfo.ConsortiaRepute = (int)reader["ConsortiaRepute"];
		playerInfo.ConsortiaLevel = (int)reader["ConsortiaLevel"];
		playerInfo.StoreLevel = (int)reader["StoreLevel"];
		playerInfo.ShopLevel = (int)reader["ShopLevel"];
		playerInfo.SmithLevel = (int)reader["SmithLevel"];
		playerInfo.ConsortiaHonor = (int)reader["ConsortiaHonor"];
		playerInfo.RichesOffer = (int)reader["RichesOffer"];
		playerInfo.RichesRob = (int)reader["RichesRob"];
		playerInfo.AntiAddiction = (int)reader["AntiAddiction"];
		playerInfo.DutyLevel = (int)reader["DutyLevel"];
		playerInfo.DutyName = ((reader["DutyName"] == null) ? "" : reader["DutyName"].ToString());
		playerInfo.Right = (int)reader["Right"];
		playerInfo.ChairmanName = ((reader["ChairmanName"] == null) ? "" : reader["ChairmanName"].ToString());
		playerInfo.AddDayGP = (int)reader["AddDayGP"];
		playerInfo.AddDayOffer = (int)reader["AddDayOffer"];
		playerInfo.AddWeekGP = (int)reader["AddWeekGP"];
		playerInfo.AddWeekOffer = (int)reader["AddWeekOffer"];
		playerInfo.ConsortiaRiches = (int)reader["ConsortiaRiches"];
		playerInfo.CheckCount = (int)reader["CheckCount"];
		playerInfo.IsMarried = (bool)reader["IsMarried"];
		playerInfo.SpouseID = (int)reader["SpouseID"];
		playerInfo.SpouseName = ((reader["SpouseName"] == null) ? "" : reader["SpouseName"].ToString());
		playerInfo.MarryInfoID = (int)reader["MarryInfoID"];
		playerInfo.IsCreatedMarryRoom = (bool)reader["IsCreatedMarryRoom"];
		playerInfo.DayLoginCount = (int)reader["DayLoginCount"];
		playerInfo.PasswordTwo = ((reader["PasswordTwo"] == null) ? "" : reader["PasswordTwo"].ToString());
		playerInfo.SelfMarryRoomID = (int)reader["SelfMarryRoomID"];
		playerInfo.IsGotRing = (bool)reader["IsGotRing"];
		playerInfo.Rename = (bool)reader["Rename"];
		playerInfo.ConsortiaRename = (bool)reader["ConsortiaRename"];
		playerInfo.IsDirty = false;
		playerInfo.IsFirst = (int)reader["IsFirst"];
		playerInfo.Nimbus = (int)reader["Nimbus"];
		playerInfo.LastAward = (DateTime)reader["LastAward"];
		playerInfo.GiftToken = (int)reader["GiftToken"];
		playerInfo.QuestSite = ((reader["QuestSite"] == null) ? new byte[200] : ((byte[])reader["QuestSite"]));
		playerInfo.PvePermission = ((reader["PvePermission"] == null) ? "" : reader["PvePermission"].ToString());
		playerInfo.FightPower = (int)reader["FightPower"];
		playerInfo.PasswordQuest1 = ((reader["PasswordQuestion1"] == null) ? "" : reader["PasswordQuestion1"].ToString());
		playerInfo.PasswordQuest2 = ((reader["PasswordQuestion2"] == null) ? "" : reader["PasswordQuestion2"].ToString());
		_ = (DateTime)reader["LastFindDate"];
		playerInfo.FailedPasswordAttemptCount = (int)reader["FailedPasswordAttemptCount"];
		playerInfo.AnswerSite = (int)reader["AnswerSite"];
		playerInfo.medal = (int)reader["Medal"];
		playerInfo.ChatCount = (int)reader["ChatCount"];
		playerInfo.SpaPubGoldRoomLimit = (int)reader["SpaPubGoldRoomLimit"];
		playerInfo.LastSpaDate = (DateTime)reader["LastSpaDate"];
		playerInfo.FightLabPermission = (string)reader["FightLabPermission"];
		playerInfo.SpaPubMoneyRoomLimit = (int)reader["SpaPubMoneyRoomLimit"];
		playerInfo.IsInSpaPubGoldToday = (bool)reader["IsInSpaPubGoldToday"];
		playerInfo.IsInSpaPubMoneyToday = (bool)reader["IsInSpaPubMoneyToday"];
		playerInfo.AchievementPoint = (int)reader["AchievementPoint"];
		playerInfo.LastWeekly = (DateTime)reader["LastWeekly"];
		playerInfo.LastWeeklyVersion = (int)reader["LastWeeklyVersion"];
		playerInfo.GiftGp = (int)reader["GiftGp"];
		playerInfo.GiftLevel = (int)reader["GiftLevel"];
		playerInfo.IsOpenGift = (bool)reader["IsOpenGift"];
		playerInfo.badgeID = (int)reader["badgeID"];
		playerInfo.typeVIP = Convert.ToByte(reader["typeVIP"]);
		playerInfo.VIPLevel = (int)reader["VIPLevel"];
		playerInfo.VIPExp = (int)reader["VIPExp"];
		playerInfo.VIPExpireDay = (DateTime)reader["VIPExpireDay"];
		playerInfo.LastVIPPackTime = (DateTime)reader["LastVIPPackTime"];
		playerInfo.CanTakeVipReward = (bool)reader["CanTakeVipReward"];
		playerInfo.WeaklessGuildProgressStr = (string)reader["WeaklessGuildProgressStr"];
		playerInfo.IsOldPlayer = (bool)reader["IsOldPlayer"];
		playerInfo.LastDate = (DateTime)reader["LastDate"];
		playerInfo.VIPLastDate = (DateTime)reader["VIPLastDate"];
		playerInfo.Score = (int)reader["Score"];
		playerInfo.OptionOnOff = (int)reader["OptionOnOff"];
		playerInfo.isOldPlayerHasValidEquitAtLogin = (bool)reader["isOldPlayerHasValidEquitAtLogin"];
		playerInfo.badLuckNumber = (int)reader["badLuckNumber"];
		playerInfo.luckyNum = (int)reader["luckyNum"];
		playerInfo.lastLuckyNumDate = (DateTime)reader["lastLuckyNumDate"];
		playerInfo.lastLuckNum = (int)reader["lastLuckNum"];
		playerInfo.CardSoul = (int)reader["CardSoul"];
		playerInfo.uesedFinishTime = (int)reader["uesedFinishTime"];
		playerInfo.totemId = (int)reader["totemId"];
		playerInfo.damageScores = (int)reader["damageScores"];
		playerInfo.petScore = (int)reader["petScore"];
		playerInfo.IsShowConsortia = (bool)reader["IsShowConsortia"];
		playerInfo.LastRefreshPet = (DateTime)reader["LastRefreshPet"];
		playerInfo.GetSoulCount = (int)reader["GetSoulCount"];
		playerInfo.isFirstDivorce = (int)reader["isFirstDivorce"];
		playerInfo.myScore = (int)reader["myScore"];
		playerInfo.LastGetEgg = (DateTime)reader["LastGetEgg"];
		playerInfo.TimeBox = (DateTime)reader["TimeBox"];
		playerInfo.IsFistGetPet = (bool)reader["IsFistGetPet"];
		playerInfo.myHonor = (int)reader["myHonor"];
		playerInfo.hardCurrency = (int)reader["hardCurrency"];
		playerInfo.MaxBuyHonor = (int)reader["MaxBuyHonor"];
		playerInfo.LeagueMoney = (int)reader["LeagueMoney"];
		playerInfo.Honor = (string)reader["Honor"];
		playerInfo.necklaceExp = (int)reader["necklaceExp"];
		playerInfo.necklaceExpAdd = (int)reader["necklaceExpAdd"];
		return playerInfo;
	}

	public PlayerInfo[] GetPlayerPage(int page, int size, ref int total, int order, int userID, ref bool resultValue)
	{
		List<PlayerInfo> list = new List<PlayerInfo>();
		try
		{
			string text = " IsExist=1 and IsFirst<> 0 ";
			if (userID != -1)
			{
				object obj = text;
				text = string.Concat(obj, " and UserID =", userID, " ");
			}
			string text2 = "GP desc";
			switch (order)
			{
			case 1:
				text2 = "Offer desc";
				break;
			case 2:
				text2 = "AddDayGP desc";
				break;
			case 3:
				text2 = "AddWeekGP desc";
				break;
			case 4:
				text2 = "AddDayOffer desc";
				break;
			case 5:
				text2 = "AddWeekOffer desc";
				break;
			case 6:
				text2 = "FightPower desc";
				break;
			case 7:
				text2 = "AchievementPoint desc";
				break;
			case 8:
				text2 = "AddDayAchievementPoint desc";
				break;
			case 9:
				text2 = "AddWeekAchievementPoint desc";
				break;
			case 10:
				text2 = "GiftGp desc";
				break;
			case 11:
				text2 = "AddDayGiftGp desc";
				break;
			case 12:
				text2 = "AddWeekGiftGp desc";
				break;
			}
			text2 += ",UserID";
			DataTable page2 = GetPage("V_Sys_Users_Detail", text, page, size, "*", text2, "UserID", ref total);
			foreach (DataRow row in page2.Rows)
			{
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.Agility = (int)row["Agility"];
				playerInfo.Attack = (int)row["Attack"];
				playerInfo.Colors = ((row["Colors"] == null) ? "" : row["Colors"].ToString());
				playerInfo.ConsortiaID = (int)row["ConsortiaID"];
				playerInfo.Defence = (int)row["Defence"];
				playerInfo.Gold = (int)row["Gold"];
				playerInfo.GP = (int)row["GP"];
				playerInfo.Grade = (int)row["Grade"];
				playerInfo.ID = (int)row["UserID"];
				playerInfo.Luck = (int)row["Luck"];
				playerInfo.Money = (int)row["Money"];
				playerInfo.NickName = ((row["NickName"] == null) ? "" : row["NickName"].ToString());
				playerInfo.Sex = (bool)row["Sex"];
				playerInfo.State = (int)row["State"];
				playerInfo.Style = ((row["Style"] == null) ? "" : row["Style"].ToString());
				playerInfo.Hide = (int)row["Hide"];
				playerInfo.Repute = (int)row["Repute"];
				playerInfo.UserName = ((row["UserName"] == null) ? "" : row["UserName"].ToString());
				playerInfo.ConsortiaName = ((row["ConsortiaName"] == null) ? "" : row["ConsortiaName"].ToString());
				playerInfo.Offer = (int)row["Offer"];
				playerInfo.Skin = ((row["Skin"] == null) ? "" : row["Skin"].ToString());
				playerInfo.IsBanChat = (bool)row["IsBanChat"];
				playerInfo.ReputeOffer = (int)row["ReputeOffer"];
				playerInfo.ConsortiaRepute = (int)row["ConsortiaRepute"];
				playerInfo.ConsortiaLevel = (int)row["ConsortiaLevel"];
				playerInfo.StoreLevel = (int)row["StoreLevel"];
				playerInfo.ShopLevel = (int)row["ShopLevel"];
				playerInfo.SmithLevel = (int)row["SmithLevel"];
				playerInfo.ConsortiaHonor = (int)row["ConsortiaHonor"];
				playerInfo.RichesOffer = (int)row["RichesOffer"];
				playerInfo.RichesRob = (int)row["RichesRob"];
				playerInfo.DutyLevel = (int)row["DutyLevel"];
				playerInfo.DutyName = ((row["DutyName"] == null) ? "" : row["DutyName"].ToString());
				playerInfo.Right = (int)row["Right"];
				playerInfo.ChairmanName = ((row["ChairmanName"] == null) ? "" : row["ChairmanName"].ToString());
				playerInfo.Win = (int)row["Win"];
				playerInfo.Total = (int)row["Total"];
				playerInfo.Escape = (int)row["Escape"];
				playerInfo.AddDayGP = (((int)row["AddDayGP"] == 0) ? playerInfo.GP : ((int)row["AddDayGP"]));
				playerInfo.AddDayOffer = (((int)row["AddDayOffer"] == 0) ? playerInfo.Offer : ((int)row["AddDayOffer"]));
				playerInfo.AddWeekGP = (((int)row["AddWeekGP"] == 0) ? playerInfo.GP : ((int)row["AddWeekyGP"]));
				playerInfo.AddWeekOffer = (((int)row["AddWeekOffer"] == 0) ? playerInfo.Offer : ((int)row["AddWeekOffer"]));
				playerInfo.ConsortiaRiches = (int)row["ConsortiaRiches"];
				playerInfo.CheckCount = (int)row["CheckCount"];
				playerInfo.Nimbus = (int)row["Nimbus"];
				playerInfo.GiftToken = (int)row["GiftToken"];
				playerInfo.QuestSite = ((row["QuestSite"] == null) ? new byte[200] : ((byte[])row["QuestSite"]));
				playerInfo.PvePermission = ((row["PvePermission"] == null) ? "" : row["PvePermission"].ToString());
				playerInfo.FightPower = (int)row["FightPower"];
				list.Add(playerInfo);
			}
			resultValue = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return list.ToArray();
	}

	public ItemInfo[] GetUserItem(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_All", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ItemInfo[] GetUserBagByType(int UserID, int bagType)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4),
				null
			};
			array[0].Value = UserID;
			array[1] = new SqlParameter("@BagType", bagType);
			db.GetReader(ref ResultDataReader, "SP_Users_BagByType", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public List<ItemInfo> GetUserEuqip(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Equip", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public List<ItemInfo> GetUserBeadEuqip(int UserID)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Bead_Equip", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public List<ItemInfo> GetUserEuqipByNick(string Nick)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NickName", SqlDbType.NVarChar, 200)
			};
			array[0].Value = Nick;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Equip_By_Nick", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItem(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public ItemInfo GetUserItemSingle(int itemID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = itemID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Single", array);
			if (ResultDataReader.Read())
			{
				return InitItem(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public ItemInfo InitItem(SqlDataReader reader)
	{
		ItemInfo itemInfo = new ItemInfo(ItemMgr.FindItemTemplate((int)reader["TemplateID"]));
		itemInfo.AgilityCompose = (int)reader["AgilityCompose"];
		itemInfo.AttackCompose = (int)reader["AttackCompose"];
		itemInfo.Color = reader["Color"].ToString();
		itemInfo.Count = (int)reader["Count"];
		itemInfo.DefendCompose = (int)reader["DefendCompose"];
		itemInfo.ItemID = (int)reader["ItemID"];
		itemInfo.LuckCompose = (int)reader["LuckCompose"];
		itemInfo.Place = (int)reader["Place"];
		itemInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		itemInfo.TemplateID = (int)reader["TemplateID"];
		itemInfo.UserID = (int)reader["UserID"];
		itemInfo.ValidDate = (int)reader["ValidDate"];
		itemInfo.IsDirty = false;
		itemInfo.IsExist = (bool)reader["IsExist"];
		itemInfo.IsBinds = (bool)reader["IsBinds"];
		itemInfo.IsUsed = (bool)reader["IsUsed"];
		itemInfo.BeginDate = (DateTime)reader["BeginDate"];
		itemInfo.IsJudge = (bool)reader["IsJudge"];
		itemInfo.BagType = (int)reader["BagType"];
		itemInfo.Skin = reader["Skin"].ToString();
		itemInfo.RemoveDate = (DateTime)reader["RemoveDate"];
		itemInfo.RemoveType = (int)reader["RemoveType"];
		itemInfo.Hole1 = (int)reader["Hole1"];
		itemInfo.Hole2 = (int)reader["Hole2"];
		itemInfo.Hole3 = (int)reader["Hole3"];
		itemInfo.Hole4 = (int)reader["Hole4"];
		itemInfo.Hole5 = (int)reader["Hole5"];
		itemInfo.Hole6 = (int)reader["Hole6"];
		itemInfo.StrengthenTimes = (int)reader["StrengthenTimes"];
		itemInfo.StrengthenExp = (int)reader["StrengthenExp"];
		itemInfo.Hole5Level = (int)reader["Hole5Level"];
		itemInfo.Hole5Exp = (int)reader["Hole5Exp"];
		itemInfo.Hole6Level = (int)reader["Hole6Level"];
		itemInfo.Hole6Exp = (int)reader["Hole6Exp"];
		itemInfo.goldBeginTime = (DateTime)reader["goldBeginTime"];
		itemInfo.goldValidDate = (int)reader["goldValidDate"];
		itemInfo.beadExp = (int)reader["beadExp"];
		itemInfo.beadLevel = (int)reader["beadLevel"];
		itemInfo.beadIsLock = (bool)reader["beadIsLock"];
		itemInfo.isShowBind = (bool)reader["isShowBind"];
		itemInfo.latentEnergyCurStr = (string)reader["latentEnergyCurStr"];
		itemInfo.latentEnergyNewStr = (string)reader["latentEnergyNewStr"];
		itemInfo.latentEnergyEndTime = (DateTime)reader["latentEnergyEndTime"];
		itemInfo.Damage = (int)reader["Damage"];
		itemInfo.Guard = (int)reader["Guard"];
		itemInfo.Blood = (int)reader["Blood"];
		itemInfo.Bless = (int)reader["Bless"];
		itemInfo.AdvanceDate = (DateTime)reader["AdvanceDate"];
		if (itemInfo.IsGold)
		{
			GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipNewTemplate(itemInfo.TemplateID);
			if (goldEquipTemplateLoadInfo != null)
			{
				itemInfo.GoldEquip = ItemMgr.FindItemTemplate(goldEquipTemplateLoadInfo.NewTemplateId);
			}
		}
		return itemInfo;
	}

	public bool AddGoods(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[47];
			array[0] = new SqlParameter("@ItemID", item.ItemID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.Template.TemplateID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@AgilityCompose", item.AgilityCompose);
			array[5] = new SqlParameter("@AttackCompose", item.AttackCompose);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@Color", (item.Color == null) ? "" : item.Color);
			array[8] = new SqlParameter("@Count", item.Count);
			array[9] = new SqlParameter("@DefendCompose", item.DefendCompose);
			array[10] = new SqlParameter("@IsBinds", item.IsBinds);
			array[11] = new SqlParameter("@IsExist", item.IsExist);
			array[12] = new SqlParameter("@IsJudge", item.IsJudge);
			array[13] = new SqlParameter("@LuckCompose", item.LuckCompose);
			array[14] = new SqlParameter("@StrengthenLevel", item.StrengthenLevel);
			array[15] = new SqlParameter("@ValidDate", item.ValidDate);
			array[16] = new SqlParameter("@BagType", item.BagType);
			array[17] = new SqlParameter("@Skin", (item.Skin == null) ? "" : item.Skin);
			array[18] = new SqlParameter("@IsUsed", item.IsUsed);
			array[19] = new SqlParameter("@RemoveType", item.RemoveType);
			array[20] = new SqlParameter("@Hole1", item.Hole1);
			array[21] = new SqlParameter("@Hole2", item.Hole2);
			array[22] = new SqlParameter("@Hole3", item.Hole3);
			array[23] = new SqlParameter("@Hole4", item.Hole4);
			array[24] = new SqlParameter("@Hole5", item.Hole5);
			array[25] = new SqlParameter("@Hole6", item.Hole6);
			array[26] = new SqlParameter("@StrengthenTimes", item.StrengthenTimes);
			array[27] = new SqlParameter("@Hole5Level", item.Hole5Level);
			array[28] = new SqlParameter("@Hole5Exp", item.Hole5Exp);
			array[29] = new SqlParameter("@Hole6Level", item.Hole6Level);
			array[30] = new SqlParameter("@Hole6Exp", item.Hole6Exp);
			array[31] = new SqlParameter("@IsGold", item.IsGold);
			array[32] = new SqlParameter("@goldValidDate", item.goldValidDate);
			array[33] = new SqlParameter("@StrengthenExp", item.StrengthenExp);
			array[34] = new SqlParameter("@beadExp", item.beadExp);
			array[35] = new SqlParameter("@beadLevel", item.beadLevel);
			array[36] = new SqlParameter("@beadIsLock", item.beadIsLock);
			array[37] = new SqlParameter("@isShowBind", item.isShowBind);
			array[38] = new SqlParameter("@Damage", item.Damage);
			array[39] = new SqlParameter("@Guard", item.Guard);
			array[40] = new SqlParameter("@Blood", item.Blood);
			array[41] = new SqlParameter("@Bless", item.Bless);
			array[42] = new SqlParameter("@goldBeginTime", item.goldBeginTime);
			array[43] = new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime);
			array[44] = new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr);
			array[45] = new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr);
			array[46] = new SqlParameter("@AdvanceDate", item.AdvanceDate);
			result = db.RunProcedure("SP_Users_Items_Add", array);
			item.ItemID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateGoods(ItemInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[48]
			{
				new SqlParameter("@ItemID", item.ItemID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.Template.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@AgilityCompose", item.AgilityCompose),
				new SqlParameter("@AttackCompose", item.AttackCompose),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@DefendCompose", item.DefendCompose),
				new SqlParameter("@IsBinds", item.IsBinds),
				new SqlParameter("@IsExist", item.IsExist),
				new SqlParameter("@IsJudge", item.IsJudge),
				new SqlParameter("@LuckCompose", item.LuckCompose),
				new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
				new SqlParameter("@ValidDate", item.ValidDate),
				new SqlParameter("@BagType", item.BagType),
				new SqlParameter("@Skin", item.Skin),
				new SqlParameter("@IsUsed", item.IsUsed),
				new SqlParameter("@RemoveDate", item.RemoveDate),
				new SqlParameter("@RemoveType", item.RemoveType),
				new SqlParameter("@Hole1", item.Hole1),
				new SqlParameter("@Hole2", item.Hole2),
				new SqlParameter("@Hole3", item.Hole3),
				new SqlParameter("@Hole4", item.Hole4),
				new SqlParameter("@Hole5", item.Hole5),
				new SqlParameter("@Hole6", item.Hole6),
				new SqlParameter("@StrengthenTimes", item.StrengthenTimes),
				new SqlParameter("@Hole5Level", item.Hole5Level),
				new SqlParameter("@Hole5Exp", item.Hole5Exp),
				new SqlParameter("@Hole6Level", item.Hole6Level),
				new SqlParameter("@Hole6Exp", item.Hole6Exp),
				new SqlParameter("@IsGold", item.IsGold),
				new SqlParameter("@goldBeginTime", item.goldBeginTime.ToString()),
				new SqlParameter("@goldValidDate", item.goldValidDate),
				new SqlParameter("@StrengthenExp", item.StrengthenExp),
				new SqlParameter("@beadExp", item.beadExp),
				new SqlParameter("@beadLevel", item.beadLevel),
				new SqlParameter("@beadIsLock", item.beadIsLock),
				new SqlParameter("@isShowBind", item.isShowBind),
				new SqlParameter("@latentEnergyCurStr", item.latentEnergyCurStr),
				new SqlParameter("@latentEnergyNewStr", item.latentEnergyNewStr),
				new SqlParameter("@latentEnergyEndTime", item.latentEnergyEndTime.ToString()),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@Blood", item.Blood),
				new SqlParameter("@Bless", item.Bless),
				new SqlParameter("@AdvanceDate", item.AdvanceDate)
			};
			result = db.RunProcedure("SP_Users_Items_Update", sqlParameters);
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool DeleteGoods(int itemID)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", itemID)
			};
			result = db.RunProcedure("SP_Users_Items_Delete", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public BestEquipInfo[] GetCelebByDayBestEquip()
	{
		List<BestEquipInfo> list = new List<BestEquipInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Users_BestEquip");
			while (ResultDataReader.Read())
			{
				BestEquipInfo bestEquipInfo = new BestEquipInfo();
				bestEquipInfo.Date = (DateTime)ResultDataReader["RemoveDate"];
				bestEquipInfo.GP = (int)ResultDataReader["GP"];
				bestEquipInfo.Grade = (int)ResultDataReader["Grade"];
				bestEquipInfo.ItemName = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				bestEquipInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				bestEquipInfo.Sex = (bool)ResultDataReader["Sex"];
				bestEquipInfo.Strengthenlevel = (int)ResultDataReader["Strengthenlevel"];
				bestEquipInfo.UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
				list.Add(bestEquipInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public MailInfo InitMail(SqlDataReader reader)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Annex1 = reader["Annex1"].ToString();
		mailInfo.Annex2 = reader["Annex2"].ToString();
		mailInfo.Content = reader["Content"].ToString();
		mailInfo.Gold = (int)reader["Gold"];
		mailInfo.ID = (int)reader["ID"];
		mailInfo.IsExist = (bool)reader["IsExist"];
		mailInfo.Money = (int)reader["Money"];
		mailInfo.GiftToken = (int)reader["GiftToken"];
		mailInfo.Receiver = reader["Receiver"].ToString();
		mailInfo.ReceiverID = (int)reader["ReceiverID"];
		mailInfo.Sender = reader["Sender"].ToString();
		mailInfo.SenderID = (int)reader["SenderID"];
		mailInfo.Title = reader["Title"].ToString();
		mailInfo.Type = (int)reader["Type"];
		mailInfo.ValidDate = (int)reader["ValidDate"];
		mailInfo.IsRead = (bool)reader["IsRead"];
		mailInfo.SendTime = (DateTime)reader["SendTime"];
		mailInfo.Annex1Name = ((reader["Annex1Name"] == null) ? "" : reader["Annex1Name"].ToString());
		mailInfo.Annex2Name = ((reader["Annex2Name"] == null) ? "" : reader["Annex2Name"].ToString());
		mailInfo.Annex3 = reader["Annex3"].ToString();
		mailInfo.Annex4 = reader["Annex4"].ToString();
		mailInfo.Annex5 = reader["Annex5"].ToString();
		mailInfo.Annex3Name = ((reader["Annex3Name"] == null) ? "" : reader["Annex3Name"].ToString());
		mailInfo.Annex4Name = ((reader["Annex4Name"] == null) ? "" : reader["Annex4Name"].ToString());
		mailInfo.Annex5Name = ((reader["Annex5Name"] == null) ? "" : reader["Annex5Name"].ToString());
		mailInfo.AnnexRemark = ((reader["AnnexRemark"] == null) ? "" : reader["AnnexRemark"].ToString());
		return mailInfo;
	}

	public MailInfo[] GetMailByUserID(int userID)
	{
		List<MailInfo> list = new List<MailInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_Mail_ByUserID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitMail(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public MailInfo[] GetMailBySenderID(int userID)
	{
		List<MailInfo> list = new List<MailInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_Mail_BySenderID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitMail(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public MailInfo GetMailSingle(int UserID, int mailID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ID", mailID),
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_Mail_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitMail(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool SendMail(MailInfo mail)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[29];
			array[0] = new SqlParameter("@ID", mail.ID);
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[4] = new SqlParameter("@Gold", mail.Gold);
			array[5] = new SqlParameter("@IsExist", true);
			array[6] = new SqlParameter("@Money", mail.Money);
			array[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[10] = new SqlParameter("@SenderID", mail.SenderID);
			array[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[12] = new SqlParameter("@IfDelS", false);
			array[13] = new SqlParameter("@IsDelete", false);
			array[14] = new SqlParameter("@IsDelR", false);
			array[15] = new SqlParameter("@IsRead", false);
			array[16] = new SqlParameter("@SendTime", DateTime.Now);
			array[17] = new SqlParameter("@Type", mail.Type);
			array[18] = new SqlParameter("@Annex1Name", (mail.Annex1Name == null) ? "" : mail.Annex1Name);
			array[19] = new SqlParameter("@Annex2Name", (mail.Annex2Name == null) ? "" : mail.Annex2Name);
			array[20] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
			array[21] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
			array[22] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
			array[23] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
			array[24] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
			array[25] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
			array[26] = new SqlParameter("@ValidDate", mail.ValidDate);
			array[27] = new SqlParameter("@AnnexRemark", (mail.AnnexRemark == null) ? "" : mail.AnnexRemark);
			array[28] = new SqlParameter("GiftToken", mail.GiftToken);
			result = db.RunProcedure("SP_Mail_Send", array);
			mail.ID = (int)array[0].Value;
			using CenterServiceClient centerServiceClient = new CenterServiceClient();
			centerServiceClient.MailNotice(mail.ReceiverID);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool DeleteMail(int UserID, int mailID, out int senderID)
	{
		bool result = false;
		senderID = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ID", mailID),
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@SenderID", SqlDbType.Int),
				null
			};
			array[2].Value = senderID;
			array[2].Direction = ParameterDirection.InputOutput;
			array[3] = new SqlParameter("@Result", SqlDbType.Int);
			array[3].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Mail_Delete", array);
			if ((int)array[3].Value == 0)
			{
				result = true;
				senderID = (int)array[2].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateMail(MailInfo mail, int oldMoney)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[30]
			{
				new SqlParameter("@ID", mail.ID),
				new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1),
				new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2),
				new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content),
				new SqlParameter("@Gold", mail.Gold),
				new SqlParameter("@IsExist", mail.IsExist),
				new SqlParameter("@Money", mail.Money),
				new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver),
				new SqlParameter("@ReceiverID", mail.ReceiverID),
				new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender),
				new SqlParameter("@SenderID", mail.SenderID),
				new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title),
				new SqlParameter("@IfDelS", false),
				new SqlParameter("@IsDelete", false),
				new SqlParameter("@IsDelR", false),
				new SqlParameter("@IsRead", mail.IsRead),
				new SqlParameter("@SendTime", mail.SendTime),
				new SqlParameter("@Type", mail.Type),
				new SqlParameter("@OldMoney", oldMoney),
				new SqlParameter("@ValidDate", mail.ValidDate),
				new SqlParameter("@Annex1Name", mail.Annex1Name),
				new SqlParameter("@Annex2Name", mail.Annex2Name),
				new SqlParameter("@Result", SqlDbType.Int),
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[22].Direction = ParameterDirection.ReturnValue;
			array[23] = new SqlParameter("@Annex3", (mail.Annex3 == null) ? "" : mail.Annex3);
			array[24] = new SqlParameter("@Annex4", (mail.Annex4 == null) ? "" : mail.Annex4);
			array[25] = new SqlParameter("@Annex5", (mail.Annex5 == null) ? "" : mail.Annex5);
			array[26] = new SqlParameter("@Annex3Name", (mail.Annex3Name == null) ? "" : mail.Annex3Name);
			array[27] = new SqlParameter("@Annex4Name", (mail.Annex4Name == null) ? "" : mail.Annex4Name);
			array[28] = new SqlParameter("@Annex5Name", (mail.Annex5Name == null) ? "" : mail.Annex5Name);
			array[29] = new SqlParameter("GiftToken", mail.GiftToken);
			db.RunProcedure("SP_Mail_Update", array);
			int num = (int)array[22].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool CancelPaymentMail(int userid, int mailID, ref int senderID)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@userid", userid),
				new SqlParameter("@mailID", mailID),
				new SqlParameter("@senderID", SqlDbType.Int),
				null
			};
			array[2].Value = senderID;
			array[2].Direction = ParameterDirection.InputOutput;
			array[3] = new SqlParameter("@Result", SqlDbType.Int);
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Mail_PaymentCancel", array);
			int num = (int)array[3].Value;
			flag = num == 0;
			if (flag)
			{
				senderID = (int)array[2].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return flag;
	}

	public bool ScanMail(ref string noticeUserID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
			};
			array[0].Direction = ParameterDirection.Output;
			db.RunProcedure("SP_Mail_Scan", array);
			noticeUserID = array[0].Value.ToString();
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool SendMailAndItem(MailInfo mail, ItemInfo item, ref int returnValue)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[34]
			{
				new SqlParameter("@ItemID", item.ItemID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Place", item.Place),
				new SqlParameter("@AgilityCompose", item.AgilityCompose),
				new SqlParameter("@AttackCompose", item.AttackCompose),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Color", (item.Color == null) ? "" : item.Color),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@DefendCompose", item.DefendCompose),
				new SqlParameter("@IsBinds", item.IsBinds),
				new SqlParameter("@IsExist", item.IsExist),
				new SqlParameter("@IsJudge", item.IsJudge),
				new SqlParameter("@LuckCompose", item.LuckCompose),
				new SqlParameter("@StrengthenLevel", item.StrengthenLevel),
				new SqlParameter("@ValidDate", item.ValidDate),
				new SqlParameter("@BagType", item.BagType),
				new SqlParameter("@ID", mail.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[17].Direction = ParameterDirection.Output;
			array[18] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[19] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[20] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[21] = new SqlParameter("@Gold", mail.Gold);
			array[22] = new SqlParameter("@Money", mail.Money);
			array[23] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[24] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[25] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[26] = new SqlParameter("@SenderID", mail.SenderID);
			array[27] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[28] = new SqlParameter("@IfDelS", false);
			array[29] = new SqlParameter("@IsDelete", false);
			array[30] = new SqlParameter("@IsDelR", false);
			array[31] = new SqlParameter("@IsRead", false);
			array[32] = new SqlParameter("@SendTime", DateTime.Now);
			array[33] = new SqlParameter("@Result", SqlDbType.Int);
			array[33].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Admin_SendUserItem", array);
			returnValue = (int)array[33].Value;
			flag = returnValue == 0;
			if (flag)
			{
				using CenterServiceClient centerServiceClient = new CenterServiceClient();
				centerServiceClient.MailNotice(mail.ReceiverID);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return flag;
	}

	public bool SendMailAndMoney(MailInfo mail, ref int returnValue)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@ID", mail.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@Annex1", (mail.Annex1 == null) ? "" : mail.Annex1);
			array[2] = new SqlParameter("@Annex2", (mail.Annex2 == null) ? "" : mail.Annex2);
			array[3] = new SqlParameter("@Content", (mail.Content == null) ? "" : mail.Content);
			array[4] = new SqlParameter("@Gold", mail.Gold);
			array[5] = new SqlParameter("@IsExist", true);
			array[6] = new SqlParameter("@Money", mail.Money);
			array[7] = new SqlParameter("@Receiver", (mail.Receiver == null) ? "" : mail.Receiver);
			array[8] = new SqlParameter("@ReceiverID", mail.ReceiverID);
			array[9] = new SqlParameter("@Sender", (mail.Sender == null) ? "" : mail.Sender);
			array[10] = new SqlParameter("@SenderID", mail.SenderID);
			array[11] = new SqlParameter("@Title", (mail.Title == null) ? "" : mail.Title);
			array[12] = new SqlParameter("@IfDelS", false);
			array[13] = new SqlParameter("@IsDelete", false);
			array[14] = new SqlParameter("@IsDelR", false);
			array[15] = new SqlParameter("@IsRead", false);
			array[16] = new SqlParameter("@SendTime", DateTime.Now);
			array[17] = new SqlParameter("@Result", SqlDbType.Int);
			array[17].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Admin_SendUserMoney", array);
			returnValue = (int)array[17].Value;
			result = returnValue == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public int SendMailAndItem(string title, string content, int UserID, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Annex1 = "";
		mailInfo.Content = title;
		mailInfo.Gold = gold;
		mailInfo.Money = money;
		mailInfo.Receiver = "";
		mailInfo.ReceiverID = UserID;
		mailInfo.Sender = "Administrators";
		mailInfo.SenderID = 0;
		mailInfo.Title = content;
		ItemInfo itemInfo = new ItemInfo(null);
		itemInfo.AgilityCompose = AgilityCompose;
		itemInfo.AttackCompose = AttackCompose;
		itemInfo.BeginDate = DateTime.Now;
		itemInfo.Color = "";
		itemInfo.DefendCompose = DefendCompose;
		itemInfo.IsDirty = false;
		itemInfo.IsExist = true;
		itemInfo.IsJudge = true;
		itemInfo.LuckCompose = LuckCompose;
		itemInfo.StrengthenLevel = StrengthenLevel;
		itemInfo.TemplateID = templateID;
		itemInfo.ValidDate = validDate;
		itemInfo.Count = count;
		itemInfo.IsBinds = isBinds;
		int returnValue = 1;
		SendMailAndItem(mailInfo, itemInfo, ref returnValue);
		return returnValue;
	}

	public int SendMailAndItemByUserName(string title, string content, string userName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		PlayerInfo userSingleByUserName = GetUserSingleByUserName(userName);
		if (userSingleByUserName != null)
		{
			return SendMailAndItem(title, content, userSingleByUserName.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
		}
		return 2;
	}

	public int SendMailAndItemByNickName(string title, string content, string NickName, int templateID, int count, int validDate, int gold, int money, int StrengthenLevel, int AttackCompose, int DefendCompose, int AgilityCompose, int LuckCompose, bool isBinds)
	{
		PlayerInfo userSingleByNickName = GetUserSingleByNickName(NickName);
		if (userSingleByNickName != null)
		{
			return SendMailAndItem(title, content, userSingleByNickName.ID, templateID, count, validDate, gold, money, StrengthenLevel, AttackCompose, DefendCompose, AgilityCompose, LuckCompose, isBinds);
		}
		return 2;
	}

	public int SendMailAndItem(string title, string content, int userID, int gold, int money, string param)
	{
		bool flag = false;
		int num = 1;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@Title", title),
				new SqlParameter("@Content", content),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Gold", gold),
				new SqlParameter("@Money", money),
				new SqlParameter("@GiftToken", SqlDbType.BigInt),
				new SqlParameter("@Param", param),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			flag = db.RunProcedure("SP_Admin_SendAllItem", array);
			num = (int)array[7].Value;
			if (num == 0)
			{
				using CenterServiceClient centerServiceClient = new CenterServiceClient();
				centerServiceClient.MailNotice(userID);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return num;
	}

	public int SendMailAndItemByUserName(string title, string content, string userName, int gold, int money, string param)
	{
		PlayerInfo userSingleByUserName = GetUserSingleByUserName(userName);
		if (userSingleByUserName != null)
		{
			return SendMailAndItem(title, content, userSingleByUserName.ID, gold, money, param);
		}
		return 2;
	}

	public int SendMailAndItemByNickName(string title, string content, string nickName, int gold, int money, string param)
	{
		PlayerInfo userSingleByNickName = GetUserSingleByNickName(nickName);
		if (userSingleByNickName != null)
		{
			return SendMailAndItem(title, content, userSingleByNickName.ID, gold, money, param);
		}
		return 2;
	}

	public Dictionary<int, int> GetFriendsIDAll(int UserID)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Friends_All", array);
			while (ResultDataReader.Read())
			{
				if (!dictionary.ContainsKey((int)ResultDataReader["FriendID"]))
				{
					dictionary.Add((int)ResultDataReader["FriendID"], (int)ResultDataReader["Relation"]);
				}
				else
				{
					dictionary[(int)ResultDataReader["FriendID"]] = (int)ResultDataReader["Relation"];
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return dictionary;
	}

	public bool AddFriends(FriendInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@AddDate", DateTime.Now),
				new SqlParameter("@FriendID", info.FriendID),
				new SqlParameter("@IsExist", true),
				new SqlParameter("@Remark", (info.Remark == null) ? "" : info.Remark),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Relation", info.Relation)
			};
			result = db.RunProcedure("SP_Users_Friends_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool DeleteFriends(int UserID, int FriendID)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ID", FriendID),
				new SqlParameter("@UserID", UserID)
			};
			result = db.RunProcedure("SP_Users_Friends_Delete", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public FriendInfo[] GetFriendsAll(int UserID)
	{
		List<FriendInfo> list = new List<FriendInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Friends", array);
			while (ResultDataReader.Read())
			{
				FriendInfo friendInfo = new FriendInfo();
				friendInfo.AddDate = (DateTime)ResultDataReader["AddDate"];
				friendInfo.Colors = ((ResultDataReader["Colors"] == null) ? "" : ResultDataReader["Colors"].ToString());
				friendInfo.FriendID = (int)ResultDataReader["FriendID"];
				friendInfo.Grade = (int)ResultDataReader["Grade"];
				friendInfo.Hide = (int)ResultDataReader["Hide"];
				friendInfo.ID = (int)ResultDataReader["ID"];
				friendInfo.IsExist = (bool)ResultDataReader["IsExist"];
				friendInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				friendInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
				friendInfo.Sex = (((bool)ResultDataReader["Sex"]) ? 1 : 0);
				friendInfo.State = (int)ResultDataReader["State"];
				friendInfo.Style = ((ResultDataReader["Style"] == null) ? "" : ResultDataReader["Style"].ToString());
				friendInfo.UserID = (int)ResultDataReader["UserID"];
				friendInfo.ConsortiaName = ((ResultDataReader["ConsortiaName"] == null) ? "" : ResultDataReader["ConsortiaName"].ToString());
				friendInfo.Offer = (int)ResultDataReader["Offer"];
				friendInfo.Win = (int)ResultDataReader["Win"];
				friendInfo.Total = (int)ResultDataReader["Total"];
				friendInfo.Escape = (int)ResultDataReader["Escape"];
				friendInfo.Relation = (int)ResultDataReader["Relation"];
				friendInfo.Repute = (int)ResultDataReader["Repute"];
				friendInfo.UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
				friendInfo.DutyName = ((ResultDataReader["DutyName"] == null) ? "" : ResultDataReader["DutyName"].ToString());
				friendInfo.Nimbus = (int)ResultDataReader["Nimbus"];
				friendInfo.typeVIP = Convert.ToByte(ResultDataReader["typeVIP"]);
				friendInfo.VIPLevel = (int)ResultDataReader["VIPLevel"];
				friendInfo.IsMarried = (bool)ResultDataReader["IsMarried"];
				friendInfo.LastDate = (DateTime)ResultDataReader["AddDate"];
				list.Add(friendInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ArrayList GetFriendsGood(string UserName)
	{
		ArrayList arrayList = new ArrayList();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserName", SqlDbType.NVarChar)
			};
			array[0].Value = UserName;
			db.GetReader(ref ResultDataReader, "SP_Users_Friends_Good", array);
			while (ResultDataReader.Read())
			{
				arrayList.Add((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return arrayList;
	}

	public FriendInfo[] GetFriendsBbs(string condictArray)
	{
		List<FriendInfo> list = new List<FriendInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@SearchUserName", SqlDbType.NVarChar, 4000)
			};
			array[0].Value = condictArray;
			db.GetReader(ref ResultDataReader, "SP_Users_FriendsBbs", array);
			while (ResultDataReader.Read())
			{
				FriendInfo friendInfo = new FriendInfo();
				friendInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				friendInfo.UserID = (int)ResultDataReader["UserID"];
				friendInfo.UserName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
				friendInfo.IsExist = (int)ResultDataReader["UserID"] > 0;
				list.Add(friendInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public QuestDataInfo[] GetUserQuest(int userID)
	{
		List<QuestDataInfo> list = new List<QuestDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_QuestData_All", array);
			while (ResultDataReader.Read())
			{
				QuestDataInfo questDataInfo = new QuestDataInfo();
				questDataInfo.CompletedDate = (DateTime)ResultDataReader["CompletedDate"];
				questDataInfo.IsComplete = (bool)ResultDataReader["IsComplete"];
				questDataInfo.Condition1 = (int)ResultDataReader["Condition1"];
				questDataInfo.Condition2 = (int)ResultDataReader["Condition2"];
				questDataInfo.Condition3 = (int)ResultDataReader["Condition3"];
				questDataInfo.Condition4 = (int)ResultDataReader["Condition4"];
				questDataInfo.Condition5 = (int)ResultDataReader["Condition5"];
				questDataInfo.Condition6 = (int)ResultDataReader["Condition6"];
				questDataInfo.Condition7 = (int)ResultDataReader["Condition7"];
				questDataInfo.Condition8 = (int)ResultDataReader["Condition8"];
				questDataInfo.QuestID = (int)ResultDataReader["QuestID"];
				questDataInfo.UserID = (int)ResultDataReader["UserId"];
				questDataInfo.IsExist = (bool)ResultDataReader["IsExist"];
				questDataInfo.RandDobule = (int)ResultDataReader["RandDobule"];
				questDataInfo.RepeatFinish = (int)ResultDataReader["RepeatFinish"];
				questDataInfo.QuestLevel = (int)ResultDataReader["QuestLevel"];
				list.Add(questDataInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool UpdateDbQuestDataInfo(QuestDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[16]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@QuestID", info.QuestID),
				new SqlParameter("@CompletedDate", info.CompletedDate),
				new SqlParameter("@IsComplete", info.IsComplete),
				new SqlParameter("@Condition1", info.Condition1),
				new SqlParameter("@Condition2", info.Condition2),
				new SqlParameter("@Condition3", info.Condition3),
				new SqlParameter("@Condition4", info.Condition4),
				new SqlParameter("@Condition5", info.Condition5),
				new SqlParameter("@Condition6", info.Condition6),
				new SqlParameter("@Condition7", info.Condition7),
				new SqlParameter("@Condition8", info.Condition8),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@RepeatFinish", info.RepeatFinish),
				new SqlParameter("@RandDobule", info.RandDobule),
				new SqlParameter("@QuestLevel", info.QuestLevel)
			};
			result = db.RunProcedure("SP_QuestData_Add", sqlParameters);
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public BufferInfo[] GetUserBuffer(int userID)
	{
		List<BufferInfo> list = new List<BufferInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = userID;
			db.GetReader(ref ResultDataReader, "SP_User_Buff_All", array);
			while (ResultDataReader.Read())
			{
				BufferInfo bufferInfo = new BufferInfo();
				bufferInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				bufferInfo.Data = ((ResultDataReader["Data"] == null) ? "" : ResultDataReader["Data"].ToString());
				bufferInfo.Type = (int)ResultDataReader["Type"];
				bufferInfo.UserID = (int)ResultDataReader["UserID"];
				bufferInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				bufferInfo.Value = (int)ResultDataReader["Value"];
				bufferInfo.IsExist = (bool)ResultDataReader["IsExist"];
				bufferInfo.ValidCount = (int)ResultDataReader["ValidCount"];
				bufferInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				bufferInfo.IsDirty = false;
				list.Add(bufferInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ConsortiaBufferInfo[] GetUserConsortiaBuffer(int ConsortiaID)
	{
		List<ConsortiaBufferInfo> list = new List<ConsortiaBufferInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ConsortiaID", SqlDbType.Int, 4)
			};
			array[0].Value = ConsortiaID;
			db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_All", array);
			while (ResultDataReader.Read())
			{
				ConsortiaBufferInfo consortiaBufferInfo = new ConsortiaBufferInfo();
				consortiaBufferInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaBufferInfo.BufferID = (int)ResultDataReader["BufferID"];
				consortiaBufferInfo.IsOpen = (bool)ResultDataReader["IsOpen"];
				consortiaBufferInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				consortiaBufferInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				consortiaBufferInfo.Type = (int)ResultDataReader["Type"];
				consortiaBufferInfo.Value = (int)ResultDataReader["Value"];
				list.Add(consortiaBufferInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init SP_User_Consortia_Buff_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ConsortiaBufferInfo GetUserConsortiaBufferSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_User_Consortia_Buff_Single", array);
			if (ResultDataReader.Read())
			{
				ConsortiaBufferInfo consortiaBufferInfo = new ConsortiaBufferInfo();
				consortiaBufferInfo.ConsortiaID = (int)ResultDataReader["ConsortiaID"];
				consortiaBufferInfo.BufferID = (int)ResultDataReader["BufferID"];
				consortiaBufferInfo.IsOpen = (bool)ResultDataReader["IsOpen"];
				consortiaBufferInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				consortiaBufferInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				consortiaBufferInfo.Type = (int)ResultDataReader["Type"];
				consortiaBufferInfo.Value = (int)ResultDataReader["Value"];
				return consortiaBufferInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init SP_User_Consortia_Buff_Single", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool SaveBuffer(BufferInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[9]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@Data", (info.Data == null) ? "" : info.Data),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@ValidCount", info.ValidCount),
				new SqlParameter("@Value", info.Value),
				new SqlParameter("@TemplateID", info.TemplateID)
			};
			result = db.RunProcedure("SP_User_Buff_Add", sqlParameters);
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool SaveConsortiaBuffer(ConsortiaBufferInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[7]
			{
				new SqlParameter("@ConsortiaID", info.ConsortiaID),
				new SqlParameter("@BufferID", info.BufferID),
				new SqlParameter("@IsOpen", info.IsOpen),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@Type ", info.Type),
				new SqlParameter("@Value", info.Value)
			};
			result = db.RunProcedure("SP_User_Consortia_Buff_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public LuckStarRewardRecordInfo[] GetLuckStarTopTenRank(int MinUseNum)
	{
		List<LuckStarRewardRecordInfo> list = new List<LuckStarRewardRecordInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@MinUseNum", MinUseNum)
			};
			db.GetReader(ref ResultDataReader, "SP_LuckStar_Reward_Record_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				LuckStarRewardRecordInfo luckStarRewardRecordInfo = new LuckStarRewardRecordInfo();
				luckStarRewardRecordInfo.PlayerID = (int)ResultDataReader["UserID"];
				luckStarRewardRecordInfo.useStarNum = (int)ResultDataReader["useStarNum"];
				luckStarRewardRecordInfo.isVip = (int)ResultDataReader["isVip"];
				luckStarRewardRecordInfo.nickName = (string)ResultDataReader["nickName"];
				luckStarRewardRecordInfo.rank = num;
				list.Add(luckStarRewardRecordInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init SP_LuckStar_Reward_Record_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool SaveLuckStarRankInfo(LuckStarRewardRecordInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[4]
			{
				new SqlParameter("@UserID", info.PlayerID),
				new SqlParameter("@useStarNum", info.useStarNum),
				new SqlParameter("@nickName", info.nickName),
				new SqlParameter("@isVip", info.isVip)
			};
			result = db.RunProcedure("SP_LuckStar_Rank_Info_Add", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool ResetLuckStarRank()
	{
		bool result = false;
		try
		{
			result = db.RunProcedure("SP_Reset_LuckStar_Rank_Info");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init ResetLuckStar", exception);
			}
		}
		return result;
	}

	public bool AddChargeMoney(string chargeID, string userName, int money, string payWay, decimal needMoney, out int userID, ref int isResult, DateTime date, string IP, string nickName)
	{
		bool result = false;
		userID = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ChargeID", chargeID),
				new SqlParameter("@UserName", userName),
				new SqlParameter("@Money", money),
				new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@PayWay", payWay),
				new SqlParameter("@NeedMoney", needMoney),
				new SqlParameter("@UserID", userID),
				null,
				null,
				null
			};
			array[6].Direction = ParameterDirection.InputOutput;
			array[7] = new SqlParameter("@Result", SqlDbType.Int);
			array[7].Direction = ParameterDirection.ReturnValue;
			array[8] = new SqlParameter("@IP", IP);
			array[9] = new SqlParameter("@NickName", nickName);
			result = db.RunProcedure("SP_Charge_Money_Add", array);
			userID = (int)array[6].Value;
			isResult = (int)array[7].Value;
			result = isResult == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool ChargeToUser(string userName, ref int money, string nickName)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserName", userName),
				new SqlParameter("@money", SqlDbType.Int),
				null
			};
			array[1].Direction = ParameterDirection.Output;
			array[2] = new SqlParameter("@NickName", nickName);
			result = db.RunProcedure("SP_Charge_To_User", array);
			money = (int)array[1].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public ChargeRecordInfo[] GetChargeRecordInfo(DateTime date, int SaveRecordSecond)
	{
		List<ChargeRecordInfo> list = new List<ChargeRecordInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@Second", SaveRecordSecond)
			};
			db.GetReader(ref ResultDataReader, "SP_Charge_Record", sqlParameters);
			while (ResultDataReader.Read())
			{
				ChargeRecordInfo chargeRecordInfo = new ChargeRecordInfo();
				chargeRecordInfo.BoyTotalPay = (int)ResultDataReader["BoyTotalPay"];
				chargeRecordInfo.GirlTotalPay = (int)ResultDataReader["GirlTotalPay"];
				chargeRecordInfo.PayWay = ((ResultDataReader["PayWay"] == null) ? "" : ResultDataReader["PayWay"].ToString());
				chargeRecordInfo.TotalBoy = (int)ResultDataReader["TotalBoy"];
				chargeRecordInfo.TotalGirl = (int)ResultDataReader["TotalGirl"];
				list.Add(chargeRecordInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public AuctionInfo GetAuctionSingle(int auctionID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@AuctionID", auctionID)
			};
			db.GetReader(ref ResultDataReader, "SP_Auction_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return InitAuctionInfo(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool AddAuction(AuctionInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@AuctionID", info.AuctionID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@AuctioneerID", info.AuctioneerID);
			array[2] = new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName);
			array[3] = new SqlParameter("@BeginDate", info.BeginDate);
			array[4] = new SqlParameter("@BuyerID", info.BuyerID);
			array[5] = new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName);
			array[6] = new SqlParameter("@IsExist", info.IsExist);
			array[7] = new SqlParameter("@ItemID", info.ItemID);
			array[8] = new SqlParameter("@Mouthful", info.Mouthful);
			array[9] = new SqlParameter("@PayType", info.PayType);
			array[10] = new SqlParameter("@Price", info.Price);
			array[11] = new SqlParameter("@Rise", info.Rise);
			array[12] = new SqlParameter("@ValidDate", info.ValidDate);
			array[13] = new SqlParameter("@TemplateID", info.TemplateID);
			array[14] = new SqlParameter("Name", info.Name);
			array[15] = new SqlParameter("Category", info.Category);
			array[16] = new SqlParameter("Random", info.Random);
			array[17] = new SqlParameter("goodsCount", info.goodsCount);
			result = db.RunProcedure("SP_Auction_Add", array);
			info.AuctionID = (int)array[0].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateAuction(AuctionInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@AuctionID", info.AuctionID),
				new SqlParameter("@AuctioneerID", info.AuctioneerID),
				new SqlParameter("@AuctioneerName", (info.AuctioneerName == null) ? "" : info.AuctioneerName),
				new SqlParameter("@BeginDate", info.BeginDate),
				new SqlParameter("@BuyerID", info.BuyerID),
				new SqlParameter("@BuyerName", (info.BuyerName == null) ? "" : info.BuyerName),
				new SqlParameter("@IsExist", info.IsExist),
				new SqlParameter("@ItemID", info.ItemID),
				new SqlParameter("@Mouthful", info.Mouthful),
				new SqlParameter("@PayType", info.PayType),
				new SqlParameter("@Price", info.Price),
				new SqlParameter("@Rise", info.Rise),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("Name", info.Name),
				new SqlParameter("Category", info.Category),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[15].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Auction_Update", array);
			int num = (int)array[15].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool DeleteAuction(int auctionID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@AuctionID", auctionID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Auction_Delete", array);
			int num = (int)array[2].Value;
			result = num == 0;
			switch (num)
			{
			case 0:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg1");
				break;
			case 1:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg2");
				break;
			case 2:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg3");
				break;
			default:
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Msg4");
				break;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public AuctionInfo[] GetAuctionPage(int page, string name, int type, int pay, ref int total, int userID, int buyID, int order, bool sort, int size, string AuctionIDs)
	{
		List<AuctionInfo> list = new List<AuctionInfo>();
		try
		{
			string text = " IsExist=1 ";
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " and Name like '%" + name + "%' ";
			}
			if (type != -1)
			{
				switch (type)
				{
				case 0:
				case 1:
				case 2:
				case 3:
				case 4:
				case 5:
				case 6:
				case 7:
				case 8:
				case 9:
				case 10:
				case 11:
				case 12:
				case 13:
				case 14:
				case 15:
				case 16:
				case 17:
				{
					object obj = text;
					text = string.Concat(obj, " and Category =", type, " ");
					break;
				}
				case 21:
					text += " and Category in(1,2,5,8,9) ";
					break;
				case 22:
					text += " and Category in(13,15,6,4,3) ";
					break;
				case 23:
					text += " and Category in(16,11,10) ";
					break;
				case 24:
					text += " and Category in(8,9) ";
					break;
				case 25:
					text += " and Category in (7,17) ";
					break;
				case 26:
					text += " and TemplateId>=311000 and TemplateId<=313999";
					break;
				case 27:
					text += " and TemplateId>=311000 and TemplateId<=311999 ";
					break;
				case 28:
					text += " and TemplateId>=312000 and TemplateId<=312999 ";
					break;
				case 29:
					text += " and TemplateId>=313000 and TempLateId<=313999";
					break;
				case 1100:
					text += " and TemplateID in (11019,11021,11022,11023) ";
					break;
				case 1101:
					text += " and TemplateID='11019' ";
					break;
				case 1102:
					text += " and TemplateID='11021' ";
					break;
				case 1103:
					text += " and TemplateID='11022' ";
					break;
				case 1104:
					text += " and TemplateID='11023' ";
					break;
				case 1105:
					text += " and TemplateID in (11001,11002,11003,11004,11005,11006,11007,11008,11009,11010,11011,11012,11013,11014,11015,11016) ";
					break;
				case 1106:
					text += " and TemplateID in (11001,11002,11003,11004) ";
					break;
				case 1107:
					text += " and TemplateID in (11005,11006,11007,11008) ";
					break;
				case 1108:
					text += " and TemplateID in (11009,11010,11011,11012) ";
					break;
				case 1109:
					text += " and TemplateID in (11013,11014,11015,11016) ";
					break;
				}
			}
			if (pay != -1)
			{
				object obj2 = text;
				text = string.Concat(obj2, " and PayType =", pay, " ");
			}
			if (userID != -1)
			{
				object obj3 = text;
				text = string.Concat(obj3, " and AuctioneerID =", userID, " ");
			}
			if (buyID != -1)
			{
				object obj4 = text;
				text = string.Concat(obj4, " and (BuyerID =", buyID, " or AuctionID in (", AuctionIDs, ")) ");
			}
			string text2 = "Category,Name,Price,dd,AuctioneerID";
			switch (order)
			{
			case 0:
				text2 = "Name";
				break;
			case 2:
				text2 = "dd";
				break;
			case 3:
				text2 = "AuctioneerName";
				break;
			case 4:
				text2 = "Price";
				break;
			case 5:
				text2 = "BuyerName";
				break;
			}
			text2 += (sort ? " desc" : "");
			text2 += ",AuctionID ";
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@QueryStr", "V_Auction_Scan"),
				new SqlParameter("@QueryWhere", text),
				new SqlParameter("@PageSize", size),
				new SqlParameter("@PageCurrent", page),
				new SqlParameter("@FdShow", "*"),
				new SqlParameter("@FdOrder", text2),
				new SqlParameter("@FdKey", "AuctionID"),
				new SqlParameter("@TotalRow", total)
			};
			array[7].Direction = ParameterDirection.Output;
			DataTable dataTable = db.GetDataTable("Auction", "SP_CustomPage", array);
			total = (int)array[7].Value;
			foreach (DataRow row in dataTable.Rows)
			{
				AuctionInfo auctionInfo = new AuctionInfo();
				auctionInfo.AuctioneerID = (int)row["AuctioneerID"];
				auctionInfo.AuctioneerName = row["AuctioneerName"].ToString();
				auctionInfo.AuctionID = (int)row["AuctionID"];
				auctionInfo.BeginDate = (DateTime)row["BeginDate"];
				auctionInfo.BuyerID = (int)row["BuyerID"];
				auctionInfo.BuyerName = row["BuyerName"].ToString();
				auctionInfo.Category = (int)row["Category"];
				auctionInfo.IsExist = (bool)row["IsExist"];
				auctionInfo.ItemID = (int)row["ItemID"];
				auctionInfo.Name = row["Name"].ToString();
				auctionInfo.Mouthful = (int)row["Mouthful"];
				auctionInfo.PayType = (int)row["PayType"];
				auctionInfo.Price = (int)row["Price"];
				auctionInfo.Rise = (int)row["Rise"];
				auctionInfo.ValidDate = (int)row["ValidDate"];
				auctionInfo.goodsCount = (int)row["dd"];
				list.Add(auctionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return list.ToArray();
	}

	public AuctionInfo InitAuctionInfo(SqlDataReader reader)
	{
		AuctionInfo auctionInfo = new AuctionInfo();
		auctionInfo.AuctioneerID = (int)reader["AuctioneerID"];
		auctionInfo.AuctioneerName = ((reader["AuctioneerName"] == null) ? "" : reader["AuctioneerName"].ToString());
		auctionInfo.AuctionID = (int)reader["AuctionID"];
		auctionInfo.BeginDate = (DateTime)reader["BeginDate"];
		auctionInfo.BuyerID = (int)reader["BuyerID"];
		auctionInfo.BuyerName = ((reader["BuyerName"] == null) ? "" : reader["BuyerName"].ToString());
		auctionInfo.IsExist = (bool)reader["IsExist"];
		auctionInfo.ItemID = (int)reader["ItemID"];
		auctionInfo.Mouthful = (int)reader["Mouthful"];
		auctionInfo.PayType = (int)reader["PayType"];
		auctionInfo.Price = (int)reader["Price"];
		auctionInfo.Rise = (int)reader["Rise"];
		auctionInfo.ValidDate = (int)reader["ValidDate"];
		auctionInfo.Name = reader["Name"].ToString();
		auctionInfo.Category = (int)reader["Category"];
		auctionInfo.goodsCount = (int)reader["goodsCount"];
		return auctionInfo;
	}

	public bool ScanAuction(ref string noticeUserID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@NoticeUserID", SqlDbType.NVarChar, 4000)
			};
			array[0].Direction = ParameterDirection.Output;
			db.RunProcedure("SP_Auction_Scan", array);
			noticeUserID = array[0].Value.ToString();
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddMarryInfo(MarryInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@IsPublishEquip", info.IsPublishEquip);
			array[3] = new SqlParameter("@Introduction", info.Introduction);
			array[4] = new SqlParameter("@RegistTime", info.RegistTime);
			result = db.RunProcedure("SP_MarryInfo_Add", array);
			info.ID = (int)array[0].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("AddMarryInfo", exception);
			}
		}
		return result;
	}

	public bool DeleteMarryInfo(int ID, int userID, ref string msg)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_MarryInfo_Delete", array);
			int num = (int)array[2].Value;
			result = num == 0;
			if (num == 0)
			{
				msg = LanguageMgr.GetTranslation("PlayerBussiness.DeleteAuction.Succeed");
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DeleteAuction", exception);
			}
		}
		return result;
	}

	public MarryInfo GetMarryInfoSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_MarryInfo_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				MarryInfo marryInfo = new MarryInfo();
				marryInfo.ID = (int)ResultDataReader["ID"];
				marryInfo.UserID = (int)ResultDataReader["UserID"];
				marryInfo.IsPublishEquip = (bool)ResultDataReader["IsPublishEquip"];
				marryInfo.Introduction = ResultDataReader["Introduction"].ToString();
				marryInfo.RegistTime = (DateTime)ResultDataReader["RegistTime"];
				return marryInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool UpdateMarryInfo(MarryInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@IsPublishEquip", info.IsPublishEquip),
				new SqlParameter("@Introduction", info.Introduction),
				new SqlParameter("@RegistTime", info.RegistTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_MarryInfo_Update", array);
			int num = (int)array[5].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public MarryInfo[] GetMarryInfoPage(int page, string name, bool sex, int size, ref int total)
	{
		List<MarryInfo> list = new List<MarryInfo>();
		try
		{
			string text = "";
			text = ((!sex) ? " IsExist=1 and Sex=0 and UserExist=1" : " IsExist=1 and Sex=1 and UserExist=1");
			if (!string.IsNullOrEmpty(name))
			{
				text = text + " and NickName like '%" + name + "%' ";
			}
			string value = "State desc,IsMarried";
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@QueryStr", "V_Sys_Marry_Info"),
				new SqlParameter("@QueryWhere", text),
				new SqlParameter("@PageSize", size),
				new SqlParameter("@PageCurrent", page),
				new SqlParameter("@FdShow", "*"),
				new SqlParameter("@FdOrder", value),
				new SqlParameter("@FdKey", "ID"),
				new SqlParameter("@TotalRow", total)
			};
			array[7].Direction = ParameterDirection.Output;
			DataTable dataTable = db.GetDataTable("V_Sys_Marry_Info", "SP_CustomPage", array);
			total = (int)array[7].Value;
			foreach (DataRow row in dataTable.Rows)
			{
				MarryInfo marryInfo = new MarryInfo();
				marryInfo.ID = (int)row["ID"];
				marryInfo.UserID = (int)row["UserID"];
				marryInfo.IsPublishEquip = (bool)row["IsPublishEquip"];
				marryInfo.Introduction = row["Introduction"].ToString();
				marryInfo.NickName = row["NickName"].ToString();
				marryInfo.IsConsortia = (bool)row["IsConsortia"];
				marryInfo.ConsortiaID = (int)row["ConsortiaID"];
				marryInfo.Sex = (bool)row["Sex"];
				marryInfo.Win = (int)row["Win"];
				marryInfo.Total = (int)row["Total"];
				marryInfo.Escape = (int)row["Escape"];
				marryInfo.GP = (int)row["GP"];
				marryInfo.Honor = row["Honor"].ToString();
				marryInfo.Style = row["Style"].ToString();
				marryInfo.Colors = row["Colors"].ToString();
				marryInfo.Hide = (int)row["Hide"];
				marryInfo.Grade = (int)row["Grade"];
				marryInfo.State = (int)row["State"];
				marryInfo.Repute = (int)row["Repute"];
				marryInfo.Skin = row["Skin"].ToString();
				marryInfo.Offer = (int)row["Offer"];
				marryInfo.IsMarried = (bool)row["IsMarried"];
				marryInfo.ConsortiaName = row["ConsortiaName"].ToString();
				marryInfo.DutyName = row["DutyName"].ToString();
				marryInfo.Nimbus = (int)row["Nimbus"];
				marryInfo.FightPower = (int)row["FightPower"];
				marryInfo.typeVIP = Convert.ToByte(row["typeVIP"]);
				marryInfo.VIPLevel = (int)row["VIPLevel"];
				list.Add(marryInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return list.ToArray();
	}

	public bool InsertPlayerMarryApply(MarryApplyInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@ApplyUserID", info.ApplyUserID),
				new SqlParameter("@ApplyUserName", info.ApplyUserName),
				new SqlParameter("@ApplyType", info.ApplyType),
				new SqlParameter("@ApplyResult", info.ApplyResult),
				new SqlParameter("@LoveProclamation", info.LoveProclamation),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[6].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Insert_Marry_Apply", array);
			result = (int)array[6].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InsertPlayerMarryApply", exception);
			}
		}
		return result;
	}

	public bool UpdatePlayerMarryApply(int UserID, string loveProclamation, bool isExist)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserID", UserID),
				new SqlParameter("@LoveProclamation", loveProclamation),
				new SqlParameter("@isExist", isExist),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_Marry_Apply", array);
			result = (int)array[3].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerMarryApply", exception);
			}
		}
		return result;
	}

	public MarryApplyInfo[] GetPlayerMarryApply(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		List<MarryApplyInfo> list = new List<MarryApplyInfo>();
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Marry_Apply", sqlParameters);
			while (ResultDataReader.Read())
			{
				MarryApplyInfo marryApplyInfo = new MarryApplyInfo();
				marryApplyInfo.UserID = (int)ResultDataReader["UserID"];
				marryApplyInfo.ApplyUserID = (int)ResultDataReader["ApplyUserID"];
				marryApplyInfo.ApplyUserName = ResultDataReader["ApplyUserName"].ToString();
				marryApplyInfo.ApplyType = (int)ResultDataReader["ApplyType"];
				marryApplyInfo.ApplyResult = (bool)ResultDataReader["ApplyResult"];
				marryApplyInfo.LoveProclamation = ResultDataReader["LoveProclamation"].ToString();
				marryApplyInfo.ID = (int)ResultDataReader["Id"];
				list.Add(marryApplyInfo);
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetPlayerMarryApply", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool InsertMarryRoomInfo(MarryRoomInfo info)
	{
		bool flag = false;
		try
		{
			SqlParameter[] array = new SqlParameter[20]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.InputOutput;
			array[1] = new SqlParameter("@Name", info.Name);
			array[2] = new SqlParameter("@PlayerID", info.PlayerID);
			array[3] = new SqlParameter("@PlayerName", info.PlayerName);
			array[4] = new SqlParameter("@GroomID", info.GroomID);
			array[5] = new SqlParameter("@GroomName", info.GroomName);
			array[6] = new SqlParameter("@BrideID", info.BrideID);
			array[7] = new SqlParameter("@BrideName", info.BrideName);
			array[8] = new SqlParameter("@Pwd", info.Pwd);
			array[9] = new SqlParameter("@AvailTime", info.AvailTime);
			array[10] = new SqlParameter("@MaxCount", info.MaxCount);
			array[11] = new SqlParameter("@GuestInvite", info.GuestInvite);
			array[12] = new SqlParameter("@MapIndex", info.MapIndex);
			array[13] = new SqlParameter("@BeginTime", info.BeginTime);
			array[14] = new SqlParameter("@BreakTime", info.BreakTime);
			array[15] = new SqlParameter("@RoomIntroduction", info.RoomIntroduction);
			array[16] = new SqlParameter("@ServerID", info.ServerID);
			array[17] = new SqlParameter("@IsHymeneal", info.IsHymeneal);
			array[18] = new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[19].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Insert_Marry_Room_Info", array);
			flag = (int)array[19].Value == 0;
			if (flag)
			{
				info.ID = (int)array[0].Value;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InsertMarryRoomInfo", exception);
			}
		}
		return flag;
	}

	public bool UpdateMarryRoomInfo(MarryRoomInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@AvailTime", info.AvailTime),
				new SqlParameter("@BreakTime", info.BreakTime),
				new SqlParameter("@roomIntroduction", info.RoomIntroduction),
				new SqlParameter("@isHymeneal", info.IsHymeneal),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@Pwd", info.Pwd),
				new SqlParameter("@IsGunsaluteUsed", info.IsGunsaluteUsed),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_Marry_Room_Info", array);
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateMarryRoomInfo", exception);
			}
		}
		return result;
	}

	public bool DisposeMarryRoomInfo(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Dispose_Marry_Room_Info", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DisposeMarryRoomInfo", exception);
			}
		}
		return result;
	}

	public MarryRoomInfo[] GetMarryRoomInfo()
	{
		SqlDataReader ResultDataReader = null;
		List<MarryRoomInfo> list = new List<MarryRoomInfo>();
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_Marry_Room_Info");
			while (ResultDataReader.Read())
			{
				MarryRoomInfo marryRoomInfo = new MarryRoomInfo();
				marryRoomInfo.ID = (int)ResultDataReader["ID"];
				marryRoomInfo.Name = ResultDataReader["Name"].ToString();
				marryRoomInfo.PlayerID = (int)ResultDataReader["PlayerID"];
				marryRoomInfo.PlayerName = ResultDataReader["PlayerName"].ToString();
				marryRoomInfo.GroomID = (int)ResultDataReader["GroomID"];
				marryRoomInfo.GroomName = ResultDataReader["GroomName"].ToString();
				marryRoomInfo.BrideID = (int)ResultDataReader["BrideID"];
				marryRoomInfo.BrideName = ResultDataReader["BrideName"].ToString();
				marryRoomInfo.Pwd = ResultDataReader["Pwd"].ToString();
				marryRoomInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				marryRoomInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				marryRoomInfo.GuestInvite = (bool)ResultDataReader["GuestInvite"];
				marryRoomInfo.MapIndex = (int)ResultDataReader["MapIndex"];
				marryRoomInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				marryRoomInfo.BreakTime = (DateTime)ResultDataReader["BreakTime"];
				marryRoomInfo.RoomIntroduction = ResultDataReader["RoomIntroduction"].ToString();
				marryRoomInfo.ServerID = (int)ResultDataReader["ServerID"];
				marryRoomInfo.IsHymeneal = (bool)ResultDataReader["IsHymeneal"];
				marryRoomInfo.IsGunsaluteUsed = (bool)ResultDataReader["IsGunsaluteUsed"];
				list.Add(marryRoomInfo);
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryRoomInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public MarryRoomInfo GetMarryRoomInfoSingle(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Marry_Room_Info_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				MarryRoomInfo marryRoomInfo = new MarryRoomInfo();
				marryRoomInfo.ID = (int)ResultDataReader["ID"];
				marryRoomInfo.Name = ResultDataReader["Name"].ToString();
				marryRoomInfo.PlayerID = (int)ResultDataReader["PlayerID"];
				marryRoomInfo.PlayerName = ResultDataReader["PlayerName"].ToString();
				marryRoomInfo.GroomID = (int)ResultDataReader["GroomID"];
				marryRoomInfo.GroomName = ResultDataReader["GroomName"].ToString();
				marryRoomInfo.BrideID = (int)ResultDataReader["BrideID"];
				marryRoomInfo.BrideName = ResultDataReader["BrideName"].ToString();
				marryRoomInfo.Pwd = ResultDataReader["Pwd"].ToString();
				marryRoomInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				marryRoomInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				marryRoomInfo.GuestInvite = (bool)ResultDataReader["GuestInvite"];
				marryRoomInfo.MapIndex = (int)ResultDataReader["MapIndex"];
				marryRoomInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				marryRoomInfo.BreakTime = (DateTime)ResultDataReader["BreakTime"];
				marryRoomInfo.RoomIntroduction = ResultDataReader["RoomIntroduction"].ToString();
				marryRoomInfo.ServerID = (int)ResultDataReader["ServerID"];
				marryRoomInfo.IsHymeneal = (bool)ResultDataReader["IsHymeneal"];
				marryRoomInfo.IsGunsaluteUsed = (bool)ResultDataReader["IsGunsaluteUsed"];
				return marryRoomInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryRoomInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool UpdateBreakTimeWhereServerStop()
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[0].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_Marry_Room_Info_Sever_Stop", array);
			result = (int)array[0].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateBreakTimeWhereServerStop", exception);
			}
		}
		return result;
	}

	public MarryProp GetMarryProp(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_Select_Marry_Prop", sqlParameters);
			if (ResultDataReader.Read())
			{
				MarryProp marryProp = new MarryProp();
				marryProp.IsMarried = (bool)ResultDataReader["IsMarried"];
				marryProp.SpouseID = (int)ResultDataReader["SpouseID"];
				marryProp.SpouseName = ResultDataReader["SpouseName"].ToString();
				marryProp.IsCreatedMarryRoom = (bool)ResultDataReader["IsCreatedMarryRoom"];
				marryProp.SelfMarryRoomID = (int)ResultDataReader["SelfMarryRoomID"];
				marryProp.IsGotRing = (bool)ResultDataReader["IsGotRing"];
				return marryProp;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetMarryProp", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool SavePlayerMarryNotice(MarryApplyInfo info, int answerId, ref int id)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@ApplyUserID", info.ApplyUserID),
				new SqlParameter("@ApplyUserName", info.ApplyUserName),
				new SqlParameter("@ApplyType", info.ApplyType),
				new SqlParameter("@ApplyResult", info.ApplyResult),
				new SqlParameter("@LoveProclamation", info.LoveProclamation),
				new SqlParameter("@AnswerId", answerId),
				new SqlParameter("@ouototal", SqlDbType.Int),
				null
			};
			array[7].Direction = ParameterDirection.Output;
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Insert_Marry_Notice", array);
			id = (int)array[7].Value;
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SavePlayerMarryNotice", exception);
			}
		}
		return result;
	}

	public bool UpdatePlayerGotRingProp(int groomID, int brideID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@GroomID", groomID),
				new SqlParameter("@BrideID", brideID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_GotRing_Prop", array);
			result = (int)array[2].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdatePlayerGotRingProp", exception);
			}
		}
		return result;
	}

	public HotSpringRoomInfo[] GetHotSpringRoomInfo()
	{
		SqlDataReader ResultDataReader = null;
		List<HotSpringRoomInfo> list = new List<HotSpringRoomInfo>();
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_HotSpring_Room");
			while (ResultDataReader.Read())
			{
				HotSpringRoomInfo hotSpringRoomInfo = new HotSpringRoomInfo();
				hotSpringRoomInfo.RoomID = (int)ResultDataReader["RoomID"];
				hotSpringRoomInfo.RoomName = ((ResultDataReader["RoomName"] == null) ? "" : ResultDataReader["RoomName"].ToString());
				hotSpringRoomInfo.PlayerID = (int)ResultDataReader["PlayerID"];
				hotSpringRoomInfo.PlayerName = ((ResultDataReader["PlayerName"] == null) ? "" : ResultDataReader["PlayerName"].ToString());
				hotSpringRoomInfo.Pwd = ((ResultDataReader["Pwd"].ToString() == null) ? "" : ResultDataReader["Pwd"].ToString());
				hotSpringRoomInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				hotSpringRoomInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				hotSpringRoomInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				hotSpringRoomInfo.BreakTime = (DateTime)ResultDataReader["BreakTime"];
				hotSpringRoomInfo.RoomIntroduction = ((ResultDataReader["RoomIntroduction"] == null) ? "" : ResultDataReader["RoomIntroduction"].ToString());
				hotSpringRoomInfo.RoomType = (int)ResultDataReader["RoomType"];
				hotSpringRoomInfo.ServerID = (int)ResultDataReader["ServerID"];
				hotSpringRoomInfo.RoomNumber = (int)ResultDataReader["RoomNumber"];
				HotSpringRoomInfo item = hotSpringRoomInfo;
				list.Add(item);
			}
			return list.ToArray();
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("HotSpringRoomInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public HotSpringRoomInfo GetHotSpringRoomInfoSingle(int id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@RoomID", id)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_HotSpringRoomInfo_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				HotSpringRoomInfo hotSpringRoomInfo = new HotSpringRoomInfo();
				hotSpringRoomInfo.RoomID = (int)ResultDataReader["RoomID"];
				hotSpringRoomInfo.RoomName = ResultDataReader["RoomName"].ToString();
				hotSpringRoomInfo.PlayerID = (int)ResultDataReader["PlayerID"];
				hotSpringRoomInfo.PlayerName = ResultDataReader["PlayerName"].ToString();
				hotSpringRoomInfo.Pwd = ResultDataReader["Pwd"].ToString();
				hotSpringRoomInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				hotSpringRoomInfo.MaxCount = (int)ResultDataReader["MaxCount"];
				hotSpringRoomInfo.MapIndex = (int)ResultDataReader["MapIndex"];
				hotSpringRoomInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				hotSpringRoomInfo.BreakTime = (DateTime)ResultDataReader["BreakTime"];
				hotSpringRoomInfo.RoomIntroduction = ResultDataReader["RoomIntroduction"].ToString();
				hotSpringRoomInfo.RoomType = (int)ResultDataReader["RoomType"];
				hotSpringRoomInfo.ServerID = (int)ResultDataReader["ServerID"];
				hotSpringRoomInfo.RoomNumber = (int)ResultDataReader["RoomNumber"];
				return hotSpringRoomInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("HotSpringRoomInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool UpdateHotSpringRoomInfo(HotSpringRoomInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[7]
			{
				new SqlParameter("@RoomID", info.RoomID),
				new SqlParameter("@RoomName", info.RoomName),
				new SqlParameter("@Pwd", info.Pwd),
				new SqlParameter("@AvailTime", info.AvailTime.ToString()),
				new SqlParameter("@BreakTime", info.BreakTime.ToString()),
				new SqlParameter("@roomIntroduction", info.RoomIntroduction),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[6].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_HotSpringRoomInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateHotSpringRoomInfo", exception);
			}
		}
		return result;
	}

	public bool UpdateLastVIPPackTime(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@LastVIPPackTime", DateTime.Now.Date),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserLastVIPPackTime", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserLastVIPPackTime", exception);
			}
		}
		return result;
	}

	public bool UpdateVIPInfo(PlayerInfo p)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@ID", p.ID),
				new SqlParameter("@VIPLevel", p.VIPLevel),
				new SqlParameter("@VIPExp", p.VIPExp),
				new SqlParameter("@VIPOnlineDays", SqlDbType.BigInt),
				new SqlParameter("@VIPOfflineDays", SqlDbType.BigInt),
				new SqlParameter("@VIPExpireDay", p.VIPExpireDay.ToString()),
				new SqlParameter("@VIPLastDate", DateTime.Now),
				new SqlParameter("@VIPNextLevelDaysNeeded", SqlDbType.BigInt),
				new SqlParameter("@CanTakeVipReward", p.CanTakeVipReward),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateVIPInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateVIPInfo", exception);
			}
		}
		return result;
	}

	public int VIPRenewal(string nickName, int renewalDays, ref DateTime ExpireDayOut)
	{
		int result = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@NickName", nickName),
				new SqlParameter("@RenewalDays", renewalDays),
				new SqlParameter("@ExpireDayOut", DateTime.Now),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.Output;
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_VIPRenewal_Single", array);
			ExpireDayOut = (DateTime)array[2].Value;
			result = (int)array[3].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_VIPRenewal_Single", exception);
			}
		}
		return result;
	}

	public int VIPLastdate(int ID)
	{
		int result = 0;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_VIPLastdate_Single", array);
			result = (int)array[1].Value;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_VIPLastdate_Single", exception);
			}
		}
		return result;
	}

	public bool Test(string DutyName)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@DutyName", DutyName)
			};
			result = db.RunProcedure("SP_Test1", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool TankAll()
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[0];
			result = db.RunProcedure("SP_Tank_All", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RegisterUser(string UserName, string NickName, string Password, bool Sex, int Money, int GiftToken, int Gold)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[8]
			{
				new SqlParameter("@UserName", UserName),
				new SqlParameter("@Password", Password),
				new SqlParameter("@NickName", NickName),
				new SqlParameter("@Sex", Sex),
				new SqlParameter("@Money", Money),
				new SqlParameter("@GiftToken", GiftToken),
				new SqlParameter("@Gold", Gold),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[7].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Account_Register", array);
			if ((int)array[7].Value == 0)
			{
				result = true;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init Register", exception);
			}
		}
		return result;
	}

	public bool CheckEmailIsValid(string Email)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@Email", Email),
				new SqlParameter("@count", SqlDbType.BigInt)
			};
			array[1].Direction = ParameterDirection.Output;
			db.RunProcedure("CheckEmailIsValid", array);
			if (int.Parse(array[1].Value.ToString()) == 0)
			{
				result = true;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init CheckEmailIsValid", exception);
			}
		}
		return result;
	}

	public bool RegisterUserInfo(UserInfo userinfo)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[6]
			{
				new SqlParameter("@UserID", userinfo.UserID),
				new SqlParameter("@UserEmail", userinfo.UserEmail),
				new SqlParameter("@UserPhone", (userinfo.UserPhone == null) ? string.Empty : userinfo.UserPhone),
				new SqlParameter("@UserOther1", (userinfo.UserOther1 == null) ? string.Empty : userinfo.UserOther1),
				new SqlParameter("@UserOther2", (userinfo.UserOther2 == null) ? string.Empty : userinfo.UserOther2),
				new SqlParameter("@UserOther3", (userinfo.UserOther3 == null) ? string.Empty : userinfo.UserOther3)
			};
			result = db.RunProcedure("SP_User_Info_Add", sqlParameters);
			return result;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UserInfo GetUserInfo(int UserId)
	{
		SqlDataReader ResultDataReader = null;
		UserInfo userInfo = new UserInfo();
		userInfo.UserID = UserId;
		UserInfo userInfo2 = userInfo;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserId)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_User_Info", sqlParameters);
			while (ResultDataReader.Read())
			{
				userInfo2.UserID = int.Parse(ResultDataReader["UserID"].ToString());
				userInfo2.UserEmail = ((ResultDataReader["UserEmail"] == null) ? "" : ResultDataReader["UserEmail"].ToString());
				userInfo2.UserPhone = ((ResultDataReader["UserPhone"] == null) ? "" : ResultDataReader["UserPhone"].ToString());
				userInfo2.UserOther1 = ((ResultDataReader["UserOther1"] == null) ? "" : ResultDataReader["UserOther1"].ToString());
				userInfo2.UserOther2 = ((ResultDataReader["UserOther2"] == null) ? "" : ResultDataReader["UserOther2"].ToString());
				userInfo2.UserOther3 = ((ResultDataReader["UserOther3"] == null) ? "" : ResultDataReader["UserOther3"].ToString());
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return userInfo2;
	}

	public CommunalActiveInfo[] GetAllCommunalActive()
	{
		List<CommunalActiveInfo> list = new List<CommunalActiveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CommunalActive_All");
			while (ResultDataReader.Read())
			{
				CommunalActiveInfo communalActiveInfo = new CommunalActiveInfo();
				communalActiveInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				communalActiveInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				communalActiveInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				communalActiveInfo.LimitGrade = (int)ResultDataReader["LimitGrade"];
				communalActiveInfo.DayMaxScore = (int)ResultDataReader["DayMaxScore"];
				communalActiveInfo.MinScore = (int)ResultDataReader["MinScore"];
				communalActiveInfo.AddPropertyByMoney = (string)ResultDataReader["AddPropertyByMoney"];
				communalActiveInfo.AddPropertyByProp = (string)ResultDataReader["AddPropertyByProp"];
				communalActiveInfo.IsReset = (bool)ResultDataReader["IsReset"];
				communalActiveInfo.IsSendAward = (bool)ResultDataReader["IsSendAward"];
				list.Add(communalActiveInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllCommunalActive", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public CommunalActiveAwardInfo[] GetAllCommunalActiveAward()
	{
		List<CommunalActiveAwardInfo> list = new List<CommunalActiveAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CommunalActiveAward_All");
			while (ResultDataReader.Read())
			{
				CommunalActiveAwardInfo communalActiveAwardInfo = new CommunalActiveAwardInfo();
				communalActiveAwardInfo.ID = (int)ResultDataReader["ID"];
				communalActiveAwardInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				communalActiveAwardInfo.IsArea = (int)ResultDataReader["IsArea"];
				communalActiveAwardInfo.RandID = (int)ResultDataReader["RandID"];
				communalActiveAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				communalActiveAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				communalActiveAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				communalActiveAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				communalActiveAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				communalActiveAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				communalActiveAwardInfo.Count = (int)ResultDataReader["Count"];
				communalActiveAwardInfo.IsBind = (bool)ResultDataReader["IsBind"];
				communalActiveAwardInfo.IsTime = (bool)ResultDataReader["IsTime"];
				communalActiveAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				list.Add(communalActiveAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllCommunalActiveAward", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public CommunalActiveExpInfo[] GetAllCommunalActiveExp()
	{
		List<CommunalActiveExpInfo> list = new List<CommunalActiveExpInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CommunalActiveExp_All");
			while (ResultDataReader.Read())
			{
				CommunalActiveExpInfo communalActiveExpInfo = new CommunalActiveExpInfo();
				communalActiveExpInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				communalActiveExpInfo.Grade = (int)ResultDataReader["Grade"];
				communalActiveExpInfo.Exp = (int)ResultDataReader["Exp"];
				communalActiveExpInfo.AddExpPlus = (int)ResultDataReader["AddExpPlus"];
				list.Add(communalActiveExpInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllCommunalActiveExp", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public LevelInfo[] GetAllLevel()
	{
		List<LevelInfo> list = new List<LevelInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Level_All");
			while (ResultDataReader.Read())
			{
				LevelInfo levelInfo = new LevelInfo();
				levelInfo.Grade = (int)ResultDataReader["Grade"];
				levelInfo.GP = (int)ResultDataReader["GP"];
				levelInfo.Blood = (int)ResultDataReader["Blood"];
				list.Add(levelInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllLevel", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public FairBattleRewardInfo[] GetAllFairBattleReward()
	{
		List<FairBattleRewardInfo> list = new List<FairBattleRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_FairBattleReward_All");
			while (ResultDataReader.Read())
			{
				FairBattleRewardInfo fairBattleRewardInfo = new FairBattleRewardInfo();
				fairBattleRewardInfo.Prestige = (int)ResultDataReader["Prestige"];
				fairBattleRewardInfo.Level = (int)ResultDataReader["Level"];
				fairBattleRewardInfo.Name = (string)ResultDataReader["Name"];
				fairBattleRewardInfo.PrestigeForWin = (int)ResultDataReader["PrestigeForWin"];
				fairBattleRewardInfo.PrestigeForLose = (int)ResultDataReader["PrestigeForLose"];
				fairBattleRewardInfo.Title = (string)ResultDataReader["Title"];
				list.Add(fairBattleRewardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllFairBattleReward", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ExerciseInfo[] GetAllExercise()
	{
		List<ExerciseInfo> list = new List<ExerciseInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Exercise_All");
			while (ResultDataReader.Read())
			{
				ExerciseInfo exerciseInfo = new ExerciseInfo();
				exerciseInfo.Grage = (int)ResultDataReader["Grage"];
				exerciseInfo.GP = (int)ResultDataReader["GP"];
				exerciseInfo.ExerciseA = (int)ResultDataReader["ExerciseA"];
				exerciseInfo.ExerciseAG = (int)ResultDataReader["ExerciseAG"];
				exerciseInfo.ExerciseD = (int)ResultDataReader["ExerciseD"];
				exerciseInfo.ExerciseH = (int)ResultDataReader["ExerciseH"];
				exerciseInfo.ExerciseL = (int)ResultDataReader["ExerciseL"];
				list.Add(exerciseInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllExercise", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public LevelInfo GetUserLevelSingle(int Grade)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Grade", Grade)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Level_By_Grade", sqlParameters);
			if (ResultDataReader.Read())
			{
				LevelInfo levelInfo = new LevelInfo();
				levelInfo.Grade = (int)ResultDataReader["Grade"];
				levelInfo.GP = (int)ResultDataReader["GP"];
				levelInfo.Blood = (int)ResultDataReader["Blood"];
				return levelInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetLevelInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public ExerciseInfo GetExerciseSingle(int Grade)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Grage", Grade)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_Exercise_By_Grade", sqlParameters);
			if (ResultDataReader.Read())
			{
				ExerciseInfo exerciseInfo = new ExerciseInfo();
				exerciseInfo.Grage = (int)ResultDataReader["Grage"];
				exerciseInfo.GP = (int)ResultDataReader["GP"];
				exerciseInfo.ExerciseA = (int)ResultDataReader["ExerciseA"];
				exerciseInfo.ExerciseAG = (int)ResultDataReader["ExerciseAG"];
				exerciseInfo.ExerciseD = (int)ResultDataReader["ExerciseD"];
				exerciseInfo.ExerciseH = (int)ResultDataReader["ExerciseH"];
				exerciseInfo.ExerciseL = (int)ResultDataReader["ExerciseL"];
				return exerciseInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetExerciseInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public TexpInfo GetUserTexpInfoSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", ID)
			};
			db.GetReader(ref ResultDataReader, "SP_Get_UserTexp_By_ID", sqlParameters);
			if (ResultDataReader.Read())
			{
				TexpInfo texpInfo = new TexpInfo();
				texpInfo.UserID = (int)ResultDataReader["UserID"];
				texpInfo.attTexpExp = (int)ResultDataReader["attTexpExp"];
				texpInfo.defTexpExp = (int)ResultDataReader["defTexpExp"];
				texpInfo.hpTexpExp = (int)ResultDataReader["hpTexpExp"];
				texpInfo.lukTexpExp = (int)ResultDataReader["lukTexpExp"];
				texpInfo.spdTexpExp = (int)ResultDataReader["spdTexpExp"];
				texpInfo.texpCount = (int)ResultDataReader["texpCount"];
				texpInfo.texpTaskCount = (int)ResultDataReader["texpTaskCount"];
				texpInfo.texpTaskDate = (DateTime)ResultDataReader["texpTaskDate"];
				return texpInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetTexpInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool UpdateUserTexpInfo(TexpInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@attTexpExp", info.attTexpExp),
				new SqlParameter("@defTexpExp", info.defTexpExp),
				new SqlParameter("@hpTexpExp", info.hpTexpExp),
				new SqlParameter("@lukTexpExp", info.lukTexpExp),
				new SqlParameter("@spdTexpExp", info.spdTexpExp),
				new SqlParameter("@texpCount", info.texpCount),
				new SqlParameter("@texpTaskCount", info.texpTaskCount),
				new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString()),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserTexp_Update", array);
			int num = (int)array[9].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool InsertUserTexpInfo(TexpInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@attTexpExp", info.attTexpExp),
				new SqlParameter("@defTexpExp", info.defTexpExp),
				new SqlParameter("@hpTexpExp", info.hpTexpExp),
				new SqlParameter("@lukTexpExp", info.lukTexpExp),
				new SqlParameter("@spdTexpExp", info.spdTexpExp),
				new SqlParameter("@texpCount", info.texpCount),
				new SqlParameter("@texpTaskCount", info.texpTaskCount),
				new SqlParameter("@texpTaskDate", info.texpTaskDate.ToString()),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserTexp_Add", array);
			result = (int)array[9].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("InsertTexpInfo", exception);
			}
		}
		return result;
	}

	public bool AddeqPet(PetEquipDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@PetID", info.PetID);
			array[3] = new SqlParameter("@eqType", info.eqType);
			array[4] = new SqlParameter("@eqTemplateID", info.eqTemplateID);
			array[5] = new SqlParameter("@startTime", info.startTime);
			array[6] = new SqlParameter("@ValidDate", info.ValidDate);
			array[7] = new SqlParameter("@IsExit", info.IsExit);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_User_Add_eqPet", array);
			int num = (int)array[8].Value;
			result = num == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateqPet(PetEquipDataInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@PetID", info.PetID),
				new SqlParameter("@eqType", info.eqType),
				new SqlParameter("@eqTemplateID", info.eqTemplateID),
				new SqlParameter("@startTime", info.startTime),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_eqPet_Update", array);
			int num = (int)array[8].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public PetEquipDataInfo[] GetEqPetSingles(int UserID)
	{
		List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_eqPet_Single", array);
			while (ResultDataReader.Read())
			{
				PetEquipDataInfo petEquipDataInfo = new PetEquipDataInfo(ItemMgr.FindItemTemplate((int)ResultDataReader["eqTemplateID"]));
				petEquipDataInfo.ID = (int)ResultDataReader["ID"];
				petEquipDataInfo.UserID = (int)ResultDataReader["UserID"];
				petEquipDataInfo.PetID = (int)ResultDataReader["PetID"];
				petEquipDataInfo.eqType = (int)ResultDataReader["eqType"];
				petEquipDataInfo.eqTemplateID = (int)ResultDataReader["eqTemplateID"];
				petEquipDataInfo.startTime = (DateTime)ResultDataReader["startTime"];
				petEquipDataInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				petEquipDataInfo.IsExit = (bool)ResultDataReader["IsExit"];
				list.Add(petEquipDataInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool AddUserPet(UsersPetinfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[30]
			{
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@AttackGrow", info.AttackGrow),
				new SqlParameter("@DefenceGrow", info.DefenceGrow),
				new SqlParameter("@LuckGrow", info.LuckGrow),
				new SqlParameter("@AgilityGrow", info.AgilityGrow),
				new SqlParameter("@BloodGrow", info.BloodGrow),
				new SqlParameter("@DamageGrow", info.DamageGrow),
				new SqlParameter("@GuardGrow", info.GuardGrow),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@GP", info.GP),
				new SqlParameter("@MaxGP", info.MaxGP),
				new SqlParameter("@Hunger", info.Hunger),
				new SqlParameter("@PetHappyStar", info.PetHappyStar),
				new SqlParameter("@MP", info.MP),
				new SqlParameter("@IsEquip", info.IsEquip),
				new SqlParameter("@Skill", info.Skill),
				new SqlParameter("@SkillEquip", info.SkillEquip),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@ID", info.ID),
				null
			};
			array[28].Direction = ParameterDirection.Output;
			array[29] = new SqlParameter("@Result", SqlDbType.Int);
			array[29].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_User_Add_Pet", array);
			int num = (int)array[29].Value;
			result = num == 0;
			info.ID = (int)array[28].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserPet(UsersPetinfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[30]
			{
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@AttackGrow", info.AttackGrow),
				new SqlParameter("@DefenceGrow", info.DefenceGrow),
				new SqlParameter("@LuckGrow", info.LuckGrow),
				new SqlParameter("@AgilityGrow", info.AgilityGrow),
				new SqlParameter("@BloodGrow", info.BloodGrow),
				new SqlParameter("@DamageGrow", info.DamageGrow),
				new SqlParameter("@GuardGrow", info.GuardGrow),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@GP", info.GP),
				new SqlParameter("@MaxGP", info.MaxGP),
				new SqlParameter("@Hunger", info.Hunger),
				new SqlParameter("@PetHappyStar", info.PetHappyStar),
				new SqlParameter("@MP", info.MP),
				new SqlParameter("@IsEquip", info.IsEquip),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@Skill", info.Skill),
				new SqlParameter("@SkillEquip", info.SkillEquip),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[29].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserPet_Update", array);
			int num = (int)array[29].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool RemoveUserPet(UsersPetinfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[28]
			{
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@AttackGrow", info.AttackGrow),
				new SqlParameter("@DefenceGrow", info.DefenceGrow),
				new SqlParameter("@LuckGrow", info.LuckGrow),
				new SqlParameter("@AgilityGrow", info.AgilityGrow),
				new SqlParameter("@BloodGrow", info.BloodGrow),
				new SqlParameter("@DamageGrow", info.DamageGrow),
				new SqlParameter("@GuardGrow", info.GuardGrow),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@GP", info.GP),
				new SqlParameter("@MaxGP", info.MaxGP),
				new SqlParameter("@Hunger", info.Hunger),
				new SqlParameter("@PetHappyStar", info.PetHappyStar),
				new SqlParameter("@MP", info.MP),
				new SqlParameter("@IsEquip", info.IsEquip),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[27].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserPet_Remove", array);
			int num = (int)array[27].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UsersPetinfo GetAdoptPetSingle(int PetID)
	{
		new UsersPetinfo();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = PetID;
			db.GetReader(ref ResultDataReader, "SP_AdoptPet_By_Id", array);
			if (ResultDataReader.Read())
			{
				return InitPet(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public UsersPetinfo[] GetUserAdoptPetSingles(int UserID)
	{
		List<UsersPetinfo> list = new List<UsersPetinfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_User_AdoptPetList", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitPet(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool RemoveUserAdoptPet(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Remove_User_AdoptPet", array);
			int num = (int)array[1].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool ClearAdoptPet(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Clear_AdoptPet", array);
			int num = (int)array[1].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserAdoptPet(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Update_User_AdoptPet", array);
			int num = (int)array[1].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddUserAdoptPet(UsersPetinfo info, bool isUse)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[23]
			{
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Name", (info.Name == null) ? "Error!" : info.Name),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Blood", info.Blood),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@AttackGrow", info.AttackGrow),
				new SqlParameter("@DefenceGrow", info.DefenceGrow),
				new SqlParameter("@LuckGrow", info.LuckGrow),
				new SqlParameter("@AgilityGrow", info.AgilityGrow),
				new SqlParameter("@BloodGrow", info.BloodGrow),
				new SqlParameter("@DamageGrow", info.DamageGrow),
				new SqlParameter("@GuardGrow", info.GuardGrow),
				new SqlParameter("@Skill", info.Skill),
				new SqlParameter("@SkillEquip", info.SkillEquip),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@IsUse", isUse),
				new SqlParameter("@ID", info.ID)
			};
			array[22].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_User_AdoptPet", array);
			info.ID = (int)array[22].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UsersPetinfo[] GetUserPetSingles(int UserID)
	{
		List<UsersPetinfo> list = new List<UsersPetinfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitPet(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public List<UsersPetinfo> GetUserPetIsExitSingles(int UserID)
	{
		List<UsersPetinfo> list = new List<UsersPetinfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_IsExit", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitPet(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public UsersPetinfo GetUserPetSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserPet_By_ID", array);
			if (ResultDataReader.Read())
			{
				return InitPet(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetPetInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public UsersPetinfo InitPet(SqlDataReader reader)
	{
		UsersPetinfo usersPetinfo = new UsersPetinfo();
		usersPetinfo.ID = (int)reader["ID"];
		usersPetinfo.TemplateID = (int)reader["TemplateID"];
		usersPetinfo.Name = reader["Name"].ToString();
		usersPetinfo.UserID = (int)reader["UserID"];
		usersPetinfo.Attack = (int)reader["Attack"];
		usersPetinfo.AttackGrow = (int)reader["AttackGrow"];
		usersPetinfo.Agility = (int)reader["Agility"];
		usersPetinfo.AgilityGrow = (int)reader["AgilityGrow"];
		usersPetinfo.Defence = (int)reader["Defence"];
		usersPetinfo.DefenceGrow = (int)reader["DefenceGrow"];
		usersPetinfo.Luck = (int)reader["Luck"];
		usersPetinfo.LuckGrow = (int)reader["LuckGrow"];
		usersPetinfo.Blood = (int)reader["Blood"];
		usersPetinfo.BloodGrow = (int)reader["BloodGrow"];
		usersPetinfo.Damage = (int)reader["Damage"];
		usersPetinfo.DamageGrow = (int)reader["DamageGrow"];
		usersPetinfo.Guard = (int)reader["Guard"];
		usersPetinfo.GuardGrow = (int)reader["GuardGrow"];
		usersPetinfo.Level = (int)reader["Level"];
		usersPetinfo.GP = (int)reader["GP"];
		usersPetinfo.MaxGP = (int)reader["MaxGP"];
		usersPetinfo.Hunger = (int)reader["Hunger"];
		usersPetinfo.MP = (int)reader["MP"];
		usersPetinfo.Place = (int)reader["Place"];
		usersPetinfo.IsEquip = (bool)reader["IsEquip"];
		usersPetinfo.IsExit = (bool)reader["IsExit"];
		usersPetinfo.Skill = reader["Skill"].ToString();
		usersPetinfo.SkillEquip = reader["SkillEquip"].ToString();
		return usersPetinfo;
	}

	public PetConfig[] GetAllPetConfig()
	{
		List<PetConfig> list = new List<PetConfig>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetConfig_All");
			while (ResultDataReader.Read())
			{
				PetConfig petConfig = new PetConfig();
				petConfig.ID = (int)ResultDataReader["ID"];
				petConfig.Name = ResultDataReader["Name"].ToString();
				petConfig.Value = ResultDataReader["Value"].ToString();
				list.Add(petConfig);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetConfig", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PetLevel[] GetAllPetLevel()
	{
		List<PetLevel> list = new List<PetLevel>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetLevel_All");
			while (ResultDataReader.Read())
			{
				PetLevel petLevel = new PetLevel();
				petLevel.Level = (int)ResultDataReader["Level"];
				petLevel.GP = (int)ResultDataReader["GP"];
				list.Add(petLevel);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetLevel", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PetTemplateInfo[] GetAllPetTemplateInfo()
	{
		List<PetTemplateInfo> list = new List<PetTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetTemplateInfo_All");
			while (ResultDataReader.Read())
			{
				PetTemplateInfo petTemplateInfo = new PetTemplateInfo();
				petTemplateInfo.ID = (int)ResultDataReader["ID"];
				petTemplateInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				petTemplateInfo.Name = ResultDataReader["Name"].ToString();
				petTemplateInfo.KindID = (int)ResultDataReader["KindID"];
				petTemplateInfo.Description = ResultDataReader["Description"].ToString();
				petTemplateInfo.Pic = ResultDataReader["Pic"].ToString();
				petTemplateInfo.RareLevel = (int)ResultDataReader["RareLevel"];
				petTemplateInfo.MP = (int)ResultDataReader["MP"];
				petTemplateInfo.StarLevel = (int)ResultDataReader["StarLevel"];
				petTemplateInfo.GameAssetUrl = ResultDataReader["GameAssetUrl"].ToString();
				petTemplateInfo.EvolutionID = (int)ResultDataReader["EvolutionID"];
				list.Add(petTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetTemplateInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PetSkillTemplateInfo[] GetAllPetSkillTemplateInfo()
	{
		List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkillTemplateInfo_All");
			while (ResultDataReader.Read())
			{
				PetSkillTemplateInfo petSkillTemplateInfo = new PetSkillTemplateInfo();
				petSkillTemplateInfo.ID = (int)ResultDataReader["ID"];
				petSkillTemplateInfo.PetTemplateID = (int)ResultDataReader["PetTemplateID"];
				petSkillTemplateInfo.KindID = (int)ResultDataReader["KindID"];
				petSkillTemplateInfo.GetTypes = (int)ResultDataReader["GetType"];
				petSkillTemplateInfo.SkillID = (int)ResultDataReader["SkillID"];
				petSkillTemplateInfo.SkillBookID = (int)ResultDataReader["SkillBookID"];
				petSkillTemplateInfo.MinLevel = (int)ResultDataReader["MinLevel"];
				petSkillTemplateInfo.DeleteSkillIDs = ResultDataReader["DeleteSkillIDs"].ToString();
				list.Add(petSkillTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetSkillTemplateInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PetSkillInfo[] GetAllPetSkillInfo()
	{
		List<PetSkillInfo> list = new List<PetSkillInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkillInfo_All");
			while (ResultDataReader.Read())
			{
				PetSkillInfo petSkillInfo = new PetSkillInfo();
				petSkillInfo.ID = (int)ResultDataReader["ID"];
				petSkillInfo.Name = ResultDataReader["Name"].ToString();
				petSkillInfo.ElementIDs = ResultDataReader["ElementIDs"].ToString();
				petSkillInfo.Description = ResultDataReader["Description"].ToString();
				petSkillInfo.BallType = (int)ResultDataReader["BallType"];
				petSkillInfo.NewBallID = (int)ResultDataReader["NewBallID"];
				petSkillInfo.CostMP = (int)ResultDataReader["CostMP"];
				petSkillInfo.Pic = (int)ResultDataReader["Pic"];
				petSkillInfo.Action = ResultDataReader["Action"].ToString();
				petSkillInfo.EffectPic = ResultDataReader["EffectPic"].ToString();
				petSkillInfo.Delay = (int)ResultDataReader["Delay"];
				petSkillInfo.ColdDown = (int)ResultDataReader["ColdDown"];
				petSkillInfo.GameType = (int)ResultDataReader["GameType"];
				petSkillInfo.Probability = (int)ResultDataReader["Probability"];
				petSkillInfo.Damage = (int)ResultDataReader["Damage"];
				petSkillInfo.DamageCrit = (int)ResultDataReader["DamageCrit"];
				list.Add(petSkillInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetSkillInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PetSkillElementInfo[] GetAllPetSkillElementInfo()
	{
		List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetSkillElementInfo_All");
			while (ResultDataReader.Read())
			{
				PetSkillElementInfo petSkillElementInfo = new PetSkillElementInfo();
				petSkillElementInfo.ID = (int)ResultDataReader["ID"];
				petSkillElementInfo.Name = ResultDataReader["Name"].ToString();
				petSkillElementInfo.EffectPic = ResultDataReader["EffectPic"].ToString();
				petSkillElementInfo.Description = ResultDataReader["Description"].ToString();
				petSkillElementInfo.Pic = (int)ResultDataReader["Pic"];
				list.Add(petSkillElementInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetSkillElementInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public PetExpItemPriceInfo[] GetAllPetExpItemPriceInfoInfo()
	{
		List<PetExpItemPriceInfo> list = new List<PetExpItemPriceInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PetExpItemPriceInfo_All");
			while (ResultDataReader.Read())
			{
				PetExpItemPriceInfo petExpItemPriceInfo = new PetExpItemPriceInfo();
				petExpItemPriceInfo.ID = (int)ResultDataReader["ID"];
				petExpItemPriceInfo.Count = (int)ResultDataReader["Count"];
				petExpItemPriceInfo.Money = (int)ResultDataReader["Money"];
				petExpItemPriceInfo.ItemCount = (int)ResultDataReader["ItemCount"];
				list.Add(petExpItemPriceInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPetTemplateInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool AddCards(UsersCardInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[16]
			{
				new SqlParameter("@CardID", item.CardID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@CardType", item.CardType);
			array[2] = new SqlParameter("@UserID", item.UserID);
			array[3] = new SqlParameter("@Place", item.Place);
			array[4] = new SqlParameter("@TemplateID", item.TemplateID);
			array[5] = new SqlParameter("@isFirstGet", false);
			array[6] = new SqlParameter("@Attack", item.Attack);
			array[7] = new SqlParameter("@Defence", item.Defence);
			array[8] = new SqlParameter("@Luck", item.Luck);
			array[9] = new SqlParameter("@Agility", item.Agility);
			array[10] = new SqlParameter("@Damage", item.Damage);
			array[11] = new SqlParameter("@Guard", item.Guard);
			array[12] = new SqlParameter("@IsExit", item.IsExit);
			array[13] = new SqlParameter("@Level", item.Level);
			array[14] = new SqlParameter("@CardGP", item.CardGP);
			array[15] = new SqlParameter("@Type", item.Type);
			result = db.RunProcedure("SP_Users_Cards_Add", array);
			item.CardID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateCards(UsersCardInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@CardType", info.CardType),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Place", info.Place),
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@isFirstGet", info.isFirstGet),
				new SqlParameter("@Attack", info.Attack),
				new SqlParameter("@Defence", info.Defence),
				new SqlParameter("@Luck", info.Luck),
				new SqlParameter("@Agility", info.Agility),
				new SqlParameter("@Damage", info.Damage),
				new SqlParameter("@Guard", info.Guard),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@Level", info.Level),
				new SqlParameter("@CardGP", info.CardGP),
				new SqlParameter("@Type", info.Type),
				new SqlParameter("@CardID", info.CardID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[16].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserCardProp_Update", array);
			int num = (int)array[16].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public List<UsersCardInfo> GetUserCardEuqip(int UserID)
	{
		List<UsersCardInfo> list = new List<UsersCardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Items_Card_Equip", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitCard(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public UsersCardInfo GetUserCardByPlace(int Place)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@Place", SqlDbType.Int, 4)
			};
			array[0].Value = Place;
			db.GetReader(ref ResultDataReader, "SP_Get_UserCard_By_Place", array);
			if (ResultDataReader.Read())
			{
				return InitCard(ResultDataReader);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public UsersCardInfo[] GetUserCardSingles(int UserID)
	{
		List<UsersCardInfo> list = new List<UsersCardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Get_UserCard_By_ID", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitCard(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public UsersCardInfo InitCard(SqlDataReader reader)
	{
		UsersCardInfo usersCardInfo = new UsersCardInfo();
		usersCardInfo.UserID = (int)reader["UserID"];
		usersCardInfo.TemplateID = (int)reader["TemplateID"];
		usersCardInfo.CardID = (int)reader["CardID"];
		usersCardInfo.CardType = (int)reader["CardType"];
		usersCardInfo.Attack = (int)reader["Attack"];
		usersCardInfo.Agility = (int)reader["Agility"];
		usersCardInfo.Defence = (int)reader["Defence"];
		usersCardInfo.Luck = (int)reader["Luck"];
		usersCardInfo.Damage = (int)reader["Damage"];
		usersCardInfo.Guard = (int)reader["Guard"];
		usersCardInfo.Level = (int)reader["Level"];
		usersCardInfo.Place = (int)reader["Place"];
		usersCardInfo.isFirstGet = (bool)reader["isFirstGet"];
		usersCardInfo.Type = (int)reader["Type"];
		usersCardInfo.CardGP = (int)reader["CardGP"];
		return usersCardInfo;
	}

	public CardGrooveUpdateInfo[] GetAllCardGrooveUpdate()
	{
		List<CardGrooveUpdateInfo> list = new List<CardGrooveUpdateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CardGrooveUpdate_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardGrooveUpdate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllCardGrooveUpdate", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public CardGrooveUpdateInfo InitCardGrooveUpdate(SqlDataReader reader)
	{
		CardGrooveUpdateInfo cardGrooveUpdateInfo = new CardGrooveUpdateInfo();
		cardGrooveUpdateInfo.ID = (int)reader["ID"];
		cardGrooveUpdateInfo.Attack = (int)reader["Attack"];
		cardGrooveUpdateInfo.Defend = (int)reader["Defend"];
		cardGrooveUpdateInfo.Agility = (int)reader["Agility"];
		cardGrooveUpdateInfo.Lucky = (int)reader["Lucky"];
		cardGrooveUpdateInfo.Damage = (int)reader["Damage"];
		cardGrooveUpdateInfo.Guard = (int)reader["Guard"];
		cardGrooveUpdateInfo.Level = (int)reader["Level"];
		cardGrooveUpdateInfo.Type = (int)reader["Type"];
		cardGrooveUpdateInfo.Exp = (int)reader["Exp"];
		return cardGrooveUpdateInfo;
	}

	public CardTemplateInfo[] GetAllCardTemplate()
	{
		List<CardTemplateInfo> list = new List<CardTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_CardTemplate_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitCardTemplate(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllCardTemplateInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public CardTemplateInfo InitCardTemplate(SqlDataReader reader)
	{
		CardTemplateInfo cardTemplateInfo = new CardTemplateInfo();
		cardTemplateInfo.ID = (int)reader["ID"];
		cardTemplateInfo.CardID = (int)reader["CardID"];
		cardTemplateInfo.CardType = (int)reader["CardType"];
		cardTemplateInfo.probability = (int)reader["probability"];
		cardTemplateInfo.AttackRate = (int)reader["AttackRate"];
		cardTemplateInfo.AddAttack = (int)reader["AddAttack"];
		cardTemplateInfo.DefendRate = (int)reader["DefendRate"];
		cardTemplateInfo.AddDefend = (int)reader["AddDefend"];
		cardTemplateInfo.AgilityRate = (int)reader["AgilityRate"];
		cardTemplateInfo.AddAgility = (int)reader["AddAgility"];
		cardTemplateInfo.LuckyRate = (int)reader["LuckyRate"];
		cardTemplateInfo.AddLucky = (int)reader["AddLucky"];
		cardTemplateInfo.DamageRate = (int)reader["DamageRate"];
		cardTemplateInfo.AddDamage = (int)reader["AddDamage"];
		cardTemplateInfo.GuardRate = (int)reader["GuardRate"];
		cardTemplateInfo.AddGuard = (int)reader["AddGuard"];
		return cardTemplateInfo;
	}

	public bool AddFarm(UserFarmInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[15]
			{
				new SqlParameter("@FarmID", item.FarmID),
				new SqlParameter("@PayFieldMoney", item.PayFieldMoney),
				new SqlParameter("@PayAutoMoney", item.PayAutoMoney),
				new SqlParameter("@AutoPayTime", item.AutoPayTime.ToString()),
				new SqlParameter("@AutoValidDate", item.AutoValidDate),
				new SqlParameter("@VipLimitLevel", item.VipLimitLevel),
				new SqlParameter("@FarmerName", item.FarmerName),
				new SqlParameter("@GainFieldId", item.GainFieldId),
				new SqlParameter("@MatureId", item.MatureId),
				new SqlParameter("@KillCropId", item.KillCropId),
				new SqlParameter("@isAutoId", item.isAutoId),
				new SqlParameter("@isFarmHelper", item.isFarmHelper),
				new SqlParameter("@ID", item.ID),
				null,
				null
			};
			array[12].Direction = ParameterDirection.Output;
			array[13] = new SqlParameter("@buyExpRemainNum", item.buyExpRemainNum);
			array[14] = new SqlParameter("@isArrange", item.isArrange);
			result = db.RunProcedure("SP_Users_Farm_Add", array);
			item.ID = (int)array[12].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateFarm(UserFarmInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[15]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@FarmID", info.FarmID),
				new SqlParameter("@PayFieldMoney", info.PayFieldMoney),
				new SqlParameter("@PayAutoMoney", info.PayAutoMoney),
				new SqlParameter("@AutoPayTime", info.AutoPayTime.ToString()),
				new SqlParameter("@AutoValidDate", info.AutoValidDate),
				new SqlParameter("@VipLimitLevel", info.VipLimitLevel),
				new SqlParameter("@FarmerName", info.FarmerName),
				new SqlParameter("@GainFieldId", info.GainFieldId),
				new SqlParameter("@MatureId", info.MatureId),
				new SqlParameter("@KillCropId", info.KillCropId),
				new SqlParameter("@isAutoId", info.isAutoId),
				new SqlParameter("@isFarmHelper", info.isFarmHelper),
				new SqlParameter("@buyExpRemainNum", info.buyExpRemainNum),
				new SqlParameter("@isArrange", info.isArrange)
			};
			result = db.RunProcedure("SP_Users_Farm_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool AddFields(UserFieldInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[17]
			{
				new SqlParameter("@FarmID", item.FarmID),
				new SqlParameter("@FieldID", item.FieldID),
				new SqlParameter("@SeedID", item.SeedID),
				new SqlParameter("@PlantTime", item.PlantTime.ToString()),
				new SqlParameter("@AccelerateTime", item.AccelerateTime),
				new SqlParameter("@FieldValidDate", item.FieldValidDate),
				new SqlParameter("@PayTime", item.PayTime.ToString()),
				new SqlParameter("@GainCount", item.GainCount),
				new SqlParameter("@AutoSeedID", item.AutoSeedID),
				new SqlParameter("@AutoFertilizerID", item.AutoFertilizerID),
				new SqlParameter("@AutoSeedIDCount", item.AutoSeedIDCount),
				new SqlParameter("@AutoFertilizerCount", item.AutoFertilizerCount),
				new SqlParameter("@isAutomatic", item.isAutomatic),
				new SqlParameter("@AutomaticTime", item.AutomaticTime.ToString()),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@payFieldTime", item.payFieldTime),
				new SqlParameter("@ID", item.ID)
			};
			array[16].Direction = ParameterDirection.Output;
			result = db.RunProcedure("SP_Users_Fields_Add", array);
			item.ID = (int)array[16].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateFields(UserFieldInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[17]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@FarmID", info.FarmID),
				new SqlParameter("@FieldID", info.FieldID),
				new SqlParameter("@SeedID", info.SeedID),
				new SqlParameter("@PlantTime", info.PlantTime.ToString()),
				new SqlParameter("@AccelerateTime", info.AccelerateTime),
				new SqlParameter("@FieldValidDate", info.FieldValidDate),
				new SqlParameter("@PayTime", info.PayTime.ToString()),
				new SqlParameter("@GainCount", info.GainCount),
				new SqlParameter("@AutoSeedID", info.AutoSeedID),
				new SqlParameter("@AutoFertilizerID", info.AutoFertilizerID),
				new SqlParameter("@AutoSeedIDCount", info.AutoSeedIDCount),
				new SqlParameter("@AutoFertilizerCount", info.AutoFertilizerCount),
				new SqlParameter("@isAutomatic", info.isAutomatic),
				new SqlParameter("@AutomaticTime", info.AutomaticTime.ToString()),
				new SqlParameter("@IsExit", info.IsExit),
				new SqlParameter("@payFieldTime", info.payFieldTime)
			};
			result = db.RunProcedure("SP_Users_Fields_Update", sqlParameters);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public UserFarmInfo GetSingleFarm(int Id)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = Id;
			db.GetReader(ref ResultDataReader, "SP_Get_SingleFarm", array);
			if (ResultDataReader.Read())
			{
				UserFarmInfo userFarmInfo = new UserFarmInfo();
				userFarmInfo.ID = (int)ResultDataReader["ID"];
				userFarmInfo.FarmID = (int)ResultDataReader["FarmID"];
				userFarmInfo.PayFieldMoney = (string)ResultDataReader["PayFieldMoney"];
				userFarmInfo.PayAutoMoney = (string)ResultDataReader["PayAutoMoney"];
				userFarmInfo.AutoPayTime = (DateTime)ResultDataReader["AutoPayTime"];
				userFarmInfo.AutoValidDate = (int)ResultDataReader["AutoValidDate"];
				userFarmInfo.VipLimitLevel = (int)ResultDataReader["VipLimitLevel"];
				userFarmInfo.FarmerName = (string)ResultDataReader["FarmerName"];
				userFarmInfo.GainFieldId = (int)ResultDataReader["GainFieldId"];
				userFarmInfo.MatureId = (int)ResultDataReader["MatureId"];
				userFarmInfo.KillCropId = (int)ResultDataReader["KillCropId"];
				userFarmInfo.isAutoId = (int)ResultDataReader["isAutoId"];
				userFarmInfo.isFarmHelper = (bool)ResultDataReader["isFarmHelper"];
				userFarmInfo.buyExpRemainNum = (int)ResultDataReader["buyExpRemainNum"];
				userFarmInfo.isArrange = (bool)ResultDataReader["isArrange"];
				return userFarmInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetSingleFarm", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public UserFieldInfo[] GetSingleFields(int ID)
	{
		List<UserFieldInfo> list = new List<UserFieldInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Get_SingleFields", array);
			while (ResultDataReader.Read())
			{
				UserFieldInfo userFieldInfo = new UserFieldInfo();
				userFieldInfo.ID = (int)ResultDataReader["ID"];
				userFieldInfo.FarmID = (int)ResultDataReader["FarmID"];
				userFieldInfo.FieldID = (int)ResultDataReader["FieldID"];
				userFieldInfo.SeedID = (int)ResultDataReader["SeedID"];
				userFieldInfo.PlantTime = (DateTime)ResultDataReader["PlantTime"];
				userFieldInfo.AccelerateTime = (int)ResultDataReader["AccelerateTime"];
				userFieldInfo.FieldValidDate = (int)ResultDataReader["FieldValidDate"];
				userFieldInfo.PayTime = (DateTime)ResultDataReader["PayTime"];
				userFieldInfo.GainCount = (int)ResultDataReader["GainCount"];
				userFieldInfo.AutoSeedID = (int)ResultDataReader["AutoSeedID"];
				userFieldInfo.AutoFertilizerID = (int)ResultDataReader["AutoFertilizerID"];
				userFieldInfo.AutoSeedIDCount = (int)ResultDataReader["AutoSeedIDCount"];
				userFieldInfo.AutoFertilizerCount = (int)ResultDataReader["AutoFertilizerCount"];
				userFieldInfo.isAutomatic = (bool)ResultDataReader["isAutomatic"];
				userFieldInfo.AutomaticTime = (DateTime)ResultDataReader["AutomaticTime"];
				userFieldInfo.IsExit = (bool)ResultDataReader["IsExit"];
				userFieldInfo.payFieldTime = (int)ResultDataReader["payFieldTime"];
				list.Add(userFieldInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleFields", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool DeleteAllFields(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_RemoveAllFields", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_RemoveAllFields", exception);
			}
		}
		return result;
	}

	public List<UserGemStone> GetSingleGemStones(int ID)
	{
		List<UserGemStone> list = new List<UserGemStone>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleGemStone", array);
			while (ResultDataReader.Read())
			{
				UserGemStone userGemStone = new UserGemStone();
				userGemStone.ID = (int)ResultDataReader["ID"];
				userGemStone.UserID = (int)ResultDataReader["UserID"];
				userGemStone.FigSpiritId = (int)ResultDataReader["FigSpiritId"];
				userGemStone.FigSpiritIdValue = (string)ResultDataReader["FigSpiritIdValue"];
				userGemStone.EquipPlace = (int)ResultDataReader["EquipPlace"];
				list.Add(userGemStone);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserGemStones", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public bool AddUserGemStone(UserGemStone item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@FigSpiritId", item.FigSpiritId);
			array[3] = new SqlParameter("@FigSpiritIdValue", item.FigSpiritIdValue);
			array[4] = new SqlParameter("@EquipPlace", item.EquipPlace);
			array[5] = new SqlParameter("@Result", SqlDbType.Int);
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_GemStones_Add", array);
			int num = (int)array[5].Value;
			result = num == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateGemStoneInfo(UserGemStone g)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@ID", g.ID),
				new SqlParameter("@UserID", g.UserID),
				new SqlParameter("@FigSpiritId", g.FigSpiritId),
				new SqlParameter("@FigSpiritIdValue", g.FigSpiritIdValue),
				new SqlParameter("@EquipPlace", g.EquipPlace),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateGemStoneInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateGemStoneInfo", exception);
			}
		}
		return result;
	}

	public UserLabyrinthInfo GetSingleLabyrinth(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleLabyrinth", array);
			if (ResultDataReader.Read())
			{
				UserLabyrinthInfo userLabyrinthInfo = new UserLabyrinthInfo();
				userLabyrinthInfo.UserID = (int)ResultDataReader["UserID"];
				userLabyrinthInfo.myProgress = (int)ResultDataReader["myProgress"];
				userLabyrinthInfo.myRanking = (int)ResultDataReader["myRanking"];
				userLabyrinthInfo.completeChallenge = (bool)ResultDataReader["completeChallenge"];
				userLabyrinthInfo.isDoubleAward = (bool)ResultDataReader["isDoubleAward"];
				userLabyrinthInfo.currentFloor = (int)ResultDataReader["currentFloor"];
				userLabyrinthInfo.accumulateExp = (int)ResultDataReader["accumulateExp"];
				userLabyrinthInfo.remainTime = (int)ResultDataReader["remainTime"];
				userLabyrinthInfo.currentRemainTime = (int)ResultDataReader["currentRemainTime"];
				userLabyrinthInfo.cleanOutAllTime = (int)ResultDataReader["cleanOutAllTime"];
				userLabyrinthInfo.cleanOutGold = (int)ResultDataReader["cleanOutGold"];
				userLabyrinthInfo.tryAgainComplete = (bool)ResultDataReader["tryAgainComplete"];
				userLabyrinthInfo.isInGame = (bool)ResultDataReader["isInGame"];
				userLabyrinthInfo.isCleanOut = (bool)ResultDataReader["isCleanOut"];
				userLabyrinthInfo.serverMultiplyingPower = (bool)ResultDataReader["serverMultiplyingPower"];
				userLabyrinthInfo.LastDate = (DateTime)ResultDataReader["LastDate"];
				userLabyrinthInfo.ProcessAward = (string)ResultDataReader["ProcessAward"];
				return userLabyrinthInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserLabyrinth", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool AddUserLabyrinth(UserLabyrinthInfo laby)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@UserID", laby.UserID),
				new SqlParameter("@myProgress", laby.myProgress),
				new SqlParameter("@myRanking", laby.myRanking),
				new SqlParameter("@completeChallenge", laby.completeChallenge),
				new SqlParameter("@isDoubleAward", laby.isDoubleAward),
				new SqlParameter("@currentFloor", laby.currentFloor),
				new SqlParameter("@accumulateExp", laby.accumulateExp),
				new SqlParameter("@remainTime", laby.remainTime),
				new SqlParameter("@currentRemainTime", laby.currentRemainTime),
				new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime),
				new SqlParameter("@cleanOutGold", laby.cleanOutGold),
				new SqlParameter("@tryAgainComplete", laby.tryAgainComplete),
				new SqlParameter("@isInGame", laby.isInGame),
				new SqlParameter("@isCleanOut", laby.isCleanOut),
				new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower),
				new SqlParameter("@LastDate", laby.LastDate),
				new SqlParameter("@ProcessAward", laby.ProcessAward),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[17].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_Labyrinth_Add", array);
			int num = (int)array[17].Value;
			result = num == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateLabyrinthInfo(UserLabyrinthInfo laby)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[18]
			{
				new SqlParameter("@UserID", laby.UserID),
				new SqlParameter("@myProgress", laby.myProgress),
				new SqlParameter("@myRanking", laby.myRanking),
				new SqlParameter("@completeChallenge", laby.completeChallenge),
				new SqlParameter("@isDoubleAward", laby.isDoubleAward),
				new SqlParameter("@currentFloor", laby.currentFloor),
				new SqlParameter("@accumulateExp", laby.accumulateExp),
				new SqlParameter("@remainTime", laby.remainTime),
				new SqlParameter("@currentRemainTime", laby.currentRemainTime),
				new SqlParameter("@cleanOutAllTime", laby.cleanOutAllTime),
				new SqlParameter("@cleanOutGold", laby.cleanOutGold),
				new SqlParameter("@tryAgainComplete", laby.tryAgainComplete),
				new SqlParameter("@isInGame", laby.isInGame),
				new SqlParameter("@isCleanOut", laby.isCleanOut),
				new SqlParameter("@serverMultiplyingPower", laby.serverMultiplyingPower),
				new SqlParameter("@LastDate", laby.LastDate),
				new SqlParameter("@ProcessAward", laby.ProcessAward),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[17].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateLabyrinthInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateLabyrinthInfo", exception);
			}
		}
		return result;
	}

	public TotemInfo[] GetAllTotem()
	{
		List<TotemInfo> list = new List<TotemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Totem_All");
			while (ResultDataReader.Read())
			{
				TotemInfo totemInfo = new TotemInfo();
				totemInfo.ID = (int)ResultDataReader["ID"];
				totemInfo.ConsumeExp = (int)ResultDataReader["ConsumeExp"];
				totemInfo.ConsumeHonor = (int)ResultDataReader["ConsumeHonor"];
				totemInfo.AddAttack = (int)ResultDataReader["AddAttack"];
				totemInfo.AddDefence = (int)ResultDataReader["AddDefence"];
				totemInfo.AddAgility = (int)ResultDataReader["AddAgility"];
				totemInfo.AddLuck = (int)ResultDataReader["AddLuck"];
				totemInfo.AddBlood = (int)ResultDataReader["AddBlood"];
				totemInfo.AddDamage = (int)ResultDataReader["AddDamage"];
				totemInfo.AddGuard = (int)ResultDataReader["AddGuard"];
				totemInfo.Random = (int)ResultDataReader["Random"];
				totemInfo.Page = (int)ResultDataReader["Page"];
				totemInfo.Layers = (int)ResultDataReader["Layers"];
				totemInfo.Location = (int)ResultDataReader["Location"];
				totemInfo.Point = (int)ResultDataReader["Point"];
				list.Add(totemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetTotemAll", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public FightSpiritTemplateInfo[] GetAllFightSpiritTemplate()
	{
		List<FightSpiritTemplateInfo> list = new List<FightSpiritTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_FightSpiritTemplate_All");
			while (ResultDataReader.Read())
			{
				FightSpiritTemplateInfo fightSpiritTemplateInfo = new FightSpiritTemplateInfo();
				fightSpiritTemplateInfo.ID = (int)ResultDataReader["ID"];
				fightSpiritTemplateInfo.FightSpiritID = (int)ResultDataReader["FightSpiritID"];
				fightSpiritTemplateInfo.FightSpiritIcon = (string)ResultDataReader["FightSpiritIcon"];
				fightSpiritTemplateInfo.Level = (int)ResultDataReader["Level"];
				fightSpiritTemplateInfo.Exp = (int)ResultDataReader["Exp"];
				fightSpiritTemplateInfo.Attack = (int)ResultDataReader["Attack"];
				fightSpiritTemplateInfo.Defence = (int)ResultDataReader["Defence"];
				fightSpiritTemplateInfo.Agility = (int)ResultDataReader["Agility"];
				fightSpiritTemplateInfo.Lucky = (int)ResultDataReader["Lucky"];
				fightSpiritTemplateInfo.Blood = (int)ResultDataReader["Blood"];
				list.Add(fightSpiritTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetFightSpiritTemplateAll", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public TotemHonorTemplateInfo[] GetAllTotemHonorTemplate()
	{
		List<TotemHonorTemplateInfo> list = new List<TotemHonorTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_TotemHonorTemplate_All");
			while (ResultDataReader.Read())
			{
				TotemHonorTemplateInfo totemHonorTemplateInfo = new TotemHonorTemplateInfo();
				totemHonorTemplateInfo.ID = (int)ResultDataReader["ID"];
				totemHonorTemplateInfo.NeedMoney = (int)ResultDataReader["NeedMoney"];
				totemHonorTemplateInfo.Type = (int)ResultDataReader["Type"];
				totemHonorTemplateInfo.AddHonor = (int)ResultDataReader["AddHonor"];
				list.Add(totemHonorTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetTotemHonorTemplateInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public Dictionary<int, UserDrillInfo> GetPlayerDrillByID(int UserID)
	{
		Dictionary<int, UserDrillInfo> dictionary = new Dictionary<int, UserDrillInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_Users_Drill_All", array);
			while (ResultDataReader.Read())
			{
				UserDrillInfo userDrillInfo = new UserDrillInfo();
				userDrillInfo.UserID = (int)ResultDataReader["UserID"];
				userDrillInfo.BeadPlace = (int)ResultDataReader["BeadPlace"];
				userDrillInfo.HoleLv = (int)ResultDataReader["HoleLv"];
				userDrillInfo.HoleExp = (int)ResultDataReader["HoleExp"];
				userDrillInfo.DrillPlace = (int)ResultDataReader["DrillPlace"];
				if (!dictionary.ContainsKey(userDrillInfo.DrillPlace))
				{
					dictionary.Add(userDrillInfo.DrillPlace, userDrillInfo);
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return dictionary;
	}

	public bool AddUserUserDrill(UserDrillInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@BeadPlace", item.BeadPlace),
				new SqlParameter("@HoleExp", item.HoleExp),
				new SqlParameter("@HoleLv", item.HoleLv),
				new SqlParameter("@DrillPlace", item.DrillPlace),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_UserDrill_Add", array);
			int num = (int)array[5].Value;
			result = num == 0;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserDrillInfo(UserDrillInfo g)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@UserID", g.UserID),
				new SqlParameter("@BeadPlace", g.BeadPlace),
				new SqlParameter("@HoleExp", g.HoleExp),
				new SqlParameter("@HoleLv", g.HoleLv),
				new SqlParameter("@DrillPlace", g.DrillPlace),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserDrillInfo", array);
			result = true;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserDrillInfo", exception);
			}
		}
		return result;
	}

	public TreasureAwardInfo[] GetAllTreasureAward()
	{
		List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Treasure_All");
			while (ResultDataReader.Read())
			{
				TreasureAwardInfo treasureAwardInfo = new TreasureAwardInfo();
				treasureAwardInfo.ID = (int)ResultDataReader["ID"];
				treasureAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				treasureAwardInfo.Name = (string)ResultDataReader["Name"];
				treasureAwardInfo.Count = (int)ResultDataReader["Count"];
				treasureAwardInfo.Validate = (int)ResultDataReader["Validate"];
				treasureAwardInfo.Random = (int)ResultDataReader["Random"];
				list.Add(treasureAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetTreasureAwardAll", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public UserTreasureInfo GetSingleTreasure(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleTreasure", array);
			if (ResultDataReader.Read())
			{
				UserTreasureInfo userTreasureInfo = new UserTreasureInfo();
				userTreasureInfo.ID = (int)ResultDataReader["ID"];
				userTreasureInfo.UserID = (int)ResultDataReader["UserID"];
				userTreasureInfo.NickName = (string)ResultDataReader["NickName"];
				userTreasureInfo.logoinDays = (int)ResultDataReader["logoinDays"];
				userTreasureInfo.treasure = (int)ResultDataReader["treasure"];
				userTreasureInfo.treasureAdd = (int)ResultDataReader["treasureAdd"];
				userTreasureInfo.friendHelpTimes = (int)ResultDataReader["friendHelpTimes"];
				userTreasureInfo.isEndTreasure = (bool)ResultDataReader["isEndTreasure"];
				userTreasureInfo.isBeginTreasure = (bool)ResultDataReader["isBeginTreasure"];
				userTreasureInfo.LastLoginDay = (DateTime)ResultDataReader["LastLoginDay"];
				return userTreasureInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleTreasure", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public List<TreasureDataInfo> GetSingleTreasureData(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		List<TreasureDataInfo> list = new List<TreasureDataInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleTreasureData", array);
			while (ResultDataReader.Read())
			{
				TreasureDataInfo treasureDataInfo = new TreasureDataInfo();
				treasureDataInfo.ID = (int)ResultDataReader["ID"];
				treasureDataInfo.UserID = (int)ResultDataReader["UserID"];
				treasureDataInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				treasureDataInfo.Count = (int)ResultDataReader["Count"];
				treasureDataInfo.ValidDate = (int)ResultDataReader["Validate"];
				treasureDataInfo.pos = (int)ResultDataReader["pos"];
				treasureDataInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				treasureDataInfo.IsExit = (bool)ResultDataReader["IsExit"];
				list.Add(treasureDataInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleTreasureData", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public bool AddUserTreasureInfo(UserTreasureInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[11]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@NickName", item.NickName);
			array[3] = new SqlParameter("@logoinDays", item.logoinDays);
			array[4] = new SqlParameter("@treasure", item.treasure);
			array[5] = new SqlParameter("@treasureAdd", item.treasureAdd);
			array[6] = new SqlParameter("@friendHelpTimes", item.friendHelpTimes);
			array[7] = new SqlParameter("@isEndTreasure", item.isEndTreasure);
			array[8] = new SqlParameter("@isBeginTreasure", item.isBeginTreasure);
			array[9] = new SqlParameter("@LastLoginDay", item.LastLoginDay);
			array[10] = new SqlParameter("@Result", SqlDbType.Int);
			array[10].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Users_Treasure_Add", array);
			result = (int)array[10].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserTreasureInfo(UserTreasureInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[10]
			{
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@NickName", item.NickName),
				new SqlParameter("@logoinDays", item.logoinDays),
				new SqlParameter("@treasure", item.treasure),
				new SqlParameter("@treasureAdd", item.treasureAdd),
				new SqlParameter("@friendHelpTimes", item.friendHelpTimes),
				new SqlParameter("@isEndTreasure", item.isEndTreasure),
				new SqlParameter("@isBeginTreasure", item.isBeginTreasure),
				new SqlParameter("@LastLoginDay", item.LastLoginDay),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[9].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserTreasure", array);
			result = (int)array[9].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserTreasure", exception);
			}
		}
		return result;
	}

	public bool AddTreasureData(TreasureDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@TemplateID", item.TemplateID);
			array[3] = new SqlParameter("@Count", item.Count);
			array[4] = new SqlParameter("@Validate", item.ValidDate);
			array[5] = new SqlParameter("@Pos", item.pos);
			array[6] = new SqlParameter("@BeginDate", item.BeginDate);
			array[7] = new SqlParameter("@IsExit", item.IsExit);
			array[8] = new SqlParameter("@Result", SqlDbType.Int);
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_TreasureData_Add", array);
			result = (int)array[8].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateTreasureData(TreasureDataInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[9]
			{
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@TemplateID", item.TemplateID),
				new SqlParameter("@Count", item.Count),
				new SqlParameter("@Validate", item.ValidDate),
				new SqlParameter("@Pos", item.pos),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[8].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateTreasureData", array);
			result = (int)array[8].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateTreasureData", exception);
			}
		}
		return result;
	}

	public bool RemoveTreasureDataByUser(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_RemoveTreasureDataByUser", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_RemoveTreasureDataByUser", exception);
			}
		}
		return result;
	}

	public bool RemoveIsArrange(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_RemoveIsArrange", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_RemoveIsArrange", exception);
			}
		}
		return result;
	}

	public bool UpdateFriendHelpTimes(int ID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[2]
			{
				new SqlParameter("@UserID", ID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[1].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateFriendHelpTimes", array);
			result = (int)array[1].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateFriendHelpTimes", exception);
			}
		}
		return result;
	}

	public PyramidInfo GetSinglePyramid(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSinglePyramid", array);
			if (ResultDataReader.Read())
			{
				PyramidInfo pyramidInfo = new PyramidInfo();
				pyramidInfo.ID = (int)ResultDataReader["ID"];
				pyramidInfo.UserID = (int)ResultDataReader["UserID"];
				pyramidInfo.currentLayer = (int)ResultDataReader["currentLayer"];
				pyramidInfo.maxLayer = (int)ResultDataReader["maxLayer"];
				pyramidInfo.totalPoint = (int)ResultDataReader["totalPoint"];
				pyramidInfo.turnPoint = (int)ResultDataReader["turnPoint"];
				pyramidInfo.pointRatio = (int)ResultDataReader["pointRatio"];
				pyramidInfo.currentFreeCount = (int)ResultDataReader["currentFreeCount"];
				pyramidInfo.currentReviveCount = (int)ResultDataReader["currentReviveCount"];
				pyramidInfo.isPyramidStart = (bool)ResultDataReader["isPyramidStart"];
				pyramidInfo.LayerItems = (string)ResultDataReader["LayerItems"];
				return pyramidInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSinglePyramid", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool AddPyramid(PyramidInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[12]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@currentLayer", info.currentLayer);
			array[3] = new SqlParameter("@maxLayer", info.maxLayer);
			array[4] = new SqlParameter("@totalPoint", info.totalPoint);
			array[5] = new SqlParameter("@turnPoint", info.turnPoint);
			array[6] = new SqlParameter("@pointRatio", info.pointRatio);
			array[7] = new SqlParameter("@currentFreeCount", info.currentFreeCount);
			array[8] = new SqlParameter("@currentReviveCount", info.currentReviveCount);
			array[9] = new SqlParameter("@isPyramidStart", info.isPyramidStart);
			array[10] = new SqlParameter("@LayerItems", info.LayerItems);
			array[11] = new SqlParameter("@Result", SqlDbType.Int);
			array[11].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Pyramid_Add", array);
			result = (int)array[11].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Pyramid_Add", exception);
			}
		}
		return result;
	}

	public bool UpdatePyramid(PyramidInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[12]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@currentLayer", info.currentLayer),
				new SqlParameter("@maxLayer", info.maxLayer),
				new SqlParameter("@totalPoint", info.totalPoint),
				new SqlParameter("@turnPoint", info.turnPoint),
				new SqlParameter("@pointRatio", info.pointRatio),
				new SqlParameter("@currentFreeCount", info.currentFreeCount),
				new SqlParameter("@currentReviveCount", info.currentReviveCount),
				new SqlParameter("@isPyramidStart", info.isPyramidStart),
				new SqlParameter("@LayerItems", info.LayerItems),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[11].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdatePyramid", array);
			result = (int)array[11].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdatePyramid", exception);
			}
		}
		return result;
	}

	public NewChickenBoxItemInfo[] GetSingleNewChickenBox(int UserID)
	{
		List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleNewChickenBox", array);
			while (ResultDataReader.Read())
			{
				NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
				newChickenBoxItemInfo.ID = (int)ResultDataReader["ID"];
				newChickenBoxItemInfo.UserID = (int)ResultDataReader["UserID"];
				newChickenBoxItemInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				newChickenBoxItemInfo.Count = (int)ResultDataReader["Count"];
				newChickenBoxItemInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				newChickenBoxItemInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				newChickenBoxItemInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				newChickenBoxItemInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				newChickenBoxItemInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				newChickenBoxItemInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				newChickenBoxItemInfo.Position = (int)ResultDataReader["Position"];
				newChickenBoxItemInfo.IsSelected = (bool)ResultDataReader["IsSelected"];
				newChickenBoxItemInfo.IsSeeded = (bool)ResultDataReader["IsSeeded"];
				newChickenBoxItemInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				list.Add(newChickenBoxItemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleNewChickenBox", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public bool AddNewChickenBox(NewChickenBoxItemInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[15]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@TemplateID", info.TemplateID);
			array[3] = new SqlParameter("@Count", info.Count);
			array[4] = new SqlParameter("@ValidDate", info.ValidDate);
			array[5] = new SqlParameter("@StrengthenLevel", info.StrengthenLevel);
			array[6] = new SqlParameter("@AttackCompose", info.AttackCompose);
			array[7] = new SqlParameter("@DefendCompose", info.DefendCompose);
			array[8] = new SqlParameter("@AgilityCompose", info.AgilityCompose);
			array[9] = new SqlParameter("@LuckCompose", info.LuckCompose);
			array[10] = new SqlParameter("@Position", info.Position);
			array[11] = new SqlParameter("@IsSelected", info.IsSelected);
			array[12] = new SqlParameter("@IsSeeded", info.IsSeeded);
			array[13] = new SqlParameter("@IsBinds", info.IsBinds);
			array[14] = new SqlParameter("@Result", SqlDbType.Int);
			array[14].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_NewChickenBox_Add", array);
			result = (int)array[14].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_NewChickenBox_Add", exception);
			}
		}
		return result;
	}

	public bool UpdateNewChickenBox(NewChickenBoxItemInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[15]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@TemplateID", info.TemplateID),
				new SqlParameter("@Count", info.Count),
				new SqlParameter("@ValidDate", info.ValidDate),
				new SqlParameter("@StrengthenLevel", info.StrengthenLevel),
				new SqlParameter("@AttackCompose", info.AttackCompose),
				new SqlParameter("@DefendCompose", info.DefendCompose),
				new SqlParameter("@AgilityCompose", info.AgilityCompose),
				new SqlParameter("@LuckCompose", info.LuckCompose),
				new SqlParameter("@Position", info.Position),
				new SqlParameter("@IsSelected", info.IsSelected),
				new SqlParameter("@IsSeeded", info.IsSeeded),
				new SqlParameter("@IsBinds", info.IsBinds),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[14].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateNewChickenBox", array);
			result = (int)array[14].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateNewChickenBox", exception);
			}
		}
		return result;
	}

	public UserChristmasInfo GetSingleUserChristmas(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUserChristmas", array);
			if (ResultDataReader.Read())
			{
				UserChristmasInfo userChristmasInfo = new UserChristmasInfo();
				userChristmasInfo.ID = (int)ResultDataReader["ID"];
				userChristmasInfo.UserID = (int)ResultDataReader["UserID"];
				userChristmasInfo.exp = (int)ResultDataReader["exp"];
				userChristmasInfo.awardState = (int)ResultDataReader["awardState"];
				userChristmasInfo.count = (int)ResultDataReader["count"];
				userChristmasInfo.packsNumber = (int)ResultDataReader["packsNumber"];
				userChristmasInfo.lastPacks = (int)ResultDataReader["lastPacks"];
				userChristmasInfo.gameBeginTime = (DateTime)ResultDataReader["gameBeginTime"];
				userChristmasInfo.gameEndTime = (DateTime)ResultDataReader["gameEndTime"];
				userChristmasInfo.isEnter = (bool)ResultDataReader["isEnter"];
				userChristmasInfo.dayPacks = (int)ResultDataReader["dayPacks"];
				userChristmasInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				return userChristmasInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserChristmas", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool AddUserChristmas(UserChristmasInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[13]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@exp", info.exp);
			array[3] = new SqlParameter("@awardState", info.awardState);
			array[4] = new SqlParameter("@count", info.count);
			array[5] = new SqlParameter("@packsNumber", info.packsNumber);
			array[6] = new SqlParameter("@lastPacks", info.lastPacks);
			array[7] = new SqlParameter("@gameBeginTime", info.gameBeginTime);
			array[8] = new SqlParameter("@gameEndTime", info.gameEndTime);
			array[9] = new SqlParameter("@isEnter", info.isEnter);
			array[10] = new SqlParameter("@dayPacks", info.dayPacks);
			array[11] = new SqlParameter("@AvailTime", info.AvailTime);
			array[12] = new SqlParameter("@Result", SqlDbType.Int);
			array[12].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserChristmas_Add", array);
			result = (int)array[12].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserChristmas(UserChristmasInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[13]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@exp", info.exp),
				new SqlParameter("@awardState", info.awardState),
				new SqlParameter("@count", info.count),
				new SqlParameter("@packsNumber", info.packsNumber),
				new SqlParameter("@lastPacks", info.lastPacks),
				new SqlParameter("@gameBeginTime", info.gameBeginTime),
				new SqlParameter("@gameEndTime", info.gameEndTime),
				new SqlParameter("@isEnter", info.isEnter),
				new SqlParameter("@dayPacks", info.dayPacks),
				new SqlParameter("@AvailTime", info.AvailTime),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[12].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserChristmas", array);
			result = (int)array[12].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserChristmas", exception);
			}
		}
		return result;
	}

	public bool ResetDragonBoat()
	{
		bool result = false;
		try
		{
			result = db.RunProcedure("SP_Reset_DragonBoat_Data");
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init ResetDragonBoat", exception);
			}
		}
		return result;
	}

	public bool ResetCommunalActive(int ActiveID, bool IsReset)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@ActiveID", ActiveID),
				new SqlParameter("@IsReset", IsReset),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Reset_CommunalActive", array);
			result = (int)array[2].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init CommunalActive", exception);
			}
		}
		return result;
	}

	public LuckyStartToptenAwardInfo[] GetAllLuckyStartToptenAward()
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_LuckyStart_Topten_Award_All");
			while (ResultDataReader.Read())
			{
				LuckyStartToptenAwardInfo luckyStartToptenAwardInfo = new LuckyStartToptenAwardInfo();
				luckyStartToptenAwardInfo.ID = (int)ResultDataReader["ID"];
				luckyStartToptenAwardInfo.Type = (int)ResultDataReader["Type"];
				luckyStartToptenAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				luckyStartToptenAwardInfo.Validate = (int)ResultDataReader["Validate"];
				luckyStartToptenAwardInfo.Count = (int)ResultDataReader["Count"];
				luckyStartToptenAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				luckyStartToptenAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				luckyStartToptenAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				luckyStartToptenAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				luckyStartToptenAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				luckyStartToptenAwardInfo.IsBinds = (bool)ResultDataReader["IsBind"];
				list.Add(luckyStartToptenAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetLuckyStart_Topten_Award_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public LuckyStartToptenAwardInfo[] GetAllLanternriddlesTopTenAward()
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_LanternriddlesTopTenAward_All");
			while (ResultDataReader.Read())
			{
				LuckyStartToptenAwardInfo luckyStartToptenAwardInfo = new LuckyStartToptenAwardInfo();
				luckyStartToptenAwardInfo.ID = (int)ResultDataReader["ID"];
				luckyStartToptenAwardInfo.Type = (int)ResultDataReader["Type"];
				luckyStartToptenAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				luckyStartToptenAwardInfo.Validate = (int)ResultDataReader["Validate"];
				luckyStartToptenAwardInfo.Count = (int)ResultDataReader["Count"];
				luckyStartToptenAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				luckyStartToptenAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				luckyStartToptenAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				luckyStartToptenAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				luckyStartToptenAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				luckyStartToptenAwardInfo.IsBinds = (bool)ResultDataReader["IsBind"];
				list.Add(luckyStartToptenAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetLuckyStart_Topten_Award_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActivitySystemItemInfo[] GetAllActivitySystemItem()
	{
		List<ActivitySystemItemInfo> list = new List<ActivitySystemItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ActivitySystemItem_All");
			while (ResultDataReader.Read())
			{
				ActivitySystemItemInfo activitySystemItemInfo = new ActivitySystemItemInfo();
				activitySystemItemInfo.ID = (int)ResultDataReader["ID"];
				activitySystemItemInfo.ActivityType = (int)ResultDataReader["ActivityType"];
				activitySystemItemInfo.Quality = (int)ResultDataReader["Quality"];
				activitySystemItemInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				activitySystemItemInfo.Count = (int)ResultDataReader["Count"];
				activitySystemItemInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				activitySystemItemInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				activitySystemItemInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				activitySystemItemInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				activitySystemItemInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				activitySystemItemInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				activitySystemItemInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				activitySystemItemInfo.Random = (int)ResultDataReader["Random"];
				list.Add(activitySystemItemInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllActivitySystemItem", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActiveSystemInfo[] GetAllActiveSystemData()
	{
		List<ActiveSystemInfo> list = new List<ActiveSystemInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ActiveSystem_All");
			while (ResultDataReader.Read())
			{
				ActiveSystemInfo activeSystemInfo = new ActiveSystemInfo();
				activeSystemInfo.UserID = (int)ResultDataReader["UserID"];
				activeSystemInfo.useableScore = (int)ResultDataReader["useableScore"];
				activeSystemInfo.totalScore = (int)ResultDataReader["totalScore"];
				activeSystemInfo.NickName = (string)ResultDataReader["NickName"];
				activeSystemInfo.myRank = num;
				activeSystemInfo.CanGetGift = (bool)ResultDataReader["CanGetGift"];
				list.Add(activeSystemInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllActiveSystem", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public ActiveSystemInfo GetSingleActiveSystem(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleActiveSystem", array);
			if (ResultDataReader.Read())
			{
				ActiveSystemInfo activeSystemInfo = new ActiveSystemInfo();
				activeSystemInfo.ID = (int)ResultDataReader["ID"];
				activeSystemInfo.UserID = (int)ResultDataReader["UserID"];
				activeSystemInfo.useableScore = (int)ResultDataReader["useableScore"];
				activeSystemInfo.totalScore = (int)ResultDataReader["totalScore"];
				activeSystemInfo.AvailTime = (int)ResultDataReader["AvailTime"];
				activeSystemInfo.NickName = (string)ResultDataReader["NickName"];
				activeSystemInfo.CanGetGift = (bool)ResultDataReader["CanGetGift"];
				activeSystemInfo.canOpenCounts = (int)ResultDataReader["canOpenCounts"];
				activeSystemInfo.canEagleEyeCounts = (int)ResultDataReader["canEagleEyeCounts"];
				activeSystemInfo.lastFlushTime = (DateTime)ResultDataReader["lastFlushTime"];
				activeSystemInfo.isShowAll = (bool)ResultDataReader["isShowAll"];
				activeSystemInfo.ActiveMoney = (int)ResultDataReader["AvtiveMoney"];
				activeSystemInfo.activityTanabataNum = (int)ResultDataReader["activityTanabataNum"];
				activeSystemInfo.ChallengeNum = (int)ResultDataReader["ChallengeNum"];
				activeSystemInfo.BuyBuffNum = (int)ResultDataReader["BuyBuffNum"];
				activeSystemInfo.lastEnterYearMonter = (DateTime)ResultDataReader["lastEnterYearMonter"];
				activeSystemInfo.DamageNum = (int)ResultDataReader["DamageNum"];
				activeSystemInfo.BoxState = (string)ResultDataReader["BoxState"];
				activeSystemInfo.LuckystarCoins = (int)ResultDataReader["LuckystarCoins"];
				return activeSystemInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleActiveSystem", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool AddActiveSystem(ActiveSystemInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[20]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@useableScore", info.useableScore);
			array[3] = new SqlParameter("@totalScore", info.totalScore);
			array[4] = new SqlParameter("@AvailTime", info.AvailTime);
			array[5] = new SqlParameter("@NickName", info.NickName);
			array[6] = new SqlParameter("@CanGetGift", info.CanGetGift);
			array[7] = new SqlParameter("@canOpenCounts", info.canOpenCounts);
			array[8] = new SqlParameter("@canEagleEyeCounts", info.canEagleEyeCounts);
			array[9] = new SqlParameter("@lastFlushTime", info.lastFlushTime);
			array[10] = new SqlParameter("@isShowAll", info.isShowAll);
			array[11] = new SqlParameter("@AvtiveMoney", info.ActiveMoney);
			array[12] = new SqlParameter("@activityTanabataNum", info.activityTanabataNum);
			array[13] = new SqlParameter("@ChallengeNum", info.ChallengeNum);
			array[14] = new SqlParameter("@BuyBuffNum", info.BuyBuffNum);
			array[15] = new SqlParameter("@lastEnterYearMonter", info.lastEnterYearMonter);
			array[16] = new SqlParameter("@DamageNum", info.DamageNum);
			array[17] = new SqlParameter("@BoxState", info.BoxState);
			array[18] = new SqlParameter("@LuckystarCoins", info.LuckystarCoins);
			array[19] = new SqlParameter("@Result", SqlDbType.Int);
			array[19].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ActiveSystem_Add", array);
			result = (int)array[19].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateActiveSystem(ActiveSystemInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[20]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@useableScore", info.useableScore),
				new SqlParameter("@totalScore", info.totalScore),
				new SqlParameter("@AvailTime", info.AvailTime),
				new SqlParameter("@NickName", info.NickName),
				new SqlParameter("@CanGetGift", info.CanGetGift),
				new SqlParameter("@canOpenCounts", info.canOpenCounts),
				new SqlParameter("@canEagleEyeCounts", info.canEagleEyeCounts),
				new SqlParameter("@lastFlushTime", info.lastFlushTime),
				new SqlParameter("@isShowAll", info.isShowAll),
				new SqlParameter("@AvtiveMoney", info.ActiveMoney),
				new SqlParameter("@activityTanabataNum", info.activityTanabataNum),
				new SqlParameter("@ChallengeNum", info.ChallengeNum),
				new SqlParameter("@BuyBuffNum", info.BuyBuffNum),
				new SqlParameter("@lastEnterYearMonter", info.lastEnterYearMonter),
				new SqlParameter("@DamageNum", info.DamageNum),
				new SqlParameter("@BoxState", info.BoxState),
				new SqlParameter("@LuckystarCoins", info.LuckystarCoins),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[19].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateActiveSystem", array);
			result = (int)array[19].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateActiveSystem", exception);
			}
		}
		return result;
	}

	public LightriddleQuestInfo[] GetAllLightriddleQuestInfo()
	{
		List<LightriddleQuestInfo> list = new List<LightriddleQuestInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Lightriddle_Quest_All");
			while (ResultDataReader.Read())
			{
				LightriddleQuestInfo lightriddleQuestInfo = new LightriddleQuestInfo();
				lightriddleQuestInfo.QuestionID = (int)ResultDataReader["QuestionID"];
				lightriddleQuestInfo.QuestionContent = (string)ResultDataReader["QuestionContent"];
				lightriddleQuestInfo.Option1 = (string)ResultDataReader["Option1"];
				lightriddleQuestInfo.Option2 = (string)ResultDataReader["Option2"];
				lightriddleQuestInfo.Option3 = (string)ResultDataReader["Option3"];
				lightriddleQuestInfo.Option4 = (string)ResultDataReader["Option4"];
				lightriddleQuestInfo.OptionTrue = (int)ResultDataReader["OptionTrue"];
				list.Add(lightriddleQuestInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_Lightriddle_Quest_All", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public UserMatchInfo[] GetAllUserMatchInfo()
	{
		List<UserMatchInfo> list = new List<UserMatchInfo>();
		SqlDataReader ResultDataReader = null;
		int num = 1;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_UserMatch_All_DESC");
			while (ResultDataReader.Read())
			{
				UserMatchInfo userMatchInfo = new UserMatchInfo();
				userMatchInfo.UserID = (int)ResultDataReader["UserID"];
				userMatchInfo.totalPrestige = (int)ResultDataReader["totalPrestige"];
				userMatchInfo.rank = num;
				list.Add(userMatchInfo);
				num++;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllUserMatchDESC", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list.ToArray();
	}

	public UserMatchInfo GetSingleUserMatchInfo(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUserMatchInfo", array);
			if (ResultDataReader.Read())
			{
				UserMatchInfo userMatchInfo = new UserMatchInfo();
				userMatchInfo.ID = (int)ResultDataReader["ID"];
				userMatchInfo.UserID = (int)ResultDataReader["UserID"];
				userMatchInfo.dailyScore = (int)ResultDataReader["dailyScore"];
				userMatchInfo.dailyWinCount = (int)ResultDataReader["dailyWinCount"];
				userMatchInfo.dailyGameCount = (int)ResultDataReader["dailyGameCount"];
				userMatchInfo.DailyLeagueFirst = (bool)ResultDataReader["DailyLeagueFirst"];
				userMatchInfo.DailyLeagueLastScore = (int)ResultDataReader["DailyLeagueLastScore"];
				userMatchInfo.weeklyScore = (int)ResultDataReader["weeklyScore"];
				userMatchInfo.weeklyGameCount = (int)ResultDataReader["weeklyGameCount"];
				userMatchInfo.weeklyRanking = (int)ResultDataReader["weeklyRanking"];
				userMatchInfo.addDayPrestge = (int)ResultDataReader["addDayPrestge"];
				userMatchInfo.totalPrestige = (int)ResultDataReader["totalPrestige"];
				userMatchInfo.restCount = (int)ResultDataReader["restCount"];
				return userMatchInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserMatchInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return null;
	}

	public bool AddUserMatchInfo(UserMatchInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", info.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", info.UserID);
			array[2] = new SqlParameter("@dailyScore", info.dailyScore);
			array[3] = new SqlParameter("@dailyWinCount", info.dailyWinCount);
			array[4] = new SqlParameter("@dailyGameCount", info.dailyGameCount);
			array[5] = new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst);
			array[6] = new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore);
			array[7] = new SqlParameter("@weeklyScore", info.weeklyScore);
			array[8] = new SqlParameter("@weeklyGameCount", info.weeklyGameCount);
			array[9] = new SqlParameter("@weeklyRanking", info.weeklyRanking);
			array[10] = new SqlParameter("@addDayPrestge", info.addDayPrestge);
			array[11] = new SqlParameter("@totalPrestige", info.totalPrestige);
			array[12] = new SqlParameter("@restCount", info.restCount);
			array[13] = new SqlParameter("@Result", SqlDbType.Int);
			array[13].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserMatch_Add", array);
			result = (int)array[13].Value == 0;
			info.ID = (int)array[0].Value;
			info.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserMatchInfo(UserMatchInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@dailyScore", info.dailyScore),
				new SqlParameter("@dailyWinCount", info.dailyWinCount),
				new SqlParameter("@dailyGameCount", info.dailyGameCount),
				new SqlParameter("@DailyLeagueFirst", info.DailyLeagueFirst),
				new SqlParameter("@DailyLeagueLastScore", info.DailyLeagueLastScore),
				new SqlParameter("@weeklyScore", info.weeklyScore),
				new SqlParameter("@weeklyGameCount", info.weeklyGameCount),
				new SqlParameter("@weeklyRanking", info.weeklyRanking),
				new SqlParameter("@addDayPrestge", info.addDayPrestge),
				new SqlParameter("@totalPrestige", info.totalPrestige),
				new SqlParameter("@restCount", info.restCount),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[13].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserMatch", array);
			result = (int)array[13].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserMatch", exception);
			}
		}
		return result;
	}

	public List<UserRankInfo> GetSingleUserRank(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		List<UserRankInfo> list = new List<UserRankInfo>();
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@UserID", SqlDbType.Int, 4)
			};
			array[0].Value = UserID;
			db.GetReader(ref ResultDataReader, "SP_GetSingleUserRank", array);
			while (ResultDataReader.Read())
			{
				UserRankInfo userRankInfo = new UserRankInfo();
				userRankInfo.ID = (int)ResultDataReader["ID"];
				userRankInfo.UserID = (int)ResultDataReader["UserID"];
				userRankInfo.UserRank = (string)ResultDataReader["UserRank"];
				userRankInfo.Attack = (int)ResultDataReader["Attack"];
				userRankInfo.Defence = (int)ResultDataReader["Defence"];
				userRankInfo.Luck = (int)ResultDataReader["Luck"];
				userRankInfo.Agility = (int)ResultDataReader["Agility"];
				userRankInfo.HP = (int)ResultDataReader["HP"];
				userRankInfo.Damage = (int)ResultDataReader["Damage"];
				userRankInfo.Guard = (int)ResultDataReader["Guard"];
				userRankInfo.BeginDate = (DateTime)ResultDataReader["BeginDate"];
				userRankInfo.Validate = (int)ResultDataReader["Validate"];
				userRankInfo.IsExit = (bool)ResultDataReader["IsExit"];
				list.Add(userRankInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_GetSingleUserRankInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public bool AddUserRank(UserRankInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", item.ID),
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null,
				null
			};
			array[0].Direction = ParameterDirection.Output;
			array[1] = new SqlParameter("@UserID", item.UserID);
			array[2] = new SqlParameter("@UserRank", item.UserRank);
			array[3] = new SqlParameter("@Attack", item.Attack);
			array[4] = new SqlParameter("@Defence", item.Defence);
			array[5] = new SqlParameter("@Luck", item.Luck);
			array[6] = new SqlParameter("@Agility", item.Agility);
			array[7] = new SqlParameter("@HP", item.HP);
			array[8] = new SqlParameter("@Damage", item.Damage);
			array[9] = new SqlParameter("@Guard", item.Guard);
			array[10] = new SqlParameter("@BeginDate", item.BeginDate);
			array[11] = new SqlParameter("@Validate", item.Validate);
			array[12] = new SqlParameter("@IsExit", item.IsExit);
			array[13] = new SqlParameter("@Result", SqlDbType.Int);
			array[13].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UserRank_Add", array);
			result = (int)array[13].Value == 0;
			item.ID = (int)array[0].Value;
			item.IsDirty = false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
		}
		return result;
	}

	public bool UpdateUserRank(UserRankInfo item)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[14]
			{
				new SqlParameter("@ID", item.ID),
				new SqlParameter("@UserID", item.UserID),
				new SqlParameter("@UserRank", item.UserRank),
				new SqlParameter("@Attack", item.Attack),
				new SqlParameter("@Defence", item.Defence),
				new SqlParameter("@Luck", item.Luck),
				new SqlParameter("@Agility", item.Agility),
				new SqlParameter("@HP", item.HP),
				new SqlParameter("@Damage", item.Damage),
				new SqlParameter("@Guard", item.Guard),
				new SqlParameter("@BeginDate", item.BeginDate),
				new SqlParameter("@Validate", item.Validate),
				new SqlParameter("@IsExit", item.IsExit),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[13].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_UpdateUserRank", array);
			result = (int)array[13].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("SP_UpdateUserRank", exception);
			}
		}
		return result;
	}
}
