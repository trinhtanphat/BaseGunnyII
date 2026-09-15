using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Quests;

public class QuestInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_lock;

	protected List<BaseQuest> m_list;

	protected List<QuestDataInfo> m_datas;

	protected ArrayList m_clearList;

	private GamePlayer m_player;

	private byte[] m_states;

	private UnicodeEncoding m_converter;

	protected List<BaseQuest> m_changedQuests = new List<BaseQuest>();

	private int m_changeCount;

	public QuestInventory(GamePlayer player)
	{
		m_converter = new UnicodeEncoding();
		m_player = player;
		m_lock = new object();
		m_list = new List<BaseQuest>();
		m_clearList = new ArrayList();
		m_datas = new List<QuestDataInfo>();
	}

	public void LoadFromDatabase(int playerId)
	{
		lock (m_lock)
		{
			m_states = ((m_player.PlayerCharacter.QuestSite.Length == 0) ? InitQuest() : m_player.PlayerCharacter.QuestSite);
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				QuestDataInfo[] userQuest = playerBussiness.GetUserQuest(playerId);
				BeginChanges();
				QuestDataInfo[] array = userQuest;
				foreach (QuestDataInfo questDataInfo in array)
				{
					QuestInfo singleQuest = QuestMgr.GetSingleQuest(questDataInfo.QuestID);
					if (singleQuest != null)
					{
						AddQuest(new BaseQuest(singleQuest, questDataInfo));
					}
					AddQuestData(questDataInfo);
				}
				CommitChanges();
			}
			List<BaseQuest> list = m_list;
		}
	}

	public void SaveToDatabase()
	{
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			foreach (BaseQuest item in m_list)
			{
				item.SaveData();
				if (item.Data.IsDirty)
				{
					playerBussiness.UpdateDbQuestDataInfo(item.Data);
				}
			}
			foreach (BaseQuest clear in m_clearList)
			{
				clear.SaveData();
				playerBussiness.UpdateDbQuestDataInfo(clear.Data);
			}
			m_clearList.Clear();
		}
	}

	private bool AddQuest(BaseQuest quest)
	{
		lock (m_list)
		{
			m_list.Add(quest);
		}
		OnQuestsChanged(quest);
		quest.AddToPlayer(m_player);
		return true;
	}

	private bool AddQuestData(QuestDataInfo data)
	{
		lock (m_list)
		{
			m_datas.Add(data);
		}
		return true;
	}

	public bool AddQuest(QuestInfo info, out string msg)
	{
		msg = "";
		try
		{
			if (info == null)
			{
				msg = "Game.Server.Quests.NoQuest";
				return false;
			}
			if (info.TimeMode && DateTime.Now.CompareTo(info.StartDate) < 0)
			{
				msg = "Game.Server.Quests.NoTime";
			}
			if (info.TimeMode && DateTime.Now.CompareTo(info.EndDate) > 0)
			{
				msg = "Game.Server.Quests.TimeOver";
			}
			if (m_player.PlayerCharacter.Grade < info.NeedMinLevel)
			{
				msg = "Game.Server.Quests.LevelLow";
			}
			if (m_player.PlayerCharacter.Grade > info.NeedMaxLevel)
			{
				msg = "Game.Server.Quests.LevelTop";
			}
			if (info.PreQuestID != "0,")
			{
				string[] array = info.PreQuestID.Split(',');
				for (int i = 0; i < array.Length - 1; i++)
				{
					if (!IsQuestFinish(Convert.ToInt32(array[i])))
					{
						msg = "Game.Server.Quests.NoFinish";
					}
				}
			}
		}
		catch (Exception ex)
		{
			log.Info(ex.InnerException);
		}
		if (info.IsOther == 1 && !m_player.PlayerCharacter.IsConsortia)
		{
			msg = "Game.Server.Quest.QuestInventory.HaveMarry";
		}
		if (info.IsOther == 2 && !m_player.PlayerCharacter.IsMarried)
		{
			msg = "Game.Server.Quest.QuestInventory.HaveMarry";
		}
		BaseQuest baseQuest = FindQuest(info.ID);
		if (baseQuest != null && baseQuest.Data.IsComplete)
		{
			msg = "Game.Server.Quests.Have";
		}
		if (baseQuest != null && !baseQuest.Info.CanRepeat)
		{
			msg = "Game.Server.Quests.NoRepeat";
		}
		if (baseQuest != null && DateTime.Now.CompareTo(baseQuest.Data.CompletedDate.Date.AddDays(baseQuest.Info.RepeatInterval)) < 0 && baseQuest.Data.RepeatFinish < 1)
		{
			msg = "Game.Server.Quests.Rest";
		}
		BaseQuest baseQuest2 = m_player.QuestInventory.FindQuest(info.ID);
		if (baseQuest2 != null)
		{
			msg = "Game.Server.Quests.Have";
		}
		if (msg == "")
		{
			QuestMgr.GetQuestCondiction(info);
			int rand = 1;
			if ((decimal)ThreadSafeRandom.NextStatic(1000000) <= info.Rands)
			{
				rand = info.RandDouble;
			}
			BeginChanges();
			if (baseQuest == null)
			{
				baseQuest = new BaseQuest(info, new QuestDataInfo());
				AddQuest(baseQuest);
				baseQuest.Reset(m_player, rand);
			}
			else
			{
				baseQuest.Reset(m_player, rand);
				baseQuest.AddToPlayer(m_player);
				OnQuestsChanged(baseQuest);
			}
			CommitChanges();
			return true;
		}
		msg = LanguageMgr.GetTranslation(msg);
		return false;
	}

	public bool FindFinishQuestData(int ID, int UserID)
	{
		bool result = false;
		lock (m_datas)
		{
			foreach (QuestDataInfo data in m_datas)
			{
				if (data.QuestID == ID && data.UserID == UserID)
				{
					result = data.IsComplete;
				}
			}
		}
		return result;
	}

	public bool RemoveQuest(BaseQuest quest)
	{
		if (!quest.Info.CanRepeat)
		{
			bool flag = false;
			lock (m_list)
			{
				if (m_list.Remove(quest))
				{
					m_clearList.Add(quest);
					flag = true;
				}
			}
			if (flag)
			{
				quest.RemoveFromPlayer(m_player);
				OnQuestsChanged(quest);
			}
			return flag;
		}
		quest.Reset(m_player, 2);
		quest.Data.RepeatFinish++;
		quest.SaveData();
		OnQuestsChanged(quest);
		return true;
	}

	public void Update(BaseQuest quest)
	{
		OnQuestsChanged(quest);
	}

	public bool Finish(BaseQuest baseQuest, int selectedItem)
	{
		string text = "";
		string empty = string.Empty;
		QuestInfo info = baseQuest.Info;
		QuestDataInfo data = baseQuest.Data;
		m_player.BeginAllChanges();
		try
		{
			if (baseQuest.Finish(m_player))
			{
				RemoveQuest(baseQuest);
				List<QuestAwardInfo> questGoods = QuestMgr.GetQuestGoods(info);
				List<ItemInfo> list = new List<ItemInfo>();
				List<ItemInfo> list2 = new List<ItemInfo>();
				List<ItemInfo> list3 = new List<ItemInfo>();
				List<ItemInfo> list4 = new List<ItemInfo>();
				List<ItemInfo> list5 = new List<ItemInfo>();
				foreach (QuestAwardInfo item in questGoods)
				{
					if (item.IsSelect && item.RewardItemID != selectedItem)
					{
						continue;
					}
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(item.RewardItemID);
					if (itemTemplateInfo == null)
					{
						continue;
					}
					text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardProp", itemTemplateInfo.Name, item.RewardItemCount1) + " ";
					int num = item.RewardItemCount1;
					if (item.IsCount)
					{
						num *= data.RandDobule;
					}
					for (int i = 0; i < num; i += itemTemplateInfo.MaxCount)
					{
						int count = ((i + itemTemplateInfo.MaxCount > item.RewardItemCount1) ? (item.RewardItemCount1 - i) : itemTemplateInfo.MaxCount);
						ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 106);
						if (itemInfo != null)
						{
							itemInfo.ValidDate = item.RewardItemValid;
							itemInfo.IsBinds = true;
							itemInfo.StrengthenLevel = item.StrengthenLevel;
							itemInfo.AttackCompose = item.AttackCompose;
							itemInfo.DefendCompose = item.DefendCompose;
							itemInfo.AgilityCompose = item.AgilityCompose;
							itemInfo.LuckCompose = item.LuckCompose;
							if (itemTemplateInfo.BagType == eBageType.PropBag)
							{
								list2.Add(itemInfo);
							}
							else if (itemTemplateInfo.BagType == eBageType.FarmBag)
							{
								list3.Add(itemInfo);
							}
							else if (itemTemplateInfo.BagType == eBageType.BeadBag)
							{
								list4.Add(itemInfo);
							}
							else
							{
								list.Add(itemInfo);
							}
						}
					}
				}
				if (list.Count > 0 && m_player.MainBag.GetEmptyCount() < list.Count)
				{
					baseQuest.CancelFinish(m_player);
					m_player.Out.SendMessage(eMessageType.ERROR, m_player.GetInventoryName(eBageType.MainBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
					return false;
				}
				if (list2.Count > 0 && m_player.PropBag.GetEmptyCount() < list2.Count)
				{
					baseQuest.CancelFinish(m_player);
					m_player.Out.SendMessage(eMessageType.ERROR, m_player.GetInventoryName(eBageType.PropBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
					return false;
				}
				if (list3.Count > 0 && m_player.FarmBag.GetEmptyCount() < list3.Count)
				{
					baseQuest.CancelFinish(m_player);
					m_player.Out.SendMessage(eMessageType.ERROR, m_player.GetInventoryName(eBageType.FarmBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
					return false;
				}
				if (list4.Count > 0 && m_player.BeadBag.GetEmptyCount() < list4.Count)
				{
					baseQuest.CancelFinish(m_player);
					m_player.Out.SendMessage(eMessageType.ERROR, m_player.GetInventoryName(eBageType.BeadBag) + LanguageMgr.GetTranslation("Game.Server.Quests.BagFull") + " ");
					return false;
				}
				foreach (ItemInfo item2 in list)
				{
					if (!m_player.MainBag.StackItemToAnother(item2) && !m_player.MainBag.AddItem(item2))
					{
						list5.Add(item2);
					}
				}
				foreach (ItemInfo item3 in list2)
				{
					if (item3.Template.CategoryID != 10)
					{
						if (!m_player.PropBag.StackItemToAnother(item3) && !m_player.PropBag.AddItem(item3))
						{
							list5.Add(item3);
						}
						continue;
					}
					switch (item3.TemplateID)
					{
					case 10001:
						m_player.PlayerCharacter.openFunction(Step.PICK_TWO_TWENTY);
						break;
					case 10003:
						m_player.PlayerCharacter.openFunction(Step.POP_WIN);
						break;
					case 10004:
						m_player.PlayerCharacter.openFunction(Step.FIFTY_OPEN);
						m_player.AddGift(eGiftType.MONEY);
						m_player.AddGift(eGiftType.BIG_EXP);
						m_player.AddGift(eGiftType.PET_EXP);
						break;
					case 10005:
						m_player.PlayerCharacter.openFunction(Step.FORTY_OPEN);
						break;
					case 10006:
						m_player.PlayerCharacter.openFunction(Step.THIRTY_OPEN);
						break;
					case 10007:
						m_player.PlayerCharacter.openFunction(Step.POP_TWO_TWENTY);
						m_player.AddGift(eGiftType.SMALL_EXP);
						break;
					case 10008:
						m_player.PlayerCharacter.openFunction(Step.POP_TIP_ONE);
						break;
					case 10024:
						m_player.PlayerCharacter.openFunction(Step.PICK_ONE);
						break;
					case 10025:
						m_player.PlayerCharacter.openFunction(Step.POP_EXPLAIN_ONE);
						break;
					}
				}
				foreach (ItemInfo item4 in list3)
				{
					if (!m_player.FarmBag.StackItemToAnother(item4) && !m_player.FarmBag.AddItem(item4))
					{
						list5.Add(item4);
					}
				}
				foreach (ItemInfo item5 in list4)
				{
					if (!m_player.BeadBag.AddItem(item5))
					{
						list5.Add(item5);
					}
				}
				if (list5.Count > 0)
				{
					m_player.SendItemsToMail(list5, "Bagfull trả về thư!", "Phần thưởng nhiệm vụ!", eMailType.ItemOverdue);
					m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
				}
				text = LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.Reward") + text;
				if (info.RewardBuffID > 0 && info.RewardBuffDate > 0)
				{
					ItemTemplateInfo itemTemplateInfo2 = ItemMgr.FindItemTemplate(info.RewardBuffID);
					if (itemTemplateInfo2 != null)
					{
						int num2 = info.RewardBuffDate * data.RandDobule;
						AbstractBuffer abstractBuffer = BufferList.CreateBufferHour(itemTemplateInfo2, num2);
						abstractBuffer.Start(m_player);
						text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardBuff", itemTemplateInfo2.Name, num2) + " ";
					}
				}
				if (info.RewardGold != 0)
				{
					int num3 = info.RewardGold * data.RandDobule;
					m_player.AddGold(num3);
					text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGold", num3) + " ";
				}
				if (info.RewardMoney != 0)
				{
					int num4 = info.RewardMoney * data.RandDobule;
					m_player.AddMoney(info.RewardMoney * data.RandDobule);
					text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardMoney", num4) + " ";
				}
				if (info.RewardGP != 0)
				{
					int num5 = info.RewardGP * data.RandDobule;
					m_player.AddGP(num5);
					text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGB1", num5) + " ";
				}
				if (info.RewardRiches != 0 && m_player.PlayerCharacter.ConsortiaID != 0)
				{
					int riches = info.RewardRiches * data.RandDobule;
					m_player.AddRichesOffer(riches);
					using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
					{
						consortiaBussiness.ConsortiaRichAdd(m_player.PlayerCharacter.ConsortiaID, ref riches);
					}
					text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardRiches", riches) + " ";
				}
				if (info.RewardOffer != 0)
				{
					int num6 = info.RewardOffer * data.RandDobule;
					m_player.AddOffer(num6, IsRate: false);
					text = text + LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardOffer", num6) + " ";
				}
				if (info.RewardBindMoney != 0)
				{
					int num7 = info.RewardBindMoney * data.RandDobule;
					m_player.AddGiftToken(num7);
					text += LanguageMgr.GetTranslation("Game.Server.Quests.FinishQuest.RewardGiftToken", num7 + " ");
				}
				m_player.Out.SendMessage(eMessageType.Normal, text);
				SetQuestFinish(baseQuest.Info.ID);
				m_player.PlayerCharacter.QuestSite = m_states;
			}
			OnQuestsChanged(baseQuest);
		}
		catch (Exception ex)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Quest Finish：" + ex);
			}
			return false;
		}
		finally
		{
			m_player.CommitAllChanges();
		}
		return true;
	}

	public BaseQuest FindQuest(int id)
	{
		foreach (BaseQuest item in m_list)
		{
			if (item.Info.ID == id)
			{
				return item;
			}
		}
		return null;
	}

	protected void OnQuestsChanged(BaseQuest quest)
	{
		if (!m_changedQuests.Contains(quest))
		{
			m_changedQuests.Add(quest);
		}
		if (m_changeCount <= 0 && m_changedQuests.Count > 0)
		{
			UpdateChangedQuests();
		}
	}

	private void BeginChanges()
	{
		Interlocked.Increment(ref m_changeCount);
	}

	private void CommitChanges()
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
		if (num <= 0 && m_changedQuests.Count > 0)
		{
			UpdateChangedQuests();
		}
	}

	public void UpdateChangedQuests()
	{
		m_player.Out.SendUpdateQuests(m_player, m_states, m_changedQuests.ToArray());
		m_changedQuests.Clear();
	}

	private byte[] InitQuest()
	{
		byte[] array = new byte[200];
		for (int i = 0; i < 200; i++)
		{
			array[i] = 0;
		}
		return array;
	}

	private bool SetQuestFinish(int questId)
	{
		if (questId > m_states.Length * 8 || questId < 1)
		{
			return false;
		}
		questId--;
		int num = questId / 8;
		int num2 = questId % 8;
		m_states[num] = (byte)(m_states[num] | (1 << num2));
		return true;
	}

	private bool IsQuestFinish(int questId)
	{
		if (questId > m_states.Length * 8 || questId < 1)
		{
			return false;
		}
		questId--;
		int num = questId / 8;
		int num2 = questId % 8;
		int num3 = m_states[num] & (1 << num2);
		return num3 != 0;
	}

	public bool ClearConsortiaQuest()
	{
		return true;
	}

	public bool ClearMarryQuest()
	{
		return true;
	}
}
