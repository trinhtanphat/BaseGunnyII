using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils
{

public abstract class CardAbstractInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	private int m_capalility;

	private int m_beginSlot;

	protected UsersCardInfo[] m_cards;

	protected UsersCardInfo temp_card;

	protected List<int> m_changedPlaces = new List<int>();

	private int m_changeCount;

	public int BeginSlot => m_beginSlot;

	public int Capalility
	{
		get
		{
			return m_capalility;
		}
		set
		{
			m_capalility = ((value >= 0) ? ((value > m_cards.Length) ? m_cards.Length : value) : 0);
		}
	}

	public bool IsEmpty(int slot)
	{
		return slot < 0 || slot >= m_capalility || m_cards[slot] == null;
	}

	public CardAbstractInventory(int capability, int beginSlot)
	{
		m_capalility = capability;
		m_beginSlot = beginSlot;
		m_cards = new UsersCardInfo[capability];
		temp_card = new UsersCardInfo();
	}

	public virtual void UpdateTempCard(UsersCardInfo card)
	{
		lock (m_lock)
		{
			temp_card = card;
		}
	}

	public virtual void UpdateCard()
	{
		int place = temp_card.Place;
		int templateID = temp_card.TemplateID;
		int toSlot;
		if (place < 5)
		{
			ReplaceCardTo(temp_card, place);
			toSlot = FindPlaceByTamplateId(5, templateID);
			MoveCard(place, toSlot);
			return;
		}
		ReplaceCardTo(temp_card, place);
		toSlot = FindPlaceByTamplateId(0, 5, templateID);
		if (GetItemAt(toSlot) != null && GetItemAt(toSlot).TemplateID == templateID)
		{
			MoveCard(place, toSlot);
		}
	}

	public virtual bool UpdateCardType(int templateID, int type)
	{
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				if (m_cards[i] != null && m_cards[i].TemplateID == templateID)
				{
					num = i;
					m_cards[i].CardType = type;
				}
			}
			if (num != -1)
			{
				OnPlaceChanged(num);
			}
		}
		return num != -1;
	}

	public bool RemoveCardAt(int place)
	{
		return RemoveCard(GetItemAt(place));
	}

	public virtual bool RemoveCard(UsersCardInfo item)
	{
		if (item == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				if (m_cards[i] == item)
				{
					num = i;
					m_cards[i] = null;
					break;
				}
			}
		}
		if (num != -1)
		{
			OnPlaceChanged(num);
			item.Place = -1;
		}
		return num != -1;
	}

	public bool AddCard(UsersCardInfo card)
	{
		return AddCard(card, m_beginSlot);
	}

	public bool AddCard(UsersCardInfo card, int minSlot)
	{
		if (card == null)
		{
			return false;
		}
		int place = FindFirstEmptySlot(minSlot);
		return AddCardTo(card, place);
	}

	public virtual bool AddCardTo(UsersCardInfo card, int place)
	{
		if (card == null || place >= m_capalility || place < 0)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_cards[place] != null)
			{
				place = -1;
			}
			else
			{
				m_cards[place] = card;
				card.Place = place;
			}
		}
		if (place != -1)
		{
			OnPlaceChanged(place);
		}
		return place != -1;
	}

	public virtual bool ReplaceCardTo(UsersCardInfo card, int place)
	{
		if (card == null || place >= m_capalility || place < 0)
		{
			return false;
		}
		lock (m_lock)
		{
			m_cards[place] = card;
			card.Place = place;
			OnPlaceChanged(place);
		}
		return true;
	}

	public virtual bool MoveCard(int fromSlot, int toSlot)
	{
		if (fromSlot < 0 || toSlot < 0 || fromSlot >= m_capalility || toSlot >= m_capalility)
		{
			return false;
		}
		bool flag = false;
		lock (m_lock)
		{
			flag = ExchangeCards(fromSlot, toSlot);
		}
		if (flag)
		{
			BeginChanges();
			try
			{
				OnPlaceChanged(toSlot);
			}
			finally
			{
				CommitChanges();
			}
		}
		return flag;
	}

	public bool IsSolt(int slot)
	{
		return slot >= 0 && slot < m_capalility;
	}

	protected virtual bool ExchangeCards(int fromSlot, int toSlot)
	{
		UsersCardInfo usersCardInfo = m_cards[fromSlot];
		if (usersCardInfo == null)
		{
			return false;
		}
		if (fromSlot == toSlot)
		{
			m_cards[toSlot].CardType = 0;
			m_cards[toSlot].TemplateID = 0;
			m_cards[toSlot].Attack = 0;
			m_cards[toSlot].Defence = 0;
			m_cards[toSlot].Agility = 0;
			m_cards[toSlot].Luck = 0;
			m_cards[toSlot].Damage = 0;
			m_cards[toSlot].Guard = 0;
		}
		else
		{
			m_cards[toSlot].CardType = usersCardInfo.CardType;
			m_cards[toSlot].TemplateID = usersCardInfo.TemplateID;
			m_cards[toSlot].Attack = usersCardInfo.Attack;
			m_cards[toSlot].Defence = usersCardInfo.Defence;
			m_cards[toSlot].Agility = usersCardInfo.Agility;
			m_cards[toSlot].Luck = usersCardInfo.Luck;
			m_cards[toSlot].Damage = usersCardInfo.Damage;
			m_cards[toSlot].Guard = usersCardInfo.Guard;
		}
		return true;
	}

	public virtual bool ResetCardSoul()
	{
		lock (m_lock)
		{
			for (int i = 0; i < 5; i++)
			{
				m_cards[i].Level = 0;
				m_cards[i].CardGP = 0;
			}
		}
		return true;
	}

	public virtual bool UpGraceSlot(int soulPoint, int lv, int place)
	{
		lock (m_lock)
		{
			m_cards[place].CardGP += soulPoint;
			m_cards[place].Level = lv;
		}
		return true;
	}

	public virtual UsersCardInfo GetItemAt(int slot)
	{
		if (slot < 0 || slot >= m_capalility)
		{
			return null;
		}
		return m_cards[slot];
	}

	public int FindFirstEmptySlot()
	{
		return FindFirstEmptySlot(m_beginSlot);
	}

	public int FindFirstEmptySlot(int minSlot)
	{
		if (minSlot >= m_capalility)
		{
			return -1;
		}
		int result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_cards[i] == null)
				{
					result = i;
					return result;
				}
			}
			result = -1;
		}
		return result;
	}

	public int FindPlaceByTamplateId(int minSlot, int templateId)
	{
		if (minSlot >= m_capalility)
		{
			return -1;
		}
		int result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_cards[i] != null && m_cards[i].TemplateID == templateId)
				{
					result = m_cards[i].Place;
					return result;
				}
			}
			result = -1;
		}
		return result;
	}

	public bool FindEquipCard(int templateId)
	{
		bool result;
		lock (m_lock)
		{
			for (int i = 0; i < 5; i++)
			{
				if (m_cards[i].TemplateID == templateId)
				{
					result = true;
					return result;
				}
			}
			result = false;
		}
		return result;
	}

	public int FindPlaceByTamplateId(int minSlot, int maxSlot, int templateId)
	{
		if (minSlot >= m_capalility)
		{
			return -1;
		}
		int result;
		lock (m_lock)
		{
			for (int i = minSlot; i < maxSlot; i++)
			{
				if (m_cards[i].TemplateID == templateId)
				{
					result = m_cards[i].Place;
					return result;
				}
			}
			result = -1;
		}
		return result;
	}

	public int FindLastEmptySlot()
	{
		int result;
		lock (m_lock)
		{
			for (int num = m_capalility - 1; num >= 0; num--)
			{
				if (m_cards[num] == null)
				{
					result = num;
					return result;
				}
			}
			result = -1;
		}
		return result;
	}

	public virtual void Clear()
	{
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				m_cards[i] = null;
			}
		}
	}

	public virtual UsersCardInfo GetItemByTemplateID(int minSlot, int templateId)
	{
		UsersCardInfo result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_cards[i] != null && m_cards[i].TemplateID == templateId)
				{
					result = m_cards[i];
					return result;
				}
			}
			result = null;
		}
		return result;
	}

	public virtual UsersCardInfo GetItemByPlace(int minSlot, int place)
	{
		UsersCardInfo result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_cards[i] != null && m_cards[i].Place == place)
				{
					result = m_cards[i];
					return result;
				}
			}
			result = null;
		}
		return result;
	}

	public virtual List<UsersCardInfo> GetCards()
	{
		return GetCards(0, m_capalility);
	}

	public virtual List<UsersCardInfo> GetCards(int minSlot, int maxSlot)
	{
		List<UsersCardInfo> list = new List<UsersCardInfo>();
		lock (m_lock)
		{
			for (int i = minSlot; i < maxSlot; i++)
			{
				if (m_cards[i] != null)
				{
					list.Add(m_cards[i]);
				}
			}
		}
		return list;
	}

	public int GetEmptyCount()
	{
		return GetEmptyCount(m_beginSlot);
	}

	public virtual int GetEmptyCount(int minSlot)
	{
		if (minSlot < 0 || minSlot > m_capalility - 1)
		{
			return 0;
		}
		int num = 0;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_cards[i] == null)
				{
					num++;
				}
			}
		}
		return num;
	}

	protected void OnPlaceChanged(int place)
	{
		if (!m_changedPlaces.Contains(place))
		{
			m_changedPlaces.Add(place);
		}
		if (m_changeCount <= 0 && m_changedPlaces.Count > 0)
		{
			UpdateChangedPlaces();
		}
	}

	public void BeginChanges()
	{
		Interlocked.Increment(ref m_changeCount);
	}

	public void CommitChanges()
	{
		int num = Interlocked.Decrement(ref m_changeCount);
		if (num < 0)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
			}
			Thread.VolatileWrite(ref m_changeCount, 0);
		}
		if (num <= 0 && m_changedPlaces.Count > 0)
		{
			UpdateChangedPlaces();
		}
	}

	public virtual void UpdateChangedPlaces()
	{
		m_changedPlaces.Clear();
	}

	public void ClearBag()
	{
		BeginChanges();
		lock (m_lock)
		{
			for (int i = 5; i < m_capalility; i++)
			{
				if (m_cards[i] != null)
				{
					RemoveCard(m_cards[i]);
				}
			}
		}
		CommitChanges();
	}

	public UsersCardInfo[] GetRawSpaces()
	{
		lock (m_lock)
		{
			return m_cards.Clone() as UsersCardInfo[];
		}
	}
}
}
