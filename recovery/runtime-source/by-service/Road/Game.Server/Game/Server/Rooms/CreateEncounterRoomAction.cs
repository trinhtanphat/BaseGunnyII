using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Rooms;

public class CreateEncounterRoomAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	public CreateEncounterRoomAction(GamePlayer player, eRoomType roomType)
	{
		m_player = player;
		m_name = "Couple PvP";
		m_password = "<redacted-runtime-room-password>";
		m_roomType = roomType;
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
		BaseRoom[] rooms = RoomMgr.Rooms;
		BaseRoom baseRoom = FindRandomRoom(rooms);
		if (baseRoom == null)
		{
			for (int i = 0; i < rooms.Length; i++)
			{
				if (!rooms[i].IsUsing)
				{
					baseRoom = rooms[i];
					break;
				}
			}
			if (baseRoom != null)
			{
				RoomMgr.WaitingRoom.RemovePlayer(m_player);
				baseRoom.Start();
				baseRoom.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
				m_player.Out.SendSingleRoomCreate(baseRoom);
				baseRoom.AddPlayerUnsafe(m_player);
			}
		}
		else
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			m_player.Out.SendRoomLoginResult(result: true);
			m_player.Out.SendSingleRoomCreate(baseRoom);
			baseRoom.AddPlayerUnsafe(m_player);
		}
		if (baseRoom.PlayerCount == 2)
		{
			BattleServer battleServer = BattleMgr.AddRoom(baseRoom);
			if (battleServer != null)
			{
				baseRoom.BattleServer = battleServer;
				baseRoom.IsPlaying = true;
				baseRoom.SendStartPickUp();
			}
			else
			{
				GSPacketIn pkg = baseRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
				baseRoom.SendToAll(pkg, baseRoom.Host);
				baseRoom.SendCancelPickUp();
			}
		}
	}

	private BaseRoom FindRandomRoom(BaseRoom[] rooms)
	{
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i] != null && rooms[i].RoomType == m_roomType && !rooms[i].IsPlaying && rooms[i].PlayerCount < 2 && rooms[i].RoomType == eRoomType.Encounter && rooms[i].Host != null && rooms[i].Host.PlayerCharacter.Sex != m_player.PlayerCharacter.Sex)
			{
				return rooms[i];
			}
		}
		return null;
	}
}
