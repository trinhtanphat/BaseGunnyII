using System;
using System.Data;
using System.Data.SqlClient;

namespace Bussiness;

public class OrderBussiness : BaseBussiness
{
	public bool AddOrder(string order, double amount, string username, string payWay, string serverId)
	{
		bool result = false;
		try
		{
			SqlParameter[] array = new SqlParameter[6]
			{
				new SqlParameter("@Order", order),
				new SqlParameter("@Amount", amount),
				new SqlParameter("@Username", username),
				new SqlParameter("@PayWay", payWay),
				new SqlParameter("@ServerId", serverId),
				new SqlParameter("@Result", SqlDbType.Int)
			};
			array[5].Direction = ParameterDirection.ReturnValue;
			db.RunProcedure("SP_Charge_Order", array);
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

	public string GetOrderToName(string order, ref string serverId)
	{
		SqlDataReader ResultDataReader = null;
		try
		{
			SqlParameter[] sqlParameters = new SqlParameter[1]
			{
				new SqlParameter("@Order", order)
			};
			db.GetReader(ref ResultDataReader, "SP_Charge_Order_Single", sqlParameters);
			if (ResultDataReader.Read())
			{
				serverId = ((ResultDataReader["ServerId"] == null) ? "" : ResultDataReader["ServerId"].ToString());
				return (ResultDataReader["UserName"] == null) ? "" : ResultDataReader["UserName"].ToString();
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
		return "";
	}
}
