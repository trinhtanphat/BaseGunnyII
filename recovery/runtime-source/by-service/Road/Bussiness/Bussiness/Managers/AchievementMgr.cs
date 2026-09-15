using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class AchievementMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, AchievementInfo> m_achievement = new Dictionary<int, AchievementInfo>();

	private static Dictionary<int, List<AchievementConditionInfo>> m_achievementCondition = new Dictionary<int, List<AchievementConditionInfo>>();

	private static Dictionary<int, List<AchievementRewardInfo>> m_achievementReward = new Dictionary<int, List<AchievementRewardInfo>>();

	private static Hashtable m_distinctCondition = new Hashtable();

	private static Dictionary<int, List<ItemRecordTypeInfo>> m_itemRecordType = new Dictionary<int, List<ItemRecordTypeInfo>>();

	private static Hashtable m_ItemRecordTypeInfo = new Hashtable();

	private static Dictionary<int, List<int>> m_recordLimit = new Dictionary<int, List<int>>();

	public static Dictionary<int, AchievementInfo> Achievement => m_achievement;

	public static Hashtable ItemRecordType => m_ItemRecordTypeInfo;

	public static List<AchievementConditionInfo> GetAchievementCondition(AchievementInfo info)
	{
		if (m_achievementCondition.ContainsKey(info.ID))
		{
			return m_achievementCondition[info.ID];
		}
		return null;
	}

	public static List<AchievementRewardInfo> GetAchievementReward(AchievementInfo info)
	{
		if (m_achievementReward.ContainsKey(info.ID))
		{
			return m_achievementReward[info.ID];
		}
		return null;
	}

	public static int GetNextLimit(int recordType, int recordValue)
	{
		if (m_recordLimit.ContainsKey(recordType))
		{
			foreach (int item in m_recordLimit[recordType])
			{
				if (item > recordValue)
				{
					return item;
				}
			}
			return int.MaxValue;
		}
		return int.MaxValue;
	}

	public static AchievementInfo GetSingleAchievement(int id)
	{
		if (m_achievement.ContainsKey(id))
		{
			return m_achievement[id];
		}
		return null;
	}

	public static bool Init()
	{
		return Reload();
	}

	public static Dictionary<int, List<AchievementConditionInfo>> LoadAchievementConditionInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
	{
		Dictionary<int, List<AchievementConditionInfo>> dictionary = new Dictionary<int, List<AchievementConditionInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			AchievementConditionInfo[] aLlAchievementCondition = produceBussiness.GetALlAchievementCondition();
			AchievementInfo achievementInfo;
			foreach (AchievementInfo value in achievementInfos.Values)
			{
				achievementInfo = value;
				IEnumerable<AchievementConditionInfo> enumerable = aLlAchievementCondition.Where((AchievementConditionInfo s) => s.AchievementID == achievementInfo.ID);
				dictionary.Add(achievementInfo.ID, enumerable.ToList());
				if (enumerable == null)
				{
					continue;
				}
				foreach (AchievementConditionInfo item in enumerable)
				{
					if (!m_distinctCondition.Contains(item.CondictionType))
					{
						m_distinctCondition.Add(item.CondictionType, item.CondictionType);
					}
				}
			}
			AchievementConditionInfo[] array = aLlAchievementCondition;
			foreach (AchievementConditionInfo achievementConditionInfo in array)
			{
				int condictionType = achievementConditionInfo.CondictionType;
				int condiction_Para = achievementConditionInfo.Condiction_Para2;
				if (!m_recordLimit.ContainsKey(condictionType))
				{
					m_recordLimit.Add(condictionType, new List<int>());
				}
				if (!m_recordLimit[condictionType].Contains(condiction_Para))
				{
					m_recordLimit[condictionType].Add(condiction_Para);
				}
			}
			foreach (int key in m_recordLimit.Keys)
			{
				m_recordLimit[key].Sort();
			}
		}
		return dictionary;
	}

	public static Dictionary<int, AchievementInfo> LoadAchievementInfoDB()
	{
		Dictionary<int, AchievementInfo> dictionary = new Dictionary<int, AchievementInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			AchievementInfo[] aLlAchievement = produceBussiness.GetALlAchievement();
			AchievementInfo[] array = aLlAchievement;
			foreach (AchievementInfo achievementInfo in array)
			{
				if (!dictionary.ContainsKey(achievementInfo.ID))
				{
					dictionary.Add(achievementInfo.ID, achievementInfo);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<AchievementRewardInfo>> LoadAchievementRewardInfoDB(Dictionary<int, AchievementInfo> achievementInfos)
	{
		Dictionary<int, List<AchievementRewardInfo>> dictionary = new Dictionary<int, List<AchievementRewardInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			AchievementRewardInfo[] aLlAchievementReward = produceBussiness.GetALlAchievementReward();
			AchievementInfo achievementInfo;
			foreach (AchievementInfo value in achievementInfos.Values)
			{
				achievementInfo = value;
				IEnumerable<AchievementRewardInfo> source = aLlAchievementReward.Where((AchievementRewardInfo s) => s.AchievementID == achievementInfo.ID);
				dictionary.Add(achievementInfo.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static void LoadItemRecordTypeInfoDB()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		ItemRecordTypeInfo[] allItemRecordType = produceBussiness.GetAllItemRecordType();
		ItemRecordTypeInfo[] array = allItemRecordType;
		foreach (ItemRecordTypeInfo itemRecordTypeInfo in array)
		{
			if (!m_ItemRecordTypeInfo.Contains(itemRecordTypeInfo.RecordID))
			{
				m_ItemRecordTypeInfo.Add(itemRecordTypeInfo.RecordID, itemRecordTypeInfo.Name);
			}
		}
	}

	public static bool Reload()
	{
		try
		{
			LoadItemRecordTypeInfoDB();
			Dictionary<int, AchievementInfo> dictionary = LoadAchievementInfoDB();
			Dictionary<int, List<AchievementConditionInfo>> value = LoadAchievementConditionInfoDB(dictionary);
			Dictionary<int, List<AchievementRewardInfo>> value2 = LoadAchievementRewardInfoDB(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_achievement, dictionary);
				Interlocked.Exchange(ref m_achievementCondition, value);
				Interlocked.Exchange(ref m_achievementReward, value2);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("AchievementMgr", exception);
		}
		return false;
	}
}
