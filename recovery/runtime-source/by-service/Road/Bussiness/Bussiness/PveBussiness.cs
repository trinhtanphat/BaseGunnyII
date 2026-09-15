using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using SqlDataProvider.Data;

namespace Bussiness;

public class PveBussiness : BaseBussiness
{
	public PveInfo[] GetAllPveInfos()
	{
		List<PveInfo> list = new List<PveInfo>();
		SqlDataReader ResultDataReader = null;
		try
		{
			db.GetReader(ref ResultDataReader, "SP_PveInfos_All");
			while (ResultDataReader.Read())
			{
				PveInfo pveInfo = new PveInfo();
				pveInfo.ID = (int)ResultDataReader["Id"];
				pveInfo.Name = ((ResultDataReader["Name"] == null) ? "" : ResultDataReader["Name"].ToString());
				pveInfo.Type = (int)ResultDataReader["Type"];
				pveInfo.LevelLimits = (int)ResultDataReader["LevelLimits"];
				pveInfo.SimpleTemplateIds = ((ResultDataReader["SimpleTemplateIds"] == null) ? "" : ResultDataReader["SimpleTemplateIds"].ToString());
				pveInfo.NormalTemplateIds = ((ResultDataReader["NormalTemplateIds"] == null) ? "" : ResultDataReader["NormalTemplateIds"].ToString());
				pveInfo.HardTemplateIds = ((ResultDataReader["HardTemplateIds"] == null) ? "" : ResultDataReader["HardTemplateIds"].ToString());
				pveInfo.TerrorTemplateIds = ((ResultDataReader["TerrorTemplateIds"] == null) ? "" : ResultDataReader["TerrorTemplateIds"].ToString());
				pveInfo.Pic = ((ResultDataReader["Pic"] == null) ? "" : ResultDataReader["Pic"].ToString());
				pveInfo.Description = ((ResultDataReader["Description"] == null) ? "" : ResultDataReader["Description"].ToString());
				pveInfo.Ordering = (int)ResultDataReader["Ordering"];
				pveInfo.AdviceTips = ((ResultDataReader["AdviceTips"] == null) ? "" : ResultDataReader["AdviceTips"].ToString());
				pveInfo.SimpleGameScript = ResultDataReader["SimpleGameScript"] as string;
				pveInfo.NormalGameScript = ResultDataReader["NormalGameScript"] as string;
				pveInfo.HardGameScript = ResultDataReader["HardGameScript"] as string;
				pveInfo.TerrorGameScript = ResultDataReader["TerrorGameScript"] as string;
				pveInfo.EpicTemplateIds = ((ResultDataReader["EpicTemplateIds"] == null) ? "" : ResultDataReader["EpicTemplateIds"].ToString());
				pveInfo.EpicGameScript = ResultDataReader["EpicGameScript"] as string;
				PveInfo item = pveInfo;
				list.Add(item);
			}
		}
		catch (Exception exception)
		{
			if (BaseBussiness.log.IsErrorEnabled)
			{
				BaseBussiness.log.Error("GetAllPveInfos", exception);
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
