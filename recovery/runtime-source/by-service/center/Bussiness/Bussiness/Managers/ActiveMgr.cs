using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class ActiveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static Dictionary<int, ActiveAwardInfo> m_ActiveAwardInfo = new Dictionary<int, ActiveAwardInfo>();

	public static Dictionary<int, List<ActiveConditionInfo>> m_ActiveConditionInfo = new Dictionary<int, List<ActiveConditionInfo>>();

	public static List<ActiveAwardInfo> GetAwardInfo(DateTime lastDate, int playerGrade)
	{
		string text = null;
		int num = (DateTime.Now - lastDate).Days;
		if (DateTime.Now.DayOfYear > lastDate.DayOfYear)
		{
			num++;
		}
		List<ActiveAwardInfo> list = new List<ActiveAwardInfo>();
		foreach (List<ActiveConditionInfo> value2 in m_ActiveConditionInfo.Values)
		{
			foreach (ActiveConditionInfo item in value2)
			{
				if (IsValid(item) && IsInGrade(item.LimitGrade, playerGrade) && item.Condition <= num)
				{
					text = item.AwardId;
					_ = item.ActiveID;
				}
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			string[] array = text.Split(',');
			string[] array2 = array;
			foreach (string value in array2)
			{
				if (!string.IsNullOrEmpty(value) && m_ActiveAwardInfo.ContainsKey(Convert.ToInt32(value)))
				{
					list.Add(m_ActiveAwardInfo[Convert.ToInt32(value)]);
				}
			}
		}
		return list;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	private static bool IsInGrade(string limitGrade, int playerGrade)
	{
		bool result = false;
		int num = 0;
		int num2 = 0;
		if (limitGrade != null)
		{
			string[] array = limitGrade.Split('-');
			if (array.Length == 2)
			{
				num = Convert.ToInt32(array[0]);
				num2 = Convert.ToInt32(array[1]);
			}
			if (num <= playerGrade && num2 >= playerGrade)
			{
				result = true;
			}
		}
		return result;
	}

	public static bool IsValid(ActiveConditionInfo info)
	{
		_ = info.StartTime;
		_ = info.EndTime;
		if (info.StartTime.Ticks > DateTime.Now.Ticks || info.EndTime.Ticks < DateTime.Now.Ticks)
		{
			return false;
		}
		return true;
	}

	public static Dictionary<int, ActiveAwardInfo> LoadActiveAwardDb(Dictionary<int, List<ActiveConditionInfo>> conditions)
	{
		Dictionary<int, ActiveAwardInfo> dictionary = new Dictionary<int, ActiveAwardInfo>();
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		ActiveAwardInfo[] allActiveAwardInfo = produceBussiness.GetAllActiveAwardInfo();
		foreach (int key in conditions.Keys)
		{
			ActiveAwardInfo[] array = allActiveAwardInfo;
			foreach (ActiveAwardInfo activeAwardInfo in array)
			{
				if (key == activeAwardInfo.ActiveID && !dictionary.ContainsKey(activeAwardInfo.ID))
				{
					dictionary.Add(activeAwardInfo.ID, activeAwardInfo);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<ActiveConditionInfo>> LoadActiveConditionDb()
	{
		Dictionary<int, List<ActiveConditionInfo>> dictionary = new Dictionary<int, List<ActiveConditionInfo>>();
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		ActiveConditionInfo[] allActiveConditionInfo = produceBussiness.GetAllActiveConditionInfo();
		ActiveConditionInfo[] array = allActiveConditionInfo;
		foreach (ActiveConditionInfo activeConditionInfo in array)
		{
			List<ActiveConditionInfo> list = new List<ActiveConditionInfo>();
			if (!dictionary.ContainsKey(activeConditionInfo.ActiveID))
			{
				list.Add(activeConditionInfo);
				dictionary.Add(activeConditionInfo.ActiveID, list);
			}
			else
			{
				dictionary[activeConditionInfo.ActiveID].Add(activeConditionInfo);
			}
		}
		return dictionary;
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, List<ActiveConditionInfo>> dictionary = LoadActiveConditionDb();
			Dictionary<int, ActiveAwardInfo> value = LoadActiveAwardDb(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_ActiveConditionInfo, dictionary);
				Interlocked.Exchange(ref m_ActiveAwardInfo, value);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("QuestMgr", exception);
		}
		return false;
	}
}
