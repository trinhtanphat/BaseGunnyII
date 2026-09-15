using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class CardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static CardGrooveUpdateInfo[] m_grooveUpdate;

	private static Dictionary<int, List<CardGrooveUpdateInfo>> m_grooveUpdates;

	private static CardTemplateInfo[] m_cardBox;

	private static Dictionary<int, List<CardTemplateInfo>> m_cardBoxs;

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool ReLoad()
	{
		try
		{
			CardGrooveUpdateInfo[] array = LoadGrooveUpdateDb();
			Dictionary<int, List<CardGrooveUpdateInfo>> value = LoadGrooveUpdates(array);
			if (array != null)
			{
				Interlocked.Exchange(ref m_grooveUpdate, array);
				Interlocked.Exchange(ref m_grooveUpdates, value);
			}
			CardTemplateInfo[] array2 = LoadCardBoxDb();
			Dictionary<int, List<CardTemplateInfo>> value2 = LoadCardBoxs(array2);
			if (array2 != null)
			{
				Interlocked.Exchange(ref m_cardBox, array2);
				Interlocked.Exchange(ref m_cardBoxs, value2);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
			return false;
		}
		return true;
	}

	public static bool Init()
	{
		return ReLoad();
	}

	public static CardGrooveUpdateInfo[] LoadGrooveUpdateDb()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		return playerBussiness.GetAllCardGrooveUpdate();
	}

	public static Dictionary<int, List<CardGrooveUpdateInfo>> LoadGrooveUpdates(CardGrooveUpdateInfo[] GrooveUpdates)
	{
		Dictionary<int, List<CardGrooveUpdateInfo>> dictionary = new Dictionary<int, List<CardGrooveUpdateInfo>>();
		foreach (CardGrooveUpdateInfo info in GrooveUpdates)
		{
			if (!dictionary.Keys.Contains(info.Type))
			{
				IEnumerable<CardGrooveUpdateInfo> source = GrooveUpdates.Where((CardGrooveUpdateInfo s) => s.Type == info.Type);
				dictionary.Add(info.Type, source.ToList());
			}
		}
		return dictionary;
	}

	public static CardTemplateInfo[] LoadCardBoxDb()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		return playerBussiness.GetAllCardTemplate();
	}

	public static Dictionary<int, List<CardTemplateInfo>> LoadCardBoxs(CardTemplateInfo[] CardBoxs)
	{
		Dictionary<int, List<CardTemplateInfo>> dictionary = new Dictionary<int, List<CardTemplateInfo>>();
		foreach (CardTemplateInfo info in CardBoxs)
		{
			if (!dictionary.Keys.Contains(info.CardID))
			{
				IEnumerable<CardTemplateInfo> source = CardBoxs.Where((CardTemplateInfo s) => s.CardID == info.CardID);
				dictionary.Add(info.CardID, source.ToList());
			}
		}
		return dictionary;
	}

	public static CardTemplateInfo GetCard(int cardId)
	{
		CardTemplateInfo cardTemplateInfo = new CardTemplateInfo();
		List<CardTemplateInfo> list = FindCardBox(cardId);
		if (list == null)
		{
			return null;
		}
		int num = 1;
		int maxRound = ThreadSafeRandom.NextStatic(list.Select((CardTemplateInfo s) => s.probability).Max());
		List<CardTemplateInfo> list2 = list.Where((CardTemplateInfo s) => s.probability >= maxRound).ToList();
		int num2 = list2.Count();
		if (num2 > 0)
		{
			num = ((num > num2) ? num2 : num);
			int[] randomUnrepeatArray = GetRandomUnrepeatArray(0, num2 - 1, num);
			int[] array = randomUnrepeatArray;
			foreach (int index in array)
			{
				cardTemplateInfo = list2[index];
			}
		}
		if (cardTemplateInfo.CardType <= 0)
		{
			return null;
		}
		return cardTemplateInfo;
	}

	public static int[] GetRandomUnrepeatArray(int minValue, int maxValue, int count)
	{
		int[] array = new int[count];
		for (int i = 0; i < count; i++)
		{
			int num = random.Next(minValue, maxValue + 1);
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

	public static List<CardTemplateInfo> FindCardBox(int cardId)
	{
		if (m_cardBoxs == null)
		{
			Init();
		}
		if (m_cardBoxs.ContainsKey(cardId))
		{
			return m_cardBoxs[cardId];
		}
		return null;
	}

	public static CardTemplateInfo FindCardTemplate(int cardId, int type)
	{
		if (m_cardBoxs == null)
		{
			Init();
		}
		if (m_cardBoxs.ContainsKey(cardId))
		{
			List<CardTemplateInfo> list = m_cardBoxs[cardId];
			foreach (CardTemplateInfo item in list)
			{
				if (type == item.CardType)
				{
					return item;
				}
			}
		}
		return null;
	}

	public static CardTemplateInfo GetSingleCard(int id)
	{
		List<CardTemplateInfo> allCard = GetAllCard();
		foreach (CardTemplateInfo item in allCard)
		{
			if (item.ID == id)
			{
				return item;
			}
		}
		return null;
	}

	public static List<CardTemplateInfo> GetAllCard()
	{
		if (m_cardBox == null)
		{
			Init();
		}
		List<CardTemplateInfo> list = new List<CardTemplateInfo>();
		Dictionary<int, CardTemplateInfo> dictionary = new Dictionary<int, CardTemplateInfo>();
		CardTemplateInfo[] cardBox = m_cardBox;
		foreach (CardTemplateInfo cardTemplateInfo in cardBox)
		{
			if (!dictionary.Keys.Contains(cardTemplateInfo.CardID))
			{
				if (cardTemplateInfo.CardID != 314150)
				{
					list.Add(cardTemplateInfo);
				}
				dictionary.Add(cardTemplateInfo.CardID, cardTemplateInfo);
			}
		}
		return list;
	}

	public static List<CardGrooveUpdateInfo> FindCardGrooveUpdate(int type)
	{
		if (m_grooveUpdates == null)
		{
			Init();
		}
		if (m_grooveUpdates.ContainsKey(type))
		{
			return m_grooveUpdates[type];
		}
		return null;
	}

	public static int CardCount()
	{
		return m_cardBox.Count();
	}

	public static int MaxLv(int type)
	{
		return FindCardGrooveUpdate(type).Count - 1;
	}

	public static int GetLevel(int GP, int type)
	{
		if (GP >= FindCardGrooveUpdate(type)[MaxLv(type)].Exp)
		{
			return FindCardGrooveUpdate(type)[MaxLv(type)].Level;
		}
		for (int i = 1; i <= MaxLv(type); i++)
		{
			if (GP < FindCardGrooveUpdate(type)[i].Exp)
			{
				int index = ((i - 1 != -1) ? (i - 1) : 0);
				return FindCardGrooveUpdate(type)[index].Level;
			}
		}
		return 0;
	}

	public static int GetProp(UsersCardInfo slot, int type)
	{
		int num = 0;
		for (int i = 0; i < slot.Level; i++)
		{
			num += GetGrooveSlot(slot.Type, i, type);
		}
		if (slot.CardID != 0)
		{
			num += GetPropCard(slot.CardType, slot.CardID, type);
		}
		return num;
	}

	public static int GetGrooveSlot(int type, int lv, int typeProp)
	{
		CardGrooveUpdateInfo[] grooveUpdate = m_grooveUpdate;
		foreach (CardGrooveUpdateInfo cardGrooveUpdateInfo in grooveUpdate)
		{
			if (cardGrooveUpdateInfo.Type == type && cardGrooveUpdateInfo.Level == lv)
			{
				int result;
				switch (typeProp)
				{
				case 0:
					result = cardGrooveUpdateInfo.Attack;
					break;
				case 1:
					result = cardGrooveUpdateInfo.Defend;
					break;
				case 2:
					result = cardGrooveUpdateInfo.Agility;
					break;
				case 3:
					result = cardGrooveUpdateInfo.Lucky;
					break;
				case 4:
					result = cardGrooveUpdateInfo.Damage;
					break;
				case 5:
					result = cardGrooveUpdateInfo.Guard;
					break;
				default:
					continue;
				}
				return result;
			}
		}
		return 0;
	}

	public static int GetPropCard(int cardtype, int cardID, int type)
	{
		CardTemplateInfo[] cardBox = m_cardBox;
		foreach (CardTemplateInfo cardTemplateInfo in cardBox)
		{
			if (cardTemplateInfo.CardType == cardtype && cardTemplateInfo.CardID == cardID)
			{
				int result;
				switch (type)
				{
				case 0:
					result = cardTemplateInfo.AddAttack;
					break;
				case 1:
					result = cardTemplateInfo.AddDefend;
					break;
				case 2:
					result = cardTemplateInfo.AddAgility;
					break;
				case 3:
					result = cardTemplateInfo.AddLucky;
					break;
				case 4:
					result = cardTemplateInfo.AddDamage;
					break;
				case 5:
					result = cardTemplateInfo.AddGuard;
					break;
				default:
					continue;
				}
				return result;
			}
		}
		return 0;
	}

	public static int GetGP(int level, int type)
	{
		for (int i = 1; i <= MaxLv(type); i++)
		{
			if (level == FindCardGrooveUpdate(type)[i].Level)
			{
				return FindCardGrooveUpdate(type)[i].Exp;
			}
		}
		return 0;
	}
}
