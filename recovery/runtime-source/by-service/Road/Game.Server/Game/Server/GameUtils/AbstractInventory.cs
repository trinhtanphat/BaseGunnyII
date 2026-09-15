using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Game.Logic;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public abstract class AbstractInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	private int m_type;

	private int m_capalility;

	private int m_beginSlot;

	private bool m_autoStack;

	protected ItemInfo[] m_items;

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
			m_capalility = ((value >= 0) ? ((value > m_items.Length) ? m_items.Length : value) : 0);
		}
	}

	public int BagType => m_type;

	public bool IsEmpty(int slot)
	{
		return slot < 0 || slot >= m_capalility || m_items[slot] == null;
	}

	public AbstractInventory(int capability, int type, int beginSlot, bool autoStack)
	{
		m_capalility = capability;
		m_type = type;
		m_beginSlot = beginSlot;
		m_autoStack = autoStack;
		m_items = new ItemInfo[capability];
	}

	public bool AddItem(ItemInfo item)
	{
		return AddItem(item, m_beginSlot);
	}

	public bool AddItem(ItemInfo item, int minSlot)
	{
		if (item == null)
		{
			return false;
		}
		int place = FindFirstEmptySlot(minSlot);
		return AddItemTo(item, place);
	}

	public virtual bool AddItemTo(ItemInfo item, int place)
	{
		if (item == null || place >= m_capalility || place < 0)
		{
			return false;
		}
		RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
		if (runeTemplateInfo != null)
		{
			int baseLevel = runeTemplateInfo.BaseLevel;
			int hole = RuneMgr.FindRuneExp(baseLevel - 1);
			if (item.IsBead() && item.Hole1 <= 0 && item.Hole2 <= 0)
			{
				item.Hole1 = baseLevel;
				item.Hole2 = hole;
			}
		}
		lock (m_lock)
		{
			if (m_items[place] != null)
			{
				place = -1;
			}
			else
			{
				m_items[place] = item;
				item.Place = place;
				item.BagType = m_type;
			}
		}
		if (place != -1)
		{
			OnPlaceChanged(place);
		}
		return place != -1;
	}

	public virtual bool TakeOutItem(ItemInfo item)
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
				if (m_items[i] == item)
				{
					num = i;
					m_items[i] = null;
					break;
				}
			}
		}
		if (num != -1)
		{
			OnPlaceChanged(num);
			if (item.BagType == BagType)
			{
				item.Place = -1;
				item.BagType = -1;
			}
		}
		return num != -1;
	}

	public bool TakeOutItemAt(int place)
	{
		return TakeOutItem(GetItemAt(place));
	}

	public void RemoveAllItem(List<int> places)
	{
		BeginChanges();
		lock (m_lock)
		{
			for (int i = 0; i < places.Count; i++)
			{
				int num = places[i];
				if (m_items[num] != null)
				{
					RemoveItem(m_items[num]);
				}
			}
		}
		CommitChanges();
	}

	public virtual bool RemoveItem(ItemInfo item)
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
				if (m_items[i] == item)
				{
					num = i;
					m_items[i] = null;
					break;
				}
			}
		}
		if (num != -1)
		{
			OnPlaceChanged(num);
			if (item.BagType == BagType)
			{
				item.Place = -1;
				item.BagType = -1;
			}
		}
		return num != -1;
	}

	public bool RemoveItemAt(int place)
	{
		return RemoveItem(GetItemAt(place));
	}

	public virtual bool AddCountToStack(ItemInfo item, int count)
	{
		if (item == null)
		{
			return false;
		}
		if (count <= 0 || item.BagType != m_type)
		{
			return false;
		}
		if (item.Count + count > item.Template.MaxCount)
		{
			return false;
		}
		item.Count += count;
		OnPlaceChanged(item.Place);
		return true;
	}

	public virtual bool RemoveCountFromStack(ItemInfo item, int count)
	{
		if (item == null)
		{
			return false;
		}
		if (count <= 0 || item.BagType != m_type)
		{
			return false;
		}
		if (item.Count < count)
		{
			return false;
		}
		if (item.Count == count)
		{
			return RemoveItem(item);
		}
		item.Count -= count;
		OnPlaceChanged(item.Place);
		return true;
	}

	public virtual bool AddTemplateAt(ItemInfo cloneItem, int count, int place)
	{
		return AddTemplate(cloneItem, count, place, m_capalility - 1);
	}

	public virtual bool AddTemplate(ItemInfo cloneItem, int count)
	{
		return AddTemplate(cloneItem, count, m_beginSlot, m_capalility - 1);
	}

	public virtual bool AddTemplate(ItemInfo cloneItem, int count, int minSlot, int maxSlot)
	{
		if (cloneItem == null)
		{
			return false;
		}
		ItemTemplateInfo template = cloneItem.Template;
		if (template == null)
		{
			return false;
		}
		if (count <= 0)
		{
			return false;
		}
		if (minSlot < m_beginSlot || minSlot > m_capalility - 1)
		{
			return false;
		}
		if (maxSlot < m_beginSlot || maxSlot > m_capalility - 1)
		{
			return false;
		}
		if (minSlot > maxSlot)
		{
			return false;
		}
		bool result;
		lock (m_lock)
		{
			List<int> list = new List<int>();
			int num = count;
			for (int i = minSlot; i <= maxSlot; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo == null)
				{
					num -= template.MaxCount;
					list.Add(i);
				}
				else if (m_autoStack && cloneItem.CanStackedTo(itemInfo))
				{
					num -= template.MaxCount - itemInfo.Count;
					list.Add(i);
				}
				if (num <= 0)
				{
					break;
				}
			}
			if (num <= 0)
			{
				BeginChanges();
				try
				{
					num = count;
					foreach (int item in list)
					{
						ItemInfo itemInfo2 = m_items[item];
						if (itemInfo2 == null)
						{
							itemInfo2 = cloneItem.Clone();
							itemInfo2.Count = ((num < template.MaxCount) ? num : template.MaxCount);
							num -= itemInfo2.Count;
							AddItemTo(itemInfo2, item);
						}
						else if (itemInfo2.TemplateID == template.TemplateID)
						{
							int num2 = ((itemInfo2.Count + num < template.MaxCount) ? num : (template.MaxCount - itemInfo2.Count));
							itemInfo2.Count += num2;
							num -= num2;
							OnPlaceChanged(item);
						}
						else
						{
							log.Error("Add template erro: select slot's TemplateId not equest templateId");
						}
					}
					if (num != 0)
					{
						log.Error("Add template error: last count not equal Zero.");
					}
				}
				finally
				{
					CommitChanges();
				}
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public virtual bool RemoveTemplate(int templateId, int count)
	{
		return RemoveTemplate(templateId, count, 0, m_capalility - 1);
	}

	public virtual bool RemoveTemplate(int templateId, int count, int minSlot, int maxSlot)
	{
		if (count <= 0)
		{
			return false;
		}
		if (minSlot < 0 || minSlot > m_capalility - 1)
		{
			return false;
		}
		if (maxSlot <= 0 || maxSlot > m_capalility - 1)
		{
			return false;
		}
		if (minSlot > maxSlot)
		{
			return false;
		}
		bool result;
		lock (m_lock)
		{
			List<int> list = new List<int>();
			int num = count;
			for (int i = minSlot; i <= maxSlot; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo != null && itemInfo.TemplateID == templateId)
				{
					list.Add(i);
					num -= itemInfo.Count;
					if (num <= 0)
					{
						break;
					}
				}
			}
			if (num <= 0)
			{
				BeginChanges();
				num = count;
				try
				{
					foreach (int item in list)
					{
						ItemInfo itemInfo2 = m_items[item];
						if (itemInfo2 != null && itemInfo2.TemplateID == templateId)
						{
							if (itemInfo2.Count <= num)
							{
								RemoveItem(itemInfo2);
								num -= itemInfo2.Count;
								continue;
							}
							int num2 = ((itemInfo2.Count - num < itemInfo2.Count) ? num : 0);
							itemInfo2.Count -= num2;
							num -= num2;
							OnPlaceChanged(item);
						}
					}
					if (num != 0)
					{
						log.Error("Remove templat error:last itemcoutj not equal Zero.");
					}
				}
				finally
				{
					CommitChanges();
				}
				result = true;
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public virtual bool MoveItem(int fromSlot, int toSlot, int count)
	{
		if (fromSlot < 0 || toSlot < 0 || fromSlot >= m_capalility || toSlot >= m_capalility)
		{
			return false;
		}
		bool flag = false;
		lock (m_lock)
		{
			flag = CombineItems(fromSlot, toSlot) || StackItems(fromSlot, toSlot, count) || ExchangeItems(fromSlot, toSlot);
		}
		if (flag)
		{
			BeginChanges();
			try
			{
				OnPlaceChanged(fromSlot);
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

	public void ClearBag()
	{
		BeginChanges();
		lock (m_lock)
		{
			for (int i = m_beginSlot; i < m_capalility; i++)
			{
				if (m_items[i] != null)
				{
					RemoveItem(m_items[i]);
				}
			}
		}
		CommitChanges();
	}

	public bool StackItemToAnother(ItemInfo item)
	{
		lock (m_lock)
		{
			for (int num = m_capalility - 1; num >= 0; num--)
			{
				if (item != null && m_items[num] != null && m_items[num] != item && item.CanStackedTo(m_items[num]) && m_items[num].Count + item.Count <= item.Template.MaxCount)
				{
					m_items[num].Count += item.Count;
					item.IsExist = false;
					item.RemoveType = 26;
					UpdateItem(m_items[num]);
					return true;
				}
			}
		}
		return false;
	}

	protected virtual bool CombineItems(int fromSlot, int toSlot)
	{
		return false;
	}

	protected virtual bool StackItems(int fromSlot, int toSlot, int itemCount)
	{
		ItemInfo itemInfo = m_items[fromSlot];
		ItemInfo itemInfo2 = m_items[toSlot];
		if (itemCount == 0)
		{
			itemCount = ((itemInfo.Count <= 0) ? 1 : itemInfo.Count);
		}
		if (itemInfo2 != null && itemInfo2.TemplateID == itemInfo.TemplateID && itemInfo2.CanStackedTo(itemInfo))
		{
			if (itemInfo.Count + itemInfo2.Count > itemInfo.Template.MaxCount)
			{
				itemInfo.Count -= itemInfo2.Template.MaxCount - itemInfo2.Count;
				itemInfo2.Count = itemInfo2.Template.MaxCount;
			}
			else
			{
				itemInfo2.Count += itemCount;
				RemoveItem(itemInfo);
			}
			return true;
		}
		if (itemInfo2 != null || itemInfo.Count <= itemCount)
		{
			return false;
		}
		ItemInfo itemInfo3 = itemInfo.Clone();
		itemInfo3.Count = itemCount;
		if (AddItemTo(itemInfo3, toSlot))
		{
			itemInfo.Count -= itemCount;
			return true;
		}
		return false;
	}

	protected virtual bool ExchangeItems(int fromSlot, int toSlot)
	{
		ItemInfo itemInfo = m_items[toSlot];
		ItemInfo itemInfo2 = m_items[fromSlot];
		m_items[fromSlot] = itemInfo;
		m_items[toSlot] = itemInfo2;
		if (itemInfo != null)
		{
			itemInfo.Place = fromSlot;
		}
		if (itemInfo2 != null)
		{
			itemInfo2.Place = toSlot;
		}
		return true;
	}

	public virtual ItemInfo GetItemAt(int slot)
	{
		if (slot < 0 || slot >= m_capalility)
		{
			return null;
		}
		return m_items[slot];
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
				if (m_items[i] == null)
				{
					result = i;
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
				if (m_items[num] == null)
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
				m_items[i] = null;
			}
		}
	}

	public virtual ItemInfo GetItemByCategoryID(int minSlot, int categoryID, int property)
	{
		ItemInfo result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_items[i] != null && m_items[i].Template.CategoryID == categoryID && (property == -1 || m_items[i].Template.Property1 == property))
				{
					result = m_items[i];
					return result;
				}
			}
			result = null;
		}
		return result;
	}

	public virtual ItemInfo GetItemByTemplateID(int minSlot, int templateId)
	{
		ItemInfo result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_items[i] != null && m_items[i].TemplateID == templateId)
				{
					result = m_items[i];
					return result;
				}
			}
			result = null;
		}
		return result;
	}

	public virtual int GetItemCount(int templateId)
	{
		return GetItemCount(m_beginSlot, templateId);
	}

	public int GetItemCount(int minSlot, int templateId)
	{
		int num = 0;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_items[i] != null && m_items[i].TemplateID == templateId)
				{
					num += m_items[i].Count;
				}
			}
		}
		return num;
	}

	public virtual List<ItemInfo> GetItems()
	{
		return GetItems(0, m_capalility);
	}

	public virtual List<ItemInfo> GetItems(int minSlot, int maxSlot)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		lock (m_lock)
		{
			for (int i = minSlot; i < maxSlot; i++)
			{
				if (m_items[i] != null)
				{
					list.Add(m_items[i]);
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
				if (m_items[i] == null)
				{
					num++;
				}
			}
		}
		return num;
	}

	public virtual void UseItem(ItemInfo item)
	{
		bool flag = false;
		if (!item.IsBinds && (item.Template.BindType == 2 || item.Template.BindType == 3))
		{
			item.IsBinds = true;
			flag = true;
		}
		if (!item.IsUsed)
		{
			item.IsUsed = true;
			item.BeginDate = DateTime.Now;
			flag = true;
		}
		if (flag)
		{
			OnPlaceChanged(item.Place);
		}
	}

	public virtual void UpdateItem(ItemInfo item)
	{
		if (item.BagType == m_type)
		{
			if (item.Count <= 0)
			{
				RemoveItem(item);
			}
			else
			{
				OnPlaceChanged(item.Place);
			}
		}
	}

	public virtual bool RemoveCountFromStack(ItemInfo item, int count, eItemRemoveType type)
	{
		if (item == null)
		{
			return false;
		}
		if (count <= 0 || item.BagType != m_type)
		{
			return false;
		}
		if (item.Count < count)
		{
			return false;
		}
		if (item.Count == count)
		{
			return RemoveItem(item);
		}
		item.Count -= count;
		OnPlaceChanged(item.Place);
		return true;
	}

	public virtual bool RemoveItem(ItemInfo item, eItemRemoveType type)
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
				if (m_items[i] == item)
				{
					num = i;
					m_items[i] = null;
					break;
				}
			}
		}
		if (num != -1)
		{
			OnPlaceChanged(num);
			if (item.BagType == BagType && item.Place == num)
			{
				item.Place = -1;
				item.BagType = -1;
			}
		}
		return num != -1;
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

	public ItemInfo[] GetRawSpaces()
	{
		lock (m_lock)
		{
			return m_items.Clone() as ItemInfo[];
		}
	}
}
