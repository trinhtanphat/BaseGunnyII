using System;
using System.Collections;
using System.Configuration;
using System.Reflection;
using Bussiness.Protocol;
using Center.Server;
using Game.Base;
using log4net;

namespace Game.Service.actions;

public class ConsoleStart : IAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public string HelpStr => ConfigurationManager.AppSettings["HelpStr"];

	public string Name => "--start";

	public string Syntax => "--start [-config=./config/serverconfig.xml]";

	public string Description => "Starts the DOL server in console mode";

	private static bool StartServer()
	{
		Console.WriteLine("Starting the server");
		return CenterServer.Instance.Start();
	}

	public void OnAction(Hashtable parameters)
	{
		Console.WriteLine("GUNNY II fix by trinhtanphat!");
		Console.WriteLine("Starting GameServer ... please wait a moment!");
		CenterServer.CreateInstance(new CenterServerConfig());
		StartServer();
		ConsoleClient client = new ConsoleClient();
		bool flag = true;
		while (flag)
		{
			try
			{
				Console.Write("> ");
				string text = Console.ReadLine();
				string[] array = text.Split('&');
				string text2;
				switch (text2 = array[0].ToLower())
				{
				case "exit":
					flag = false;
					continue;
				case "notice":
					if (array.Length < 2)
					{
						Console.WriteLine("公告需要公告内容,用&隔开!");
					}
					else
					{
						CenterServer.Instance.SendSystemNotice(array[1]);
					}
					continue;
				case "reload":
					if (array.Length < 2)
					{
						Console.WriteLine("加载需要指定表,用&隔开!");
					}
					else
					{
						CenterServer.Instance.SendReload(array[1]);
					}
					continue;
				case "shutdown":
					CenterServer.Instance.SendShutdown();
					continue;
				case "help":
					Console.WriteLine(HelpStr);
					continue;
				case "AAS":
					if (array.Length < 2)
					{
						Console.WriteLine("加载需要指定状态true or false,用&隔开!");
					}
					else
					{
						CenterServer.Instance.SendAAS(bool.Parse(array[1]));
					}
					continue;
				}
				if (text.Length <= 0)
				{
					continue;
				}
				if (text[0] == '/')
				{
					text = text.Remove(0, 1);
					text = text.Insert(0, "&");
				}
				try
				{
					if (!CommandMgr.HandleCommandNoPlvl(client, text))
					{
						Console.WriteLine("Unknown command: " + text);
					}
				}
				catch (Exception ex)
				{
					Console.WriteLine(ex.ToString());
				}
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Error:" + ex2.ToString());
			}
		}
		if (CenterServer.Instance != null)
		{
			CenterServer.Instance.Stop();
		}
	}

	public void Reload(eReloadType type)
	{
	}
}
