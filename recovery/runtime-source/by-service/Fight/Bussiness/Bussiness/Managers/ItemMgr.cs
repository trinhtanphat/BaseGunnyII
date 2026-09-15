using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class ItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ItemTemplateInfo> _items;

	private static Dictionary<int, LoadUserBoxInfo> _timeBoxs;

	private static List<ItemTemplateInfo> Lists;

	private static ReaderWriterLock m_lock;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ItemTemplateInfo> dictionary = new Dictionary<int, ItemTemplateInfo>();
			Dictionary<int, LoadUserBoxInfo> dictionary2 = new Dictionary<int, LoadUserBoxInfo>();
			if (LoadItem(dictionary, dictionary2))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_items = dictionary;
					_timeBoxs = dictionary2;
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
			_items = new Dictionary<int, ItemTemplateInfo>();
			_timeBoxs = new Dictionary<int, LoadUserBoxInfo>();
			Lists = new List<ItemTemplateInfo>();
			return LoadItem(_items, _timeBoxs);
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

	public static bool LoadItem(Dictionary<int, ItemTemplateInfo> infos, Dictionary<int, LoadUserBoxInfo> userBoxs)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ItemTemplateInfo[] allGoods = produceBussiness.GetAllGoods();
			ItemTemplateInfo[] array = allGoods;
			foreach (ItemTemplateInfo itemTemplateInfo in array)
			{
				if (!infos.Keys.Contains(itemTemplateInfo.TemplateID))
				{
					infos.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
				}
			}
			LoadUserBoxInfo[] allTimeBoxAward = produceBussiness.GetAllTimeBoxAward();
			LoadUserBoxInfo[] array2 = allTimeBoxAward;
			foreach (LoadUserBoxInfo loadUserBoxInfo in array2)
			{
				if (!userBoxs.Keys.Contains(loadUserBoxInfo.ID))
				{
					userBoxs.Add(loadUserBoxInfo.ID, loadUserBoxInfo);
				}
			}
		}
		return true;
	}

	public static LoadUserBoxInfo FindItemBoxTypeAndLv(int type, int lv)
	{
		if (_timeBoxs == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (LoadUserBoxInfo value in _timeBoxs.Values)
			{
				if (value.Type == type && value.Level == lv)
				{
					return value;
				}
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static LoadUserBoxInfo FindItemBoxTemplate(int Id)
	{
		if (_timeBoxs == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_timeBoxs.Keys.Contains(Id))
			{
				return _timeBoxs[Id];
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static ItemTemplateInfo FindItemTemplate(int templateId)
	{
		if (_items == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_items.Keys.Contains(templateId))
			{
				return _items[templateId];
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static ItemTemplateInfo GetGoodsbyFusionTypeandQuality(int fusionType, int quality)
	{
		if (_items == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (ItemTemplateInfo value in _items.Values)
			{
				if (value.FusionType == fusionType && value.Quality == quality)
				{
					return value;
				}
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static ItemTemplateInfo GetGoodsbyFusionTypeandLevel(int fusionType, int level)
	{
		if (_items == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (ItemTemplateInfo value in _items.Values)
			{
				if (value.FusionType == fusionType && value.Level == level)
				{
					return value;
				}
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static List<ItemInfo> SpiltGoodsMaxCount(ItemInfo itemInfo)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < itemInfo.Count; i += itemInfo.Template.MaxCount)
		{
			int count = ((itemInfo.Count < itemInfo.Template.MaxCount) ? itemInfo.Count : itemInfo.Template.MaxCount);
			ItemInfo itemInfo2 = itemInfo.Clone();
			itemInfo2.Count = count;
			list.Add(itemInfo2);
		}
		return list;
	}
}
