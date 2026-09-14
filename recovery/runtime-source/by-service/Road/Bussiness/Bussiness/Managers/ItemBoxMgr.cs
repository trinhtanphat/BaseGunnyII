using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class ItemBoxMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ItemBoxInfo[] m_itemBox;

	private static Dictionary<int, List<ItemBoxInfo>> m_itemBoxs;

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool ReLoad()
	{
		try
		{
			ItemBoxInfo[] array = LoadItemBoxDb();
			Dictionary<int, List<ItemBoxInfo>> value = LoadItemBoxs(array);
			if (array != null)
			{
				Interlocked.Exchange(ref m_itemBox, array);
				Interlocked.Exchange(ref m_itemBoxs, value);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static ItemBoxInfo[] LoadItemBoxDb()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		return produceBussiness.GetItemBoxInfos();
	}

	public static Dictionary<int, List<ItemBoxInfo>> LoadItemBoxs(ItemBoxInfo[] itemBoxs)
	{
		Dictionary<int, List<ItemBoxInfo>> dictionary = new Dictionary<int, List<ItemBoxInfo>>();
		foreach (ItemBoxInfo info in itemBoxs)
		{
			if (!dictionary.Keys.Contains(info.DataId))
			{
				IEnumerable<ItemBoxInfo> source = itemBoxs.Where((ItemBoxInfo s) => s.DataId == info.DataId);
				dictionary.Add(info.DataId, source.ToList());
			}
		}
		return dictionary;
	}

	public static List<ItemBoxInfo> FindItemBox(int DataId)
	{
		if (m_itemBoxs.ContainsKey(DataId))
		{
			return m_itemBoxs[DataId];
		}
		return null;
	}

	public static List<ItemInfo> GetAllItemBoxAward(int DataId)
	{
		List<ItemBoxInfo> list = FindItemBox(DataId);
		List<ItemInfo> list2 = new List<ItemInfo>();
		foreach (ItemBoxInfo item in list)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item.TemplateId), item.ItemCount, 105);
			itemInfo.IsBinds = item.IsBind;
			itemInfo.ValidDate = item.ItemValid;
			list2.Add(itemInfo);
		}
		return list2;
	}

	public static ItemBoxInfo FindSpecialItemBox(int DataId)
	{
		ItemBoxInfo itemBoxInfo = new ItemBoxInfo();
		switch (DataId)
		{
		case -300:
			itemBoxInfo.TemplateId = 11420;
			itemBoxInfo.ItemCount = 1;
			break;
		case -1100:
			itemBoxInfo.TemplateId = 11213;
			itemBoxInfo.ItemCount = 1;
			break;
		case 11408:
			itemBoxInfo.TemplateId = 11420;
			itemBoxInfo.ItemCount = 1;
			break;
		case -100:
			itemBoxInfo.TemplateId = 11233;
			itemBoxInfo.ItemCount = 1;
			break;
		case -200:
			itemBoxInfo.TemplateId = 112244;
			itemBoxInfo.ItemCount = 1;
			break;
		}
		return itemBoxInfo;
	}

	public static bool CreateItemBox(int DateId, List<ItemInfo> itemInfos, ref int gold, ref int point, ref int giftToken, ref int medal, ref int exp, ref int honor, ref int hardCurrency, ref int leagueMoney, ref int useableScore, ref int prestge)
	{
		List<ItemBoxInfo> list = new List<ItemBoxInfo>();
		List<ItemBoxInfo> list2 = FindItemBox(DateId);
		if (list2 == null)
		{
			return false;
		}
		list = list2.Where((ItemBoxInfo s) => s.IsSelect).ToList();
		int num = 1;
		int maxRound = 0;
		if (list.Count < list2.Count)
		{
			maxRound = ThreadSafeRandom.NextStatic((from s in list2
				where !s.IsSelect
				select s.Random).Max());
		}
		List<ItemBoxInfo> list3 = list2.Where((ItemBoxInfo s) => !s.IsSelect && s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				ItemBoxInfo item = list3[index];
				if (list == null)
				{
					list = new List<ItemBoxInfo>();
				}
				list.Add(item);
			}
		}
		foreach (ItemBoxInfo item2 in list)
		{
			if (item2 == null)
			{
				return false;
			}
			switch (item2.TemplateId)
			{
			case -1300:
				prestge += item2.ItemCount;
				continue;
			case -1200:
				useableScore += item2.ItemCount;
				continue;
			case -1100:
				giftToken += item2.ItemCount;
				continue;
			case -1000:
				leagueMoney += item2.ItemCount;
				continue;
			case -900:
				hardCurrency += item2.ItemCount;
				continue;
			case -800:
				honor += item2.ItemCount;
				continue;
			case -300:
				medal += item2.ItemCount;
				continue;
			case -200:
				point += item2.ItemCount;
				continue;
			case -100:
				gold += item2.ItemCount;
				continue;
			case 11107:
				exp += item2.ItemCount;
				continue;
			}
			ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item2.TemplateId);
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, item2.ItemCount, 101);
			if (itemInfo != null)
			{
				itemInfo.Count = item2.ItemCount;
				itemInfo.IsBinds = item2.IsBind;
				itemInfo.ValidDate = item2.ItemValid;
				itemInfo.StrengthenLevel = item2.StrengthenLevel;
				itemInfo.AttackCompose = item2.AttackCompose;
				itemInfo.DefendCompose = item2.DefendCompose;
				itemInfo.AgilityCompose = item2.AgilityCompose;
				itemInfo.LuckCompose = item2.LuckCompose;
				itemInfo.IsTips = item2.IsTips != 0;
				itemInfo.IsLogs = item2.IsLogs;
				if (itemInfos == null)
				{
					itemInfos = new List<ItemInfo>();
				}
				itemInfos.Add(itemInfo);
			}
		}
		return true;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = random.Next(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}
}
