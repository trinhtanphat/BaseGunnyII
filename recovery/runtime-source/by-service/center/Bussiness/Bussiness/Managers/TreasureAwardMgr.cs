using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class TreasureAwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, TreasureAwardInfo> _treasureAward;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, TreasureAwardInfo> treasureAward = new Dictionary<int, TreasureAwardInfo>();
			if (Load(treasureAward))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_treasureAward = treasureAward;
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
				log.Error("TreasureAwardMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_treasureAward = new Dictionary<int, TreasureAwardInfo>();
			rand = new ThreadSafeRandom();
			return Load(_treasureAward);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("TreasureAwardMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<int, TreasureAwardInfo> treasureAward)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			TreasureAwardInfo[] allTreasureAward = playerBussiness.GetAllTreasureAward();
			TreasureAwardInfo[] array = allTreasureAward;
			foreach (TreasureAwardInfo treasureAwardInfo in array)
			{
				if (!treasureAward.ContainsKey(treasureAwardInfo.ID))
				{
					treasureAward.Add(treasureAwardInfo.ID, treasureAwardInfo);
				}
			}
		}
		return true;
	}

	public static TreasureAwardInfo FindTreasureAwardInfo(int ID)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_treasureAward.ContainsKey(ID))
			{
				return _treasureAward[ID];
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

	public static List<TreasureAwardInfo> GetTreasureInfos()
	{
		if (_treasureAward == null)
		{
			Init();
		}
		List<TreasureAwardInfo> list = new List<TreasureAwardInfo>();
		for (int i = 1; i <= _treasureAward.Count; i++)
		{
			list.Add(_treasureAward[i]);
		}
		return list;
	}

	public static List<TreasureDataInfo> CreateTreasureData(int UserID)
	{
		List<TreasureDataInfo> list = new List<TreasureDataInfo>();
		Dictionary<int, TreasureDataInfo> dictionary = new Dictionary<int, TreasureDataInfo>();
		int num = 0;
		while (list.Count < 16)
		{
			List<TreasureDataInfo> treasureData = GetTreasureData();
			int index = rand.Next(treasureData.Count);
			TreasureDataInfo treasureDataInfo = treasureData[index];
			treasureDataInfo.UserID = UserID;
			if (!dictionary.Keys.Contains(treasureDataInfo.TemplateID))
			{
				dictionary.Add(treasureDataInfo.TemplateID, treasureDataInfo);
				list.Add(treasureDataInfo);
			}
			num++;
		}
		dictionary = null;
		return list;
	}

	public static List<ItemInfo> CreateDiceAward()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		int num = 0;
		while (list.Count < 19)
		{
			List<ItemInfo> diceAward = GetDiceAward();
			int index = rand.Next(diceAward.Count);
			ItemInfo itemInfo = diceAward[index];
			if (!dictionary.Keys.Contains(itemInfo.TemplateID))
			{
				dictionary.Add(itemInfo.TemplateID, itemInfo);
				list.Add(itemInfo);
			}
			num++;
		}
		dictionary = null;
		return list;
	}

	public static List<CardInfo> GetFightFootballTimeAward()
	{
		List<CardInfo> list = new List<CardInfo>();
		List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
		List<TreasureAwardInfo> treasureInfos = GetTreasureInfos();
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(treasureInfos.Select((TreasureAwardInfo s) => s.Random).Max());
		List<TreasureAwardInfo> list3 = treasureInfos.Where((TreasureAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				TreasureAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (TreasureAwardInfo item2 in list2)
		{
			CardInfo cardInfo = new CardInfo();
			cardInfo.templateID = item2.TemplateID;
			cardInfo.count = item2.Count;
			list.Add(cardInfo);
		}
		return list;
	}

	public static Dictionary<int, CardInfo> CreateFightFootballTimeAward()
	{
		Dictionary<int, CardInfo> dictionary = new Dictionary<int, CardInfo>();
		Dictionary<int, CardInfo> dictionary2 = new Dictionary<int, CardInfo>();
		int num = 0;
		int num2 = 0;
		while (dictionary.Count < 9)
		{
			List<CardInfo> fightFootballTimeAward = GetFightFootballTimeAward();
			int index = rand.Next(fightFootballTimeAward.Count);
			CardInfo cardInfo = fightFootballTimeAward[index];
			if (!dictionary2.Keys.Contains(cardInfo.templateID))
			{
				dictionary2.Add(cardInfo.templateID, cardInfo);
				cardInfo.place = num;
				cardInfo.count = cardInfo.count;
				dictionary.Add(num, cardInfo);
				num++;
			}
			num2++;
		}
		dictionary2 = null;
		return dictionary;
	}

	public static NewChickenBoxItemInfo[] CreateChickenBoxAward(int count)
	{
		List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
		Dictionary<int, NewChickenBoxItemInfo> dictionary = new Dictionary<int, NewChickenBoxItemInfo>();
		int num = 0;
		int num2 = 0;
		while (list.Count < count)
		{
			List<NewChickenBoxItemInfo> newChickenBoxAward = GetNewChickenBoxAward();
			int index = rand.Next(newChickenBoxAward.Count);
			NewChickenBoxItemInfo newChickenBoxItemInfo = newChickenBoxAward[index];
			if (!dictionary.Keys.Contains(newChickenBoxItemInfo.TemplateID))
			{
				dictionary.Add(newChickenBoxItemInfo.TemplateID, newChickenBoxItemInfo);
				newChickenBoxItemInfo.Position = num;
				list.Add(newChickenBoxItemInfo);
				num++;
			}
			num2++;
		}
		dictionary = null;
		return list.ToArray();
	}

	public static List<NewChickenBoxItemInfo> GetNewChickenBoxAward()
	{
		List<NewChickenBoxItemInfo> list = new List<NewChickenBoxItemInfo>();
		List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
		List<TreasureAwardInfo> treasureInfos = GetTreasureInfos();
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(treasureInfos.Select((TreasureAwardInfo s) => s.Random).Max());
		List<TreasureAwardInfo> list3 = treasureInfos.Where((TreasureAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				TreasureAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (TreasureAwardInfo item2 in list2)
		{
			NewChickenBoxItemInfo newChickenBoxItemInfo = new NewChickenBoxItemInfo();
			newChickenBoxItemInfo.TemplateID = item2.TemplateID;
			newChickenBoxItemInfo.IsBinds = item2.isBind;
			newChickenBoxItemInfo.ValidDate = item2.Validate;
			newChickenBoxItemInfo.Count = item2.Count;
			newChickenBoxItemInfo.StrengthenLevel = item2.strengthLevel;
			newChickenBoxItemInfo.AttackCompose = 0;
			newChickenBoxItemInfo.DefendCompose = 0;
			newChickenBoxItemInfo.AgilityCompose = 0;
			newChickenBoxItemInfo.LuckCompose = 0;
			newChickenBoxItemInfo.Quality = ItemMgr.FindItemTemplate(item2.TemplateID)?.Quality ?? 2;
			newChickenBoxItemInfo.IsSelected = false;
			newChickenBoxItemInfo.IsSeeded = false;
			list.Add(newChickenBoxItemInfo);
		}
		return list;
	}

	public static List<ItemInfo> CreateDiceLevelAward(int type)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		Dictionary<int, ItemInfo> dictionary = new Dictionary<int, ItemInfo>();
		int num = 4;
		switch (type)
		{
		case 2:
			num = 5;
			break;
		case 3:
			num = 6;
			break;
		case 4:
			num = 7;
			break;
		case 5:
			num = 9;
			break;
		}
		int num2 = 0;
		while (list.Count < num)
		{
			List<ItemInfo> diceAward = GetDiceAward();
			int index = rand.Next(diceAward.Count);
			ItemInfo itemInfo = diceAward[index];
			if (!dictionary.Keys.Contains(itemInfo.TemplateID))
			{
				dictionary.Add(itemInfo.TemplateID, itemInfo);
				list.Add(itemInfo);
			}
			num2++;
		}
		dictionary = null;
		return list;
	}

	public static List<ItemInfo> GetDiceAward()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
		List<TreasureAwardInfo> treasureInfos = GetTreasureInfos();
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(treasureInfos.Select((TreasureAwardInfo s) => s.Random).Max());
		List<TreasureAwardInfo> list3 = treasureInfos.Where((TreasureAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				TreasureAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (TreasureAwardInfo item2 in list2)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(item2.TemplateID), item2.Count, 102);
			itemInfo.IsBinds = item2.isBind;
			itemInfo.ValidDate = item2.Validate;
			itemInfo.Count = item2.Count;
			itemInfo.StrengthenLevel = item2.strengthLevel;
			list.Add(itemInfo);
		}
		return list;
	}

	public static List<TreasureDataInfo> GetTreasureData()
	{
		List<TreasureDataInfo> list = new List<TreasureDataInfo>();
		List<TreasureAwardInfo> list2 = new List<TreasureAwardInfo>();
		List<TreasureAwardInfo> treasureInfos = GetTreasureInfos();
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(treasureInfos.Select((TreasureAwardInfo s) => s.Random).Max());
		List<TreasureAwardInfo> list3 = treasureInfos.Where((TreasureAwardInfo s) => s.Random >= maxRound).ToList();
		int num2 = list3.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				TreasureAwardInfo item = list3[index];
				list2.Add(item);
			}
		}
		foreach (TreasureAwardInfo item2 in list2)
		{
			TreasureDataInfo treasureDataInfo = new TreasureDataInfo();
			treasureDataInfo.ID = 0;
			treasureDataInfo.UserID = 0;
			treasureDataInfo.TemplateID = item2.TemplateID;
			treasureDataInfo.Count = item2.Count;
			treasureDataInfo.ValidDate = item2.Validate;
			treasureDataInfo.pos = -1;
			treasureDataInfo.BeginDate = DateTime.Now;
			treasureDataInfo.IsExit = true;
			list.Add(treasureDataInfo);
		}
		return list;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = ThreadSafeRandom.NextStatic(minValue, maxValue + 1);
			int num2 = 0;
			for (int j = 0; j < i; j++)
			{
				if (array[j] == num)
				{
					num2++;
				}
			}
			if (num2 == 0)
			{
				array[i] = num;
			}
			else
			{
				i--;
			}
		}
		return array;
	}
}
