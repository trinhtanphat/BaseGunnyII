using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class MapBussiness : BaseBussiness
{
	public MapInfo[] GetAllMap()
	{
		List<MapInfo> list = new List<MapInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Maps_All");
			while (ResultDataReader.Read())
			{
				list.Add(new MapInfo
				{
					BackMusic = ((ResultDataReader["BackMusic"] == null) ? "" : ResultDataReader["BackMusic"].ToString()),
					BackPic = ((ResultDataReader["BackPic"] == null) ? "" : ResultDataReader["BackPic"].ToString()),
					BackroundHeight = (int)ResultDataReader["BackroundHeight"],
					BackroundWidht = (int)ResultDataReader["BackroundWidht"],
					DeadHeight = (int)ResultDataReader["DeadHeight"],
					DeadPic = ((ResultDataReader["DeadPic"] == null) ? "" : ResultDataReader["DeadPic"].ToString()),
					DeadWidth = (int)ResultDataReader["DeadWidth"],
					Description = ((ResultDataReader["Description"] == null) ? "" : ResultDataReader["Description"].ToString()),
					DragIndex = (int)ResultDataReader["DragIndex"],
					ForegroundHeight = (int)ResultDataReader["ForegroundHeight"],
					ForegroundWidth = (int)ResultDataReader["ForegroundWidth"],
					ForePic = ((ResultDataReader["ForePic"] == null) ? "" : ResultDataReader["ForePic"].ToString()),
					ID = (int)ResultDataReader["ID"],
					Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString()),
					Pic = ((ResultDataReader["Pic"] == null) ? "" : ResultDataReader["Pic"].ToString()),
					Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString()),
					Weight = (int)ResultDataReader["Weight"],
					PosX = ((ResultDataReader["PosX"] == null) ? "" : ResultDataReader["PosX"].ToString()),
					PosX1 = ((ResultDataReader["PosX1"] == null) ? "" : ResultDataReader["PosX1"].ToString()),
					Type = (byte)(int)ResultDataReader["Type"]
				});
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllMap", exception);
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

	public ServerMapInfo[] GetAllServerMap()
	{
		List<ServerMapInfo> list = new List<ServerMapInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_Maps_Server_All");
			while (ResultDataReader.Read())
			{
				list.Add(new ServerMapInfo
				{
					ServerID = (int)ResultDataReader["ServerID"],
					OpenMap = ResultDataReader["OpenMap"].ToString(),
					IsSpecial = (int)ResultDataReader["IsSpecial"]
				});
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllMapWeek", exception);
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
