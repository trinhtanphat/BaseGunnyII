using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class FusionMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, FusionInfo> _fusions;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, FusionInfo> dictionary = new Dictionary<int, FusionInfo>();
			if (LoadFusion(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_fusions = dictionary;
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
				log.Error("FusionMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_fusions = new Dictionary<int, FusionInfo>();
			rand = new ThreadSafeRandom();
			return LoadFusion(_fusions);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("FusionMgr", exception);
			}
			return false;
		}
	}

	private static bool LoadFusion(Dictionary<int, FusionInfo> fusion)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			FusionInfo[] allFusion = produceBussiness.GetAllFusion();
			FusionInfo[] array = allFusion;
			foreach (FusionInfo fusionInfo in array)
			{
				if (!fusion.ContainsKey(fusionInfo.FusionID))
				{
					fusion.Add(fusionInfo.FusionID, fusionInfo);
				}
			}
		}
		return true;
	}

	public static FusionInfo FindItemFusion(int fusionId)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_fusions.ContainsKey(fusionId))
			{
				return _fusions[fusionId];
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
