using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class FairBattleRewardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, FairBattleRewardInfo> _fairBattleRewards;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_fairBattleRewards = new Dictionary<int, FairBattleRewardInfo>();
			rand = new ThreadSafeRandom();
			return LoadFairBattleReward(_fairBattleRewards);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("FairBattleRewardMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, FairBattleRewardInfo> dictionary = new Dictionary<int, FairBattleRewardInfo>();
			if (LoadFairBattleReward(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_fairBattleRewards = dictionary;
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
				log.Error("FairBattleMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadFairBattleReward(Dictionary<int, FairBattleRewardInfo> Level)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			FairBattleRewardInfo[] allFairBattleReward = playerBussiness.GetAllFairBattleReward();
			FairBattleRewardInfo[] array = allFairBattleReward;
			foreach (FairBattleRewardInfo fairBattleRewardInfo in array)
			{
				if (!Level.ContainsKey(fairBattleRewardInfo.Level))
				{
					Level.Add(fairBattleRewardInfo.Level, fairBattleRewardInfo);
				}
			}
		}
		return true;
	}

	public static FairBattleRewardInfo FindLevel(int Level)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_fairBattleRewards.ContainsKey(Level))
			{
				return _fairBattleRewards[Level];
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

	public static FairBattleRewardInfo GetBattleDataByPrestige(int Prestige)
	{
		int count = _fairBattleRewards.Values.Count;
		for (int num = count - 1; num >= 0; num--)
		{
			if (Prestige >= _fairBattleRewards[num].Prestige)
			{
				return _fairBattleRewards[num];
			}
		}
		return null;
	}

	public static int MaxLevel()
	{
		if (_fairBattleRewards == null)
		{
			Init();
		}
		return _fairBattleRewards.Values.Count;
	}

	public static int GetLevel(int GP)
	{
		if (GP >= FindLevel(MaxLevel()).Prestige)
		{
			return MaxLevel();
		}
		for (int i = 1; i <= MaxLevel(); i++)
		{
			if (GP < FindLevel(i).Prestige)
			{
				if (i - 1 != 0)
				{
					return i - 1;
				}
				return 1;
			}
		}
		return 1;
	}

	public static int GetGP(int level)
	{
		if (MaxLevel() > level && level > 0)
		{
			return FindLevel(level - 1).Prestige;
		}
		return 0;
	}
}
