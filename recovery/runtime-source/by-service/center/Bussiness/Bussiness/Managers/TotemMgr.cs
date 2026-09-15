using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class TotemMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TotemInfo> _totem;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, TotemInfo> totem = new Dictionary<int, TotemInfo>();
			if (Load(totem))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_totem = totem;
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
				log.Error("TotemMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_totem = new Dictionary<int, TotemInfo>();
			rand = new ThreadSafeRandom();
			return Load(_totem);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TotemMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<int, TotemInfo> totem)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			TotemInfo[] allTotem = playerBussiness.GetAllTotem();
			TotemInfo[] array = allTotem;
			foreach (TotemInfo totemInfo in array)
			{
				if (!totem.ContainsKey(totemInfo.ID))
				{
					totem.Add(totemInfo.ID, totemInfo);
				}
			}
		}
		return true;
	}

	public static TotemInfo FindTotemInfo(int ID)
	{
		if (ID < 10000)
		{
			ID = 10001;
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_totem.ContainsKey(ID))
			{
				return _totem[ID];
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

	public static int GetTotemProp(int id, string typeOf)
	{
		int num = 0;
		for (int i = 10001; i <= id; i++)
		{
			TotemInfo totemInfo = FindTotemInfo(i);
			switch (typeOf)
			{
			case "att":
				num += totemInfo.AddAttack;
				break;
			case "agi":
				num += totemInfo.AddAgility;
				break;
			case "def":
				num += totemInfo.AddDefence;
				break;
			case "luc":
				num += totemInfo.AddLuck;
				break;
			case "blo":
				num += totemInfo.AddBlood;
				break;
			case "dam":
				num += totemInfo.AddDamage;
				break;
			case "gua":
				num += totemInfo.AddGuard;
				break;
			}
		}
		return num;
	}
}
