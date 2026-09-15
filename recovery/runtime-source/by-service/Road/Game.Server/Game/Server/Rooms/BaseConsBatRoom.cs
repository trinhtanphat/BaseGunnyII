using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;

namespace Game.Server.Rooms;

public class BaseConsBatRoom
{
	private Dictionary<int, GamePlayer> m_players;

	private Dictionary<int, ConsortiaBattlePlayerInfo> m_consortiaBattlePlayerInfo;

	private Point[] brithPoint = new Point[8]
	{
		new Point(353, 570),
		new Point(246, 760),
		new Point(593, 590),
		new Point(466, 898),
		new Point(800, 950),
		new Point(946, 748),
		new Point(1152, 873),
		new Point(1172, 874)
	};

	public BaseConsBatRoom()
	{
		m_players = new Dictionary<int, GamePlayer>();
		m_consortiaBattlePlayerInfo = new Dictionary<int, ConsortiaBattlePlayerInfo>();
	}

	public ConsortiaBattlePlayerInfo CreateConsortiaBattlePlayerInfo(GamePlayer player)
	{
		ConsortiaBattlePlayerInfo consortiaBattlePlayerInfo = new ConsortiaBattlePlayerInfo();
		consortiaBattlePlayerInfo.PlayerID = player.PlayerCharacter.ID;
		consortiaBattlePlayerInfo.Sex = player.PlayerCharacter.Sex;
		consortiaBattlePlayerInfo.curHp = player.PlayerCharacter.hp;
		consortiaBattlePlayerInfo.posX = 631;
		consortiaBattlePlayerInfo.posY = 959;
		consortiaBattlePlayerInfo.consortiaID = player.PlayerCharacter.ConsortiaID;
		consortiaBattlePlayerInfo.consortiaName = player.PlayerCharacter.ConsortiaName;
		consortiaBattlePlayerInfo.tombstoneEndTime = DateTime.Now.AddMinutes(-10.0);
		consortiaBattlePlayerInfo.status = 1;
		consortiaBattlePlayerInfo.victoryCount = 0;
		consortiaBattlePlayerInfo.winningStreak = 0;
		consortiaBattlePlayerInfo.failBuffCount = 0;
		consortiaBattlePlayerInfo.score = 0;
		consortiaBattlePlayerInfo.isPowerFullUsed = false;
		consortiaBattlePlayerInfo.isDoubleScoreUsed = false;
		return consortiaBattlePlayerInfo;
	}

