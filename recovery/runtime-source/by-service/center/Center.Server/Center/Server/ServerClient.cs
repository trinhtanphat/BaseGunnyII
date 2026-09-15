using System;
using System.Collections.Generic;
using System.Reflection;
using System.Security.Cryptography;
using System.Text;
using Bussiness;
using Bussiness.Protocol;
using Center.Server.Managers;
using Game.Base;
using Game.Base.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Center.Server;

public class ServerClient : BaseClient
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private RSACryptoServiceProvider _rsa;

	private CenterServer _svr;

	public bool NeedSyncMacroDrop;

	public ServerInfo Info { get; set; }

	protected override void OnConnect()
	{
		base.OnConnect();
		_rsa = new RSACryptoServiceProvider();
		RSAParameters rSAParameters = _rsa.ExportParameters(includePrivateParameters: false);
		SendRSAKey(rSAParameters.Modulus, rSAParameters.Exponent);
	}

	protected override void OnDisconnect()
	{
		base.OnDisconnect();
		_rsa = null;
		List<Player> serverPlayers = LoginMgr.GetServerPlayers(this);
		LoginMgr.RemovePlayer(serverPlayers);
		SendUserOffline(serverPlayers);
		if (Info != null)
		{
			Info.State = 1;
			Info.Online = 0;
			Info = null;
		}
	}

	public override void OnRecvPacket(GSPacketIn pkg)
	{
		switch (pkg.Code)
		{
		case 1:
			HandleLogin(pkg);
			break;
		case 3:
			HandleUserLogin(pkg);
			break;
		case 4:
			HandleUserOffline(pkg);
			break;
		case 5:
			HandleUserOnline(pkg);
			break;
		case 6:
			HandleQuestUserState(pkg);
			break;
		case 10:
			HandkeItemStrengthen(pkg);
			break;
		case 11:
			HandleReload(pkg);
			break;
		case 12:
			HandlePing(pkg);
			break;
		case 13:
			HandleUpdatePlayerState(pkg);
			break;
		case 14:
			HandleMarryRoomInfoToPlayer(pkg);
			break;
		case 15:
			HandleShutdown(pkg);
			break;
		case 19:
			HandleChatScene(pkg);
			break;
		case 37:
			HandleChatPersonal(pkg);
			break;
		case 72:
			HandleBigBugle(pkg);
			break;
		case 81:
			HandleWorldBossRank(pkg, update: true);
			break;
		case 82:
			HandleWorldBossFightOver(pkg);
			break;
		case 83:
			HandleWorldBossRoomClose(pkg);
			break;
		case 84:
			HandleWorldBossUpdateBlood(pkg);
			break;
		case 85:
			HandleWorldBossPrivateInfo(pkg);
			break;
		case 86:
			HandleWorldBossRank(pkg, update: false);
			break;
		case 90:
			HandleEventRank(pkg);
			break;
		case 91:
			HandleWorldEvent(pkg);
			break;
		case 117:
			HandleMailResponse(pkg);
			break;
		case 128:
			HandleConsortiaResponse(pkg);
			break;
		case 130:
			HandleConsortiaCreate(pkg);
			break;
		case 156:
			HandleConsortiaOffer(pkg);
			break;
		case 158:
			HandleConsortiaFight(pkg);
			break;
		case 160:
			HandleFriend(pkg);
			break;
		case 178:
			HandleMacroDrop(pkg);
			break;
		case 180:
			HandleRecvConsortiaBossAdd(pkg);
			break;
		case 181:
			HandleRecvConsortiaBossUpdateRank(pkg);
			break;
		case 182:
			HandleRecvConsortiaBossExtendAvailable(pkg);
			break;
		case 183:
			HandleRecvConsortiaBossCreate(pkg);
			break;
		case 184:
			HandleRecvConsortiaBossReload(pkg);
			break;
		case 186:
			HandleRecvConsortiaBossUpdateBlood(pkg);
			break;
		case 240:
			HandleIPAndPort(pkg);
			break;
		}
	}

	public void HandleWorldEvent(GSPacketIn pkg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(91);
		switch (pkg.ReadByte())
		{
		case 1:
		{
			LanternriddlesInfo lanternriddlesInfo = new LanternriddlesInfo();
			lanternriddlesInfo.PlayerID = pkg.ReadInt();
			lanternriddlesInfo.QuestionIndex = pkg.ReadInt();
			lanternriddlesInfo.QuestionView = pkg.ReadInt();
			lanternriddlesInfo.EndDate = pkg.ReadDateTime();
			lanternriddlesInfo.DoubleFreeCount = pkg.ReadInt();
			lanternriddlesInfo.DoublePrice = pkg.ReadInt();
			lanternriddlesInfo.HitFreeCount = pkg.ReadInt();
			lanternriddlesInfo.HitPrice = pkg.ReadInt();
			lanternriddlesInfo.MyInteger = pkg.ReadInt();
			lanternriddlesInfo.QuestionNum = pkg.ReadInt();
			lanternriddlesInfo.Option = pkg.ReadInt();
			lanternriddlesInfo.IsHint = pkg.ReadBoolean();
			lanternriddlesInfo.IsDouble = pkg.ReadBoolean();
			WorldMgr.AddOrUpdateLanternriddles(lanternriddlesInfo.PlayerID, lanternriddlesInfo);
			break;
		}
		case 2:
		{
			int playerID = pkg.ReadInt();
			LanternriddlesInfo lanternriddles = WorldMgr.GetLanternriddles(playerID);
			gSPacketIn.WriteByte(2);
			if (lanternriddles == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(1);
				gSPacketIn.WriteInt(lanternriddles.PlayerID);
				gSPacketIn.WriteInt(lanternriddles.QuestionIndex);
				gSPacketIn.WriteInt(lanternriddles.QuestionView);
				gSPacketIn.WriteDateTime(lanternriddles.EndDate);
				gSPacketIn.WriteInt(lanternriddles.DoubleFreeCount);
				gSPacketIn.WriteInt(lanternriddles.DoublePrice);
				gSPacketIn.WriteInt(lanternriddles.HitFreeCount);
				gSPacketIn.WriteInt(lanternriddles.HitPrice);
				gSPacketIn.WriteInt(lanternriddles.MyInteger);
				gSPacketIn.WriteInt(lanternriddles.QuestionNum);
				gSPacketIn.WriteInt(lanternriddles.Option);
				gSPacketIn.WriteBoolean(lanternriddles.IsHint);
				gSPacketIn.WriteBoolean(lanternriddles.IsDouble);
			}
			_svr.SendToALL(gSPacketIn);
			break;
		}
		}
	}

	public void HandleEventRank(GSPacketIn pkg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(90);
		switch (pkg.ReadByte())
		{
		case 1:
		{
			RankingLightriddleInfo rankingLightriddleInfo = null;
			string text = pkg.ReadString();
			int val = pkg.ReadInt();
			gSPacketIn.WriteByte(1);
			List<RankingLightriddleInfo> list = WorldMgr.SelectTopEight();
			gSPacketIn.WriteInt(list.Count);
			foreach (RankingLightriddleInfo item in list)
			{
				gSPacketIn.WriteInt(item.Rank);
				gSPacketIn.WriteString(item.NickName);
				gSPacketIn.WriteByte((byte)item.TypeVIP);
				gSPacketIn.WriteInt(item.Integer);
				if (text == item.NickName)
				{
					rankingLightriddleInfo = item;
				}
			}
			if (rankingLightriddleInfo == null)
			{
				gSPacketIn.WriteInt(0);
			}
			else
			{
				gSPacketIn.WriteInt(rankingLightriddleInfo.Rank);
			}
			gSPacketIn.WriteInt(val);
			_svr.SendToALL(gSPacketIn);
			break;
		}
		case 2:
		{
			string nickName2 = pkg.ReadString();
			int typeVip = pkg.ReadInt();
			int integer = pkg.ReadInt();
			int playerId = pkg.ReadInt();
			WorldMgr.UpdateLightriddleRank(integer, typeVip, nickName2, playerId);
			break;
		}
		case 3:
			break;
		case 4:
		{
			int playerID = pkg.ReadInt();
			string nickName = pkg.ReadString();
			int templateID = pkg.ReadInt();
			int count = pkg.ReadInt();
			int isVip = pkg.ReadInt();
			WorldMgr.UpdateLuckStarRewardRecord(playerID, nickName, templateID, count, isVip);
			break;
		}
		}
	}

	public void HandleRecvConsortiaBossUpdateBlood(GSPacketIn pkg)
	{
		int consortiaId = pkg.ReadInt();
		int damage = pkg.ReadInt();
		ConsortiaBossMgr.UpdateBlood(consortiaId, damage);
	}

	public void HandleRecvConsortiaBossUpdateRank(GSPacketIn pkg)
	{
		int consortiaId = pkg.ReadInt();
		int damage = pkg.ReadInt();
		int richer = pkg.ReadInt();
		int honor = pkg.ReadInt();
		string nickName = pkg.ReadString();
		int userID = pkg.ReadInt();
		ConsortiaBossMgr.UpdateRank(consortiaId, damage, richer, honor, nickName, userID);
	}

	public void HandleRecvConsortiaBossExtendAvailable(GSPacketIn pkg)
	{
		int consortiaId = pkg.ReadInt();
		int riches = pkg.ReadInt();
		if (ConsortiaBossMgr.ExtendAvailable(consortiaId, riches))
		{
			ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(consortiaId);
			if (consortiaById != null)
			{
				HandleSendConsortiaBossInfo(consortiaById, 182);
			}
		}
		else
		{
			ConsortiaInfo consortiaById2 = ConsortiaBossMgr.GetConsortiaById(consortiaId);
			if (consortiaById2 != null)
			{
				HandleSendConsortiaBossInfo(consortiaById2, 184);
			}
		}
	}

	public void HandleRecvConsortiaBossReload(GSPacketIn pkg)
	{
		ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(pkg.ReadInt());
		if (consortiaById == null)
		{
			return;
		}
		if (consortiaById.bossState == 2 && consortiaById.SendToClient)
		{
			if (consortiaById.IsBossDie)
			{
				HandleSendConsortiaBossInfo(consortiaById, 188);
			}
			else
			{
				HandleSendConsortiaBossInfo(consortiaById, 187);
			}
			ConsortiaBossMgr.UpdateSendToClient(consortiaById.ConsortiaID);
		}
		else
		{
			HandleSendConsortiaBossInfo(consortiaById, 184);
		}
	}

	public void HandleRecvConsortiaBossCreate(GSPacketIn pkg)
	{
		int consortiaId = pkg.ReadInt();
		byte bossState = pkg.ReadByte();
		DateTime endTime = pkg.ReadDateTime();
		DateTime lastOpenBoss = pkg.ReadDateTime();
		long maxBlood = pkg.ReadInt();
		if (ConsortiaBossMgr.UpdateConsortia(consortiaId, bossState, endTime, lastOpenBoss, maxBlood))
		{
			ConsortiaInfo consortiaById = ConsortiaBossMgr.GetConsortiaById(consortiaId);
			if (consortiaById != null)
			{
				HandleSendConsortiaBossInfo(consortiaById, 183);
			}
		}
	}

	public void HandleRecvConsortiaBossAdd(GSPacketIn pkg)
	{
		ConsortiaInfo consortiaInfo = new ConsortiaInfo();
		consortiaInfo.ConsortiaID = pkg.ReadInt();
		consortiaInfo.ChairmanID = pkg.ReadInt();
		consortiaInfo.bossState = pkg.ReadByte();
		consortiaInfo.endTime = pkg.ReadDateTime();
		consortiaInfo.extendAvailableNum = pkg.ReadInt();
		consortiaInfo.callBossLevel = pkg.ReadInt();
		consortiaInfo.Level = pkg.ReadInt();
		consortiaInfo.SmithLevel = pkg.ReadInt();
		consortiaInfo.StoreLevel = pkg.ReadInt();
		consortiaInfo.SkillLevel = pkg.ReadInt();
		consortiaInfo.Riches = pkg.ReadInt();
		consortiaInfo.LastOpenBoss = pkg.ReadDateTime();
		if (!ConsortiaBossMgr.AddConsortia(consortiaInfo.ConsortiaID, consortiaInfo))
		{
			consortiaInfo = ConsortiaBossMgr.GetConsortiaById(consortiaInfo.ConsortiaID);
		}
		HandleSendConsortiaBossInfo(consortiaInfo, 180);
	}

	public void HandleSendConsortiaBossInfo(ConsortiaInfo consortia, byte code)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(180);
		gSPacketIn.WriteInt(consortia.ConsortiaID);
		gSPacketIn.WriteInt(consortia.ChairmanID);
		gSPacketIn.WriteByte((byte)consortia.bossState);
		gSPacketIn.WriteDateTime(consortia.endTime);
		gSPacketIn.WriteInt(consortia.extendAvailableNum);
		gSPacketIn.WriteInt(consortia.callBossLevel);
		gSPacketIn.WriteInt(consortia.Level);
		gSPacketIn.WriteInt(consortia.SmithLevel);
		gSPacketIn.WriteInt(consortia.StoreLevel);
		gSPacketIn.WriteInt(consortia.SkillLevel);
		gSPacketIn.WriteInt(consortia.Riches);
		gSPacketIn.WriteDateTime(consortia.LastOpenBoss);
		gSPacketIn.WriteLong(consortia.MaxBlood);
		gSPacketIn.WriteLong(consortia.TotalAllMemberDame);
		gSPacketIn.WriteBoolean(consortia.IsBossDie);
		List<RankingPersonInfo> list = ConsortiaBossMgr.SelectRank(consortia.ConsortiaID);
		gSPacketIn.WriteInt(list.Count);
		int num = 1;
		foreach (RankingPersonInfo item in list)
		{
			gSPacketIn.WriteString(item.Name);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteInt(item.TotalDamage);
			gSPacketIn.WriteInt(item.Honor);
			gSPacketIn.WriteInt(item.Damage);
			num++;
		}
		gSPacketIn.WriteByte(code);
		_svr.SendToALL(gSPacketIn);
	}

	public void HandleLogin(GSPacketIn pkg)
	{
		byte[] rgb = pkg.ReadBytes();
		string[] array = Encoding.UTF8.GetString(_rsa.Decrypt(rgb, fOAEP: false)).Split(',');
		if (array.Length != 2)
		{
			log.ErrorFormat("Error Login Packet from {0}", base.TcpEndpoint);
			Disconnect();
			return;
		}
		_rsa = null;
		int num = int.Parse(array[0]);
		Info = ServerMgr.GetServerInfo(num);
		if (Info == null || Info.State != 1)
		{
			log.ErrorFormat("Error Login Packet from {0} want to login serverid:{1}", base.TcpEndpoint, num);
			Disconnect();
			return;
		}
		base.Strict = false;
		CenterServer.Instance.SendConfigState();
		CenterServer.Instance.SendUpdateWorldEvent();
		Info.Online = 0;
		Info.State = 2;
	}

	public void HandleIPAndPort(GSPacketIn pkg)
	{
	}

	private void HandleUserLogin(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		if (LoginMgr.TryLoginPlayer(num, this))
		{
			SendAllowUserLogin(num, allow: true);
		}
		else
		{
			SendAllowUserLogin(num, allow: false);
		}
	}

	private void HandleUserOnline(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int id = pkg.ReadInt();
			pkg.ReadInt();
			LoginMgr.PlayerLogined(id, this);
		}
		_svr.SendToALL(pkg, this);
	}

	private void HandleUserOffline(GSPacketIn pkg)
	{
		new List<int>();
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int id = pkg.ReadInt();
			pkg.ReadInt();
			LoginMgr.PlayerLoginOut(id, this);
		}
		_svr.SendToALL(pkg);
	}

	private void HandleUserPrivateMsg(GSPacketIn pkg, int playerid)
	{
		LoginMgr.GetServerClient(playerid)?.SendTCP(pkg);
	}

	public void HandleUserPublicMsg(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleQuestUserState(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		if (LoginMgr.GetServerClient(num) == null)
		{
			SendUserState(num, state: false);
		}
		else
		{
			SendUserState(num, state: true);
		}
	}

	public void HandlePing(GSPacketIn pkg)
	{
		Info.Online = pkg.ReadInt();
		Info.State = ServerMgr.GetState(Info.Online, Info.Total);
	}

	public void HandleChatPersonal(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg);
	}

	public void HandleBigBugle(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleFriend(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleFriendState(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleFirendResponse(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleMailResponse(GSPacketIn pkg)
	{
		int playerid = pkg.ReadInt();
		HandleUserPrivateMsg(pkg, playerid);
	}

	public void HandleReload(GSPacketIn pkg)
	{
		eReloadType eReloadType2 = (eReloadType)pkg.ReadInt();
		int num = pkg.ReadInt();
		bool flag = pkg.ReadBoolean();
		Console.WriteLine(num + " " + eReloadType2.ToString() + " is reload " + (flag ? "succeed!" : "fail"));
	}

	public void HandleChatScene(GSPacketIn pkg)
	{
		byte b = pkg.ReadByte();
		byte b2 = b;
		if (b2 == 3)
		{
			HandleChatConsortia(pkg);
		}
	}

	public void HandleChatConsortia(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleConsortiaResponse(GSPacketIn pkg)
	{
		int num = pkg.ReadByte();
		_svr.SendToALL(pkg, null);
	}

	public void HandleConsortiaOffer(GSPacketIn pkg)
	{
		pkg.ReadInt();
		pkg.ReadInt();
		pkg.ReadInt();
	}

	public void HandleBuyBadge(GSPacketIn pkg)
	{
		pkg.ReadInt();
		_svr.SendToALL(pkg, null);
	}

	public void HandleConsortiaCreate(GSPacketIn pkg)
	{
		pkg.ReadInt();
		pkg.ReadInt();
		_svr.SendToALL(pkg, null);
	}

	public void HandleConsortiaUpGrade(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleConsortiaFight(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg);
	}

	public void HandkeItemStrengthen(GSPacketIn pkg)
	{
		_svr.SendToALL(pkg, this);
	}

	public void HandleUpdatePlayerState(GSPacketIn pkg)
	{
		int playerId = pkg.ReadInt();
		Player player = LoginMgr.GetPlayer(playerId);
		if (player != null && player.CurrentServer != null)
		{
			player.CurrentServer.SendTCP(pkg);
		}
	}

	public void HandleMarryRoomInfoToPlayer(GSPacketIn pkg)
	{
		int playerId = pkg.ReadInt();
		Player player = LoginMgr.GetPlayer(playerId);
		if (player != null && player.CurrentServer != null)
		{
			player.CurrentServer.SendTCP(pkg);
		}
	}

	public void HandleShutdown(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		if (pkg.ReadBoolean())
		{
			Console.WriteLine(num + "  begin stoping !");
		}
		else
		{
			Console.WriteLine(num + "  is stoped !");
		}
	}

	public void HandleWorldBossUpdateBlood(GSPacketIn pkg)
	{
		int num = pkg.ReadInt();
		if (num > 0)
		{
			WorldMgr.ReduceBlood(num);
		}
		_svr.SendUpdateWorldBlood();
	}

	public void HandleWorldBossFightOver(GSPacketIn pkg)
	{
		WorldMgr.WorldBossFightOver();
		_svr.SendWorldBossFightOver();
	}

	public void HandleWorldBossRoomClose(GSPacketIn pkg)
	{
		WorldMgr.WorldBossRoomClose();
		_svr.SendRoomClose(0);
	}

	public void HandleWorldBossRank(GSPacketIn pkg, bool update)
	{
		if (update)
		{
			int damage = pkg.ReadInt();
			int honor = pkg.ReadInt();
			string nickName = pkg.ReadString();
			WorldMgr.UpdateRank(damage, honor, nickName);
		}
		_svr.SendUpdateRank(type: false);
	}

	public void HandleWorldBossPrivateInfo(GSPacketIn pkg)
	{
		string name = pkg.ReadString();
		_svr.SendPrivateInfo(name);
	}

	public void HandleMacroDrop(GSPacketIn pkg)
	{
		Dictionary<int, int> dictionary = new Dictionary<int, int>();
		int num = pkg.ReadInt();
		for (int i = 0; i < num; i++)
		{
			int key = pkg.ReadInt();
			int value = pkg.ReadInt();
			dictionary.Add(key, value);
		}
		MacroDropMgr.DropNotice(dictionary);
		NeedSyncMacroDrop = true;
	}

	public void SendRSAKey(byte[] m, byte[] e)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(0);
		gSPacketIn.Write(m);
		gSPacketIn.Write(e);
		SendTCP(gSPacketIn);
	}

	public void SendAllowUserLogin(int playerid, bool allow)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteBoolean(allow);
		SendTCP(gSPacketIn);
	}

	public void SendKitoffUser(int playerid)
	{
		SendKitoffUser(playerid, LanguageMgr.GetTranslation("Center.Server.SendKitoffUser"));
	}

	public void SendKitoffUser(int playerid, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(2);
		gSPacketIn.WriteInt(playerid);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendUserOffline(List<Player> users)
	{
		for (int i = 0; i < users.Count; i += 100)
		{
			int num = ((i + 100 > users.Count) ? (users.Count - i) : 100);
			GSPacketIn gSPacketIn = new GSPacketIn(4);
			gSPacketIn.WriteInt(num);
			for (int j = i; j < i + num; j++)
			{
				gSPacketIn.WriteInt(users[j].Id);
				gSPacketIn.WriteInt(0);
			}
			SendTCP(gSPacketIn);
			_svr.SendToALL(gSPacketIn, this);
		}
	}

	public void SendUserState(int player, bool state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(6, player);
		gSPacketIn.WriteBoolean(state);
		SendTCP(gSPacketIn);
	}

	public void SendChargeMoney(int player, string chargeID)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(9, player);
		gSPacketIn.WriteString(chargeID);
		SendTCP(gSPacketIn);
	}

	public void SendASS(bool state)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(7);
		gSPacketIn.WriteBoolean(state);
		SendTCP(gSPacketIn);
	}

	public ServerClient(CenterServer svr)
		: base(new byte[8192], new byte[8192])
	{
		_svr = svr;
	}
}
