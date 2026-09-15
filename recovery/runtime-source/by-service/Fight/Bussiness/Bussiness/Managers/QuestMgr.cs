using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class QuestMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, QuestInfo> m_questinfo = new Dictionary<int, QuestInfo>();

	private static Dictionary<int, List<QuestConditionInfo>> m_questcondiction = new Dictionary<int, List<QuestConditionInfo>>();

	private static Dictionary<int, List<QuestAwardInfo>> m_questgoods = new Dictionary<int, List<QuestAwardInfo>>();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, QuestInfo> dictionary = LoadQuestInfoDb();
			Dictionary<int, List<QuestConditionInfo>> value = LoadQuestCondictionDb(dictionary);
			Dictionary<int, List<QuestAwardInfo>> value2 = LoadQuestGoodDb(dictionary);
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_questinfo, dictionary);
				Interlocked.Exchange(ref m_questcondiction, value);
				Interlocked.Exchange(ref m_questgoods, value2);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("QuestMgr", exception);
		}
		return false;
	}

	public static Dictionary<int, QuestInfo> LoadQuestInfoDb()
	{
		Dictionary<int, QuestInfo> dictionary = new Dictionary<int, QuestInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			QuestInfo[] aLlQuest = produceBussiness.GetALlQuest();
			QuestInfo[] array = aLlQuest;
			foreach (QuestInfo questInfo in array)
			{
				if (!dictionary.ContainsKey(questInfo.ID))
				{
					dictionary.Add(questInfo.ID, questInfo);
				}
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<QuestConditionInfo>> LoadQuestCondictionDb(Dictionary<int, QuestInfo> quests)
	{
		Dictionary<int, List<QuestConditionInfo>> dictionary = new Dictionary<int, List<QuestConditionInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			QuestConditionInfo[] allQuestCondiction = produceBussiness.GetAllQuestCondiction();
			foreach (QuestInfo quest in quests.Values)
			{
				IEnumerable<QuestConditionInfo> source = allQuestCondiction.Where((QuestConditionInfo s) => s.QuestID == quest.ID);
				dictionary.Add(quest.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static Dictionary<int, List<QuestAwardInfo>> LoadQuestGoodDb(Dictionary<int, QuestInfo> quests)
	{
		Dictionary<int, List<QuestAwardInfo>> dictionary = new Dictionary<int, List<QuestAwardInfo>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			QuestAwardInfo[] allQuestGoods = produceBussiness.GetAllQuestGoods();
			foreach (QuestInfo quest in quests.Values)
			{
				IEnumerable<QuestAwardInfo> source = allQuestGoods.Where((QuestAwardInfo s) => s.QuestID == quest.ID);
				dictionary.Add(quest.ID, source.ToList());
			}
		}
		return dictionary;
	}

	public static QuestInfo GetSingleQuest(int id)
	{
		if (m_questinfo.ContainsKey(id))
		{
			return m_questinfo[id];
		}
		return null;
	}

	public static List<QuestAwardInfo> GetQuestGoods(QuestInfo info)
	{
		if (m_questgoods.ContainsKey(info.ID))
		{
			return m_questgoods[info.ID];
		}
		return null;
	}

	public static List<QuestConditionInfo> GetQuestCondiction(QuestInfo info)
	{
		if (m_questcondiction.ContainsKey(info.ID))
		{
			return m_questcondiction[info.ID];
		}
		return null;
	}
}
