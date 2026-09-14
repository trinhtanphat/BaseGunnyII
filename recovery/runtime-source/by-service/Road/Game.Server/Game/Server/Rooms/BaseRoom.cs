using System.Collections.Generic;
using System.Reflection;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Rooms;

public class BaseRoom
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private GamePlayer[] m_places;

	private int[] m_placesState;

	private byte[] m_playerState;

	private int m_playerCount;

	private int m_placesCount = 10;

	private bool m_isUsing;

	private GamePlayer m_host;

	public bool IsPlaying;

	public int RoomId;

	public int maxViewerCnt = 2;

	private int m_viewerCnt;

	public int GameStyle;

	public int barrierNum;

	public string Name;

	public string Pic;

	public string Password;

	public bool isCrosszone;

	public bool isWithinLeageTime;

	public bool isOpenBoss;

	public eRoomType RoomType;

	public eGameType GameType;

	public eHardLevel HardLevel;

	public int LevelLimits;

	public int currentFloor;

	public byte TimeMode;

	public int MapId;

	public string m_roundName;

	private int m_avgLevel;

	private AbstractGame m_game;

	public BattleServer BattleServer;

	public int viewerCnt => m_viewerCnt;

	public GamePlayer Host => m_host;

	public byte[] PlayerState
	{
		get
		{
			return m_playerState;
		}
		set
		{
			m_playerState = value;
		}
	}

	public int PlayerCount => m_playerCount;

	public int PlacesCount => m_placesCount;

	public int GuildId => m_host.PlayerCharacter.ConsortiaID;

	public bool IsUsing => m_isUsing;

	public string RoundName
	{
		get
		{
			return m_roundName;
		}
		set
		{
			m_roundName = value;
		}
	}

	public bool NeedPassword => !string.IsNullOrEmpty(Password);

	public bool IsEmpty => m_playerCount == 0;

	public int AvgLevel => m_avgLevel;

	public AbstractGame Game => m_game;

	public BaseRoom(int roomId)
	{
		RoomId = roomId;
		m_places = new GamePlayer[10];
		m_placesState = new int[10];
		m_playerState = new byte[8];
		Reset();
	}

	public void Start()
	{
		if (!m_isUsing)
		{
			m_isUsing = true;
			Reset();
		}
	}

	public void Stop()
	{
		if (m_isUsing)
		{
			m_isUsing = false;
			if (m_game != null)
			{
				m_game.GameStopped -= m_game_GameStopped;
				m_game = null;
				IsPlaying = false;
			}
		}
	}

	private void Reset()
	{
		for (int i = 0; i < 10; i++)
		{
			m_places[i] = null;
			m_placesState[i] = -1;
			if (i < 8)
			{
				m_playerState[i] = 0;
			}
		}
		m_host = null;
		IsPlaying = false;
		m_placesCount = 10;
		m_playerCount = 0;
		HardLevel = eHardLevel.Simple;
	}

	public bool CanStart()
	{
		if (RoomType == eRoomType.Freedom)
		{
			int num = 0;
			int num2 = 0;
			for (int i = 0; i < 8; i++)
			{
				if (i % 2 == 0)
				{
					if (m_playerState[i] > 0)
					{
						num++;
					}
				}
				else if (m_playerState[i] > 0)
				{
					num2++;
				}
			}
			return num > 0 && num2 > 0;
		}
		int num3 = 0;
		for (int j = 0; j < 8; j++)
		{
			if (m_playerState[j] > 0)
			{
				num3++;
			}
		}
		return num3 == m_playerCount;
	}

	public bool CanAddPlayer()
	{
		return m_playerCount < m_placesCount;
	}

	public List<GamePlayer> GetPlayers()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		lock (m_places)
		{
			for (int i = 0; i < 10; i++)
			{
				if (m_places[i] != null)
				{
					list.Add(m_places[i]);
				}
			}
		}
		return list;
	}

	public void SetHost(GamePlayer player)
	{
		if (m_host != player)
		{
			if (m_host != null)
			{
				UpdatePlayerState(player, 0, sendToClient: false);
			}
			m_host = player;
			UpdatePlayerState(player, 2, sendToClient: true);
		}
	}

	public void UpdateRoom(string name, string pwd, eRoomType roomType, byte timeMode, int mapId)
	{
		Name = name;
		Password = pwd;
		RoomType = roomType;
		TimeMode = timeMode;
		MapId = mapId;
		UpdateRoomGameType();
		if (roomType == eRoomType.Freedom)
		{
			m_placesCount = 8;
		}
		else
		{
			m_placesCount = 4;
		}
	}

	public void UpdateRoomGameType()
	{
		switch (RoomType)
		{
		case eRoomType.Match:
		case eRoomType.Freedom:
			GameType = eGameType.Free;
			break;
		case eRoomType.BattleRoom:
			GameType = eGameType.BattleGame;
			break;
		case eRoomType.FightLib:
			GameType = eGameType.FightLib;
			break;
		case eRoomType.Freshman:
			GameType = eGameType.Freshman;
			break;
		case eRoomType.Encounter:
			GameType = eGameType.Encounter;
			break;
		case eRoomType.ConsortiaBoss:
			GameType = eGameType.ConsortiaBoss;
			break;
		case eRoomType.ConsortiaBattle:
			GameType = eGameType.ConsortiaBattle;
			break;
		case eRoomType.CampBattleBattle:
			GameType = eGameType.CampBattleModelPve;
			break;
		case eRoomType.FightFootballTime:
			GameType = eGameType.FightFootballTime;
			break;
		case eRoomType.Dungeon:
		case eRoomType.AcademyDungeon:
		case eRoomType.Lanbyrinth:
		case eRoomType.ActivityDungeon:
		case eRoomType.SpecialActivityDungeon:
		case eRoomType.Christmas:
			GameType = eGameType.Dungeon;
			break;
		default:
			GameType = eGameType.ALL;
			break;
		}
	}

	public void UpdatePlayerState(GamePlayer player, byte state, bool sendToClient)
	{
		m_playerState[player.CurrentRoomIndex] = state;
		if (sendToClient)
		{
			SendPlayerState();
		}
	}

	public void UpdateAvgLevel()
	{
		int num = 0;
		for (int i = 0; i < 8; i++)
		{
			if (m_places[i] != null)
			{
				num += m_places[i].PlayerCharacter.Grade;
			}
		}
		m_avgLevel = num / m_playerCount;
	}

	public void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public void SendToAll(GSPacketIn pkg, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_places)
		{
			array = (GamePlayer[])m_places.Clone();
		}
		if (array == null)
		{
			return;
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i] != except)
			{
				array[i].Out.SendTCP(pkg);
			}
		}
	}

	public void SendToTeam(GSPacketIn pkg, int team)
	{
		SendToTeam(pkg, team, null);
	}

	public void SendToTeam(GSPacketIn pkg, int team, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_places)
		{
			array = (GamePlayer[])m_places.Clone();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i].CurrentRoomTeam == team && array[i] != except)
			{
				array[i].Out.SendTCP(pkg);
			}
		}
	}

	public void SendToHost(GSPacketIn pkg)
	{
		GamePlayer[] array = null;
		lock (m_places)
		{
			array = (GamePlayer[])m_places.Clone();
		}
		for (int i = 0; i < array.Length; i++)
		{
			if (array[i] != null && array[i] == Host)
			{
				array[i].Out.SendTCP(pkg);
			}
		}
	}

	public void SendPlayerState()
	{
		GSPacketIn pkg = m_host.Out.SendRoomUpdatePlayerStates(m_playerState);
		SendToAll(pkg, m_host);
	}

	public void SendPlaceState()
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendRoomUpdatePlacesStates(m_placesState);
			SendToAll(pkg, m_host);
		}
	}

	public void SendCancelPickUp()
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendRoomPairUpCancel(this);
			SendToAll(pkg, m_host);
		}
	}

	public void SendStartPickUp()
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendRoomPairUpStart(this);
			SendToAll(pkg, m_host);
		}
	}

	public void SendMessage(eMessageType type, string msg)
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendMessage(type, msg);
			SendToAll(pkg, m_host);
		}
	}

	public void SendRoomSetupChange(BaseRoom room)
	{
		if (m_host != null)
		{
			GSPacketIn pkg = m_host.Out.SendGameRoomSetupChange(room);
			SendToAll(pkg);
		}
	}

	public bool UpdatePosUnsafe(int pos, bool isOpened, int place, int placeView)
	{
		if (pos < 0 || pos > 9)
		{
			return false;
		}
		if (m_placesState[pos] != place)
		{
			if (m_places[pos] != null)
			{
				RemovePlayerUnsafe(m_places[pos]);
			}
			m_placesState[pos] = place;
			SendPlaceState();
			if (place == -1)
			{
				if (pos < 8)
				{
					m_placesCount++;
				}
			}
			else if (pos < 8)
			{
				m_placesCount--;
			}
			return true;
		}
		return false;
	}

	public bool IsAllSameGuild()
	{
		int guildId = GuildId;
		if (guildId == 0)
		{
			return false;
		}
		List<GamePlayer> players = GetPlayers();
		if (players.Count >= 2)
		{
			foreach (GamePlayer item in players)
			{
				if (item.PlayerCharacter.ConsortiaID != guildId)
				{
					return false;
				}
			}
			return true;
		}
		return false;
	}

	public void UpdateGameStyle()
	{
		if (m_host != null && RoomType == eRoomType.Match)
		{
			if (IsAllSameGuild())
			{
				GameStyle = 1;
				GameType = eGameType.Guild;
			}
			else
			{
				GameStyle = 0;
				GameType = eGameType.Free;
			}
			GSPacketIn pkg = m_host.Out.SendRoomType(m_host, this);
			SendToAll(pkg);
		}
	}

	public bool AddPlayerUnsafe(GamePlayer player)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < 10; i++)
			{
				if (m_places[i] == null && m_placesState[i] == -1)
				{
					m_places[i] = player;
					m_placesState[i] = player.PlayerId;
					m_playerCount++;
					num = i;
					break;
				}
			}
		}
		if (num != -1)
		{
			UpdatePosUnsafe(8, isOpened: false, 0, -100);
			UpdatePosUnsafe(9, isOpened: false, 0, -100);
			player.CurrentRoom = this;
			player.CurrentRoomIndex = num;
			if (RoomType == eRoomType.Freedom)
			{
				player.CurrentRoomTeam = num % 2 + 1;
			}
			else
			{
				player.CurrentRoomTeam = 1;
			}
			GSPacketIn pkg = player.Out.SendRoomPlayerAdd(player);
			SendToAll(pkg, player);
			GSPacketIn pkg2 = player.Out.SendBufferList(player, player.BufferList.GetAllBufferByTemplate());
			SendToAll(pkg2, player);
			List<GamePlayer> players = GetPlayers();
			foreach (GamePlayer item in players)
			{
				if (item != player)
				{
					player.Out.SendRoomPlayerAdd(item);
					player.Out.SendBufferList(item, item.BufferList.GetAllBufferByTemplate());
				}
			}
			if (m_host == null)
			{
				m_host = player;
				UpdatePlayerState(player, 2, sendToClient: true);
			}
			else
			{
				UpdatePlayerState(player, 0, sendToClient: true);
			}
			SendPlaceState();
			UpdateGameStyle();
		}
		return num != -1;
	}

	public bool RemovePlayerUnsafe(GamePlayer player)
	{
		return RemovePlayerUnsafe(player, isKick: false);
	}

	public bool RemovePlayerUnsafe(GamePlayer player, bool isKick)
	{
		int num = -1;
		lock (m_places)
		{
			for (int i = 0; i < 10; i++)
			{
				if (m_places[i] == player)
				{
					m_places[i] = null;
					m_playerState[i] = 0;
					m_placesState[i] = -1;
					m_playerCount--;
					num = i;
					break;
				}
			}
		}
		if (num != -1)
		{
			UpdatePosUnsafe(num, isOpened: false, -1, -100);
			player.CurrentRoom = null;
			player.TempBag.ClearBag();
			GSPacketIn pkg = player.Out.SendRoomPlayerRemove(player);
			SendToAll(pkg);
			if (isKick)
			{
				player.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.KickRoom"));
			}
			bool flag = false;
			if (m_host == player)
			{
				if (m_playerCount > 0)
				{
					for (int j = 0; j < 10; j++)
					{
						if (m_places[j] != null)
						{
							SetHost(m_places[j]);
							flag = true;
							break;
						}
					}
				}
				else
				{
					m_host = null;
				}
			}
			if (IsPlaying)
			{
				if (m_game != null)
				{
					if (flag && m_game is PVEGame)
					{
						PVEGame pVEGame = m_game as PVEGame;
						foreach (Player value in pVEGame.Players.Values)
						{
							if (value.PlayerDetail == m_host)
							{
								value.Ready = false;
							}
						}
					}
					m_game.RemovePlayer(player, isKick);
				}
				if (BattleServer != null)
				{
					if (m_game != null)
					{
						BattleServer.Server.SendPlayerDisconnet(Game.Id, player.GamePlayerId, RoomId);
						if (PlayerCount == 0)
						{
							BattleServer.RemoveRoom(this);
						}
					}
					else
					{
						SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed"));
						RoomMgr.AddAction(new CancelPickupAction(BattleServer, this));
						BattleServer.RemoveRoom(this);
						IsPlaying = false;
					}
				}
			}
			else
			{
				UpdateGameStyle();
				if (flag)
				{
					foreach (GamePlayer player2 in GetPlayers())
					{
						player2.Out.SendGameRoomSetupChange(this);
					}
				}
			}
		}
		return num != -1;
	}

	public void RemovePlayerAtUnsafe(int pos)
	{
		if (pos >= 0 && pos <= 9)
		{
			if (m_places[pos].KickProtect)
			{
				string translation = LanguageMgr.GetTranslation("Game.Server.SceneGames.Protect", m_places[pos].PlayerCharacter.NickName);
				GSPacketIn gSPacketIn = new GSPacketIn(3);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteString(translation);
				SendToHost(gSPacketIn);
			}
			else if (m_places[pos] != null)
			{
				RemovePlayerUnsafe(m_places[pos], isKick: true);
			}
		}
	}

	public bool SwitchTeamUnsafe(GamePlayer m_player)
	{
		if (RoomType == eRoomType.Match)
		{
			return false;
		}
		int num = -1;
		lock (m_places)
		{
			for (int i = (m_player.CurrentRoomIndex + 1) % 2; i < 8; i += 2)
			{
				if (m_places[i] == null && m_placesState[i] == -1)
				{
					num = i;
					m_places[m_player.CurrentRoomIndex] = null;
					m_places[i] = m_player;
					m_placesState[m_player.CurrentRoomIndex] = -1;
					m_placesState[i] = m_player.PlayerId;
					m_playerState[i] = m_playerState[m_player.CurrentRoomIndex];
					m_playerState[m_player.CurrentRoomIndex] = 0;
					break;
				}
			}
		}
		if (num != -1)
		{
			m_player.CurrentRoomIndex = num;
			m_player.CurrentRoomTeam = num % 2 + 1;
			GSPacketIn pkg = m_player.Out.SendRoomPlayerChangedTeam(m_player);
			SendToAll(pkg, m_player);
			SendPlaceState();
			return true;
		}
		return false;
	}

	public eLevelLimits GetLevelLimit(GamePlayer player)
	{
		if (player.PlayerCharacter.Grade <= 10)
		{
			return eLevelLimits.ZeroToTen;
		}
		if (player.PlayerCharacter.Grade <= 20)
		{
			return eLevelLimits.ElevenToTwenty;
		}
		return eLevelLimits.TwentyOneToThirty;
	}

	public void StartGame(AbstractGame game)
	{
		if (m_game != null)
		{
			List<GamePlayer> players = GetPlayers();
			foreach (GamePlayer item in players)
			{
				m_game.RemovePlayer(item, IsKick: false);
			}
			m_game_GameStopped(m_game);
		}
		m_game = game;
		IsPlaying = true;
		m_game.GameStopped += m_game_GameStopped;
	}

	private void m_game_GameStopped(AbstractGame game)
	{
		if (game != null)
		{
			m_game.GameStopped -= m_game_GameStopped;
			m_game = null;
			IsPlaying = false;
			UpdateGameStyle();
			ResetPlayerState();
		}
	}

	public void ResetPlayerState()
	{
		for (int i = 0; i < m_playerState.Length; i++)
		{
			if (m_playerState[i] != 2)
			{
				m_playerState[i] = 0;
			}
		}
	}

	public string GetNameByMapId()
	{
		string text = "Chưa lựa chọn?";
		string text2 = "<Mời tham gia Phó bản>";
		MapInfo mapInfo = MapMgr.FindMapInfo(MapId);
		if (mapInfo != null)
		{
			text = mapInfo.Name;
		}
		PveInfo pveInfoById = PveInfoMgr.GetPveInfoById(MapId);
		if (pveInfoById != null)
		{
			text = pveInfoById.Name + GetNameHardLv();
		}
		else
		{
			text2 = "<Mời tham gia đấu đội>";
		}
		return text2 + text;
	}

	public string GetNameHardLv()
	{
		string result = " (Dể)";
		switch (HardLevel)
		{
		case eHardLevel.Normal:
			result = " (Thường)";
			break;
		case eHardLevel.Hard:
			result = " (Khó)";
			break;
		case eHardLevel.Terror:
			result = " (Anh hùng)";
			break;
		case eHardLevel.Epic:
			result = " (Legend)";
			break;
		}
		return result;
	}

	public void ProcessData(GSPacketIn packet)
	{
		if (m_game != null)
		{
			m_game.ProcessData(packet);
		}
	}

	public void RemoveAllPlayer()
	{
		for (int i = 0; i < 10; i++)
		{
			if (m_places[i] != null)
			{
				RoomMgr.AddAction(new ExitRoomAction(this, m_places[i]));
				RoomMgr.AddAction(new EnterWaitingRoomAction(m_places[i]));
			}
		}
	}

	public override string ToString()
	{
		return $"Id:{RoomId},player:{PlayerCount},game:{Game},isPlaying:{IsPlaying}";
	}
}
