using Game.Base;
using Game.Base.Packets;
using Game.Logic;

namespace Game.Server.Battle;

public class ProxyGame : AbstractGame
{
	private FightServerConnector m_fightingServer;

	public ProxyGame(int id, FightServerConnector fightServer, eRoomType roomType, eGameType gameType, int timeType)
		: base(id, roomType, gameType, timeType)
	{
		m_fightingServer = fightServer;
		m_fightingServer.Disconnected += m_fightingServer_Disconnected;
	}

	private void m_fightingServer_Disconnected(BaseClient client)
	{
		Stop();
	}

	public override void ProcessData(GSPacketIn pkg)
	{
		m_fightingServer.SendToGame(base.Id, pkg);
	}
}
