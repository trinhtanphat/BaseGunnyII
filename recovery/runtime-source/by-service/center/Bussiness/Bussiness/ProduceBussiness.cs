using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class ProduceBussiness : BaseBussiness
{
	public LoadUserBoxInfo[] GetAllTimeBoxAward()
	{
		List<LoadUserBoxInfo> list = new List<LoadUserBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_TimeBox_Award_All");
			while (ResultDataReader.Read())
			{
				LoadUserBoxInfo loadUserBoxInfo = new LoadUserBoxInfo();
				loadUserBoxInfo.ID = (int)ResultDataReader["ID"];
				loadUserBoxInfo.Type = (int)ResultDataReader["Type"];
				loadUserBoxInfo.Level = (int)ResultDataReader["Level"];
				loadUserBoxInfo.Condition = (int)ResultDataReader["Condition"];
				loadUserBoxInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				list.Add(loadUserBoxInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllDaily", exception);
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

	public QQtipsMessagesInfo[] GetAllQQtipsMessagesLoad()
	{
		List<QQtipsMessagesInfo> list = new List<QQtipsMessagesInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_QQtipsMessages_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQQtipsMessagesLoad(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllQQtipsMessagesLoad", exception);
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

	public QQtipsMessagesInfo InitQQtipsMessagesLoad(SqlDataReader reader)
	{
		QQtipsMessagesInfo qQtipsMessagesInfo = new QQtipsMessagesInfo();
		qQtipsMessagesInfo.ID = (int)reader["ID"];
		qQtipsMessagesInfo.title = ((reader["title"] == null) ? "QQTips" : reader["title"].ToString());
		qQtipsMessagesInfo.content = ((reader["content"] == null) ? "Thông báo, gợi ý hệ thống" : reader["content"].ToString());
		qQtipsMessagesInfo.maxLevel = (int)reader["maxLevel"];
		qQtipsMessagesInfo.minLevel = (int)reader["minLevel"];
		qQtipsMessagesInfo.outInType = (int)reader["outInType"];
		qQtipsMessagesInfo.moduleType = (int)reader["moduleType"];
		qQtipsMessagesInfo.inItemID = (int)reader["inItemID"];
		qQtipsMessagesInfo.url = ((reader["url"] == null) ? "http://gunny.zing.vn" : reader["url"].ToString());
		return qQtipsMessagesInfo;
	}

	public GoldEquipTemplateLoadInfo[] GetAllGoldEquipTemplateLoad()
	{
		List<GoldEquipTemplateLoadInfo> list = new List<GoldEquipTemplateLoadInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_GoldEquipTemplateLoad_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitGoldEquipTemplateLoad(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllGoldEquipTemplateLoad", exception);
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

	public GoldEquipTemplateLoadInfo InitGoldEquipTemplateLoad(SqlDataReader reader)
	{
		GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = new GoldEquipTemplateLoadInfo();
		goldEquipTemplateLoadInfo.ID = (int)reader["ID"];
		goldEquipTemplateLoadInfo.OldTemplateId = (int)reader["OldTemplateId"];
		goldEquipTemplateLoadInfo.NewTemplateId = (int)reader["NewTemplateId"];
		goldEquipTemplateLoadInfo.CategoryID = (int)reader["CategoryID"];
		goldEquipTemplateLoadInfo.Strengthen = (int)reader["Strengthen"];
		goldEquipTemplateLoadInfo.Attack = (int)reader["Attack"];
		goldEquipTemplateLoadInfo.Defence = (int)reader["Defence"];
		goldEquipTemplateLoadInfo.Agility = (int)reader["Agility"];
		goldEquipTemplateLoadInfo.Luck = (int)reader["Luck"];
		goldEquipTemplateLoadInfo.Damage = (int)reader["Damage"];
		goldEquipTemplateLoadInfo.Guard = (int)reader["Guard"];
		goldEquipTemplateLoadInfo.Boold = (int)reader["Boold"];
		goldEquipTemplateLoadInfo.BlessID = (int)reader["BlessID"];
		goldEquipTemplateLoadInfo.Pic = ((reader["pic"] == null) ? "" : reader["pic"].ToString());
		return goldEquipTemplateLoadInfo;
	}

	public AchievementInfo[] GetALlAchievement()
	{
		List<AchievementInfo> list = new List<AchievementInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Achievement_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievement(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetALlAchievement:", exception);
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

	public AchievementConditionInfo[] GetALlAchievementCondition()
	{
		List<AchievementConditionInfo> list = new List<AchievementConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Achievement_Condition_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievementCondition(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetALlAchievementCondition:", exception);
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

	public AchievementDataInfo[] GetAllAchievementData(int userID)
	{
		List<AchievementDataInfo> list = new List<AchievementDataInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", userID)
			};
			db.GetReader(ref ResultDataReader, "SP_Achievement_Data_All", sqlParameters);
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievementData(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllAchievementData", exception);
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

	public AchievementRewardInfo[] GetALlAchievementReward()
	{
		List<AchievementRewardInfo> list = new List<AchievementRewardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Achievement_Reward_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitAchievementReward(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetALlAchievementReward", exception);
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

	public ActiveAwardInfo[] GetAllActiveAwardInfo()
	{
		List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Active_Award");
			while (ResultDataReader.Read())
			{
				ActiveAwardInfo activeAwardInfo = new ActiveAwardInfo();
				activeAwardInfo.ID = (int)ResultDataReader["ID"];
				activeAwardInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				activeAwardInfo.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				activeAwardInfo.AttackCompose = (int)ResultDataReader["AttackCompose"];
				activeAwardInfo.Count = (int)ResultDataReader["Count"];
				activeAwardInfo.DefendCompose = (int)ResultDataReader["DefendCompose"];
				activeAwardInfo.Gold = (int)ResultDataReader["Gold"];
				activeAwardInfo.ItemID = (int)ResultDataReader["ItemID"];
				activeAwardInfo.LuckCompose = (int)ResultDataReader["LuckCompose"];
				activeAwardInfo.Mark = (int)ResultDataReader["Mark"];
				activeAwardInfo.Money = (int)ResultDataReader["Money"];
				activeAwardInfo.Sex = (int)ResultDataReader["Sex"];
				activeAwardInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				activeAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				activeAwardInfo.GiftToken = (int)ResultDataReader["GiftToken"];
				ActiveAwardInfo item = activeAwardInfo;
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllActiveAwardInfo", exception);
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

	public ActiveConditionInfo[] GetAllActiveConditionInfo()
	{
		List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Active_Condition");
			while (ResultDataReader.Read())
			{
				ActiveConditionInfo activeConditionInfo = new ActiveConditionInfo();
				activeConditionInfo.ID = (int)ResultDataReader["ID"];
				activeConditionInfo.ActiveID = (int)ResultDataReader["ActiveID"];
				activeConditionInfo.Conditiontype = (int)ResultDataReader["Conditiontype"];
				activeConditionInfo.Condition = (int)ResultDataReader["Condition"];
				activeConditionInfo.LimitGrade = ((ResultDataReader["LimitGrade"].ToString() == null) ? "" : ResultDataReader["LimitGrade"].ToString());
				activeConditionInfo.AwardId = ((ResultDataReader["AwardId"].ToString() == null) ? "" : ResultDataReader["AwardId"].ToString());
				activeConditionInfo.IsMult = (bool)ResultDataReader["IsMult"];
				activeConditionInfo.StartTime = (DateTime)ResultDataReader["StartTime"];
				activeConditionInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				ActiveConditionInfo item = activeConditionInfo;
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllActiveConditionInfo", exception);
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

	public List<BigBugleInfo> GetAllAreaBigBugleRecord()
	{
		SqlDataReader ResultDataReader = null;
		List<BigBugleInfo> list = new List<BigBugleInfo>();
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Get_AreaBigBugle_Record");
			while (ResultDataReader.Read())
			{
				BigBugleInfo bigBugleInfo = new BigBugleInfo();
				bigBugleInfo.ID = (int)ResultDataReader["ID"];
				bigBugleInfo.UserID = (int)ResultDataReader["UserID"];
				bigBugleInfo.AreaID = (int)ResultDataReader["AreaID"];
				bigBugleInfo.NickName = ((ResultDataReader["NickName"] == null) ? "" : ResultDataReader["NickName"].ToString());
				bigBugleInfo.Message = ((ResultDataReader["Message"] == null) ? "" : ResultDataReader["Message"].ToString());
				bigBugleInfo.State = (bool)ResultDataReader["State"];
				bigBugleInfo.IP = ((ResultDataReader["IP"] == null) ? "" : ResultDataReader["IP"].ToString());
				BigBugleInfo item = bigBugleInfo;
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllAreaBigBugleRecord", exception);
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

	public AchievementInfo InitAchievement(SqlDataReader reader)
	{
		AchievementInfo achievementInfo = new AchievementInfo();
		achievementInfo.ID = (int)reader["ID"];
		achievementInfo.PlaceID = (int)reader["PlaceID"];
		achievementInfo.Title = ((reader["Title"] == null) ? "" : reader["Title"].ToString());
		achievementInfo.Detail = ((reader["Detail"] == null) ? "" : reader["Detail"].ToString());
		achievementInfo.NeedMinLevel = (int)reader["NeedMinLevel"];
		achievementInfo.NeedMaxLevel = (int)reader["NeedMaxLevel"];
		achievementInfo.PreAchievementID = ((reader["PreAchievementID"] == null) ? "" : reader["PreAchievementID"].ToString());
		achievementInfo.IsOther = (int)reader["IsOther"];
		achievementInfo.AchievementType = (int)reader["AchievementType"];
		achievementInfo.CanHide = (bool)reader["CanHide"];
		achievementInfo.StartDate = (DateTime)reader["StartDate"];
		achievementInfo.EndDate = (DateTime)reader["EndDate"];
		achievementInfo.AchievementPoint = (int)reader["AchievementPoint"];
		achievementInfo.IsActive = (int)reader["IsActive"];
		achievementInfo.PicID = (int)reader["PicID"];
		achievementInfo.IsShare = (bool)reader["IsShare"];
		return achievementInfo;
	}

	public AchievementConditionInfo InitAchievementCondition(SqlDataReader reader)
	{
		AchievementConditionInfo achievementConditionInfo = new AchievementConditionInfo();
		achievementConditionInfo.AchievementID = (int)reader["AchievementID"];
		achievementConditionInfo.CondictionID = (int)reader["CondictionID"];
		achievementConditionInfo.CondictionType = (int)reader["CondictionType"];
		achievementConditionInfo.Condiction_Para1 = ((reader["Condiction_Para1"] == null) ? "" : reader["Condiction_Para1"].ToString());
		achievementConditionInfo.Condiction_Para2 = (int)reader["Condiction_Para2"];
		return achievementConditionInfo;
	}

	public AchievementDataInfo InitAchievementData(SqlDataReader reader)
	{
		AchievementDataInfo achievementDataInfo = new AchievementDataInfo();
		achievementDataInfo.UserID = (int)reader["UserID"];
		achievementDataInfo.AchievementID = (int)reader["AchievementID"];
		achievementDataInfo.IsComplete = (bool)reader["IsComplete"];
		achievementDataInfo.CompletedDate = (DateTime)reader["CompletedDate"];
		return achievementDataInfo;
	}

	public AchievementRewardInfo InitAchievementReward(SqlDataReader reader)
	{
		AchievementRewardInfo achievementRewardInfo = new AchievementRewardInfo();
		achievementRewardInfo.AchievementID = (int)reader["AchievementID"];
		achievementRewardInfo.RewardType = (int)reader["RewardType"];
		achievementRewardInfo.RewardPara = ((reader["RewardPara"] == null) ? "" : reader["RewardPara"].ToString());
		achievementRewardInfo.RewardValueId = (int)reader["RewardValueId"];
		achievementRewardInfo.RewardCount = (int)reader["RewardCount"];
		return achievementRewardInfo;
	}

	public ItemRecordTypeInfo[] GetAllItemRecordType()
	{
		List<ItemRecordTypeInfo> list = new List<ItemRecordTypeInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Record_Type_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemRecordType(ResultDataReader));
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllItemRecordType:", exception);
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

	public ItemRecordTypeInfo InitItemRecordType(SqlDataReader reader)
	{
		ItemRecordTypeInfo itemRecordTypeInfo = new ItemRecordTypeInfo();
		itemRecordTypeInfo.RecordID = (int)reader["RecordID"];
		itemRecordTypeInfo.Name = ((reader["Name"] == null) ? "" : reader["Name"].ToString());
		itemRecordTypeInfo.Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString());
		return itemRecordTypeInfo;
	}

	public ItemTemplateInfo[] GetAllGoodsASC()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_All_ASC");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ItemTemplateInfo[] GetAllGoods()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ShopGoodsShowListInfo InitShopGoodsShowListInfo(SqlDataReader reader)
	{
		ShopGoodsShowListInfo shopGoodsShowListInfo = new ShopGoodsShowListInfo();
		shopGoodsShowListInfo.Type = (int)reader["Type"];
		shopGoodsShowListInfo.ShopId = (int)reader["ShopId"];
		return shopGoodsShowListInfo;
	}

	public ShopGoodsShowListInfo[] GetAllShopGoodsShowList()
	{
		List<ShopGoodsShowListInfo> list = new List<ShopGoodsShowListInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ShopGoodsShowList_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitShopGoodsShowListInfo(ResultDataReader));
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

	public ItemBoxInfo[] GetSingleItemsBox(int DataID)
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = DataID;
			db.GetReader(ref ResultDataReader, "SP_ItemsBox_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItemBoxInfo(ResultDataReader));
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

	public ItemTemplateInfo GetSingleGoods(int goodsID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = goodsID;
			db.GetReader(ref ResultDataReader, "SP_Items_Single", array);
			if (ResultDataReader.Read())
			{
				return InitItemTemplateInfo(ResultDataReader);
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

	public ItemTemplateInfo[] GetSingleCategory(int CategoryID)
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@CategoryID", SqlDbType.Int, 4)
			};
			array[0].Value = CategoryID;
			db.GetReader(ref ResultDataReader, "SP_Items_Category_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ItemTemplateInfo[] GetFusionType()
	{
		List<ItemTemplateInfo> list = new List<ItemTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_FusionType");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemTemplateInfo(ResultDataReader));
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

	public ItemTemplateInfo InitItemTemplateInfo(SqlDataReader reader)
	{
		ItemTemplateInfo itemTemplateInfo = new ItemTemplateInfo();
		itemTemplateInfo.AddTime = reader["AddTime"].ToString();
		itemTemplateInfo.Agility = (int)reader["Agility"];
		itemTemplateInfo.Attack = (int)reader["Attack"];
		itemTemplateInfo.CanDelete = (bool)reader["CanDelete"];
		itemTemplateInfo.CanDrop = (bool)reader["CanDrop"];
		itemTemplateInfo.CanEquip = (bool)reader["CanEquip"];
		itemTemplateInfo.CanUse = (bool)reader["CanUse"];
		itemTemplateInfo.CategoryID = (int)reader["CategoryID"];
		itemTemplateInfo.Colors = reader["Colors"].ToString();
		itemTemplateInfo.Defence = (int)reader["Defence"];
		itemTemplateInfo.Description = reader["Description"].ToString();
		itemTemplateInfo.Level = (int)reader["Level"];
		itemTemplateInfo.Luck = (int)reader["Luck"];
		itemTemplateInfo.MaxCount = (int)reader["MaxCount"];
		itemTemplateInfo.Name = reader["Name"].ToString();
		itemTemplateInfo.NeedSex = (int)reader["NeedSex"];
		itemTemplateInfo.Pic = reader["Pic"].ToString();
		itemTemplateInfo.Data = ((reader["Data"] == null) ? "" : reader["Data"].ToString());
		itemTemplateInfo.Property1 = (int)reader["Property1"];
		itemTemplateInfo.Property2 = (int)reader["Property2"];
		itemTemplateInfo.Property3 = (int)reader["Property3"];
		itemTemplateInfo.Property4 = (int)reader["Property4"];
		itemTemplateInfo.Property5 = (int)reader["Property5"];
		itemTemplateInfo.Property6 = (int)reader["Property6"];
		itemTemplateInfo.Property7 = (int)reader["Property7"];
		itemTemplateInfo.Property8 = (int)reader["Property8"];
		itemTemplateInfo.Quality = (int)reader["Quality"];
		itemTemplateInfo.Script = reader["Script"].ToString();
		itemTemplateInfo.TemplateID = (int)reader["TemplateID"];
		itemTemplateInfo.CanCompose = (bool)reader["CanCompose"];
		itemTemplateInfo.CanStrengthen = (bool)reader["CanStrengthen"];
		itemTemplateInfo.NeedLevel = (int)reader["NeedLevel"];
		itemTemplateInfo.BindType = (int)reader["BindType"];
		itemTemplateInfo.FusionType = (int)reader["FusionType"];
		itemTemplateInfo.FusionRate = (int)reader["FusionRate"];
		itemTemplateInfo.FusionNeedRate = (int)reader["FusionNeedRate"];
		itemTemplateInfo.Hole = ((reader["Hole"] == null) ? "" : reader["Hole"].ToString());
		itemTemplateInfo.RefineryLevel = (int)reader["RefineryLevel"];
		itemTemplateInfo.ReclaimValue = (int)reader["ReclaimValue"];
		itemTemplateInfo.ReclaimType = (int)reader["ReclaimType"];
		itemTemplateInfo.CanRecycle = (int)reader["CanRecycle"];
		itemTemplateInfo.SuitId = (int)reader["SuitId"];
		itemTemplateInfo.FloorPrice = (int)reader["FloorPrice"];
		itemTemplateInfo.IsDirty = false;
		return itemTemplateInfo;
	}

	public ItemBoxInfo[] GetItemBoxInfos()
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_ItemsBox_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitItemBoxInfo(ResultDataReader));
			}
		}
		catch (Exception ex)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init@Shop_Goods_Box：" + ex);
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

	public ItemBoxInfo InitItemBoxInfo(SqlDataReader reader)
	{
		ItemBoxInfo itemBoxInfo = new ItemBoxInfo();
		itemBoxInfo.Id = (int)reader["id"];
		itemBoxInfo.DataId = (int)reader["DataId"];
		itemBoxInfo.TemplateId = (int)reader["TemplateId"];
		itemBoxInfo.IsSelect = (bool)reader["IsSelect"];
		itemBoxInfo.IsBind = (bool)reader["IsBind"];
		itemBoxInfo.ItemValid = (int)reader["ItemValid"];
		itemBoxInfo.ItemCount = (int)reader["ItemCount"];
		itemBoxInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		itemBoxInfo.AttackCompose = (int)reader["AttackCompose"];
		itemBoxInfo.DefendCompose = (int)reader["DefendCompose"];
		itemBoxInfo.AgilityCompose = (int)reader["AgilityCompose"];
		itemBoxInfo.LuckCompose = (int)reader["LuckCompose"];
		itemBoxInfo.Random = (int)reader["Random"];
		itemBoxInfo.IsTips = (int)reader["IsTips"];
		itemBoxInfo.IsLogs = (bool)reader["IsLogs"];
		return itemBoxInfo;
	}

	public bool UpdatePlayerInfoHistory(PlayerInfoHistory info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@LastQuestsTime", info.LastQuestsTime),
				new SqlParameter("@LastTreasureTime", info.LastTreasureTime),
				new SqlParameter("@OutPut", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.Output;
			db.RunProcedure("SP_User_Update_History", array);
			result = (int)array[6].Value == 1;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("User_Update_BoxProgression", exception);
			}
		}
		return result;
	}

	public CategoryInfo[] GetAllCategory()
	{
		List<CategoryInfo> list = new List<CategoryInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Items_Category_All");
			while (ResultDataReader.Read())
			{
				CategoryInfo categoryInfo = new CategoryInfo();
				categoryInfo.ID = (int)ResultDataReader["ID"];
				categoryInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				categoryInfo.Place = (int)ResultDataReader["Place"];
				categoryInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
				list.Add(categoryInfo);
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

	public PropInfo[] GetAllProp()
	{
		List<PropInfo> list = new List<PropInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Prop_All");
			while (ResultDataReader.Read())
			{
				PropInfo propInfo = new PropInfo();
				propInfo.AffectArea = (int)ResultDataReader["AffectArea"];
				propInfo.AffectTimes = (int)ResultDataReader["AffectTimes"];
				propInfo.AttackTimes = (int)ResultDataReader["AttackTimes"];
				propInfo.BoutTimes = (int)ResultDataReader["BoutTimes"];
				propInfo.BuyGold = (int)ResultDataReader["BuyGold"];
				propInfo.BuyMoney = (int)ResultDataReader["BuyMoney"];
				propInfo.Category = (int)ResultDataReader["Category"];
				propInfo.Delay = (int)ResultDataReader["Delay"];
				propInfo.Description = ResultDataReader["Description"].ToString();
				propInfo.Icon = ResultDataReader["Icon"].ToString();
				propInfo.ID = (int)ResultDataReader["ID"];
				propInfo.Name = ResultDataReader["Name"].ToString();
				propInfo.Parameter = (int)ResultDataReader["Parameter"];
				propInfo.Pic = ResultDataReader["Pic"].ToString();
				propInfo.Property1 = (int)ResultDataReader["Property1"];
				propInfo.Property2 = (int)ResultDataReader["Property2"];
				propInfo.Property3 = (int)ResultDataReader["Property3"];
				propInfo.Random = (int)ResultDataReader["Random"];
				propInfo.Script = ResultDataReader["Script"].ToString();
				list.Add(propInfo);
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

	public BallInfo[] GetAllBall()
	{
		List<BallInfo> list = new List<BallInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Ball_All");
			while (ResultDataReader.Read())
			{
				BallInfo ballInfo = new BallInfo();
				ballInfo.Amount = (int)ResultDataReader["Amount"];
				ballInfo.ID = (int)ResultDataReader["ID"];
				ballInfo.Name = ResultDataReader["Name"].ToString();
				ballInfo.Crater = ((ResultDataReader["Crater"] == null) ? "" : ResultDataReader["Crater"].ToString());
				ballInfo.Power = (double)ResultDataReader["Power"];
				ballInfo.Radii = (int)ResultDataReader["Radii"];
				ballInfo.AttackResponse = (int)ResultDataReader["AttackResponse"];
				ballInfo.BombPartical = ResultDataReader["BombPartical"].ToString();
				ballInfo.FlyingPartical = ResultDataReader["FlyingPartical"].ToString();
				ballInfo.IsSpin = (bool)ResultDataReader["IsSpin"];
				ballInfo.Mass = (int)ResultDataReader["Mass"];
				ballInfo.SpinV = (int)ResultDataReader["SpinV"];
				ballInfo.SpinVA = (double)ResultDataReader["SpinVA"];
				ballInfo.Wind = (int)ResultDataReader["Wind"];
				ballInfo.DragIndex = (int)ResultDataReader["DragIndex"];
				ballInfo.Weight = (int)ResultDataReader["Weight"];
				ballInfo.Shake = (bool)ResultDataReader["Shake"];
				ballInfo.Delay = (int)ResultDataReader["Delay"];
				ballInfo.ShootSound = ((ResultDataReader["ShootSound"] == null) ? "" : ResultDataReader["ShootSound"].ToString());
				ballInfo.BombSound = ((ResultDataReader["BombSound"] == null) ? "" : ResultDataReader["BombSound"].ToString());
				ballInfo.ActionType = (int)ResultDataReader["ActionType"];
				ballInfo.HasTunnel = (bool)ResultDataReader["HasTunnel"];
				list.Add(ballInfo);
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

	public LuckstarActivityRankInfo[] GetAllLuckstarActivityRank()
	{
		List<LuckstarActivityRankInfo> list = new List<LuckstarActivityRankInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "[SP_Luckstar_Activity_Rank_All]");
			int num = 1;
			while (ResultDataReader.Read())
			{
				LuckstarActivityRankInfo luckstarActivityRankInfo = new LuckstarActivityRankInfo();
				luckstarActivityRankInfo.rank = num;
				luckstarActivityRankInfo.UserID = (int)ResultDataReader["UserID"];
				luckstarActivityRankInfo.useStarNum = (int)ResultDataReader["useStarNum"];
				luckstarActivityRankInfo.isVip = (int)ResultDataReader["isVip"];
				luckstarActivityRankInfo.nickName = (string)ResultDataReader["nickName"];
				list.Add(luckstarActivityRankInfo);
				num++;
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

	public BallConfigInfo[] GetAllBallConfig()
	{
		List<BallConfigInfo> list = new List<BallConfigInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "[SP_Ball_Config_All]");
			while (ResultDataReader.Read())
			{
				BallConfigInfo ballConfigInfo = new BallConfigInfo();
				ballConfigInfo.Common = (int)ResultDataReader["Common"];
				ballConfigInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				ballConfigInfo.CommonAddWound = (int)ResultDataReader["CommonAddWound"];
				ballConfigInfo.CommonMultiBall = (int)ResultDataReader["CommonMultiBall"];
				ballConfigInfo.Special = (int)ResultDataReader["Special"];
				list.Add(ballConfigInfo);
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

	public ShopItemInfo[] GetALllShop()
	{
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Shop_All");
			while (ResultDataReader.Read())
			{
				ShopItemInfo shopItemInfo = new ShopItemInfo();
				shopItemInfo.ID = int.Parse(ResultDataReader["ID"].ToString());
				shopItemInfo.ShopID = int.Parse(ResultDataReader["ShopID"].ToString());
				shopItemInfo.GroupID = int.Parse(ResultDataReader["GroupID"].ToString());
				shopItemInfo.TemplateID = int.Parse(ResultDataReader["TemplateID"].ToString());
				shopItemInfo.BuyType = int.Parse(ResultDataReader["BuyType"].ToString());
				shopItemInfo.Sort = int.Parse(ResultDataReader["Sort"].ToString());
				shopItemInfo.IsVouch = int.Parse(ResultDataReader["IsVouch"].ToString());
				shopItemInfo.Label = int.Parse(ResultDataReader["Label"].ToString());
				shopItemInfo.Beat = decimal.Parse(ResultDataReader["Beat"].ToString());
				shopItemInfo.AUnit = int.Parse(ResultDataReader["AUnit"].ToString());
				shopItemInfo.APrice1 = int.Parse(ResultDataReader["APrice1"].ToString());
				shopItemInfo.AValue1 = int.Parse(ResultDataReader["AValue1"].ToString());
				shopItemInfo.APrice2 = int.Parse(ResultDataReader["APrice2"].ToString());
				shopItemInfo.AValue2 = int.Parse(ResultDataReader["AValue2"].ToString());
				shopItemInfo.APrice3 = int.Parse(ResultDataReader["APrice3"].ToString());
				shopItemInfo.AValue3 = int.Parse(ResultDataReader["AValue3"].ToString());
				shopItemInfo.BUnit = int.Parse(ResultDataReader["BUnit"].ToString());
				shopItemInfo.BPrice1 = int.Parse(ResultDataReader["BPrice1"].ToString());
				shopItemInfo.BValue1 = int.Parse(ResultDataReader["BValue1"].ToString());
				shopItemInfo.BPrice2 = int.Parse(ResultDataReader["BPrice2"].ToString());
				shopItemInfo.BValue2 = int.Parse(ResultDataReader["BValue2"].ToString());
				shopItemInfo.BPrice3 = int.Parse(ResultDataReader["BPrice3"].ToString());
				shopItemInfo.BValue3 = int.Parse(ResultDataReader["BValue3"].ToString());
				shopItemInfo.CUnit = int.Parse(ResultDataReader["CUnit"].ToString());
				shopItemInfo.CPrice1 = int.Parse(ResultDataReader["CPrice1"].ToString());
				shopItemInfo.CValue1 = int.Parse(ResultDataReader["CValue1"].ToString());
				shopItemInfo.CPrice2 = int.Parse(ResultDataReader["CPrice2"].ToString());
				shopItemInfo.CValue2 = int.Parse(ResultDataReader["CValue2"].ToString());
				shopItemInfo.CPrice3 = int.Parse(ResultDataReader["CPrice3"].ToString());
				shopItemInfo.CValue3 = int.Parse(ResultDataReader["CValue3"].ToString());
				shopItemInfo.IsContinue = bool.Parse(ResultDataReader["IsContinue"].ToString());
				shopItemInfo.IsCheap = bool.Parse(ResultDataReader["IsCheap"].ToString());
				shopItemInfo.LimitCount = int.Parse(ResultDataReader["LimitCount"].ToString());
				shopItemInfo.StartDate = DateTime.Parse(ResultDataReader["StartDate"].ToString());
				shopItemInfo.EndDate = DateTime.Parse(ResultDataReader["EndDate"].ToString());
				list.Add(shopItemInfo);
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

	public FusionInfo[] GetAllFusionDesc()
	{
		List<FusionInfo> list = new List<FusionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Fusion_All_Desc");
			while (ResultDataReader.Read())
			{
				FusionInfo fusionInfo = new FusionInfo();
				fusionInfo.FusionID = (int)ResultDataReader["FusionID"];
				fusionInfo.Item1 = (int)ResultDataReader["Item1"];
				fusionInfo.Item2 = (int)ResultDataReader["Item2"];
				fusionInfo.Item3 = (int)ResultDataReader["Item3"];
				fusionInfo.Item4 = (int)ResultDataReader["Item4"];
				fusionInfo.Formula = (int)ResultDataReader["Formula"];
				fusionInfo.Reward = (int)ResultDataReader["Reward"];
				list.Add(fusionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllFusion", exception);
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

	public FusionInfo[] GetAllFusion()
	{
		List<FusionInfo> list = new List<FusionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Fusion_All");
			while (ResultDataReader.Read())
			{
				FusionInfo fusionInfo = new FusionInfo();
				fusionInfo.FusionID = (int)ResultDataReader["FusionID"];
				fusionInfo.Item1 = (int)ResultDataReader["Item1"];
				fusionInfo.Item2 = (int)ResultDataReader["Item2"];
				fusionInfo.Item3 = (int)ResultDataReader["Item3"];
				fusionInfo.Item4 = (int)ResultDataReader["Item4"];
				fusionInfo.Count1 = (int)ResultDataReader["Count1"];
				fusionInfo.Count2 = (int)ResultDataReader["Count2"];
				fusionInfo.Count3 = (int)ResultDataReader["Count3"];
				fusionInfo.Count4 = (int)ResultDataReader["Count4"];
				fusionInfo.Formula = (int)ResultDataReader["Formula"];
				fusionInfo.FusionRate = (int)ResultDataReader["FusionRate"];
				fusionInfo.FusionType = (int)ResultDataReader["FusionType"];
				fusionInfo.Reward = (int)ResultDataReader["Reward"];
				list.Add(fusionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllFusion", exception);
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

	public StrengthenInfo[] GetAllStrengthen()
	{
		List<StrengthenInfo> list = new List<StrengthenInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Strengthen_All");
			while (ResultDataReader.Read())
			{
				StrengthenInfo strengthenInfo = new StrengthenInfo();
				strengthenInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				strengthenInfo.Random = (int)ResultDataReader["Random"];
				strengthenInfo.Rock = (int)ResultDataReader["Rock"];
				strengthenInfo.Rock1 = (int)ResultDataReader["Rock1"];
				strengthenInfo.Rock2 = (int)ResultDataReader["Rock2"];
				strengthenInfo.Rock3 = (int)ResultDataReader["Rock3"];
				strengthenInfo.StoneLevelMin = (int)ResultDataReader["StoneLevelMin"];
				list.Add(strengthenInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllStrengthen", exception);
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

	public RuneTemplateInfo[] GetAllRuneTemplate()
	{
		List<RuneTemplateInfo> list = new List<RuneTemplateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_RuneTemplate_All");
			while (ResultDataReader.Read())
			{
				RuneTemplateInfo runeTemplateInfo = new RuneTemplateInfo();
				runeTemplateInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				runeTemplateInfo.NextTemplateID = (int)ResultDataReader["NextTemplateID"];
				runeTemplateInfo.Name = (string)ResultDataReader["Name"];
				runeTemplateInfo.BaseLevel = (int)ResultDataReader["BaseLevel"];
				runeTemplateInfo.MaxLevel = (int)ResultDataReader["MaxLevel"];
				runeTemplateInfo.Type1 = (int)ResultDataReader["Type1"];
				runeTemplateInfo.Attribute1 = (string)ResultDataReader["Attribute1"];
				runeTemplateInfo.Turn1 = (int)ResultDataReader["Turn1"];
				runeTemplateInfo.Rate1 = (int)ResultDataReader["Rate1"];
				runeTemplateInfo.Type2 = (int)ResultDataReader["Type2"];
				runeTemplateInfo.Attribute2 = (string)ResultDataReader["Attribute2"];
				runeTemplateInfo.Turn2 = (int)ResultDataReader["Turn2"];
				runeTemplateInfo.Rate2 = (int)ResultDataReader["Rate2"];
				runeTemplateInfo.Type3 = (int)ResultDataReader["Type3"];
				runeTemplateInfo.Attribute3 = (string)ResultDataReader["Attribute3"];
				runeTemplateInfo.Turn3 = (int)ResultDataReader["Turn3"];
				runeTemplateInfo.Rate3 = (int)ResultDataReader["Rate3"];
				list.Add(runeTemplateInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetRuneTemplateInfo", exception);
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

	public StrengThenExpInfo[] GetAllStrengThenExp()
	{
		List<StrengThenExpInfo> list = new List<StrengThenExpInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_StrengThenExp_All");
			while (ResultDataReader.Read())
			{
				StrengThenExpInfo strengThenExpInfo = new StrengThenExpInfo();
				strengThenExpInfo.ID = (int)ResultDataReader["ID"];
				strengThenExpInfo.Level = (int)ResultDataReader["Level"];
				strengThenExpInfo.Exp = (int)ResultDataReader["Exp"];
				strengThenExpInfo.NecklaceStrengthExp = (int)ResultDataReader["NecklaceStrengthExp"];
				strengThenExpInfo.NecklaceStrengthPlus = (int)ResultDataReader["NecklaceStrengthPlus"];
				list.Add(strengThenExpInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetStrengThenExpInfo", exception);
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

	public StrengthenGoodsInfo[] GetAllStrengthenGoodsInfo()
	{
		List<StrengthenGoodsInfo> list = new List<StrengthenGoodsInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_StrengthenGoodsInfo_All");
			while (ResultDataReader.Read())
			{
				StrengthenGoodsInfo strengthenGoodsInfo = new StrengthenGoodsInfo();
				strengthenGoodsInfo.ID = (int)ResultDataReader["ID"];
				strengthenGoodsInfo.Level = (int)ResultDataReader["Level"];
				strengthenGoodsInfo.CurrentEquip = (int)ResultDataReader["CurrentEquip"];
				strengthenGoodsInfo.GainEquip = (int)ResultDataReader["GainEquip"];
				strengthenGoodsInfo.OrginEquip = (int)ResultDataReader["OrginEquip"];
				list.Add(strengthenGoodsInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllStrengthenGoodsInfo", exception);
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

	public StrengthenInfo[] GetAllRefineryStrengthen()
	{
		List<StrengthenInfo> list = new List<StrengthenInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Refinery_Strengthen_All");
			while (ResultDataReader.Read())
			{
				StrengthenInfo strengthenInfo = new StrengthenInfo();
				strengthenInfo.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				strengthenInfo.Rock = (int)ResultDataReader["Rock"];
				list.Add(strengthenInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllRefineryStrengthen", exception);
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

	public List<RefineryInfo> GetAllRefineryInfo()
	{
		List<RefineryInfo> list = new List<RefineryInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Item_Refinery_All");
			while (ResultDataReader.Read())
			{
				RefineryInfo refineryInfo = new RefineryInfo();
				refineryInfo.RefineryID = (int)ResultDataReader["RefineryID"];
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip1"]);
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip2"]);
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip3"]);
				refineryInfo.m_Equip.Add((int)ResultDataReader["Equip4"]);
				refineryInfo.Item1 = (int)ResultDataReader["Item1"];
				refineryInfo.Item2 = (int)ResultDataReader["Item2"];
				refineryInfo.Item3 = (int)ResultDataReader["Item3"];
				refineryInfo.Item1Count = (int)ResultDataReader["Item1Count"];
				refineryInfo.Item2Count = (int)ResultDataReader["Item2Count"];
				refineryInfo.Item3Count = (int)ResultDataReader["Item3Count"];
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material1"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate1"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward1"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material2"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate2"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward2"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material3"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate3"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward3"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Material4"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Operate4"]);
				refineryInfo.m_Reward.Add((int)ResultDataReader["Reward4"]);
				list.Add(refineryInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllRefineryInfo", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return list;
	}

	public QuestInfo[] GetALlQuest()
	{
		List<QuestInfo> list = new List<QuestInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuest(ResultDataReader));
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

	public QuestAwardInfo[] GetAllQuestGoods()
	{
		List<QuestAwardInfo> list = new List<QuestAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_Goods_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuestGoods(ResultDataReader));
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

	public QuestConditionInfo[] GetAllQuestCondiction()
	{
		List<QuestConditionInfo> list = new List<QuestConditionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_Condiction_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuestCondiction(ResultDataReader));
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

	public QuestRateInfo[] GetAllQuestRate()
	{
		List<QuestRateInfo> list = new List<QuestRateInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Quest_Rate_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitQuestRate(ResultDataReader));
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

	public QuestInfo GetSingleQuest(int questID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@QuestID", SqlDbType.Int, 4)
			};
			array[0].Value = questID;
			db.GetReader(ref ResultDataReader, "SP_Quest_Single", array);
			if (ResultDataReader.Read())
			{
				return InitQuest(ResultDataReader);
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

	public QuestInfo InitQuest(SqlDataReader reader)
	{
		QuestInfo questInfo = new QuestInfo();
		questInfo.ID = (int)reader["ID"];
		questInfo.QuestID = (int)reader["QuestID"];
		questInfo.Title = ((reader["Title"] == null) ? "" : reader["Title"].ToString());
		questInfo.Detail = ((reader["Detail"] == null) ? "" : reader["Detail"].ToString());
		questInfo.Objective = ((reader["Objective"] == null) ? "" : reader["Objective"].ToString());
		questInfo.NeedMinLevel = (int)reader["NeedMinLevel"];
		questInfo.NeedMaxLevel = (int)reader["NeedMaxLevel"];
		questInfo.PreQuestID = ((reader["PreQuestID"] == null) ? "" : reader["PreQuestID"].ToString());
		questInfo.NextQuestID = ((reader["NextQuestID"] == null) ? "" : reader["NextQuestID"].ToString());
		questInfo.IsOther = (int)reader["IsOther"];
		questInfo.CanRepeat = (bool)reader["CanRepeat"];
		questInfo.RepeatInterval = (int)reader["RepeatInterval"];
		questInfo.RepeatMax = (int)reader["RepeatMax"];
		questInfo.RewardGP = (int)reader["RewardGP"];
		questInfo.RewardGold = (int)reader["RewardGold"];
		questInfo.RewardBindMoney = (int)reader["RewardBindMoney"];
		questInfo.RewardOffer = (int)reader["RewardOffer"];
		questInfo.RewardRiches = (int)reader["RewardRiches"];
		questInfo.RewardBuffID = (int)reader["RewardBuffID"];
		questInfo.RewardBuffDate = (int)reader["RewardBuffDate"];
		questInfo.RewardMoney = (int)reader["RewardMoney"];
		questInfo.Rands = (decimal)reader["Rands"];
		questInfo.RandDouble = (int)reader["RandDouble"];
		questInfo.TimeMode = (bool)reader["TimeMode"];
		questInfo.StartDate = (DateTime)reader["StartDate"];
		questInfo.EndDate = (DateTime)reader["EndDate"];
		questInfo.MapID = (int)reader["MapID"];
		questInfo.AutoEquip = (bool)reader["AutoEquip"];
		questInfo.OneKeyFinishNeedMoney = (int)reader["OneKeyFinishNeedMoney"];
		questInfo.Rank = ((reader["Rank"] == null) ? "" : reader["Rank"].ToString());
		questInfo.StarLev = (int)reader["StarLev"];
		questInfo.NotMustCount = (int)reader["NotMustCount"];
		questInfo.Level2NeedMoney = (int)reader["Level2NeedMoney"];
		questInfo.Level3NeedMoney = (int)reader["Level3NeedMoney"];
		questInfo.Level4NeedMoney = (int)reader["Level4NeedMoney"];
		questInfo.Level5NeedMoney = (int)reader["Level5NeedMoney"];
		questInfo.CollocationCost = (int)reader["CollocationCost"];
		questInfo.CollocationColdTime = (int)reader["CollocationColdTime"];
		return questInfo;
	}

	public QuestAwardInfo InitQuestGoods(SqlDataReader reader)
	{
		QuestAwardInfo questAwardInfo = new QuestAwardInfo();
		questAwardInfo.QuestID = (int)reader["QuestID"];
		questAwardInfo.RewardItemID = (int)reader["RewardItemID"];
		questAwardInfo.IsSelect = (bool)reader["IsSelect"];
		questAwardInfo.RewardItemValid = (int)reader["RewardItemValid"];
		questAwardInfo.RewardItemCount1 = (int)reader["RewardItemCount1"];
		questAwardInfo.RewardItemCount2 = (int)reader["RewardItemCount2"];
		questAwardInfo.RewardItemCount3 = (int)reader["RewardItemCount3"];
		questAwardInfo.RewardItemCount4 = (int)reader["RewardItemCount4"];
		questAwardInfo.RewardItemCount5 = (int)reader["RewardItemCount5"];
		questAwardInfo.StrengthenLevel = (int)reader["StrengthenLevel"];
		questAwardInfo.AttackCompose = (int)reader["AttackCompose"];
		questAwardInfo.DefendCompose = (int)reader["DefendCompose"];
		questAwardInfo.AgilityCompose = (int)reader["AgilityCompose"];
		questAwardInfo.LuckCompose = (int)reader["LuckCompose"];
		questAwardInfo.IsCount = (bool)reader["IsCount"];
		questAwardInfo.IsBind = (bool)reader["IsBind"];
		return questAwardInfo;
	}

	public QuestConditionInfo InitQuestCondiction(SqlDataReader reader)
	{
		QuestConditionInfo questConditionInfo = new QuestConditionInfo();
		questConditionInfo.QuestID = (int)reader["QuestID"];
		questConditionInfo.CondictionID = (int)reader["CondictionID"];
		questConditionInfo.CondictionTitle = ((reader["CondictionTitle"] == null) ? "" : reader["CondictionTitle"].ToString());
		questConditionInfo.CondictionType = (int)reader["CondictionType"];
		questConditionInfo.Para1 = (int)reader["Para1"];
		questConditionInfo.Para2 = (int)reader["Para2"];
		questConditionInfo.isOpitional = (bool)reader["isOpitional"];
		return questConditionInfo;
	}

	public QuestRateInfo InitQuestRate(SqlDataReader reader)
	{
		QuestRateInfo questRateInfo = new QuestRateInfo();
		questRateInfo.BindMoneyRate = ((reader["BindMoneyRate"] == null) ? "" : reader["BindMoneyRate"].ToString());
		questRateInfo.ExpRate = ((reader["ExpRate"] == null) ? "" : reader["ExpRate"].ToString());
		questRateInfo.GoldRate = ((reader["GoldRate"] == null) ? "" : reader["GoldRate"].ToString());
		questRateInfo.ExploitRate = ((reader["ExploitRate"] == null) ? "" : reader["ExploitRate"].ToString());
		questRateInfo.CanOneKeyFinishTime = (int)reader["CanOneKeyFinishTime"];
		return questRateInfo;
	}

	public DropCondiction[] GetAllDropCondictions()
	{
		List<DropCondiction> list = new List<DropCondiction>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Drop_Condiction_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitDropCondiction(ResultDataReader));
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

	public DropItem[] GetAllDropItems()
	{
		List<DropItem> list = new List<DropItem>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Drop_Item_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitDropItem(ResultDataReader));
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

	public DropCondiction InitDropCondiction(SqlDataReader reader)
	{
		DropCondiction dropCondiction = new DropCondiction();
		dropCondiction.DropId = (int)reader["DropID"];
		dropCondiction.CondictionType = (int)reader["CondictionType"];
		dropCondiction.Para1 = (string)reader["Para1"];
		dropCondiction.Para2 = (string)reader["Para2"];
		return dropCondiction;
	}

	public DropItem InitDropItem(SqlDataReader reader)
	{
		DropItem dropItem = new DropItem();
		dropItem.Id = (int)reader["Id"];
		dropItem.DropId = (int)reader["DropId"];
		dropItem.ItemId = (int)reader["ItemId"];
		dropItem.ValueDate = (int)reader["ValueDate"];
		dropItem.IsBind = (bool)reader["IsBind"];
		dropItem.Random = (int)reader["Random"];
		dropItem.BeginData = (int)reader["BeginData"];
		dropItem.EndData = (int)reader["EndData"];
		dropItem.IsLogs = (bool)reader["IsLogs"];
		dropItem.IsTips = (bool)reader["IsTips"];
		return dropItem;
	}

	public AASInfo[] GetAllAASInfo()
	{
		List<AASInfo> list = new List<AASInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_AASInfo_All");
			while (ResultDataReader.Read())
			{
				AASInfo aASInfo = new AASInfo();
				aASInfo.UserID = (int)ResultDataReader["ID"];
				aASInfo.Name = ResultDataReader["Name"].ToString();
				aASInfo.IDNumber = ResultDataReader["IDNumber"].ToString();
				aASInfo.State = (int)ResultDataReader["State"];
				list.Add(aASInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllAASInfo", exception);
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

	public bool AddAASInfo(AASInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@Name", info.Name),
				new SqlParameter("@IDNumber", info.IDNumber),
				new SqlParameter("@State", info.State),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_ASSInfo_Add", array);
			result = (int)array[4].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateAASInfo", exception);
			}
		}
		return result;
	}

	public string GetASSInfoSingle(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_ASSInfo_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				return ResultDataReader["IDNumber"].ToString();
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetASSInfoSingle", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return "";
	}

	public bool AddDailyLogList(DailyLogListInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@UserAwardLog", info.UserAwardLog),
				new SqlParameter("@DayLog", info.DayLog),
				new SqlParameter("@Result", SqlDbType.Int),
				null
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_DailyLogList_Add", array);
			result = (int)array[3].Value == 0;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("UpdateAASInfo", exception);
			}
		}
		return result;
	}

	public bool UpdateDailyLogList(DailyLogListInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[5]
			{
				new SqlParameter("@UserID", info.UserID),
				new SqlParameter("@UserAwardLog", info.UserAwardLog),
				new SqlParameter("@DayLog", info.DayLog),
				new SqlParameter("@LastDate", info.LastDate.ToString()),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[4].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_DailyLogList_Update", array);
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("User_Update_BoxProgression", exception);
			}
		}
		return result;
	}

	public DailyLogListInfo GetDailyLogListSingle(int UserID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@UserID", UserID)
			};
			db.GetReader(ref ResultDataReader, "SP_DailyLogList_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				DailyLogListInfo dailyLogListInfo = new DailyLogListInfo();
				dailyLogListInfo.ID = (int)ResultDataReader["ID"];
				dailyLogListInfo.UserID = (int)ResultDataReader["UserID"];
				dailyLogListInfo.UserAwardLog = (int)ResultDataReader["UserAwardLog"];
				dailyLogListInfo.DayLog = (string)ResultDataReader["DayLog"];
				dailyLogListInfo.LastDate = (DateTime)ResultDataReader["LastDate"];
				return dailyLogListInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("DailyLogList", exception);
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

	public DailyAwardInfo[] GetAllDailyAward()
	{
		List<DailyAwardInfo> list = new List<DailyAwardInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Daily_Award_All");
			while (ResultDataReader.Read())
			{
				DailyAwardInfo dailyAwardInfo = new DailyAwardInfo();
				dailyAwardInfo.Count = (int)ResultDataReader["Count"];
				dailyAwardInfo.ID = (int)ResultDataReader["ID"];
				dailyAwardInfo.IsBinds = (bool)ResultDataReader["IsBinds"];
				dailyAwardInfo.TemplateID = (int)ResultDataReader["TemplateID"];
				dailyAwardInfo.Type = (int)ResultDataReader["Type"];
				dailyAwardInfo.ValidDate = (int)ResultDataReader["ValidDate"];
				dailyAwardInfo.Sex = (int)ResultDataReader["Sex"];
				dailyAwardInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
				dailyAwardInfo.CountRemark = ((ResultDataReader["CountRemark"] == null) ? "" : ResultDataReader["CountRemark"].ToString());
				dailyAwardInfo.GetWay = (int)ResultDataReader["GetWay"];
				dailyAwardInfo.AwardDays = (int)ResultDataReader["AwardDays"];
				list.Add(dailyAwardInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllDaily", exception);
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

	public NpcInfo[] GetAllNPCInfo()
	{
		List<NpcInfo> list = new List<NpcInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_NPC_Info_All");
			while (ResultDataReader.Read())
			{
				NpcInfo npcInfo = new NpcInfo();
				npcInfo.ID = (int)ResultDataReader["ID"];
				npcInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				npcInfo.Level = (int)ResultDataReader["Level"];
				npcInfo.Camp = (int)ResultDataReader["Camp"];
				npcInfo.Type = (int)ResultDataReader["Type"];
				npcInfo.Blood = (int)ResultDataReader["Blood"];
				npcInfo.X = (int)ResultDataReader["X"];
				npcInfo.Y = (int)ResultDataReader["Y"];
				npcInfo.Width = (int)ResultDataReader["Width"];
				npcInfo.Height = (int)ResultDataReader["Height"];
				npcInfo.MoveMin = (int)ResultDataReader["MoveMin"];
				npcInfo.MoveMax = (int)ResultDataReader["MoveMax"];
				npcInfo.BaseDamage = (int)ResultDataReader["BaseDamage"];
				npcInfo.BaseGuard = (int)ResultDataReader["BaseGuard"];
				npcInfo.Attack = (int)ResultDataReader["Attack"];
				npcInfo.Defence = (int)ResultDataReader["Defence"];
				npcInfo.Agility = (int)ResultDataReader["Agility"];
				npcInfo.Lucky = (int)ResultDataReader["Lucky"];
				npcInfo.ModelID = ((ResultDataReader["ModelID"] == null) ? "" : ResultDataReader["ModelID"].ToString());
				npcInfo.ResourcesPath = ((ResultDataReader["ResourcesPath"] == null) ? "" : ResultDataReader["ResourcesPath"].ToString());
				npcInfo.DropRate = ((ResultDataReader["DropRate"] == null) ? "" : ResultDataReader["DropRate"].ToString());
				npcInfo.Experience = (int)ResultDataReader["Experience"];
				npcInfo.Delay = (int)ResultDataReader["Delay"];
				npcInfo.Immunity = (int)ResultDataReader["Immunity"];
				npcInfo.Alert = (int)ResultDataReader["Alert"];
				npcInfo.Range = (int)ResultDataReader["Range"];
				npcInfo.Preserve = (int)ResultDataReader["Preserve"];
				npcInfo.Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString());
				npcInfo.FireX = (int)ResultDataReader["FireX"];
				npcInfo.FireY = (int)ResultDataReader["FireY"];
				npcInfo.DropId = (int)ResultDataReader["DropId"];
				npcInfo.CurrentBallId = (int)ResultDataReader["CurrentBallId"];
				npcInfo.speed = (int)ResultDataReader["speed"];
				list.Add(npcInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllNPCInfo", exception);
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

	public MissionInfo[] GetAllMissionInfo()
	{
		List<MissionInfo> list = new List<MissionInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Mission_Info_All");
			while (ResultDataReader.Read())
			{
				MissionInfo missionInfo = new MissionInfo();
				missionInfo.Id = (int)ResultDataReader["ID"];
				missionInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				missionInfo.TotalCount = (int)ResultDataReader["TotalCount"];
				missionInfo.TotalTurn = (int)ResultDataReader["TotalTurn"];
				missionInfo.Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString());
				missionInfo.Success = ((ResultDataReader["Success"] == null) ? "" : ResultDataReader["Success"].ToString());
				missionInfo.Failure = ((ResultDataReader["Failure"] == null) ? "" : ResultDataReader["Failure"].ToString());
				missionInfo.Description = ((ResultDataReader["Description"] == null) ? "" : ResultDataReader["Description"].ToString());
				missionInfo.IncrementDelay = (int)ResultDataReader["IncrementDelay"];
				missionInfo.Delay = (int)ResultDataReader["Delay"];
				missionInfo.Title = ((ResultDataReader["Title"] == null) ? "" : ResultDataReader["Title"].ToString());
				missionInfo.Param1 = (int)ResultDataReader["Param1"];
				missionInfo.Param2 = (int)ResultDataReader["Param2"];
				list.Add(missionInfo);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllMissionInfo", exception);
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
}
