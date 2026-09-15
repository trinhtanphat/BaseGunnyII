using System;
using System.Collections.Generic;
using System.Configuration;
using System.Drawing;
using System.Reflection;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class PVPGame : BaseGame
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static readonly double GP_RATE = int.Parse(ConfigurationManager.AppSettings["GP_RATE"]);

	private static readonly double MONEY_RATE = int.Parse(ConfigurationManager.AppSettings["MONEY_RATE"]);

	private static readonly int MONEY_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_LOSE"]);

	private static readonly int MONEY_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_LOSE"]);

	private static readonly int MONEY_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["MONEY_MIN_RATE_WIN"]);

	private static readonly int MONEY_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["MONEY_MAX_RATE_WIN"]);

	private static readonly int DDT_MONEY_ACTIVE = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_ACTIVE"]);

	private static readonly int DDT_MONEY_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MIN_RATE_LOSE"]);

	private static readonly int DDT_MONEY_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MAX_RATE_LOSE"]);

	private static readonly int DDT_MONEY_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MIN_RATE_WIN"]);

	private static readonly int DDT_MONEY_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["DDT_MONEY_MAX_RATE_WIN"]);

	private static readonly int EXP_MIN_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_LOSE"]);

	private static readonly int EXP_MAX_RATE_LOSE = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_LOSE"]);

	private static readonly int EXP_MIN_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["EXP_MIN_RATE_WIN"]);

	private static readonly int EXP_MAX_RATE_WIN = int.Parse(ConfigurationManager.AppSettings["EXP_MAX_RATE_WIN"]);

	private static readonly int LeagueMoney_Lose = int.Parse(ConfigurationManager.AppSettings["LeagueMoney_Lose"]);

	private static readonly int LeagueMoney_Win = int.Parse(ConfigurationManager.AppSettings["LeagueMoney_Win"]);

	private List<Player> m_redTeam;

	private float m_redAvgLevel;

	private List<Player> m_blueTeam;

	private float m_blueAvgLevel;

	private int BeginPlayerCount;

	private string teamAStr;

	private string teamBStr;

	private DateTime beginTime;

	public Player CurrentPlayer => m_currentLiving as Player;

	public PVPGame(int id, int roomId, List<IGamePlayer> red, List<IGamePlayer> blue, Map map, eRoomType roomType, eGameType gameType, int timeType)
		: base(id, roomId, map, roomType, gameType, timeType)
	{
		m_redTeam = new List<Player>();
		m_blueTeam = new List<Player>();
		StringBuilder stringBuilder = new StringBuilder();
		m_redAvgLevel = 0f;
		foreach (IGamePlayer item in red)
		{
			Player player = new Player(item, PhysicalId++, this, 1, item.PlayerCharacter.hp);
			stringBuilder.Append(item.PlayerCharacter.ID).Append(",");
			player.Reset();
			player.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
			AddPlayer(item, player);
			m_redTeam.Add(player);
			m_redAvgLevel += item.PlayerCharacter.Grade;
		}
		m_redAvgLevel /= m_redTeam.Count;
		teamAStr = stringBuilder.ToString();
		StringBuilder stringBuilder2 = new StringBuilder();
		m_blueAvgLevel = 0f;
		foreach (IGamePlayer item2 in blue)
		{
			Player player2 = new Player(item2, PhysicalId++, this, 2, item2.PlayerCharacter.hp);
			stringBuilder2.Append(item2.PlayerCharacter.ID).Append(",");
			player2.Reset();
			player2.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
			AddPlayer(item2, player2);
			m_blueTeam.Add(player2);
			m_blueAvgLevel += item2.PlayerCharacter.Grade;
		}
		m_blueAvgLevel /= blue.Count;
		teamBStr = stringBuilder2.ToString();
		BeginPlayerCount = m_redTeam.Count + m_blueTeam.Count;
		beginTime = DateTime.Now;
	}

	public void Prepare()
	{
		if (base.GameState == eGameState.Inited)
		{
			SendCreateGame();
			m_gameState = eGameState.Prepared;
			CheckState(0);
		}
	}

	public void StartLoading()
	{
		if (base.GameState != eGameState.Prepared)
		{
			return;
		}
		if (base.RoomType == eRoomType.FightFootballTime)
		{
			LoadFightFootballResources();
		}
		ClearWaitTimer();
		SendStartLoading(60);
		VaneLoading();
		if (base.RoomType == eRoomType.Encounter)
		{
			List<Player> allFightPlayers = GetAllFightPlayers();
			foreach (Player item in allFightPlayers)
			{
				SendSelectObject(item.Id);
			}
		}
		AddAction(new WaitPlayerLoadingAction(this, 61000));
		m_gameState = eGameState.Loading;
	}

	public void LoadFightFootballResources()
	{
		int[] array = new int[5] { 10008, 10005, 10006, 10009, 10007 };
		int[] array2 = array;
		foreach (int id in array2)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(id);
			if (npcInfoById == null)
			{
				log.Error("LoadResources npcInfo resoure is not exits");
			}
			else
			{
				AddLoadingFile(2, npcInfoById.ResourcesPath, npcInfoById.ModelID);
			}
		}
		AddLoadingFile(1, "bombs/24.swf", "tank.resource.bombs.Bomb24");
	}

	public void StartGame()
	{
		if (base.GameState != eGameState.Loading)
		{
			return;
		}
		m_gameState = eGameState.Playing;
		ClearWaitTimer();
		SendSyncLifeTime();
		List<Player> allFightPlayers = GetAllFightPlayers();
		MapPoint mapRandomPos = MapMgr.GetMapRandomPos(m_map.Info.ID);
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(99);
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			item.Reset();
			Point playerPoint = GetPlayerPoint(mapRandomPos, item.Team);
			item.SetXY(playerPoint);
			m_map.AddPhysical(item);
			item.StartMoving();
			item.StartGame();
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.X);
			gSPacketIn.WriteInt(item.Y);
			gSPacketIn.WriteInt(item.Direction);
			gSPacketIn.WriteInt(item.Blood);
			gSPacketIn.WriteInt(item.MaxBlood);
			gSPacketIn.WriteInt(item.Team);
			gSPacketIn.WriteInt(item.Weapon.RefineryLevel);
			gSPacketIn.WriteInt(item.deputyWeaponCount);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(item.Dander);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.PlayerDetail.FightBuffs.Count);
			foreach (BufferInfo fightBuff in item.PlayerDetail.FightBuffs)
			{
				gSPacketIn.WriteInt(fightBuff.Type);
				gSPacketIn.WriteInt(fightBuff.Value);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(item.IsFrost);
			gSPacketIn.WriteBoolean(item.IsHide);
			gSPacketIn.WriteBoolean(item.IsNoHole);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(0);
		}
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteDateTime(DateTime.Now);
		SendToAll(gSPacketIn);
		VaneLoading();
		WaitTime(allFightPlayers.Count * 1000);
		OnGameStarted();
	}

	public void CreateNpc(int npcId, int x, int y, int type, int direction)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleNpc simpleNpc = new SimpleNpc(PhysicalId++, this, npcInfoById, type, direction);
		simpleNpc.Reset();
		simpleNpc.SetXY(x, y);
		AddLiving(simpleNpc);
		simpleNpc.StartMoving();
	}

	public void CreateNpc()
	{
		int[] array = new int[5] { 10008, 10005, 10006, 10009, 10007 };
		int[] array2 = new int[3] { 350, 500, 680 };
		Shuffer(array2);
		Shuffer(array);
		ClearAllNpc();
		int num = array2[base.Random.Next(array2.Length)];
		foreach (int npcId in array)
		{
			CreateNpc(npcId, num, 259, 1, -1);
			num += 210;
		}
	}

	public void NextTurn()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		ClearWaitTimer();
		ClearDiedPhysicals();
		CheckBox();
		base.m_turnIndex++;
		List<Box> newBoxes = CreateBox();
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item in allPhysicalSafe)
		{
			item.PrepareNewTurn();
		}
		LastTurnLiving = m_currentLiving;
		if (base.RoomType == eRoomType.FightFootballTime)
		{
			CreateNpc();
			m_currentLiving = FindNextTurnedFightFootball();
		}
		else
		{
			m_currentLiving = FindNextTurnedLiving();
		}
		if (m_currentLiving.VaneOpen)
		{
			UpdateWind(GetNextWind(), sendToClient: false);
		}
		MinusDelays(m_currentLiving.Delay);
		m_currentLiving.PrepareSelfTurn();
		if (!base.CurrentLiving.IsFrost && m_currentLiving.IsLiving)
		{
			m_currentLiving.StartAttacking();
			SendGameNextTurn(m_currentLiving, this, newBoxes);
			if (m_currentLiving.IsAttacking)
			{
				AddAction(new WaitLivingAttackingAction(m_currentLiving, base.m_turnIndex, (getTurnTime() + 20) * 1000));
			}
		}
		OnBeginNewTurn();
	}

	public override bool TakeCard(Player player)
	{
		int index = 0;
		for (int i = 0; i < Cards.Length; i++)
		{
			if (Cards[i] == 0)
			{
				index = i;
				break;
			}
		}
		return TakeCard(player, index);
	}

	public override bool TakeCard(Player player, int index)
	{
		if (player.CanTakeOut == 0 || !player.IsActive || index < 0 || index > Cards.Length || player.FinishTakeCard || Cards[index] > 0)
		{
			return false;
		}
		player.CanTakeOut--;
		int gold = 0;
		int money = 0;
		int giftToken = 0;
		int medal = 0;
		int honor = 0;
		int hardCurrency = 0;
		int token = 0;
		int dragonToken = 0;
		int num = 0;
		int count = 0;
		List<ItemInfo> info = null;
		if (DropInventory.CardDrop(base.RoomType, ref info) && info != null)
		{
			foreach (ItemInfo item in info)
			{
				num = item.TemplateID;
				count = item.Count;
				ShopMgr.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken);
				if (num > 0)
				{
					player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.NoneTypeView);
				}
			}
		}
		player.FinishTakeCard = true;
		Cards[index] = 1;
		player.PlayerDetail.AddGold(gold);
		player.PlayerDetail.AddMoney(money);
		player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
		player.PlayerDetail.AddGiftToken(giftToken);
		SendGamePlayerTakeCard(player, index, num, count);
		return true;
	}

	private int CalculateExperience(Player player, int winTeam, ref int reward)
	{
		int num = 1;
		if (m_roomType == eRoomType.Match || base.RoomType == eRoomType.BattleRoom)
		{
			float num2 = ((player.Team == 1) ? m_blueAvgLevel : m_redAvgLevel);
			float num3 = ((player.Team == 1) ? m_blueTeam.Count : m_redTeam.Count);
			Math.Abs(num2 - (float)player.PlayerDetail.PlayerCharacter.Grade);
			if (player.TotalHurt == 0)
			{
				if (((double)(num2 - m_blueAvgLevel) < 5.0 && (double)(num2 - m_redAvgLevel) < 5.0) || TotalHurt <= 0)
				{
					return 1;
				}
				SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward"), null, 2);
				reward = 200;
				return 201;
			}
			float num4 = ((player.Team == winTeam) ? 2f : 0f);
			double num5 = GP_RATE / 10.0;
			player.TotalShootCount = ((player.TotalShootCount == 0) ? 1 : player.TotalShootCount);
			if (player.TotalShootCount < player.TotalHitTargetCount)
			{
				player.TotalShootCount = player.TotalHitTargetCount;
			}
			int num6 = ((player.Team == 1) ? ((int)((double)((float)m_blueTeam.Count * m_blueAvgLevel) * 300.0)) : ((int)((double)(m_redAvgLevel * (float)m_redTeam.Count) * 300.0)));
			int num7 = ((player.TotalHurt > num6) ? num6 : player.TotalHurt);
			int num8 = (int)Math.Ceiling(((double)num4 + (double)num7 * (0.019 + num5) + (double)player.TotalKill * 0.5 + (double)(player.TotalHitTargetCount / player.TotalShootCount * 2)) * (double)num2 * (0.9 + ((double)num3 - 1.0) * 0.3));
			if (((double)(num2 - m_blueAvgLevel) >= 5.0 || (double)(num2 - m_redAvgLevel) >= 5.0) && TotalHurt > 0)
			{
				SendMessage(player.PlayerDetail, LanguageMgr.GetTranslation("GetGPreward"), null, 2);
				reward = 200;
				num8 += 200;
			}
			num = GainCoupleGP(player, num8);
			if (num > 100000)
			{
				num = 100000;
			}
		}
		if (m_roomType == eRoomType.FightFootballTime)
		{
			int num9 = 0;
			foreach (int item in player.ScoreArr)
			{
				num9 += item;
			}
			num = num9 * 50;
		}
		if (num >= 1)
		{
			return num;
		}
		return 1;
	}

	public int GainCoupleGP(Player player, int gp)
	{
		Player[] sameTeamPlayer = GetSameTeamPlayer(player);
		foreach (Player player2 in sameTeamPlayer)
		{
			if (player2.PlayerDetail.PlayerCharacter.SpouseID == player.PlayerDetail.PlayerCharacter.SpouseID)
			{
				return (int)((double)gp * 1.2);
			}
		}
		return gp;
	}

	public Player[] GetSameTeamPlayer(Player player)
	{
		List<Player> list = new List<Player>();
		foreach (Player allFightPlayer in GetAllFightPlayers())
		{
			if (allFightPlayer != player && allFightPlayer.Team == player.Team)
			{
				list.Add(allFightPlayer);
			}
		}
		return list.ToArray();
	}

	public void GameOver()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		m_gameState = eGameState.GameOver;
		ClearWaitTimer();
		CurrentTurnTotalDamage = 0;
		List<Player> allFightPlayers = GetAllFightPlayers();
		int num = -1;
		foreach (Player item in allFightPlayers)
		{
			if (item.IsLiving)
			{
				num = item.Team;
				break;
			}
		}
		if (num == -1 && CurrentPlayer != null)
		{
			num = CurrentPlayer.Team;
		}
		int num2 = CalculateGuildMatchResult(allFightPlayers, num);
		if (base.RoomType == eRoomType.Match && base.GameType == eGameType.Guild)
		{
			int num3 = 10;
			int num4 = -10;
			num3 += allFightPlayers.Count / 2;
			num4 += (int)Math.Round((double)(allFightPlayers.Count / 2) * 0.5);
		}
		int num5 = 0;
		int num6 = 0;
		foreach (Player item2 in allFightPlayers)
		{
			if (item2.TotalHurt > 0)
			{
				if (item2.Team == 1)
				{
					num6 = 1;
				}
				else
				{
					num5 = 1;
				}
			}
		}
		int val = 0;
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(100);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(base.PlayerCount);
		foreach (Player item3 in allFightPlayers)
		{
			float num7 = ((item3.Team == 1) ? m_blueAvgLevel : m_redAvgLevel);
			if (item3.Team != 1)
			{
				int count = m_redTeam.Count;
			}
			else
			{
				int count2 = m_blueTeam.Count;
			}
			float num8 = Math.Abs(num7 - (float)item3.PlayerDetail.PlayerCharacter.Grade);
			int team = item3.Team;
			int num9 = 0;
			int reward = 0;
			if (item3.TotalShootCount != 0)
			{
				int totalShootCount = item3.TotalShootCount;
			}
			if (base.RoomType == eRoomType.BattleRoom || m_roomType == eRoomType.Match || num8 < 5f)
			{
				num9 = CalculateExperience(item3, num, ref reward);
			}
			num9 = ((num9 == 0) ? 1 : num9);
			string text = ". ";
			item3.CanTakeOut = ((item3.Team == 1) ? num6 : num5);
			num2 += item3.GainOffer;
			bool flag = ((base.RoomType != eRoomType.FightFootballTime) ? (item3.Team == num) : ((item3.Team == 1 && blueScore > redScore) || (item3.Team == 2 && blueScore < redScore)));
			if (base.RoomType == eRoomType.Match || base.RoomType == eRoomType.BattleRoom)
			{
				int num10 = base.Random.Next(MONEY_MIN_RATE_LOSE, MONEY_MAX_RATE_LOSE);
				int num11 = base.Random.Next(DDT_MONEY_MIN_RATE_LOSE, DDT_MONEY_MAX_RATE_LOSE);
				int num12 = LeagueMoney_Lose;
				int num13 = base.Random.Next(EXP_MIN_RATE_LOSE, EXP_MAX_RATE_LOSE);
				if (flag)
				{
					num12 = LeagueMoney_Win;
					num10 = base.Random.Next(MONEY_MIN_RATE_WIN, MONEY_MAX_RATE_WIN);
					num11 = base.Random.Next(DDT_MONEY_MIN_RATE_WIN, DDT_MONEY_MAX_RATE_WIN);
					num13 = base.Random.Next(EXP_MIN_RATE_WIN, EXP_MAX_RATE_WIN);
				}
				item3.PlayerDetail.AddMoney(num10);
				item3.PlayerDetail.AddActiveMoney(num10);
				string text2 = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg1", num10);
				if (DDT_MONEY_ACTIVE > 0)
				{
					item3.PlayerDetail.AddGiftToken(num10);
					text2 += $", {num11}  Xu khóa";
				}
				num9 += num13;
				if (item3.PlayerDetail.CanX2Exp)
				{
					num9 *= 2;
					text += "Bạn nhận x2 exp từ Event thuyền rông.";
				}
				if (item3.PlayerDetail.CanX3Exp)
				{
					num9 *= 3;
					text += "Bạn nhận x3 exp từ Event thuyền rông.";
				}
				item3.PlayerDetail.SendHideMessage(text2 + text);
				int restCount = item3.PlayerDetail.MatchInfo.restCount;
				int maxCount = item3.PlayerDetail.MatchInfo.maxCount;
				bool flag2 = base.PlayerCount == 4;
				bool flag3 = item3.PlayerDetail.PlayerCharacter.Grade >= 20;
				if (flag2 && flag3 && restCount > 0)
				{
					text2 = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg6", num12);
					if (!flag)
					{
						text2 = LanguageMgr.GetTranslation("PVPGame.SendGameOVer.Msg7", num12);
					}
					item3.PlayerDetail.SendMessage(text2);
					item3.PlayerDetail.AddLeagueMoney(num12);
					item3.PlayerDetail.UpdateRestCount();
				}
			}
			if (base.RoomType == eRoomType.BattleRoom)
			{
				item3.PlayerDetail.AddPrestige(flag);
			}
			if (item3.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
			{
				num9 += num9 * item3.FightBuffers.ConsortionAddPercentGoldOrGP / 100;
			}
			if (item3.FightBuffers.ConsortionAddOfferRate > 0)
			{
				num2 *= item3.FightBuffers.ConsortionAddOfferRate;
			}
			item3.PlayerDetail.FootballTakeOut(flag);
			item3.GainGP = item3.PlayerDetail.AddGP(num9);
			item3.GainOffer = item3.PlayerDetail.AddOffer(num2);
			gSPacketIn.WriteInt(item3.Id);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteInt(item3.Grade);
			gSPacketIn.WriteInt(item3.PlayerDetail.PlayerCharacter.GP);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(num9);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item3.GainGP);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item3.GainOffer);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item3.CanTakeOut);
		}
		gSPacketIn.WriteInt(num2);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		SendToAll(gSPacketIn);
		new StringBuilder();
		foreach (Player item4 in allFightPlayers)
		{
			item4.PlayerDetail.OnGameOver(this, item4.Team == num, item4.GainGP);
		}
		string playResult = "";
		OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, beginTime, DateTime.Now, BeginPlayerCount, base.Map.Info.ID, teamAStr, teamBStr, playResult, num, BossWarField);
		WaitTime(15000);
		OnGameOverred();
	}

	public override void Stop()
	{
		if (base.GameState != eGameState.GameOver)
		{
			return;
		}
		m_gameState = eGameState.Stopped;
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.IsActive && !item.FinishTakeCard && item.CanTakeOut > 0 && base.RoomType != eRoomType.FightFootballTime)
			{
				TakeCard(item);
			}
		}
		lock (m_players)
		{
			m_players.Clear();
		}
		base.Stop();
	}

	private int CalculateGuildMatchResult(List<Player> players, int winTeam)
	{
		if (base.RoomType == eRoomType.Match)
		{
			StringBuilder stringBuilder = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
			StringBuilder stringBuilder2 = new StringBuilder(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg5"));
			IGamePlayer gamePlayer = null;
			IGamePlayer gamePlayer2 = null;
			int num = 0;
			foreach (Player player in players)
			{
				if (player.Team == winTeam)
				{
					stringBuilder.Append($"[{player.PlayerDetail.PlayerCharacter.NickName}]");
					gamePlayer = player.PlayerDetail;
				}
				else
				{
					stringBuilder2.Append($"{player.PlayerDetail.PlayerCharacter.NickName}");
					gamePlayer2 = player.PlayerDetail;
					num++;
				}
			}
			if (gamePlayer2 != null)
			{
				stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg1") + gamePlayer2.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg2"));
				stringBuilder2.Append(LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg3") + gamePlayer.PlayerCharacter.ConsortiaName + LanguageMgr.GetTranslation("Game.Server.SceneGames.OnStopping.Msg4"));
				int num2 = 0;
				if (base.GameType == eGameType.Guild)
				{
					num2 = num + TotalHurt / 2000;
				}
				gamePlayer.ConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, TotalHurt, players.Count);
				if (gamePlayer.ServerID != gamePlayer2.ServerID)
				{
					gamePlayer2.ConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, gamePlayer2.PlayerCharacter.ConsortiaID, base.Players, base.RoomType, base.GameType, TotalHurt, players.Count);
				}
				if (base.GameType == eGameType.Guild)
				{
					gamePlayer.SendConsortiaFight(gamePlayer.PlayerCharacter.ConsortiaID, num2, stringBuilder.ToString());
				}
				return num2;
			}
		}
		return 0;
	}

	public bool CanGameOver()
	{
		if (base.RoomType == eRoomType.FightFootballTime && base.TurnIndex > 7)
		{
			return true;
		}
		bool flag = true;
		bool flag2 = true;
		foreach (Player item in m_redTeam)
		{
			if (item.IsLiving)
			{
				flag = false;
				break;
			}
		}
		foreach (Player item2 in m_blueTeam)
		{
			if (item2.IsLiving)
			{
				flag2 = false;
				break;
			}
		}
		return flag || flag2;
	}

	public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
	{
		Player player = base.RemovePlayer(gp, IsKick);
		if (player != null && player.IsLiving && base.GameState != eGameState.Loading)
		{
			gp.RemoveGP(gp.PlayerCharacter.Grade * 12);
			string msg = null;
			string msg2 = null;
			if (base.RoomType == eRoomType.Match)
			{
				if (base.GameType == eGameType.Guild)
				{
					msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 15);
					gp.RemoveOffer(15);
					msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 15);
				}
				else if (base.GameType == eGameType.Free)
				{
					msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", gp.PlayerCharacter.Grade * 12, 5);
					gp.RemoveOffer(5);
					msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12, 5);
				}
			}
			else
			{
				msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", gp.PlayerCharacter.Grade * 12);
				msg2 = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12);
			}
			SendMessage(gp, msg, msg2, 3);
			if (GetSameTeam())
			{
				base.CurrentLiving.StopAttacking();
				CheckState(0);
			}
		}
		return player;
	}

	public override void CheckState(int delay)
	{
		AddAction(new CheckPVPGameStateAction(delay));
	}
}
