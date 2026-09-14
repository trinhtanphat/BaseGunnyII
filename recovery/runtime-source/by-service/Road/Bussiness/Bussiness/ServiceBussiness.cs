using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class ServiceBussiness : BaseBussiness
{
	public ServerInfo GetServiceSingle(int ID)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@ID", SqlDbType.Int, 4)
			};
			array[0].Value = ID;
			db.GetReader(ref ResultDataReader, "SP_Service_Single", array);
			if (ResultDataReader.Read())
			{
				ServerInfo serverInfo = new ServerInfo();
				serverInfo.ID = (int)ResultDataReader["ID"];
				serverInfo.IP = ResultDataReader["IP"].ToString();
				serverInfo.Name = ResultDataReader["Name"].ToString();
				serverInfo.Online = (int)ResultDataReader["Online"];
				serverInfo.Port = (int)ResultDataReader["Port"];
				serverInfo.Remark = ResultDataReader["Remark"].ToString();
				serverInfo.Room = (int)ResultDataReader["Room"];
				serverInfo.State = (int)ResultDataReader["State"];
				serverInfo.Total = (int)ResultDataReader["Total"];
				serverInfo.RSA = ResultDataReader["RSA"].ToString();
				serverInfo.NewerServer = (bool)ResultDataReader["NewerServer"];
				return serverInfo;
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

	public ServerInfo[] GetServiceByIP(string IP)
	{
		List<ServerInfo> list = new List<ServerInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] array = new SqlParameter[1]
			{
				new SqlParameter("@IP", SqlDbType.NVarChar, 50)
			};
			array[0].Value = IP;
			db.GetReader(ref ResultDataReader, "SP_Service_ListByIP", array);
			while (ResultDataReader.Read())
			{
				list.Add(new ServerInfo
				{
					ID = (int)ResultDataReader["ID"],
					IP = ResultDataReader["IP"].ToString(),
					Name = ResultDataReader["Name"].ToString(),
					Online = (int)ResultDataReader["Online"],
					Port = (int)ResultDataReader["Port"],
					Remark = ResultDataReader["Remark"].ToString(),
					Room = (int)ResultDataReader["Room"],
					State = (int)ResultDataReader["State"],
					Total = (int)ResultDataReader["Total"],
					RSA = ResultDataReader["RSA"].ToString()
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

	public ServerInfo[] GetServerList()
	{
		List<ServerInfo> list = new List<ServerInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Service_List");
			while (ResultDataReader.Read())
			{
				list.Add(new ServerInfo
				{
					ID = (int)ResultDataReader["ID"],
					IP = ResultDataReader["IP"].ToString(),
					Name = ResultDataReader["Name"].ToString(),
					Online = (int)ResultDataReader["Online"],
					Port = (int)ResultDataReader["Port"],
					Remark = ResultDataReader["Remark"].ToString(),
					Room = (int)ResultDataReader["Room"],
					State = (int)ResultDataReader["State"],
					Total = (int)ResultDataReader["Total"],
					RSA = ResultDataReader["RSA"].ToString(),
					MustLevel = (int)ResultDataReader["MustLevel"],
					LowestLevel = (int)ResultDataReader["LowestLevel"]
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

	public RecordInfo GetRecordInfo(DateTime date, int SaveRecordSecond)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@Date", date.ToString("yyyy-MM-dd HH:mm:ss")),
				new SqlParameter("@Second", SaveRecordSecond)
			};
			db.GetReader(ref ResultDataReader, "SP_Server_Record", sqlParameters);
			if (ResultDataReader.Read())
			{
				RecordInfo recordInfo = new RecordInfo();
				recordInfo.ActiveExpendBoy = (int)ResultDataReader["ActiveExpendBoy"];
				recordInfo.ActiveExpendGirl = (int)ResultDataReader["ActiveExpendGirl"];
				recordInfo.ActviePayBoy = (int)ResultDataReader["ActviePayBoy"];
				recordInfo.ActviePayGirl = (int)ResultDataReader["ActviePayGirl"];
				recordInfo.ExpendBoy = (int)ResultDataReader["ExpendBoy"];
				recordInfo.ExpendGirl = (int)ResultDataReader["ExpendGirl"];
				recordInfo.OnlineBoy = (int)ResultDataReader["OnlineBoy"];
				recordInfo.OnlineGirl = (int)ResultDataReader["OnlineGirl"];
				recordInfo.TotalBoy = (int)ResultDataReader["TotalBoy"];
				recordInfo.TotalGirl = (int)ResultDataReader["TotalGirl"];
				recordInfo.ActiveOnlineBoy = (int)ResultDataReader["ActiveOnlineBoy"];
				recordInfo.ActiveOnlineGirl = (int)ResultDataReader["ActiveOnlineGirl"];
				return recordInfo;
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

	public bool UpdateService(ServerInfo info)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[3]
			{
				new SqlParameter("@ID", info.ID),
				new SqlParameter("@Online", info.Online),
				new SqlParameter("@State", info.State)
			};
			result = db.RunProcedure("SP_Service_Update", sqlParameters);
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

	public bool UpdateRSA(int ID, string RSA)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ID", ID),
				new SqlParameter("@RSA", RSA)
			};
			result = db.RunProcedure("SP_Service_UpdateRSA", sqlParameters);
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

	public Dictionary<string, string> GetServerConfig()
	{
		Dictionary<string, string> dictionary = new Dictionary<string, string>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Server_Config");
			while (ResultDataReader.Read())
			{
				if (!dictionary.ContainsKey(ResultDataReader["Name"].ToString()))
				{
					dictionary.Add(ResultDataReader["Name"].ToString(), ResultDataReader["Value"].ToString());
				}
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetServerConfig", exception);
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

	public ServerProperty GetServerPropertyByKey(string key)
	{
		ServerProperty serverProperty = null;
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Key", key)
			};
			db.GetReader(ref ResultDataReader, "SP_Server_Config_Single", sqlParameters);
			while (ResultDataReader.Read())
			{
				serverProperty = new ServerProperty();
				serverProperty.Key = ResultDataReader["Name"].ToString();
				serverProperty.Value = ResultDataReader["Value"].ToString();
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetServerConfig", exception);
			}
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
		return serverProperty;
	}

	public bool UpdateServerPropertyByKey(string key, string value)
	{
		bool result = false;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@Key", key),
				new SqlParameter("@Value", value)
			};
			result = db.RunProcedure("SP_Server_Config_Update", sqlParameters);
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

	public ArrayList GetRate(int serverId)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			ArrayList arrayList = new ArrayList();
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ServerID", serverId)
			};
			db.GetReader(ref ResultDataReader, "SP_Rate", sqlParameters);
			while (ResultDataReader.Read())
			{
				arrayList.Add(new RateInfo
				{
					ServerID = (int)ResultDataReader["ServerID"],
					Rate = (float)(decimal)ResultDataReader["Rate"],
					BeginDay = (DateTime)ResultDataReader["BeginDay"],
					EndDay = (DateTime)ResultDataReader["EndDay"],
					BeginTime = (DateTime)ResultDataReader["BeginTime"],
					EndTime = (DateTime)ResultDataReader["EndTime"],
					Type = (int)ResultDataReader["Type"]
				});
			}
			arrayList.TrimToSize();
			return arrayList;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetRates", exception);
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

	public RateInfo GetRateWithType(int serverId, int type)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[2]
			{
				new SqlParameter("@ServerID", serverId),
				new SqlParameter("@Type", type)
			};
			db.GetReader(ref ResultDataReader, "SP_Rate_WithType", sqlParameters);
			if (ResultDataReader.Read())
			{
				RateInfo rateInfo = new RateInfo();
				rateInfo.ServerID = (int)ResultDataReader["ServerID"];
				rateInfo.Type = type;
				rateInfo.Rate = (float)ResultDataReader["Rate"];
				rateInfo.BeginDay = (DateTime)ResultDataReader["BeginDay"];
				rateInfo.EndDay = (DateTime)ResultDataReader["EndDay"];
				rateInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				rateInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				return rateInfo;
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetRate type: " + type, exception);
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

	public FightRateInfo[] GetFightRate(int serverId)
	{
		SqlDataReader ResultDataReader = null;
		List<FightRateInfo> list = new List<FightRateInfo>();
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@ServerID", serverId)
			};
			db.GetReader(ref ResultDataReader, "SP_Fight_Rate", sqlParameters);
			if (ResultDataReader.Read())
			{
				list.Add(new FightRateInfo
				{
					ID = (int)ResultDataReader["ID"],
					ServerID = (int)ResultDataReader["ServerID"],
					Rate = (int)ResultDataReader["Rate"],
					BeginDay = (DateTime)ResultDataReader["BeginDay"],
					EndDay = (DateTime)ResultDataReader["EndDay"],
					BeginTime = (DateTime)ResultDataReader["BeginTime"],
					EndTime = (DateTime)ResultDataReader["EndTime"],
					SelfCue = ((ResultDataReader["SelfCue"] == null) ? "" : ResultDataReader["SelfCue"].ToString()),
					EnemyCue = ((ResultDataReader["EnemyCue"] == null) ? "" : ResultDataReader["EnemyCue"].ToString()),
					BoyTemplateID = (int)ResultDataReader["BoyTemplateID"],
					GirlTemplateID = (int)ResultDataReader["GirlTemplateID"],
					Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString())
				});
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetFightRate", exception);
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

	public string GetGameEdition()
	{
		string result = string.Empty;
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Server_Edition");
			if (ResultDataReader.Read())
			{
				result = ((ResultDataReader["value"] == null) ? "" : ResultDataReader["value"].ToString());
				return result;
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
		return result;
	}
}
