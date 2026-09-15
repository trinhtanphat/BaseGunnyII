using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class RankMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_lock = new ReaderWriterLock();

	private static Dictionary<int, UserMatchInfo> _matchs;

	protected static Timer _timer;

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_matchs = new Dictionary<int, UserMatchInfo>();
			BeginTimer();
			return ReLoad();
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("RankMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, UserMatchInfo> dictionary = new Dictionary<int, UserMatchInfo>();
			if (LoadData(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_matchs = dictionary;
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
				log.Error("RankMgr", exception);
			}
		}
		return false;
	}

	public static UserMatchInfo FindRank(int UserID)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_matchs.ContainsKey(UserID))
			{
				return _matchs[UserID];
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

	private static bool LoadData(Dictionary<int, UserMatchInfo> Match)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			UserMatchInfo[] allUserMatchInfo = playerBussiness.GetAllUserMatchInfo();
			UserMatchInfo[] array = allUserMatchInfo;
			foreach (UserMatchInfo userMatchInfo in array)
			{
				if (!Match.ContainsKey(userMatchInfo.UserID))
				{
					Match.Add(userMatchInfo.UserID, userMatchInfo);
				}
			}
		}
		return true;
	}

	public static void BeginTimer()
	{
		int num = 3600000;
		if (_timer == null)
		{
			_timer = new Timer(TimeCheck, null, num, num);
		}
		else
		{
			_timer.Change(num, num);
		}
	}

	protected static void TimeCheck(object sender)
	{
		try
		{
			int tickCount = Environment.TickCount;
			ThreadPriority priority = Thread.CurrentThread.Priority;
			Thread.CurrentThread.Priority = ThreadPriority.Lowest;
			ReLoad();
			Thread.CurrentThread.Priority = priority;
			tickCount = Environment.TickCount - tickCount;
		}
		catch (Exception ex)
		{
			Console.WriteLine("TimeCheck Rank: " + ex);
		}
	}

	public void StopTimer()
	{
		if (_timer != null)
		{
			_timer.Dispose();
			_timer = null;
		}
	}
}
