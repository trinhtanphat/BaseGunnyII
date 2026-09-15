using System;
using System.IO;
using System.Reflection;
using System.ServiceProcess;
using Game.Server;

namespace Game.Service;

public class GameServerService : ServiceBase
{
	public GameServerService()
	{
		base.ServiceName = "ROAD";
		base.AutoLog = false;
		base.CanHandlePowerEvent = false;
		base.CanPauseAndContinue = false;
		base.CanShutdown = true;
		base.CanStop = true;
	}

	private static bool StartServer()
	{
		FileInfo fileInfo = new FileInfo(Assembly.GetExecutingAssembly().Location);
		Directory.SetCurrentDirectory(fileInfo.DirectoryName);
		new FileInfo("./config/serverconfig.xml");
		GameServerConfig config = new GameServerConfig();
		GameServer.CreateInstance(config);
		return GameServer.Instance.Start();
	}

	private static void StopServer()
	{
		GameServer.Instance.Stop();
	}

	protected override void OnStart(string[] args)
	{
		if (!StartServer())
		{
			throw new ApplicationException("Failed to start server!");
		}
	}

	protected override void OnStop()
	{
		StopServer();
	}

	protected override void OnShutdown()
	{
		StopServer();
	}

	public static ServiceController GetDOLService()
	{
		ServiceController[] services = ServiceController.GetServices();
		foreach (ServiceController serviceController in services)
		{
			if (serviceController.ServiceName.ToLower().Equals("ROAD"))
			{
				return serviceController;
			}
		}
		return null;
	}
}
