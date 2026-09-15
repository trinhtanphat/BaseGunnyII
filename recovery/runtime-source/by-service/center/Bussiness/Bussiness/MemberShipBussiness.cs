using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using SqlDataProvider.BaseClass;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness;

public class MemberShipBussiness : IDisposable
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected Sql_DbObject db;

	public MemberShipBussiness()
	{
		db = new Sql_DbObject("AppConfig", "membershipDb");
	}

	public eStoreInfo[] GetAlleStoreSale()
	{
		List<eStoreInfo> list = new List<eStoreInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_eStore_Desc_Sale");
			while (ResultDataReader.Read())
			{
				eStoreInfo eStoreInfo2 = new eStoreInfo();
				eStoreInfo2.StoreID = (int)ResultDataReader["StoreID"];
				eStoreInfo2.TemplateID = (int)ResultDataReader["TemplateID"];
				eStoreInfo2.PriceValue = (int)ResultDataReader["PriceValue1"];
				eStoreInfo2.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				eStoreInfo2.AttackCompose = (int)ResultDataReader["AttackCompose"];
				eStoreInfo2.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				eStoreInfo2.DefendCompose = (int)ResultDataReader["DefendCompose"];
				eStoreInfo2.LuckCompose = (int)ResultDataReader["LuckCompose"];
				eStoreInfo2.IsBinds = (bool)ResultDataReader["IsBinds"];
				eStoreInfo2.ValidDate = (int)ResultDataReader["ValidDate"];
				eStoreInfo2.OldPriceValue = (int)ResultDataReader["PriceValue"];
				list.Add(eStoreInfo2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SP_eStore_Desc_Sale", exception);
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

	public eStoreInfo[] GetAlleStoreTopBuy()
	{
		List<eStoreInfo> list = new List<eStoreInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_eStore_Desc_Buy");
			while (ResultDataReader.Read())
			{
				eStoreInfo eStoreInfo2 = new eStoreInfo();
				eStoreInfo2.StoreID = (int)ResultDataReader["StoreID"];
				eStoreInfo2.TemplateID = (int)ResultDataReader["TemplateID"];
				eStoreInfo2.PriceValue = (int)ResultDataReader["PriceValue"];
				eStoreInfo2.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				eStoreInfo2.AttackCompose = (int)ResultDataReader["AttackCompose"];
				eStoreInfo2.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				eStoreInfo2.DefendCompose = (int)ResultDataReader["DefendCompose"];
				eStoreInfo2.LuckCompose = (int)ResultDataReader["LuckCompose"];
				eStoreInfo2.IsBinds = (bool)ResultDataReader["IsBinds"];
				eStoreInfo2.ValidDate = (int)ResultDataReader["ValidDate"];
				list.Add(eStoreInfo2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("SP_eStore_Desc_Buy", exception);
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

	public eStoreInfo[] GetAlleStoreByDesc()
	{
		List<eStoreInfo> list = new List<eStoreInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_eStore_Desc");
			while (ResultDataReader.Read())
			{
				eStoreInfo eStoreInfo2 = new eStoreInfo();
				eStoreInfo2.StoreID = (int)ResultDataReader["StoreID"];
				eStoreInfo2.TemplateID = (int)ResultDataReader["TemplateID"];
				eStoreInfo2.PriceValue = (int)ResultDataReader["PriceValue"];
				eStoreInfo2.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				eStoreInfo2.AttackCompose = (int)ResultDataReader["AttackCompose"];
				eStoreInfo2.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				eStoreInfo2.DefendCompose = (int)ResultDataReader["DefendCompose"];
				eStoreInfo2.LuckCompose = (int)ResultDataReader["LuckCompose"];
				eStoreInfo2.IsBinds = (bool)ResultDataReader["IsBinds"];
				eStoreInfo2.ValidDate = (int)ResultDataReader["ValidDate"];
				list.Add(eStoreInfo2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("eStore", exception);
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

	public eStoreInfo[] GetAlleStore()
	{
		List<eStoreInfo> list = new List<eStoreInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_eStore_All");
			while (ResultDataReader.Read())
			{
				eStoreInfo eStoreInfo2 = new eStoreInfo();
				eStoreInfo2.StoreID = (int)ResultDataReader["StoreID"];
				eStoreInfo2.TemplateID = (int)ResultDataReader["TemplateID"];
				eStoreInfo2.PriceValue = (int)ResultDataReader["PriceValue"];
				eStoreInfo2.StrengthenLevel = (int)ResultDataReader["StrengthenLevel"];
				eStoreInfo2.AttackCompose = (int)ResultDataReader["AttackCompose"];
				eStoreInfo2.AgilityCompose = (int)ResultDataReader["AgilityCompose"];
				eStoreInfo2.DefendCompose = (int)ResultDataReader["DefendCompose"];
				eStoreInfo2.LuckCompose = (int)ResultDataReader["LuckCompose"];
				eStoreInfo2.IsBinds = (bool)ResultDataReader["IsBinds"];
				eStoreInfo2.ValidDate = (int)ResultDataReader["ValidDate"];
				list.Add(eStoreInfo2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("eStore", exception);
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

	public bool CheckUsername(string applicationname, string username, string password)
	{
		SqlParameter[] array = new SqlParameter[4]
		{
			new SqlParameter("@ApplicationName", applicationname),
			new SqlParameter("@UserName", username),
			new SqlParameter("@password", password),
			new SqlParameter("@UserId", SqlDbType.Int)
		};
		array[3].Direction = ParameterDirection.Output;
		db.RunProcedure("Mem_Users_Accede", array);
		int result = 0;
		int.TryParse(array[3].Value.ToString(), out result);
		return result > 0;
	}

	public bool CreateUsername(string applicationname, string username, string password, string email, string passwordformat, string passwordsalt, bool usersex)
	{
		bool flag = false;
		SqlParameter[] array = new SqlParameter[8]
		{
			new SqlParameter("@ApplicationName", applicationname),
			new SqlParameter("@UserName", username),
			new SqlParameter("@password", password),
			new SqlParameter("@email", email),
			new SqlParameter("@PasswordFormat", passwordformat),
			new SqlParameter("@PasswordSalt", passwordsalt),
			new SqlParameter("@UserSex", usersex),
			new SqlParameter("@UserId", SqlDbType.Int)
		};
		array[7].Direction = ParameterDirection.Output;
		flag = db.RunProcedure("Mem_Users_CreateUser", array);
		if (flag)
		{
			flag = (int)array[7].Value > 0;
		}
		return flag;
	}

	public void Dispose()
	{
		db.Dispose();
		GC.SuppressFinalize(this);
	}

	public bool ExistsUsername(string username)
	{
		SqlParameter[] array = new SqlParameter[2]
		{
			new SqlParameter("@UserName", username),
			new SqlParameter("@UserCOUNT", SqlDbType.Int)
		};
		array[1].Direction = ParameterDirection.Output;
		db.RunProcedure("Mem_UserInfo_SearchName", array);
		return (int)array[1].Value > 0;
	}

	public int CheckPoint(string username)
	{
		SqlParameter[] array = new SqlParameter[2]
		{
			new SqlParameter("@UserName", username),
			new SqlParameter("@Point", SqlDbType.Int)
		};
		array[1].Direction = ParameterDirection.Output;
		db.RunProcedure("Mem_User_Point", array);
		return (int)array[1].Value;
	}

	public bool RemovePoint(string username, int Point)
	{
		bool flag = false;
		SqlParameter[] array = new SqlParameter[3]
		{
			new SqlParameter("@UserName", username),
			new SqlParameter("@Point", Point),
			new SqlParameter("@Result", SqlDbType.Int)
		};
		array[2].Direction = ParameterDirection.ReturnValue;
		db.RunProcedure("Mem_User_Remove_Point", array);
		return (int)array[2].Value == 0;
	}

	public bool CheckAdmin(string username)
	{
		SqlParameter[] array = new SqlParameter[2]
		{
			new SqlParameter("@UserName", username),
			new SqlParameter("@UserCOUNT", SqlDbType.Int)
		};
		array[1].Direction = ParameterDirection.Output;
		db.RunProcedure("Mem_UserInfo_Addmin", array);
		return (int)array[1].Value > 0;
	}
}
