using System.Collections.Generic;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms;

public class CreateCampBattleBossAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	private int m_bossLevel;

	private int m_mapId;

	public CreateCampBattleBossAction(GamePlayer player, eRoomType roomType, int bossLevel, int mapId)
	{
		m_player = player;
		m_name = "Camp Battle Boss";
		m_password = "<redacted-runtime-room-password>";
		m_roomType = roomType;
		m_timeType = 2;
		m_bossLevel = bossLevel;
		m_mapId = mapId;
	}

	public void Execute()
	{
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		if (!m_player.IsActive)
		{
			return;
		}
		BaseRoom[] rooms = RoomMgr.Rooms;
		BaseRoom baseRoom = null;
		for (int i = 0; i < rooms.Length; i++)
		{
			if (!rooms[i].IsUsing)
			{
				baseRoom = rooms[i];
				break;
			}
		}
		if (baseRoom == null)
		{
			return;
		}
		RoomMgr.WaitingRoom.RemovePlayer(m_player);
		baseRoom.Start();
		baseRoom.HardLevel = eHardLevel.Normal;
		baseRoom.LevelLimits = (int)baseRoom.GetLevelLimit(m_player);
		baseRoom.isCrosszone = false;
		baseRoom.isOpenBoss = false;
		baseRoom.MapId = m_mapId;
		baseRoom.currentFloor = m_bossLevel;
		baseRoom.TimeMode = m_timeType;
		baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, baseRoom.MapId);
		m_player.Out.SendRoomCreate(baseRoom);
		if (!baseRoom.AddPlayerUnsafe(m_player))
		{
			return;
		}
		List<GamePlayer> players = baseRoom.GetPlayers();
		List<IGamePlayer> list = new List<IGamePlayer>();
		foreach (GamePlayer item in players)
		{
			if (item != null)
			{
				list.Add(item);
			}
		}
		BaseGame baseGame = GameMgr.StartPVEGame(baseRoom.RoomId, list, baseRoom.MapId, baseRoom.RoomType, baseRoom.GameType, baseRoom.TimeMode, baseRoom.HardLevel, baseRoom.LevelLimits, baseRoom.currentFloor);
		if (baseGame != null)
		{
			baseRoom.IsPlaying = true;
			baseRoom.StartGame(baseGame);
		}
		else
		{
			baseRoom.IsPlaying = false;
			baseRoom.SendPlayerState();
		}
	}
}
