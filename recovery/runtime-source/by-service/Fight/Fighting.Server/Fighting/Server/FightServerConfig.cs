using System;
using System.IO;
using System.Reflection;
using Game.Base.Config;

namespace Fighting.Server;

public class FightServerConfig : BaseAppConfig
{
	public string LogConfigFile = "logconfig.xml";

	public string RootDirectory;

	[ConfigProperty("ZoneId", "服务器编号", 4)]
	public int ZoneId;

	[ConfigProperty("ServerName", "频道的名称", "7Road")]
	public string ServerName;

	[ConfigProperty("IP", "频道的IP", "127.0.0.1")]
	public string Ip;

	[ConfigProperty("Port", "频道开放端口", 9208)]
	public int Port;

	[ConfigProperty("ScriptAssemblies", "脚本编译引用库", "")]
	public string ScriptAssemblies;

	[ConfigProperty("ScriptCompilationTarget", "脚本编译目标名称", "")]
	public string ScriptCompilationTarget;

	public void LoadConfiguration()
	{
		Load(typeof(FightServerConfig));
	}

	protected override void Load(Type type)
	{
		if (Assembly.GetEntryAssembly() != null)
		{
			RootDirectory = new FileInfo(Assembly.GetEntryAssembly().Location).DirectoryName;
		}
		else
		{
			RootDirectory = new FileInfo(Assembly.GetAssembly(typeof(FightServer)).Location).DirectoryName;
		}
		base.Load(type);
	}
}
