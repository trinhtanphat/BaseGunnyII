using System.Threading;
using Game.Base.Packets;
using Game.Logic.Phy.Object;

namespace Game.Logic;

public class AbstractGame
{
	private int m_id;

	protected eRoomType m_roomType;

	protected eGameType m_gameType;

	protected eMapType m_mapType;

	protected int m_timeType;

	private int m_disposed;

	public int Id => m_id;

	public eRoomType RoomType => m_roomType;

	public eGameType GameType => m_gameType;

	public int TimeType => m_timeType;

	public event GameEventHandle GameStarted;

	public event GameEventHandle GameStopped;

	public AbstractGame(int id, eRoomType roomType, eGameType gameType, int timeType)
	{
		m_id = id;
		m_roomType = roomType;
		m_gameType = gameType;
		m_timeType = timeType;
		switch (m_roomType)
		{
		case eRoomType.Match:
			m_mapType = eMapType.PairUp;
			break;
		case eRoomType.Freedom:
			m_mapType = eMapType.Normal;
			break;
		default:
			m_mapType = eMapType.Normal;
			break;
		}
	}

	public virtual void Start()
	{
		OnGameStarted();
	}

	public virtual void Stop()
	{
		OnGameStopped();
	}

	public virtual bool CanAddPlayer()
	{
		return false;
	}

	public virtual void Pause(int time)
	{
	}

	public virtual void Resume()
	{
	}

	public virtual void MissionStart(IGamePlayer host)
	{
	}

	public virtual void ProcessData(GSPacketIn pkg)
	{
	}

	public virtual Player AddPlayer(IGamePlayer player)
	{
		return null;
	}

	public virtual Player RemovePlayer(IGamePlayer player, bool IsKick)
	{
		return null;
	}

	public void Dispose()
	{
		if (Interlocked.Exchange(ref m_disposed, 1) == 0)
		{
			Dispose(disposing: true);
		}
	}

	protected virtual void Dispose(bool disposing)
	{
	}

	protected void OnGameStarted()
	{
		if (GameStarted != null)
		{
			GameStarted(this);
		}
	}

	protected void OnGameStopped()
	{
		if (GameStopped != null)
		{
			GameStopped(this);
		}
	}
}
