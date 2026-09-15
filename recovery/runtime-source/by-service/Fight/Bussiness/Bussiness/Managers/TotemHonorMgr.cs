using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class TotemHonorMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TotemHonorTemplateInfo> _totemHonorTemplate;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, TotemHonorTemplateInfo> totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
			if (Load(totemHonorTemplate))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_totemHonorTemplate = totemHonorTemplate;
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
				log.Error("ConsortiaLevelMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_totemHonorTemplate = new Dictionary<int, TotemHonorTemplateInfo>();
			rand = new ThreadSafeRandom();
			return Load(_totemHonorTemplate);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ConsortiaLevelMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<int, TotemHonorTemplateInfo> TotemHonorTemplate)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			TotemHonorTemplateInfo[] allTotemHonorTemplate = playerBussiness.GetAllTotemHonorTemplate();
			TotemHonorTemplateInfo[] array = allTotemHonorTemplate;
			foreach (TotemHonorTemplateInfo totemHonorTemplateInfo in array)
			{
				if (!TotemHonorTemplate.ContainsKey(totemHonorTemplateInfo.ID))
				{
					TotemHonorTemplate.Add(totemHonorTemplateInfo.ID, totemHonorTemplateInfo);
				}
			}
		}
		return true;
	}

	public static TotemHonorTemplateInfo FindTotemHonorTemplateInfo(int ID)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_totemHonorTemplate.ContainsKey(ID))
			{
				return _totemHonorTemplate[ID];
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
