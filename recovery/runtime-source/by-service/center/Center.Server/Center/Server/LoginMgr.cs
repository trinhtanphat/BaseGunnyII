using System;
using System.Collections.Generic;
using System.Linq;

namespace Center.Server;

public class LoginMgr
{
	private static Dictionary<int, Player> m_players = new Dictionary<int, Player>();

	private static object syc_obj = new object();

	public static void CreatePlayer(Player player)
	{
		Player player2 = null;
		lock (syc_obj)
		{
			player.LastTime = DateTime.Now.Ticks;
			if (m_players.ContainsKey(player.Id))
			{
				player2 = m_players[player.Id];
				player.State = player2.State;
				player.CurrentServer = player2.CurrentServer;
				m_players[player.Id] = player;
			}
			else
			{
				player2 = GetPlayerByName(player.Name);
				if (player2 != null && m_players.ContainsKey(player2.Id))
				{
					m_players.Remove(player2.Id);
				}
				player.State = ePlayerState.NotLogin;
				m_players.Add(player.Id, player);
			}
		}
		if (player2 != null && player2.CurrentServer != null)
		{
			player2.CurrentServer.SendKitoffUser(player2.Id);
		}
	}

	public static bool TryLoginPlayer(int id, ServerClient server)
	{
		lock (syc_obj)
		{
			if (m_players.ContainsKey(id))
			{
				Player player = m_players[id];
				if (player.CurrentServer == null)
				{
					player.CurrentServer = server;
					player.State = ePlayerState.Logining;
					return true;
				}
				if (player.State == ePlayerState.Play)
				{
					player.CurrentServer.SendKitoffUser(id);
				}
				return false;
			}
			return false;
		}
	}

	public static void PlayerLogined(int id, ServerClient server)
	{
		lock (syc_obj)
		{
			if (m_players.ContainsKey(id))
			{
				Player player = m_players[id];
				if (player != null)
				{
					player.CurrentServer = server;
					player.State = ePlayerState.Play;
				}
			}
		}
	}

	public static void PlayerLoginOut(int id, ServerClient server)
	{
		lock (syc_obj)
		{
			if (m_players.ContainsKey(id))
			{
				Player player = m_players[id];
				if (player != null && player.CurrentServer == server)
				{
					player.CurrentServer = null;
					player.State = ePlayerState.NotLogin;
				}
			}
		}
	}

	public static Player GetPlayerByName(string name)
	{
		Player[] allPlayer = GetAllPlayer();
		if (allPlayer != null)
		{
			Player[] array = allPlayer;
			foreach (Player player in array)
			{
				if (player.Name == name)
				{
					return player;
				}
			}
		}
		return null;
	}

	public static Player[] GetAllPlayer()
	{
		lock (syc_obj)
		{
			return m_players.Values.ToArray();
		}
	}

	public static void RemovePlayer(int playerId)
	{
		lock (syc_obj)
		{
			if (m_players.ContainsKey(playerId))
			{
				m_players.Remove(playerId);
			}
		}
	}

	public static void RemovePlayer(List<Player> players)
	{
		lock (syc_obj)
		{
			foreach (Player player in players)
			{
				m_players.Remove(player.Id);
			}
		}
	}

	public static Player GetPlayer(int playerId)
	{
		lock (syc_obj)
		{
			if (m_players.ContainsKey(playerId))
			{
				return m_players[playerId];
			}
		}
		return null;
	}

	public static ServerClient GetServerClient(int playerId)
	{
		return GetPlayer(playerId)?.CurrentServer;
	}

	public static int GetOnlineCount()
	{
		Player[] allPlayer = GetAllPlayer();
		int num = 0;
		Player[] array = allPlayer;
		foreach (Player player in array)
		{
			if (player.State != ePlayerState.NotLogin)
			{
				num++;
			}
		}
		return num;
	}

	public static Dictionary<int, int> GetOnlineForLine()
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		Player[] allPlayer = GetAllPlayer();
		Player[] array = allPlayer;
		foreach (Player player in array)
		{
			if (player.CurrentServer != null)
			{
				if (dictionary.ContainsKey(player.CurrentServer.Info.ID))
				{
					dictionary[player.CurrentServer.Info.ID]++;
				}
				else
				{
					dictionary.Add(player.CurrentServer.Info.ID, 1);
				}
			}
		}
		return dictionary;
	}

	public static List<Player> GetServerPlayers(ServerClient server)
	{
		List<Player> list = new List<Player>();
		Player[] allPlayer = GetAllPlayer();
		Player[] array = allPlayer;
		foreach (Player player in array)
		{
			if (player.CurrentServer == server)
			{
				list.Add(player);
			}
		}
		return list;
	}
}
