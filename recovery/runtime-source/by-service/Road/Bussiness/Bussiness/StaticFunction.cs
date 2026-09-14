using System;
using System.Configuration;
using System.Reflection;
using log4net;

namespace Bussiness;

public class StaticFunction
{
	protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static bool UpdateConfig(string fileName, string name, string value)
	{
		try
		{
			ExeConfigurationFileMap exeConfigurationFileMap = new ExeConfigurationFileMap();
			exeConfigurationFileMap.ExeConfigFilename = fileName;
			Configuration configuration = ConfigurationManager.OpenMappedExeConfiguration(exeConfigurationFileMap, ConfigurationUserLevel.None);
			configuration.AppSettings.Settings[name].Value = value;
			configuration.Save();
			ConfigurationManager.RefreshSection("appSettings");
			return true;
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("UpdateConfig", exception);
			}
		}
		return false;
	}
}
