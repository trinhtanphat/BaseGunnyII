using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class LightriddleQuestMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, LightriddleQuestInfo> _lightriddleQuests;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, LightriddleQuestInfo> dictionary = new Dictionary<int, LightriddleQuestInfo>();
			if (LoadData(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_lightriddleQuests = dictionary;
					return true;
				}
				catch
				{
				}
				finally
				{
					m_lock.ReleaseWriterLock();
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_lightriddleQuests = new Dictionary<int, LightriddleQuestInfo>();
			return LoadData(_lightriddleQuests);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Init", exception);
			}
			return false;
		}
	}

	public static bool LoadData(Dictionary<int, LightriddleQuestInfo> infos)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			LightriddleQuestInfo[] allLightriddleQuestInfo = playerBussiness.GetAllLightriddleQuestInfo();
			LightriddleQuestInfo[] array = allLightriddleQuestInfo;
			foreach (LightriddleQuestInfo lightriddleQuestInfo in array)
			{
				if (!infos.Keys.Contains(lightriddleQuestInfo.QuestionID))
				{
					infos.Add(lightriddleQuestInfo.QuestionID, lightriddleQuestInfo);
				}
			}
		}
		return true;
	}

	public static Dictionary<int, LightriddleQuestInfo> Get30LightriddleQuest()
	{
		if (_lightriddleQuests == null)
		{
			Init();
		}
		Dictionary<int, LightriddleQuestInfo> dictionary = new Dictionary<int, LightriddleQuestInfo>();
		Dictionary<int, LightriddleQuestInfo> dictionary2 = new Dictionary<int, LightriddleQuestInfo>();
		m_lock.AcquireReaderLock(-1);
		try
		{
			int count = _lightriddleQuests.Count;
			int num = 1;
			int num2 = 0;
			while (dictionary.Count < 30)
			{
				int key = random.Next(1, count);
				LightriddleQuestInfo lightriddleQuestInfo = _lightriddleQuests[key];
				if (!dictionary2.Keys.Contains(lightriddleQuestInfo.QuestionID))
				{
					dictionary.Add(num, lightriddleQuestInfo);
					dictionary2.Add(lightriddleQuestInfo.QuestionID, lightriddleQuestInfo);
					num++;
				}
				num2++;
			}
			return dictionary;
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
	}
}
