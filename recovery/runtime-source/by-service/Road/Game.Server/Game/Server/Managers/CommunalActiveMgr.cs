using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class CommunalActiveMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, CommunalActiveInfo> _communalActives;

	private static Dictionary<int, CommunalActiveAwardInfo> _communalActiveAwards;

	private static Dictionary<int, CommunalActiveExpInfo> _communalActiveExps;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_communalActives = new Dictionary<int, CommunalActiveInfo>();
			_communalActiveAwards = new Dictionary<int, CommunalActiveAwardInfo>();
			_communalActiveExps = new Dictionary<int, CommunalActiveExpInfo>();
			rand = new ThreadSafeRandom();
			return LoadData(_communalActives, _communalActiveAwards, _communalActiveExps);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("CommunalActiveMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, CommunalActiveInfo> communalActives = new Dictionary<int, CommunalActiveInfo>();
			Dictionary<int, CommunalActiveAwardInfo> communalActiveAwards = new Dictionary<int, CommunalActiveAwardInfo>();
			Dictionary<int, CommunalActiveExpInfo> communalActiveExps = new Dictionary<int, CommunalActiveExpInfo>();
			if (LoadData(communalActives, communalActiveAwards, communalActiveExps))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_communalActives = communalActives;
					_communalActiveAwards = communalActiveAwards;
					_communalActiveExps = communalActiveExps;
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
				log.Error("CommunalActiveMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadData(Dictionary<int, CommunalActiveInfo> CommunalActives, Dictionary<int, CommunalActiveAwardInfo> CommunalActiveAwards, Dictionary<int, CommunalActiveExpInfo> CommunalActiveExps)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			CommunalActiveInfo[] allCommunalActive = playerBussiness.GetAllCommunalActive();
			CommunalActiveInfo[] array = allCommunalActive;
			foreach (CommunalActiveInfo communalActiveInfo in array)
			{
				if (!CommunalActives.ContainsKey(communalActiveInfo.ActiveID))
				{
					CommunalActives.Add(communalActiveInfo.ActiveID, communalActiveInfo);
				}
			}
			CommunalActiveAwardInfo[] allCommunalActiveAward = playerBussiness.GetAllCommunalActiveAward();
			CommunalActiveAwardInfo[] array2 = allCommunalActiveAward;
			foreach (CommunalActiveAwardInfo communalActiveAwardInfo in array2)
			{
				if (!CommunalActiveAwards.ContainsKey(communalActiveAwardInfo.ID))
				{
					CommunalActiveAwards.Add(communalActiveAwardInfo.ID, communalActiveAwardInfo);
				}
			}
			CommunalActiveExpInfo[] allCommunalActiveExp = playerBussiness.GetAllCommunalActiveExp();
			CommunalActiveExpInfo[] array3 = allCommunalActiveExp;
			foreach (CommunalActiveExpInfo communalActiveExpInfo in array3)
			{
				if (!CommunalActiveExps.ContainsKey(communalActiveExpInfo.Grade))
				{
					CommunalActiveExps.Add(communalActiveExpInfo.Grade, communalActiveExpInfo);
				}
			}
		}
		return true;
	}

	public static List<ItemInfo> GetAwardInfos(int type, int rank)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<CommunalActiveAwardInfo> list2 = FindCommunalAwards(type);
		foreach (CommunalActiveAwardInfo item in list2)
		{
			if (item.RandID == rank)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item.TemplateID);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, item.Count, 102);
				if (itemInfo != null)
				{
					itemInfo.Count = item.Count;
					itemInfo.IsBinds = item.IsBind;
					itemInfo.ValidDate = item.ValidDate;
					itemInfo.StrengthenLevel = item.StrengthenLevel;
					itemInfo.AttackCompose = item.AttackCompose;
					itemInfo.DefendCompose = item.DefendCompose;
					itemInfo.AgilityCompose = item.AgilityCompose;
					itemInfo.LuckCompose = item.LuckCompose;
					list.Add(itemInfo);
				}
			}
		}
		return list;
	}

	public static List<CommunalActiveAwardInfo> FindCommunalAwards(int isArea)
	{
		if (_communalActiveAwards == null)
		{
			Init();
		}
		List<CommunalActiveAwardInfo> list = new List<CommunalActiveAwardInfo>();
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (CommunalActiveAwardInfo value in _communalActiveAwards.Values)
			{
				if (value.IsArea == isArea)
				{
					list.Add(value);
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
		return list;
	}

	public static CommunalActiveInfo FindCommunalActive(int ActiveID)
	{
		if (_communalActives == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_communalActives.ContainsKey(ActiveID))
			{
				return _communalActives[ActiveID];
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

	public static int GetGP(int level)
	{
		if (_communalActiveExps == null)
		{
			Init();
		}
		if (_communalActiveExps.ContainsKey(level))
		{
			return _communalActiveExps[level].Exp;
		}
		return 0;
	}

	public static void ResetEvent()
	{
	}
}
