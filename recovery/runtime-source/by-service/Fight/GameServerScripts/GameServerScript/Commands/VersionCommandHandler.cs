using Game.Base;

namespace GameServerScript.Commands;

[Cmd("&version", ePrivLevel.Player, "Get the version of the GameServer", new string[] { "/version" })]
public class VersionCommandHandler : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		return true;
	}
}
