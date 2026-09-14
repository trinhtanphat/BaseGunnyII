using System.Collections.Generic;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;

namespace Game.Server.Rooms;

public class CreateCampBattleRoomAction : IAction
{
	private GamePlayer m_player;

	private GamePlayer m_challenge;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	public CreateCampBattleRoomAction(GamePlayer player, GamePlayer ChallengePlayer)
	{
		m_player = player;
		m_challenge = ChallengePlayer;
		m_name = "Camp Battle";
		m_password = "<redacted-runtime-room-password>";
		m_roomType = eRoomType.ConsortiaBattle;
		m_timeType = 2;
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
		if (m_challenge.CurrentRoom != null)
		{
			m_challenge.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		if (!m_challenge.IsActive)
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
		baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
		m_player.Out.SendRoomCreate(baseRoom);
		if (baseRoom.AddPlayerUnsafe(m_player))
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_challenge);
			m_challenge.Out.SendRoomLoginResult(result: true);
			m_challenge.Out.SendRoomCreate(baseRoom);
			baseRoom.AddPlayerUnsafe(m_challenge);
		}
		if (baseRoom.PlayerCount == 2)
		{
			List<IGamePlayer> list = new List<IGamePlayer>();
			List<IGamePlayer> list2 = new List<IGamePlayer>();
			list.Add(m_player);
			list2.Add(m_challenge);
			BaseGame baseGame = GameMgr.StartPVPGame(baseRoom.RoomId, list, list2, baseRoom.MapId, baseRoom.RoomType, baseRoom.GameType, baseRoom.TimeMode);
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
}
