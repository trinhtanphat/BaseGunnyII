using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class FightSpiritTemplateMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, FightSpiritTemplateInfo> _fightSpiritTemplate;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, FightSpiritTemplateInfo> dictionary = new Dictionary<int, FightSpiritTemplateInfo>();
			if (Load(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_fightSpiritTemplate = dictionary;
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
			_fightSpiritTemplate = new Dictionary<int, FightSpiritTemplateInfo>();
			rand = new ThreadSafeRandom();
			return Load(_fightSpiritTemplate);
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

	private static bool Load(Dictionary<int, FightSpiritTemplateInfo> consortiaLevel)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			FightSpiritTemplateInfo[] allFightSpiritTemplate = playerBussiness.GetAllFightSpiritTemplate();
			FightSpiritTemplateInfo[] array = allFightSpiritTemplate;
			foreach (FightSpiritTemplateInfo fightSpiritTemplateInfo in array)
			{
				if (!consortiaLevel.ContainsKey(fightSpiritTemplateInfo.ID))
				{
					consortiaLevel.Add(fightSpiritTemplateInfo.ID, fightSpiritTemplateInfo);
				}
			}
		}
		return true;
	}

	public static FightSpiritTemplateInfo FindFightSpiritTemplateInfo(int FigSpiritId, int lv)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (FightSpiritTemplateInfo value in _fightSpiritTemplate.Values)
			{
				if (value.FightSpiritID == FigSpiritId && value.Level == lv)
				{
					return value;
				}
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

	public static int getProp(int FigSpiritId, int lv, int place)
	{
		FightSpiritTemplateInfo fightSpiritTemplateInfo = FindFightSpiritTemplateInfo(FigSpiritId, lv);
		return place switch
		{
			2 => fightSpiritTemplateInfo.Attack,
			5 => fightSpiritTemplateInfo.Agility,
			11 => fightSpiritTemplateInfo.Defence,
			3 => fightSpiritTemplateInfo.Lucky,
			13 => fightSpiritTemplateInfo.Blood,
			_ => 0,
		};
	}
}
