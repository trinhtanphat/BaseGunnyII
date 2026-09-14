using System.Diagnostics;

namespace SqlDataProvider.BaseClass;

public static class ApplicationLog
{
	public static void WriteError(string message)
	{
		WriteLog(TraceLevel.Error, message);
	}

	private static void WriteLog(TraceLevel level, string messageText)
	{
		try
		{
			EventLogEntryType type = ((level != TraceLevel.Error) ? EventLogEntryType.Error : EventLogEntryType.Error);
			string text = "Application";
			if (!EventLog.SourceExists(text))
			{
				EventLog.CreateEventSource(text, "BIZ");
			}
			EventLog eventLog = new EventLog(text, ".", text);
			eventLog.WriteEntry(messageText, type);
		}
		catch
		{
		}
	}
}
