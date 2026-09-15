using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Packets;

namespace Game.Server.Rooms;

public class CreateGroupBattleRoomAction : IAction
{
	private GamePlayer m_player;

	private string m_name;

	private string m_password;

	private eRoomType m_roomType;

	private byte m_timeType;

	private int m_groupType;

	public CreateGroupBattleRoomAction(GamePlayer player, int groupType)
	{
		m_player = player;
		m_name = "GroupBattle PvP";
		m_password = "<redacted-runtime-room-password>";
		m_roomType = eRoomType.SingleBattle;
		m_timeType = 2;
		m_groupType = groupType;
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
		int num = GetGroup(m_groupType);
		if (num == 1)
		{
			BaseRoom baseRoom = null;
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
			return;
		}
		BaseRoom baseRoom2 = FindRandomRoom(rooms, num);
		if (baseRoom2 == null)
		{
			for (int j = 0; j < rooms.Length; j++)
			{
				if (!rooms[j].IsUsing)
				{
					baseRoom2 = rooms[j];
					break;
				}
			}
			if (baseRoom2 != null)
			{
				RoomMgr.WaitingRoom.RemovePlayer(m_player);
				baseRoom2.Start();
				baseRoom2.UpdateRoom(m_name, m_password, m_roomType, m_timeType, 0);
				m_player.Out.SendSingleRoomCreate(baseRoom2);
				baseRoom2.AddPlayerUnsafe(m_player);
			}
		}
		else
		{
			RoomMgr.WaitingRoom.RemovePlayer(m_player);
			m_player.Out.SendRoomLoginResult(result: true);
			m_player.Out.SendSingleRoomCreate(baseRoom2);
			baseRoom2.AddPlayerUnsafe(m_player);
		}
		if (baseRoom2.PlayerCount == num)
		{
			BattleServer battleServer2 = BattleMgr.AddRoom(baseRoom2);
			if (battleServer2 != null)
			{
				baseRoom2.BattleServer = battleServer2;
				baseRoom2.IsPlaying = true;
				baseRoom2.SendStartPickUp();
			}
			else
			{
				GSPacketIn pkg2 = baseRoom2.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
				baseRoom2.SendToAll(pkg2, baseRoom2.Host);
				baseRoom2.SendCancelPickUp();
			}
		}
	}

	private int GetGroup(int group)
	{
		return group switch
		{
			7 => 1,
			8 => 2,
			9 => 3,
			10 => 4,
			_ => 1,
		};
	}

	private BaseRoom FindRandomRoom(BaseRoom[] rooms, int group)
	{
		for (int i = 0; i < rooms.Length; i++)
		{
			if (rooms[i] != null && rooms[i].RoomType == m_roomType && !rooms[i].IsPlaying && rooms[i].PlayerCount < group && rooms[i].RoomType == eRoomType.SingleBattle)
			{
				return rooms[i];
			}
		}
		return null;
	}
}
