namespace Game.Server.Packets;

public enum ePlayerState
{
	Offline = 0,
	Online = 1,
	Away = 2,
	Busy = 3,
	Shoping = 4,
	No_Distrub = 5,
	CompleteLoad = 13,
	Auto = Offline,
	Manual = Online,
	RoomList = Online,
	Dungeon = Away
}
