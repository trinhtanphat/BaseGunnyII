using System;
using System.Configuration;
using System.Data;
using System.Reflection;
using Bussiness;
using log4net;

namespace Center.Server.Statics;

public class LogMgr
{
	public static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static object _syncStop = new object();

	private static int _gameType;

	private static int _serverId;

	private static int _areaId;

	public static DataTable m_LogServer;

	private static int regCount;

	public static object _sysObj = new object();

	public static int GameType => int.Parse(ConfigurationManager.AppSettings["GameType"]);

	public static int ServerID => int.Parse(ConfigurationManager.AppSettings["ServerID"]);

	public static int AreaID => int.Parse(ConfigurationManager.AppSettings["AreaID"]);

	public static int SaveRecordSecond => int.Parse(ConfigurationManager.AppSettings["SaveRecordInterval"]) * 60;

	public static int RegCount
	{
		get
		{
			lock (_sysObj)
			{
				return regCount;
			}
		}
		set
		{
			lock (_sysObj)
			{
				regCount = value;
			}
		}
	}

	public static bool Setup()
	{
		return Setup(GameType, ServerID, AreaID);
	}

	public static bool Setup(int gametype, int serverid, int areaid)
	{
		_gameType = gametype;
		_serverId = serverid;
		_areaId = areaid;
		m_LogServer = new DataTable("Log_Server");
		m_LogServer.Columns.Add("ApplicationId", typeof(int));
		m_LogServer.Columns.Add("SubId", typeof(int));
		m_LogServer.Columns.Add("EnterTime", typeof(DateTime));
		m_LogServer.Columns.Add("Online", typeof(int));
		m_LogServer.Columns.Add("Reg", typeof(int));
		return true;
	}

	public static void Reset()
	{
		lock (m_LogServer)
		{
			m_LogServer.Clear();
		}
	}

	public static void Save()
	{
		int onlineCount = LoginMgr.GetOnlineCount();
		int gameType = _gameType;
		int serverId = _serverId;
		DateTime now = DateTime.Now;
		int num = RegCount;
		RegCount = 0;
		int saveRecordSecond = SaveRecordSecond;
		using ItemRecordBussiness itemRecordBussiness = new ItemRecordBussiness();
		itemRecordBussiness.LogServerDb(m_LogServer);
	}

	public static void AddRegCount()
	{
		lock (_sysObj)
		{
			regCount++;
		}
	}
}
