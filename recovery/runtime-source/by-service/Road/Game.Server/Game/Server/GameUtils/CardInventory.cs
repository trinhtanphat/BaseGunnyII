using System.Collections.Generic;
using Bussiness;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class CardInventory : CardAbstractInventory
{
	protected GamePlayer m_player;

	private bool m_saveToDb;

	private List<UsersCardInfo> m_removedList = new List<UsersCardInfo>();

	public GamePlayer Player => m_player;

	public CardInventory(GamePlayer player, bool saveTodb, int capibility, int beginSlot)
		: base(capibility, beginSlot)
	{
		m_player = player;
		m_saveToDb = saveTodb;
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		UsersCardInfo[] userCardSingles = playerBussiness.GetUserCardSingles(m_player.PlayerCharacter.ID);
		BeginChanges();
		try
		{
			UsersCardInfo[] array = userCardSingles;
			foreach (UsersCardInfo usersCardInfo in array)
			{
				AddCardTo(usersCardInfo, usersCardInfo.Place);
			}
		}
		finally
		{
			CommitChanges();
		}
	}

	public virtual void SaveToDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			for (int i = 0; i < m_cards.Length; i++)
			{
				UsersCardInfo usersCardInfo = m_cards[i];
				if (usersCardInfo != null && usersCardInfo.IsDirty)
				{
					if (usersCardInfo.CardID > 0)
					{
						playerBussiness.UpdateCards(usersCardInfo);
					}
					else
					{
						playerBussiness.AddCards(usersCardInfo);
					}
				}
			}
		}
		lock (m_removedList)
		{
			foreach (UsersCardInfo removed in m_removedList)
			{
				if (removed.CardID > 0)
				{
					playerBussiness.UpdateCards(removed);
				}
			}
			m_removedList.Clear();
		}
	}

	public override bool AddCardTo(UsersCardInfo item, int place)
	{
		if (base.AddCardTo(item, place))
		{
			item.UserID = m_player.PlayerCharacter.ID;
			item.IsExit = true;
			return true;
		}
		return false;
	}

	public override void UpdateChangedPlaces()
	{
		int[] updatedSlots = m_changedPlaces.ToArray();
		m_player.Out.SendPlayerCardInfo(this, updatedSlots);
		m_player.Out.SendPlayerCardSlot(m_player.PlayerCharacter, GetCards(0, 5));
		base.UpdateChangedPlaces();
	}
}
