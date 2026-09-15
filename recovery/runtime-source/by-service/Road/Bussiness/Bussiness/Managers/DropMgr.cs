using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness.Protocol;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class DropMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static string[] m_DropTypes = Enum.GetNames(typeof(eDropType));

	private static List<DropCondiction> m_dropcondiction = new List<DropCondiction>();

	private static Dictionary<int, List<DropItem>> m_dropitem = new Dictionary<int, List<DropItem>>();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			List<DropCondiction> value = LoadDropConditionDb();
			Interlocked.Exchange(ref m_dropcondiction, value);
			Dictionary<int, List<DropItem>> value2 = LoadDropItemDb();
			Interlocked.Exchange(ref m_dropitem, value2);
			return true;
		}
		catch (Exception exception)
		{
			log.Error("DropMgr", exception);
		}
		return false;
	}

	public static List<DropCondiction> LoadDropConditionDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetAllDropCondictions()?.ToList();
	}

	public static Dictionary<int, List<DropItem>> LoadDropItemDb()
	{
		Dictionary<int, List<DropItem>> dictionary = new Dictionary<int, List<DropItem>>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			DropItem[] allDropItems = produceBussiness.GetAllDropItems();
			foreach (DropCondiction info in m_dropcondiction)
			{
				IEnumerable<DropItem> source = allDropItems.Where((DropItem s) => s.DropId == info.DropId);
				dictionary.Add(info.DropId, source.ToList());
			}
		}
		return dictionary;
	}

	public static int FindCondiction(eDropType type, string para1, string para2)
	{
		string value = "," + para1 + ",";
		string value2 = "," + para2 + ",";
		foreach (DropCondiction item in m_dropcondiction)
		{
			if (item.CondictionType == (int)type && item.Para1.IndexOf(value) != -1 && item.Para2.IndexOf(value2) != -1)
			{
				return item.DropId;
			}
		}
		return 0;
	}

	public static List<DropItem> FindDropItem(int dropId)
	{
		if (m_dropitem.ContainsKey(dropId))
		{
			return m_dropitem[dropId];
		}
		return null;
	}
}