	public bool AddPlayer(GamePlayer player)
	{
		bool result = true;
		lock (m_players)
		{
			if (!m_players.ContainsKey(player.PlayerId))
			{
				m_players.Add(player.PlayerId, player);
			}
		}
		ConsortiaBattlePlayerInfo consortiaBattlePlayerInfo = CreateConsortiaBattlePlayerInfo(player);
		lock (m_consortiaBattlePlayerInfo)
		{
			if (!m_consortiaBattlePlayerInfo.ContainsKey(player.PlayerId))
			{
				m_consortiaBattlePlayerInfo.Add(player.PlayerId, consortiaBattlePlayerInfo);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(153, player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(2);
		gSPacketIn.WriteBoolean(val: true);
		gSPacketIn.WriteDateTime(consortiaBattlePlayerInfo.tombstoneEndTime);
		gSPacketIn.WriteInt(consortiaBattlePlayerInfo.posX);
		gSPacketIn.WriteInt(consortiaBattlePlayerInfo.posY);
		gSPacketIn.WriteInt(consortiaBattlePlayerInfo.curHp);
		gSPacketIn.WriteInt(consortiaBattlePlayerInfo.victoryCount);
		gSPacketIn.WriteInt(consortiaBattlePlayerInfo.winningStreak);
		gSPacketIn.WriteInt(consortiaBattlePlayerInfo.score);
		gSPacketIn.WriteBoolean(consortiaBattlePlayerInfo.isDoubleScoreUsed);
		gSPacketIn.WriteBoolean(consortiaBattlePlayerInfo.isPowerFullUsed);
		player.SendTCP(gSPacketIn);
		return result;
	}

	public bool RemovePlayer(GamePlayer player)
	{
		bool flag = false;
		lock (m_players)
		{
			flag = m_players.Remove(player.PlayerId) && m_consortiaBattlePlayerInfo.Remove(player.PlayerId);
		}
		if (flag)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(153);
			gSPacketIn.WriteByte(5);
			gSPacketIn.WriteInt(player.PlayerCharacter.ID);
			player.SendTCP(gSPacketIn);
			SendToALL(gSPacketIn, player);
		}
		return true;
	}

	public void SendUpdateRoom(GamePlayer player)
	{
		int count = m_consortiaBattlePlayerInfo.Count;
		GSPacketIn gSPacketIn = new GSPacketIn(153);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(count);
		foreach (ConsortiaBattlePlayerInfo value in m_consortiaBattlePlayerInfo.Values)
		{
			gSPacketIn.WriteInt(value.PlayerID);
			gSPacketIn.WriteDateTime(value.tombstoneEndTime);
			gSPacketIn.WriteByte(value.status);
			gSPacketIn.WriteInt(value.posX);
			gSPacketIn.WriteInt(value.posY);
			gSPacketIn.WriteBoolean(value.Sex);
			gSPacketIn.WriteInt(value.consortiaID);
			gSPacketIn.WriteString(value.consortiaName);
			gSPacketIn.WriteInt(value.winningStreak);
			gSPacketIn.WriteInt(value.failBuffCount);
		}
		player.SendTCP(gSPacketIn);
	}

	public void Challenge(int PlayerId, int ChallengeId)
	{
		lock (m_consortiaBattlePlayerInfo)
		{
			GamePlayer gamePlayer = null;
			GamePlayer gamePlayer2 = null;
			if (m_consortiaBattlePlayerInfo.ContainsKey(ChallengeId) && m_players.ContainsKey(ChallengeId) && m_consortiaBattlePlayerInfo[ChallengeId].status == 1)
			{
				m_consortiaBattlePlayerInfo[ChallengeId].status = 2;
				SendUpdatePlayerStatus(m_consortiaBattlePlayerInfo[ChallengeId]);
				gamePlayer2 = m_players[ChallengeId];
				gamePlayer2.isPowerFullUsed = m_consortiaBattlePlayerInfo[ChallengeId].isPowerFullUsed;
				gamePlayer2.winningStreak = m_consortiaBattlePlayerInfo[ChallengeId].winningStreak;
				gamePlayer2.PlayerCharacter.hp = m_consortiaBattlePlayerInfo[PlayerId].curHp;
				gamePlayer2.CurrentRoomTeam = 2;
			}
			if (m_consortiaBattlePlayerInfo.ContainsKey(PlayerId) && m_players.ContainsKey(PlayerId) && m_consortiaBattlePlayerInfo[PlayerId].status == 1)
			{
				m_consortiaBattlePlayerInfo[PlayerId].status = 2;
				SendUpdatePlayerStatus(m_consortiaBattlePlayerInfo[PlayerId]);
				gamePlayer = m_players[PlayerId];
				gamePlayer.isPowerFullUsed = m_consortiaBattlePlayerInfo[PlayerId].isPowerFullUsed;
				gamePlayer.winningStreak = m_consortiaBattlePlayerInfo[PlayerId].winningStreak;
				gamePlayer.PlayerCharacter.hp = m_consortiaBattlePlayerInfo[PlayerId].curHp;
				gamePlayer.CurrentRoomTeam = 1;
			}
			if (gamePlayer != null && gamePlayer2 != null)
			{
				RoomMgr.CreateConsortiaBattleRoom(gamePlayer, gamePlayer2);
			}
		}
	}

	public void SendUpdatePlayerStatus(ConsortiaBattlePlayerInfo info)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(153, info.PlayerID);
		gSPacketIn.WriteByte(7);
		gSPacketIn.WriteInt(info.PlayerID);
		gSPacketIn.WriteDateTime(info.tombstoneEndTime);
		gSPacketIn.WriteByte(info.status);
		gSPacketIn.WriteInt(info.posX);
		gSPacketIn.WriteInt(info.posY);
		gSPacketIn.WriteInt(info.winningStreak);
		gSPacketIn.WriteInt(info.failBuffCount);
		SendToALL(gSPacketIn);
	}

	public void PlayerMove(int PosX, int PosY, int PlayerId)
	{
		lock (m_consortiaBattlePlayerInfo)
		{
			if (m_consortiaBattlePlayerInfo.ContainsKey(PlayerId))
			{
				m_consortiaBattlePlayerInfo[PlayerId].posX = PosX;
				m_consortiaBattlePlayerInfo[PlayerId].posY = PosY;
			}
		}
	}

	public void BattleWin(int WinPlayerId, int lostwinningStreak, int curHp)
	{
		int num = 30;
		lock (m_consortiaBattlePlayerInfo)
		{
			if (m_consortiaBattlePlayerInfo.ContainsKey(WinPlayerId))
			{
				m_consortiaBattlePlayerInfo[WinPlayerId].status = 1;
				m_consortiaBattlePlayerInfo[WinPlayerId].tombstoneEndTime = DateTime.Now.AddSeconds(-1.0);
				m_consortiaBattlePlayerInfo[WinPlayerId].curHp = curHp;
				m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak++;
				m_consortiaBattlePlayerInfo[WinPlayerId].victoryCount++;
				if (m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak == 3)
				{
					num = 50;
				}
				else if (m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak == 6)
				{
					num = 70;
				}
				else if (m_consortiaBattlePlayerInfo[WinPlayerId].winningStreak == 10)
				{
					num = 110;
				}
				if (m_consortiaBattlePlayerInfo[WinPlayerId].isDoubleScoreUsed)
				{
					num *= 2;
					m_consortiaBattlePlayerInfo[WinPlayerId].isDoubleScoreUsed = false;
				}
				m_consortiaBattlePlayerInfo[WinPlayerId].score += num;
				if (lostwinningStreak >= 3 && lostwinningStreak < 6)
				{
					m_consortiaBattlePlayerInfo[WinPlayerId].score += 50;
				}
				else if (lostwinningStreak >= 6 && lostwinningStreak < 10)
				{
					m_consortiaBattlePlayerInfo[WinPlayerId].score += 70;
				}
				else if (lostwinningStreak >= 10)
				{
					m_consortiaBattlePlayerInfo[WinPlayerId].score += 90;
				}
				m_consortiaBattlePlayerInfo[WinPlayerId].isPowerFullUsed = false;
			}
		}
	}

	public void BattleLost(int LostPlayerId)
	{
		int num = 5;
		lock (m_consortiaBattlePlayerInfo)
		{
			if (m_consortiaBattlePlayerInfo.ContainsKey(LostPlayerId))
			{
				int winningStreak = m_consortiaBattlePlayerInfo[LostPlayerId].winningStreak;
				m_consortiaBattlePlayerInfo[LostPlayerId].status = 1;
				m_consortiaBattlePlayerInfo[LostPlayerId].winningStreak = 0;
				m_consortiaBattlePlayerInfo[LostPlayerId].tombstoneEndTime = DateTime.Now.AddSeconds(15.0);
				m_consortiaBattlePlayerInfo[LostPlayerId].posX = 631;
				m_consortiaBattlePlayerInfo[LostPlayerId].posY = 959;
				if (m_consortiaBattlePlayerInfo[LostPlayerId].isDoubleScoreUsed)
				{
					num *= 2;
					m_consortiaBattlePlayerInfo[LostPlayerId].isDoubleScoreUsed = false;
				}
				m_consortiaBattlePlayerInfo[LostPlayerId].isPowerFullUsed = false;
				m_consortiaBattlePlayerInfo[LostPlayerId].score += num;
			}
		}
	}

	public void SendConfirmEnterRoom(GamePlayer player)
	{
		int count = m_consortiaBattlePlayerInfo.Count;
		GSPacketIn gSPacketIn = new GSPacketIn(153, player.PlayerCharacter.ID);
		gSPacketIn.WriteByte(3);
		gSPacketIn.WriteInt(count);
		foreach (ConsortiaBattlePlayerInfo value in m_consortiaBattlePlayerInfo.Values)
		{
			gSPacketIn.WriteInt(value.PlayerID);
			gSPacketIn.WriteDateTime(value.tombstoneEndTime);
			gSPacketIn.WriteByte(value.status);
			gSPacketIn.WriteInt(value.posX);
			gSPacketIn.WriteInt(value.posY);
			gSPacketIn.WriteBoolean(value.Sex);
			gSPacketIn.WriteInt(value.consortiaID);
			gSPacketIn.WriteString(value.consortiaName);
			gSPacketIn.WriteInt(value.winningStreak);
			gSPacketIn.WriteInt(value.failBuffCount);
		}
		SendToALL(gSPacketIn, player);
	}

	public void SendToALL(GSPacketIn packet)
	{
		SendToALL(packet, null);
	}

	public void SendToALL(GSPacketIn packet, GamePlayer except)
	{
		GamePlayer[] array = null;
		lock (m_players)
		{
			array = new GamePlayer[m_players.Count];
			m_players.Values.CopyTo(array, 0);
		}
		if (array == null)
		{
			return;
		}
		GamePlayer[] array2 = array;
		foreach (GamePlayer gamePlayer in array2)
		{
			if (gamePlayer != null && gamePlayer != except)
			{
				gamePlayer.Out.SendTCP(packet);
			}
		}
	}

	public GamePlayer[] GetPlayersSafe()
	{
		GamePlayer[] array = null;
		lock (m_players)
		{
			array = new GamePlayer[m_players.Count];
			m_players.Values.CopyTo(array, 0);
		}
		if (array != null)
		{
			return array;
		}
		return new GamePlayer[0];
	}
}
