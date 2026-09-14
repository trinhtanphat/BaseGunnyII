using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class ConsortiaExtraMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ConsortiaLevelInfo> _consortiaLevel;

	private static Dictionary<int, ConsortiaBuffTempInfo> _consortiaBuffTemp;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ConsortiaLevelInfo> consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
			Dictionary<int, ConsortiaBuffTempInfo> consortiaBuffTemp = new Dictionary<int, ConsortiaBuffTempInfo>();
			if (Load(consortiaLevel, consortiaBuffTemp))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_consortiaLevel = consortiaLevel;
					_consortiaBuffTemp = consortiaBuffTemp;
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
			_consortiaLevel = new Dictionary<int, ConsortiaLevelInfo>();
			_consortiaBuffTemp = new Dictionary<int, ConsortiaBuffTempInfo>();
			rand = new ThreadSafeRandom();
			return Load(_consortiaLevel, _consortiaBuffTemp);
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

	private static bool Load(Dictionary<int, ConsortiaLevelInfo> consortiaLevel, Dictionary<int, ConsortiaBuffTempInfo> consortiaBuffTemp)
	{
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaLevelInfo[] allConsortiaLevel = consortiaBussiness.GetAllConsortiaLevel();
			ConsortiaLevelInfo[] array = allConsortiaLevel;
			foreach (ConsortiaLevelInfo consortiaLevelInfo in array)
			{
				if (!consortiaLevel.ContainsKey(consortiaLevelInfo.Level))
				{
					consortiaLevel.Add(consortiaLevelInfo.Level, consortiaLevelInfo);
				}
			}
			ConsortiaBuffTempInfo[] allConsortiaBuffTemp = consortiaBussiness.GetAllConsortiaBuffTemp();
			ConsortiaBuffTempInfo[] array2 = allConsortiaBuffTemp;
			foreach (ConsortiaBuffTempInfo consortiaBuffTempInfo in array2)
			{
				if (!consortiaBuffTemp.ContainsKey(consortiaBuffTempInfo.id))
				{
					consortiaBuffTemp.Add(consortiaBuffTempInfo.id, consortiaBuffTempInfo);
				}
			}
		}
		return true;
	}

	public static ConsortiaLevelInfo FindConsortiaLevelInfo(int level)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_consortiaLevel.ContainsKey(level))
			{
				return _consortiaLevel[level];
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

	public static ConsortiaBuffTempInfo FindConsortiaBuffInfo(int id)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_consortiaBuffTemp.ContainsKey(id))
			{
				return _consortiaBuffTemp[id];
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

	public static List<ConsortiaBuffTempInfo> GetAllConsortiaBuff()
	{
		m_lock.AcquireReaderLock(-1);
		List<ConsortiaBuffTempInfo> list = new List<ConsortiaBuffTempInfo>();
		try
		{
			foreach (ConsortiaBuffTempInfo value in _consortiaBuffTemp.Values)
			{
				list.Add(value);
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return list;
	}
}
