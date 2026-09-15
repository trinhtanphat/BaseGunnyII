using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class ConsortiaMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<string, int> _ally;

	private static ReaderWriterLock m_lock;

	private static Dictionary<int, ConsortiaInfo> _consortia;

	private static Dictionary<int, ConsortiaBossConfigInfo> _consortiaBossConfigInfos;

	public static bool ReLoad()
	{
		try
		{
			Dictionary<string, int> ally = new Dictionary<string, int>();
			Dictionary<int, ConsortiaInfo> consortia = new Dictionary<int, ConsortiaInfo>();
			Dictionary<int, ConsortiaBossConfigInfo> dictionary = new Dictionary<int, ConsortiaBossConfigInfo>();
			if (Load(ally) && LoadConsortia(consortia, dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_ally = ally;
					_consortia = consortia;
					_consortiaBossConfigInfos = dictionary;
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
				log.Error("ConsortiaMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_ally = new Dictionary<string, int>();
			if (!Load(_ally))
			{
				return false;
			}
			_consortia = new Dictionary<int, ConsortiaInfo>();
			_consortiaBossConfigInfos = new Dictionary<int, ConsortiaBossConfigInfo>();
			if (!LoadConsortia(_consortia, _consortiaBossConfigInfos))
			{
				return false;
			}
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ConsortiaMgr", exception);
			}
			return false;
		}
	}

	private static bool Load(Dictionary<string, int> ally)
	{
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaAllyInfo[] consortiaAllyAll = consortiaBussiness.GetConsortiaAllyAll();
			ConsortiaAllyInfo[] array = consortiaAllyAll;
			foreach (ConsortiaAllyInfo consortiaAllyInfo in array)
			{
				if (consortiaAllyInfo.IsExist)
				{
					string key = ((consortiaAllyInfo.Consortia1ID >= consortiaAllyInfo.Consortia2ID) ? (consortiaAllyInfo.Consortia2ID + "&" + consortiaAllyInfo.Consortia1ID) : (consortiaAllyInfo.Consortia1ID + "&" + consortiaAllyInfo.Consortia2ID));
					if (!ally.ContainsKey(key))
					{
						ally.Add(key, consortiaAllyInfo.State);
					}
				}
			}
		}
		return true;
	}

	private static bool LoadConsortia(Dictionary<int, ConsortiaInfo> consortia, Dictionary<int, ConsortiaBossConfigInfo> consortiaBossConfig)
	{
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			ConsortiaInfo[] consortiaAll = consortiaBussiness.GetConsortiaAll();
			ConsortiaInfo[] array = consortiaAll;
			foreach (ConsortiaInfo consortiaInfo in array)
			{
				if (consortiaInfo.IsExist && !consortia.ContainsKey(consortiaInfo.ConsortiaID))
				{
					consortia.Add(consortiaInfo.ConsortiaID, consortiaInfo);
				}
			}
			ConsortiaBossConfigInfo[] consortiaBossConfigAll = consortiaBussiness.GetConsortiaBossConfigAll();
			ConsortiaBossConfigInfo[] array2 = consortiaBossConfigAll;
			foreach (ConsortiaBossConfigInfo consortiaBossConfigInfo in array2)
			{
				if (!consortiaBossConfig.ContainsKey(consortiaBossConfigInfo.BossLevel))
				{
					consortiaBossConfig.Add(consortiaBossConfigInfo.BossLevel, consortiaBossConfigInfo);
				}
			}
		}
		return true;
	}

	public static int UpdateConsortiaAlly(int cosortiaID1, int consortiaID2, int state)
	{
		string key = ((cosortiaID1 >= consortiaID2) ? (consortiaID2 + "&" + cosortiaID1) : (cosortiaID1 + "&" + consortiaID2));
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (!_ally.ContainsKey(key))
			{
				_ally.Add(key, state);
			}
			else
			{
				_ally[key] = state;
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
		return 0;
	}

	public static bool ConsortiaUpGrade(int consortiaID, int consortiaLevel)
	{
		bool result = false;
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
			{
				_consortia[consortiaID].Level = consortiaLevel;
			}
			else
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.BuildDate = DateTime.Now;
				consortiaInfo.Level = consortiaLevel;
				consortiaInfo.IsExist = true;
				_consortia.Add(consortiaID, consortiaInfo);
			}
		}
		catch (Exception exception)
		{
			log.Error("ConsortiaUpGrade", exception);
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
		return result;
	}

	public static bool ConsortiaStoreUpGrade(int consortiaID, int storeLevel)
	{
		bool result = false;
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
			{
				_consortia[consortiaID].StoreLevel = storeLevel;
			}
		}
		catch (Exception exception)
		{
			log.Error("ConsortiaUpGrade", exception);
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
		return result;
	}

	public static bool ConsortiaShopUpGrade(int consortiaID, int shopLevel)
	{
		bool result = false;
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
			{
				_consortia[consortiaID].ShopLevel = shopLevel;
			}
		}
		catch (Exception exception)
		{
			log.Error("ConsortiaUpGrade", exception);
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
		return result;
	}

	public static bool ConsortiaSmithUpGrade(int consortiaID, int smithLevel)
	{
		bool result = false;
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (_consortia.ContainsKey(consortiaID) && _consortia[consortiaID].IsExist)
			{
				_consortia[consortiaID].SmithLevel = smithLevel;
			}
		}
		catch (Exception exception)
		{
			log.Error("ConsortiaUpGrade", exception);
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
		return result;
	}

	public static bool AddConsortia(int consortiaID)
	{
		bool result = false;
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (!_consortia.ContainsKey(consortiaID))
			{
				ConsortiaInfo consortiaInfo = new ConsortiaInfo();
				consortiaInfo.BuildDate = DateTime.Now;
				consortiaInfo.Level = 1;
				consortiaInfo.IsExist = true;
				consortiaInfo.ConsortiaName = "";
				consortiaInfo.ConsortiaID = consortiaID;
				_consortia.Add(consortiaID, consortiaInfo);
			}
		}
		catch (Exception exception)
		{
			log.Error("ConsortiaUpGrade", exception);
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
		return result;
	}

	public static ConsortiaInfo FindConsortiaInfo(int consortiaID)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_consortia.ContainsKey(consortiaID))
			{
				return _consortia[consortiaID];
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

	public static ConsortiaBossConfigInfo FindConsortiaBossConfig(int level)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_consortiaBossConfigInfos.ContainsKey(level))
			{
				return _consortiaBossConfigInfos[level];
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

	public static int FindConsortiaBossBossMaxLevel(int param1, ConsortiaInfo info)
	{
		int num = ((param1 != 0) ? param1 : (info.Level + info.SmithLevel + info.ShopLevel + info.StoreLevel + info.SkillLevel));
		int result = 1;
		for (int num2 = _consortiaBossConfigInfos.Count; num2 >= 0; num2--)
		{
			if (num >= _consortiaBossConfigInfos[num2].Level)
			{
				result = num2;
				break;
			}
		}
		return result;
	}

	public static int CanConsortiaFight(int consortiaID1, int consortiaID2)
	{
		if (consortiaID1 == 0 || consortiaID2 == 0 || consortiaID1 == consortiaID2)
		{
			return -1;
		}
		ConsortiaInfo consortiaInfo = FindConsortiaInfo(consortiaID1);
		ConsortiaInfo consortiaInfo2 = FindConsortiaInfo(consortiaID2);
		if (consortiaInfo == null || consortiaInfo2 == null || consortiaInfo.Level < 3 || consortiaInfo2.Level < 3)
		{
			return -1;
		}
		return FindConsortiaAlly(consortiaID1, consortiaID2);
	}

	public static int FindConsortiaAlly(int cosortiaID1, int consortiaID2)
	{
		if (cosortiaID1 == 0 || consortiaID2 == 0 || cosortiaID1 == consortiaID2)
		{
			return -1;
		}
		string key = ((cosortiaID1 >= consortiaID2) ? (consortiaID2 + "&" + cosortiaID1) : (cosortiaID1 + "&" + consortiaID2));
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_ally.ContainsKey(key))
			{
				return _ally[key];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return 0;
	}

	public static int GetOffer(int cosortiaID1, int consortiaID2, eGameType gameType)
	{
		return GetOffer(FindConsortiaAlly(cosortiaID1, consortiaID2), gameType);
	}

	private static int GetOffer(int state, eGameType gameType)
	{
		switch (gameType)
		{
		case eGameType.Free:
			switch (state)
			{
			case 0:
				return 1;
			case 1:
				return 0;
			case 2:
				return 3;
			}
			break;
		case eGameType.Guild:
			switch (state)
			{
			case 0:
				return 5;
			case 1:
				return 0;
			case 2:
				return 10;
			}
			break;
		}
		return 0;
	}

	public static int KillPlayer(GamePlayer win, GamePlayer lose, Dictionary<GamePlayer, Player> players, eRoomType roomType, eGameType gameClass)
	{
		if (roomType != eRoomType.Match)
		{
			return -1;
		}
		int num = FindConsortiaAlly(win.PlayerCharacter.ConsortiaID, lose.PlayerCharacter.ConsortiaID);
		if (num == -1)
		{
			return num;
		}
		int offer = GetOffer(num, gameClass);
		if (lose.PlayerCharacter.Offer < offer)
		{
			offer = lose.PlayerCharacter.Offer;
		}
		if (offer != 0)
		{
			players[win].GainOffer = offer;
			players[lose].GainOffer = -offer;
		}
		return num;
	}

	public static int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int playercount)
	{
		if (roomType != eRoomType.Match)
		{
			return 0;
		}
		int num = playercount / 2;
		int riches = 0;
		int state = 2;
		int num2 = 1;
		int num3 = 3;
		if (gameClass == eGameType.Guild)
		{
			num3 = 10;
			num2 = (int)RateMgr.GetRate(eRateType.Offer_Rate);
		}
		float rate = RateMgr.GetRate(eRateType.Riches_Rate);
		using (ConsortiaBussiness consortiaBussiness = new ConsortiaBussiness())
		{
			if (gameClass == eGameType.Free)
			{
				num = 0;
			}
			else
			{
				consortiaBussiness.ConsortiaFight(consortiaWin, consortiaLose, num, out riches, state, totalKillHealth, rate);
			}
			foreach (KeyValuePair<int, Player> player in players)
			{
				if (player.Value != null)
				{
					if (player.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaWin)
					{
						player.Value.PlayerDetail.AddOffer((num + num3) * num2);
						player.Value.PlayerDetail.PlayerCharacter.RichesRob += riches;
					}
					else if (player.Value.PlayerDetail.PlayerCharacter.ConsortiaID == consortiaLose)
					{
						player.Value.PlayerDetail.AddOffer((int)Math.Round((double)num * 0.5) * num2);
						player.Value.PlayerDetail.RemoveOffer(num3);
					}
				}
			}
		}
		return riches;
	}
}
