using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Fighting.Server.GameObjects;

public class ProxyPlayer : IGamePlayer
{
	private ServerClient m_client;

	private PlayerInfo m_character;

	private UserMatchInfo m_matchInfo;

	private ItemTemplateInfo m_currentWeapon;

	private ItemInfo m_secondWeapon;

	private ItemInfo m_healstone;

	private UsersPetinfo m_pet;

	private bool m_canUseProp;

	private bool m_canX2Exp;

	private bool m_canX3Exp;

	private int m_gamePlayerId;

	private double GPRate;

	private double OfferRate;

	public double m_antiAddictionRate;

	public List<BufferInfo> Buffers;

	public int m_serverid;

	public string FightFootballStyle;

	private List<BufferInfo> m_fightBuffs;

	private double m_baseAglilty;

	private double m_baseAttack;

	private double m_baseDefence;

	private double m_baseBlood;

	private List<ItemInfo> m_equipEffect;

	public int ServerID
	{
		get
		{
			return m_serverid;
		}
		set
		{
			m_serverid = value;
		}
	}

	public int GamePlayerId
	{
		get
		{
			return m_gamePlayerId;
		}
		set
		{
			m_gamePlayerId = value;
			m_client.SendGamePlayerId(this);
		}
	}

	public PlayerInfo PlayerCharacter => m_character;

	public List<BufferInfo> FightBuffs => m_fightBuffs;

	public UserMatchInfo MatchInfo => m_matchInfo;

	public ItemTemplateInfo MainWeapon => m_currentWeapon;

	public ItemInfo SecondWeapon => m_secondWeapon;

	public ItemInfo Healstone => m_healstone;

	public UsersPetinfo Pet => m_pet;

	public long WorldbossBood => 0L;

	public long AllWorldDameBoss => 0L;

	public string ProcessLabyrinthAward { get; set; }

	public bool CanUseProp
	{
		get
		{
			return m_canUseProp;
		}
		set
		{
			m_canUseProp = value;
		}
	}

	public bool CanX2Exp
	{
		get
		{
			return m_canX2Exp;
		}
		set
		{
			m_canX2Exp = value;
		}
	}

	public bool CanX3Exp
	{
		get
		{
			return m_canX3Exp;
		}
		set
		{
			m_canX3Exp = value;
		}
	}

	public List<ItemInfo> EquipEffect => m_equipEffect;

	public ProxyPlayer(ServerClient client, PlayerInfo character, UserMatchInfo matchInfo, ProxyPlayerInfo proxyPlayer, UsersPetinfo pet, List<BufferInfo> infos, List<ItemInfo> euipEffects, List<BufferInfo> fightBuffs)
	{
		m_client = client;
		m_character = character;
		m_matchInfo = matchInfo;
		m_pet = pet;
		m_serverid = proxyPlayer.ServerId;
		m_baseAttack = proxyPlayer.BaseAttack;
		m_baseDefence = proxyPlayer.BaseDefence;
		m_baseAglilty = proxyPlayer.BaseAgility;
		m_baseBlood = proxyPlayer.BaseBlood;
		m_currentWeapon = proxyPlayer.GetItemTemplateInfo();
		m_secondWeapon = proxyPlayer.GetItemInfo();
		m_healstone = proxyPlayer.GetHealstone();
		GPRate = proxyPlayer.GPAddPlus;
		OfferRate = proxyPlayer.OfferAddPlus;
		m_antiAddictionRate = proxyPlayer.AntiAddictionRate;
		m_equipEffect = euipEffects;
		Buffers = infos;
		m_fightBuffs = fightBuffs;
		FightFootballStyle = proxyPlayer.FightFootballStyle;
	}

	public double GetBaseAgility()
	{
		return m_baseAglilty;
	}

	public double GetBaseAttack()
	{
		return m_baseAttack;
	}

	public double GetBaseDefence()
	{
		return m_baseDefence;
	}

	public void ClearFightBuffOneMatch()
	{
	}

	public string GetFightFootballStyle(int team)
	{
		if (team == 1)
		{
			return FightFootballStyle.Split(';')[0];
		}
		return FightFootballStyle.Split(';')[1];
	}

	public void UpdatePveResult(string type, int value, bool isWin)
	{
	}

	public double GetBaseBlood()
	{
		return m_baseBlood;
	}

	public void UpdateBarrier(int barrier, string pic)
	{
	}

	public void FootballTakeOut(bool isWin)
	{
		m_client.SendFootballTakeOut(PlayerCharacter.ID, isWin);
	}

