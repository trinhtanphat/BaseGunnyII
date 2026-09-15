using Game.Base;

namespace GameServerScript.Commands;

[Cmd("&changetask", ePrivLevel.Admin, "Test", new string[] { "changetast...." })]
public class TestTaskCommand : AbstractCommandHandler, ICommandHandler
{
	public bool OnCommand(BaseClient client, string[] args)
	{
		foreach (string message in args)
		{
			DisplayMessage(client, message);
		}
		return true;
	}
}
