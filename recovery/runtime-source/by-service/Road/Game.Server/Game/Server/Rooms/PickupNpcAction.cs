using System.Collections.Generic;
using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Managers;

namespace Game.Server.Rooms;

public class PickupNpcAction : IAction
{
	private int m_roomID;

	public PickupNpcAction(int roomID)
	{
		m_roomID = roomID;
	}

	public GamePlayer CreateNpcPlayer()
	{
		return new GamePlayer(1, "Null Player", null, null);
	}

	public void Execute()
	{
		BaseRoom baseRoom = null;
		BaseRoom[] rooms = RoomMgr.Rooms;
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i].RoomId == m_roomID)
			{
				baseRoom = rooms[i];
				break;
			}
		}
		if (baseRoom == null)
		{
			return;
		}
		GamePlayer playerById = WorldMgr.GetPlayerById(19);
		if (playerById == null)
		{
			return;
		}
		RoomMgr.WaitingRoom.RemovePlayer(playerById);
		playerById.Out.SendRoomLoginResult(result: true);
		playerById.Out.SendRoomCreate(baseRoom);
		baseRoom.AddPlayerUnsafe(playerById);
		if (baseRoom.PlayerCount == 2)
		{
			List<IGamePlayer> list = new List<IGamePlayer>();
			List<IGamePlayer> list2 = new List<IGamePlayer>();
			list.Add(baseRoom.Host);
			list2.Add(playerById);
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
