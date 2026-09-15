using System;
using System.Reflection;
using Game.Base.Packets;
using Game.Logic.Cmd;
using Game.Logic.Phy.Object;
using log4net;

namespace Game.Logic.Actions;

public class ProcessPacketAction : IAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private Player m_player;

	private GSPacketIn m_packet;

	public ProcessPacketAction(Player player, GSPacketIn pkg)
	{
		m_player = player;
		m_packet = pkg;
	}

	public void Execute(BaseGame game, long tick)
	{
		if (!m_player.IsActive)
		{
			return;
		}
		eTankCmdType eTankCmdType2 = (eTankCmdType)m_packet.ReadByte();
		try
		{
			ICommandHandler commandHandler = CommandMgr.LoadCommandHandler((int)eTankCmdType2);
			if (commandHandler != null)
			{
				commandHandler.HandleCommand(game, m_player, m_packet);
			}
			else
			{
				log.Error($"Player Id: {m_player.Id}");
			}
		}
		catch (Exception exception)
		{
			log.Error($"Player Id: {m_player.Id}  cmd:0x{(byte)eTankCmdType2:X2}", exception);
		}
	}

	public bool IsFinished(long tick)
	{
		return true;
	}
}
