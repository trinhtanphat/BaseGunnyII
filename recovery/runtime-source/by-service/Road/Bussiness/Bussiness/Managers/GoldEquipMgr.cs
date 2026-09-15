using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class GoldEquipMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, GoldEquipTemplateLoadInfo> _items;

	private static ReaderWriterLock m_lock;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, GoldEquipTemplateLoadInfo> dictionary = new Dictionary<int, GoldEquipTemplateLoadInfo>();
			if (LoadItem(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_items = dictionary;
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
			_items = new Dictionary<int, GoldEquipTemplateLoadInfo>();
			return LoadItem(_items);
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

	public static bool LoadItem(Dictionary<int, GoldEquipTemplateLoadInfo> infos)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			GoldEquipTemplateLoadInfo[] allGoldEquipTemplateLoad = produceBussiness.GetAllGoldEquipTemplateLoad();
			GoldEquipTemplateLoadInfo[] array = allGoldEquipTemplateLoad;
			foreach (GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo in array)
			{
				if (!infos.Keys.Contains(goldEquipTemplateLoadInfo.ID))
				{
					infos.Add(goldEquipTemplateLoadInfo.ID, goldEquipTemplateLoadInfo);
				}
			}
		}
		return true;
	}

	public static GoldEquipTemplateLoadInfo FindGoldEquipCategoryID(int CategoryID)
	{
		if (_items == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (GoldEquipTemplateLoadInfo value in _items.Values)
			{
				if (value.CategoryID == CategoryID)
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

	public static GoldEquipTemplateLoadInfo FindGoldEquipNewTemplate(int TemplateId)
	{
		if (_items == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (GoldEquipTemplateLoadInfo value in _items.Values)
			{
				if (value.OldTemplateId == TemplateId)
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

	public static GoldEquipTemplateLoadInfo FindGoldEquipOldTemplate(int TemplateId)
	{
		if (_items == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (GoldEquipTemplateLoadInfo value in _items.Values)
			{
				if (value.NewTemplateId == TemplateId && value.OldTemplateId.ToString().Substring(4) != "4")
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
}
