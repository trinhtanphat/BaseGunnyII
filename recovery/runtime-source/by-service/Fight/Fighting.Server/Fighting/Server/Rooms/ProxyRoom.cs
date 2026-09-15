using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Game.Base.Packets;
using Game.Logic;
using log4net;

namespace Fighting.Server.Rooms;

public class ProxyRoom
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private List<IGamePlayer> m_players;

	private int m_roomId;

	private int m_orientRoomId;

	private ServerClient m_client;

	public int PickUpCount;

	public bool PickUpNPC;

	public int PickUpNPCID;

	public bool IsPlaying;

	public eGameType GameType;

	public eRoomType RoomType;

	public int GuildId;

	public string GuildName;

	public int AvgLevel;

	public int FightPower;

	private BaseGame m_game;

	public int RoomId => m_roomId;

	public int OrientRoomId => m_orientRoomId;

	public ServerClient Client => m_client;

	public int PlayerCount => m_players.Count;

	public BaseGame Game => m_game;

	public ProxyRoom(int roomId, int orientRoomId, IGamePlayer[] players, ServerClient client, int totallevel, int totalFightPower)
	{
		m_roomId = roomId;
		m_orientRoomId = orientRoomId;
		m_players = new List<IGamePlayer>();
		m_players.AddRange(players);
		m_client = client;
		PickUpCount = 0;
		FightPower = totalFightPower;
		AvgLevel = totallevel / players.Count();
		PickUpNPC = false;
	}

	public void SendToAll(GSPacketIn pkg)
	{
		SendToAll(pkg, null);
	}

	public void SendToAll(GSPacketIn pkg, IGamePlayer except)
	{
		m_client.SendToRoom(m_orientRoomId, pkg, except);
	}

	public List<IGamePlayer> GetPlayers()
	{
		List<IGamePlayer> list = new List<IGamePlayer>();
		lock (m_players)
		{
			list.AddRange(m_players);
		}
		return list;
	}

	public bool RemovePlayer(IGamePlayer player)
	{
		bool result = false;
		lock (m_players)
		{
			if (m_players.Remove(player))
			{
				result = true;
			}
		}
		if (PlayerCount == 0)
		{
			ProxyRoomMgr.RemoveRoom(this);
		}
		return result;
	}

	public void StartGame(BaseGame game)
	{
		IsPlaying = true;
		m_game = game;
		game.GameStopped += game_GameStopped;
		m_client.SendStartGame(m_orientRoomId, game);
	}

	private void game_GameStopped(AbstractGame game)
	{
		m_game.GameStopped -= game_GameStopped;
		IsPlaying = false;
		m_client.SendStopGame(m_orientRoomId, m_game.Id);
	}

	public void Dispose()
	{
		m_client.RemoveRoom(m_orientRoomId, this);
	}

	public override string ToString()
	{
		return $"RoomId:{m_roomId} OriendId:{m_orientRoomId} PlayerCount:{m_players.Count},IsPlaying:{IsPlaying},GuildId:{GuildId},GuildName:{GuildName}";
	}
}
