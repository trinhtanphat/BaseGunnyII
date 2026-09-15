using Game.Logic;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Rooms;

internal class EnterRoomAction : IAction
{
	private GamePlayer m_player;

	private int m_roomId;

	private string m_pwd;

	private int m_type;

	public EnterRoomAction(GamePlayer player, int roomId, string pwd, int type)
	{
		m_player = player;
		m_roomId = roomId;
		m_pwd = pwd;
		m_type = type;
	}

	public void Execute()
	{
		if (!m_player.IsActive)
		{
			return;
		}
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.RemovePlayerUnsafe(m_player);
		}
		BaseRoom[] rooms = RoomMgr.Rooms;
		BaseRoom baseRoom;
		if (m_roomId == -1)
		{
			baseRoom = FindRandomRoom(rooms);
			if (baseRoom == null)
			{
				m_player.Out.SendMessage(eMessageType.ERROR, "Không có phòng game nào!");
				m_player.Out.SendRoomLoginResult(result: false);
				return;
			}
		}
		else
		{
			baseRoom = rooms[m_roomId - 1];
		}
		if (!baseRoom.IsUsing)
		{
			m_player.Out.SendMessage(eMessageType.Normal, "Phòng đang sử dụng!");
		}
		else if (baseRoom != null && baseRoom.RoomType == eRoomType.ActivityDungeon && m_player.Actives.Info.activityTanabataNum > 0)
		{
			m_player.Out.SendMessage(eMessageType.Normal, "Bạn đã tham gia hôm nay!");
		}
		else if (baseRoom.PlayerCount == baseRoom.PlacesCount)
		{
			m_player.Out.SendMessage(eMessageType.ERROR, "Phòng đã đấy！");
		}
		else if (!baseRoom.NeedPassword || baseRoom.Password == m_pwd)
		{
			if (baseRoom.Game != null && !baseRoom.Game.CanAddPlayer())
			{
				return;
			}
			if (baseRoom.LevelLimits > (int)baseRoom.GetLevelLimit(m_player))
			{
				m_player.Out.SendMessage(eMessageType.ERROR, "Level chưa đủ！");
				return;
			}
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			m_player.Out.SendRoomLoginResult(result: true);
			m_player.Out.SendRoomCreate(baseRoom);
			if (baseRoom.AddPlayerUnsafe(m_player) && baseRoom.Game != null)
			{
				baseRoom.Game.AddPlayer(m_player);
			}
			RoomMgr.WaitingRoom.SendUpdateRoom(baseRoom);
			m_player.Out.SendGameRoomSetupChange(baseRoom);
		}
		else
		{
			m_player.Out.SendMessage(eMessageType.ERROR, "Password không đúng!");
			m_player.Out.SendRoomLoginResult(result: false);
		}
	}

	private BaseRoom FindRandomRoom(BaseRoom[] rooms)
	{
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i].PlayerCount <= 0 || !rooms[i].CanAddPlayer() || rooms[i].NeedPassword || rooms[i].IsPlaying)
			{
				continue;
			}
			if (10 != m_type)
			{
				if (rooms[i].RoomType == (eRoomType)m_type)
				{
					return rooms[i];
				}
			}
			else if (rooms[i].RoomType == (eRoomType)m_type && rooms[i].LevelLimits < (int)rooms[i].GetLevelLimit(m_player))
			{
				return rooms[i];
			}
		}
		return null;
	}
}
