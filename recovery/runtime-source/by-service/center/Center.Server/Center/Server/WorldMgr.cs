using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml.Linq;
using Bussiness;
using Bussiness.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Center.Server;

public class WorldMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static List<string> NotceList = new List<string>();

	private static object _syncStop = new object();

	private static Dictionary<string, RankingPersonInfo> m_rankList;

	private static Dictionary<string, RankingLightriddleInfo> m_lightriddleRankList;

	public static string[] name = new string[4] { "Cuồng long", "Bá tước hắc ám", "Bá tước hắc ám", "Đội trưởng" };

	public static string[] bossResourceId = new string[4] { "1", "2", "2", "4" };

	public static int[] Pve_Id = new int[4] { 1243, 30001, 30002, 30004 };

	public static readonly long MAX_BLOOD = -1294967296L;

	public static long current_blood = 0L;

	public static DateTime begin_time;

	public static DateTime end_time;

	public static bool fightOver;

	public static bool roomClose;

	public static bool worldOpen;

	private static readonly int worldbossTime = 60;

	public static DateTime LeagueOpenTime;

	public static bool IsLeagueOpen;

	public static DateTime BattleGoundOpenTime;

	public static bool IsBattleGoundOpen;

	public static DateTime FightFootballTime;

	public static bool IsFightFootballTime;

	public static int currentPVE_ID;

	public static int fight_time;

	private static Dictionary<int, LanternriddlesInfo> m_lanternriddlesInfo;

	private static Dictionary<int, LuckStarRewardRecordInfo> m_luckStarRewardRecordInfo;

	public static bool CanSendLightriddleAward;

	public static bool CanSendLuckyStarAward;

	private static int LuckStarCountDown;

	private static string SystemNoticeFile => ConfigurationManager.AppSettings["SystemNoticePath"];

	public static bool Start()
	{
		try
		{
			CanSendLightriddleAward = true;
			CanSendLuckyStarAward = true;
			m_rankList = new Dictionary<string, RankingPersonInfo>();
			m_lightriddleRankList = new Dictionary<string, RankingLightriddleInfo>();
			m_lanternriddlesInfo = new Dictionary<int, LanternriddlesInfo>();
			m_luckStarRewardRecordInfo = new Dictionary<int, LuckStarRewardRecordInfo>();
			current_blood = MAX_BLOOD;
			begin_time = DateTime.Now;
			LeagueOpenTime = DateTime.Now;
			BattleGoundOpenTime = DateTime.Now;
			FightFootballTime = DateTime.Now;
			ResetLuckStar();
			end_time = begin_time.AddDays(1.0);
			fightOver = true;
			roomClose = true;
			worldOpen = false;
			IsLeagueOpen = false;
			IsBattleGoundOpen = false;
			IsFightFootballTime = false;
			return LoadNotice("");
		}
		catch (Exception arg)
		{
			log.ErrorFormat("Load server list from db failed:{0}", arg);
			return false;
		}
	}

	public static void ResetLightriddleRank()
	{
		m_lightriddleRankList.Clear();
	}

	public static void ResetLuckStar()
	{
		m_luckStarRewardRecordInfo.Clear();
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			playerBussiness.ResetLuckStarRank();
		}
		LuckStarCountDown = Math.Abs(60 - DateTime.Now.Minute - 30);
	}

	public static void UpdateLuckStarRewardRecord(int PlayerID, string nickName, int TemplateID, int Count, int isVip)
	{
		if (m_luckStarRewardRecordInfo.Keys.Contains(PlayerID))
		{
			m_luckStarRewardRecordInfo[PlayerID].useStarNum++;
			return;
		}
		LuckStarRewardRecordInfo luckStarRewardRecordInfo = new LuckStarRewardRecordInfo();
		luckStarRewardRecordInfo.PlayerID = PlayerID;
		luckStarRewardRecordInfo.nickName = nickName;
		luckStarRewardRecordInfo.useStarNum = 1;
		luckStarRewardRecordInfo.TemplateID = TemplateID;
		luckStarRewardRecordInfo.Count = Count;
		luckStarRewardRecordInfo.isVip = isVip;
		m_luckStarRewardRecordInfo.Add(PlayerID, luckStarRewardRecordInfo);
	}

	public static void SendLuckyStarTopTenAward()
	{
		int minUseNum = GameProperties.MinUseNum;
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			LuckStarRewardRecordInfo[] luckStarTopTenRank = playerBussiness.GetLuckStarTopTenRank(minUseNum);
			LuckStarRewardRecordInfo[] array = luckStarTopTenRank;
			foreach (LuckStarRewardRecordInfo luckStarRewardRecordInfo in array)
			{
				string format = "Phần thưởng hạng {0} hoạt động Sao may mắn";
				List<LuckyStartToptenAwardInfo> luckyStartAwardByRank = WorldEventMgr.GetLuckyStartAwardByRank(luckStarRewardRecordInfo.rank);
				List<ItemInfo> list = new List<ItemInfo>();
				foreach (LuckyStartToptenAwardInfo item in luckyStartAwardByRank)
				{
					ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item.TemplateID);
					ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 105);
					itemInfo.IsBinds = item.IsBinds;
					itemInfo.ValidDate = item.Validate;
					itemInfo.Count = item.Count;
					list.Add(itemInfo);
				}
				format = string.Format(format, luckStarRewardRecordInfo.rank);
				WorldEventMgr.SendItemsToMail(list, luckStarRewardRecordInfo.PlayerID, luckStarRewardRecordInfo.nickName, format);
			}
		}
		CanSendLuckyStarAward = false;
	}

	public static List<LuckStarRewardRecordInfo> GetAllLuckyStarRank()
	{
		List<LuckStarRewardRecordInfo> list = new List<LuckStarRewardRecordInfo>();
		foreach (LuckStarRewardRecordInfo value in m_luckStarRewardRecordInfo.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public static void SavekyStarToDatabase()
	{
		List<LuckStarRewardRecordInfo> allLuckyStarRank = GetAllLuckyStarRank();
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		foreach (LuckStarRewardRecordInfo item in allLuckyStarRank)
		{
			playerBussiness.SaveLuckStarRankInfo(item);
		}
	}

	public static void SaveLuckyStarRewardRecord()
	{
		int hour = DateTime.Now.Hour;
		if (LuckStarCountDown > 0)
		{
			LuckStarCountDown--;
		}
		if (LuckStarCountDown == 0)
		{
			if (CanSendLuckyStarAward)
			{
				SavekyStarToDatabase();
				LuckStarCountDown = 30;
			}
			else
			{
				LuckStarCountDown = -1;
			}
		}
	}

	public static void AddOrUpdateLanternriddles(int playerID, LanternriddlesInfo Lanternriddles)
	{
		if (!m_lanternriddlesInfo.ContainsKey(playerID))
		{
			m_lanternriddlesInfo.Add(playerID, Lanternriddles);
		}
		else
		{
			m_lanternriddlesInfo[playerID] = Lanternriddles;
		}
	}

	public static LanternriddlesInfo GetLanternriddles(int playerID)
	{
		if (m_lanternriddlesInfo.ContainsKey(playerID))
		{
			return m_lanternriddlesInfo[playerID];
		}
		return null;
	}

	public static void UpdateLightriddleRank(int integer, int typeVip, string nickName, int playerId)
	{
		if (m_lightriddleRankList.Keys.Contains(nickName))
		{
			m_lightriddleRankList[nickName].Integer = integer;
		}
		else
		{
			RankingLightriddleInfo rankingLightriddleInfo = new RankingLightriddleInfo();
			rankingLightriddleInfo.NickName = nickName;
			rankingLightriddleInfo.Integer = integer;
			rankingLightriddleInfo.TypeVIP = typeVip;
			rankingLightriddleInfo.PlayerId = playerId;
			m_lightriddleRankList.Add(nickName, rankingLightriddleInfo);
		}
		SortRank();
	}

	public static void SendLightriddleTopEightAward()
	{
		List<RankingLightriddleInfo> list = SelectTopEight();
		foreach (RankingLightriddleInfo item in list)
		{
			string format = "Phần thưởng hạng {0} hoạt động nguyên tiêu";
			List<LuckyStartToptenAwardInfo> lanternriddlesAwardByRank = WorldEventMgr.GetLanternriddlesAwardByRank(item.Rank);
			List<ItemInfo> list2 = new List<ItemInfo>();
			foreach (LuckyStartToptenAwardInfo item2 in lanternriddlesAwardByRank)
			{
				ItemTemplateInfo goods = ItemMgr.FindItemTemplate(item2.TemplateID);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(goods, 1, 105);
				itemInfo.IsBinds = item2.IsBinds;
				itemInfo.ValidDate = item2.Validate;
				itemInfo.Count = item2.Count;
				list2.Add(itemInfo);
			}
			format = string.Format(format, item.Rank);
			WorldEventMgr.SendItemsToMail(list2, item.PlayerId, item.NickName, format);
		}
		CanSendLightriddleAward = false;
	}

	public static List<RankingLightriddleInfo> SelectTopEight()
	{
		List<RankingLightriddleInfo> list = new List<RankingLightriddleInfo>();
		IOrderedEnumerable<KeyValuePair<string, RankingLightriddleInfo>> orderedEnumerable = from pair in m_lightriddleRankList
			where pair.Value.Integer > 1
			orderby pair.Value.Integer descending
			select pair;
		foreach (KeyValuePair<string, RankingLightriddleInfo> item in orderedEnumerable)
		{
			if (list.Count == 8)
			{
				break;
			}
			list.Add(item.Value);
		}
		return list;
	}

	public static void SortRank()
	{
		IOrderedEnumerable<KeyValuePair<string, RankingLightriddleInfo>> orderedEnumerable = m_lightriddleRankList.OrderByDescending((KeyValuePair<string, RankingLightriddleInfo> pair) => pair.Value.Integer);
		int num = 1;
		Dictionary<string, RankingLightriddleInfo> dictionary = new Dictionary<string, RankingLightriddleInfo>();
		foreach (KeyValuePair<string, RankingLightriddleInfo> item in orderedEnumerable)
		{
			item.Value.Rank = num;
			dictionary.Add(item.Key, item.Value);
			num++;
		}
		m_lightriddleRankList = dictionary;
	}

	public static void UpdateRank(int damage, int honor, string nickName)
	{
		if (m_rankList.Keys.Contains(nickName))
		{
			m_rankList[nickName].Damage += damage;
			m_rankList[nickName].Honor += honor;
			return;
		}
		RankingPersonInfo rankingPersonInfo = new RankingPersonInfo();
		rankingPersonInfo.ID = m_rankList.Count + 1;
		rankingPersonInfo.Name = nickName;
		rankingPersonInfo.Damage = damage;
		rankingPersonInfo.Honor = honor;
		m_rankList.Add(nickName, rankingPersonInfo);
	}

	public static bool CheckName(string NickName)
	{
		return m_rankList.Keys.Contains(NickName);
	}

	public static RankingPersonInfo GetSingleRank(string name)
	{
		return m_rankList[name];
	}

	public static List<RankingPersonInfo> SelectTopTen()
	{
		List<RankingPersonInfo> list = new List<RankingPersonInfo>();
		IOrderedEnumerable<KeyValuePair<string, RankingPersonInfo>> orderedEnumerable = m_rankList.OrderByDescending((KeyValuePair<string, RankingPersonInfo> pair) => pair.Value.Damage);
		foreach (KeyValuePair<string, RankingPersonInfo> item in orderedEnumerable)
		{
			if (list.Count == 10)
			{
				break;
			}
			list.Add(item.Value);
		}
		return list;
	}

	public static void SetupWorldBoss(int id)
	{
		current_blood = MAX_BLOOD;
		begin_time = DateTime.Now;
		end_time = begin_time.AddDays(1.0);
		fight_time = worldbossTime - begin_time.Minute;
		fightOver = false;
		roomClose = false;
		currentPVE_ID = id;
		worldOpen = true;
	}

	public static void WorldBossFightOver()
	{
		fightOver = true;
	}

	public static void WorldBossRoomClose()
	{
		roomClose = true;
	}

	public static void UpdateFightTime()
	{
		if (!fightOver)
		{
			fight_time = worldbossTime - begin_time.Minute;
		}
	}

	public static void WorldBossClose()
	{
		worldOpen = false;
	}

	public static void WorldBossClearRank()
	{
		m_rankList.Clear();
	}

	public static void ReduceBlood(int value)
	{
		if (current_blood > 0)
		{
			current_blood -= value;
		}
	}

	public static bool LoadNotice(string path)
	{
		string text = path + SystemNoticeFile;
		if (!File.Exists(text))
		{
			log.Error("SystemNotice file : " + text + " not found !");
		}
		else
		{
			try
			{
				XDocument xDocument = XDocument.Load(text);
				foreach (XElement item in xDocument.Root.Nodes())
				{
					try
					{
						int.Parse(item.Attribute("id").Value);
						string value = item.Attribute("notice").Value;
						NotceList.Add(value);
					}
					catch (Exception exception)
					{
						log.Error("BattleMgr setup error:", exception);
					}
				}
			}
			catch (Exception exception2)
			{
				log.Error("BattleMgr setup error:", exception2);
			}
		}
		log.InfoFormat("Total {0} syterm notice loaded.", NotceList.Count);
		return true;
	}
}
