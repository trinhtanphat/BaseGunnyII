using System;
using System.Collections;
using System.Data;
using System.Data.SqlClient;
using System.Xml;

namespace DAL;

public sealed class SqlHelper
{
	private enum SqlConnectionOwnership
	{
		Internal,
		External
	}

	private SqlHelper()
	{
	}

	private static void AttachParameters(SqlCommand command, SqlParameter[] commandParameters)
	{
		foreach (SqlParameter sqlParameter in commandParameters)
		{
			if (sqlParameter.Direction == ParameterDirection.InputOutput && sqlParameter.Value == null)
			{
				sqlParameter.Value = DBNull.Value;
			}
			command.Parameters.Add(sqlParameter);
		}
	}

	public static void AssignParameterValues(SqlParameter[] commandParameters, params object[] parameterValues)
	{
		if (commandParameters == null || parameterValues == null)
		{
			return;
		}
		if (commandParameters.Length != parameterValues.Length)
		{
			throw new ArgumentException("Parameter count does not match Parameter Value count.");
		}
		int i = 0;
		for (int num = commandParameters.Length; i < num; i++)
		{
			if (parameterValues[i] != null && (commandParameters[i].Direction == ParameterDirection.Input || commandParameters[i].Direction == ParameterDirection.InputOutput))
			{
				commandParameters[i].Value = parameterValues[i];
			}
		}
	}

	public static void AssignParameterValues(SqlParameter[] commandParameters, Hashtable parameterValues)
	{
		if (commandParameters == null || parameterValues == null)
		{
			return;
		}
		if (commandParameters.Length != parameterValues.Count)
		{
			throw new ArgumentException("Parameter count does not match Parameter Value count.");
		}
		int i = 0;
		for (int num = commandParameters.Length; i < num; i++)
		{
			if (parameterValues[commandParameters[i].ParameterName] != null && (commandParameters[i].Direction == ParameterDirection.Input || commandParameters[i].Direction == ParameterDirection.InputOutput))
			{
				commandParameters[i].Value = parameterValues[commandParameters[i].ParameterName];
			}
		}
	}

	private static void PrepareCommand(SqlCommand command, SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters)
	{
		if (connection.State != ConnectionState.Open)
		{
			connection.Open();
		}
		command.Connection = connection;
		command.CommandText = commandText;
		if (transaction != null)
		{
			command.Transaction = transaction;
		}
		command.CommandType = commandType;
		if (commandParameters != null)
		{
			AttachParameters(command, commandParameters);
		}
	}

