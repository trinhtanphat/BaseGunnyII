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
				list.Add(new LoadUserBoxInfo
				{
					ID = (int)ResultDataReader["ID"],
					Type = (int)ResultDataReader["Type"],
					Level = (int)ResultDataReader["Level"],
					Condition = (int)ResultDataReader["Condition"],
					TemplateID = (int)ResultDataReader["TemplateID"]
				});
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
				list.Add(new CategoryInfo
				{
					ID = (int)ResultDataReader["ID"],
					Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString()),
					Place = (int)ResultDataReader["Place"],
					Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString())
				});
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
				list.Add(new PropInfo
				{
					AffectArea = (int)ResultDataReader["AffectArea"],
					AffectTimes = (int)ResultDataReader["AffectTimes"],
					AttackTimes = (int)ResultDataReader["AttackTimes"],
					BoutTimes = (int)ResultDataReader["BoutTimes"],
					BuyGold = (int)ResultDataReader["BuyGold"],
					BuyMoney = (int)ResultDataReader["BuyMoney"],
					Category = (int)ResultDataReader["Category"],
					Delay = (int)ResultDataReader["Delay"],
					Description = ResultDataReader["Description"].ToString(),
					Icon = ResultDataReader["Icon"].ToString(),
					ID = (int)ResultDataReader["ID"],
					Name = ResultDataReader["Name"].ToString(),
					Parameter = (int)ResultDataReader["Parameter"],
					Pic = ResultDataReader["Pic"].ToString(),
					Property1 = (int)ResultDataReader["Property1"],
					Property2 = (int)ResultDataReader["Property2"],
					Property3 = (int)ResultDataReader["Property3"],
					Random = (int)ResultDataReader["Random"],
					Script = ResultDataReader["Script"].ToString()
				});
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
				list.Add(new BallInfo
				{
					Amount = (int)ResultDataReader["Amount"],
					ID = (int)ResultDataReader["ID"],
					Name = ResultDataReader["Name"].ToString(),
					Crater = ((ResultDataReader["Crater"] == null) ? "" : ResultDataReader["Crater"].ToString()),
					Power = (double)ResultDataReader["Power"],
					Radii = (int)ResultDataReader["Radii"],
					AttackResponse = (int)ResultDataReader["AttackResponse"],
					BombPartical = ResultDataReader["BombPartical"].ToString(),
					FlyingPartical = ResultDataReader["FlyingPartical"].ToString(),
					IsSpin = (bool)ResultDataReader["IsSpin"],
					Mass = (int)ResultDataReader["Mass"],
					SpinV = (int)ResultDataReader["SpinV"],
					SpinVA = (double)ResultDataReader["SpinVA"],
					Wind = (int)ResultDataReader["Wind"],
					DragIndex = (int)ResultDataReader["DragIndex"],
					Weight = (int)ResultDataReader["Weight"],
					Shake = (bool)ResultDataReader["Shake"],
					Delay = (int)ResultDataReader["Delay"],
					ShootSound = ((ResultDataReader["ShootSound"] == null) ? "" : ResultDataReader["ShootSound"].ToString()),
					BombSound = ((ResultDataReader["BombSound"] == null) ? "" : ResultDataReader["BombSound"].ToString()),
					ActionType = (int)ResultDataReader["ActionType"],
					HasTunnel = (bool)ResultDataReader["HasTunnel"]
				});
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
				list.Add(new LuckstarActivityRankInfo
				{
					rank = num,
					UserID = (int)ResultDataReader["UserID"],
					useStarNum = (int)ResultDataReader["useStarNum"],
					isVip = (int)ResultDataReader["isVip"],
					nickName = (string)ResultDataReader["nickName"]
				});
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
				list.Add(new BallConfigInfo
				{
					Common = (int)ResultDataReader["Common"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					CommonAddWound = (int)ResultDataReader["CommonAddWound"],
					CommonMultiBall = (int)ResultDataReader["CommonMultiBall"],
					Special = (int)ResultDataReader["Special"]
				});
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
				list.Add(new ShopItemInfo
				{
					ID = int.Parse(ResultDataReader["ID"].ToString()),
					ShopID = int.Parse(ResultDataReader["ShopID"].ToString()),
					GroupID = int.Parse(ResultDataReader["GroupID"].ToString()),
					TemplateID = int.Parse(ResultDataReader["TemplateID"].ToString()),
					BuyType = int.Parse(ResultDataReader["BuyType"].ToString()),
					Sort = int.Parse(ResultDataReader["Sort"].ToString()),
					IsVouch = int.Parse(ResultDataReader["IsVouch"].ToString()),
					Label = int.Parse(ResultDataReader["Label"].ToString()),
					Beat = decimal.Parse(ResultDataReader["Beat"].ToString()),
					AUnit = int.Parse(ResultDataReader["AUnit"].ToString()),
					APrice1 = int.Parse(ResultDataReader["APrice1"].ToString()),
					AValue1 = int.Parse(ResultDataReader["AValue1"].ToString()),
					APrice2 = int.Parse(ResultDataReader["APrice2"].ToString()),
					AValue2 = int.Parse(ResultDataReader["AValue2"].ToString()),
					APrice3 = int.Parse(ResultDataReader["APrice3"].ToString()),
					AValue3 = int.Parse(ResultDataReader["AValue3"].ToString()),
					BUnit = int.Parse(ResultDataReader["BUnit"].ToString()),
					BPrice1 = int.Parse(ResultDataReader["BPrice1"].ToString()),
					BValue1 = int.Parse(ResultDataReader["BValue1"].ToString()),
					BPrice2 = int.Parse(ResultDataReader["BPrice2"].ToString()),
					BValue2 = int.Parse(ResultDataReader["BValue2"].ToString()),
					BPrice3 = int.Parse(ResultDataReader["BPrice3"].ToString()),
					BValue3 = int.Parse(ResultDataReader["BValue3"].ToString()),
					CUnit = int.Parse(ResultDataReader["CUnit"].ToString()),
					CPrice1 = int.Parse(ResultDataReader["CPrice1"].ToString()),
					CValue1 = int.Parse(ResultDataReader["CValue1"].ToString()),
					CPrice2 = int.Parse(ResultDataReader["CPrice2"].ToString()),
					CValue2 = int.Parse(ResultDataReader["CValue2"].ToString()),
					CPrice3 = int.Parse(ResultDataReader["CPrice3"].ToString()),
					CValue3 = int.Parse(ResultDataReader["CValue3"].ToString()),
					IsContinue = bool.Parse(ResultDataReader["IsContinue"].ToString()),
					IsCheap = bool.Parse(ResultDataReader["IsCheap"].ToString()),
					LimitCount = int.Parse(ResultDataReader["LimitCount"].ToString()),
					StartDate = DateTime.Parse(ResultDataReader["StartDate"].ToString()),
					EndDate = DateTime.Parse(ResultDataReader["EndDate"].ToString())
				});
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
				list.Add(new FusionInfo
				{
					FusionID = (int)ResultDataReader["FusionID"],
					Item1 = (int)ResultDataReader["Item1"],
					Item2 = (int)ResultDataReader["Item2"],
					Item3 = (int)ResultDataReader["Item3"],
					Item4 = (int)ResultDataReader["Item4"],
					Formula = (int)ResultDataReader["Formula"],
					Reward = (int)ResultDataReader["Reward"]
				});
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
				list.Add(new FusionInfo
				{
					FusionID = (int)ResultDataReader["FusionID"],
					Item1 = (int)ResultDataReader["Item1"],
					Item2 = (int)ResultDataReader["Item2"],
					Item3 = (int)ResultDataReader["Item3"],
					Item4 = (int)ResultDataReader["Item4"],
					Count1 = (int)ResultDataReader["Count1"],
					Count2 = (int)ResultDataReader["Count2"],
					Count3 = (int)ResultDataReader["Count3"],
					Count4 = (int)ResultDataReader["Count4"],
					Formula = (int)ResultDataReader["Formula"],
					FusionRate = (int)ResultDataReader["FusionRate"],
					FusionType = (int)ResultDataReader["FusionType"],
					Reward = (int)ResultDataReader["Reward"]
				});
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
				list.Add(new StrengthenInfo
				{
					StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
					Random = (int)ResultDataReader["Random"],
					Rock = (int)ResultDataReader["Rock"],
					Rock1 = (int)ResultDataReader["Rock1"],
					Rock2 = (int)ResultDataReader["Rock2"],
					Rock3 = (int)ResultDataReader["Rock3"],
					StoneLevelMin = (int)ResultDataReader["StoneLevelMin"]
				});
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
				list.Add(new RuneTemplateInfo
				{
					TemplateID = (int)ResultDataReader["TemplateID"],
					NextTemplateID = (int)ResultDataReader["NextTemplateID"],
					Name = (string)ResultDataReader["Name"],
					BaseLevel = (int)ResultDataReader["BaseLevel"],
					MaxLevel = (int)ResultDataReader["MaxLevel"],
					Type1 = (int)ResultDataReader["Type1"],
					Attribute1 = (string)ResultDataReader["Attribute1"],
					Turn1 = (int)ResultDataReader["Turn1"],
					Rate1 = (int)ResultDataReader["Rate1"],
					Type2 = (int)ResultDataReader["Type2"],
					Attribute2 = (string)ResultDataReader["Attribute2"],
					Turn2 = (int)ResultDataReader["Turn2"],
					Rate2 = (int)ResultDataReader["Rate2"],
					Type3 = (int)ResultDataReader["Type3"],
					Attribute3 = (string)ResultDataReader["Attribute3"],
					Turn3 = (int)ResultDataReader["Turn3"],
					Rate3 = (int)ResultDataReader["Rate3"]
				});
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
				list.Add(new StrengThenExpInfo
				{
					ID = (int)ResultDataReader["ID"],
					Level = (int)ResultDataReader["Level"],
					Exp = (int)ResultDataReader["Exp"],
					NecklaceStrengthExp = (int)ResultDataReader["NecklaceStrengthExp"],
					NecklaceStrengthPlus = (int)ResultDataReader["NecklaceStrengthPlus"]
				});
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
				list.Add(new StrengthenGoodsInfo
				{
					ID = (int)ResultDataReader["ID"],
					Level = (int)ResultDataReader["Level"],
					CurrentEquip = (int)ResultDataReader["CurrentEquip"],
					GainEquip = (int)ResultDataReader["GainEquip"],
					OrginEquip = (int)ResultDataReader["OrginEquip"]
				});
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
				list.Add(new StrengthenInfo
				{
					StrengthenLevel = (int)ResultDataReader["StrengthenLevel"],
					Rock = (int)ResultDataReader["Rock"]
				});
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
				list.Add(new RefineryInfo
				{
					RefineryID = (int)ResultDataReader["RefineryID"],
					m_Equip =
					{
						(int)ResultDataReader["Equip1"],
						(int)ResultDataReader["Equip2"],
						(int)ResultDataReader["Equip3"],
						(int)ResultDataReader["Equip4"]
					},
					Item1 = (int)ResultDataReader["Item1"],
					Item2 = (int)ResultDataReader["Item2"],
					Item3 = (int)ResultDataReader["Item3"],
					Item1Count = (int)ResultDataReader["Item1Count"],
					Item2Count = (int)ResultDataReader["Item2Count"],
					Item3Count = (int)ResultDataReader["Item3Count"],
					m_Reward =
					{
						(int)ResultDataReader["Material1"],
						(int)ResultDataReader["Operate1"],
						(int)ResultDataReader["Reward1"],
						(int)ResultDataReader["Material2"],
						(int)ResultDataReader["Operate2"],
						(int)ResultDataReader["Reward2"],
						(int)ResultDataReader["Material3"],
						(int)ResultDataReader["Operate3"],
						(int)ResultDataReader["Reward3"],
						(int)ResultDataReader["Material4"],
						(int)ResultDataReader["Operate4"],
						(int)ResultDataReader["Reward4"]
					}
				});
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
				list.Add(new AASInfo
				{
					UserID = (int)ResultDataReader["ID"],
					Name = ResultDataReader["Name"].ToString(),
					IDNumber = ResultDataReader["IDNumber"].ToString(),
					State = (int)ResultDataReader["State"]
				});
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
				list.Add(new DailyAwardInfo
				{
					Count = (int)ResultDataReader["Count"],
					ID = (int)ResultDataReader["ID"],
					IsBinds = (bool)ResultDataReader["IsBinds"],
					TemplateID = (int)ResultDataReader["TemplateID"],
					Type = (int)ResultDataReader["Type"],
					ValidDate = (int)ResultDataReader["ValidDate"],
					Sex = (int)ResultDataReader["Sex"],
					Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString()),
					CountRemark = ((ResultDataReader["CountRemark"] == null) ? "" : ResultDataReader["CountRemark"].ToString()),
					GetWay = (int)ResultDataReader["GetWay"],
					AwardDays = (int)ResultDataReader["AwardDays"]
				});
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
				list.Add(new NpcInfo
				{
					ID = (int)ResultDataReader["ID"],
					Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString()),
					Level = (int)ResultDataReader["Level"],
					Camp = (int)ResultDataReader["Camp"],
					Type = (int)ResultDataReader["Type"],
					Blood = (int)ResultDataReader["Blood"],
					X = (int)ResultDataReader["X"],
					Y = (int)ResultDataReader["Y"],
					Width = (int)ResultDataReader["Width"],
					Height = (int)ResultDataReader["Height"],
					MoveMin = (int)ResultDataReader["MoveMin"],
					MoveMax = (int)ResultDataReader["MoveMax"],
					BaseDamage = (int)ResultDataReader["BaseDamage"],
					BaseGuard = (int)ResultDataReader["BaseGuard"],
					Attack = (int)ResultDataReader["Attack"],
					Defence = (int)ResultDataReader["Defence"],
					Agility = (int)ResultDataReader["Agility"],
					Lucky = (int)ResultDataReader["Lucky"],
					ModelID = ((ResultDataReader["ModelID"] == null) ? "" : ResultDataReader["ModelID"].ToString()),
					ResourcesPath = ((ResultDataReader["ResourcesPath"] == null) ? "" : ResultDataReader["ResourcesPath"].ToString()),
					DropRate = ((ResultDataReader["DropRate"] == null) ? "" : ResultDataReader["DropRate"].ToString()),
					Experience = (int)ResultDataReader["Experience"],
					Delay = (int)ResultDataReader["Delay"],
					Immunity = (int)ResultDataReader["Immunity"],
					Alert = (int)ResultDataReader["Alert"],
					Range = (int)ResultDataReader["Range"],
					Preserve = (int)ResultDataReader["Preserve"],
					Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString()),
					FireX = (int)ResultDataReader["FireX"],
					FireY = (int)ResultDataReader["FireY"],
					DropId = (int)ResultDataReader["DropId"],
					CurrentBallId = (int)ResultDataReader["CurrentBallId"],
					speed = (int)ResultDataReader["speed"]
				});
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
				list.Add(new MissionInfo
				{
					Id = (int)ResultDataReader["ID"],
					Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString()),
					TotalCount = (int)ResultDataReader["TotalCount"],
					TotalTurn = (int)ResultDataReader["TotalTurn"],
					Script = ((ResultDataReader["Script"] == null) ? "" : ResultDataReader["Script"].ToString()),
					Success = ((ResultDataReader["Success"] == null) ? "" : ResultDataReader["Success"].ToString()),
					Failure = ((ResultDataReader["Failure"] == null) ? "" : ResultDataReader["Failure"].ToString()),
					Description = ((ResultDataReader["Description"] == null) ? "" : ResultDataReader["Description"].ToString()),
					IncrementDelay = (int)ResultDataReader["IncrementDelay"],
					Delay = (int)ResultDataReader["Delay"],
					Title = ((ResultDataReader["Title"] == null) ? "" : ResultDataReader["Title"].ToString()),
					Param1 = (int)ResultDataReader["Param1"],
					Param2 = (int)ResultDataReader["Param2"]
				});
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
