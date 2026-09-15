using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class StrengthenMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, StrengthenInfo> _strengthens;

	private static Dictionary<int, StrengthenInfo> _Refinery_Strengthens;

	private static Dictionary<int, StrengthenGoodsInfo> _Strengthens_Goods;

	private static Dictionary<int, StrengThenExpInfo> _Strengthens_Exps;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, StrengthenInfo> dictionary = new Dictionary<int, StrengthenInfo>();
			Dictionary<int, StrengthenInfo> dictionary2 = new Dictionary<int, StrengthenInfo>();
			Dictionary<int, StrengThenExpInfo> dictionary3 = new Dictionary<int, StrengThenExpInfo>();
			Dictionary<int, StrengthenGoodsInfo> dictionary4 = new Dictionary<int, StrengthenGoodsInfo>();
			if (LoadStrengthen(dictionary, dictionary2, dictionary3, dictionary4))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_strengthens = dictionary;
					_Refinery_Strengthens = dictionary2;
					_Strengthens_Exps = dictionary3;
					_Strengthens_Goods = dictionary4;
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
				log.Error("StrengthenMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_strengthens = new Dictionary<int, StrengthenInfo>();
			_Refinery_Strengthens = new Dictionary<int, StrengthenInfo>();
			_Strengthens_Goods = new Dictionary<int, StrengthenGoodsInfo>();
			_Strengthens_Exps = new Dictionary<int, StrengThenExpInfo>();
			rand = new ThreadSafeRandom();
			return LoadStrengthen(_strengthens, _Refinery_Strengthens, _Strengthens_Exps, _Strengthens_Goods);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("StrengthenMgr", exception);
			}
			return false;
		}
	}

	private static bool LoadStrengthen(Dictionary<int, StrengthenInfo> strengthen, Dictionary<int, StrengthenInfo> RefineryStrengthen, Dictionary<int, StrengThenExpInfo> StrengthenExp, Dictionary<int, StrengthenGoodsInfo> StrengthensGoods)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			StrengthenInfo[] allStrengthen = produceBussiness.GetAllStrengthen();
			StrengthenInfo[] allRefineryStrengthen = produceBussiness.GetAllRefineryStrengthen();
			StrengThenExpInfo[] allStrengThenExp = produceBussiness.GetAllStrengThenExp();
			StrengthenGoodsInfo[] allStrengthenGoodsInfo = produceBussiness.GetAllStrengthenGoodsInfo();
			StrengthenInfo[] array = allStrengthen;
			foreach (StrengthenInfo strengthenInfo in array)
			{
				if (!strengthen.ContainsKey(strengthenInfo.StrengthenLevel))
				{
					strengthen.Add(strengthenInfo.StrengthenLevel, strengthenInfo);
				}
			}
			StrengthenInfo[] array2 = allRefineryStrengthen;
			foreach (StrengthenInfo strengthenInfo2 in array2)
			{
				if (!RefineryStrengthen.ContainsKey(strengthenInfo2.StrengthenLevel))
				{
					RefineryStrengthen.Add(strengthenInfo2.StrengthenLevel, strengthenInfo2);
				}
			}
			StrengThenExpInfo[] array3 = allStrengThenExp;
			foreach (StrengThenExpInfo strengThenExpInfo in array3)
			{
				if (!StrengthenExp.ContainsKey(strengThenExpInfo.Level))
				{
					StrengthenExp.Add(strengThenExpInfo.Level, strengThenExpInfo);
				}
			}
			StrengthenGoodsInfo[] array4 = allStrengthenGoodsInfo;
			foreach (StrengthenGoodsInfo strengthenGoodsInfo in array4)
			{
				if (!StrengthensGoods.ContainsKey(strengthenGoodsInfo.ID))
				{
					StrengthensGoods.Add(strengthenGoodsInfo.ID, strengthenGoodsInfo);
				}
			}
		}
		return true;
	}

	public static StrengThenExpInfo FindStrengthenExpInfo(int level)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_Strengthens_Exps.ContainsKey(level))
			{
				return _Strengthens_Exps[level];
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

	public static bool canUpLv(int exp, int level)
	{
		StrengThenExpInfo strengThenExpInfo = FindStrengthenExpInfo(level + 1);
		return strengThenExpInfo != null && exp >= strengThenExpInfo.Exp;
	}

	public static int getNeedExp(int Exp, int level)
	{
		StrengThenExpInfo strengThenExpInfo = FindStrengthenExpInfo(level + 1);
		if (strengThenExpInfo == null)
		{
			return 0;
		}
		return strengThenExpInfo.Exp - Exp;
	}

	public static int GetNecklacePlus(int exp, int currentPlus)
	{
		foreach (StrengThenExpInfo value in _Strengthens_Exps.Values)
		{
			if (exp < value.NecklaceStrengthExp)
			{
				int necklaceLevel = GetNecklaceLevel(exp);
				return FindStrengthenExpInfo(necklaceLevel)?.NecklaceStrengthPlus ?? currentPlus;
			}
		}
		return currentPlus;
	}

	public static int GetNecklaceLevel(int exp)
	{
		foreach (StrengThenExpInfo value in _Strengthens_Exps.Values)
		{
			if (exp < value.NecklaceStrengthExp)
			{
				return (value.Level - 1 >= 0) ? (value.Level - 1) : 0;
			}
		}
		return 0;
	}

	public static int GetNecklaceMaxExp(int lv)
	{
		return FindStrengthenExpInfo(lv)?.NecklaceStrengthExp ?? 0;
	}

	public static int GetNecklaceMaxPlus(int lv)
	{
		return FindStrengthenExpInfo(lv)?.NecklaceStrengthPlus ?? 0;
	}

	public static StrengthenInfo FindStrengthenInfo(int level)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_strengthens.ContainsKey(level))
			{
				return _strengthens[level];
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

	public static StrengthenInfo FindRefineryStrengthenInfo(int level)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_Refinery_Strengthens.ContainsKey(level))
			{
				return _Refinery_Strengthens[level];
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

	public static StrengthenGoodsInfo FindStrengthenGoodsInfo(int level, int templateId)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (StrengthenGoodsInfo value in _Strengthens_Goods.Values)
			{
				if (value.Level == level && templateId == value.CurrentEquip)
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

	public static StrengthenGoodsInfo FindTransferInfo(int level, int templateId)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (StrengthenGoodsInfo value in _Strengthens_Goods.Values)
			{
				if (value.Level == level && templateId == value.CurrentEquip)
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

	public static StrengthenGoodsInfo FindTransferInfo(int templateId)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (StrengthenGoodsInfo value in _Strengthens_Goods.Values)
			{
				if (templateId == value.GainEquip || templateId == value.CurrentEquip)
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

	public static bool TransferCondition(ItemInfo itemAtZero, ItemInfo itemAtOne)
	{
		return (itemAtZero.Template.CategoryID == 7 || itemAtOne.Template.CategoryID == 7) && (itemAtZero.StrengthenLevel >= 10 || itemAtOne.StrengthenLevel >= 10);
	}

	public static void InheritProperty(ItemInfo Item, ref ItemInfo item)
	{
		if (Item.Hole1 >= 0)
		{
			item.Hole1 = Item.Hole1;
		}
		if (Item.Hole2 >= 0)
		{
			item.Hole2 = Item.Hole2;
		}
		if (Item.Hole3 >= 0)
		{
			item.Hole3 = Item.Hole3;
		}
		if (Item.Hole4 >= 0)
		{
			item.Hole4 = Item.Hole4;
		}
		if (Item.Hole5 >= 0)
		{
			item.Hole5 = Item.Hole5;
		}
		if (Item.Hole6 >= 0)
		{
			item.Hole6 = Item.Hole6;
		}
		item.AttackCompose = Item.AttackCompose;
		item.DefendCompose = Item.DefendCompose;
		item.LuckCompose = Item.LuckCompose;
		item.AgilityCompose = Item.AgilityCompose;
		item.IsBinds = Item.IsBinds;
		item.ValidDate = Item.ValidDate;
	}

	public static void InheritTransferProperty(ref ItemInfo itemZero, ref ItemInfo itemOne, bool tranHole, bool tranHoleFivSix)
	{
		int hole = itemZero.Hole1;
		int hole2 = itemZero.Hole2;
		int hole3 = itemZero.Hole3;
		int hole4 = itemZero.Hole4;
		int hole5 = itemZero.Hole5;
		int hole6 = itemZero.Hole6;
		int hole5Exp = itemZero.Hole5Exp;
		int hole5Level = itemZero.Hole5Level;
		int hole6Exp = itemZero.Hole6Exp;
		int hole6Level = itemZero.Hole6Level;
		int attackCompose = itemZero.AttackCompose;
		int defendCompose = itemZero.DefendCompose;
		int agilityCompose = itemZero.AgilityCompose;
		int luckCompose = itemZero.LuckCompose;
		int strengthenLevel = itemZero.StrengthenLevel;
		int strengthenExp = itemZero.StrengthenExp;
		bool isGold = itemZero.IsGold;
		int goldValidDate = itemZero.goldValidDate;
		DateTime goldBeginTime = itemZero.goldBeginTime;
		string latentEnergyCurStr = itemZero.latentEnergyCurStr;
		string latentEnergyNewStr = itemZero.latentEnergyNewStr;
		DateTime latentEnergyEndTime = itemZero.latentEnergyEndTime;
		int hole7 = itemOne.Hole1;
		int hole8 = itemOne.Hole2;
		int hole9 = itemOne.Hole3;
		int hole10 = itemOne.Hole4;
		int hole11 = itemOne.Hole5;
		int hole12 = itemOne.Hole6;
		int hole5Exp2 = itemOne.Hole5Exp;
		int hole5Level2 = itemOne.Hole5Level;
		int hole6Exp2 = itemOne.Hole6Exp;
		int hole6Level2 = itemOne.Hole6Level;
		int attackCompose2 = itemOne.AttackCompose;
		int defendCompose2 = itemOne.DefendCompose;
		int agilityCompose2 = itemOne.AgilityCompose;
		int luckCompose2 = itemOne.LuckCompose;
		int strengthenLevel2 = itemOne.StrengthenLevel;
		int strengthenExp2 = itemOne.StrengthenExp;
		bool isGold2 = itemOne.IsGold;
		int goldValidDate2 = itemOne.goldValidDate;
		DateTime goldBeginTime2 = itemOne.goldBeginTime;
		string latentEnergyCurStr2 = itemOne.latentEnergyCurStr;
		string latentEnergyNewStr2 = itemOne.latentEnergyNewStr;
		DateTime latentEnergyEndTime2 = itemOne.latentEnergyEndTime;
		if (tranHole)
		{
			itemOne.Hole1 = hole;
			itemZero.Hole1 = hole7;
			itemOne.Hole2 = hole2;
			itemZero.Hole2 = hole8;
			itemOne.Hole3 = hole3;
			itemZero.Hole3 = hole9;
			itemOne.Hole4 = hole4;
			itemZero.Hole4 = hole10;
		}
		if (tranHoleFivSix)
		{
			itemOne.Hole5 = hole5;
			itemZero.Hole5 = hole11;
			itemOne.Hole6 = hole6;
			itemZero.Hole6 = hole12;
		}
		itemOne.Hole5Exp = hole5Exp;
		itemZero.Hole5Exp = hole5Exp2;
		itemOne.Hole5Level = hole5Level;
		itemZero.Hole5Level = hole5Level2;
		itemOne.Hole6Exp = hole6Exp;
		itemZero.Hole6Exp = hole6Exp2;
		itemOne.Hole6Level = hole6Level;
		itemZero.Hole6Level = hole6Level2;
		itemZero.StrengthenLevel = strengthenLevel2;
		itemOne.StrengthenLevel = strengthenLevel;
		itemZero.StrengthenExp = strengthenExp2;
		itemOne.StrengthenExp = strengthenExp;
		itemZero.AttackCompose = attackCompose2;
		itemOne.AttackCompose = attackCompose;
		itemZero.DefendCompose = defendCompose2;
		itemOne.DefendCompose = defendCompose;
		itemZero.LuckCompose = luckCompose2;
		itemOne.LuckCompose = luckCompose;
		itemZero.AgilityCompose = agilityCompose2;
		itemOne.AgilityCompose = agilityCompose;
		if (itemZero.IsBinds || itemOne.IsBinds)
		{
			itemOne.IsBinds = true;
			itemZero.IsBinds = true;
		}
		itemZero.goldBeginTime = goldBeginTime2;
		itemOne.goldBeginTime = goldBeginTime;
		itemZero.goldValidDate = goldValidDate2;
		itemOne.goldValidDate = goldValidDate;
		itemZero.latentEnergyCurStr = latentEnergyCurStr2;
		itemOne.latentEnergyCurStr = latentEnergyCurStr;
		itemZero.latentEnergyNewStr = latentEnergyNewStr2;
		itemOne.latentEnergyNewStr = latentEnergyNewStr;
		itemZero.latentEnergyEndTime = latentEnergyEndTime2;
		itemOne.latentEnergyEndTime = latentEnergyEndTime;
	}
}
