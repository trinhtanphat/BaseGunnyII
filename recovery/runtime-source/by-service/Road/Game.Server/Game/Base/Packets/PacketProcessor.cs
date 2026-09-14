using System;
using System.Reflection;
using System.Threading;
using Game.Base.Events;
using Game.Server;
using Game.Server.Packets.Client;
using log4net;

namespace Game.Base.Packets;

public class PacketProcessor
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected IPacketHandler m_activePacketHandler;

	protected int m_handlerThreadID;

	protected GameClient m_client;

	protected static readonly IPacketHandler[] m_packetHandlers = new IPacketHandler[512];

	public PacketProcessor(GameClient client)
	{
		m_client = client;
	}

	public void HandlePacket(GSPacketIn packet)
	{
		int code = packet.Code;
		Statistics.BytesIn += packet.Length;
		Statistics.PacketsIn++;
		IPacketHandler packetHandler = null;
		if (code < m_packetHandlers.Length)
		{
			packetHandler = m_packetHandlers[code];
			try
			{
				packetHandler.ToString();
			}
			catch
			{
				Console.WriteLine("______________ERROR______________");
				Console.WriteLine("___ Received code: " + code + " <" + $"0x{code:x}" + "> ____");
				Console.WriteLine("_________________________________");
			}
		}
		else if (log.IsErrorEnabled)
		{
			log.ErrorFormat("Received packet code is outside of m_packetHandlers array bounds! " + m_client.ToString());
			log.Error(Marshal.ToHexDump(string.Format("===> <{2}> Packet 0x{0:X2} (0x{1:X2}) length: {3} (ThreadId={4})", code, code ^ 0xA8, m_client.TcpEndpoint, packet.Length, Thread.CurrentThread.ManagedThreadId), packet.Buffer));
		}
		if (packetHandler == null)
		{
			return;
		}
		long num = Environment.TickCount;
		try
		{
			if (m_client != null && packet != null && m_client.TcpEndpoint != "not connected")
			{
				packetHandler.HandlePacket(m_client, packet);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				string tcpEndpoint = m_client.TcpEndpoint;
				log.Error("Error while processing packet (handler=" + packetHandler.GetType().FullName + "  client: " + tcpEndpoint + ")", exception);
				log.Error(Marshal.ToHexDump("Package Buffer:", packet.Buffer, 0, packet.Length));
			}
		}
		long num2 = Environment.TickCount - num;
		m_activePacketHandler = null;
		if (log.IsDebugEnabled)
		{
			log.Debug("Package process Time:" + num2 + "ms!");
		}
		if (num2 > 1500)
		{
			string tcpEndpoint2 = m_client.TcpEndpoint;
			if (log.IsWarnEnabled)
			{
				log.Warn(string.Concat("(", tcpEndpoint2, ") Handle packet Thread ", Thread.CurrentThread.ManagedThreadId, " ", packetHandler, " took ", num2, "ms!"));
			}
		}
	}

	[ScriptLoadedEvent]
	public static void OnScriptCompiled(RoadEvent ev, object sender, EventArgs args)
	{
		Array.Clear(m_packetHandlers, 0, m_packetHandlers.Length);
		int num = SearchPacketHandlers("v168", Assembly.GetAssembly(typeof(GameServer)));
		if (log.IsInfoEnabled)
		{
			log.Info("PacketProcessor: Loaded " + num + " handlers from GameServer Assembly!");
		}
	}

	public static void RegisterPacketHandler(int packetCode, IPacketHandler handler)
	{
		m_packetHandlers[packetCode] = handler;
	}

	protected static int SearchPacketHandlers(string version, Assembly assembly)
	{
		int num = 0;
		Type[] types = assembly.GetTypes();
		foreach (Type type in types)
		{
			if (type.IsClass && type.GetInterface("Game.Server.Packets.Client.IPacketHandler") != null)
			{
				PacketHandlerAttribute[] array = (PacketHandlerAttribute[])type.GetCustomAttributes(typeof(PacketHandlerAttribute), inherit: true);
				if (array.Length > 0)
				{
					num++;
					RegisterPacketHandler(array[0].Code, (IPacketHandler)Activator.CreateInstance(type));
				}
			}
		}
		return num;
	}
}
