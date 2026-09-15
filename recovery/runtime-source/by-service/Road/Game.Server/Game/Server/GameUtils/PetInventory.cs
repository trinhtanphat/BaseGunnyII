using System;
using System.Collections.Generic;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;

namespace Game.Server.GameUtils;

public class PetInventory : PetAbstractInventory
{
	protected GamePlayer m_player;

	private bool m_saveToDb;

	private List<UsersPetinfo> m_removedList;

	public GamePlayer Player => m_player;

	public PetInventory(GamePlayer player, bool saveTodb, int capibility, int aCapability, int beginSlot)
		: base(capibility, aCapability, beginSlot)
	{
		m_player = player;
		m_saveToDb = saveTodb;
		m_removedList = new List<UsersPetinfo>();
	}

	public virtual void LoadFromDatabase()
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		int iD = m_player.PlayerCharacter.ID;
		UsersPetinfo[] userPetSingles = playerBussiness.GetUserPetSingles(iD);
		UsersPetinfo[] userAdoptPetSingles = playerBussiness.GetUserAdoptPetSingles(iD);
		PetEquipDataInfo[] eqPetSingles = playerBussiness.GetEqPetSingles(iD);
		BeginChanges();
		try
		{
			UsersPetinfo[] array = userPetSingles;
			foreach (UsersPetinfo usersPetinfo in array)
			{
				usersPetinfo.EquipList = GetPetEquip(usersPetinfo.ID, eqPetSingles);
				AddPetTo(usersPetinfo, usersPetinfo.Place);
			}
			UsersPetinfo[] array2 = userAdoptPetSingles;
			foreach (UsersPetinfo usersPetinfo2 in array2)
			{
				AddAdoptPetTo(usersPetinfo2, usersPetinfo2.Place);
			}
		}
		finally
		{
			CommitChanges();
		}
	}

	private List<PetEquipDataInfo> GetPetEquip(int petID, PetEquipDataInfo[] eqs)
	{
		List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
		foreach (PetEquipDataInfo petEquipDataInfo in eqs)
		{
			if (petID == petEquipDataInfo.PetID)
			{
				list.Add(petEquipDataInfo);
			}
		}
		return list;
	}

	public virtual void SaveToDatabase(bool saveAdopt)
	{
		if (!m_saveToDb)
		{
			return;
		}
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			for (int i = 0; i < m_pets.Length; i++)
			{
				UsersPetinfo usersPetinfo = m_pets[i];
				if (usersPetinfo != null && usersPetinfo.IsDirty)
				{
					if (usersPetinfo.ID > 0)
					{
						playerBussiness.UpdateUserPet(usersPetinfo);
					}
					else
					{
						playerBussiness.AddUserPet(usersPetinfo);
					}
					SaveqPet(playerBussiness, usersPetinfo);
				}
			}
			if (saveAdopt)
			{
				for (int j = 0; j < m_adoptPets.Length; j++)
				{
					UsersPetinfo usersPetinfo2 = m_adoptPets[j];
					if (usersPetinfo2 != null && usersPetinfo2.IsDirty && usersPetinfo2.ID == 0)
					{
						playerBussiness.AddUserAdoptPet(usersPetinfo2, isUse: false);
					}
				}
			}
		}
		lock (m_removedList)
		{
			foreach (UsersPetinfo removed in m_removedList)
			{
				playerBussiness.UpdateUserPet(removed);
			}
			m_removedList.Clear();
		}
	}

	public virtual void SaveqPet(PlayerBussiness pb, UsersPetinfo p)
	{
		for (int i = 0; i < p.EquipList.Count; i++)
		{
			PetEquipDataInfo petEquipDataInfo = p.EquipList[i];
			petEquipDataInfo.PetID = p.ID;
			if (petEquipDataInfo != null && petEquipDataInfo.IsDirty)
			{
				if (petEquipDataInfo.ID > 0)
				{
					pb.UpdateqPet(petEquipDataInfo);
				}
				else
				{
					pb.AddeqPet(petEquipDataInfo);
				}
			}
		}
	}

	public override bool AddPetTo(UsersPetinfo pet, int place)
	{
		if (pet.EquipList == null || pet.EquipList.Count == 0)
		{
			pet.EquipList = EmptyPetEquip(m_player.PlayerCharacter.ID);
		}
		if (base.AddPetTo(pet, place))
		{
			pet.UserID = m_player.PlayerCharacter.ID;
			return true;
		}
		return false;
	}

	public virtual void ReduceHunger()
	{
		UsersPetinfo petIsEquip = GetPetIsEquip();
		if (petIsEquip == null)
		{
			return;
		}
		int num = 40;
		int num2 = 100;
		if (petIsEquip.Hunger > 0)
		{
			if (petIsEquip.Level >= 60)
			{
				petIsEquip.Hunger -= num2;
			}
			else
			{
				petIsEquip.Hunger -= num;
			}
			UpdatePet(petIsEquip, petIsEquip.Place);
		}
	}

	private List<PetEquipDataInfo> EmptyPetEquip(int UserID)
	{
		List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
		for (int i = 0; i < 3; i++)
		{
			list.Add(new PetEquipDataInfo(null)
			{
				ID = 0,
				UserID = UserID,
				PetID = 0,
				eqType = i,
				eqTemplateID = -1,
				startTime = DateTime.Now,
				ValidDate = 7,
				IsExit = true
			});
		}
		return list;
	}

	public override bool RemovePet(UsersPetinfo pet)
	{
		List<PetEquipDataInfo> equip = pet.GetEquip();
		if (base.RemovePet(pet))
		{
			MoveEqAllToBag(equip);
			lock (m_removedList)
			{
				pet.IsExit = false;
				m_removedList.Add(pet);
			}
			return true;
		}
		return false;
	}

	public List<PetEquipDataInfo> RemoveEq(List<PetEquipDataInfo> Eqs)
	{
		List<PetEquipDataInfo> list = new List<PetEquipDataInfo>();
		for (int i = 0; i < Eqs.Count; i++)
		{
			PetEquipDataInfo petEquipDataInfo = Eqs[i];
			petEquipDataInfo.eqTemplateID = -1;
			petEquipDataInfo.ValidDate = 7;
			list.Add(petEquipDataInfo);
		}
		return list;
	}

	public virtual bool MoveEqAllToBag(List<PetEquipDataInfo> eps)
	{
		int num = 0;
		for (int i = 0; i < eps.Count; i++)
		{
			MoveEqToBag(eps[i]);
			num++;
		}
		return num > 0;
	}

	public virtual void MoveEqToBag(PetEquipDataInfo ep)
	{
		if (!ep.IsValidate())
		{
			return;
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(ep.eqTemplateID);
		if (itemTemplateInfo != null)
		{
			ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 102);
			itemInfo.IsUsed = true;
			itemInfo.IsBinds = true;
			itemInfo.ValidDate = ep.ValidDate;
			itemInfo.BeginDate = ep.startTime;
			List<ItemInfo> list = new List<ItemInfo>();
			if (!m_player.MainBag.AddItem(itemInfo))
			{
				list.Add(itemInfo);
			}
			if (list.Count > 0)
			{
				m_player.SendItemsToMail(list, "Bagfull trả về thư!", "Trả trang bị pet về thư!", eMailType.ItemOverdue);
				m_player.Out.SendMailResponse(m_player.PlayerCharacter.ID, eMailRespose.Receiver);
			}
		}
	}

	public virtual bool MoveEqFromBag(int place, int eqslot, ItemInfo item)
	{
		UsersPetinfo petAt = GetPetAt(place);
		if (petAt == null)
		{
			return false;
		}
		if (item.Template.Property2 > petAt.Level)
		{
			return false;
		}
		PetEquipDataInfo petEquipDataInfo = petAt.EquipList[eqslot];
		if (petEquipDataInfo == null)
		{
			return false;
		}
		if (petEquipDataInfo.eqTemplateID > 0)
		{
			MoveEqToBag(petEquipDataInfo);
		}
		petEquipDataInfo.eqTemplateID = item.TemplateID;
		petEquipDataInfo.ValidDate = item.ValidDate;
		petEquipDataInfo.startTime = item.BeginDate;
		petEquipDataInfo = petEquipDataInfo.addTempalte(ItemMgr.FindItemTemplate(item.TemplateID));
		return UpdateQPet(place, eqslot, petEquipDataInfo);
	}

	public override bool AddAdoptPetTo(UsersPetinfo pet, int place)
	{
		return base.AddAdoptPetTo(pet, place);
	}

	public override bool RemoveAdoptPet(UsersPetinfo pet)
	{
		return base.RemoveAdoptPet(pet);
	}

	public override void UpdateChangedPlaces()
	{
		int[] slots = m_changedPlaces.ToArray();
		m_player.Out.SendUpdateUserPet(this, slots);
		base.UpdateChangedPlaces();
	}

	public virtual void ClearAdoptPets()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		lock (m_lock)
		{
			for (int i = 0; i < base.ACapalility; i++)
			{
				if (m_adoptPets[i] != null && m_adoptPets[i].ID > 0)
				{
					playerBussiness.ClearAdoptPet(m_adoptPets[i].ID);
				}
				m_adoptPets[i] = null;
			}
		}
	}
}
