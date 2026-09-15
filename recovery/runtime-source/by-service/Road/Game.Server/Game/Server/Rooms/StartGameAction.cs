using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Battle;
using Game.Server.GameObjects;
using Game.Server.Games;
using Game.Server.Packets;

namespace Game.Server.Rooms;

public class StartGameAction : IAction
{
	private BaseRoom m_room;

	public StartGameAction(BaseRoom room)
	{
		m_room = room;
	}

	public void Execute()
	{
		if (!m_room.CanStart())
		{
			return;
		}
		List<GamePlayer> players = m_room.GetPlayers();
		if (m_room.RoomType == eRoomType.Freedom)
		{
			List<IGamePlayer> list = new List<IGamePlayer>();
			List<IGamePlayer> list2 = new List<IGamePlayer>();
			foreach (GamePlayer item in players)
			{
				if (item != null)
				{
					if (item.CurrentRoomTeam == 1)
					{
						list.Add(item);
					}
					else
					{
						list2.Add(item);
					}
				}
			}
			BaseGame game = GameMgr.StartPVPGame(m_room.RoomId, list, list2, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode);
			StartGame(game);
		}
		else if (IsPVE(m_room.RoomType))
		{
			List<IGamePlayer> list3 = new List<IGamePlayer>();
			foreach (GamePlayer item2 in players)
			{
				if (item2 != null)
				{
					list3.Add(item2);
				}
			}
			UpdatePveRoomTimeMode();
			BaseGame game2 = GameMgr.StartPVEGame(m_room.RoomId, list3, m_room.MapId, m_room.RoomType, m_room.GameType, m_room.TimeMode, m_room.HardLevel, m_room.LevelLimits, m_room.currentFloor);
			StartGame(game2);
		}
		else if (m_room.RoomType == eRoomType.Match)
		{
			m_room.UpdateAvgLevel();
			BattleServer battleServer = BattleMgr.AddRoom(m_room);
			if (battleServer != null)
			{
				m_room.BattleServer = battleServer;
				m_room.IsPlaying = true;
				m_room.SendStartPickUp();
			}
			else
			{
				GSPacketIn pkg = m_room.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("StartGameAction.noBattleServe"));
				m_room.SendToAll(pkg, m_room.Host);
				m_room.SendCancelPickUp();
			}
		}
		RoomMgr.WaitingRoom.SendUpdateRoom(m_room);
	}

	private void StartGame(BaseGame game)
	{
		if (game != null)
		{
			m_room.IsPlaying = true;
			m_room.StartGame(game);
		}
		else
		{
			m_room.IsPlaying = false;
			m_room.SendPlayerState();
		}
	}

	private bool IsPVE(eRoomType roomType)
	{
		if (roomType <= eRoomType.ConsortiaBoss)
		{
			switch (roomType)
			{
			case eRoomType.ScoreLeage:
			case eRoomType.GuildLeageRank:
			case eRoomType.Encounter:
				return false;
			default:
				return false;
			case eRoomType.Dungeon:
			case eRoomType.FightLib:
			case eRoomType.Freshman:
			case eRoomType.AcademyDungeon:
			case eRoomType.WordBossFight:
			case eRoomType.Lanbyrinth:
			case eRoomType.ConsortiaBoss:
				break;
			}
		}
		else
		{
			switch (roomType)
			{
			case eRoomType.TransnationalFight:
				return false;
			default:
				return false;
			case eRoomType.ActivityDungeon:
			case eRoomType.SpecialActivityDungeon:
			case eRoomType.Christmas:
				break;
			}
		}
		return true;
	}

	private void UpdatePveRoomTimeMode()
	{
		if (IsPVE(m_room.RoomType))
		{
			switch (m_room.HardLevel)
			{
			case eHardLevel.Simple:
				m_room.TimeMode = 3;
				break;
			case eHardLevel.Normal:
				m_room.TimeMode = 2;
				break;
			case eHardLevel.Hard:
				m_room.TimeMode = 1;
				break;
			case eHardLevel.Terror:
				m_room.TimeMode = 1;
				break;
			}
		}
	}
}
