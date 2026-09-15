using System;
using System.Text;
using Bussiness;
using Bussiness.Interface;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.Managers;
using SqlDataProvider.Data;

namespace Game.Server.Packets.Client;

[PacketHandler(1, "User Login handler")]
public class UserLoginHandler : IPacketHandler
{
	public int HandlePacket(GameClient client, GSPacketIn packet)
	{
		try
		{
			if (client.Player == null)
			{
				int version = packet.ReadInt();
				packet.ReadInt();
				byte[] array = new byte[8];
				byte[] rgb = packet.ReadBytes();
				try
				{
					rgb = WorldMgr.RsaCryptor.Decrypt(rgb, fOAEP: false);
				}
				catch (ExecutionEngineException exception)
				{
					client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError"));
					client.Disconnect();
					GameServer.log.Error("ExecutionEngineException", exception);
					return 0;
				}
				catch (Exception exception2)
				{
					client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.RsaCryptorError"));
					client.Disconnect();
					GameServer.log.Error("RsaCryptor", exception2);
					return 0;
				}
				string edition = GameServer.Edition;
				for (int i = 0; i < 8; i++)
				{
					array[i] = rgb[i + 7];
				}
				client.setKey(array);
				string[] array2 = Encoding.UTF8.GetString(rgb, 15, rgb.Length - 15).Split(',');
				if (array2.Length == 2)
				{
					string text = array2[0];
					string pass = array2[1];
					if (!LoginMgr.ContainsUser(text))
					{
						bool isFirst = false;
						BaseInterface baseInterface = BaseInterface.CreateInterface();
						PlayerInfo playerInfo = baseInterface.LoginGame(text, pass, ref isFirst);
						if (playerInfo != null && playerInfo.ID != 0)
						{
							if (playerInfo.ID == -2)
							{
								client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid"));
								client.Disconnect();
								return 0;
							}
							if (!isFirst)
							{
								client.Player = new GamePlayer(playerInfo.ID, text, client, playerInfo);
								LoginMgr.Add(playerInfo.ID, client);
								client.Server.LoginServer.SendAllowUserLogin(playerInfo.ID);
								client.Version = version;
							}
							else
							{
								client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Register"));
								client.Disconnect();
							}
						}
						else
						{
							client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.OverTime"));
							client.Disconnect();
						}
					}
					else
					{
						client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LoginError"));
						client.Disconnect();
					}
				}
				else
				{
					client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.LengthError"));
					client.Disconnect();
				}
			}
		}
		catch (Exception exception3)
		{
			client.Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.ServerError"));
			client.Disconnect();
			GameServer.log.Error(LanguageMgr.GetTranslation("UserLoginHandler.ServerError"), exception3);
		}
		return 1;
	}
}
