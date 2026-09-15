using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bussiness;
using Bussiness.Managers;
using Fighting.Server.GameObjects;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Base;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using log4net;

namespace Fighting.Server;

public class ServerClient : BaseClient
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private RSACryptoServiceProvider m_rsa;

	private FightServer m_svr;

	private Dictionary<int, ProxyRoom> m_rooms = new Dictionary<int, ProxyRoom>();

	protected override void OnConnect()
	{
		base.OnConnect();
		m_rsa = new RSACryptoServiceProvider();
		RSAParameters rSAParameters = m_rsa.ExportParameters(includePrivateParameters: false);
		SendRSAKey(rSAParameters.Modulus, rSAParameters.Exponent);
	}

	protected override void OnDisconnect()
	{
		base.OnDisconnect();
		m_rsa = null;
	}

	public override void OnRecvPacket(GSPacketIn pkg)
	{
		switch (pkg.Code)
		{
		case 1:
			HandleLogin(pkg);
			break;
		case 2:
			HanleSendToGame(pkg);
			break;
		case 3:
			HandleSysNotice(pkg);
			break;
		case 19:
			HandlePlayerMessage(pkg);
			break;
		case 36:
			HandlePlayerUsingProp(pkg);
			break;
		case 64:
			HandleGameRoomCreate(pkg);
			break;
		case 65:
			HandleGameRoomCancel(pkg);
			break;
		case 77:
			HandleConsortiaAlly(pkg);
			break;
		case 83:
			HandlePlayerExit(pkg);
			break;
		}
	}

	private void HandlePlayerUsingProp(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame == null)
		{
			return;
		}
		baseGame.Resume();
		if (pkg.ReadBoolean())
		{
			Player player = baseGame.FindPlayer(pkg.Parameter1);
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(pkg.Parameter2);
			if (player != null && itemTemplateInfo != null)
			{
				player.UseItem(itemTemplateInfo);
			}
		}
	}

	private void HandlePlayerExit(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame == null)
		{
			return;
		}
		Player player = baseGame.FindPlayer(pkg.Parameter1);
		if (player != null)
		{
			GSPacketIn pkg2 = new GSPacketIn(83, player.PlayerDetail.PlayerCharacter.ID);
			baseGame.SendToAll(pkg2);
			baseGame.RemovePlayer(player.PlayerDetail, IsKick: false);
			ProxyRoom roomUnsafe = ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Red.RoomId);
			if (roomUnsafe != null && !roomUnsafe.RemovePlayer(player.PlayerDetail))
			{
				ProxyRoomMgr.GetRoomUnsafe((baseGame as BattleGame).Blue.RoomId)?.RemovePlayer(player.PlayerDetail);
			}
		}
	}

	public void HandleConsortiaAlly(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame != null)
		{
			baseGame.ConsortiaAlly = pkg.ReadInt();
			baseGame.RichesRate = pkg.ReadInt();
		}
	}

	private void HandleSysNotice(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame != null)
		{
			Player player = baseGame.FindPlayer(pkg.Parameter1);
			GSPacketIn gSPacketIn = new GSPacketIn(3);
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg6", player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
			player.PlayerDetail.SendTCP(gSPacketIn);
			gSPacketIn.ClearContext();
			gSPacketIn.WriteInt(3);
			gSPacketIn.WriteString(LanguageMgr.GetTranslation("AbstractPacketLib.SendGamePlayerLeave.Msg7", player.PlayerDetail.PlayerCharacter.NickName, player.PlayerDetail.PlayerCharacter.Grade * 12, 15));
			baseGame.SendToAll(gSPacketIn, player.PlayerDetail);
		}
	}

	private void HandlePlayerMessage(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame == null)
		{
			return;
		}
		Player player = baseGame.FindPlayer(pkg.ReadInt());
		bool flag = pkg.ReadBoolean();
		string str = pkg.ReadString();
		if (player != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(19);
			gSPacketIn.ClientID = player.PlayerDetail.PlayerCharacter.ID;
			gSPacketIn.WriteInt(4);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteBoolean(flag);
			gSPacketIn.WriteString(player.PlayerDetail.PlayerCharacter.NickName);
			gSPacketIn.WriteString(str);
			if (flag)
			{
				baseGame.SendToTeam(pkg, player.Team);
			}
			else
			{
				baseGame.SendToAll(gSPacketIn);
			}
		}
	}

	public void HandleLogin(GSPacketIn pkg)
	{
		byte[] rgb = pkg.ReadBytes();
		string[] array = Encoding.UTF8.GetString(m_rsa.Decrypt(rgb, fOAEP: false)).Split(',');
		if (array.Length == 2)
		{
			m_rsa = null;
			int.Parse(array[0]);
			base.Strict = false;
		}
		else
		{
			log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
			Disconnect();
		}
	}

	public void HandleGameRoomCreate(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		int roomType = pkg.ReadInt();
		int gameType = pkg.ReadInt();
		int guildId = pkg.ReadInt();
		int num2 = pkg.ReadInt();
		int num3 = 0;
		int num4 = 0;
		IGamePlayer[] array = new IGamePlayer[num2];
		for (int i = 0; i < num2; i++)
		{
			PlayerInfo playerInfo = new PlayerInfo();
			ProxyPlayerInfo proxyPlayerInfo = new ProxyPlayerInfo();
			playerInfo.ID = pkg.ReadInt();
			playerInfo.NickName = pkg.ReadString();
			playerInfo.Sex = pkg.ReadBoolean();
			playerInfo.typeVIP = pkg.ReadByte();
			playerInfo.VIPLevel = pkg.ReadInt();
			playerInfo.Hide = pkg.ReadInt();
			playerInfo.Style = pkg.ReadString();
			proxyPlayerInfo.FightFootballStyle = pkg.ReadString();
			playerInfo.Colors = pkg.ReadString();
			playerInfo.Skin = pkg.ReadString();
			playerInfo.Offer = pkg.ReadInt();
			playerInfo.GP = pkg.ReadInt();
			playerInfo.Grade = pkg.ReadInt();
			playerInfo.Repute = pkg.ReadInt();
			playerInfo.ConsortiaID = pkg.ReadInt();
			playerInfo.ConsortiaName = pkg.ReadString();
			playerInfo.ConsortiaLevel = pkg.ReadInt();
			playerInfo.ConsortiaRepute = pkg.ReadInt();
			playerInfo.badgeID = pkg.ReadInt();
			playerInfo.weaklessGuildProgress = Base64.decodeToByteArray(pkg.ReadString());
			playerInfo.Honor = pkg.ReadString();
			playerInfo.Attack = pkg.ReadInt();
			playerInfo.Defence = pkg.ReadInt();
			playerInfo.Agility = pkg.ReadInt();
			playerInfo.Luck = pkg.ReadInt();
			playerInfo.hp = pkg.ReadInt();
			playerInfo.FightPower = pkg.ReadInt();
			playerInfo.IsMarried = pkg.ReadBoolean();
			if (playerInfo.IsMarried)
			{
				playerInfo.SpouseID = pkg.ReadInt();
				playerInfo.SpouseName = pkg.ReadString();
			}
			num4 += playerInfo.FightPower;
			proxyPlayerInfo.BaseAttack = pkg.ReadDouble();
			proxyPlayerInfo.BaseDefence = pkg.ReadDouble();
			proxyPlayerInfo.BaseAgility = pkg.ReadDouble();
			proxyPlayerInfo.BaseBlood = pkg.ReadDouble();
			proxyPlayerInfo.TemplateId = pkg.ReadInt();
			proxyPlayerInfo.CanUserProp = pkg.ReadBoolean();
			proxyPlayerInfo.SecondWeapon = pkg.ReadInt();
			proxyPlayerInfo.StrengthLevel = pkg.ReadInt();
			proxyPlayerInfo.Healstone = pkg.ReadInt();
			proxyPlayerInfo.HealstoneCount = pkg.ReadInt();
			proxyPlayerInfo.GPAddPlus = pkg.ReadDouble();
			proxyPlayerInfo.OfferAddPlus = pkg.ReadDouble();
			proxyPlayerInfo.AntiAddictionRate = pkg.ReadDouble();
			proxyPlayerInfo.ServerId = pkg.ReadInt();
			proxyPlayerInfo.CanX2Exp = pkg.ReadBoolean();
			proxyPlayerInfo.CanX3Exp = pkg.ReadBoolean();
			UsersPetinfo usersPetinfo = new UsersPetinfo();
			int num5 = pkg.ReadInt();
			if (num5 == 1)
			{
				usersPetinfo.Place = pkg.ReadInt();
				usersPetinfo.TemplateID = pkg.ReadInt();
				usersPetinfo.ID = pkg.ReadInt();
				usersPetinfo.Name = pkg.ReadString();
				usersPetinfo.UserID = pkg.ReadInt();
				usersPetinfo.Level = pkg.ReadInt();
				usersPetinfo.Skill = pkg.ReadString();
				usersPetinfo.SkillEquip = pkg.ReadString();
			}
			else
			{
				usersPetinfo = null;
			}
			List<BufferInfo> list = new List<BufferInfo>();
			int num6 = pkg.ReadInt();
			for (int j = 0; j < num6; j++)
			{
				BufferInfo bufferInfo = new BufferInfo();
				bufferInfo.Type = pkg.ReadInt();
				bufferInfo.IsExist = pkg.ReadBoolean();
				bufferInfo.BeginDate = pkg.ReadDateTime();
				bufferInfo.ValidDate = pkg.ReadInt();
				bufferInfo.Value = pkg.ReadInt();
				bufferInfo.ValidCount = pkg.ReadInt();
				bufferInfo.TemplateID = pkg.ReadInt();
				if (playerInfo != null)
				{
					list.Add(bufferInfo);
				}
			}
			List<ItemInfo> list2 = new List<ItemInfo>();
			int num7 = pkg.ReadInt();
			for (int k = 0; k < num7; k++)
			{
				int templateId = pkg.ReadInt();
				int hole = pkg.ReadInt();
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(ItemMgr.FindItemTemplate(templateId), 1, 1);
				itemInfo.Hole1 = hole;
				list2.Add(itemInfo);
			}
			UserMatchInfo userMatchInfo = new UserMatchInfo();
			userMatchInfo.restCount = pkg.ReadInt();
			userMatchInfo.maxCount = pkg.ReadInt();
			List<BufferInfo> list3 = new List<BufferInfo>();
			int num8 = pkg.ReadInt();
			for (int l = 0; l < num8; l++)
			{
				BufferInfo bufferInfo2 = new BufferInfo();
				bufferInfo2.Type = pkg.ReadInt();
				bufferInfo2.Value = pkg.ReadInt();
				if (playerInfo != null)
				{
					list3.Add(bufferInfo2);
				}
			}
			num3 += playerInfo.Grade;
			array[i] = new ProxyPlayer(this, playerInfo, userMatchInfo, proxyPlayerInfo, usersPetinfo, list, list2, list3);
			array[i].CanUseProp = proxyPlayerInfo.CanUserProp;
			array[i].CanX2Exp = proxyPlayerInfo.CanX2Exp;
			array[i].CanX3Exp = proxyPlayerInfo.CanX3Exp;
		}
		ProxyRoom proxyRoom = new ProxyRoom(ProxyRoomMgr.NextRoomId(), num, array, this, num3, num4);
		proxyRoom.GuildId = guildId;
		proxyRoom.RoomType = (eRoomType)roomType;
		proxyRoom.GameType = (eGameType)gameType;
		lock (m_rooms)
		{
			if (!m_rooms.ContainsKey(num))
			{
				m_rooms.Add(num, proxyRoom);
			}
			else
			{
				proxyRoom = null;
			}
		}
		if (proxyRoom != null)
		{
			ProxyRoomMgr.AddRoom(proxyRoom);
		}
		else
		{
			log.WarnFormat("Room already exists:{0}", num);
		}
	}

	public void HandleGameRoomCancel(GSPacketIn pkg)
	{
		ProxyRoom proxyRoom = null;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(pkg.Parameter1))
			{
				proxyRoom = m_rooms[pkg.Parameter1];
			}
		}
		if (proxyRoom != null)
		{
			ProxyRoomMgr.RemoveRoom(proxyRoom);
		}
	}

	public void HanleSendToGame(GSPacketIn pkg)
	{
		BaseGame baseGame = GameMgr.FindGame(pkg.ClientID);
		if (baseGame != null)
		{
			GSPacketIn pkg2 = pkg.ReadPacket();
			baseGame.ProcessData(pkg2);
		}
	}

	public void SendRSAKey(byte[] m, byte[] e)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(0);
		gSPacketIn.Write(m);
		gSPacketIn.Write(e);
		SendTCP(gSPacketIn);
	}

	public void SendPacketToPlayer(int playerId, GSPacketIn pkg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(32, playerId);
		gSPacketIn.WritePacket(pkg);
		SendTCP(gSPacketIn);
	}

	public void SendRemoveRoom(int roomId)
	{
		GSPacketIn pkg = new GSPacketIn(65, roomId);
		SendTCP(pkg);
	}

	public void SendToRoom(int roomId, GSPacketIn pkg, IGamePlayer except)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(67, roomId);
		if (except != null)
		{
			gSPacketIn.Parameter1 = except.PlayerCharacter.ID;
			gSPacketIn.Parameter2 = except.GamePlayerId;
		}
		else
		{
			gSPacketIn.Parameter1 = 0;
			gSPacketIn.Parameter2 = 0;
		}
		gSPacketIn.WritePacket(pkg);
		SendTCP(gSPacketIn);
	}

	public void SendStartGame(int roomId, AbstractGame game)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(66);
		gSPacketIn.Parameter1 = roomId;
		gSPacketIn.Parameter2 = game.Id;
		gSPacketIn.WriteInt((int)game.RoomType);
		gSPacketIn.WriteInt((int)game.GameType);
		gSPacketIn.WriteInt(game.TimeType);
		SendTCP(gSPacketIn);
	}

	public void SendStopGame(int roomId, int gameId)
	{
		SendTCP(new GSPacketIn(68)
		{
			Parameter1 = roomId,
			Parameter2 = gameId
		});
	}

	public void SendGamePlayerId(IGamePlayer player)
	{
		SendTCP(new GSPacketIn(33)
		{
			Parameter1 = player.PlayerCharacter.ID,
			Parameter2 = player.GamePlayerId
		});
	}

	public void SendDisconnectPlayer(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(34, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerOnGameOver(int playerId, int gameId, bool isWin, int gainXp)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(35, playerId);
		gSPacketIn.Parameter1 = gameId;
		gSPacketIn.WriteBoolean(isWin);
		gSPacketIn.WriteInt(gainXp);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerUsePropInGame(int playerId, int bag, int place, int templateId, bool isLiving)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(36, playerId);
		gSPacketIn.Parameter1 = bag;
		gSPacketIn.Parameter2 = place;
		gSPacketIn.WriteInt(templateId);
		gSPacketIn.WriteBoolean(isLiving);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddGold(int playerId, int value)
	{
		SendTCP(new GSPacketIn(38, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendFootballTakeOut(int playerId, bool isWin)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(87, playerId);
		gSPacketIn.WriteBoolean(isWin);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddMoney(int playerId, int value)
	{
		SendTCP(new GSPacketIn(74, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerAddActiveMoney(int playerId, int value)
	{
		SendTCP(new GSPacketIn(89, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerAddGiftToken(int playerId, int value)
	{
		SendTCP(new GSPacketIn(75, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerRemoveHealstone(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(73, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerAddMedal(int playerId, int value)
	{
		SendTCP(new GSPacketIn(76, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerAddLeagueMoney(int playerId, int value)
	{
		SendTCP(new GSPacketIn(84, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerAddPrestige(int playerId, bool isWin)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(85, playerId);
		gSPacketIn.WriteBoolean(isWin);
		SendTCP(gSPacketIn);
	}

	public void SendUpdateRestCount(int playerId)
	{
		GSPacketIn pkg = new GSPacketIn(86, playerId);
		SendTCP(pkg);
	}

	public void SendPlayerAddGP(int playerId, int value)
	{
		SendTCP(new GSPacketIn(39, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerRemoveGP(int playerId, int value)
	{
		SendTCP(new GSPacketIn(49, playerId)
		{
			Parameter1 = value
		});
	}

	public void SendPlayerOnKillingLiving(int playerId, AbstractGame game, int type, int id, bool isLiving, int demage)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(40, playerId);
		gSPacketIn.WriteInt(type);
		gSPacketIn.WriteBoolean(isLiving);
		gSPacketIn.WriteInt(demage);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerOnMissionOver(int playerId, AbstractGame game, bool isWin, int MissionID, int turnNum)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(41, playerId);
		gSPacketIn.WriteBoolean(isWin);
		gSPacketIn.WriteInt(MissionID);
		gSPacketIn.WriteInt(turnNum);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerConsortiaFight(int playerId, int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(42, playerId);
		gSPacketIn.WriteInt(consortiaWin);
		gSPacketIn.WriteInt(consortiaLose);
		gSPacketIn.WriteInt(players.Count);
		for (int i = 0; i < players.Count; i++)
		{
			gSPacketIn.WriteInt(players[i].PlayerDetail.PlayerCharacter.ID);
		}
		gSPacketIn.WriteByte((byte)roomType);
		gSPacketIn.WriteByte((byte)gameClass);
		gSPacketIn.WriteInt(totalKillHealth);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerSendConsortiaFight(int playerId, int consortiaID, int riches, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(43, playerId);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(riches);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveGold(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(44, playerId);
		gSPacketIn.WriteInt(value);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveMoney(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(45, playerId);
		gSPacketIn.WriteInt(value);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerRemoveOffer(int playerId, int value)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(50, playerId);
		gSPacketIn.WriteInt(value);
		SendTCP(gSPacketIn);
	}

	public void SendPlayerAddTemplate(int playerId, ItemInfo cloneItem, eBageType bagType, int count)
	{
		if (cloneItem != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(48, playerId);
			gSPacketIn.WriteInt(cloneItem.TemplateID);
			gSPacketIn.WriteByte((byte)bagType);
			gSPacketIn.WriteInt(count);
			gSPacketIn.WriteInt(cloneItem.ValidDate);
			gSPacketIn.WriteBoolean(cloneItem.IsBinds);
			gSPacketIn.WriteBoolean(cloneItem.IsUsed);
			SendTCP(gSPacketIn);
		}
	}

	public void SendConsortiaAlly(int Consortia1, int Consortia2, int GameId)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(77);
		gSPacketIn.WriteInt(Consortia1);
		gSPacketIn.WriteInt(Consortia2);
		gSPacketIn.WriteInt(GameId);
		SendTCP(gSPacketIn);
	}

	public void SendBeginFightNpc(int GameId, int RoomType, int GameType)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(88);
		gSPacketIn.WriteInt(GameId);
		gSPacketIn.WriteInt(RoomType);
		gSPacketIn.WriteInt(GameType);
		SendTCP(gSPacketIn);
	}

	public ServerClient(FightServer svr)
		: base(new byte[8192], new byte[8192])
	{
		m_svr = svr;
	}

	public override string ToString()
	{
		return $"Server Client: {0} IsConnected:{base.IsConnected}  RoomCount:{m_rooms.Count}";
	}

	public void RemoveRoom(int orientId, ProxyRoom room)
	{
		bool flag = false;
		lock (m_rooms)
		{
			if (m_rooms.ContainsKey(orientId) && m_rooms[orientId] == room)
			{
				flag = m_rooms.Remove(orientId);
			}
		}
		if (flag)
		{
			SendRemoveRoom(orientId);
		}
	}
}
