using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using log4net;

namespace Game.Logic;

public class DropInfoMgr
{
	protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	protected static ReaderWriterLock m_lock = new ReaderWriterLock();

	public static Dictionary<int, MacroDropInfo> DropInfo;

	public static bool CanDrop(int templateId)
	{
		if (DropInfo == null)
		{
			return true;
		}
		m_lock.AcquireWriterLock(-1);
		try
		{
			if (DropInfo.ContainsKey(templateId))
			{
				MacroDropInfo macroDropInfo = DropInfo[templateId];
				if (macroDropInfo.DropCount < macroDropInfo.MaxDropCount || macroDropInfo.SelfDropCount >= macroDropInfo.DropCount)
				{
					macroDropInfo.SelfDropCount++;
					macroDropInfo.DropCount++;
					return true;
				}
				return false;
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
		return true;
	}
}
