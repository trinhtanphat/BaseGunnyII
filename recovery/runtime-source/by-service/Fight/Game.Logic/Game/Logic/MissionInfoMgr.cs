using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public static class MissionInfoMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, MissionInfo> m_missionInfos = new Dictionary<int, MissionInfo>();

	private static ReaderWriterLock m_lock = new ReaderWriterLock();

	private static ThreadSafeRandom m_rand = new ThreadSafeRandom();

	public static bool Init()
	{
		return Reload();
	}

	public static bool Reload()
	{
		try
		{
			Dictionary<int, MissionInfo> dictionary = LoadFromDatabase();
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_missionInfos, dictionary);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("MissionInfoMgr", exception);
		}
		return false;
	}

	private static Dictionary<int, MissionInfo> LoadFromDatabase()
	{
		Dictionary<int, MissionInfo> dictionary = new Dictionary<int, MissionInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			MissionInfo[] allMissionInfo = produceBussiness.GetAllMissionInfo();
			MissionInfo[] array = allMissionInfo;
			foreach (MissionInfo missionInfo in array)
			{
				if (!dictionary.ContainsKey(missionInfo.Id))
				{
					dictionary.Add(missionInfo.Id, missionInfo);
				}
			}
		}
		return dictionary;
	}

	public static MissionInfo GetMissionInfo(int id)
	{
		if (m_missionInfos.ContainsKey(id))
		{
			return m_missionInfos[id];
		}
		return null;
	}
}
