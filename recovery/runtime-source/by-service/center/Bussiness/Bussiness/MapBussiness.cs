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
				MapInfo mapInfo = new MapInfo();
				mapInfo.BackMusic = ((ResultDataReader["BackMusic"] == null) ? "" : ResultDataReader["BackMusic"].ToString());
				mapInfo.BackPic = ((ResultDataReader["BackPic"] == null) ? "" : ResultDataReader["BackPic"].ToString());
				mapInfo.BackroundHeight = (int)ResultDataReader["BackroundHeight"];
				mapInfo.BackroundWidht = (int)ResultDataReader["BackroundWidht"];
				mapInfo.DeadHeight = (int)ResultDataReader["DeadHeight"];
				mapInfo.DeadPic = ((ResultDataReader["DeadPic"] == null) ? "" : ResultDataReader["DeadPic"].ToString());
				mapInfo.DeadWidth = (int)ResultDataReader["DeadWidth"];
				mapInfo.Description = ((ResultDataReader["Description"] == null) ? "" : ResultDataReader["Description"].ToString());
				mapInfo.DragIndex = (int)ResultDataReader["DragIndex"];
				mapInfo.ForegroundHeight = (int)ResultDataReader["ForegroundHeight"];
				mapInfo.ForegroundWidth = (int)ResultDataReader["ForegroundWidth"];
				mapInfo.ForePic = ((ResultDataReader["ForePic"] == null) ? "" : ResultDataReader["ForePic"].ToString());
				mapInfo.ID = (int)ResultDataReader["ID"];
				mapInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				mapInfo.Pic = ((ResultDataReader["Pic"] == null) ? "" : ResultDataReader["Pic"].ToString());
				mapInfo.Remark = ((ResultDataReader["Remark"] == null) ? "" : ResultDataReader["Remark"].ToString());
				mapInfo.Weight = (int)ResultDataReader["Weight"];
				mapInfo.PosX = ((ResultDataReader["PosX"] == null) ? "" : ResultDataReader["PosX"].ToString());
				mapInfo.PosX1 = ((ResultDataReader["PosX1"] == null) ? "" : ResultDataReader["PosX1"].ToString());
				mapInfo.Type = (byte)(int)ResultDataReader["Type"];
				list.Add(mapInfo);
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
				ServerMapInfo serverMapInfo = new ServerMapInfo();
				serverMapInfo.ServerID = (int)ResultDataReader["ServerID"];
				serverMapInfo.OpenMap = ResultDataReader["OpenMap"].ToString();
				serverMapInfo.IsSpecial = (int)ResultDataReader["IsSpecial"];
				list.Add(serverMapInfo);
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
