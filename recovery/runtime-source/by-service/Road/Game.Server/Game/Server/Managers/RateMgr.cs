using System;
using System.Collections;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class RateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_lock = new ReaderWriterLock();

	private static ArrayList m_RateInfos = new ArrayList();

	public static bool Init(GameServerConfig config)
	{
		m_lock.AcquireWriterLock(-1);
		try
		{
			using (ServiceBussiness serviceBussiness = new ServiceBussiness())
			{
				m_RateInfos = serviceBussiness.GetRate(config.ServerID);
			}
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("RateMgr", exception);
			}
			return false;
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
	}

	public static bool ReLoad()
	{
		return Init(GameServer.Instance.Configuration);
	}

	public static float GetRate(eRateType eType)
	{
		float result = 1f;
		m_lock.AcquireReaderLock(-1);
		try
		{
			RateInfo rateInfoWithType = GetRateInfoWithType((int)eType);
			if (rateInfoWithType == null)
			{
				return result;
			}
			if (rateInfoWithType.Rate == 0f)
			{
				return 1f;
			}
			if (IsValid(rateInfoWithType))
			{
				result = rateInfoWithType.Rate;
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return result;
	}

	private static RateInfo GetRateInfoWithType(int type)
	{
		foreach (RateInfo rateInfo in m_RateInfos)
		{
			if (rateInfo.Type == type)
			{
				return rateInfo;
			}
		}
		return null;
	}

	private static bool IsValid(RateInfo _RateInfo)
	{
		DateTime beginDay = _RateInfo.BeginDay;
		DateTime endDay = _RateInfo.EndDay;
		return _RateInfo.BeginDay.Year <= DateTime.Now.Year && DateTime.Now.Year <= _RateInfo.EndDay.Year && _RateInfo.BeginDay.DayOfYear <= DateTime.Now.DayOfYear && DateTime.Now.DayOfYear <= _RateInfo.EndDay.DayOfYear && !(_RateInfo.BeginTime.TimeOfDay > DateTime.Now.TimeOfDay) && !(DateTime.Now.TimeOfDay > _RateInfo.EndTime.TimeOfDay);
	}
}
