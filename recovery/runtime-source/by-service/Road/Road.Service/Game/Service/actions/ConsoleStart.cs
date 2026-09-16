using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base;
using Game.Logic;
using Game.Server;
using Game.Server.GameObjects;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Rooms;
using log4net;

namespace Game.Service.actions;

public class ConsoleStart : IAction
{
	private delegate int ConsoleCtrlDelegate(ConsoleEvent ctrlType);

	private enum ConsoleEvent
	{
		Ctrl_C,
		Ctrl_Break,
		Close,
		Logoff,
		Shutdown
	}

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Timer _timer;

	private static int _count;

	private static ConsoleCtrlDelegate handler;

	public string Name => "--start";

	public string Syntax => "--start [-config=./config/serverconfig.xml]";

	public string Description => "Starts the DOL server in console mode";

	public void OnAction(Hashtable parameters)
	{
		Console.ForegroundColor = ConsoleColor.Yellow;
		Console.Title = "GUNNYII";
		Console.WriteLine("GUNNY II fix by trinhtanphat!");
		Console.WriteLine("Dang chay Road.Service... xin doi!");
		GameServer.CreateInstance(new GameServerConfig());
		GameServer.Instance.Start();
		GameServer.KeepRunning = true;
		Console.WriteLine("Server started!");
		ConsoleClient client = new ConsoleClient();
		while (GameServer.KeepRunning)
		{
			try
			{
				handler = ConsoleCtrHandler;
				SetConsoleCtrlHandler(handler, add: true);
				Console.Write("> ");
				string text = Console.ReadLine();
				string[] array = text.Split(' ');
				string text2;
				switch (text2 = array[0])
				{
				case "exit":
					GameServer.KeepRunning = false;
					continue;
				case "cp":
				{
					GameClient[] allClients = GameServer.Instance.GetAllClients();
					int num = ((allClients != null) ? allClients.Length : 0);
					GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
					int num2 = ((allPlayers != null) ? allPlayers.Length : 0);
					List<BaseRoom> allUsingRoom = RoomMgr.GetAllUsingRoom();
					int num3 = 0;
					int num4 = 0;
					foreach (BaseRoom item in allUsingRoom)
					{
						if (!item.IsEmpty)
						{
							num3++;
							if (item.IsPlaying)
							{
								num4++;
							}
						}
					}
					double num5 = GC.GetTotalMemory(forceFullCollection: false);
					Console.WriteLine($"Total Clients/Players:{num}/{num2}");
					Console.WriteLine($"Total Rooms/Games:{num3}/{num4}");
					Console.WriteLine($"Total Momey Used:{num5 / 1024.0 / 1024.0} MB");
					continue;
				}
				case "shutdown":
					_count = 6;
					_timer = new Timer(ShutDownCallBack, null, 0, 60000);
					continue;
				case "savemap":
					continue;
				case "clear":
					Console.Clear();
					continue;
				case "ball&reload":
					if (BallMgr.ReLoad())
					{
						Console.WriteLine("Ball info is Reload!");
					}
					else
					{
						Console.WriteLine("Ball info is Error!");
					}
					continue;
				case "map&reload":
					if (MapMgr.ReLoadMap())
					{
						Console.WriteLine("Map info is Reload!");
					}
					else
					{
						Console.WriteLine("Map info is Error!");
					}
					continue;
				case "mapserver&reload":
					if (MapMgr.ReLoadMapServer())
					{
						Console.WriteLine("mapserver info is Reload!");
					}
					else
					{
						Console.WriteLine("mapserver info is Error!");
					}
					continue;
				case "prop&reload":
					if (PropItemMgr.Reload())
					{
						Console.WriteLine("prop info is Reload!");
					}
					else
					{
						Console.WriteLine("prop info is Error!");
					}
					continue;
				case "item&reload":
					if (ItemMgr.ReLoad())
					{
						Console.WriteLine("item info is Reload!");
					}
					else
					{
						Console.WriteLine("item info is Error!");
					}
					continue;
				case "shop&reload":
					if (ShopMgr.ReLoad())
					{
						Console.WriteLine("shop info is Reload!");
					}
					else
					{
						Console.WriteLine("shop info is Error!");
					}
					continue;
				case "quest&reload":
					if (QuestMgr.ReLoad())
					{
						Console.WriteLine("quest info is Reload!");
					}
					else
					{
						Console.WriteLine("quest info is Error!");
					}
					continue;
				case "fusion&reload":
					if (FusionMgr.ReLoad())
					{
						Console.WriteLine("fusion info is Reload!");
					}
					else
					{
						Console.WriteLine("fusion info is Error!");
					}
					continue;
				case "consortia&reload":
					if (ConsortiaMgr.ReLoad())
					{
						Console.WriteLine("consortiaMgr info is Reload!");
					}
					else
					{
						Console.WriteLine("consortiaMgr info is Error!");
					}
					continue;
				case "rate&reload":
					if (RateMgr.ReLoad())
					{
						Console.WriteLine("Rate Rate is Reload!");
					}
					else
					{
						Console.WriteLine("Rate Rate is Error!");
					}
					continue;
				case "npc":
					if (NPCInfoMgr.ReLoad())
					{
						Console.WriteLine("NPCInfo is Reload!");
					}
					else
					{
						Console.WriteLine("NPCInfo is Error!");
					}
					continue;
				case "fight&reload":
					if (FightRateMgr.ReLoad())
					{
						Console.WriteLine("FightRateMgr is Reload!");
					}
					else
					{
						Console.WriteLine("FightRateMgr is Error!");
					}
					continue;
				case "dailyaward&reload":
					if (AwardMgr.ReLoad())
					{
						Console.WriteLine("dailyaward is Reload!");
					}
					else
					{
						Console.WriteLine("dailyaward is Error!");
					}
					continue;
				case "language&reload":
					if (LanguageMgr.Reload(""))
					{
						Console.WriteLine("language is Reload!");
					}
					else
					{
						Console.WriteLine("language is Error!");
					}
					continue;
				case "nickname":
				{
					Console.WriteLine("Please enter the nickname");
					string nickName = Console.ReadLine();
					string playerStringByPlayerNickName = WorldMgr.GetPlayerStringByPlayerNickName(nickName);
					Console.WriteLine(playerStringByPlayerNickName);
					continue;
				}
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
			catch (Exception value)
			{
				Console.WriteLine(value);
			}
		}
		if (GameServer.Instance != null)
		{
			GameServer.Instance.Stop();
		}
		LogManager.Shutdown();
	}

	private static void ShutDownCallBack(object state)
	{
		_count--;
		Console.WriteLine($"Server will shutdown after {_count} mins!");
		GameClient[] allClients = GameServer.Instance.GetAllClients();
		GameClient[] array = allClients;
		foreach (GameClient gameClient in array)
		{
			if (gameClient.Out != null)
			{
				gameClient.Out.SendMessage(eMessageType.Normal, string.Format("{0}{1}{2}", LanguageMgr.GetTranslation("Game.Service.actions.ShutDown1"), _count, LanguageMgr.GetTranslation("Game.Service.actions.ShutDown2")));
			}
		}
		if (_count == 0)
		{
			_timer.Dispose();
			_timer = null;
			GameServer.Instance.Stop();
			Console.WriteLine("Server has stopped!");
		}
	}

	[DllImport("kernel32.dll", CallingConvention = CallingConvention.StdCall)]
	private static extern int SetConsoleCtrlHandler(ConsoleCtrlDelegate HandlerRoutine, bool add);

	private static int ConsoleCtrHandler(ConsoleEvent e)
	{
		SetConsoleCtrlHandler(handler, add: false);
		if (GameServer.Instance != null)
		{
			GameServer.Instance.Stop();
		}
		return 0;
	}
}
