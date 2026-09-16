using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.Reflection;
using Fighting.Server;
using Fighting.Server.Games;
using Fighting.Server.Rooms;
using Game.Logic;
using log4net;

namespace Fighting.Service.action;

public class ConsoleStart : IAction
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public string HelpStr => ConfigurationManager.AppSettings["HelpStr"];

	public string Name => "--start";

	public string Syntax => "--start [-config=./config/serverconfig.xml]";

	public string Description => "Starts the Fighting server in console mode";

	public void OnAction(Hashtable parameters)
	{
		Console.WriteLine("GUNNY II fix by trinhtanphat!");
		Console.WriteLine("Starting FightingServer ... please wait a moment!");
		FightServerConfig fightServerConfig = new FightServerConfig();
		try
		{
			fightServerConfig.LoadConfiguration();
		}
		catch (Exception ex)
		{
			Console.WriteLine(ex.Message);
			Console.ReadKey();
			return;
		}
		FightServer.CreateInstance(fightServerConfig);
		FightServer.Instance.Start();
		bool flag = true;
		while (flag)
		{
			try
			{
				Console.Write("> ");
				string text = Console.ReadLine();
				string[] array = text.Split(' ');
				switch (array[0].ToLower())
				{
				case "exit":
					flag = false;
					break;
				case "list":
					if (array.Length > 1)
					{
						switch (array[1])
						{
						case "-game":
						{
							Console.WriteLine("game list:");
							Console.WriteLine("-------------------------------");
							List<BaseGame> games = GameMgr.GetGames();
							foreach (BaseGame item in games)
							{
								Console.WriteLine(item.ToString());
							}
							Console.WriteLine("-------------------------------");
							break;
						}
						case "-room":
						{
							Console.WriteLine("room list:");
							Console.WriteLine("-------------------------------");
							ProxyRoom[] allRoom = ProxyRoomMgr.GetAllRoom();
							ProxyRoom[] array3 = allRoom;
							foreach (ProxyRoom proxyRoom in array3)
							{
								Console.WriteLine(proxyRoom.ToString());
							}
							Console.WriteLine("-------------------------------");
							break;
						}
						case "-client":
						{
							Console.WriteLine("server client list:");
							Console.WriteLine("--------------------");
							ServerClient[] allClients = FightServer.Instance.GetAllClients();
							ServerClient[] array2 = allClients;
							foreach (ServerClient serverClient in array2)
							{
								Console.WriteLine(serverClient.ToString());
							}
							Console.WriteLine("-------------------");
							break;
						}
						}
					}
					else
					{
						Console.WriteLine("list [-client][-room][-game]");
					}
					break;
				case "clear":
					Console.Clear();
					break;
				}
			}
			catch (Exception ex2)
			{
				Console.WriteLine("Error:" + ex2.ToString());
			}
		}
		if (FightServer.Instance != null)
		{
			FightServer.Instance.Stop();
		}
	}
}
