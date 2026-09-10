using System;
using Game.Base;
using Game.Base.Config;

namespace Game.Server
{
    public enum eGameServerType
    {
        GST_Normal = 0
    }

    public sealed class GameServerConfiguration : BaseServerConfiguration
    {
        public GameServerConfiguration()
        {
            ServerType = eGameServerType.GST_Normal;
            ServerName = "7Road Server";
            ServerNameShort = "7Road";
            DBConnectionString = string.Empty;
        }

        public eGameServerType ServerType { get; set; }
        public string ServerName { get; set; }
        public string ServerNameShort { get; set; }
        public string DBConnectionString { get; set; }

        protected override void LoadFromConfig(ConfigElement root)
        {
            base.LoadFromConfig(root);
            int serverType = root["Server"]["ServerType"].GetInt((int)ServerType);
            ServerType = Enum.IsDefined(typeof(eGameServerType), serverType)
                ? (eGameServerType)serverType
                : eGameServerType.GST_Normal;
            ServerName = root["Server"]["ServerName"].GetString(ServerName);
            ServerNameShort = root["Server"]["ServerNameShort"].GetString(ServerNameShort);
            DBConnectionString = root["Server"]["DBConnectionString"].GetString(DBConnectionString);
        }

        protected override void SaveToConfig(ConfigElement root)
        {
            base.SaveToConfig(root);
            root["Server"]["ServerType"].Set((int)ServerType);
            root["Server"]["ServerName"].Set(ServerName);
            root["Server"]["ServerNameShort"].Set(ServerNameShort);
            root["Server"]["DBConnectionString"].Set(DBConnectionString);
        }
    }
}
