namespace Game.Server.Packets;

public enum eMessageType
{
	Normal = 0,
	ERROR = 1,
	ChatNormal = 2,
	ChatERROR = 3,
	ALERT = 4,
	DailyAward = 5,
	Defence = 6,
	GM_NOTICE = Normal,
	BIGBUGLE_NOTICE = ERROR,
	SYS_TIP_NOTICE = ChatNormal,
	SYS_NOTICE = ChatERROR,
	CONSORTIA_NOTICE = 8,
	CROSS_NOTICE = 12
}
