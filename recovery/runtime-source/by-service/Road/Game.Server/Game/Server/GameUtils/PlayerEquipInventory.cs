using System;
using System.Collections.Generic;
using Bussiness.Managers;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PlayerEquipInventory : PlayerInventory
{
	private const int BAG_START = 31;

	private static readonly int[] StyleIndex = new int[15]
	{
		1, 2, 3, 4, 5, 6, 11, 13, 14, 15,
		16, 17, 18, 19, 20
	};

	public PlayerEquipInventory(GamePlayer player)
		: base(player, saveTodb: true, 80, 0, 31, autoStack: true)
	{
	}

	public override void LoadFromDatabase()
	{
		BeginChanges();
		try
		{
			base.LoadFromDatabase();
			List<ItemInfo> list = new List<ItemInfo>();
			for (int i = 0; i < 31; i++)
			{
				ItemInfo itemInfo = m_items[i];
				if (m_items[i] != null && !m_items[i].IsValidItem())
				{
					int num = FindFirstEmptySlot(31);
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
				m_player.SendItemsToMail(list, null, "Item quá hạn trả về thư.", eMailType.ItemOverdue);
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
		if (m_items[fromSlot] == null)
		{
			return false;
		}
		if (IsEquipSlot(fromSlot) && !IsEquipSlot(toSlot) && m_items[toSlot] != null && m_items[toSlot].Template.CategoryID != m_items[fromSlot].Template.CategoryID)
		{
			if (!CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
			{
				toSlot = FindFirstEmptySlot(31);
			}
		}
		else
		{
			if (IsEquipSlot(toSlot))
			{
				if (!CanEquipSlotContains(toSlot, m_items[fromSlot].Template))
				{
					UpdateItem(m_items[fromSlot]);
					return false;
				}
				if (!m_player.CanEquip(m_items[fromSlot].Template) || !m_items[fromSlot].IsValidItem())
				{
					UpdateItem(m_items[fromSlot]);
					return false;
				}
			}
			if (IsEquipSlot(fromSlot) && m_items[toSlot] != null && !CanEquipSlotContains(fromSlot, m_items[toSlot].Template))
			{
				UpdateItem(m_items[toSlot]);
				return false;
			}
		}
		return base.MoveItem(fromSlot, toSlot, count);
	}

	public override void UpdateChangedPlaces()
	{
		int[] array = m_changedPlaces.ToArray();
		bool flag = false;
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
				m_player.OnUsingItem(itemAt.TemplateID);
				itemAt.IsBinds = true;
				if (!itemAt.IsUsed)
				{
					itemAt.IsUsed = true;
					itemAt.BeginDate = DateTime.Now;
				}
			}
			flag = true;
			break;
		}
		base.UpdateChangedPlaces();
		if (flag)
		{
			UpdatePlayerProperties();
		}
	}

	public void UpdatePlayerProperties()
	{
		m_player.BeginChanges();
		try
		{
			int attack = 0;
			int defence = 0;
			int agility = 0;
			int lucky = 0;
			int hp = 0;
			int num = 0;
			string text = "";
			string text2 = "";
			string skin = "";
			int attack2 = 0;
			int defence2 = 0;
			int agility2 = 0;
			int lucky2 = 0;
			int hp2 = 0;
			int num2 = 0;
			int num3 = 0;
			int num4 = 0;
			int num5 = 0;
			int num6 = 0;
			int num7 = 0;
			int num8 = 0;
			int num9 = 0;
			int num10 = 0;
			int num11 = 0;
			int num12 = 0;
			int num13 = 0;
			int num14 = 0;
			int num15 = 0;
			int attack3 = 0;
			int defence3 = 0;
			int agility3 = 0;
			int lucky3 = 0;
			int hp3 = 0;
			m_player.UpdatePet(m_player.PetBag.GetPetIsEquip());
			List<UsersCardInfo> cards = m_player.CardBag.GetCards(0, 5);
			lock (m_lock)
			{
				text = ((m_items[0] == null) ? "" : (m_items[0].TemplateID + "|" + m_items[0].Template.Pic));
				text2 = ((m_items[0] == null) ? "" : m_items[0].Color);
				skin = ((m_items[5] == null) ? "" : m_items[5].Skin);
				for (int i = 0; i < 31; i++)
				{
					ItemInfo itemInfo = m_items[i];
					if (itemInfo != null)
					{
						attack += itemInfo.Attack;
						defence += itemInfo.Defence;
						agility += itemInfo.Agility;
						lucky += itemInfo.Luck;
						num = ((num > itemInfo.StrengthenLevel) ? num : itemInfo.StrengthenLevel);
						AddBaseLatentProperty(itemInfo, ref attack, ref defence, ref agility, ref lucky);
						AddBaseGemstoneProperty(itemInfo, ref attack2, ref defence2, ref agility2, ref lucky2, ref hp2);
					}
					AddBeadProperty(i, ref attack3, ref defence3, ref agility3, ref lucky3, ref hp3);
				}
				AddBaseTotemProperty(m_player.PlayerCharacter, ref attack, ref defence, ref agility, ref lucky, ref hp);
				if (m_player.Pet != null)
				{
					num6 += m_player.Pet.TotalAttack;
					num7 += m_player.Pet.TotalDefence;
					num8 += m_player.Pet.TotalAgility;
					num9 += m_player.Pet.TotalLuck;
					num10 += m_player.Pet.TotalBlood;
				}
				UserRankInfo rank = m_player.Rank.GetRank(m_player.PlayerCharacter.Honor);
				if (rank != null)
				{
					attack += rank.Attack;
					defence += rank.Defence;
					agility += rank.Agility;
					lucky += rank.Luck;
					hp += rank.HP;
				}
				foreach (UsersCardInfo item in cards)
				{
					num2 += CardMgr.GetProp(item, 0);
					num3 += CardMgr.GetProp(item, 1);
					num4 += CardMgr.GetProp(item, 2);
					num5 += CardMgr.GetProp(item, 3);
					if (item.CardID > 0)
					{
						num2 += item.Attack;
						num3 += item.Defence;
						num4 += item.Agility;
						num5 += item.Luck;
					}
					if (item.TemplateID > 0)
					{
						CardTemplateInfo cardTemplateInfo = CardMgr.FindCardTemplate(item.TemplateID, item.CardType);
						if (cardTemplateInfo != null)
						{
							num2 += cardTemplateInfo.AddAttack;
							num3 += cardTemplateInfo.AddDefend;
							num4 += cardTemplateInfo.AddAgility;
							num5 += cardTemplateInfo.AddLucky;
						}
					}
				}
				num11 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.attTexpExp, "A");
				num12 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.defTexpExp, "D");
				num13 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.spdTexpExp, "AG");
				num14 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.lukTexpExp, "L");
				num15 += ExerciseMgr.GetExercise(m_player.PlayerCharacter.Texp.hpTexpExp, "H");
				for (int j = 0; j < StyleIndex.Length; j++)
				{
					text += ",";
					text2 += ",";
					if (m_items[StyleIndex[j]] != null)
					{
						object obj = text;
						text = string.Concat(obj, m_items[StyleIndex[j]].TemplateID, "|", m_items[StyleIndex[j]].Pic);
						text2 += m_items[StyleIndex[j]].Color;
					}
				}
				EquipBuffer();
			}
			attack += attack2 + num2 + num6 + num11 + attack3;
			defence += defence2 + num3 + num7 + num12 + defence3;
			agility += agility2 + num4 + num8 + num13 + agility3;
			lucky += lucky2 + num5 + num9 + num14 + lucky3;
			hp += hp2 + num10 + num15 + hp3;
			m_player.UpdateBaseProperties(attack, defence, agility, lucky, hp);
			m_player.UpdateStyle(text, text2, skin);
			m_player.ApertureEquip(num);
			m_player.UpdateWeapon(m_items[6]);
			m_player.UpdateSecondWeapon(m_items[15]);
			m_player.UpdateHealstone(m_items[18]);
			m_player.PlayerProp.CreateProp(isSelf: true, "Texp", num11, num12, num13, num14, num15);
			m_player.PlayerProp.CreateProp(isSelf: true, "Card", num2, num3, num4, num5, 0);
			m_player.PlayerProp.CreateProp(isSelf: true, "Pet", num6, num7, num8, num9, num10);
			m_player.PlayerProp.CreateProp(isSelf: true, "Gem", attack2, defence2, agility2, lucky2, hp2);
			m_player.PlayerProp.CreateProp(isSelf: true, "Bead", attack3, defence3, agility3, lucky3, hp3);
			m_player.UpdateFightPower();
			GetUserNimbus();
			m_player.PlayerProp.ViewCurrent();
		}
		finally
		{
			m_player.CommitChanges();
		}
	}

	public Dictionary<string, int> GetProp(int attack, int defence, int agility, int lucky, int hp)
	{
		Dictionary<string, int> dictionary = new Dictionary<string, int>();
		dictionary.Add("Attack", attack);
		dictionary.Add("Defence", defence);
		dictionary.Add("Agility", agility);
		dictionary.Add("Luck", lucky);
		dictionary.Add("HP", hp);
		return dictionary;
	}

	public int FindItemEpuipSlot(ItemTemplateInfo item)
	{
		switch (item.CategoryID)
		{
		case 8:
			if (m_items[7] == null)
			{
				return 7;
			}
			return 8;
		case 9:
			if (m_items[9] == null)
			{
				return 9;
			}
			return 10;
		case 13:
			return 11;
		case 14:
			return 12;
		case 15:
			return 13;
		case 16:
			return 14;
		case 27:
			return 6;
		case 40:
			return 17;
		case 17:
		case 31:
			return 15;
		default:
			return item.CategoryID - 1;
		}
	}

	public bool CanEquipSlotContains(int slot, ItemTemplateInfo temp)
	{
		if (temp.CategoryID == 8)
		{
			return slot == 7 || slot == 8;
		}
		if (temp.CategoryID == 9)
		{
			if (temp.TemplateID == 9022 || temp.TemplateID == 9122 || temp.TemplateID == 9222 || temp.TemplateID == 9322 || temp.TemplateID == 9422 || temp.TemplateID == 9522)
			{
				return slot == 9 || slot == 10 || slot == 16;
			}
			return slot == 9 || slot == 10;
		}
		if (temp.CategoryID == 13)
		{
			return slot == 11;
		}
		if (temp.CategoryID == 14)
		{
			return slot == 12;
		}
		if (temp.CategoryID == 15)
		{
			return slot == 13;
		}
		if (temp.CategoryID == 16)
		{
			return slot == 14;
		}
		if (temp.CategoryID == 17 || temp.CategoryID == 31)
		{
			return slot == 15;
		}
		if (temp.CategoryID == 27)
		{
			return slot == 6;
		}
		if (temp.CategoryID == 40)
		{
			return slot == 17;
		}
		return temp.CategoryID - 1 == slot;
	}

	public bool IsEquipSlot(int slot)
	{
		return slot >= 0 && slot < 31;
	}

	public void GetUserNimbus()
	{
		int num = 0;
		int num2 = 0;
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt = GetItemAt(i);
			if (itemAt == null)
			{
				continue;
			}
			if (itemAt.StrengthenLevel >= 5 && itemAt.StrengthenLevel <= 8)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num <= 1) ? 1 : num);
				}
				if (itemAt.Template.CategoryID == 7)
				{
					num2 = ((num2 <= 1) ? 1 : num2);
				}
			}
			if (itemAt.StrengthenLevel >= 9 && itemAt.StrengthenLevel <= 11)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num > 1) ? num : 2);
				}
				if (itemAt.Template.CategoryID == 7)
				{
					num2 = ((num2 > 1) ? num2 : 2);
				}
			}
			if (itemAt.StrengthenLevel == 12 && !itemAt.IsGold)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num > 1) ? num : 3);
				}
				if (itemAt.Template.CategoryID == 7)
				{
					num2 = ((num2 > 1) ? num2 : 3);
				}
			}
			if (itemAt.IsGold || itemAt.StrengthenLevel == 15)
			{
				if (itemAt.Template.CategoryID == 1 || itemAt.Template.CategoryID == 5)
				{
					num = ((num > 1) ? num : 5);
				}
				if (itemAt.Template.CategoryID == 7)
				{
					num2 = ((num2 > 1) ? num2 : 5);
				}
			}
		}
		m_player.PlayerCharacter.Nimbus = num * 100 + num2;
		m_player.Out.SendUpdatePublicPlayer(m_player.PlayerCharacter, m_player.BattleData.MatchInfo);
	}

	public void EquipBuffer()
	{
		m_player.EquipEffect.Clear();
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt = m_player.BeadBag.GetItemAt(i);
			if (itemAt != null)
			{
				RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(itemAt.TemplateID);
				if (runeTemplateInfo != null && (runeTemplateInfo.Type1 == 37 || runeTemplateInfo.Type1 == 39 || runeTemplateInfo.Type1 < 31))
				{
					m_player.AddBeadEffect(itemAt);
				}
			}
		}
	}

	public void AddBeadProperty(int place, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		ItemInfo itemAt = m_player.BeadBag.GetItemAt(place);
		if (itemAt != null)
		{
			AddRuneProperty(itemAt, ref attack, ref defence, ref agility, ref lucky, ref hp);
		}
	}

	public void AddRuneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
		if (runeTemplateInfo == null)
		{
			return;
		}
		string[] array = runeTemplateInfo.Attribute1.Split('|');
		string[] array2 = runeTemplateInfo.Attribute2.Split('|');
		int num = 0;
		int num2 = 0;
		if (item.Hole1 > runeTemplateInfo.BaseLevel)
		{
			if (array.Length > 1)
			{
				num = 1;
			}
			if (array2.Length > 1)
			{
				num2 = 1;
			}
		}
		int num3 = Convert.ToInt32(array[num]);
		int num4 = Convert.ToInt32(array2[num2]);
		switch (runeTemplateInfo.Type1)
		{
		case 31:
			attack += num3;
			hp += num4;
			break;
		case 32:
			defence += num3;
			hp += num4;
			break;
		case 33:
			agility += num3;
			hp += num4;
			break;
		case 34:
			lucky += num3;
			hp += num4;
			break;
		case 35:
			hp += num4;
			break;
		case 36:
			hp += num4;
			break;
		case 37:
			hp += num3;
			break;
		}
	}

	public void AddBaseTotemProperty(PlayerInfo p, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		attack += TotemMgr.GetTotemProp(p.totemId, "att");
		defence += TotemMgr.GetTotemProp(p.totemId, "def");
		agility += TotemMgr.GetTotemProp(p.totemId, "agi");
		lucky += TotemMgr.GetTotemProp(p.totemId, "luc");
		hp += TotemMgr.GetTotemProp(p.totemId, "blo");
	}

	public void AddBaseLatentProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky)
	{
		if (item != null && !item.IsValidLatentEnergy())
		{
			string[] array = item.latentEnergyCurStr.Split(',');
			attack += Convert.ToInt32(array[0]);
			defence += Convert.ToInt32(array[1]);
			agility += Convert.ToInt32(array[2]);
			lucky += Convert.ToInt32(array[3]);
		}
	}

	public void AddBaseGemstoneProperty(ItemInfo item, ref int attack, ref int defence, ref int agility, ref int lucky, ref int hp)
	{
		List<UserGemStone> gemStone = m_player.GemStone;
		foreach (UserGemStone item2 in gemStone)
		{
			int figSpiritId = item2.FigSpiritId;
			int lv = Convert.ToInt32(item2.FigSpiritIdValue.Split('|')[0].Split(',')[0]);
			int num = item2.FigSpiritIdValue.Split('|').Length;
			int place = item.Place;
			switch (item.Place)
			{
			case 2:
				attack += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
				break;
			case 3:
				lucky += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
				break;
			case 5:
				agility += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
				break;
			case 11:
				defence += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
				break;
			case 13:
				hp += FightSpiritTemplateMgr.getProp(figSpiritId, lv, place) * num;
				break;
			}
		}
	}
}
