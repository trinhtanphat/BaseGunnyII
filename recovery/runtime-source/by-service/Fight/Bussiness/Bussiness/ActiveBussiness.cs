using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class ActiveBussiness : BaseBussiness
{
	public bool AddActiveNumber(string AwardID, int ActiveID)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[3]
			{
				new SqlParameter("@AwardID", AwardID),
				new SqlParameter("@ActiveID", ActiveID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[2].Direction = ParameterDirection.ReturnValue;
			result = db.RunProcedure("SP_Active_Number_Add", array);
			result = (int)array[2].Value == 0;
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

	public ActiveInfo[] GetAllActives()
	{
		List<ActiveInfo> list = new List<ActiveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Active_All");
			while (ResultDataReader.Read())
			{
				list.Add(InitActiveInfo(ResultDataReader));
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

	public ActiveInfo GetSingleActives(int activeID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = activeID;
			db.GetReader(ref ResultDataReader, "SP_Active_Single", array);
			if (ResultDataReader.Read())
			{
				return InitActiveInfo(ResultDataReader);
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

	public ActiveInfo InitActiveInfo(SqlDataReader reader)
	{
		ActiveInfo activeInfo = new ActiveInfo();
		activeInfo.ActiveID = (int)reader["ActiveID"];
		activeInfo.Description = ((reader["Description"] == null) ? "" : reader["Description"].ToString());
		activeInfo.Content = ((reader["Content"] == null) ? "" : reader["Content"].ToString());
		activeInfo.AwardContent = ((reader["AwardContent"] == null) ? "" : reader["AwardContent"].ToString());
		activeInfo.HasKey = (int)reader["HasKey"];
		if (!string.IsNullOrEmpty(reader["EndDate"].ToString()))
		{
			activeInfo.EndDate = (DateTime)reader["EndDate"];
		}
		activeInfo.IsOnly = (int)reader["IsOnly"];
		activeInfo.StartDate = (DateTime)reader["StartDate"];
		activeInfo.Title = reader["Title"].ToString();
		activeInfo.Type = (int)reader["Type"];
		activeInfo.ActiveType = (int)reader["ActiveType"];
		activeInfo.ActionTimeContent = ((reader["ActionTimeContent"] == null) ? "" : reader["ActionTimeContent"].ToString());
		activeInfo.IsAdvance = (bool)reader["IsAdvance"];
		activeInfo.GoodsExchangeTypes = ((reader["GoodsExchangeTypes"] == null) ? "" : reader["GoodsExchangeTypes"].ToString());
		activeInfo.GoodsExchangeNum = ((reader["GoodsExchangeNum"] == null) ? "" : reader["GoodsExchangeNum"].ToString());
		activeInfo.limitType = ((reader["limitType"] == null) ? "" : reader["limitType"].ToString());
		activeInfo.limitValue = ((reader["limitValue"] == null) ? "" : reader["limitValue"].ToString());
		activeInfo.IsShow = (bool)reader["IsShow"];
		activeInfo.IconID = (int)reader["IconID"];
		return activeInfo;
	}

	public ActiveConvertItemInfo[] GetSingleActiveConvertItems(int activeID)
	{
		List<ActiveConvertItemInfo> list = new List<ActiveConvertItemInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = activeID;
			db.GetReader(ref ResultDataReader, "SP_Active_Convert_Item_Info_Single", array);
			while (ResultDataReader.Read())
			{
				list.Add(InitActiveConvertItemInfo(ResultDataReader));
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

	public ActiveConvertItemInfo InitActiveConvertItemInfo(SqlDataReader reader)
	{
		ActiveConvertItemInfo activeConvertItemInfo = new ActiveConvertItemInfo();
		activeConvertItemInfo.ID = (int)reader["ID"];
		activeConvertItemInfo.ActiveID = (int)reader["ActiveID"];
		activeConvertItemInfo.TemplateID = (int)reader["TemplateID"];
		activeConvertItemInfo.ItemType = (int)reader["ItemType"];
		activeConvertItemInfo.ItemCount = (int)reader["ItemCount"];
		activeConvertItemInfo.LimitValue = (int)reader["LimitValue"];
		activeConvertItemInfo.IsBind = (bool)reader["IsBind"];
		activeConvertItemInfo.ValidDate = (int)reader["ValidDate"];
		return activeConvertItemInfo;
	}

	public int PullDown(int activeID, string awardID, int userID, ref string msg)
	{
		int num = 1;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@ActiveID", activeID),
				new SqlParameter("@AwardID", awardID),
				new SqlParameter("@UserID", userID),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			if (db.RunProcedure("SP_Active_PullDown", array))
			{
				num = (int)array[3].Value;
				switch (num)
				{
				case 0:
					msg = "ActiveBussiness.Msg0";
					break;
				case 1:
					msg = "ActiveBussiness.Msg1";
					break;
				case 2:
					msg = "ActiveBussiness.Msg2";
					break;
				case 3:
					msg = "ActiveBussiness.Msg3";
					break;
				case 4:
					msg = "ActiveBussiness.Msg4";
					break;
				case 5:
					msg = "ActiveBussiness.Msg5";
					break;
				case 6:
					msg = "ActiveBussiness.Msg6";
					break;
				case 7:
					msg = "ActiveBussiness.Msg7";
					break;
				case 8:
					msg = "ActiveBussiness.Msg8";
					break;
				default:
					msg = "ActiveBussiness.Msg9";
					break;
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
		return num;
	}
}
