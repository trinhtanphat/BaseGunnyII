using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Threading;
using System.Timers;
using Bussiness;
using Game.Base.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Center.Server.Managers;

public class MacroDropMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static ReaderWriterLock m_lock;

	private static Dictionary<int, DropInfo> m_DropInfo;

	private static string FilePath;

	private static int counter;

	public static bool Init()
	{
		m_lock = new ReaderWriterLock();
		FilePath = Directory.GetCurrentDirectory() + "\\macrodrop\\macroDrop.ini";
		return Reload();
	}

	public static bool Reload()
	{
		try
		{
			Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
			m_DropInfo = new Dictionary<int, DropInfo>();
			dictionary = LoadDropInfo();
			if (dictionary != null && dictionary.Count > 0)
			{
				Interlocked.Exchange(ref m_DropInfo, dictionary);
			}
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DropInfoMgr", exception);
			}
		}
		return false;
	}

	private static void MacroDropReset()
	{
		m_lock.AcquireWriterLock(-1);
		try
		{
			foreach (KeyValuePair<int, DropInfo> item in m_DropInfo)
			{
				int key = item.Key;
				DropInfo value = item.Value;
				if (counter > value.Time && value.Time > 0 && counter % value.Time == 0)
				{
					value.Count = value.MaxCount;
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DropInfoMgr MacroDropReset", exception);
			}
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
	}

	private static void MacroDropSync()
	{
		bool flag = true;
		ServerClient[] allClients = CenterServer.Instance.GetAllClients();
		ServerClient[] array = allClients;
		foreach (ServerClient serverClient in array)
		{
			if (!serverClient.NeedSyncMacroDrop)
			{
				flag = false;
				break;
			}
		}
		if (allClients.Length <= 0 || !flag)
		{
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(178);
		int count = m_DropInfo.Count;
		gSPacketIn.WriteInt(count);
		m_lock.AcquireReaderLock(-1);
		try
		{
			foreach (KeyValuePair<int, DropInfo> item in m_DropInfo)
			{
				DropInfo value = item.Value;
				gSPacketIn.WriteInt(value.ID);
				gSPacketIn.WriteInt(value.Count);
				gSPacketIn.WriteInt(value.MaxCount);
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DropInfoMgr MacroDropReset", exception);
			}
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		ServerClient[] array2 = allClients;
		foreach (ServerClient serverClient2 in array2)
		{
			serverClient2.NeedSyncMacroDrop = false;
			serverClient2.SendTCP(gSPacketIn);
		}
	}

	private static void OnTimeEvent(object source, ElapsedEventArgs e)
	{
		counter++;
		if (counter % 12 == 0)
		{
			MacroDropReset();
		}
		MacroDropSync();
	}

	public static void Start()
	{
		counter = 0;
		System.Timers.Timer timer = new System.Timers.Timer();
		timer.Elapsed += OnTimeEvent;
		timer.Interval = 300000.0;
		timer.Enabled = true;
	}

	private static Dictionary<int, DropInfo> LoadDropInfo()
	{
		Dictionary<int, DropInfo> dictionary = new Dictionary<int, DropInfo>();
		if (File.Exists(FilePath))
		{
			IniReader iniReader = new IniReader(FilePath);
			int num = 1;
			while (iniReader.GetIniString(num.ToString(), "TemplateId") != "")
			{
				string section = num.ToString();
				int id = Convert.ToInt32(iniReader.GetIniString(section, "TemplateId"));
				int time = Convert.ToInt32(iniReader.GetIniString(section, "Time"));
				int num2 = Convert.ToInt32(iniReader.GetIniString(section, "Count"));
				DropInfo dropInfo = new DropInfo(id, time, num2, num2);
				dictionary.Add(dropInfo.ID, dropInfo);
				num++;
			}
			return dictionary;
		}
		return null;
	}

	public static void DropNotice(Dictionary<int, int> temp)
	{
		m_lock.AcquireWriterLock(-1);
		try
		{
			foreach (KeyValuePair<int, int> item in temp)
			{
				if (m_DropInfo.ContainsKey(item.Key))
				{
					DropInfo dropInfo = m_DropInfo[item.Key];
					if (dropInfo.Count > 0)
					{
						dropInfo.Count -= item.Value;
					}
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("DropInfoMgr CanDrop", exception);
			}
		}
		finally
		{
			m_lock.ReleaseWriterLock();
		}
	}
}
