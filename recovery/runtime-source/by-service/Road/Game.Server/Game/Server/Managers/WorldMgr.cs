using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using System.Threading;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using Game.Server.GameUtils;
using Game.Server.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public sealed class WorldMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_clientLocker = new ReaderWriterLock();

	private static Dictionary<int, GamePlayer> m_players = new Dictionary<int, GamePlayer>();

	private static Dictionary<int, Dictionary<int, ItemInfo>> m_playerMailBags = new Dictionary<int, Dictionary<int, ItemInfo>>();

	public static Scene _marryScene;

	public static Scene _hotSpring;

	private static RSACryptoServiceProvider m_rsa;

	public static Scene MarryScene => _marryScene;

	public static Scene HotSpring => _hotSpring;

	public static RSACryptoServiceProvider RsaCryptor => m_rsa;

	public static bool Init()
	{
		bool result = false;
		try
		{
			m_rsa = new RSACryptoServiceProvider();
			m_rsa.FromXmlString(GameServer.Instance.Configuration.PrivateKey);
			m_players.Clear();
			using ServiceBussiness serviceBussiness = new ServiceBussiness();
			ServerInfo serviceSingle = serviceBussiness.GetServiceSingle(GameServer.Instance.Configuration.ServerID);
			if (serviceSingle != null)
			{
				_marryScene = new Scene(serviceSingle);
				result = true;
			}
		}
		catch (Exception exception)
		{
			log.Error("WordMgr Init", exception);
		}
		return result;
	}

	public static void AddItemToMailBag(int playerId, List<ItemInfo> infos)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_playerMailBags.ContainsKey(playerId))
			{
				Dictionary<int, ItemInfo> dictionary = m_playerMailBags[playerId];
				{
					foreach (ItemInfo info in infos)
					{
						if (!dictionary.ContainsKey(info.TemplateID))
						{
							dictionary.Add(info.TemplateID, info);
						}
						else
						{
							dictionary[info.TemplateID].Count += info.Count;
						}
					}
					return;
				}
			}
			Dictionary<int, ItemInfo> dictionary2 = new Dictionary<int, ItemInfo>();
			foreach (ItemInfo info2 in infos)
			{
				if (!dictionary2.ContainsKey(info2.TemplateID))
				{
					dictionary2.Add(info2.TemplateID, info2);
				}
				else
				{
					dictionary2[info2.TemplateID].Count += info2.Count;
				}
			}
			m_playerMailBags.Add(playerId, dictionary2);
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
	}

	public static Dictionary<int, Dictionary<int, ItemInfo>> GetAllBagMails()
	{
		Dictionary<int, Dictionary<int, ItemInfo>> dictionary = new Dictionary<int, Dictionary<int, ItemInfo>>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			foreach (KeyValuePair<int, Dictionary<int, ItemInfo>> playerMailBag in m_playerMailBags)
			{
				dictionary.Add(playerMailBag.Key, playerMailBag.Value);
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return dictionary;
	}

	public static void ScanBagMail()
	{
		Dictionary<int, Dictionary<int, ItemInfo>> allBagMails = GetAllBagMails();
		foreach (KeyValuePair<int, Dictionary<int, ItemInfo>> item in allBagMails)
		{
			if (item.Value != null)
			{
				SendItemsToMail(item.Value, item.Key);
			}
		}
	}

	public static void SendItemsToMail(Dictionary<int, ItemInfo> infos, int PlayerId)
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (ItemInfo value in infos.Values)
		{
			if (value.Template.MaxCount == 1)
			{
				for (int i = 0; i < value.Count; i++)
				{
					ItemInfo itemInfo = ItemInfo.CloneFromTemplate(value.Template, value);
					itemInfo.Count = 1;
					list.Add(itemInfo);
				}
			}
			else
			{
				list.Add(value);
			}
		}
		for (int j = 0; j < list.Count; j += 5)
		{
			MailInfo mailInfo = new MailInfo();
			mailInfo.Title = "Vật phẩm chuyển về từ -Túi ẩn-";
			mailInfo.Gold = 0;
			mailInfo.IsExist = true;
			mailInfo.Money = 0;
			mailInfo.Receiver = "Túi ẩn";
			mailInfo.ReceiverID = PlayerId;
			mailInfo.Sender = "Hệ thống";
			mailInfo.SenderID = 0;
			mailInfo.Type = 9;
			mailInfo.GiftToken = 0;
			MailInfo mailInfo2 = mailInfo;
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			int num = j;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					playerBussiness.AddGoods(itemInfo2);
				}
				mailInfo2.Annex1 = itemInfo2.ItemID.ToString();
				mailInfo2.Annex1Name = itemInfo2.Template.Name;
				stringBuilder.Append("1、" + mailInfo2.Annex1Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("1、" + mailInfo2.Annex1Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 1;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					playerBussiness.AddGoods(itemInfo2);
				}
				mailInfo2.Annex2 = itemInfo2.ItemID.ToString();
				mailInfo2.Annex2Name = itemInfo2.Template.Name;
				stringBuilder.Append("2、" + mailInfo2.Annex2Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("2、" + mailInfo2.Annex2Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 2;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					playerBussiness.AddGoods(itemInfo2);
				}
				mailInfo2.Annex3 = itemInfo2.ItemID.ToString();
				mailInfo2.Annex3Name = itemInfo2.Template.Name;
				stringBuilder.Append("3、" + mailInfo2.Annex3Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("3、" + mailInfo2.Annex3Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 3;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					playerBussiness.AddGoods(itemInfo2);
				}
				mailInfo2.Annex4 = itemInfo2.ItemID.ToString();
				mailInfo2.Annex4Name = itemInfo2.Template.Name;
				stringBuilder.Append("4、" + mailInfo2.Annex4Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("4、" + mailInfo2.Annex4Name + "x" + itemInfo2.Count + ";");
			}
			num = j + 4;
			if (list.Count > num)
			{
				ItemInfo itemInfo2 = list[num];
				if (itemInfo2.ItemID == 0)
				{
					playerBussiness.AddGoods(itemInfo2);
				}
				mailInfo2.Annex5 = itemInfo2.ItemID.ToString();
				mailInfo2.Annex5Name = itemInfo2.Template.Name;
				stringBuilder.Append("5、" + mailInfo2.Annex5Name + "x" + itemInfo2.Count + ";");
				stringBuilder2.Append("5、" + mailInfo2.Annex5Name + "x" + itemInfo2.Count + ";");
			}
			mailInfo2.AnnexRemark = stringBuilder.ToString();
			mailInfo2.Content = stringBuilder2.ToString();
			if (playerBussiness.SendMail(mailInfo2))
			{
				m_playerMailBags.Remove(PlayerId);
			}
		}
	}

	public static void SendToAll(GSPacketIn pkg)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			gamePlayer.SendTCP(pkg);
		}
	}

	public static GSPacketIn SendSysNotice(eMessageType type, string msg, int ItemID, int TemplateID, string key)
	{
		int val = msg.IndexOf("@");
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt((int)type);
		gSPacketIn.WriteString(msg.Replace("@", ""));
		if (type == eMessageType.CROSS_NOTICE)
		{
			gSPacketIn.WriteInt(4);
		}
		if (ItemID > 0)
		{
			gSPacketIn.WriteByte(1);
			gSPacketIn.WriteInt(val);
			gSPacketIn.WriteInt(TemplateID);
			gSPacketIn.WriteInt(ItemID);
			gSPacketIn.WriteString(key);
		}
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static GSPacketIn SendSysNotice(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(10);
		gSPacketIn.WriteInt(2);
		gSPacketIn.WriteString(msg);
		SendToAll(gSPacketIn);
		return gSPacketIn;
	}

	public static bool AddPlayer(int playerId, GamePlayer player)
	{
		m_clientLocker.AcquireWriterLock(-1);
		try
		{
			if (m_players.ContainsKey(playerId))
			{
				return false;
			}
			m_players.Add(playerId, player);
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		return true;
	}

	public static bool RemovePlayer(int playerId)
	{
		m_clientLocker.AcquireWriterLock(-1);
		GamePlayer gamePlayer = null;
		try
		{
			if (m_players.ContainsKey(playerId))
			{
				gamePlayer = m_players[playerId];
				m_players.Remove(playerId);
			}
		}
		finally
		{
			m_clientLocker.ReleaseWriterLock();
		}
		if (gamePlayer == null)
		{
			return false;
		}
		GameServer.Instance.LoginServer.SendUserOffline(playerId, gamePlayer.PlayerCharacter.ConsortiaID);
		return true;
	}

	public static GamePlayer GetPlayerById(int playerId)
	{
		GamePlayer result = null;
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			if (m_players.ContainsKey(playerId))
			{
				result = m_players[playerId];
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return result;
	}

	public static GamePlayer GetClientByPlayerNickName(string nickName)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == nickName)
			{
				return gamePlayer;
			}
		}
		return null;
	}

	public static GamePlayer[] GetAllPlayers()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			foreach (GamePlayer value in m_players.Values)
			{
				if (value != null && value.PlayerCharacter != null)
				{
					list.Add(value);
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}

	public static GamePlayer[] GetAllPlayersNoGame()
	{
		List<GamePlayer> list = new List<GamePlayer>();
		m_clientLocker.AcquireReaderLock(-1);
		try
		{
			GamePlayer[] allPlayers = GetAllPlayers();
			foreach (GamePlayer gamePlayer in allPlayers)
			{
				if (gamePlayer.CurrentRoom == null)
				{
					list.Add(gamePlayer);
				}
			}
		}
		finally
		{
			m_clientLocker.ReleaseReaderLock();
		}
		return list.ToArray();
	}

	public static string GetPlayerStringByPlayerNickName(string nickName)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == nickName)
			{
				return gamePlayer.ToString();
			}
		}
		return nickName + " is not online!";
	}

	public static string DisconnectPlayerByName(string nickName)
	{
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if (gamePlayer.PlayerCharacter.NickName == nickName)
			{
				gamePlayer.Disconnect();
				return "OK";
			}
		}
		return nickName + " is not online!";
	}

	public static void OnPlayerOffline(int playerid, int consortiaID)
	{
		ChangePlayerState(playerid, 0, consortiaID);
	}

	public static void OnPlayerOnline(int playerid, int consortiaID)
	{
		ChangePlayerState(playerid, 1, consortiaID);
	}

	public static void ChangePlayerState(int playerID, int state, int consortiaID)
	{
		GSPacketIn gSPacketIn = null;
		GamePlayer[] allPlayers = GetAllPlayers();
		GamePlayer[] array = allPlayers;
		foreach (GamePlayer gamePlayer in array)
		{
			if ((gamePlayer.Friends != null && gamePlayer.Friends.ContainsKey(playerID) && gamePlayer.Friends[playerID] == 0) || (gamePlayer.PlayerCharacter.ConsortiaID != 0 && gamePlayer.PlayerCharacter.ConsortiaID == consortiaID))
			{
				if (gSPacketIn == null)
				{
					gSPacketIn = gamePlayer.Out.SendFriendState(playerID, state, gamePlayer.PlayerCharacter.typeVIP, gamePlayer.PlayerCharacter.VIPLevel);
				}
				else
				{
					gamePlayer.SendTCP(gSPacketIn);
				}
			}
		}
	}
}
