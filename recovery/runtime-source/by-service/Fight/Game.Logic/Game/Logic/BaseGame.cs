using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class BaseGame : AbstractGame
{
	public delegate void GameOverLogEventHandle(int roomId, eRoomType roomType, eGameType fightType, int changeTeam, DateTime playBegin, DateTime playEnd, int userCount, int mapId, string teamA, string teamB, string playResult, int winTeam, string BossWar);

	public delegate void GameNpcDieEventHandle(int NpcId);

	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected int turnIndex;

	protected int m_nextPlayerId;

	protected int m_nextWind;

	protected eGameState m_gameState;

	protected Map m_map;

	protected Dictionary<int, Player> m_players;

	protected List<Living> m_livings;

	protected Random m_random;

	protected TurnedLiving m_currentLiving;

	public TurnedLiving LastTurnLiving;

	public int PhysicalId;

	public int CurrentTurnTotalDamage;

	public int TotalHurt;

	public int redScore;

	public int blueScore;

	public int ConsortiaAlly;

	public int RichesRate;

	public string BossWarField;

	private ArrayList m_actions;

	private List<TurnedLiving> m_turnQueue;

	private int m_roomId;

	public int[] Cards;

	private int m_lifeTime;

	private long m_waitTimer;

	private long m_passTick;

	public int CurrentActionCount;

	private List<Ball> m_tempBall;

	private List<Box> m_tempBox;

	private List<Point> m_tempPoints;

	private List<LoadingFileInfo> m_loadingFiles = new List<LoadingFileInfo>();

	private List<PetSkillElementInfo> GameNeedPetSkillInfo = new List<PetSkillElementInfo>();

	public int TotalCostMoney;

	public int TotalCostGold;

	protected int m_turnIndex
	{
		get
		{
			return turnIndex;
		}
		set
		{
			turnIndex = value;
		}
	}

	public int RoomId => m_roomId;

	public Dictionary<int, Player> Players => m_players;

	public int PlayerCount
	{
		get
		{
			lock (m_players)
			{
				return m_players.Count;
			}
		}
	}

	public int TurnIndex
	{
		get
		{
			return m_turnIndex;
		}
		set
		{
			m_turnIndex = value;
		}
	}

	public int nextPlayerId
	{
		get
		{
			return m_nextPlayerId;
		}
		set
		{
			m_nextPlayerId = value;
		}
	}

	public eGameState GameState => m_gameState;

	public float Wind => m_map.wind;

	public Map Map => m_map;

	public List<TurnedLiving> TurnQueue => m_turnQueue;

	public bool HasPlayer => m_players.Count > 0;

	public Random Random => m_random;

	public TurnedLiving CurrentLiving => m_currentLiving;

	public int LifeTime => m_lifeTime;

	public event GameEventHandle GameOverred;

	public event GameEventHandle BeginNewTurn;

	public event GameOverLogEventHandle GameOverLog;

	public event GameNpcDieEventHandle GameNpcDie;

	public BaseGame(int id, int roomId, Map map, eRoomType roomType, eGameType gameType, int timeType)
		: base(id, roomType, gameType, timeType)
	{
		m_roomId = roomId;
		m_players = new Dictionary<int, Player>();
		m_turnQueue = new List<TurnedLiving>();
		m_livings = new List<Living>();
		m_random = new Random();
		m_map = map;
		m_actions = new ArrayList();
		PhysicalId = 0;
		BossWarField = "";
		m_tempBox = new List<Box>();
		m_tempBall = new List<Ball>();
		m_tempPoints = new List<Point>();
		GameNeedPetSkillInfo = PetMgr.GameNeedPetSkill();
		if (base.RoomType == eRoomType.Dungeon || base.RoomType == eRoomType.SpecialActivityDungeon)
		{
			Cards = new int[21];
		}
		else
		{
			Cards = new int[9];
		}
		m_gameState = eGameState.Inited;
	}

	public void SetWind(int wind)
	{
		m_map.wind = wind;
	}

	public bool SetMap(int mapId)
	{
		if (GameState == eGameState.Playing)
		{
			return false;
		}
		Map map = MapMgr.CloneMap(mapId);
		if (map != null)
		{
			m_map = map;
			return true;
		}
		return false;
	}

	public int GetTurnWaitTime()
	{
		return m_timeType;
	}

	protected void AddPlayer(IGamePlayer gp, Player fp)
	{
		lock (m_players)
		{
			m_players.Add(fp.Id, fp);
			if (fp.Weapon != null)
			{
				m_turnQueue.Add(fp);
			}
		}
	}

	public void SelectObject(int id, int zoneId)
	{
		lock (m_players)
		{
		}
	}

	public virtual void AddLiving(Living living)
	{
		m_map.AddPhysical(living);
		if (living is Player)
		{
			Player player = living as Player;
			if (player.Weapon == null)
			{
				return;
			}
		}
		if (living is TurnedLiving)
		{
			m_turnQueue.Add(living as TurnedLiving);
		}
		else
		{
			m_livings.Add(living);
		}
		SendAddLiving(living);
	}

	public virtual void AddPhysicalObj(PhysicalObj phy, bool sendToClient)
	{
		m_map.AddPhysical(phy);
		phy.SetGame(this);
		if (sendToClient)
		{
			SendAddPhysicalObj(phy);
		}
	}

	public virtual void AddPhysicalTip(PhysicalObj phy, bool sendToClient)
	{
		m_map.AddPhysical(phy);
		phy.SetGame(this);
		if (sendToClient)
		{
			SendAddPhysicalTip(phy);
		}
	}

	public override Player RemovePlayer(IGamePlayer gp, bool IsKick)
	{
		Player player = null;
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.PlayerDetail == gp)
				{
					player = value;
					m_players.Remove(value.Id);
					break;
				}
			}
		}
		if (player != null)
		{
			AddAction(new RemovePlayerAction(player));
		}
		return player;
	}

	public void RemovePhysicalObj(PhysicalObj phy, bool sendToClient)
	{
		m_map.RemovePhysical(phy);
		phy.SetGame(null);
		if (sendToClient)
		{
			SendRemovePhysicalObj(phy);
		}
	}

	public void RemoveLiving(int id)
	{
		foreach (Living living in m_livings)
		{
			if (living.Id == id)
			{
				m_map.RemovePhysical(living);
				if (living is TurnedLiving)
				{
					m_turnQueue.Remove(living as TurnedLiving);
				}
				else
				{
					m_livings.Remove(living);
				}
			}
		}
		SendRemoveLiving(id);
	}

	public List<Living> GetLivedLivings()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public List<Living> GetLivedLivingsHadTurn()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living is SimpleNpc && living.Config.IsTurn)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public List<Living> GetBossLivings()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living is SimpleBoss)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public void ClearAllChild()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living.IsLiving && living is SimpleNpc)
			{
				list.Add(living);
			}
		}
		foreach (Living item in list)
		{
			m_livings.Remove(item);
			item.Dispose();
			RemoveLiving(item.Id);
		}
	}

	public List<Living> GetFightFootballLivings()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc)
			{
				list.Add(living);
			}
		}
		return list;
	}

	public void ClearAllNpc()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc)
			{
				list.Add(living);
			}
		}
		foreach (Living item in list)
		{
			m_livings.Remove(item);
			item.Dispose();
			SendRemoveLiving(item.Id);
		}
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item2 in allPhysicalSafe)
		{
			if (item2 is SimpleNpc)
			{
				m_map.RemovePhysical(item2);
			}
		}
	}

	public void ClearDiedPhysicals()
	{
		List<Living> list = new List<Living>();
		foreach (Living living in m_livings)
		{
			if (!living.IsLiving)
			{
				list.Add(living);
			}
		}
		foreach (Living item2 in list)
		{
			m_livings.Remove(item2);
			item2.Dispose();
		}
		List<Living> list2 = new List<Living>();
		foreach (TurnedLiving item3 in m_turnQueue)
		{
			if (!item3.IsLiving)
			{
				list2.Add(item3);
			}
		}
		foreach (TurnedLiving item4 in list2)
		{
			m_turnQueue.Remove(item4);
		}
		List<Physics> allPhysicalSafe = m_map.GetAllPhysicalSafe();
		foreach (Physics item5 in allPhysicalSafe)
		{
			if (!item5.IsLiving && !(item5 is Player))
			{
				m_map.RemovePhysical(item5);
			}
		}
	}

	public bool IsAllComplete()
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.LoadingProcess < 100)
			{
				return false;
			}
		}
		return true;
	}

	public Player FindPlayer(int id)
	{
		lock (m_players)
		{
			if (m_players.ContainsKey(id))
			{
				return m_players[id];
			}
		}
		return null;
	}

	public TurnedLiving FindNextTurnedFightFootball()
	{
		if (m_turnQueue.Count == 0)
		{
			return null;
		}
		int index = m_random.Next(m_turnQueue.Count - 1);
		TurnedLiving turnedLiving = m_turnQueue[index];
		if (TurnIndex > 0)
		{
			for (int i = 0; i < m_turnQueue.Count; i++)
			{
				if ((m_turnQueue[i] as Player).PlayerDetail.PlayerCharacter.ID == m_nextPlayerId)
				{
					turnedLiving = m_turnQueue[i];
					break;
				}
			}
		}
		turnedLiving.TurnNum++;
		for (int j = 0; j < m_turnQueue.Count; j++)
		{
			if (m_turnQueue[j].Team != turnedLiving.Team && m_turnQueue[j].TurnNum < turnedLiving.TurnNum)
			{
				m_nextPlayerId = (m_turnQueue[j] as Player).PlayerDetail.PlayerCharacter.ID;
				break;
			}
			index = m_random.Next(m_turnQueue.Count - 1);
			m_nextPlayerId = (m_turnQueue[index] as Player).PlayerDetail.PlayerCharacter.ID;
		}
		return turnedLiving;
	}

	public TurnedLiving FindNextTurnedLiving()
	{
		if (m_turnQueue.Count == 0)
		{
			return null;
		}
		int index = m_random.Next(m_turnQueue.Count - 1);
		TurnedLiving turnedLiving = m_turnQueue[index];
		int delay = turnedLiving.Delay;
		for (int i = 0; i < m_turnQueue.Count; i++)
		{
			if (m_turnQueue[i].Delay < delay && m_turnQueue[i].IsLiving)
			{
				delay = m_turnQueue[i].Delay;
				turnedLiving = m_turnQueue[i];
			}
		}
		turnedLiving.TurnNum++;
		return turnedLiving;
	}

	public TurnedLiving[] GetNextAllTurnedLiving()
	{
		if (m_turnQueue.Count == 0)
		{
			return null;
		}
		List<TurnedLiving> list = new List<TurnedLiving>();
		for (int i = 0; i < m_turnQueue.Count; i++)
		{
			if (m_turnQueue[i].IsLiving && !m_turnQueue[i].IsFrost && !m_turnQueue[i].IsAttacking && m_turnQueue[i] is Player)
			{
				m_turnQueue[i].TurnNum++;
				list.Add(m_turnQueue[i]);
			}
		}
		return list.ToArray();
	}

	public virtual void MinusDelays(int lowestDelay)
	{
		foreach (TurnedLiving item in m_turnQueue)
		{
			item.Delay -= lowestDelay;
		}
	}

	public SimpleBoss[] FindAllBoss()
	{
		List<SimpleBoss> list = new List<SimpleBoss>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleBoss)
			{
				list.Add(living as SimpleBoss);
			}
		}
		return list.ToArray();
	}

	public SimpleNpc[] FindAllNpc()
	{
		List<SimpleNpc> list = new List<SimpleNpc>();
		foreach (Living living in m_livings)
		{
			if (living is SimpleNpc)
			{
				list.Add(living as SimpleNpc);
				return list.ToArray();
			}
		}
		return null;
	}

	public float GetNextWind()
	{
		int num = (int)(Wind * 10f);
		int num2;
		if (num > m_nextWind)
		{
			num2 = num - m_random.Next(11);
			if (num <= m_nextWind)
			{
				m_nextWind = m_random.Next(-40, 40);
			}
		}
		else
		{
			num2 = num + m_random.Next(11);
			if (num >= m_nextWind)
			{
				m_nextWind = m_random.Next(-40, 40);
			}
		}
		return (float)num2 / 10f;
	}

	public void UpdateWind(float wind, bool sendToClient)
	{
		if (m_map.wind != wind)
		{
			m_map.wind = wind;
			if (sendToClient)
			{
				SendGameUpdateWind(wind);
			}
		}
	}

	public int GetDiedPlayerCount()
	{
		int num = 0;
		foreach (Player value in m_players.Values)
		{
			if (value.IsActive && !value.IsLiving)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDiedNPCCount()
	{
		int num = 0;
		SimpleNpc[] array = FindAllNpc();
		foreach (SimpleNpc simpleNpc in array)
		{
			if (!simpleNpc.IsLiving)
			{
				num++;
			}
		}
		return num;
	}

	public int GetDiedCount()
	{
		return GetDiedNPCCount() + GetDiedBossCount();
	}

	public int GetDiedBossCount()
	{
		int num = 0;
		SimpleBoss[] array = FindAllBoss();
		foreach (SimpleBoss simpleBoss in array)
		{
			if (!simpleBoss.IsLiving)
			{
				num++;
			}
		}
		return num;
	}

	protected Point GetPlayerPoint(MapPoint mapPos, int team)
	{
		List<Point> list = ((team == 1) ? mapPos.PosX : mapPos.PosX1);
		int index = m_random.Next(list.Count);
		Point point = list[index];
		list.Remove(point);
		return point;
	}

	public virtual void CheckState(int delay)
	{
	}

	public override void ProcessData(GSPacketIn packet)
	{
		if (m_players.ContainsKey(packet.Parameter1))
		{
			Player player = m_players[packet.Parameter1];
			AddAction(new ProcessPacketAction(player, packet));
		}
	}

	public Player GetPlayerByIndex(int index)
	{
		return m_players.ElementAt(index).Value;
	}

	public Player FindNearestPlayer(int x, int y)
	{
		double num = double.MaxValue;
		Player result = null;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				double num2 = value.Distance(x, y);
				if (num2 < num)
				{
					num = num2;
					result = value;
				}
			}
		}
		return result;
	}

	public Player FindRandomPlayer()
	{
		List<Player> list = new List<Player>();
		Player result = null;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				list.Add(value);
			}
		}
		int index = Random.Next(0, list.Count);
		if (list.Count > 0)
		{
			result = list[index];
		}
		return result;
	}

	public Living FindRandomLiving()
	{
		List<Living> list = new List<Living>();
		Living result = null;
		foreach (Living living in m_livings)
		{
			if (living.IsLiving)
			{
				list.Add(living);
			}
		}
		int index = Random.Next(0, list.Count);
		if (list.Count > 0)
		{
			result = list[index];
		}
		return result;
	}

	public int FindlivingbyDir(Living npc)
	{
		int num = 0;
		int num2 = 0;
		foreach (Player value in m_players.Values)
		{
			if (value.IsLiving)
			{
				if (value.X > npc.X)
				{
					num2++;
				}
				else
				{
					num++;
				}
			}
		}
		if (num2 > num)
		{
			return 1;
		}
		if (num2 < num)
		{
			return -1;
		}
		return -npc.Direction;
	}

	public PhysicalObj[] FindPhysicalObjByName(string name)
	{
		List<PhysicalObj> list = new List<PhysicalObj>();
		foreach (PhysicalObj item in m_map.GetAllPhysicalObjSafe())
		{
			if (item.Name == name)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public PhysicalObj[] FindPhysicalObjByName(string name, bool CanPenetrate)
	{
		List<PhysicalObj> list = new List<PhysicalObj>();
		foreach (PhysicalObj item in m_map.GetAllPhysicalObjSafe())
		{
			if (item.Name == name && item.CanPenetrate == CanPenetrate)
			{
				list.Add(item);
			}
		}
		return list.ToArray();
	}

	public Player GetFrostPlayerRadom()
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		List<Player> list = new List<Player>();
		foreach (Player item in allFightPlayers)
		{
			if (item.IsFrost)
			{
				list.Add(item);
			}
		}
		if (list.Count > 0)
		{
			int index = Random.Next(0, list.Count);
			return list.ElementAt(index);
		}
		return null;
	}

	public void Shuffer<T>(T[] array)
	{
		for (int num = array.Length; num > 1; num--)
		{
			int num2 = Random.Next(num);
			T val = array[num2];
			array[num2] = array[num - 1];
			array[num - 1] = val;
		}
	}

	public virtual bool TakeCard(Player player)
	{
		return false;
	}

	public virtual bool TakeCard(Player player, int index)
	{
		return false;
	}

	public override void Pause(int time)
	{
		m_passTick = Math.Max(m_passTick, TickHelper.GetTickCount() + time);
	}

	public override void Resume()
	{
		m_passTick = 0L;
	}

	public void AddAction(IAction action)
	{
		lock (m_actions)
		{
			m_actions.Add(action);
		}
	}

	public void AddAction(ArrayList actions)
	{
		lock (m_actions)
		{
			m_actions.AddRange(actions);
		}
	}

	public void ClearWaitTimer()
	{
		m_waitTimer = 0L;
	}

	public void WaitTime(int delay)
	{
		m_waitTimer = Math.Max(m_waitTimer, TickHelper.GetTickCount() + delay);
	}

	public long GetWaitTimer()
	{
		return m_waitTimer;
	}

	public void Update(long tick)
	{
		if (m_passTick >= tick)
		{
			return;
		}
		m_lifeTime++;
		ArrayList arrayList;
		lock (m_actions)
		{
			arrayList = (ArrayList)m_actions.Clone();
			m_actions.Clear();
		}
		if (arrayList == null || GameState == eGameState.Stopped)
		{
			return;
		}
		CurrentActionCount = arrayList.Count;
		if (arrayList.Count > 0)
		{
			ArrayList arrayList2 = new ArrayList();
			foreach (IAction item in arrayList)
			{
				try
				{
					item.Execute(this, tick);
					if (!item.IsFinished(tick))
					{
						arrayList2.Add(item);
					}
				}
				catch (Exception exception)
				{
					log.Error("Map update error:", exception);
				}
			}
			AddAction(arrayList2);
		}
		else if (m_waitTimer < tick)
		{
			CheckState(0);
		}
	}

	public List<Player> GetAllFightPlayers()
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			list.AddRange(m_players.Values);
		}
		return list;
	}

	public List<Player> GetAllTeamPlayers(Living living)
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.Team == living.Team)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public List<Player> GetAllLivingPlayers()
	{
		List<Player> list = new List<Player>();
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.IsLiving)
				{
					list.Add(value);
				}
			}
		}
		return list;
	}

	public bool GetSameTeam()
	{
		bool result = false;
		Player[] allPlayers = GetAllPlayers();
		Player[] array = allPlayers;
		foreach (Player player in array)
		{
			if (player.Team != allPlayers[0].Team)
			{
				result = false;
				break;
			}
			result = true;
		}
		return result;
	}

	public Player[] GetAllPlayers()
	{
		return GetAllFightPlayers().ToArray();
	}

	public Player GetPlayer(IGamePlayer gp)
	{
		Player result = null;
		lock (m_players)
		{
			foreach (Player value in m_players.Values)
			{
				if (value.PlayerDetail == gp)
				{
					result = value;
					break;
				}
			}
		}
		return result;
	}

	public int GetPlayerCount()
	{
		return GetAllFightPlayers().Count;
	}

	public virtual void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public virtual void SendToAll(GSPacketIn pkg, IGamePlayer except)
	{
		if (pkg.Parameter2 == 0)
		{
			pkg.Parameter2 = LifeTime;
		}
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.IsActive && item.PlayerDetail != except)
			{
				item.PlayerDetail.SendTCP(pkg);
			}
		}
	}

	public virtual void SendToTeam(GSPacketIn pkg, int team)
	{
		SendToTeam(pkg, team, null);
	}

	public virtual void SendToTeam(GSPacketIn pkg, int team, IGamePlayer except)
	{
		List<Player> allFightPlayers = GetAllFightPlayers();
		foreach (Player item in allFightPlayers)
		{
			if (item.IsActive && item.PlayerDetail != except && item.Team == team)
			{
				item.PlayerDetail.SendTCP(pkg);
			}
		}
	}

	public Ball AddBall(Point pos, bool sendToClient)
	{
		Ball ball = new Ball(PhysicalId++, "1");
		ball.SetXY(pos);
		AddPhysicalObj(ball, sendToClient);
		return AddBall(ball, sendToClient);
	}

	public Ball AddBall(Ball ball, bool sendToClient)
	{
		m_tempBall.Add(ball);
		AddPhysicalObj(ball, sendToClient);
		return ball;
	}

	public void ClearBall()
	{
		List<Ball> list = new List<Ball>();
		foreach (Ball item in m_tempBall)
		{
			list.Add(item);
		}
		foreach (Ball item2 in list)
		{
			m_tempBall.Remove(item2);
			RemovePhysicalObj(item2, sendToClient: true);
		}
	}

	public void AddTempPoint(int x, int y)
	{
		m_tempPoints.Add(new Point(x, y));
	}

	public Box AddBox(ItemInfo item, Point pos, bool sendToClient)
	{
		Box box = new Box(PhysicalId++, "1", item);
		box.SetXY(pos);
		AddPhysicalObj(box, sendToClient);
		return AddBox(box, sendToClient);
	}

	public Box AddBox(Box box, bool sendToClient)
	{
		m_tempBox.Add(box);
		AddPhysicalObj(box, sendToClient);
		return box;
	}

	public void CheckBox()
	{
		List<Box> list = new List<Box>();
		foreach (Box item in m_tempBox)
		{
			if (!item.IsLiving)
			{
				list.Add(item);
			}
		}
		foreach (Box item2 in list)
		{
			m_tempBox.Remove(item2);
			RemovePhysicalObj(item2, sendToClient: true);
		}
	}

	public List<Box> CreateBox()
	{
		int num = m_players.Count + 2;
		int num2 = 0;
		List<ItemInfo> info = null;
		if (CurrentTurnTotalDamage > 0)
		{
			num2 = m_random.Next(1, 3);
			if (m_tempBox.Count + num2 > num)
			{
				num2 = num - m_tempBox.Count;
			}
			if (num2 > 0)
			{
				DropInventory.BoxDrop(m_roomType, ref info);
			}
		}
		int diedPlayerCount = GetDiedPlayerCount();
		int num3 = 0;
		if (diedPlayerCount > 0)
		{
			num3 = m_random.Next(diedPlayerCount);
		}
		if (m_tempBox.Count + num2 + num3 > num)
		{
			num3 = num - m_tempBox.Count - num2;
		}
		List<Box> list = new List<Box>();
		if (info != null)
		{
			for (int i = 0; i < m_tempPoints.Count; i++)
			{
				int index = m_random.Next(m_tempPoints.Count);
				Point value = m_tempPoints[index];
				m_tempPoints[index] = m_tempPoints[i];
				m_tempPoints[i] = value;
			}
			int num4 = Math.Min(info.Count, m_tempPoints.Count);
			for (int j = 0; j < num4; j++)
			{
				list.Add(AddBox(info[j], m_tempPoints[j], sendToClient: false));
			}
		}
		m_tempPoints.Clear();
		return list;
	}

	public void AddLoadingFile(int type, string file, string className)
	{
		if (file != null && className != null)
		{
			m_loadingFiles.Add(new LoadingFileInfo(type, file, className));
		}
	}

	public void ClearLoadingFiles()
	{
		m_loadingFiles.Clear();
	}

	public void AfterUseItem(ItemInfo item)
	{
	}

	internal void SendCreateGame()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(101);
		gSPacketIn.WriteInt((int)m_roomType);
		gSPacketIn.WriteInt((int)m_gameType);
		gSPacketIn.WriteInt(m_timeType);
		List<Player> allFightPlayers = GetAllFightPlayers();
		bool flag = m_roomType == eRoomType.FightFootballTime;
		gSPacketIn.WriteInt(allFightPlayers.Count);
		foreach (Player item in allFightPlayers)
		{
			IGamePlayer playerDetail = item.PlayerDetail;
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteString("zonename");
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.NickName);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteByte(playerDetail.PlayerCharacter.typeVIP);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.VIPLevel);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.Sex);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Hide);
			if (flag)
			{
				gSPacketIn.WriteString(playerDetail.GetFightFootballStyle(item.Team));
			}
			else
			{
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.Style);
			}
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Colors);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Skin);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Grade);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Repute);
			if (flag)
			{
				gSPacketIn.WriteInt(70396);
			}
			else
			{
				gSPacketIn.WriteInt(playerDetail.MainWeapon.TemplateID);
			}
			gSPacketIn.WriteInt(playerDetail.MainWeapon.RefineryLevel);
			gSPacketIn.WriteString(playerDetail.MainWeapon.Name);
			gSPacketIn.WriteDateTime(DateTime.Now);
			if (playerDetail.SecondWeapon == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(playerDetail.SecondWeapon.TemplateID);
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Nimbus);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.IsShowConsortia);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.ConsortiaID);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.ConsortiaName);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.badgeID);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Win);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Total);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.FightPower);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteString("");
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.AchievementPoint);
			gSPacketIn.WriteString(playerDetail.PlayerCharacter.Honor);
			gSPacketIn.WriteInt(playerDetail.PlayerCharacter.Offer);
			gSPacketIn.WriteBoolean(val: false);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(playerDetail.PlayerCharacter.IsMarried);
			if (playerDetail.PlayerCharacter.IsMarried)
			{
				gSPacketIn.WriteInt(playerDetail.PlayerCharacter.SpouseID);
				gSPacketIn.WriteString(playerDetail.PlayerCharacter.SpouseName);
			}
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(5);
			gSPacketIn.WriteInt(item.Team);
			gSPacketIn.WriteInt(item.Id);
			gSPacketIn.WriteInt(item.MaxBlood);
			if (item.Pet == null || flag)
			{
				gSPacketIn.WriteInt(0);
				continue;
			}
			gSPacketIn.WriteInt(1);
			gSPacketIn.WriteInt(item.Pet.Place);
			gSPacketIn.WriteInt(item.Pet.TemplateID);
			gSPacketIn.WriteInt(item.Pet.ID);
			gSPacketIn.WriteString(item.Pet.Name);
			gSPacketIn.WriteInt(item.Pet.UserID);
			gSPacketIn.WriteInt(item.Pet.Level);
			List<string> skillEquip = item.Pet.GetSkillEquip();
			gSPacketIn.WriteInt(skillEquip.Count);
			foreach (string item2 in skillEquip)
			{
				gSPacketIn.WriteInt(int.Parse(item2.Split(',')[1]));
				gSPacketIn.WriteInt(int.Parse(item2.Split(',')[0]));
			}
		}
		SendToAll(gSPacketIn);
	}

	internal void SendOpenSelectLeaderWindow(int maxTime)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(102);
		gSPacketIn.WriteInt(maxTime);
		SendToAll(gSPacketIn);
	}

	internal void SendSelectObject(int playerId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, playerId);
		gSPacketIn.Parameter1 = playerId;
		gSPacketIn.WriteByte(138);
		int count = Players.Count;
		gSPacketIn.WriteInt(count);
		for (int i = 0; i < count; i++)
		{
			gSPacketIn.WriteInt(Players[i].PlayerDetail.PlayerCharacter.ID);
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteInt(Players[i].Team);
			gSPacketIn.WriteInt(Players[i].Id);
			gSPacketIn.WriteInt(4);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendSkipNext(Player player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(12);
		player.PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendPetBuff(Living player, int ID, bool isActive)
	{
		PetSkillElementInfo petSkillElementInfo = PetMgr.FindPetSkillElement(ID);
		if (petSkillElementInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(145);
			gSPacketIn.WriteInt(petSkillElementInfo.ID);
			gSPacketIn.WriteString("");
			gSPacketIn.WriteString("");
			gSPacketIn.WriteString(petSkillElementInfo.Pic.ToString());
			gSPacketIn.WriteString(petSkillElementInfo.EffectPic);
			gSPacketIn.WriteBoolean(isActive);
			SendToAll(gSPacketIn);
		}
	}

	internal void SendPetSkillCd(Living player, int skillInfoID, int ColdDown)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(147);
		gSPacketIn.WriteInt(skillInfoID);
		gSPacketIn.WriteInt(ColdDown);
		(player as Player).PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendFightStatus(Living player, int status)
	{
		if (base.RoomType == eRoomType.ActivityDungeon)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
			gSPacketIn.Parameter1 = player.Id;
			gSPacketIn.WriteByte(76);
			gSPacketIn.WriteInt(status);
			SendToAll(gSPacketIn);
		}
	}

	internal void SendIsLastMission(bool isLast)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(160);
		gSPacketIn.WriteBoolean(isLast);
		SendToAll(gSPacketIn);
	}

	internal void SendMissionTryAgain(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(119);
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal bool isTrainer()
	{
		return base.RoomType == eRoomType.Freshman;
	}

	internal void SendStartLoading(int maxTime)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(103);
		gSPacketIn.WriteInt(maxTime);
		gSPacketIn.WriteInt(m_map.Info.ID);
		gSPacketIn.WriteInt(m_loadingFiles.Count);
		foreach (LoadingFileInfo loadingFile in m_loadingFiles)
		{
			gSPacketIn.WriteInt(loadingFile.Type);
			gSPacketIn.WriteString(loadingFile.Path);
			gSPacketIn.WriteString(loadingFile.ClassName);
		}
		if (isTrainer() || base.RoomType == eRoomType.FightFootballTime)
		{
			gSPacketIn.WriteInt(0);
		}
		else
		{
			gSPacketIn.WriteInt(GameNeedPetSkillInfo.Count);
			foreach (PetSkillElementInfo item in GameNeedPetSkillInfo)
			{
				gSPacketIn.WriteString(item.Pic.ToString());
				gSPacketIn.WriteString(item.EffectPic);
			}
		}
		SendToAll(gSPacketIn);
	}

	internal void SendAddPhysicalObj(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(48);
		gSPacketIn.WriteInt(obj.Id);
		gSPacketIn.WriteInt(obj.Type);
		gSPacketIn.WriteInt(obj.X);
		gSPacketIn.WriteInt(obj.Y);
		gSPacketIn.WriteString(obj.Model);
		gSPacketIn.WriteString(obj.CurrentAction);
		gSPacketIn.WriteInt(obj.Scale);
		gSPacketIn.WriteInt(obj.Scale);
		gSPacketIn.WriteInt(obj.Rotation);
		gSPacketIn.WriteInt(obj.phyBringToFront);
		gSPacketIn.WriteInt(obj.typeEffect);
		gSPacketIn.WriteInt(obj.ActionMapping.Count);
		foreach (string key in obj.ActionMapping.Keys)
		{
			gSPacketIn.WriteString(key);
			gSPacketIn.WriteString(obj.ActionMapping[key]);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendAddPhysicalTip(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(68);
		gSPacketIn.WriteInt(obj.Id);
		gSPacketIn.WriteInt(obj.Type);
		gSPacketIn.WriteInt(obj.X);
		gSPacketIn.WriteInt(obj.Y);
		gSPacketIn.WriteString(obj.Model);
		gSPacketIn.WriteString(obj.CurrentAction);
		gSPacketIn.WriteInt(obj.Scale);
		gSPacketIn.WriteInt(obj.Rotation);
		SendToAll(gSPacketIn);
	}

	internal void SendPhysicalObjFocus(Physics obj, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(62);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteInt(obj.X);
		gSPacketIn.WriteInt(obj.Y);
		SendToAll(gSPacketIn);
	}

	internal void SendPhysicalObjPlayAction(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(66);
		gSPacketIn.WriteInt(obj.Id);
		gSPacketIn.WriteString(obj.CurrentAction);
		SendToAll(gSPacketIn);
	}

	internal void SendRemovePhysicalObj(PhysicalObj obj)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(53);
		gSPacketIn.WriteInt(obj.Id);
		SendToAll(gSPacketIn);
	}

	internal void SendRemoveLiving(int id)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(53);
		gSPacketIn.WriteInt(id);
		SendToAll(gSPacketIn);
	}

	internal void SendAddLiving(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(64);
		gSPacketIn.WriteByte((byte)living.Type);
		gSPacketIn.WriteInt(living.Id);
		gSPacketIn.WriteString(living.Name);
		gSPacketIn.WriteString(living.ModelId);
		gSPacketIn.WriteString(living.ActionStr);
		gSPacketIn.WriteInt(living.X);
		gSPacketIn.WriteInt(living.Y);
		gSPacketIn.WriteInt(living.Blood);
		gSPacketIn.WriteInt(living.MaxBlood);
		gSPacketIn.WriteInt(living.Team);
		gSPacketIn.WriteByte((byte)living.Direction);
		gSPacketIn.WriteByte(living.Config.isBotom);
		gSPacketIn.WriteBoolean(living.Config.isShowBlood);
		gSPacketIn.WriteBoolean(living.Config.isShowSmallMapPoint);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteBoolean(living.IsFrost);
		gSPacketIn.WriteBoolean(living.IsHide);
		gSPacketIn.WriteBoolean(living.IsNoHole);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteInt(0);
		if (base.RoomType == eRoomType.ActivityDungeon && living is SimpleBoss)
		{
			gSPacketIn.WriteInt((living as SimpleBoss).NpcInfo.ID);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerMove(Player player, int type, int x, int y, byte dir, bool isLiving)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		gSPacketIn.WriteByte(dir);
		gSPacketIn.WriteBoolean(isLiving);
		if (type == 2)
		{
			gSPacketIn.WriteInt(m_tempBox.Count);
			foreach (Box item in m_tempBox)
			{
				gSPacketIn.WriteInt(item.X);
				gSPacketIn.WriteInt(item.Y);
			}
		}
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerMove(Player player, int type, int x, int y, byte dir)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		gSPacketIn.WriteByte(dir);
		gSPacketIn.WriteBoolean(player.IsLiving);
		if (type == 2)
		{
			gSPacketIn.WriteInt(m_tempBox.Count);
			foreach (Box item in m_tempBox)
			{
				gSPacketIn.WriteInt(item.X);
				gSPacketIn.WriteInt(item.Y);
			}
		}
		SendToAll(gSPacketIn, player.PlayerDetail);
	}

	internal void SendLivingMoveTo(Living living, int fromX, int fromY, int toX, int toY, string action, int speed, string sAction)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(55);
		gSPacketIn.WriteInt(fromX);
		gSPacketIn.WriteInt(fromY);
		gSPacketIn.WriteInt(toX);
		gSPacketIn.WriteInt(toY);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteString(sAction);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingSay(Living living, string msg, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(59);
		gSPacketIn.WriteString(msg);
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingFall(Living living, int toX, int toY, int speed, string action, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(56);
		gSPacketIn.WriteInt(toX);
		gSPacketIn.WriteInt(toY);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingJump(Living living, int toX, int toY, int speed, string action, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(57);
		gSPacketIn.WriteInt(toX);
		gSPacketIn.WriteInt(toY);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingBeat(Living living, Living target, int totalDemageAmount, string action, int livingCount, int attackEffect)
	{
		int val = 0;
		if (target is Player)
		{
			Player player = target as Player;
			val = player.Dander;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(58);
		gSPacketIn.WriteString((!string.IsNullOrEmpty(action)) ? action : "");
		gSPacketIn.WriteInt(livingCount);
		for (int i = 1; i <= livingCount; i++)
		{
			gSPacketIn.WriteInt(target.Id);
			gSPacketIn.WriteInt(totalDemageAmount);
			gSPacketIn.WriteInt(target.Blood);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(attackEffect);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendLivingPlayMovie(Living living, string action)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(60);
		gSPacketIn.WriteString(action);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateHealth(Living living, int type, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(11);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(living.Blood);
		gSPacketIn.WriteInt(value);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateDander(TurnedLiving player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(14);
		gSPacketIn.WriteInt(player.Dander);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateFrozenState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(33);
		gSPacketIn.WriteBoolean(player.IsFrost);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateNoHoleState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(82);
		gSPacketIn.WriteBoolean(player.IsNoHole);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateHideState(Living player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(35);
		gSPacketIn.WriteBoolean(player.IsHide);
		SendToAll(gSPacketIn);
	}

	internal void SendGamePlayerProperty(Living living, string type, string state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(41);
		gSPacketIn.WriteString(type);
		gSPacketIn.WriteString(state);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingTurnRotation(Player player, int rotation, int speed, string endPlay)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(85);
		gSPacketIn.WriteInt(rotation);
		gSPacketIn.WriteInt(speed);
		gSPacketIn.WriteString(endPlay);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateShootCount(Player player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(46);
		gSPacketIn.WriteByte((byte)player.ShootCount);
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateBall(Player player, bool Special)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(20);
		gSPacketIn.WriteBoolean(Special);
		gSPacketIn.WriteInt(player.CurrentBall.ID);
		SendToAll(gSPacketIn);
	}

	internal void SendGamePickBox(Living player, int index, int arkType, string goods)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(49);
		gSPacketIn.WriteByte((byte)index);
		gSPacketIn.WriteByte((byte)arkType);
		gSPacketIn.WriteString(goods);
		SendToAll(gSPacketIn);
	}

	internal void SendGameActionMapping(Living player, Ball ball)
	{
		string currentAction = ball.CurrentAction;
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(223);
		gSPacketIn.WriteInt(ball.Id);
		gSPacketIn.WriteString(currentAction);
		gSPacketIn.WriteString(ball.ActionMapping[currentAction]);
		SendToAll(gSPacketIn);
	}

	internal void SendGameBigBox(Living player, List<int> listTemplate)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.WriteByte(136);
		gSPacketIn.WriteInt(listTemplate.Count);
		foreach (int item in listTemplate)
		{
			gSPacketIn.WriteInt(item);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendGameUpdateWind(float wind)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(38);
		int num = (int)(wind * 10f);
		gSPacketIn.WriteInt(num);
		gSPacketIn.WriteBoolean(num > 0);
		gSPacketIn.WriteByte(GetVane(num, 1));
		gSPacketIn.WriteByte(GetVane(num, 2));
		gSPacketIn.WriteByte(GetVane(num, 3));
		SendToAll(gSPacketIn);
	}

	public byte GetVane(int Wind, int param)
	{
		int wind = Math.Abs(Wind);
		return param switch
		{
			1 => WindMgr.GetWindID(wind, 1),
			3 => WindMgr.GetWindID(wind, 3),
			_ => 0,
		};
	}

	public void VaneLoading()
	{
		List<WindInfo> wind = WindMgr.GetWind();
		foreach (WindInfo item in wind)
		{
			SendGameWindPic((byte)item.WindID, item.WindPic);
		}
	}

	internal void SendGameWindPic(byte windId, byte[] windpic)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(241);
		gSPacketIn.WriteByte(windId);
		gSPacketIn.Write(windpic);
		SendToAll(gSPacketIn);
	}

	internal void SendPetUseKill(Player player)
	{
		SendPetUseKill(player, player.PetEffects.PetSkillStase, player.PetEffects.IsPetUseSkill);
	}

	internal void SendPetUseKill(Player player, int killId, bool isUse)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(144);
		gSPacketIn.WriteInt(killId);
		gSPacketIn.WriteBoolean(isUse);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingBoltmove(Player player, int x, int y)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(72);
		gSPacketIn.WriteInt(x);
		gSPacketIn.WriteInt(y);
		SendToAll(gSPacketIn);
	}

	internal void SendAddAnimation(Player player, int type, bool states)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(79);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(states);
		gSPacketIn.WriteInt(player.Id);
		if (type == 1)
		{
			gSPacketIn.WriteDateTime(DateTime.Now.AddMilliseconds(9000.0));
		}
		SendToAll(gSPacketIn);
	}

	internal void SendUseDeputyWeapon(Player player, int ResCount)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(84);
		gSPacketIn.WriteInt(ResCount);
		player.PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendPlayerUseProp(Player player, int type, int place, int templateID)
	{
		SendPlayerUseProp(player, type, place, templateID, player);
	}

	internal void SendPlayerUseProp(Living player, int type, int place, int templateID, Player p)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(32);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteInt(place);
		gSPacketIn.WriteInt(templateID);
		gSPacketIn.WriteInt(p.Id);
		gSPacketIn.WriteBoolean(templateID == 10017);
		SendToAll(gSPacketIn);
	}

	internal void SendGamePlayerTakeCard(Player player, int index, int templateID, int count)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, player.Id);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(98);
		gSPacketIn.WriteBoolean(val: false);
		gSPacketIn.WriteByte((byte)index);
		gSPacketIn.WriteInt(templateID);
		gSPacketIn.WriteInt(count);
		gSPacketIn.WriteBoolean(val: false);
		SendToAll(gSPacketIn);
	}

	public int getTurnTime()
	{
		return m_timeType switch
		{
			1 => 8,
			2 => 10,
			3 => 12,
			4 => 16,
			5 => 21,
			6 => 31,
			_ => -1,
		};
	}

	internal void SendGameNextTurn(Living living, BaseGame game, List<Box> newBoxes)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, living.Id);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(6);
		int num = (int)(m_map.wind * 10f);
		gSPacketIn.WriteBoolean(num > 0);
		gSPacketIn.WriteByte(GetVane(num, 1));
		gSPacketIn.WriteByte(GetVane(num, 2));
		gSPacketIn.WriteByte(GetVane(num, 3));
		gSPacketIn.WriteBoolean(living.IsHide);
		gSPacketIn.WriteInt(getTurnTime());
		gSPacketIn.WriteInt(newBoxes.Count);
		foreach (Box newBox in newBoxes)
		{
			gSPacketIn.WriteInt(newBox.Id);
			gSPacketIn.WriteInt(newBox.X);
			gSPacketIn.WriteInt(newBox.Y);
			gSPacketIn.WriteInt(newBox.Type);
		}
		if (living is TurnedLiving)
		{
			List<Player> allFightPlayers = game.GetAllFightPlayers();
			gSPacketIn.WriteInt(allFightPlayers.Count);
			foreach (Player item in allFightPlayers)
			{
				gSPacketIn.WriteInt(item.Id);
				gSPacketIn.WriteBoolean(item.IsLiving);
				gSPacketIn.WriteInt(item.X);
				gSPacketIn.WriteInt(item.Y);
				gSPacketIn.WriteInt(item.Blood);
				gSPacketIn.WriteBoolean(item.IsNoHole);
				gSPacketIn.WriteInt(item.Energy);
				gSPacketIn.WriteInt(item.psychic);
				gSPacketIn.WriteInt(item.Dander);
				if (item.Pet == null)
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
				}
				else
				{
					gSPacketIn.WriteInt(item.PetMaxMP);
					gSPacketIn.WriteInt(item.PetMP);
				}
				gSPacketIn.WriteInt(item.ShootCount);
				gSPacketIn.WriteInt(item.flyCount);
			}
			gSPacketIn.WriteInt(game.TurnIndex);
		}
		gSPacketIn.WriteBoolean(val: false);
		if (base.RoomType == eRoomType.FightFootballTime)
		{
			gSPacketIn.WriteInt(game.nextPlayerId);
		}
		SendToAll(gSPacketIn);
	}

	internal void SendSigleNextTurn(Living p, BaseGame game, List<Box> newBoxes)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91, p.Id);
		gSPacketIn.Parameter1 = p.Id;
		gSPacketIn.WriteByte(6);
		int num = (int)(m_map.wind * 10f);
		gSPacketIn.WriteBoolean(num > 0);
		gSPacketIn.WriteByte(GetVane(num, 1));
		gSPacketIn.WriteByte(GetVane(num, 2));
		gSPacketIn.WriteByte(GetVane(num, 3));
		gSPacketIn.WriteBoolean(p.IsHide);
		gSPacketIn.WriteInt(getTurnTime());
		gSPacketIn.WriteInt(newBoxes.Count);
		foreach (Box newBox in newBoxes)
		{
			gSPacketIn.WriteInt(newBox.Id);
			gSPacketIn.WriteInt(newBox.X);
			gSPacketIn.WriteInt(newBox.Y);
			gSPacketIn.WriteInt(newBox.Type);
		}
		gSPacketIn.WriteInt(1);
		gSPacketIn.WriteInt(p.Id);
		gSPacketIn.WriteBoolean(p.IsLiving);
		gSPacketIn.WriteInt(p.X);
		gSPacketIn.WriteInt(p.Y);
		gSPacketIn.WriteInt(p.Blood);
		gSPacketIn.WriteBoolean(p.IsNoHole);
		gSPacketIn.WriteInt(((Player)p).Energy);
		gSPacketIn.WriteInt(((Player)p).psychic);
		gSPacketIn.WriteInt(((Player)p).Dander);
		if (((Player)p).Pet == null)
		{
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
		}
		else
		{
			gSPacketIn.WriteInt(((Player)p).PetMaxMP);
			gSPacketIn.WriteInt(((Player)p).PetMP);
		}
		gSPacketIn.WriteInt(((Player)p).ShootCount);
		gSPacketIn.WriteInt(((Player)p).flyCount);
		gSPacketIn.WriteInt(game.TurnIndex);
		gSPacketIn.WriteBoolean(val: false);
		((Player)p).PlayerDetail.SendTCP(gSPacketIn);
	}

	internal void SendLivingUpdateDirection(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(living.Direction);
		SendToAll(gSPacketIn);
	}

	internal void SendLivingUpdateAngryState(Living living)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(118);
		gSPacketIn.WriteInt(living.State);
		SendToAll(gSPacketIn);
	}

	internal void SendEquipEffect(Living player, string buffer)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(buffer);
		SendToAll(gSPacketIn);
	}

	internal void SendMessage(IGamePlayer player, string msg, string msg1, int type)
	{
		if (msg != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(type);
			gSPacketIn.WriteString(msg);
			player.SendTCP(gSPacketIn);
		}
		if (msg1 != null)
		{
			GSPacketIn gSPacketIn2 = new GSPacketIn(3);
			gSPacketIn2.WriteInt(type);
			gSPacketIn2.WriteString(msg1);
			SendToAll(gSPacketIn2, player);
		}
	}

	internal void SendFightAchievement(Living living, int achievID, int dis, int delay)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(238);
		gSPacketIn.WriteInt(achievID);
		gSPacketIn.WriteInt(dis);
		gSPacketIn.WriteInt(delay);
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerPicture(Living living, int type, bool state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = living.Id;
		gSPacketIn.WriteByte(128);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(state);
		SendToAll(gSPacketIn);
	}

	internal void SendPlayerRemove(Player player)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(94, player.PlayerDetail.PlayerCharacter.ID);
		gSPacketIn.WriteByte(5);
		gSPacketIn.WriteInt(4);
		SendToAll(gSPacketIn);
	}

	internal void SendAttackEffect(Living player, int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.Parameter1 = player.Id;
		gSPacketIn.WriteByte(129);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteInt(type);
		SendToAll(gSPacketIn);
	}

	internal void SendSyncLifeTime()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		gSPacketIn.WriteByte(131);
		gSPacketIn.WriteInt(m_lifeTime);
		SendToAll(gSPacketIn);
	}

	protected void OnGameOverred()
	{
		if (GameOverred != null)
		{
			GameOverred(this);
		}
	}

	protected void OnBeginNewTurn()
	{
		if (BeginNewTurn != null)
		{
			BeginNewTurn(this);
		}
	}

	public void OnGameOverLog(int _roomId, eRoomType _roomType, eGameType _fightType, int _changeTeam, DateTime _playBegin, DateTime _playEnd, int _userCount, int _mapId, string _teamA, string _teamB, string _playResult, int _winTeam, string BossWar)
	{
		if (GameOverLog != null)
		{
			GameOverLog(_roomId, _roomType, _fightType, _changeTeam, _playBegin, _playEnd, _userCount, _mapId, _teamA, _teamB, _playResult, _winTeam, BossWarField);
		}
	}

	public void OnGameNpcDie(int Id)
	{
		if (GameNpcDie != null)
		{
			GameNpcDie(Id);
		}
	}

	public override string ToString()
	{
		return $"Id:{base.Id},player:{PlayerCount},state:{GameState},current:{CurrentLiving},turnIndex:{m_turnIndex},actions:{m_actions.Count}";
	}
}
