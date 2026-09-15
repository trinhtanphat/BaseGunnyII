using System;
using System.Collections.Generic;
using System.Drawing;
using System.Reflection;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic.AI;
using Game.Logic.AI.Game;
using Game.Logic.AI.Mission;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class PVEGame : BaseGame
{
	private new static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private APVEGameControl m_gameAI;

	private AMissionControl m_missionAI;

	public int SessionId;

	public bool IsWin;

	public bool IsKillWorldBoss;

	public bool CanEnterGate;

	public bool CanShowBigBox;

	public int TotalMissionCount;

	public int TotalCount;

	public int TotalTurn;

	public int Param1;

	public int Param2;

	public int Param3;

	public int Param4;

	public string Pic;

	public int TotalKillCount;

	public double TotalNpcExperience;

	public double TotalNpcGrade;

	private int BeginPlayersCount;

	private PveInfo m_info;

	private List<string> m_gameOverResources;

	public Dictionary<int, MissionInfo> Misssions;

	private MapPoint mapPos;

	public int WantTryAgain;

	public long WorldbossBood;

	public long AllWorldDameBoss;

	private eHardLevel m_hardLevel;

	private DateTime beginTime;

	private string m_IsBossType;

	private MissionInfo m_missionInfo;

	private List<int> m_mapHistoryIds;

	public int[] BossCards;

	private int m_bossCardCount;

	private int m_pveGameDelay;

	public MissionInfo MissionInfo
	{
		get
		{
			return m_missionInfo;
		}
		set
		{
			m_missionInfo = value;
		}
	}

	public Player CurrentPlayer => m_currentLiving as Player;

	public TurnedLiving CurrentTurnLiving => m_currentLiving;

	public List<int> MapHistoryIds
	{
		get
		{
			return m_mapHistoryIds;
		}
		set
		{
			m_mapHistoryIds = value;
		}
	}

	public eHardLevel HandLevel => m_hardLevel;

	public MapPoint MapPos => mapPos;

	public string IsBossWar
	{
		get
		{
			return m_IsBossType;
		}
		set
		{
			m_IsBossType = value;
		}
	}

	public List<string> GameOverResources => m_gameOverResources;

	public int BossCardCount
	{
		get
		{
			return m_bossCardCount;
		}
		set
		{
			if (value > 0)
			{
				BossCards = new int[9];
				m_bossCardCount = value;
			}
		}
	}

	public int PveGameDelay
	{
		get
		{
			return m_pveGameDelay;
		}
		set
		{
			m_pveGameDelay = value;
		}
	}

	public PVEGame(int id, int roomId, PveInfo info, List<IGamePlayer> players, Map map, eRoomType roomType, eGameType gameType, int timeType, eHardLevel hardLevel, int currentFloor)
		: base(id, roomId, map, roomType, gameType, timeType)
	{
		foreach (IGamePlayer player in players)
		{
			AddPlayer(player, new Player(player, PhysicalId++, this, 1, player.PlayerCharacter.hp)
			{
				Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1))
			});
			WorldbossBood = player.WorldbossBood;
			AllWorldDameBoss = player.AllWorldDameBoss;
		}
		m_info = info;
		BeginPlayersCount = players.Count;
		TotalKillCount = 0;
		TotalNpcGrade = 0.0;
		TotalNpcExperience = 0.0;
		TotalHurt = 0;
		m_IsBossType = "";
		WantTryAgain = 0;
		if (currentFloor > 0)
		{
			SessionId = currentFloor - 1;
		}
		else
		{
			SessionId = 0;
		}
		m_gameOverResources = new List<string>();
		Misssions = new Dictionary<int, MissionInfo>();
		m_mapHistoryIds = new List<int>();
		m_hardLevel = hardLevel;
		string script = GetScript(info, hardLevel);
		m_gameAI = ScriptMgr.CreateInstance(script) as APVEGameControl;
		if (m_gameAI == null)
		{
			log.ErrorFormat("Can't create game ai :{0}", script);
			m_gameAI = SimplePVEGameControl.Simple;
		}
		m_gameAI.Game = this;
		m_gameAI.OnCreated();
		m_missionAI = SimpleMissionControl.Simple;
		beginTime = DateTime.Now;
		m_bossCardCount = 0;
	}

	private string GetScript(PveInfo pveInfo, eHardLevel hardLevel)
	{
		string empty = string.Empty;
		return hardLevel switch
		{
			eHardLevel.Simple => pveInfo.SimpleGameScript,
			eHardLevel.Normal => pveInfo.NormalGameScript,
			eHardLevel.Hard => pveInfo.HardGameScript,
			eHardLevel.Terror => pveInfo.TerrorGameScript,
			eHardLevel.Epic => pveInfo.EpicGameScript,
			_ => pveInfo.SimpleGameScript,
		};
	}

	public string GetMissionIdStr(string missionIds, int randomCount)
	{
		if (string.IsNullOrEmpty(missionIds))
		{
			return "";
		}
		string[] array = missionIds.Split(',');
		if (array.Length < randomCount)
		{
			return "";
		}
		List<string> list = new List<string>();
		int maxValue = array.Length;
		int num = 0;
		while (num < randomCount)
		{
			int num2 = base.Random.Next(maxValue);
			string item = array[num2];
			if (!list.Contains(item))
			{
				list.Add(item);
				num++;
			}
		}
		StringBuilder stringBuilder = new StringBuilder();
		foreach (string item2 in list)
		{
			stringBuilder.Append(item2).Append(",");
		}
		return stringBuilder.Remove(stringBuilder.Length - 1, 1).ToString();
	}

	public void SetupMissions(string missionIds)
	{
		if (!string.IsNullOrEmpty(missionIds))
		{
			int num = 0;
			string[] array = missionIds.Split(',');
			string[] array2 = array;
			foreach (string s in array2)
			{
				num++;
				MissionInfo missionInfo = MissionInfoMgr.GetMissionInfo(int.Parse(s));
				Misssions.Add(num, missionInfo);
			}
		}
	}

	public LivingConfig BaseLivingConfig()
	{
		LivingConfig livingConfig = new LivingConfig();
		livingConfig.isBotom = 1;
		livingConfig.IsTurn = true;
		livingConfig.isShowBlood = true;
		livingConfig.isShowSmallMapPoint = true;
		livingConfig.ReduceBloodStart = 1;
		return livingConfig;
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction)
	{
		return CreateNpc(npcId, x, y, type, direction, BaseLivingConfig());
	}

	public SimpleNpc CreateNpc(int npcId, int x, int y, int type, int direction, LivingConfig config)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleNpc simpleNpc = new SimpleNpc(PhysicalId++, this, npcInfoById, type, direction);
		if (config != null)
		{
			simpleNpc.Config = config;
		}
		if (simpleNpc.Config.ReduceBloodStart > 1)
		{
			simpleNpc.Blood = npcInfoById.Blood / simpleNpc.Config.ReduceBloodStart;
		}
		else
		{
			simpleNpc.Reset();
		}
		simpleNpc.SetXY(x, y);
		AddLiving(simpleNpc);
		simpleNpc.StartMoving();
		return simpleNpc;
	}

	public SimpleNpc CreateNpc(int npcId, int type, int direction)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleNpc simpleNpc = new SimpleNpc(PhysicalId++, this, npcInfoById, type, direction);
		Point playerPoint = GetPlayerPoint(mapPos, npcInfoById.Camp);
		simpleNpc.Reset();
		simpleNpc.SetXY(playerPoint);
		AddLiving(simpleNpc);
		simpleNpc.StartMoving();
		return simpleNpc;
	}

	public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action)
	{
		return CreateBoss(npcId, x, y, direction, type, action, BaseLivingConfig());
	}

	public SimpleBoss CreateBoss(int npcId, int x, int y, int direction, int type, string action, LivingConfig config)
	{
		NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(npcId);
		SimpleBoss simpleBoss = new SimpleBoss(PhysicalId++, this, npcInfoById, direction, type, action);
		if (config != null)
		{
			simpleBoss.Config = config;
		}
		if (simpleBoss.Config.ReduceBloodStart > 1)
		{
			simpleBoss.Blood = npcInfoById.Blood / simpleBoss.Config.ReduceBloodStart;
		}
		else
		{
			simpleBoss.Reset();
			if (simpleBoss.Config.IsWorldBoss && WorldbossBood < int.MaxValue)
			{
				simpleBoss.Blood = (int)WorldbossBood;
			}
			if (simpleBoss.Config.isConsortiaBoss)
			{
				simpleBoss.Blood -= (int)AllWorldDameBoss;
			}
		}
		simpleBoss.SetXY(x, y);
		AddLiving(simpleBoss);
		simpleBoss.StartMoving();
		return simpleBoss;
	}

	public Box CreateBox(int x, int y, string model, ItemInfo item)
	{
		Box box = new Box(PhysicalId++, model, item);
		box.SetXY(x, y);
		m_map.AddPhysical(box);
		AddBox(box, sendToClient: true);
		return box;
	}

	public Ball CreateBall(int x, int y, string action)
	{
		Ball ball = new Ball(PhysicalId++, action);
		ball.SetXY(x, y);
		m_map.AddPhysical(ball);
		AddBall(ball, sendToClient: true);
		return ball;
	}

	public void SendGameFocus(Physics p, int delay, int finishTime)
	{
		AddAction(new FocusAction(p, 1, delay, finishTime));
	}

	public PhysicalObj CreatePhysicalObj(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		PhysicalObj physicalObj = new PhysicalObj(PhysicalId++, name, model, defaultAction, scale, rotation);
		physicalObj.SetXY(x, y);
		AddPhysicalObj(physicalObj, sendToClient: true);
		return physicalObj;
	}

	public bool isDragonLair()
	{
		int key = 1 + SessionId;
		return Misssions.ContainsKey(key) && m_info.ID == 5;
	}

	public Layer Createlayer(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		Layer layer = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
		layer.SetXY(x, y);
		AddPhysicalObj(layer, sendToClient: true);
		return layer;
	}

	public Layer CreateTip(int x, int y, string name, string model, string defaultAction, int scale, int rotation)
	{
		Layer layer = new Layer(PhysicalId++, name, model, defaultAction, scale, rotation);
		layer.SetXY(x, y);
		AddPhysicalTip(layer, sendToClient: true);
		return layer;
	}

	public void CreateGate(bool isEnter)
	{
		CanEnterGate = isEnter;
	}

	public void ClearMissionData()
	{
		foreach (Living living in m_livings)
		{
			living.Dispose();
		}
		m_livings.Clear();
		List<TurnedLiving> list = new List<TurnedLiving>();
		foreach (TurnedLiving item in base.TurnQueue)
		{
			if (item is Player)
			{
				if (item.IsLiving)
				{
					list.Add(item);
				}
			}
			else
			{
				item.Dispose();
			}
		}
		base.TurnQueue.Clear();
		foreach (TurnedLiving item2 in list)
		{
			base.TurnQueue.Add(item2);
		}
		if (m_map == null)
		{
			return;
		}
		foreach (PhysicalObj item3 in m_map.GetAllPhysicalObjSafe())
		{
			item3.Dispose();
		}
	}

	public void AddAllPlayerToTurn()
	{
		foreach (Player value in base.Players.Values)
		{
			base.TurnQueue.Add(value);
		}
	}

	public override void AddLiving(Living living)
	{
		base.AddLiving(living);
		living.Died += living_Died;
	}

	private void living_Died(Living living)
	{
		if (base.CurrentLiving != null && base.CurrentLiving is Player && !(living is Player) && living != base.CurrentLiving)
		{
			TotalKillCount++;
			TotalNpcExperience += living.Experience;
			TotalNpcGrade += living.Grade;
		}
	}

	public override void MissionStart(IGamePlayer host)
	{
		if (base.GameState != eGameState.SessionPrepared && base.GameState != eGameState.GameOver)
		{
			return;
		}
		foreach (Player value in base.Players.Values)
		{
			value.Ready = true;
		}
		CheckState(0);
	}

	public override bool CanAddPlayer()
	{
		lock (m_players)
		{
			return base.GameState == eGameState.SessionPrepared && m_players.Count < 4;
		}
	}

	public override Player AddPlayer(IGamePlayer gp)
	{
		if (CanAddPlayer())
		{
			Player player = new Player(gp, PhysicalId++, this, 1, gp.PlayerCharacter.hp);
			player.Direction = ((m_random.Next(0, 1) == 0) ? 1 : (-1));
			AddPlayer(gp, player);
			SendCreateGameToSingle(this, gp);
			SendPlayerInfoInGame(this, gp, player);
			return player;
		}
		return null;
	}

	public override Player RemovePlayer(IGamePlayer gp, bool isKick)
	{
		Player player = GetPlayer(gp);
		if (player != null)
		{
			player.PlayerDetail.RemoveGP(gp.PlayerCharacter.Grade * 12);
			player.PlayerDetail.ClearFightBuffOneMatch();
			string msg = null;
			if (player.IsLiving && base.GameState == eGameState.Playing)
			{
				msg = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg4", gp.PlayerCharacter.Grade * 12);
				string translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg5", gp.PlayerCharacter.NickName, gp.PlayerCharacter.Grade * 12);
				SendMessage(gp, msg, translation, 3);
			}
			else
			{
				string translation = LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg1", gp.PlayerCharacter.NickName);
				SendMessage(gp, msg, translation, 3);
			}
			base.RemovePlayer(gp, isKick);
		}
		return player;
	}

	public void LoadResources(int[] npcIds)
	{
		if (npcIds == null || npcIds.Length == 0)
		{
			return;
		}
		foreach (int id in npcIds)
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
	}

	public void LoadNpcGameOverResources(int[] npcIds)
	{
		if (npcIds == null || npcIds.Length == 0)
		{
			return;
		}
		foreach (int id in npcIds)
		{
			NpcInfo npcInfoById = NPCInfoMgr.GetNpcInfoById(id);
			if (npcInfoById == null)
			{
				log.Error("LoadGameOverResources npcInfo resoure is not exits");
			}
			else
			{
				m_gameOverResources.Add(npcInfoById.ModelID);
			}
		}
	}

	public void Prepare()
	{
		if (base.GameState == eGameState.Inited)
		{
			m_gameState = eGameState.Prepared;
			SendCreateGame();
			CheckState(0);
			try
			{
				m_gameAI.OnPrepated();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
		}
	}

	public void PrepareNewSession()
	{
		if (base.GameState != eGameState.Prepared && base.GameState != eGameState.GameOver && base.GameState != eGameState.ALLSessionStopped)
		{
			return;
		}
		m_gameState = eGameState.SessionPrepared;
		SessionId++;
		ClearLoadingFiles();
		ClearMissionData();
		m_gameOverResources.Clear();
		WantTryAgain = 0;
		m_missionInfo = Misssions[SessionId];
		m_pveGameDelay = m_missionInfo.Delay;
		TotalCount = m_missionInfo.TotalCount;
		TotalTurn = m_missionInfo.TotalTurn;
		Param1 = m_missionInfo.Param1;
		Param2 = m_missionInfo.Param2;
		Param3 = -1;
		Param4 = -1;
		Pic = $"show{SessionId}.jpg";
		m_missionAI = ScriptMgr.CreateInstance(m_missionInfo.Script) as AMissionControl;
		if (m_missionAI == null)
		{
			log.ErrorFormat("Can't create game mission ai :{0}", m_missionInfo.Script);
			m_missionAI = SimpleMissionControl.Simple;
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdateBarrier(SessionId, Pic);
		}
		m_missionAI.Game = this;
		try
		{
			m_missionAI.OnPrepareNewSession();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
	}

	public bool CanStartNewSession()
	{
		return base.m_turnIndex == 0 || IsAllReady();
	}

	public bool IsAllReady()
	{
		foreach (Player value in base.Players.Values)
		{
			if (!value.Ready)
			{
				return false;
			}
		}
		return true;
	}

	public void StartLoading()
	{
		if (base.GameState == eGameState.SessionPrepared)
		{
			m_gameState = eGameState.Loading;
			base.m_turnIndex = 0;
			SendMissionInfo();
			SendStartLoading(60);
			VaneLoading();
			AddAction(new WaitPlayerLoadingAction(this, 61000));
		}
	}

	public void StartGameMovie()
	{
		if (base.GameState == eGameState.Loading)
		{
			try
			{
				m_missionAI.OnStartMovie();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
		}
	}

	public void StartGame()
	{
		if (base.GameState != eGameState.Loading)
		{
			return;
		}
		m_gameState = eGameState.GameStart;
		SendSyncLifeTime();
		TotalKillCount = 0;
		TotalNpcGrade = 0.0;
		TotalNpcExperience = 0.0;
		TotalHurt = 0;
		m_bossCardCount = 0;
		BossCards = null;
		List<Player> allFightPlayers = GetAllFightPlayers();
		mapPos = MapMgr.GetPVEMapRandomPos(m_map.Info.ID);
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(99);
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			if (!item.IsLiving)
			{
				AddLiving(item);
			}
			item.Reset();
			Point playerPoint = GetPlayerPoint(mapPos, item.Team);
			item.SetXY(playerPoint);
			m_map.AddPhysical(item);
			item.StartMoving();
			item.StartGame();
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.X);
			gSPacketIn.WriteInt(item.Y);
			if (playerPoint.X < 600)
			{
				item.Direction = 1;
			}
			else
			{
				item.Direction = -1;
			}
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
		SendUpdateUiData();
		WaitTime(base.PlayerCount * 2500 + 1000);
		OnGameStarted();
	}

	public void PrepareNewGame()
	{
		if (base.GameState == eGameState.GameStart)
		{
			m_gameState = eGameState.Playing;
			WaitTime(base.PlayerCount * 1000);
			try
			{
				m_missionAI.OnStartGame();
			}
			catch (Exception arg)
			{
				log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
			}
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
		LivingRandSay();
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item in allPhysicalSafe)
		{
			item.PrepareNewTurn();
		}
		List<Box> newBoxes = CreateBox();
		try
		{
			m_missionAI.OnNewTurnStarted();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		LastTurnLiving = m_currentLiving;
		m_currentLiving = FindNextTurnedLiving();
		if (m_currentLiving != null)
		{
			base.m_turnIndex++;
			SendUpdateUiData();
			List<Living> livedLivingsHadTurn = GetLivedLivingsHadTurn();
			if (livedLivingsHadTurn.Count > 0 && m_currentLiving.Delay >= m_pveGameDelay)
			{
				MinusDelays(m_pveGameDelay);
				foreach (Living living in m_livings)
				{
					living.PrepareSelfTurn();
					if (!living.IsFrost)
					{
						living.StartAttacking();
					}
				}
				SendGameNextTurn(livedLivingsHadTurn[0], this, newBoxes);
				foreach (Living living2 in m_livings)
				{
					if (living2.IsAttacking)
					{
						living2.StopAttacking();
					}
				}
				m_pveGameDelay += MissionInfo.IncrementDelay;
				CheckState(0);
			}
			else if (base.RoomType == eRoomType.ActivityDungeon && m_currentLiving is Player)
			{
				TurnedLiving[] nextAllTurnedLiving = GetNextAllTurnedLiving();
				UpdateWind(GetNextWind(), sendToClient: false);
				CurrentTurnTotalDamage = 0;
				TurnedLiving[] array = nextAllTurnedLiving;
				foreach (TurnedLiving turnedLiving in array)
				{
					MinusDelays(turnedLiving.Delay);
					turnedLiving.PrepareSelfTurn();
					turnedLiving.StartAttacking();
					SendSyncLifeTime();
					SendSigleNextTurn(turnedLiving, this, newBoxes);
					SendFightStatus(turnedLiving, 1);
					AddAction(new WaitLivingAttackingAction(turnedLiving, base.m_turnIndex, (m_timeType + 20) * 1000));
				}
			}
			else
			{
				if (CanShowBigBox)
				{
					ShowBigBox();
					CanEnterGate = true;
				}
				MinusDelays(m_currentLiving.Delay);
				if (m_currentLiving is Player)
				{
					UpdateWind(GetNextWind(), sendToClient: false);
				}
				CurrentTurnTotalDamage = 0;
				m_currentLiving.PrepareSelfTurn();
				if (m_currentLiving.IsLiving && !m_currentLiving.IsFrost)
				{
					m_currentLiving.StartAttacking();
					SendSyncLifeTime();
					SendGameNextTurn(m_currentLiving, this, newBoxes);
					if (m_currentLiving.IsAttacking)
					{
						AddAction(new WaitLivingAttackingAction(m_currentLiving, base.m_turnIndex, (getTurnTime() + 20) * 1000));
					}
				}
			}
		}
		OnBeginNewTurn();
		try
		{
			m_missionAI.OnBeginNewTurn();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
		}
	}

	public void LivingRandSay()
	{
		if (m_livings == null || m_livings.Count == 0)
		{
			return;
		}
		int count = m_livings.Count;
		foreach (Living living in m_livings)
		{
			living.IsSay = false;
		}
		if (base.TurnIndex % 2 == 0)
		{
			return;
		}
		int num = ((count <= 5) ? base.Random.Next(0, 2) : ((count <= 5 || count > 10) ? base.Random.Next(1, 4) : base.Random.Next(1, 3)));
		if (num <= 0)
		{
			return;
		}
		int num2 = 0;
		while (num2 < num)
		{
			int index = base.Random.Next(0, count);
			if (!m_livings[index].IsSay)
			{
				m_livings[index].IsSay = true;
				num2++;
			}
		}
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
		if (player.CanTakeOut == 0)
		{
			return false;
		}
		if (!player.IsActive || index < 0 || index > Cards.Length || player.FinishTakeCard || Cards[index] > 0)
		{
			return false;
		}
		int gold = 0;
		int money = 0;
		int giftToken = 0;
		int medal = 0;
		int honor = 0;
		int hardCurrency = 0;
		int token = 0;
		int dragonToken = 0;
		int templateID = 0;
		int count = 0;
		List<ItemInfo> info = null;
		if (DropInventory.CopyDrop(m_missionInfo.Id, 1, ref info) && info != null)
		{
			foreach (ItemInfo item in info)
			{
				ShopMgr.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken);
				if (item != null)
				{
					templateID = item.TemplateID;
					count = item.Count;
					player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
				}
			}
			player.PlayerDetail.AddGold(gold);
			player.PlayerDetail.AddMoney(money);
			player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_TakeCard, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
			player.PlayerDetail.AddGiftToken(giftToken);
		}
		if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon)
		{
			player.CanTakeOut--;
			if (player.CanTakeOut == 0)
			{
				player.FinishTakeCard = true;
			}
		}
		else
		{
			player.FinishTakeCard = true;
		}
		Cards[index] = 1;
		SendGamePlayerTakeCard(player, index, templateID, count);
		return true;
	}

	public bool CanGameOver()
	{
		if (base.PlayerCount == 0)
		{
			return true;
		}
		if (GetDiedPlayerCount() == base.PlayerCount)
		{
			IsWin = false;
			return true;
		}
		try
		{
			return m_missionAI.CanGameOver();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		return true;
	}

	public void TakeSnow()
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(201144);
		if (itemTemplateInfo == null)
		{
			return;
		}
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 101);
		itemInfo.IsBinds = true;
		int num = base.Random.Next(3, 9);
		string text = "";
		foreach (Player allFightPlayer in GetAllFightPlayers())
		{
			allFightPlayer.PlayerDetail.AddTemplate(itemInfo, itemInfo.Template.BagType, num, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
			text += $"Bạn nhận được {itemInfo.Template.Name} x{num}, trong lần tấn công này.";
			allFightPlayer.PlayerDetail.SendMessage(text);
		}
	}

	public void TakeConsortiaBossAward(bool isWin)
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.UpdatePveResult("consortiaboss", item.TotalDameLiving, isWin);
		}
	}

	public void ShowBigBox()
	{
		List<ItemInfo> info = null;
		DropInventory.CopyDrop(m_missionInfo.Id, SessionId, ref info);
		List<int> list = new List<int>();
		if (info != null)
		{
			foreach (ItemInfo item in info)
			{
				list.Add(item.TemplateID);
			}
		}
		foreach (Player allFightPlayer in GetAllFightPlayers())
		{
			if (CanGetLabyrinthAward(allFightPlayer.PlayerDetail.ProcessLabyrinthAward))
			{
				SendGameBigBox(allFightPlayer, list);
				allFightPlayer.PlayerDetail.UpdateLabyrinth(SessionId, m_missionInfo.Id, bigAward: true);
			}
		}
	}

	private bool CanGetLabyrinthAward(string param2)
	{
		bool result = false;
		if (param2.Length > 0)
		{
			string[] array = param2.Split('-');
			string[] array2 = array;
			foreach (string text in array2)
			{
				string text2 = text;
				if (text2 == SessionId.ToString())
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public void GameOverMovie()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		m_gameState = eGameState.GameOver;
		ClearWaitTimer();
		ClearDiedPhysicals();
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (CanGetLabyrinthAward(item.PlayerDetail.ProcessLabyrinthAward))
			{
				item.PlayerDetail.UpdateLabyrinth(SessionId, m_missionInfo.Id, bigAward: false);
			}
		}
		try
		{
			m_missionAI.OnGameOverMovie();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		bool flag = HasNextSession();
		if (!flag)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(112);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(base.PlayerCount);
			foreach (Player item2 in allFightPlayers)
			{
				item2.PlayerDetail.ClearFightBuffOneMatch();
				item2.PlayerDetail.OutLabyrinth(IsWin);
				int gp = CalculateExperience(item2);
				int num = CalculateScore(item2);
				m_missionAI.CalculateScoreGrade(item2.TotalAllScore);
				if (item2.CurrentIsHitTarget)
				{
					item2.TotalHitTargetCount++;
				}
				CalculateHitRate(item2.TotalHitTargetCount, item2.TotalShootCount);
				item2.TotalAllHurt += item2.TotalHurt;
				item2.TotalAllCure += item2.TotalCure;
				item2.TotalAllHitTargetCount += item2.TotalHitTargetCount;
				item2.TotalAllShootCount += item2.TotalShootCount;
				item2.GainGP = item2.PlayerDetail.AddGP(gp);
				item2.TotalAllExperience += item2.GainGP;
				item2.TotalAllScore += num;
				item2.BossCardCount = BossCardCount;
				gSPacketIn.WriteInt(item2.PlayerDetail.PlayerCharacter.ID);
				gSPacketIn.WriteInt(item2.PlayerDetail.PlayerCharacter.Grade);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(item2.GainGP);
				gSPacketIn.WriteBoolean(IsWin);
				gSPacketIn.WriteInt(item2.BossCardCount);
				gSPacketIn.WriteBoolean(val: false);
				gSPacketIn.WriteBoolean(val: false);
			}
			if (BossCardCount > 0)
			{
				gSPacketIn.WriteInt(m_gameOverResources.Count);
				foreach (string gameOverResource in m_gameOverResources)
				{
					gSPacketIn.WriteString(gameOverResource);
				}
			}
			SendToAll(gSPacketIn);
			OnGameStopped();
			OnGameOverred();
			return;
		}
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item3 in allPhysicalSafe)
		{
			item3.PrepareNewTurn();
		}
		m_currentLiving = FindNextTurnedLiving();
		if (m_currentLiving != null && CanEnterGate)
		{
			base.m_turnIndex++;
			m_currentLiving.PrepareSelfTurn();
			List<Box> newBoxes = new List<Box>();
			SendGameNextTurn(m_currentLiving, this, newBoxes);
			CanEnterGate = false;
			CanShowBigBox = false;
			EnterNextFloor();
		}
		OnBeginNewTurn();
		try
		{
			m_missionAI.OnBeginNewTurn();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg2);
		}
	}

	public void EnterNextFloor()
	{
		int foregroundWidth = base.Map.Info.ForegroundWidth;
		Player player = FindRandomPlayer();
		int num = 150;
		int x = player.X;
		int y = player.Y;
		x = ((x + num <= foregroundWidth) ? (x + num) : (x - num));
		Point point = m_map.FindYLineNotEmptyPoint(x, y);
		if (point == Point.Empty)
		{
			point = new Point(x, base.Map.Bound.Height + 1);
		}
		CreatePhysicalObj(point.X, point.Y - 75, "transmitted", "asset.game.transmitted", "out", 1, 1);
	}

	public bool IsLabyrinth()
	{
		return base.RoomType == eRoomType.Lanbyrinth;
	}

	public void GameOver()
	{
		if (base.GameState != eGameState.Playing)
		{
			return;
		}
		m_gameState = eGameState.GameOver;
		SendUpdateUiData();
		try
		{
			m_missionAI.OnGameOver();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		CurrentTurnTotalDamage = 0;
		m_bossCardCount = 1;
		bool flag = HasNextSession();
		if (!IsWin || !flag)
		{
			m_bossCardCount = 0;
		}
		if (IsWin && !flag && !isTrainer())
		{
			m_bossCardCount = 2;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(112);
		gSPacketIn.WriteInt(BossCardCount);
		if (flag || isDragonLair())
		{
			gSPacketIn.WriteBoolean(val: true);
			gSPacketIn.WriteString($"show{1 + SessionId}.jpg");
			gSPacketIn.WriteBoolean(val: true);
		}
		else
		{
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteBoolean(val: false);
		}
		gSPacketIn.WriteInt(base.PlayerCount);
		foreach (Player item in allFightPlayers)
		{
			item.PlayerDetail.ClearFightBuffOneMatch();
			if (!IsWin)
			{
				item.PlayerDetail.OutLabyrinth(IsWin);
			}
			int num = CalculateExperience(item);
			if (item.FightBuffers.ConsortionAddPercentGoldOrGP > 0)
			{
				num += num * item.FightBuffers.ConsortionAddPercentGoldOrGP / 100;
			}
			int num2 = CalculateScore(item);
			m_missionAI.CalculateScoreGrade(item.TotalAllScore);
			item.CanTakeOut = BossCardCount;
			if (item.CurrentIsHitTarget)
			{
				item.TotalHitTargetCount++;
			}
			CalculateHitRate(item.TotalHitTargetCount, item.TotalShootCount);
			item.TotalAllHurt += item.TotalHurt;
			item.TotalAllCure += item.TotalCure;
			item.TotalAllHitTargetCount += item.TotalHitTargetCount;
			item.TotalAllShootCount += item.TotalShootCount;
			item.GainGP = item.PlayerDetail.AddGP(num);
			item.TotalAllExperience += item.GainGP;
			item.TotalAllScore += num2;
			item.BossCardCount = m_bossCardCount;
			gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.GainGP);
			gSPacketIn.WriteBoolean(IsWin);
			gSPacketIn.WriteInt(item.BossCardCount);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteBoolean(val: false);
		}
		if (BossCardCount > 0)
		{
			gSPacketIn.WriteInt(m_gameOverResources.Count);
			foreach (string gameOverResource in m_gameOverResources)
			{
				gSPacketIn.WriteString(gameOverResource);
			}
		}
		SendToAll(gSPacketIn);
		StringBuilder stringBuilder = new StringBuilder();
		foreach (Player item2 in allFightPlayers)
		{
			stringBuilder.Append(item2.PlayerDetail.PlayerCharacter.ID).Append(",");
			item2.Ready = false;
			item2.PlayerDetail.OnMissionOver(item2.Game, IsWin, MissionInfo.Id, item2.TurnNum);
		}
		int winTeam = (IsWin ? 1 : 2);
		string teamA = stringBuilder.ToString();
		string teamB = "";
		string playResult = "";
		if (!IsWin)
		{
			OnGameStopped();
		}
		StringBuilder stringBuilder2 = new StringBuilder();
		if (IsWin && IsBossWar != "")
		{
			stringBuilder2.Append(IsBossWar).Append(",");
			foreach (Player item3 in allFightPlayers)
			{
				stringBuilder2.Append("PlayerCharacter ID: ").Append(item3.PlayerDetail.PlayerCharacter.ID).Append(",");
				stringBuilder2.Append("Grade: ").Append(item3.PlayerDetail.PlayerCharacter.Grade).Append(",");
				stringBuilder2.Append("TurnNum): ").Append(item3.TurnNum).Append(",");
				stringBuilder2.Append("Attack: ").Append(item3.PlayerDetail.PlayerCharacter.Attack).Append(",");
				stringBuilder2.Append("Defence: ").Append(item3.PlayerDetail.PlayerCharacter.Defence).Append(",");
				stringBuilder2.Append("Agility: ").Append(item3.PlayerDetail.PlayerCharacter.Agility).Append(",");
				stringBuilder2.Append("Luck: ").Append(item3.PlayerDetail.PlayerCharacter.Luck).Append(",");
				stringBuilder2.Append("BaseAttack: ").Append(item3.PlayerDetail.GetBaseAttack()).Append(",");
				stringBuilder2.Append("MaxBlood: ").Append(item3.MaxBlood).Append(",");
				stringBuilder2.Append("BaseDefence: ").Append(item3.PlayerDetail.GetBaseDefence()).Append(",");
				if (item3.PlayerDetail.SecondWeapon != null)
				{
					stringBuilder2.Append("SecondWeapon TemplateID: ").Append(item3.PlayerDetail.SecondWeapon.TemplateID).Append(",");
					stringBuilder2.Append("SecondWeapon StrengthenLevel: ").Append(item3.PlayerDetail.SecondWeapon.StrengthenLevel).Append(".");
				}
			}
		}
		BossWarField = stringBuilder2.ToString();
		OnGameOverLog(base.RoomId, base.RoomType, base.GameType, 0, beginTime, DateTime.Now, BeginPlayersCount, MissionInfo.Id, teamA, teamB, playResult, winTeam, BossWarField);
		OnGameOverred();
	}

	public bool HasNextSession()
	{
		if (base.RoomType == eRoomType.ConsortiaBoss || isDragonLair())
		{
			return false;
		}
		if (base.PlayerCount == 0 || !IsWin)
		{
			return false;
		}
		int key = 1 + SessionId;
		return Misssions.ContainsKey(key);
	}

	public void GameOverAllSession()
	{
		if (base.GameState != eGameState.GameOver)
		{
			return;
		}
		m_gameState = eGameState.ALLSessionStopped;
		try
		{
			m_gameAI.OnGameOverAllSession();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", base.GameState, arg);
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(115);
		int canTakeOut = 1;
		if (!IsWin)
		{
			canTakeOut = 0;
		}
		else if ((base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon) && !isTrainer())
		{
			canTakeOut = 2;
		}
		gSPacketIn.WriteInt(base.PlayerCount);
		foreach (Player item in allFightPlayers)
		{
			item.CanTakeOut = canTakeOut;
			item.PlayerDetail.OnGameOver(this, IsWin, item.GainGP);
			gSPacketIn.WriteInt(item.PlayerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.GainGP);
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
			gSPacketIn.WriteInt(item.TotalAllExperience);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(IsWin);
		}
		gSPacketIn.WriteInt(m_gameOverResources.Count);
		foreach (string gameOverResource in m_gameOverResources)
		{
			gSPacketIn.WriteString(gameOverResource);
		}
		SendToAll(gSPacketIn);
		if (isDragonLair())
		{
			WaitTime(19000);
		}
		else
		{
			WaitTime(25000);
		}
		CanStopGame();
	}

	public void CanStopGame()
	{
		if (!IsWin)
		{
			if (base.GameType == eGameType.Dungeon)
			{
				ClearWaitTimer();
			}
			return;
		}
		int key = 1 + SessionId;
		if (Misssions.ContainsKey(key) && isDragonLair())
		{
			WantTryAgain = 1;
		}
	}

	public void ShowDragonLairCard()
	{
		if (base.GameState != eGameState.ALLSessionStopped || !IsWin)
		{
			return;
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.IsActive && item.CanTakeOut > 0)
			{
				item.HasPaymentTakeCard = true;
				int canTakeOut = item.CanTakeOut;
				for (int i = 0; i < canTakeOut; i++)
				{
					TakeCard(item);
				}
			}
		}
		SendShowCards();
	}

	public override void Stop()
	{
		if (base.GameState != eGameState.ALLSessionStopped)
		{
			return;
		}
		m_gameState = eGameState.Stopped;
		if (IsWin)
		{
			List<Player> allFightPlayers = GetAllFightPlayers();
			foreach (Player item in allFightPlayers)
			{
				if (item.IsActive && item.CanTakeOut > 0)
				{
					item.HasPaymentTakeCard = true;
					int canTakeOut = item.CanTakeOut;
					for (int i = 0; i < canTakeOut; i++)
					{
						TakeCard(item);
					}
				}
			}
			if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon)
			{
				SendShowCards();
			}
			if (base.RoomType == eRoomType.Dungeon)
			{
				foreach (Player item2 in allFightPlayers)
				{
					item2.PlayerDetail.SetPvePermission(m_info.ID, m_hardLevel);
				}
			}
		}
		lock (m_players)
		{
			m_players.Clear();
		}
		OnGameStopped();
	}

	protected override void Dispose(bool disposing)
	{
		base.Dispose(disposing);
		foreach (Living living in m_livings)
		{
			living.Dispose();
		}
		try
		{
			m_missionAI.Dispose();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_missionAI.Dispose() error:{1}", arg);
		}
		try
		{
			m_gameAI.Dispose();
		}
		catch (Exception arg2)
		{
			log.ErrorFormat("game ai script m_gameAI.Dispose() error:{1}", arg2);
		}
	}

	public void DoOther()
	{
		try
		{
			m_missionAI.DoOther();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.DoOther() error:{1}", arg);
		}
	}

	internal void OnShooted()
	{
		try
		{
			m_missionAI.OnShooted();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script m_gameAI.OnShooted() error:{1}", arg);
		}
	}

	private int CalculateExperience(Player p)
	{
		if (TotalKillCount == 0)
		{
			return 1;
		}
		double num = Math.Abs((double)p.Grade - TotalNpcGrade / (double)TotalKillCount);
		if (num >= 7.0)
		{
			return 1;
		}
		double num2 = 0.0;
		if (TotalKillCount > 0)
		{
			num2 += (double)p.TotalKill / (double)TotalKillCount * 0.4;
		}
		if (TotalHurt > 0)
		{
			num2 += (double)p.TotalHurt / (double)TotalHurt * 0.4;
		}
		if (p.IsLiving)
		{
			num2 += 0.4;
		}
		double num3 = 1.0;
		if (num >= 3.0 && num <= 4.0)
		{
			num3 = 0.7;
		}
		else if (num >= 5.0 && num <= 6.0)
		{
			num3 = 0.4;
		}
		double num4 = (0.9 + (double)(BeginPlayersCount - 1) * 0.4) / (double)base.PlayerCount;
		double num5 = TotalNpcExperience * num2 * num3 * num4;
		num5 = ((num5 == 0.0) ? 1.0 : num5);
		return (int)num5;
	}

	private int CalculateScore(Player p)
	{
		int num = (200 - base.TurnIndex) * 5 + p.TotalKill * 5 + (int)((double)p.Blood / (double)p.MaxBlood) * 10;
		if (!IsWin)
		{
			num -= 400;
		}
		return num;
	}

	private int CalculateHitRate(int hitTargetCount, int shootCount)
	{
		double num = 0.0;
		if (shootCount > 0)
		{
			num = (double)hitTargetCount / (double)shootCount;
		}
		return (int)(num * 100.0);
	}

	public override void CheckState(int delay)
	{
		AddAction(new CheckPVEGameStateAction(delay));
	}

	public bool TakeBossCard(Player player)
	{
		int index = 0;
		for (int i = 0; i < BossCards.Length; i++)
		{
			if (Cards[i] == 0)
			{
				index = i;
				break;
			}
		}
		return TakeCard(player, index);
	}

	public bool TakeBossCard(Player player, int index)
	{
		if (!player.IsActive || player.BossCardCount <= 0 || index < 0 || index > BossCards.Length || BossCards[index] > 0)
		{
			return false;
		}
		List<ItemInfo> info = null;
		int templateID = 0;
		int gold = 0;
		int money = 0;
		int giftToken = 0;
		int medal = 0;
		int count = 0;
		int honor = 0;
		int hardCurrency = 0;
		int token = 0;
		int dragonToken = 0;
		int missionId = int.Parse(IsBossWar);
		DropInventory.BossDrop(missionId, ref info);
		if (info != null)
		{
			foreach (ItemInfo item in info)
			{
				ShopMgr.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken);
				if (item != null)
				{
					player.PlayerDetail.AddTemplate(item, eBageType.TempBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
					templateID = item.TemplateID;
					count = item.Count;
				}
			}
			player.PlayerDetail.AddGold(gold);
			player.PlayerDetail.AddMoney(money);
			player.PlayerDetail.LogAddMoney(AddMoneyType.Award, AddMoneyType.Award_BossDrop, player.PlayerDetail.PlayerCharacter.ID, money, player.PlayerDetail.PlayerCharacter.Money);
			player.PlayerDetail.AddGiftToken(giftToken);
		}
		player.BossCardCount--;
		BossCards[index] = 1;
		SendGamePlayerTakeCard(player, index, templateID, count);
		return true;
	}

	public void SendMissionInfo()
	{
		if (m_missionInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91);
			gSPacketIn.WriteByte(113);
			gSPacketIn.WriteString(m_missionInfo.Name);
			gSPacketIn.WriteString(m_missionInfo.Success);
			gSPacketIn.WriteString(m_missionInfo.Failure);
			gSPacketIn.WriteString(m_missionInfo.Description);
			gSPacketIn.WriteString(m_missionInfo.Title);
			gSPacketIn.WriteInt(TotalMissionCount);
			gSPacketIn.WriteInt(SessionId);
			gSPacketIn.WriteInt(TotalTurn);
			gSPacketIn.WriteInt(TotalCount);
			gSPacketIn.WriteInt(Param1);
			gSPacketIn.WriteInt(Param2);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString(Pic);
			SendToAll(gSPacketIn);
		}
	}

	public void SendUpdateUiData()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(104);
		int val = 0;
		try
		{
			val = m_missionAI.UpdateUIData();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("game ai script {0} error:{1}", $"m_missionAI.UpdateUIData()", arg);
		}
		gSPacketIn.WriteInt(base.TurnIndex);
		gSPacketIn.WriteInt(val);
		gSPacketIn.WriteInt(Param3);
		gSPacketIn.WriteInt(Param4);
		SendToAll(gSPacketIn);
	}

	internal void SendShowCards()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(89);
		int num = 0;
		List<int> list = new List<int>();
		for (int i = 0; i < Cards.Length; i++)
		{
			if (Cards[i] == 0)
			{
				list.Add(i);
				num++;
			}
		}
		int val = 0;
		int val2 = 0;
		gSPacketIn.WriteInt(num);
		foreach (int item in list)
		{
			List<ItemInfo> list2 = DropInventory.CopySystemDrop(m_missionInfo.Id, list.Count);
			if (list2 != null)
			{
				foreach (ItemInfo item2 in list2)
				{
					val = item2.TemplateID;
					val2 = item2.Count;
				}
			}
			gSPacketIn.WriteByte((byte)item);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(val2);
		}
		SendToAll(gSPacketIn);
	}

	public void SendGameObjectFocus(int type, string name, int delay, int finishTime)
	{
		Physics[] array = FindPhysicalObjByName(name);
		Physics[] array2 = array;
		foreach (Physics obj in array2)
		{
			AddAction(new FocusAction(obj, type, delay, finishTime));
		}
	}

	private void SendCreateGameToSingle(PVEGame game, IGamePlayer gamePlayer)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(121);
		gSPacketIn.WriteInt(game.Map.Info.ID);
		gSPacketIn.WriteInt((byte)game.RoomType);
		gSPacketIn.WriteInt((byte)game.GameType);
		gSPacketIn.WriteInt(game.TimeType);
		List<Player> allFightPlayers = game.GetAllFightPlayers();
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			IGamePlayer playerDetail = item.PlayerDetail;
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.NickName);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteByte(playerDetail.PlayerCharacter.typeVIP);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.Sex);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Hide);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Style);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Colors);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Repute);
			if (playerDetail.MainWeapon == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(playerDetail.MainWeapon.TemplateID);
				gSPacketIn.WriteInt(playerDetail.MainWeapon.RefineryLevel);
				gSPacketIn.WriteString(playerDetail.MainWeapon.Name);
				gSPacketIn.WriteDateTime(DateTime.MinValue);
			}
			if (playerDetail.SecondWeapon == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(playerDetail.SecondWeapon.TemplateID);
			}
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.badgeID);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(item.Team);
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.MaxBlood);
			gSPacketIn.WriteBoolean(item.Ready);
		}
		int sessionId = game.SessionId;
		MissionInfo missionInfo = game.Misssions[sessionId];
		gSPacketIn.WriteString(missionInfo.Name);
		gSPacketIn.WriteString($"show{sessionId}.jpg");
		gSPacketIn.WriteString(missionInfo.Success);
		gSPacketIn.WriteString(missionInfo.Failure);
		gSPacketIn.WriteString(missionInfo.Description);
		gSPacketIn.WriteInt(game.TotalMissionCount);
		gSPacketIn.WriteInt(sessionId);
		gamePlayer.SendTCP(gSPacketIn);
	}

	public void SendPlayerInfoInGame(PVEGame game, IGamePlayer gp, Player p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter2 = base.LifeTime;
		gSPacketIn.WriteByte(120);
		gSPacketIn.WriteInt(4);
		gSPacketIn.WriteInt(gp.PlayerCharacter.ID);
		gSPacketIn.WriteInt(p.Team);
		gSPacketIn.WriteInt(p.Id);
		gSPacketIn.WriteInt(p.MaxBlood);
		gSPacketIn.WriteBoolean(p.Ready);
		game.SendToAll(gSPacketIn);
	}

	public void SendPlaySound(string playStr)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(63);
		gSPacketIn.WriteString(playStr);
		SendToAll(gSPacketIn);
	}

	public void SendLoadResource(List<LoadingFileInfo> loadingFileInfos)
	{
		if (loadingFileInfos == null || loadingFileInfos.Count <= 0)
		{
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(67);
		gSPacketIn.WriteInt(loadingFileInfos.Count);
		foreach (LoadingFileInfo loadingFileInfo in loadingFileInfos)
		{
			gSPacketIn.WriteInt(loadingFileInfo.Type);
			gSPacketIn.WriteString(loadingFileInfo.Path);
			gSPacketIn.WriteString(loadingFileInfo.ClassName);
		}
		SendToAll(gSPacketIn);
	}

	public override void MinusDelays(int lowestDelay)
	{
		m_pveGameDelay -= lowestDelay;
		base.MinusDelays(lowestDelay);
	}

	public void Print(string str)
	{
		Console.WriteLine(str);
	}
}
