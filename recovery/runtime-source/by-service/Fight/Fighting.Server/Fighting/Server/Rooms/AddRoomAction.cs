namespace Fighting.Server.Rooms;

public class AddRoomAction : IAction
{
	private ProxyRoom m_room;

	public AddRoomAction(ProxyRoom room)
	{
		m_room = room;
	}

	public void Execute()
	{
		ProxyRoomMgr.AddRoomUnsafe(m_room);
	}
}
