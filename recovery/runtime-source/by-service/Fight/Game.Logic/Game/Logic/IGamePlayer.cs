using System.Collections.Generic;
using Game.Base.Packets;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Game.Logic;

public interface IGamePlayer
{
	PlayerInfo PlayerCharacter { get; }

	UserMatchInfo MatchInfo { get; }

	ItemTemplateInfo MainWeapon { get; }

	ItemInfo SecondWeapon { get; }

	ItemInfo Healstone { get; }

	string ProcessLabyrinthAward { get; set; }

	UsersPetinfo Pet { get; }

	bool CanUseProp { get; set; }

	bool CanX2Exp { get; set; }

	bool CanX3Exp { get; set; }

	int GamePlayerId { get; set; }

	long WorldbossBood { get; }

	long AllWorldDameBoss { get; }

	int ServerID { get; set; }

	List<ItemInfo> EquipEffect { get; }

	List<BufferInfo> FightBuffs { get; }

	bool RemoveHealstone();

	bool isDoubleAward();

	void ClearFightBuffOneMatch();

	double GetBaseBlood();

	void UpdateBarrier(int barrier, string pic);

	double GetBaseAttack();

	double GetBaseDefence();

	void FootballTakeOut(bool isWin);

	int AddGP(int gp);

	int RemoveGP(int gp);

	int AddGold(int value);

	int RemoveGold(int value);

	int AddMoney(int value);

	int AddActiveMoney(int value);

	int RemoveMoney(int value);

	string GetFightFootballStyle(int team);

	int AddGiftToken(int value);

	int RemoveGiftToken(int value);

	int AddHardCurrency(int value);

	int AddMedal(int value);

	int RemoveMedal(int value);

	int AddHonor(int value);

	int AddDamageScores(int value);

	int AddLeagueMoney(int value);

	void AddPrestige(bool isWin);

	int AddOffer(int value);

	int RemoveOffer(int value);

	bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eItemNotice typeView, eItemNotice typeGet);

	void UpdatePveResult(string type, int value, bool isWin);

	bool ClearTempBag();

	bool ClearFightBag();

	bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving);

	void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int demage);

	void OnGameOver(AbstractGame game, bool isWin, int gainXp);

	void OnMissionOver(AbstractGame game, bool isWin, int MissionID, int TurnNum);

	int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count);

	void SendConsortiaFight(int consortiaID, int riches, string msg);

	bool SetPvePermission(int missionId, eHardLevel hardLevel);

	bool IsPvePermission(int missionId, eHardLevel hardLevel);

	void Disconnect();

	void UpdateRestCount();

	void SendInsufficientMoney(int type);

	void UpdateLabyrinth(int currentFloor, int m_missionInfoId, bool bigAward);

	void OutLabyrinth(bool isWin);

	void SendMessage(string msg);

	void SendHideMessage(string msg);

	void SendTCP(GSPacketIn pkg);

	void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney);
}
