using System;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness;

public class UserInfoBussiness : BaseBussiness
{
	public bool GetFromDbByUid(string uid, ref string userName, ref string portrait)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Uid", uid)
			};
			db.GetReader(ref ResultDataReader, "SP_User_Info_QueryByUid", sqlParameters);
			while (ResultDataReader.Read())
			{
				userName = ((ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString());
				portrait = ((ResultDataReader["Portrait"] == null) ? "" : ResultDataReader["Portrait"].ToString());
			}
			if (!string.IsNullOrEmpty(userName) && !string.IsNullOrEmpty(portrait))
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

	public bool AddUserInfo(string uid, string userName, string portrait)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[4]
			{
				new SqlParameter("@Uid", uid),
				new SqlParameter("@UserName", userName),
				new SqlParameter("@Portrait", portrait),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[3].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_User_Info_Insert", array);
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
