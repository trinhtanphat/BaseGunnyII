using System;
using System.Collections.Generic;
using System.Text;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerInventory : AbstractInventory
{
	protected GamePlayer m_player;

	private bool m_saveToDb;

	private List<ItemInfo> m_removedList = new List<ItemInfo>();

	public GamePlayer Player => m_player;

	public PlayerInventory(GamePlayer player, bool saveTodb, int capibility, int type, int beginSlot, bool autoStack)
		: base(capibility, type, beginSlot, autoStack)
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
		ItemInfo[] userBagByType = playerBussiness.GetUserBagByType(m_player.PlayerCharacter.ID, base.BagType);
		BeginChanges();
		try
		{
			new List<ItemInfo>();
			ItemInfo[] array = userBagByType;
			foreach (ItemInfo itemInfo in array)
			{
				if (IsWrongPlace(itemInfo) && itemInfo.Place < 31 && base.BagType == 0)
				{
					int num = FindFirstEmptySlot(31);
					if (num != -1)
					{
						MoveItem(itemInfo.Place, num, itemInfo.Count);
					}
					else
					{
						m_player.AddTemplate(itemInfo);
					}
				}
				else
				{
					AddItemTo(itemInfo, itemInfo.Place);
				}
			}
		}
		finally
		{
			CommitChanges();
		}
	}

	public bool IsWrongPlace(ItemInfo item)
	{
		return item != null && item.Template != null && ((item.Template.CategoryID == 7 && item.Place != 6) || (item.Template.CategoryID == 27 && item.Place != 6) || (item.Template.CategoryID == 17 && item.Place != 15) || (item.Template.CategoryID == 31 && item.Place != 15));
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
			for (int i = 0; i < m_items.Length; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (itemInfo != null && itemInfo.IsDirty)
				{
					if (itemInfo.ItemID > 0)
					{
						playerBussiness.UpdateGoods(itemInfo);
					}
					else
					{
						playerBussiness.AddGoods(itemInfo);
					}
				}
			}
		}
		lock (m_removedList)
		{
			foreach (ItemInfo removed in m_removedList)
			{
				if (removed.ItemID > 0)
				{
					playerBussiness.UpdateGoods(removed);
				}
			}
			m_removedList.Clear();
		}
	}

	public override bool AddItemTo(ItemInfo item, int place)
	{
		if (base.AddItemTo(item, place))
		{
			item.UserID = m_player.PlayerCharacter.ID;
			item.IsExist = true;
			return true;
		}
		return false;
	}

	public override bool TakeOutItem(ItemInfo item)
	{
		if (base.TakeOutItem(item))
		{
			if (m_saveToDb)
			{
				lock (m_removedList)
				{
					m_removedList.Add(item);
				}
			}
			return true;
		}
		return false;
	}

	public override bool RemoveItem(ItemInfo item)
	{
		if (base.RemoveItem(item))
		{
			item.IsExist = false;
			if (m_saveToDb)
			{
				lock (m_removedList)
				{
					m_removedList.Add(item);
				}
			}
			return true;
		}
		return false;
	}

	public override void UpdateChangedPlaces()
	{
		int[] updatedSlots = m_changedPlaces.ToArray();
		m_player.Out.SendUpdateInventorySlot(this, updatedSlots);
		base.UpdateChangedPlaces();
	}

	public bool SendAllItemsToMail(string sender, string title, eMailType type)
	{
		if (m_saveToDb)
		{
			BeginChanges();
			try
			{
				using PlayerBussiness pb = new PlayerBussiness();
				lock (m_lock)
				{
					List<ItemInfo> items = GetItems();
					int count = items.Count;
					for (int i = 0; i < count; i += 5)
					{
						MailInfo mailInfo = new MailInfo();
						mailInfo.SenderID = 0;
						mailInfo.Sender = sender;
						mailInfo.ReceiverID = m_player.PlayerCharacter.ID;
						mailInfo.Receiver = m_player.PlayerCharacter.NickName;
						mailInfo.Title = title;
						mailInfo.Type = (int)type;
						mailInfo.Content = "";
						List<ItemInfo> list = new List<ItemInfo>();
						for (int j = 0; j < 5; j++)
						{
							int num = i * 5 + j;
							if (num < items.Count)
							{
								list.Add(items[num]);
							}
						}
						if (!SendItemsToMail(list, mailInfo, pb))
						{
							return false;
						}
					}
				}
			}
			catch (Exception ex)
			{
				Console.WriteLine("Send Items Mail Error:" + ex);
			}
			finally
			{
				SaveToDatabase();
				CommitChanges();
			}
			m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			return true;
		}
		return true;
	}

	public bool SendItemsToMail(List<ItemInfo> items, MailInfo mail, PlayerBussiness pb)
	{
		if (mail == null)
		{
			return false;
		}
		if (items.Count > 5)
		{
			return false;
		}
		if (m_saveToDb)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			if (items.Count > 0 && TakeOutItem(items[0]))
			{
				ItemInfo itemInfo = items[0];
				mail.Annex1 = itemInfo.ItemID.ToString();
				mail.Annex1Name = itemInfo.Template.Name;
				stringBuilder.Append("1、" + mail.Annex1Name + "x" + itemInfo.Count + ";");
				list.Add(itemInfo);
			}
			if (items.Count > 1 && TakeOutItem(items[1]))
			{
				ItemInfo itemInfo2 = items[1];
				mail.Annex2 = itemInfo2.ItemID.ToString();
				mail.Annex2Name = itemInfo2.Template.Name;
				stringBuilder.Append("2、" + mail.Annex2Name + "x" + itemInfo2.Count + ";");
				list.Add(itemInfo2);
			}
			if (items.Count > 2 && TakeOutItem(items[2]))
			{
				ItemInfo itemInfo3 = items[2];
				mail.Annex3 = itemInfo3.ItemID.ToString();
				mail.Annex3Name = itemInfo3.Template.Name;
				stringBuilder.Append("3、" + mail.Annex3Name + "x" + itemInfo3.Count + ";");
				list.Add(itemInfo3);
			}
			if (items.Count > 3 && TakeOutItem(items[3]))
			{
				ItemInfo itemInfo4 = items[3];
				mail.Annex4 = itemInfo4.ItemID.ToString();
				mail.Annex4Name = itemInfo4.Template.Name;
				stringBuilder.Append("4、" + mail.Annex4Name + "x" + itemInfo4.Count + ";");
				list.Add(itemInfo4);
			}
			if (items.Count > 4 && TakeOutItem(items[4]))
			{
				ItemInfo itemInfo5 = items[4];
				mail.Annex5 = itemInfo5.ItemID.ToString();
				mail.Annex5Name = itemInfo5.Template.Name;
				stringBuilder.Append("5、" + mail.Annex5Name + "x" + itemInfo5.Count + ";");
				list.Add(itemInfo5);
			}
			mail.AnnexRemark = stringBuilder.ToString();
			if (pb.SendMail(mail))
			{
				return true;
			}
			foreach (ItemInfo item in list)
			{
				AddItem(item);
			}
		}
		return false;
	}

	public bool SendItemToMail(ItemInfo item)
	{
		if (m_saveToDb)
		{
			using (PlayerBussiness pb = new PlayerBussiness())
			{
				return SendItemToMail(item, pb, null);
			}
		}
		return false;
	}

	public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, MailInfo mail)
	{
		if (!m_saveToDb || item.BagType != base.BagType)
		{
			return false;
		}
		if (mail == null)
		{
			mail = new MailInfo();
			mail.Annex1 = item.ItemID.ToString();
			mail.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
			mail.Gold = 0;
			mail.IsExist = true;
			mail.Money = 0;
			mail.Receiver = m_player.PlayerCharacter.NickName;
			mail.ReceiverID = item.UserID;
			mail.Sender = m_player.PlayerCharacter.NickName;
			mail.SenderID = item.UserID;
			mail.Title = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
			mail.Type = 9;
		}
		if (pb.SendMail(mail))
		{
			RemoveItem(item);
			item.IsExist = true;
			return true;
		}
		return false;
	}
}
