using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class PetMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<string, PetConfig> _configs;

	private static Dictionary<int, PetLevel> _levels;

	private static Dictionary<int, PetSkillElementInfo> _skillElements;

	private static Dictionary<int, PetSkillInfo> _skills;

	private static Dictionary<int, PetSkillTemplateInfo> _skillTemplates;

	private static Dictionary<int, PetTemplateInfo> _templateIds;

	private static Dictionary<int, PetExpItemPriceInfo> _expItemPrices;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool Init()
	{
		try
		{
			_configs = new Dictionary<string, PetConfig>();
			_levels = new Dictionary<int, PetLevel>();
			_skillElements = new Dictionary<int, PetSkillElementInfo>();
			_skills = new Dictionary<int, PetSkillInfo>();
			_skillTemplates = new Dictionary<int, PetSkillTemplateInfo>();
			_templateIds = new Dictionary<int, PetTemplateInfo>();
			_expItemPrices = new Dictionary<int, PetExpItemPriceInfo>();
			m_lock = new ReaderWriterLock();
			rand = new ThreadSafeRandom();
			return LoadPetMgr(_configs, _levels, _skillElements, _skills, _skillTemplates, _templateIds, _expItemPrices);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PetInfoMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<string, PetConfig> dictionary = new Dictionary<string, PetConfig>();
			Dictionary<int, PetLevel> dictionary2 = new Dictionary<int, PetLevel>();
			Dictionary<int, PetSkillElementInfo> dictionary3 = new Dictionary<int, PetSkillElementInfo>();
			Dictionary<int, PetSkillInfo> dictionary4 = new Dictionary<int, PetSkillInfo>();
			Dictionary<int, PetSkillTemplateInfo> dictionary5 = new Dictionary<int, PetSkillTemplateInfo>();
			new Dictionary<int, PetTemplateInfo>();
			Dictionary<int, PetTemplateInfo> dictionary6 = new Dictionary<int, PetTemplateInfo>();
			Dictionary<int, PetExpItemPriceInfo> dictionary7 = new Dictionary<int, PetExpItemPriceInfo>();
			if (LoadPetMgr(dictionary, dictionary2, dictionary3, dictionary4, dictionary5, dictionary6, dictionary7))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_configs = dictionary;
					_levels = dictionary2;
					_skillElements = dictionary3;
					_skills = dictionary4;
					_skillTemplates = dictionary5;
					_templateIds = dictionary6;
					_expItemPrices = dictionary7;
					return true;
				}
				catch
				{
				}
				finally
				{
					m_lock.ReleaseWriterLock();
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("PetMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadPetMgr(Dictionary<string, PetConfig> Config, Dictionary<int, PetLevel> Level, Dictionary<int, PetSkillElementInfo> SkillElement, Dictionary<int, PetSkillInfo> Skill, Dictionary<int, PetSkillTemplateInfo> SkillTemplate, Dictionary<int, PetTemplateInfo> TemplateId, Dictionary<int, PetExpItemPriceInfo> PetExpItemPrice)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			PetConfig[] allPetConfig = playerBussiness.GetAllPetConfig();
			PetLevel[] allPetLevel = playerBussiness.GetAllPetLevel();
			PetSkillElementInfo[] allPetSkillElementInfo = playerBussiness.GetAllPetSkillElementInfo();
			PetSkillInfo[] allPetSkillInfo = playerBussiness.GetAllPetSkillInfo();
			PetSkillTemplateInfo[] allPetSkillTemplateInfo = playerBussiness.GetAllPetSkillTemplateInfo();
			PetTemplateInfo[] allPetTemplateInfo = playerBussiness.GetAllPetTemplateInfo();
			PetExpItemPriceInfo[] allPetExpItemPriceInfoInfo = playerBussiness.GetAllPetExpItemPriceInfoInfo();
			PetExpItemPriceInfo[] array = allPetExpItemPriceInfoInfo;
			foreach (PetExpItemPriceInfo petExpItemPriceInfo in array)
			{
				if (!PetExpItemPrice.ContainsKey(petExpItemPriceInfo.Count))
				{
					PetExpItemPrice.Add(petExpItemPriceInfo.Count, petExpItemPriceInfo);
				}
			}
			PetConfig[] array2 = allPetConfig;
			foreach (PetConfig petConfig in array2)
			{
				if (!Config.ContainsKey(petConfig.Name))
				{
					Config.Add(petConfig.Name, petConfig);
				}
			}
			PetLevel[] array3 = allPetLevel;
			foreach (PetLevel petLevel in array3)
			{
				if (!Level.ContainsKey(petLevel.Level))
				{
					Level.Add(petLevel.Level, petLevel);
				}
			}
			PetSkillElementInfo[] array4 = allPetSkillElementInfo;
			foreach (PetSkillElementInfo petSkillElementInfo in array4)
			{
				if (!SkillElement.ContainsKey(petSkillElementInfo.ID))
				{
					SkillElement.Add(petSkillElementInfo.ID, petSkillElementInfo);
				}
			}
			PetSkillTemplateInfo[] array5 = allPetSkillTemplateInfo;
			foreach (PetSkillTemplateInfo petSkillTemplateInfo in array5)
			{
				if (!SkillTemplate.ContainsKey(petSkillTemplateInfo.ID))
				{
					SkillTemplate.Add(petSkillTemplateInfo.ID, petSkillTemplateInfo);
				}
			}
			PetTemplateInfo[] array6 = allPetTemplateInfo;
			foreach (PetTemplateInfo petTemplateInfo in array6)
			{
				if (!TemplateId.ContainsKey(petTemplateInfo.ID))
				{
					TemplateId.Add(petTemplateInfo.ID, petTemplateInfo);
				}
			}
			PetSkillInfo[] array7 = allPetSkillInfo;
			foreach (PetSkillInfo petSkillInfo in array7)
			{
				if (!Skill.ContainsKey(petSkillInfo.ID))
				{
					Skill.Add(petSkillInfo.ID, petSkillInfo);
				}
			}
		}
		return true;
	}

	public static PetExpItemPriceInfo FindPetExpItemPrice(int count)
	{
		if (_expItemPrices == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_expItemPrices.ContainsKey(count))
			{
				return _expItemPrices[count];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static PetConfig FindConfig(string key)
	{
		if (_configs == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_configs.ContainsKey(key))
			{
				return _configs[key];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static PetLevel FindPetLevel(int level)
	{
		if (_levels == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_levels.ContainsKey(level))
			{
				return _levels[level];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static PetSkillElementInfo FindPetSkillElement(int SkillID)
	{
		if (_skillElements == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_skillElements.ContainsKey(SkillID))
			{
				return _skillElements[SkillID];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static List<PetSkillElementInfo> GameNeedPetSkill()
	{
		if (_skillElements == null)
		{
			Init();
		}
		List<PetSkillElementInfo> list = new List<PetSkillElementInfo>();
		Dictionary<string, PetSkillElementInfo> dictionary = new Dictionary<string, PetSkillElementInfo>();
		foreach (PetSkillElementInfo value in _skillElements.Values)
		{
			if (!dictionary.Keys.Contains(value.EffectPic) && !string.IsNullOrEmpty(value.EffectPic))
			{
				list.Add(value);
				dictionary.Add(value.EffectPic, value);
			}
		}
		return list;
	}

	public static PetSkillInfo FindPetSkill(int SkillID)
	{
		if (_skills == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_skills.ContainsKey(SkillID))
			{
				return _skills[SkillID];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static PetSkillTemplateInfo GetPetSkillTemplate(int ID)
	{
		if (_skillTemplates == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_skillTemplates.ContainsKey(ID))
			{
				return _skillTemplates[ID];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static PetTemplateInfo FindPetTemplate(int TemplateID)
	{
		if (_templateIds == null)
		{
			Init();
		}
		foreach (PetTemplateInfo value in _templateIds.Values)
		{
			if (value.TemplateID == TemplateID)
			{
				return value;
			}
		}
		return null;
	}

	public static PetTemplateInfo FindPetTemplateById(int ID)
	{
		if (_templateIds == null)
		{
			Init();
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_templateIds.ContainsKey(ID))
			{
				return _templateIds[ID];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static List<int> GetPetSkillByKindID(int KindID, int lv, int playerLevel)
	{
		List<int> list = new List<int>();
		List<string> list2 = new List<string>();
		PetSkillTemplateInfo[] petSkillByKindID = GetPetSkillByKindID(KindID);
		int num = ((lv > playerLevel) ? playerLevel : lv);
		for (int i = 1; i <= num; i++)
		{
			PetSkillTemplateInfo[] array = petSkillByKindID;
			foreach (PetSkillTemplateInfo petSkillTemplateInfo in array)
			{
				if (petSkillTemplateInfo.MinLevel == i)
				{
					string[] array2 = petSkillTemplateInfo.DeleteSkillIDs.Split(',');
					string[] array3 = array2;
					foreach (string item in array3)
					{
						list2.Add(item);
					}
					list.Add(petSkillTemplateInfo.SkillID);
				}
			}
		}
		foreach (string item3 in list2)
		{
			if (!string.IsNullOrEmpty(item3))
			{
				int item2 = int.Parse(item3);
				list.Remove(item2);
			}
		}
		list.Sort();
		return list;
	}

	public static PetSkillTemplateInfo[] GetPetSkillByKindID(int KindID)
	{
		List<PetSkillTemplateInfo> list = new List<PetSkillTemplateInfo>();
		foreach (PetSkillTemplateInfo value in _skillTemplates.Values)
		{
			if (value.KindID == KindID)
			{
				list.Add(value);
			}
		}
		return list.ToArray();
	}

	public static List<UsersPetinfo> CreateAdoptList(int userID, int playerLevel)
	{
		int num = Convert.ToInt32(FindConfig("AdoptCount").Value);
		List<UsersPetinfo> list = new List<UsersPetinfo>();
		List<PetTemplateInfo> info = null;
		int num2 = 0;
		while (num2 < num)
		{
			if (DropInventory.GetPetDrop(613, 1, ref info) && info != null)
			{
				int index = rand.Next(info.Count);
				UsersPetinfo usersPetinfo = CreatePet(info[index], userID, num2, playerLevel);
				usersPetinfo.IsExit = true;
				list.Add(usersPetinfo);
				num2++;
			}
		}
		return list;
	}

	public static List<UsersPetinfo> CreateFirstAdoptList(int userID, int playerLevel)
	{
		List<int> list = new List<int>();
		list.Add(100301);
		list.Add(110301);
		list.Add(120301);
		list.Add(130301);
		List<int> list2 = list;
		List<UsersPetinfo> list3 = new List<UsersPetinfo>();
		for (int i = 0; i < list2.Count; i++)
		{
			PetTemplateInfo info = FindPetTemplate(list2[i]);
			UsersPetinfo usersPetinfo = CreatePet(info, userID, i, playerLevel);
			usersPetinfo.IsExit = true;
			list3.Add(usersPetinfo);
		}
		return list3;
	}

	public static string ActiveEquipSkill(int Level)
	{
		string text = "0,0";
		int num = 1;
		if (Level >= 20 && Level < 30)
		{
			num++;
		}
		if (Level >= 30 && Level < 50)
		{
			num += 2;
		}
		if (Level >= 50 && Level < 60)
		{
			num += 3;
		}
		if (Level == 60)
		{
			num += 4;
		}
		for (int i = 1; i < num; i++)
		{
			text = text + "|0," + i;
		}
		return text;
	}

	public static int UpdateEvolution(int TemplateID, int lv)
	{
		int num = TemplateID;
		int num2 = Convert.ToInt32(FindConfig("EvolutionLevel1").Value);
		int num3 = Convert.ToInt32(FindConfig("EvolutionLevel2").Value);
		PetTemplateInfo petTemplateInfo = FindPetTemplate(num);
		PetTemplateInfo petTemplateInfo2 = FindPetTemplate(num + 1);
		PetTemplateInfo petTemplateInfo3 = FindPetTemplate(num + 2);
		if (petTemplateInfo3 != null)
		{
			if (lv >= num2 && lv < num3 && petTemplateInfo2.EvolutionID != 0)
			{
				num = petTemplateInfo.EvolutionID;
			}
			else if (lv >= num3)
			{
				num = petTemplateInfo2.EvolutionID;
			}
		}
		else if (petTemplateInfo2 != null && lv >= num3)
		{
			num = petTemplateInfo.EvolutionID;
		}
		return num;
	}

	public static int TemplateReset(int TemplateID)
	{
		int num = TemplateID;
		PetTemplateInfo petTemplateInfo = FindPetTemplate(num - 1);
		PetTemplateInfo petTemplateInfo2 = FindPetTemplate(num - 2);
		if (petTemplateInfo != null)
		{
			num = petTemplateInfo.TemplateID;
		}
		else if (petTemplateInfo2 != null)
		{
			num = petTemplateInfo2.TemplateID;
		}
		return num;
	}

	public static string UpdateSkillPet(int Level, int TemplateID, int playerLevel)
	{
		PetTemplateInfo petTemplateInfo = FindPetTemplate(TemplateID);
		if (petTemplateInfo == null)
		{
			log.Error("Pet not found: " + TemplateID);
			return "";
		}
		List<int> petSkillByKindID = GetPetSkillByKindID(petTemplateInfo.KindID, Level, playerLevel);
		string text = petSkillByKindID[0] + ",0";
		for (int i = 1; i < petSkillByKindID.Count; i++)
		{
			object obj = text;
			text = string.Concat(obj, "|", petSkillByKindID[i], ",", i);
		}
		return text;
	}

	public static int GetLevel(int GP, int playerLevel)
	{
		if (GP >= FindPetLevel(playerLevel).GP)
		{
			return playerLevel;
		}
		for (int i = 1; i <= playerLevel; i++)
		{
			if (GP < FindPetLevel(i).GP)
			{
				if (i - 1 != 0)
				{
					return i - 1;
				}
				return 1;
			}
		}
		return 1;
	}

	public static int GetGP(int level, int playerLevel)
	{
		for (int i = 1; i <= playerLevel; i++)
		{
			if (level == FindPetLevel(i).Level)
			{
				return FindPetLevel(i).GP;
			}
		}
		return 0;
	}

	public static void PlusPetProp(UsersPetinfo pet, int min, int max, ref int blood, ref int attack, ref int defence, ref int agility, ref int lucky)
	{
		double num = (double)(pet.BloodGrow / 10) * 0.1;
		double num2 = (double)(pet.AttackGrow / 10) * 0.1;
		double num3 = (double)(pet.DefenceGrow / 10) * 0.1;
		double num4 = (double)(pet.AgilityGrow / 10) * 0.1;
		double num5 = (double)(pet.LuckGrow / 10) * 0.1;
		double num6 = 0.0;
		double num7 = pet.Blood;
		double num8 = pet.Attack;
		double num9 = pet.Defence;
		double num10 = pet.Agility;
		double num11 = pet.Luck;
		for (int i = min + 1; i <= max; i++)
		{
			num6 += (double)(min / 100);
			double x = 0.5;
			num7 += num + Math.Pow(x, i);
			num8 += num2 + Math.Pow(x, i);
			num9 += num3 + Math.Pow(x, i);
			num10 += num4 + Math.Pow(x, i);
			num11 += num5 + Math.Pow(x, i);
		}
		blood = (int)(num * (num7 / (num + num6)));
		attack = (int)(num2 * (num8 / (num2 + num6)));
		defence = (int)(num3 * (num9 / (num3 + num6)));
		agility = (int)(num4 * (num10 / (num4 + num6)));
		lucky = (int)(num5 * (num11 / (num5 + num6)));
	}

	public static UsersPetinfo CreatePet(PetTemplateInfo info, int userID, int place, int playerLevel)
	{
		UsersPetinfo usersPetinfo = new UsersPetinfo();
		int starLevel = info.StarLevel;
		int minValue = 200 + 100 * starLevel;
		int maxValue = 350 + 100 * starLevel;
		int minValue2 = 1700 + 1000 * starLevel;
		int maxValue2 = 2200 + 2500 * starLevel;
		usersPetinfo.ID = 0;
		usersPetinfo.BloodGrow = rand.Next(minValue2, maxValue2);
		usersPetinfo.AttackGrow = rand.Next(minValue, maxValue);
		usersPetinfo.DefenceGrow = rand.Next(minValue, maxValue);
		usersPetinfo.AgilityGrow = rand.Next(minValue, maxValue);
		usersPetinfo.LuckGrow = rand.Next(minValue, maxValue);
		usersPetinfo.DamageGrow = 0;
		usersPetinfo.GuardGrow = 0;
		double num = (double)rand.Next(54, 61) * 0.1;
		double num2 = (double)rand.Next(9, 13) * 0.1;
		usersPetinfo.Blood = (int)((double)(rand.Next(minValue2, maxValue2) / 10) * 0.1 * num);
		usersPetinfo.Attack = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
		usersPetinfo.Defence = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
		usersPetinfo.Agility = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
		usersPetinfo.Luck = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num2);
		usersPetinfo.Damage = 0;
		usersPetinfo.Guard = 0;
		usersPetinfo.Hunger = 10000;
		usersPetinfo.TemplateID = info.TemplateID;
		usersPetinfo.Name = info.Name;
		usersPetinfo.UserID = userID;
		usersPetinfo.Place = place;
		usersPetinfo.Level = 1;
		usersPetinfo.Skill = UpdateSkillPet(1, info.TemplateID, playerLevel);
		usersPetinfo.SkillEquip = ActiveEquipSkill(1);
		return usersPetinfo;
	}

	public static UsersPetinfo CreateNewPet()
	{
		string[] array = FindConfig("NewPet").Value.Split(',');
		int num = rand.Next(array.Length);
		PetTemplateInfo petTemplateInfo = FindPetTemplate(Convert.ToInt32(array[num]));
		UsersPetinfo usersPetinfo = new UsersPetinfo();
		int starLevel = petTemplateInfo.StarLevel;
		int minValue = 300 + 100 * starLevel;
		int maxValue = 450 + 100 * starLevel;
		int minValue2 = 1800 + 1000 * starLevel;
		int maxValue2 = 2300 + 2500 * starLevel;
		usersPetinfo.ID = 0;
		usersPetinfo.BloodGrow = rand.Next(minValue2, maxValue2);
		usersPetinfo.AttackGrow = rand.Next(minValue, maxValue);
		usersPetinfo.DefenceGrow = rand.Next(minValue, maxValue);
		usersPetinfo.AgilityGrow = rand.Next(minValue, maxValue);
		usersPetinfo.LuckGrow = rand.Next(minValue, maxValue);
		usersPetinfo.DamageGrow = 0;
		usersPetinfo.GuardGrow = 0;
		double num2 = (double)rand.Next(54, 61) * 0.1;
		double num3 = (double)rand.Next(9, 13) * 0.1;
		usersPetinfo.Blood = (int)((double)(rand.Next(minValue2, maxValue2) / 10) * 0.1 * num2);
		usersPetinfo.Attack = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num3);
		usersPetinfo.Defence = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num3);
		usersPetinfo.Agility = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num3);
		usersPetinfo.Luck = (int)((double)(rand.Next(minValue, maxValue) / 10) * 0.1 * num3);
		usersPetinfo.Damage = 0;
		usersPetinfo.Guard = 0;
		usersPetinfo.Hunger = 10000;
		usersPetinfo.TemplateID = petTemplateInfo.TemplateID;
		usersPetinfo.Name = petTemplateInfo.Name;
		usersPetinfo.UserID = -1;
		usersPetinfo.Place = -1;
		usersPetinfo.Level = 60;
		usersPetinfo.Skill = UpdateSkillPet(60, petTemplateInfo.TemplateID, 60);
		usersPetinfo.SkillEquip = ActiveEquipSkill(1);
		return usersPetinfo;
	}
}
