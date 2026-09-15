using System;
using System.Collections.Generic;
using Bussiness;
using Game.Base.Packets;
using Game.Logic;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Rooms;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(94, "游戏创建")]
public class GameRoomHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		int num = packet.ReadInt();
		switch (num)
		{
		case 0:
		{
			byte b = packet.ReadByte();
			byte timeType = packet.ReadByte();
			string name = packet.ReadString();
			string password2 = packet.ReadString();
			if (b == 15)
			{
				if (!client.Player.Labyrinth.completeChallenge)
				{
					client.Player.SendMessage("Bạn đã hết lượt khiêu chến hôm nay!");
					return 0;
				}
				client.Player.Labyrinth.isInGame = true;
			}
			if (b == 14)
			{
				if (DateTime.Compare(client.Player.LastEnterWorldBoss.AddSeconds(25.0), DateTime.Now) > 0)
				{
					int num6 = 25 - (int)(DateTime.Now - client.Player.LastEnterWorldBoss).TotalSeconds;
					client.Out.SendMessage(eMessageType.Normal, LanguageMgr.GetTranslation("Tốc độ client quá nhanh, vui lòng đợi " + num6 + "s."));
					return 0;
				}
				client.Player.LastEnterWorldBoss = DateTime.Now;
				client.Player.WorldbossBood = RoomMgr.WorldBossRoom.Blood;
				BufferList.CreatePayBuffer(400, 50000, 1)?.Start(client.Player);
				BufferList.CreatePayBuffer(406, 30000, 1)?.Start(client.Player);
			}
			RoomMgr.CreateRoom(client.Player, name, password2, (eRoomType)b, timeType);
			return 0;
		}
		case 1:
		{
			packet.ReadBoolean();
			int type = packet.ReadInt();
			int num8 = packet.ReadInt();
			int roomId = -1;
			string pwd = null;
			if (num8 == -1)
			{
				roomId = packet.ReadInt();
				pwd = packet.ReadString();
			}
			RoomMgr.EnterRoom(client.Player, roomId, pwd, type);
			return 0;
		}
		case 2:
			if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host && !client.Player.CurrentRoom.IsPlaying)
			{
				int num2 = packet.ReadInt();
				eRoomType eRoomType2 = (eRoomType)packet.ReadByte();
				bool flag = packet.ReadBoolean();
				string pic = "";
				if (flag)
				{
					pic = packet.ReadString();
				}
				string password = packet.ReadString();
				string roomname = packet.ReadString();
				byte timeMode = packet.ReadByte();
				byte hardLevel = packet.ReadByte();
				int levelLimits = packet.ReadInt();
				bool isCrosszone = packet.ReadBoolean();
				int num3 = packet.ReadInt();
				int currentFloor = 1;
				if (num2 == 0 && eRoomType2 == eRoomType.Lanbyrinth)
				{
					num2 = 401;
					currentFloor = client.Player.Labyrinth.currentFloor;
				}
				if (num2 == 10000 && eRoomType2 == eRoomType.Dungeon)
				{
					num2 = num3;
				}
				if (eRoomType2 == eRoomType.ActivityDungeon)
				{
					string format = "{0} đã tham gia hôm nay.";
					BaseRoom currentRoom = client.Player.CurrentRoom;
					List<GamePlayer> players = currentRoom.GetPlayers();
					int num4 = 0;
					string text = "";
					foreach (GamePlayer item in players)
					{
						if (item.Actives.Info.activityTanabataNum > 0)
						{
							text = ((item.PlayerCharacter.ID != client.Player.PlayerCharacter.ID) ? (text + item.PlayerCharacter.NickName) : (text + "Bạn"));
							text += ",";
							num4++;
						}
					}
					if (num4 > 0)
					{
						text = text.Substring(0, text.Length - 1);
						client.Player.SendMessage(string.Format(format, text));
						return 0;
					}
				}
				RoomMgr.UpdateRoomGameType(client.Player.CurrentRoom, eRoomType2, timeMode, (eHardLevel)hardLevel, levelLimits, num2, password, roomname, isCrosszone, flag, pic, currentFloor);
				return 0;
			}
			return 0;
		case 3:
			if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
			{
				RoomMgr.KickPlayer(client.Player.CurrentRoom, packet.ReadByte());
				return 0;
			}
			return 0;
		case 5:
			if (client.Player.CurrentRoom != null)
			{
				RoomMgr.ExitRoom(client.Player.CurrentRoom, client.Player);
				return 0;
			}
			return 0;
		case 6:
			if (client.Player.CurrentRoom == null || client.Player.CurrentRoom.RoomType == eRoomType.Match)
			{
				return 0;
			}
			RoomMgr.SwitchTeam(client.Player);
			return 0;
		case 7:
		{
			BaseRoom currentRoom2 = client.Player.CurrentRoom;
			if (currentRoom2 == null || currentRoom2.Host != client.Player)
			{
				return 0;
			}
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			if (currentRoom2.RoomType == eRoomType.Dungeon && !client.Player.IsPvePermission(currentRoom2.MapId, currentRoom2.HardLevel))
			{
				client.Player.SendMessage("Không thể tham gia phó bản này!");
				return 0;
			}
			if (currentRoom2.RoomType == eRoomType.ActivityDungeon)
			{
				List<GamePlayer> players2 = currentRoom2.GetPlayers();
				foreach (GamePlayer item2 in players2)
				{
					if (item2.Actives.Info.activityTanabataNum == 0)
					{
						item2.Actives.Info.activityTanabataNum++;
					}
				}
			}
			client.Player.PetBag.ReduceHunger();
			RoomMgr.StartGame(client.Player.CurrentRoom);
			return 0;
		}
		case 9:
		{
			packet.ReadInt();
			int num7 = packet.ReadInt();
			if (num7 == -2)
			{
				packet.ReadInt();
				packet.ReadInt();
			}
			List<BaseRoom> room;
			switch (num7)
			{
			case 3:
			case 4:
			case 5:
				room = RoomMgr.GetAllMatchRooms();
				break;
			default:
				room = RoomMgr.GetAllPveRooms();
				break;
			}
			client.Player.Out.SendUpdateRoomList(room);
			return 0;
		}
		case 10:
			if (client.Player.CurrentRoom != null && client.Player == client.Player.CurrentRoom.Host)
			{
				byte pos = packet.ReadByte();
				int place = packet.ReadInt();
				bool isOpened = packet.ReadBoolean();
				int placeView = packet.ReadInt();
				RoomMgr.UpdateRoomPos(client.Player.CurrentRoom, pos, isOpened, place, placeView);
				return 0;
			}
			return 0;
		case 11:
			if (client.Player.CurrentRoom == null || client.Player.CurrentRoom.BattleServer == null)
			{
				return 0;
			}
			client.Player.CurrentRoom.BattleServer.RemoveRoom(client.Player.CurrentRoom);
			if (client.Player != client.Player.CurrentRoom.Host)
			{
				client.Player.CurrentRoom.Host.Out.SendMessage(eMessageType.ChatERROR, LanguageMgr.GetTranslation("Game.Server.SceneGames.PairUp.Failed"));
				RoomMgr.UpdatePlayerState(client.Player, 0);
				return 0;
			}
			if (client.Player.CurrentRoom.RoomType == eRoomType.BattleRoom)
			{
				client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
				return 0;
			}
			RoomMgr.UpdatePlayerState(client.Player, 2);
			return 0;
		case 12:
			packet.ReadInt();
			if (client.Player.CurrentRoom != null)
			{
				if (packet.ReadInt() == 0)
				{
					client.Player.CurrentRoom.GameType = eGameType.Free;
				}
				else
				{
					client.Player.CurrentRoom.GameType = eGameType.Guild;
				}
				GSPacketIn pkg = client.Player.Out.SendRoomType(client.Player, client.Player.CurrentRoom);
				client.Player.CurrentRoom.SendToAll(pkg, client.Player);
				return 0;
			}
			return 0;
		case 15:
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			if (client.Player.CurrentRoom != null)
			{
				RoomMgr.UpdatePlayerState(client.Player, packet.ReadByte());
				return 0;
			}
			return 0;
		case 18:
		{
			int num5 = packet.ReadInt();
			if (client.Player.MainWeapon == null)
			{
				client.Player.SendMessage(LanguageMgr.GetTranslation("Game.Server.SceneGames.NoEquip"));
				return 0;
			}
			switch (num5)
			{
			case 1:
				RoomMgr.CreateEncounterRoom(client.Player, eRoomType.Encounter);
				return 0;
			case 2:
			{
				ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(client.Player.PlayerCharacter.ConsortiaID);
				int bossLevel = 10;
				if (consortiaById != null)
				{
					bossLevel = consortiaById.callBossLevel;
				}
				RoomMgr.CreateConsortiaBossRoom(client.Player, eRoomType.ConsortiaBoss, bossLevel);
				return 0;
			}
			case 3:
				RoomMgr.CreateBattleRoom(client.Player, eRoomType.BattleRoom);
				return 0;
			case 4:
				if (client.Player.CurrentRoom != null)
				{
					client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
				}
				if (!client.Player.IsActive)
				{
					return 0;
				}
				RoomMgr.WaitingRoom.RemovePlayer(client.Player);
				RoomMgr.ConsBatRoom.AddPlayer(client.Player);
				return 0;
			case 7:
			case 8:
			case 9:
			case 10:
				RoomMgr.CreateGroupBattleRoom(client.Player, num5);
				return 0;
			case 17:
				if (client.Player.CurrentRoom != null)
				{
					client.Player.CurrentRoom.RemovePlayerUnsafe(client.Player);
				}
				if (!client.Player.IsActive)
				{
					return 0;
				}
				RoomMgr.WaitingRoom.RemovePlayer(client.Player);
				RoomMgr.CampBattleRoom.AddPlayer(client.Player);
				return 0;
			case 20:
				client.Player.ClearFootballCard();
				RoomMgr.CreateFightFootballTimeRoom(client.Player, eRoomType.FightFootballTime);
				return 0;
			default:
				Console.WriteLine("SINGLE_ROOM_BEGIN  " + num5);
				return 0;
			}
		}
		case 19:
		{
			ItemInfo itemByTemplateID = client.Player.PropBag.GetItemByTemplateID(0, 11101);
			if (itemByTemplateID != null)
			{
				GSPacketIn gSPacketIn = new GSPacketIn(94, client.Player.PlayerId);
				gSPacketIn.WriteByte(19);
				gSPacketIn.WriteInt(client.Player.PlayerId);
				gSPacketIn.WriteString(client.Player.PlayerCharacter.NickName);
				gSPacketIn.WriteString(client.Player.CurrentRoom.GetNameByMapId());
				gSPacketIn.WriteInt(client.Player.CurrentRoom.RoomId);
				gSPacketIn.WriteString(client.Player.CurrentRoom.Password);
				client.Player.PropBag.RemoveCountFromStack(itemByTemplateID, 1);
				GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
				foreach (GamePlayer gamePlayer in allPlayers)
				{
					gSPacketIn.ClientID = gamePlayer.PlayerCharacter.ID;
					gamePlayer.Out.SendTCP(gSPacketIn);
				}
				return 0;
			}
			return 0;
		}
		default:
			Console.WriteLine("GameRoomHandler: " + (GameRoomPackageType)num);
			return 0;
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
}
