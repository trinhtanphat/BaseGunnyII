using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Game.Logic;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public abstract class PetAbstractInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	private int m_capalility;

	private int m_aCapalility;

	private int m_beginSlot;

	protected UsersPetinfo[] m_pets;

	protected UsersPetinfo[] m_adoptPets;

	protected ItemInfo[] m_adoptItems;

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
			m_capalility = ((value >= 0) ? ((value > m_pets.Length) ? m_pets.Length : value) : 0);
		}
	}

	public int ACapalility
	{
		get
		{
			return m_aCapalility;
		}
		set
		{
			m_aCapalility = ((value >= 0) ? ((value > m_adoptPets.Length) ? m_adoptPets.Length : value) : 0);
		}
	}

	public bool IsEmpty(int slot)
	{
		return slot < 0 || slot >= m_capalility || m_pets[slot] == null;
	}

	public PetAbstractInventory(int capability, int aCapability, int beginSlot)
	{
		m_capalility = capability;
		m_aCapalility = aCapability;
		m_beginSlot = beginSlot;
		m_pets = new UsersPetinfo[capability];
		m_adoptPets = new UsersPetinfo[aCapability];
	}

	public virtual UsersPetinfo GetPetIsEquip()
	{
		for (int i = 0; i < m_capalility; i++)
		{
			if (m_pets[i] != null && m_pets[i].IsEquip)
			{
				return m_pets[i];
			}
		}
		return null;
	}

	public virtual bool UpdateQPet(int petPlace, int eqType, PetEquipDataInfo eq)
	{
		if (petPlace > m_capalility)
		{
			return false;
		}
		lock (m_lock)
		{
			m_pets[petPlace].EquipList[eqType] = eq;
		}
		OnPlaceChanged(petPlace);
		return true;
	}

	public virtual bool AddAdoptPetTo(UsersPetinfo pet, int place)
	{
		if (pet == null || place >= m_aCapalility || place < 0)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_adoptPets[place] != null)
			{
				place = -1;
			}
			else
			{
				m_adoptPets[place] = pet;
				pet.Place = place;
			}
		}
		return place != -1;
	}

	public virtual bool RemoveAdoptPet(UsersPetinfo pet)
	{
		if (pet == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_aCapalility; i++)
			{
				if (m_adoptPets[i] == pet)
				{
					num = i;
					m_adoptPets[i] = null;
					break;
				}
			}
		}
		return num != -1;
	}

	public bool AddPet(UsersPetinfo pet)
	{
		return AddPet(pet, m_beginSlot);
	}

	public bool AddPet(UsersPetinfo pet, int minSlot)
	{
		if (pet == null)
		{
			return false;
		}
		int place = FindFirstEmptySlot(minSlot);
		return AddPetTo(pet, place);
	}

	public virtual bool AddPetTo(UsersPetinfo pet, int place)
	{
		if (pet == null || place >= m_capalility || place < 0)
		{
			return false;
		}
		lock (m_lock)
		{
			if (m_pets[place] == null)
			{
				m_pets[place] = pet;
				pet.Place = place;
			}
			else
			{
				place = -1;
			}
		}
		if (place != -1)
		{
			OnPlaceChanged(place);
		}
		return place != -1;
	}

	public virtual bool RemovePet(UsersPetinfo pet)
	{
		if (pet == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_capalility; i++)
			{
				if (m_pets[i] == pet)
				{
					num = i;
					m_pets[i] = null;
					break;
				}
			}
		}
		if (num != -1)
		{
			OnPlaceChanged(num);
			pet.Place = -1;
		}
		return num != -1;
	}

	public bool RemovePetAt(int place)
	{
		return RemovePet(GetPetAt(place));
	}

	public virtual UsersPetinfo GetAdoptPetAt(int slot)
	{
		if (slot < 0 || slot >= m_aCapalility)
		{
			return null;
		}
		return m_adoptPets[slot];
	}

	public virtual UsersPetinfo GetPetAt(int slot)
	{
		if (slot < 0 || slot >= m_capalility)
		{
			return null;
		}
		return m_pets[slot];
	}

	public virtual UsersPetinfo[] GetAdoptPet()
	{
		List<UsersPetinfo> list = new List<UsersPetinfo>();
		for (int i = 0; i < m_aCapalility; i++)
		{
			if (m_adoptPets[i] != null && m_adoptPets[i].IsExit)
			{
				list.Add(m_adoptPets[i]);
			}
		}
		list.Add(PetMgr.CreateNewPet());
		return list.ToArray();
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
				if (m_pets[i] == null)
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
				if (m_pets[num] == null)
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
				m_pets[i] = null;
			}
		}
	}

	public virtual UsersPetinfo GetPetByTemplateID(int minSlot, int templateId)
	{
		UsersPetinfo result;
		lock (m_lock)
		{
			for (int i = minSlot; i < m_capalility; i++)
			{
				if (m_pets[i] != null && m_pets[i].TemplateID == templateId)
				{
					result = m_pets[i];
					return result;
				}
			}
			result = null;
		}
		return result;
	}

	public virtual UsersPetinfo[] GetPets()
	{
		List<UsersPetinfo> list = new List<UsersPetinfo>();
		for (int i = 0; i < m_capalility; i++)
		{
			if (m_pets[i] != null)
			{
				list.Add(m_pets[i]);
			}
		}
		return list.ToArray();
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
				if (m_pets[i] == null)
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

	public virtual bool RenamePet(int place, string name)
	{
		lock (m_lock)
		{
			m_pets[place].Name = name;
		}
		OnPlaceChanged(place);
		return true;
	}

	public bool IsEquipSkill(int slot, string kill)
	{
		List<string> skillEquip = m_pets[slot].GetSkillEquip();
		for (int i = 0; i < skillEquip.Count; i++)
		{
			if (skillEquip[i].Split(',')[0] == kill)
			{
				return false;
			}
		}
		return true;
	}

	public virtual bool EquipSkillPet(int place, int killId, int killindex)
	{
		string skill = killId + "," + killindex;
		UsersPetinfo pet = m_pets[place];
		lock (m_lock)
		{
			if (killId == 0)
			{
				m_pets[place].SkillEquip = SetSkillEquip(pet, killindex, skill);
				OnPlaceChanged(place);
				return true;
			}
			if (IsEquipSkill(place, killId.ToString()))
			{
				m_pets[place].SkillEquip = SetSkillEquip(pet, killindex, skill);
				OnPlaceChanged(place);
				return true;
			}
		}
		return false;
	}

	public string SetSkillEquip(UsersPetinfo pet, int place, string skill)
	{
		List<string> skillEquip = pet.GetSkillEquip();
		skillEquip[place] = skill;
		string text = skillEquip[0];
		for (int i = 1; i < skillEquip.Count; i++)
		{
			text = text + "|" + skillEquip[i];
		}
		return text;
	}

	public virtual bool UpdatePet(UsersPetinfo pet, int place)
	{
		if (pet == null)
		{
			return false;
		}
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_pets.Length; i++)
			{
				if (m_pets[i] != null)
				{
					num = m_pets[i].Place;
					if (num == place)
					{
						m_pets[i] = pet;
					}
					OnPlaceChanged(num);
				}
			}
		}
		return num > -1;
	}

	public virtual bool EquipPet(int place, bool isEquip)
	{
		int num = -1;
		lock (m_lock)
		{
			for (int i = 0; i < m_pets.Length; i++)
			{
				if (m_pets[i] == null)
				{
					continue;
				}
				num = m_pets[i].Place;
				if (num == place)
				{
					if (m_pets[i].Hunger == 0)
					{
						return false;
					}
					m_pets[i].IsEquip = isEquip;
				}
				else
				{
					m_pets[i].IsEquip = false;
				}
				OnPlaceChanged(num);
			}
		}
		return num > -1;
	}

	public virtual bool UpGracePet(UsersPetinfo pet, int place, bool isUpdateProp, int min, int max, int Level, ref string msg)
	{
		if (isUpdateProp)
		{
			int blood = 0;
			int attack = 0;
			int defence = 0;
			int agility = 0;
			int lucky = 0;
			PetMgr.PlusPetProp(pet, min, max, ref blood, ref attack, ref defence, ref agility, ref lucky);
			pet.Blood = blood;
			pet.Attack = attack;
			pet.Defence = defence;
			pet.Agility = agility;
			pet.Luck = lucky;
			int num = PetMgr.UpdateEvolution(pet.TemplateID, max);
			pet.TemplateID = ((num == 0) ? pet.TemplateID : num);
			string skill = pet.Skill;
			string text = PetMgr.UpdateSkillPet(max, pet.TemplateID, Level);
			pet.Skill = ((text == "") ? skill : text);
			pet.SkillEquip = PetMgr.ActiveEquipSkill(max);
			if (max > min)
			{
				msg = pet.Name + " thăng cấp " + max;
			}
		}
		lock (m_lock)
		{
			m_pets[place] = pet;
		}
		OnPlaceChanged(place);
		return true;
	}
}
