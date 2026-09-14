using System;
using System.Collections.Generic;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerBeadInventory : PlayerInventory
{
	private const int BAG_START = 32;

	public PlayerBeadInventory(GamePlayer player)
		: base(player, saveTodb: true, 179, 21, 32, autoStack: false)
	{
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		try
		{
			base.LoadFromDatabase();
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 1; i < 32; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (m_items[i] != null && !m_items[i].IsValidItem())
				{
					int num = FindFirstEmptySlot(32);
					if (num >= 0)
					{
						MoveItem(itemInfo.Place, num, itemInfo.Count);
					}
					else
					{
						list.Add(itemInfo);
					}
				}
			}
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, null, null, eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
		finally
		{
			CommitChanges();
		}
	}

	public override bool MoveItem(int fromSlot, int toSlot, int count)
	{
		return m_items[fromSlot] != null && base.MoveItem(fromSlot, toSlot, count);
	}

	public override void UpdateChangedPlaces()
	{
		int[] array = m_changedPlaces.ToArray();
		int[] array2 = array;
		foreach (int slot in array2)
		{
			if (!IsEquipSlot(slot))
			{
				continue;
			}
			ItemInfo itemAt = GetItemAt(slot);
			if (itemAt != null)
			{
				itemAt.IsBinds = true;
				if (!itemAt.IsUsed)
				{
					itemAt.BeginDate = DateTime.Now;
				}
			}
			break;
		}
		base.UpdateChangedPlaces();
	}

	public bool IsEquipSlot(int slot)
	{
		return slot >= 0 && slot < 32;
	}
}
