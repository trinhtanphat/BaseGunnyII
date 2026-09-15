using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class BallMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, BallInfo> m_infos;

	private static Dictionary<int, Tile> m_tiles;

	public static bool Init()
	{
		return ReLoad();
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, BallInfo> dictionary = LoadFromDatabase();
			Dictionary<int, Tile> dictionary2 = LoadFromFiles(dictionary);
			if (dictionary.Values.Count > 0 && dictionary2.Values.Count > 0)
			{
				Interlocked.Exchange(ref m_infos, dictionary);
				Interlocked.Exchange(ref m_tiles, dictionary2);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("Ball Mgr init error:", exception);
		}
		return false;
	}

	private static Dictionary<int, BallInfo> LoadFromDatabase()
	{
		Dictionary<int, BallInfo> dictionary = new Dictionary<int, BallInfo>();
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			BallInfo[] allBall = produceBussiness.GetAllBall();
			BallInfo[] array = allBall;
			foreach (BallInfo ballInfo in array)
			{
				if (!dictionary.ContainsKey(ballInfo.ID))
				{
					dictionary.Add(ballInfo.ID, ballInfo);
				}
			}
		}
		return dictionary;
	}

	private static Dictionary<int, Tile> LoadFromFiles(Dictionary<int, BallInfo> list)
	{
		Dictionary<int, Tile> dictionary = new Dictionary<int, Tile>();
		foreach (BallInfo value in list.Values)
		{
			if (value.HasTunnel)
			{
				string text = $"bomb\\{value.ID}.bomb";
				Tile tile = null;
				if (File.Exists(text))
				{
					tile = new Tile(text, digable: false);
				}
				dictionary.Add(value.ID, tile);
				if (tile == null && value.ID != 1 && value.ID != 2 && value.ID != 3)
				{
					log.ErrorFormat("can't find bomb file:{0}", text);
				}
			}
		}
		return dictionary;
	}

	public static BallInfo FindBall(int id)
	{
		if (m_infos.ContainsKey(id))
		{
			return m_infos[id];
		}
		return null;
	}

	public static Tile FindTile(int id)
	{
		if (m_tiles.ContainsKey(id))
		{
			return m_tiles[id];
		}
		return null;
	}

	public static BombType GetBallType(int ballId)
	{
		if (ballId <= 59)
		{
			switch (ballId)
			{
			case 2:
			case 4:
				return BombType.Normal;
			case 3:
				return BombType.FLY;
			case 5:
				return BombType.CURE;
			default:
				return BombType.Normal;
			case 59:
				return BombType.CURE;
			case 1:
			case 56:
				break;
			}
		}
		else
		{
			switch (ballId)
			{
			case 64:
				return BombType.CURE;
			case 97:
			case 98:
				return BombType.CURE;
			default:
				return BombType.Normal;
			case 10009:
				return BombType.CURE;
			case 99:
				break;
			}
		}
		return BombType.FORZEN;
	}
}
