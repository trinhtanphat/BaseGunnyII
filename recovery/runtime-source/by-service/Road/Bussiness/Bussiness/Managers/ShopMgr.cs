using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public static class ShopMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ShopItemInfo> m_shop = new Dictionary<int, ShopItemInfo>();

	private static Dictionary<int, ShopGoodsShowListInfo> m_shopGoodsShowLists = new Dictionary<int, ShopGoodsShowListInfo>();

	private static ReaderWriterLock m_lock = new ReaderWriterLock();

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ShopItemInfo> dictionary = LoadFromDatabase();
			Dictionary<int, ShopGoodsShowListInfo> dictionary2 = LoadShowListFromDatabase();
			if (dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_shop, dictionary);
			}
			if (dictionary2.Count > 0)
			{
				Interlocked.Exchange(ref m_shopGoodsShowLists, dictionary2);
			}
			return true;
		}
		catch (Exception exception)
		{
			log.Error("ShopInfoMgr", exception);
		}
		return false;
	}

	private static Dictionary<int, ShopItemInfo> LoadFromDatabase()
	{
		Dictionary<int, ShopItemInfo> dictionary = new Dictionary<int, ShopItemInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ShopItemInfo[] aLllShop = produceBussiness.GetALllShop();
			ShopItemInfo[] array = aLllShop;
			foreach (ShopItemInfo shopItemInfo in array)
			{
				if (!dictionary.ContainsKey(shopItemInfo.ID))
				{
					dictionary.Add(shopItemInfo.ID, shopItemInfo);
				}
			}
		}
		return dictionary;
	}

	private static Dictionary<int, ShopGoodsShowListInfo> LoadShowListFromDatabase()
	{
		Dictionary<int, ShopGoodsShowListInfo> dictionary = new Dictionary<int, ShopGoodsShowListInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			ShopGoodsShowListInfo[] allShopGoodsShowList = produceBussiness.GetAllShopGoodsShowList();
			ShopGoodsShowListInfo[] array = allShopGoodsShowList;
			foreach (ShopGoodsShowListInfo shopGoodsShowListInfo in array)
			{
				if (!dictionary.ContainsKey(shopGoodsShowListInfo.ShopId))
				{
					dictionary.Add(shopGoodsShowListInfo.ShopId, shopGoodsShowListInfo);
				}
			}
		}
		return dictionary;
	}

	public static bool IsOnShop(int Id)
	{
		if (m_shopGoodsShowLists == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (m_shopGoodsShowLists.Keys.Contains(Id))
			{
				return true;
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return false;
	}

	public static bool IsSpecialItem(int Id)
	{
		if (Id <= 11008)
		{
			if (Id != 11004 && Id != 11008)
			{
				return false;
			}
		}
		else if (Id != 11012 && Id != 11016)
		{
			return false;
		}
		return true;
	}

	public static ShopItemInfo GetShopItemInfoById(int ID)
	{
		if (m_shop.ContainsKey(ID))
		{
			return m_shop[ID];
		}
		return null;
	}

	public static bool CanBuy(int shopID, int consortiaShopLevel, ref bool isBinds, int cousortiaID, int playerRiches)
	{
		bool result = false;
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			switch (shopID)
			{
			case 1:
				result = true;
				isBinds = false;
				break;
			case 2:
			case 3:
			case 4:
				result = true;
				isBinds = true;
				break;
			case 11:
			{
				ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 1, 1);
				if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			}
			case 12:
			{
				ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 2, 1);
				if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			}
			case 13:
			{
				ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 3, 1);
				if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			}
			case 14:
			{
				ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 4, 1);
				if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			}
			case 15:
			{
				ConsortiaEquipControlInfo consortiaEuqipRiches = consortiaBussiness.GetConsortiaEuqipRiches(cousortiaID, 5, 1);
				if (consortiaShopLevel >= consortiaEuqipRiches.Level && playerRiches >= consortiaEuqipRiches.Riches)
				{
					result = true;
					isBinds = true;
				}
				break;
			}
			case 72:
			case 91:
			case 92:
			case 93:
			case 94:
			case 95:
			case 98:
			case 99:
			case 100:
				result = true;
				isBinds = true;
				break;
			}
		}
		return result;
	}

	public static void FindSpecialItemInfo(ItemInfo info, ref int gold, ref int money, ref int giftToken, ref int medal, ref int honor, ref int hardCurrency, ref int token, ref int dragonToken)
	{
		switch (info.TemplateID)
		{
		case -1200:
			dragonToken += info.Count;
			info = null;
			break;
		case -1100:
			giftToken += info.Count;
			info = null;
			break;
		case -1000:
			token += info.Count;
			info = null;
			break;
		case -900:
			hardCurrency += info.Count;
			info = null;
			break;
		case -800:
			honor += info.Count;
			info = null;
			break;
		case -300:
			medal += info.Count;
			info = null;
			break;
		case -200:
			money += info.Count;
			info = null;
			break;
		case -100:
			gold += info.Count;
			info = null;
			break;
		}
	}

	public static bool SetItemType(ShopItemInfo shop, int type, ref int damageScore, ref int petScore, ref int iTemplateID, ref int iCount, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal, ref int hardCurrency, ref int LeagueMoney, ref int useableScore)
	{
		if (type == 1)
		{
			GetItemPrice(shop.APrice1, shop.AValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
			GetItemPrice(shop.APrice2, shop.AValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
			GetItemPrice(shop.APrice3, shop.AValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
		}
		if (type == 2)
		{
			GetItemPrice(shop.BPrice1, shop.BValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
			GetItemPrice(shop.BPrice2, shop.BValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
			GetItemPrice(shop.BPrice3, shop.BValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
		}
		if (type == 3)
		{
			GetItemPrice(shop.CPrice1, shop.CValue1, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
			GetItemPrice(shop.CPrice2, shop.CValue2, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
			GetItemPrice(shop.CPrice3, shop.CValue3, shop.Beat, ref damageScore, ref petScore, ref iTemplateID, ref iCount, ref gold, ref money, ref offer, ref gifttoken, ref medal, ref hardCurrency, ref LeagueMoney, ref useableScore);
		}
		return true;
	}

	public static void GetItemPrice(int Prices, int Values, decimal beat, ref int damageScore, ref int petScore, ref int iTemplateID, ref int iCount, ref int gold, ref int money, ref int offer, ref int gifttoken, ref int medal, ref int hardCurrency, ref int LeagueMoney, ref int useableScore)
	{
		switch (Prices)
		{
		case -1200:
			useableScore += (int)((decimal)Values * beat);
			return;
		case -1000:
			LeagueMoney += (int)((decimal)Values * beat);
			return;
		case -900:
			hardCurrency += (int)((decimal)Values * beat);
			return;
		case -800:
			medal += (int)((decimal)Values * beat);
			return;
		case -300:
			medal += (int)((decimal)Values * beat);
			return;
		case -8:
			petScore += (int)((decimal)Values * beat);
			return;
		case -7:
			damageScore += (int)((decimal)Values * beat);
			return;
		case -6:
			medal += (int)((decimal)Values * beat);
			return;
		case -4:
			offer += (int)((decimal)Values * beat);
			return;
		case -3:
			gold += (int)((decimal)Values * beat);
			return;
		case -2:
			gifttoken += (int)((decimal)Values * beat);
			return;
		case -1:
			money += (int)((decimal)Values * beat);
			return;
		}
		if (Prices > 0)
		{
			iTemplateID = Prices;
			iCount = Values;
		}
	}

	public static int FindItemTemplateID(int id)
	{
		if (m_shop.ContainsKey(id))
		{
			return m_shop[id].TemplateID;
		}
		return 0;
	}

	public static List<ShopItemInfo> FindShopbyTemplatID(int TemplatID)
	{
		List<ShopItemInfo> list = new List<ShopItemInfo>();
		foreach (ShopItemInfo value in m_shop.Values)
		{
			if (value.TemplateID == TemplatID)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static ShopItemInfo FindShopbyTemplateID(int TemplatID)
	{
		foreach (ShopItemInfo value in m_shop.Values)
		{
			if (value.TemplateID == TemplatID)
			{
				return value;
			}
		}
		return null;
	}
}
