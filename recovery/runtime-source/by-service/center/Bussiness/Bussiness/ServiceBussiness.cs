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
				list.Add(serverInfo);
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
				serverInfo.MustLevel = (int)ResultDataReader["MustLevel"];
				serverInfo.LowestLevel = (int)ResultDataReader["LowestLevel"];
				list.Add(serverInfo);
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
				RateInfo rateInfo = new RateInfo();
				rateInfo.ServerID = (int)ResultDataReader["ServerID"];
				rateInfo.Rate = (float)(decimal)ResultDataReader["Rate"];
				rateInfo.BeginDay = (DateTime)ResultDataReader["BeginDay"];
				rateInfo.EndDay = (DateTime)ResultDataReader["EndDay"];
				rateInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				rateInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				rateInfo.Type = (int)ResultDataReader["Type"];
				arrayList.Add(rateInfo);
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
				FightRateInfo fightRateInfo = new FightRateInfo();
				fightRateInfo.ID = (int)ResultDataReader["ID"];
				fightRateInfo.ServerID = (int)ResultDataReader["ServerID"];
				fightRateInfo.Rate = (int)ResultDataReader["Rate"];
				fightRateInfo.BeginDay = (DateTime)ResultDataReader["BeginDay"];
				fightRateInfo.EndDay = (DateTime)ResultDataReader["EndDay"];
				fightRateInfo.BeginTime = (DateTime)ResultDataReader["BeginTime"];
				fightRateInfo.EndTime = (DateTime)ResultDataReader["EndTime"];
				fightRateInfo.SelfCue = ((ResultDataReader["SelfCue"] == null) ? "" : ResultDataReader["SelfCue"].ToString());
				fightRateInfo.EnemyCue = ((ResultDataReader["EnemyCue"] == null) ? "" : ResultDataReader["EnemyCue"].ToString());
				fightRateInfo.BoyTemplateID = (int)ResultDataReader["BoyTemplateID"];
				fightRateInfo.GirlTemplateID = (int)ResultDataReader["GirlTemplateID"];
				fightRateInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				list.Add(fightRateInfo);
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
