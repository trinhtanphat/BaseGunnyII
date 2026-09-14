using System.Collections.Generic;
using Game.Server.GameObjects;

namespace Game.Server.Rooms;

public class EnterWaitingRoomAction : IAction
{
	private GamePlayer m_player;

	public EnterWaitingRoomAction(GamePlayer player)
	{
		m_player = player;
	}

	public void Execute()
	{
		if (m_player == null)
		{
			return;
		}
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		BaseWaitingRoom waitingRoom = RoomMgr.WaitingRoom;
		if (!waitingRoom.AddPlayer(m_player))
		{
			return;
		}
		List<BaseRoom> allRooms = RoomMgr.GetAllRooms();
		m_player.Out.SendUpdateRoomList(allRooms);
		m_player.Out.SendSceneAddPlayer(m_player);
		GamePlayer[] playersSafe = waitingRoom.GetPlayersSafe();
		foreach (GamePlayer gamePlayer in playersSafe)
		{
			if (gamePlayer != m_player)
			{
				m_player.Out.SendSceneAddPlayer(gamePlayer);
			}
		}
	}
}
