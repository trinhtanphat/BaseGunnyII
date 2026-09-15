using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class PropItemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	private static ReaderWriterLock m_lock;

	private static Dictionary<int, ItemTemplateInfo> _allProp;

	private static int[] PropBag = new int[8] { 10001, 10002, 10003, 10004, 10005, 10006, 10007, 10008 };

	public static bool Reload()
	{
		try
		{
			Dictionary<int, ItemTemplateInfo> allProp = new Dictionary<int, ItemTemplateInfo>();
			if (LoadProps(allProp))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_allProp = allProp;
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
				log.Error("ReloadProps", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_allProp = new Dictionary<int, ItemTemplateInfo>();
			return LoadProps(_allProp);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("InitProps", exception);
			}
			return false;
		}
	}

	private static bool LoadProps(Dictionary<int, ItemTemplateInfo> allProp)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ItemTemplateInfo[] singleCategory = produceBussiness.GetSingleCategory(10);
			ItemTemplateInfo[] array = singleCategory;
			foreach (ItemTemplateInfo itemTemplateInfo in array)
			{
				allProp.Add(itemTemplateInfo.TemplateID, itemTemplateInfo);
			}
		}
		return true;
	}

	public static ItemTemplateInfo FindAllProp(int id)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_allProp.ContainsKey(id))
			{
				return _allProp[id];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static ItemTemplateInfo FindFightingProp(int id)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (!PropBag.Contains(id))
			{
				return null;
			}
			if (_allProp.ContainsKey(id))
			{
				return _allProp[id];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}
}