	public int AddGP(int gp)
	{
		if (gp > 0)
		{
			m_client.SendPlayerAddGP(PlayerCharacter.ID, gp);
		}
		return (int)(GPRate * (double)gp);
	}

	public int RemoveGP(int gp)
	{
		m_client.SendPlayerRemoveGP(PlayerCharacter.ID, gp);
		return gp;
	}

	public int AddGold(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddGold(PlayerCharacter.ID, value);
		}
		return value;
	}

	public int AddHonor(int value)
	{
		return value;
	}

	public int AddDamageScores(int value)
	{
		return value;
	}

	public int RemoveGold(int value)
	{
		m_client.SendPlayerRemoveGold(m_character.ID, value);
		return 0;
	}

	public int AddMoney(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddMoney(m_character.ID, value);
		}
		return value;
	}

	public int AddActiveMoney(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddActiveMoney(m_character.ID, value);
		}
		return value;
	}

	public int RemoveMoney(int value)
	{
		m_client.SendPlayerRemoveMoney(m_character.ID, value);
		return 0;
	}

	public int AddGiftToken(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddGiftToken(m_character.ID, value);
		}
		return value;
	}

	public bool RemoveHealstone()
	{
		m_client.SendPlayerRemoveHealstone(m_character.ID);
		return false;
	}

	public int AddMedal(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddMedal(m_character.ID, value);
		}
		return value;
	}

	public int AddLeagueMoney(int value)
	{
		if (value > 0)
		{
			m_client.SendPlayerAddLeagueMoney(m_character.ID, value);
		}
		return value;
	}

	public void AddPrestige(bool isWin)
	{
		m_client.SendPlayerAddPrestige(m_character.ID, isWin);
	}

	public void UpdateRestCount()
	{
		m_client.SendUpdateRestCount(m_character.ID);
	}

	public int RemoveGiftToken(int value)
	{
		return 0;
	}

	public int RemoveMedal(int value)
	{
		return 0;
	}

	public int AddHardCurrency(int value)
	{
		return 0;
	}

	public bool isDoubleAward()
	{
		return false;
	}

	public void UpdateLabyrinth(int currentFloor, int m_missionInfoId, bool bigAward)
	{
	}

	public void OutLabyrinth()
	{
	}

	public int AddOffer(int baseoffer)
	{
		if (baseoffer < 0)
		{
			return baseoffer;
		}
		return (int)((double)baseoffer * OfferRate * m_antiAddictionRate);
	}

	public int RemoveOffer(int value)
	{
		m_client.SendPlayerRemoveOffer(m_character.ID, value);
		return value;
	}

	public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
	{
	}

	public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
	{
		m_client.SendPlayerUsePropInGame(PlayerCharacter.ID, bag, place, templateId, isLiving);
		game.Pause(500);
		return false;
	}

	public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
	{
		m_client.SendPlayerOnGameOver(PlayerCharacter.ID, game.Id, isWin, gainXp);
	}

	public void Disconnect()
	{
		m_client.SendDisconnectPlayer(m_character.ID);
	}

	public void SendTCP(GSPacketIn pkg)
	{
		m_client.SendPacketToPlayer(m_character.ID, pkg);
	}

	public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage)
	{
		m_client.SendPlayerOnKillingLiving(m_character.ID, game, type, id, isLiving, demage);
	}

	public void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int turnNum)
	{
		m_client.SendPlayerOnMissionOver(m_character.ID, game, isWin, MissionID, turnNum);
	}

	public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
	{
		m_client.SendPlayerConsortiaFight(m_character.ID, consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth);
		return 0;
	}

	public void SendConsortiaFight(int consortiaID, int riches, string msg)
	{
		m_client.SendPlayerSendConsortiaFight(m_character.ID, consortiaID, riches, msg);
	}

	public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eItemNotice typeView, eItemNotice typeGet)
	{
		m_client.SendPlayerAddTemplate(m_character.ID, cloneItem, bagType, count);
		return true;
	}

	public void OutLabyrinth(bool isWin)
	{
	}

	public void SendMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendHideMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public bool IsPvePermission(int missionId, eHardLevel hardLevel)
	{
		return true;
	}

	public bool SetPvePermission(int missionId, eHardLevel hardLevel)
	{
		return true;
	}

	public void SendInsufficientMoney(int type)
	{
	}

	public bool ClearTempBag()
	{
		return true;
	}

	public bool ClearFightBag()
	{
		return true;
	}
}
