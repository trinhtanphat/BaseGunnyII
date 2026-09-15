using System;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness;

public class CookieInfoBussiness : BaseBussiness
{
	public bool GetFromDbByUser(string bdSigUser, ref string bdSigPortrait, ref string bdSigSessionKey)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@BdSigUser", bdSigUser)
			};
			db.GetReader(ref ResultDataReader, "SP_Cookie_Info_QueryByUser", sqlParameters);
			while (ResultDataReader.Read())
			{
				bdSigPortrait = ((ResultDataReader["BdSigPortrait"] == null) ? "" : ResultDataReader["BdSigPortrait"].ToString());
				bdSigSessionKey = ((ResultDataReader["BdSigSessionKey"] == null) ? "" : ResultDataReader["BdSigSessionKey"].ToString());
			}
			if (!string.IsNullOrEmpty(bdSigPortrait) && !string.IsNullOrEmpty(bdSigSessionKey))
			{
				return true;
			}
			return false;
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("Init", exception);
			}
			return false;
		}
		finally
		{
			if (ResultDataReader != null && !ResultDataReader.IsClosed)
			{
				ResultDataReader.Close();
			}
		}
	}

	public bool AddCookieInfo(string bdSigUser, string bdSigPortrait, string bdSigSessionKey)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@BdSigUser", bdSigUser),
				new SqlParameter("@BdSigPortrait", bdSigPortrait),
				new SqlParameter("@BdSigSessionKey", bdSigSessionKey),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Cookie_Info_Insert", array);
			int num = (int)array[3].Value;
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
}