	public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText)
	{
		return ExecuteNonQuery(connectionString, commandType, commandText, (SqlParameter[])null);
	}

	public static int ExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		using SqlConnection sqlConnection = new SqlConnection(connectionString);
		sqlConnection.Open();
		return ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
	}

	public static int ExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
	}

	public static int ExecuteNonQuery(string connectionString, string spName, Hashtable parameterValues)
	{
		if (parameterValues != null && parameterValues.Count > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
	}

	public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText)
	{
		return ExecuteNonQuery(connection, commandType, commandText, (SqlParameter[])null);
	}

	public static int ExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
		int result = sqlCommand.ExecuteNonQuery();
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static int ExecuteNonQuery(SqlConnection connection, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteNonQuery(connection, CommandType.StoredProcedure, spName);
	}

	public static int ExecuteNonQuery(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
		int result = sqlCommand.ExecuteNonQuery();
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static int ExecuteNonQuery(SqlTransaction transaction, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteNonQuery(transaction, CommandType.StoredProcedure, spName);
	}

	public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText)
	{
		return ExecuteDataset(connectionString, commandType, commandText, (SqlParameter[])null);
	}

	public static DataSet ExecuteDataset(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		using SqlConnection sqlConnection = new SqlConnection(connectionString);
		sqlConnection.Open();
		return ExecuteDataset(sqlConnection, commandType, commandText, commandParameters);
	}

	public static DataSet ExecuteDataset(string connectionString, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteDataset(connectionString, CommandType.StoredProcedure, spName);
	}

	public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText)
	{
		return ExecuteDataset(connection, commandType, commandText, (SqlParameter[])null);
	}

	public static DataSet ExecuteDataset(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
		SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
		DataSet dataSet = new DataSet();
		sqlDataAdapter.Fill(dataSet);
		sqlCommand.Parameters.Clear();
		return dataSet;
	}

	public static DataSet ExecuteDataset(SqlConnection connection, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteDataset(connection, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteDataset(connection, CommandType.StoredProcedure, spName);
	}

	public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText)
	{
		return ExecuteDataset(transaction, commandType, commandText, (SqlParameter[])null);
	}

	public static DataSet ExecuteDataset(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
		SqlDataAdapter sqlDataAdapter = new SqlDataAdapter(sqlCommand);
		DataSet dataSet = new DataSet();
		sqlDataAdapter.Fill(dataSet);
		sqlCommand.Parameters.Clear();
		return dataSet;
	}

	public static DataSet ExecuteDataset(SqlTransaction transaction, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteDataset(transaction, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteDataset(transaction, CommandType.StoredProcedure, spName);
	}

	private static SqlDataReader ExecuteReader(SqlConnection connection, SqlTransaction transaction, CommandType commandType, string commandText, SqlParameter[] commandParameters, SqlConnectionOwnership connectionOwnership)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, connection, transaction, commandType, commandText, commandParameters);
		SqlDataReader result = ((connectionOwnership != SqlConnectionOwnership.External) ? sqlCommand.ExecuteReader(CommandBehavior.CloseConnection) : sqlCommand.ExecuteReader());
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText)
	{
		return ExecuteReader(connectionString, commandType, commandText, (SqlParameter[])null);
	}

	public static SqlDataReader ExecuteReader(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlConnection sqlConnection = new SqlConnection(connectionString);
		sqlConnection.Open();
		try
		{
			return ExecuteReader(sqlConnection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.Internal);
		}
		catch
		{
			sqlConnection.Close();
			throw;
		}
	}

	public static SqlDataReader ExecuteReader(string connectionString, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteReader(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteReader(connectionString, CommandType.StoredProcedure, spName);
	}

	public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText)
	{
		return ExecuteReader(connection, commandType, commandText, (SqlParameter[])null);
	}

	public static SqlDataReader ExecuteReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		return ExecuteReader(connection, null, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
	}

	public static SqlDataReader ExecuteReader(SqlConnection connection, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteReader(connection, CommandType.StoredProcedure, spName);
	}

	public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText)
	{
		return ExecuteReader(transaction, commandType, commandText, (SqlParameter[])null);
	}

	public static SqlDataReader ExecuteReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		return ExecuteReader(transaction.Connection, transaction, commandType, commandText, commandParameters, SqlConnectionOwnership.External);
	}

	public static SqlDataReader ExecuteReader(SqlTransaction transaction, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteReader(transaction, CommandType.StoredProcedure, spName);
	}

	public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText)
	{
		return ExecuteScalar(connectionString, commandType, commandText, (SqlParameter[])null);
	}

	public static object ExecuteScalar(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		using SqlConnection sqlConnection = new SqlConnection(connectionString);
		sqlConnection.Open();
		return ExecuteScalar(sqlConnection, commandType, commandText, commandParameters);
	}

	public static object ExecuteScalar(string connectionString, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteScalar(connectionString, CommandType.StoredProcedure, spName);
	}

	public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText)
	{
		return ExecuteScalar(connection, commandType, commandText, (SqlParameter[])null);
	}

	public static object ExecuteScalar(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
		object result = sqlCommand.ExecuteScalar();
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static object ExecuteScalar(SqlConnection connection, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteScalar(connection, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteScalar(connection, CommandType.StoredProcedure, spName);
	}

	public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText)
	{
		return ExecuteScalar(transaction, commandType, commandText, (SqlParameter[])null);
	}

	public static object ExecuteScalar(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
		object result = sqlCommand.ExecuteScalar();
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static object ExecuteScalar(SqlTransaction transaction, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteScalar(transaction, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteScalar(transaction, CommandType.StoredProcedure, spName);
	}

	public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText)
	{
		return ExecuteXmlReader(connection, commandType, commandText, (SqlParameter[])null);
	}

	public static XmlReader ExecuteXmlReader(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
		XmlReader result = sqlCommand.ExecuteXmlReader();
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static XmlReader ExecuteXmlReader(SqlConnection connection, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteXmlReader(connection, CommandType.StoredProcedure, spName);
	}

	public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText)
	{
		return ExecuteXmlReader(transaction, commandType, commandText, (SqlParameter[])null);
	}

	public static XmlReader ExecuteXmlReader(SqlTransaction transaction, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, transaction.Connection, transaction, commandType, commandText, commandParameters);
		XmlReader result = sqlCommand.ExecuteXmlReader();
		sqlCommand.Parameters.Clear();
		return result;
	}

	public static XmlReader ExecuteXmlReader(SqlTransaction transaction, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(transaction.Connection.ConnectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName, spParameterSet);
		}
		return ExecuteXmlReader(transaction, CommandType.StoredProcedure, spName);
	}

	public static void BeginExecuteNonQuery(SqlConnection connection, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		SqlCommand sqlCommand = new SqlCommand();
		PrepareCommand(sqlCommand, connection, null, commandType, commandText, commandParameters);
		sqlCommand.BeginExecuteNonQuery();
		sqlCommand.Parameters.Clear();
	}

	public static void BeginExecuteNonQuery(string connectionString, CommandType commandType, string commandText, params SqlParameter[] commandParameters)
	{
		using SqlConnection sqlConnection = new SqlConnection(connectionString);
		sqlConnection.Open();
		ExecuteNonQuery(sqlConnection, commandType, commandText, commandParameters);
	}

	public static void BeginExecuteNonQuery(string connectionString, string spName, params object[] parameterValues)
	{
		if (parameterValues != null && parameterValues.Length > 0)
		{
			SqlParameter[] spParameterSet = SqlHelperParameterCache.GetSpParameterSet(connectionString, spName);
			AssignParameterValues(spParameterSet, parameterValues);
			ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName, spParameterSet);
		}
		else
		{
			ExecuteNonQuery(connectionString, CommandType.StoredProcedure, spName);
		}
	}
}
