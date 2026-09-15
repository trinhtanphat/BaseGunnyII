using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;

namespace SqlDataProvider.BaseClass;

public sealed class Sql_DbObject : IDisposable
{
	private SqlConnection _SqlConnection;

	private SqlCommand _SqlCommand;

	private SqlDataAdapter _SqlDataAdapter;

	public Sql_DbObject()
	{
		_SqlConnection = new SqlConnection();
	}

	public Sql_DbObject(string Path_Source, string Conn_DB)
	{
		switch (Path_Source)
		{
		case "WebConfig":
			_SqlConnection = new SqlConnection(ConfigurationManager.ConnectionStrings[Conn_DB].ConnectionString);
			break;
		case "File":
			_SqlConnection = new SqlConnection(Conn_DB);
			break;
		case "AppConfig":
			_SqlConnection = new SqlConnection(ConfigurationManager.AppSettings[Conn_DB]);
			break;
		default:
			_SqlConnection = new SqlConnection(Conn_DB);
			break;
		}
	}

	private static bool OpenConnection(SqlConnection _SqlConnection)
	{
		try
		{
			if (_SqlConnection.State != ConnectionState.Open)
			{
				_SqlConnection.Open();
				return true;
			}
			return true;
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("打开数据库连接错误:" + ex.Message.Trim());
			return false;
		}
	}

	public bool Exesqlcomm(string Sqlcomm)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.CommandType = CommandType.Text;
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandText = Sqlcomm;
			_SqlCommand.ExecuteNonQuery();
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public int GetRecordCount(string Sqlcomm)
	{
		int result = 0;
		if (!OpenConnection(_SqlConnection))
		{
			result = 0;
		}
		else
		{
			try
			{
				_SqlCommand = new SqlCommand();
				_SqlCommand.Connection = _SqlConnection;
				_SqlCommand.CommandType = CommandType.Text;
				_SqlCommand.CommandText = Sqlcomm;
				result = ((_SqlCommand.ExecuteScalar() != null) ? ((int)_SqlCommand.ExecuteScalar()) : 0);
			}
			catch (SqlException ex)
			{
				ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
			}
			finally
			{
				_SqlConnection.Close();
				Dispose(disposing: true);
			}
		}
		return result;
	}

