using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class QQTipsMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, QQtipsMessagesInfo> _qqtips;

	private static ReaderWriterLock m_lock;

	private static int qqtipSelectIndex = 0;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, QQtipsMessagesInfo> dictionary = new Dictionary<int, QQtipsMessagesInfo>();
			if (LoadItem(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_qqtips = dictionary;
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
			_qqtips = new Dictionary<int, QQtipsMessagesInfo>();
			return LoadItem(_qqtips);
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

	public static bool LoadItem(Dictionary<int, QQtipsMessagesInfo> infos)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			QQtipsMessagesInfo[] allQQtipsMessagesLoad = produceBussiness.GetAllQQtipsMessagesLoad();
			QQtipsMessagesInfo[] array = allQQtipsMessagesLoad;
			foreach (QQtipsMessagesInfo qQtipsMessagesInfo in array)
			{
				if (!infos.Keys.Contains(qQtipsMessagesInfo.ID))
				{
					infos.Add(qQtipsMessagesInfo.ID, qQtipsMessagesInfo);
				}
			}
		}
		return true;
	}

	public static QQtipsMessagesInfo GetQQtipsMessages()
	{
		if (_qqtips == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (qqtipSelectIndex >= _qqtips.Count)
			{
				qqtipSelectIndex = 1;
			}
			else
			{
				qqtipSelectIndex++;
			}
			return _qqtips[qqtipSelectIndex];
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
	}
}
