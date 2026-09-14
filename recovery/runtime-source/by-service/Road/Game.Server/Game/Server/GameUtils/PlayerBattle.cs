using System.Reflection;
using Bussiness;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameUtils;

public class PlayerBattle
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected object m_lock = new object();

	public readonly int Attack = 1700;

	public readonly int Defend = 1500;

	public readonly int Agility = 1600;

	public readonly int Lucky = 1500;

	public readonly int Damage = 1000;

	public readonly int Guard = 500;

	public readonly int Blood = 25000;

	public readonly int Energy = 293;

	public readonly int LevelLimit = 15;

	public readonly int fairBattleDayPrestige = 2000;

	public readonly int maxCount = 30;

	protected GamePlayer m_player;

	private UserMatchInfo m_matchInfo;

	private bool m_saveToDb;

	public GamePlayer Player => m_player;

	public UserMatchInfo MatchInfo
	{
		get
		{
			return m_matchInfo;
		}
		set
		{
			m_matchInfo = value;
		}
	}

	public PlayerBattle(GamePlayer player, bool saveTodb)
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
		m_matchInfo = playerBussiness.GetSingleUserMatchInfo(Player.PlayerCharacter.ID);
		if (m_matchInfo == null)
		{
			CreateInfo(Player.PlayerCharacter.ID);
		}
		m_matchInfo.maxCount = maxCount;
	}

	public void Reset()
	{
		m_matchInfo.dailyScore = 0;
		m_matchInfo.addDayPrestge = 0;
		m_matchInfo.restCount = 30;
	}

	public int GetRank()
	{
		return RankMgr.FindRank(Player.PlayerCharacter.ID)?.rank ?? 0;
	}

	public void Update()
	{
		if (m_matchInfo.restCount > 0)
		{
			m_matchInfo.restCount--;
			Player.Out.SendLeagueNotice(Player.PlayerCharacter.ID, MatchInfo.restCount, maxCount, 3);
		}
	}

	public void AddPrestige(bool isWin)
	{
		int totalPrestige = m_matchInfo.totalPrestige;
		FairBattleRewardInfo battleDataByPrestige = FairBattleRewardMgr.GetBattleDataByPrestige(totalPrestige);
		if (battleDataByPrestige == null)
		{
			Player.SendMessage(LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg5"));
			return;
		}
		int num = battleDataByPrestige.PrestigeForWin;
		string translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg3", num);
		if (!isWin)
		{
			num = battleDataByPrestige.PrestigeForLose;
			translation = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg4", num);
		}
		if (m_matchInfo.addDayPrestge < fairBattleDayPrestige)
		{
			m_matchInfo.addDayPrestge += num;
			m_matchInfo.totalPrestige += num;
		}
		Player.SendMessage(translation);
	}

	public void CreateInfo(int UserID)
	{
		m_matchInfo = new UserMatchInfo();
		m_matchInfo.ID = 0;
		m_matchInfo.UserID = UserID;
		m_matchInfo.dailyScore = 0;
		m_matchInfo.dailyWinCount = 0;
		m_matchInfo.dailyGameCount = 0;
		m_matchInfo.DailyLeagueFirst = true;
		m_matchInfo.DailyLeagueLastScore = 0;
		m_matchInfo.weeklyScore = 0;
		m_matchInfo.weeklyGameCount = 0;
		m_matchInfo.weeklyRanking = 1000;
		m_matchInfo.addDayPrestge = 0;
		m_matchInfo.totalPrestige = 0;
		m_matchInfo.restCount = 30;
		m_matchInfo.maxCount = maxCount;
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
			if (m_matchInfo != null && m_matchInfo.IsDirty)
			{
				if (m_matchInfo.ID > 0)
				{
					playerBussiness.UpdateUserMatchInfo(m_matchInfo);
				}
				else
				{
					playerBussiness.AddUserMatchInfo(m_matchInfo);
				}
			}
		}
	}
}