	public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm)
	{
		DataTable dataTable = new DataTable(TableName);
		if (!OpenConnection(_SqlConnection))
		{
			return dataTable;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.Text;
			_SqlCommand.CommandText = Sqlcomm;
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(dataTable);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataTable;
	}

	public DataSet GetDataSetBySqlcomm(string TableName, string Sqlcomm)
	{
		DataSet dataSet = new DataSet();
		if (!OpenConnection(_SqlConnection))
		{
			return dataSet;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.Text;
			_SqlCommand.CommandText = Sqlcomm;
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(dataSet);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行Sql语句：" + Sqlcomm + "错误信息为：" + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataSet;
	}

	public bool FillSqlDataReader(ref SqlDataReader Sdr, string SqlComm)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.Text;
			_SqlCommand.CommandText = SqlComm;
			Sdr = _SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
			return true;
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行Sql语句：" + SqlComm + "错误信息为：" + ex.Message.Trim());
		}
		finally
		{
			Dispose(disposing: true);
		}
		return false;
	}

	public DataTable GetDataTableBySqlcomm(string TableName, string Sqlcomm, int StartRecordNo, int PageSize)
	{
		DataTable dataTable = new DataTable(TableName);
		if (!OpenConnection(_SqlConnection))
		{
			dataTable.Dispose();
			Dispose(disposing: true);
			return dataTable;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.Text;
			_SqlCommand.CommandText = Sqlcomm;
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(new DataSet
			{
				Tables = { dataTable }
			}, StartRecordNo, PageSize, TableName);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行sql语句: " + Sqlcomm + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataTable;
	}

	public bool RunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlCommand.ExecuteNonQuery();
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public bool RunProcedure(string ProcedureName)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			_SqlCommand.ExecuteNonQuery();
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			ResultDataReader = _SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		return true;
	}

	public bool GetReader(ref SqlDataReader ResultDataReader, string ProcedureName, SqlParameter[] SqlParameters)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			ResultDataReader = _SqlCommand.ExecuteReader(CommandBehavior.CloseConnection);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		return true;
	}

	public DataSet GetDataSet(string ProcedureName, SqlParameter[] SqlParameters)
	{
		DataSet dataSet = new DataSet();
		if (!OpenConnection(_SqlConnection))
		{
			dataSet.Dispose();
			return dataSet;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(dataSet);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错信信息为：" + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataSet;
	}

	public bool GetDataSet(ref DataSet ResultDataSet, ref int row_total, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			row_total = 0;
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			DataSet dataSet = new DataSet();
			row_total = _SqlDataAdapter.Fill(dataSet);
			_SqlDataAdapter.Fill(ResultDataSet, StartRecordNo, PageSize, TableName);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错误信息为：" + ex.Message.Trim());
			return false;
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public DataSet GetDateSet(string DatesetName, string ProcedureName, SqlParameter[] SqlParameters)
	{
		DataSet dataSet = new DataSet(DatesetName);
		if (!OpenConnection(_SqlConnection))
		{
			dataSet.Dispose();
			return dataSet;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(dataSet);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程：" + ProcedureName + "错信信息为：" + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataSet;
	}

	public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters)
	{
		return GetDataTable(TableName, ProcedureName, SqlParameters, -1);
	}

	public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int commandTimeout)
	{
		DataTable dataTable = new DataTable(TableName);
		if (!OpenConnection(_SqlConnection))
		{
			dataTable.Dispose();
			Dispose(disposing: true);
			return dataTable;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			if (commandTimeout >= 0)
			{
				_SqlCommand.CommandTimeout = commandTimeout;
			}
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(dataTable);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataTable;
	}

	public DataTable GetDataTable(string TableName, string ProcedureName)
	{
		DataTable dataTable = new DataTable(TableName);
		if (!OpenConnection(_SqlConnection))
		{
			dataTable.Dispose();
			Dispose(disposing: true);
			return dataTable;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(dataTable);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataTable;
	}

	public DataTable GetDataTable(string TableName, string ProcedureName, int StartRecordNo, int PageSize)
	{
		DataTable dataTable = new DataTable(TableName);
		if (!OpenConnection(_SqlConnection))
		{
			dataTable.Dispose();
			Dispose(disposing: true);
			return dataTable;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(new DataSet
			{
				Tables = { dataTable }
			}, StartRecordNo, PageSize, TableName);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataTable;
	}

	public DataTable GetDataTable(string TableName, string ProcedureName, SqlParameter[] SqlParameters, int StartRecordNo, int PageSize)
	{
		DataTable dataTable = new DataTable(TableName);
		if (!OpenConnection(_SqlConnection))
		{
			dataTable.Dispose();
			Dispose(disposing: true);
			return dataTable;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			_SqlDataAdapter.Fill(new DataSet
			{
				Tables = { dataTable }
			}, StartRecordNo, PageSize, TableName);
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return dataTable;
	}

	public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize)
	{
		ResultTable = null;
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(ResultTable);
			_SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
			ResultTable = dataSet.Tables[TableName];
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public bool GetDataTable(ref DataTable ResultTable, string TableName, string ProcedureName, int StartRecordNo, int PageSize, SqlParameter[] SqlParameters)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlDataAdapter = new SqlDataAdapter();
			_SqlDataAdapter.SelectCommand = _SqlCommand;
			DataSet dataSet = new DataSet();
			dataSet.Tables.Add(ResultTable);
			_SqlDataAdapter.Fill(dataSet, StartRecordNo, PageSize, TableName);
			ResultTable = dataSet.Tables[TableName];
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
			return false;
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return true;
	}

	public void Dispose()
	{
		Dispose(disposing: true);
		GC.SuppressFinalize(true);
	}

	private void Dispose(bool disposing)
	{
		if (!disposing || _SqlDataAdapter == null)
		{
			return;
		}
		if (_SqlDataAdapter.SelectCommand != null)
		{
			if (_SqlCommand.Connection != null)
			{
				_SqlDataAdapter.SelectCommand.Connection.Dispose();
			}
			_SqlDataAdapter.SelectCommand.Dispose();
		}
		_SqlDataAdapter.Dispose();
		_SqlDataAdapter = null;
	}

	public void BeginRunProcedure(string ProcedureName, SqlParameter[] SqlParameters)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return;
		}
		try
		{
			_SqlCommand = new SqlCommand();
			_SqlCommand.Connection = _SqlConnection;
			_SqlCommand.CommandType = CommandType.StoredProcedure;
			_SqlCommand.CommandText = ProcedureName;
			foreach (SqlParameter value in SqlParameters)
			{
				_SqlCommand.Parameters.Add(value);
			}
			_SqlCommand.BeginExecuteNonQuery();
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行存储过程: " + ProcedureName + "错误信息为: " + ex.Message.Trim());
		}
		finally
		{
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
	}

	public bool SetDataTable(DataTable dataTable, string desTableName, List<SqlBulkCopyColumnMapping> mapping)
	{
		if (!OpenConnection(_SqlConnection))
		{
			return false;
		}
		SqlBulkCopy sqlBulkCopy = null;
		bool result;
		try
		{
			sqlBulkCopy = new SqlBulkCopy(_SqlConnection, SqlBulkCopyOptions.CheckConstraints | SqlBulkCopyOptions.UseInternalTransaction, null);
			sqlBulkCopy.DestinationTableName = desTableName;
			sqlBulkCopy.NotifyAfter = dataTable.Rows.Count;
			if (mapping != null)
			{
				foreach (SqlBulkCopyColumnMapping item in mapping)
				{
					sqlBulkCopy.ColumnMappings.Add(item);
				}
			}
			sqlBulkCopy.WriteToServer(dataTable);
			result = true;
		}
		catch (SqlException ex)
		{
			ApplicationLog.WriteError("执行SqlBulk写入时发生错误，错误信息为: " + ex.Message.Trim());
			result = false;
		}
		finally
		{
			sqlBulkCopy?.Close();
			_SqlConnection.Close();
			Dispose(disposing: true);
		}
		return result;
	}
}
