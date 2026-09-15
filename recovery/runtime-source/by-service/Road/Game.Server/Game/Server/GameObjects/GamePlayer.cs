using System;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using Game.Server.Achievements;
using Game.Server.Buffer;
using Game.Server.GameUtils;
using Game.Server.Managers;
using Game.Server.Packets;
using Game.Server.Quests;
using Game.Server.Rooms;
using Game.Server.SceneMarryRooms;
using Game.Server.Statics;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.GameObjects;

public class GamePlayer : IGamePlayer
{
	public delegate void PlayerEventHandle(GamePlayer player);

	public delegate void PlayerItemPropertyEventHandle(int templateID);

	public delegate void PlayerGameOverEventHandle(AbstractGame game, bool isWin, int gainXp);

	public delegate void PlayerMissionOverEventHandle(AbstractGame game, int missionId, bool isWin);

	public delegate void PlayerMissionTurnOverEventHandle(AbstractGame game, int missionId, int turnNum);

	public delegate void PlayerItemStrengthenEventHandle(int categoryID, int level);

	public delegate void PlayerShopEventHandle(int money, int gold, int offer, int gifttoken, int medal, string payGoods);

	public delegate void PlayerAdoptPetEventHandle();

	public delegate void PlayerNewGearEventHandle(int CategoryID);

	public delegate void PlayerCropPrimaryEventHandle();

	public delegate void PlayerSeedFoodPetEventHandle();

	public delegate void PlayerUserToemGemstoneEventHandle();

	public delegate void PlayerUnknowQuestConditionEventHandle();

	public delegate void PlayerItemInsertEventHandle();

	public delegate void PlayerItemFusionEventHandle(int fusionType);

	public delegate void PlayerItemMeltEventHandle(int categoryID);

	public delegate void PlayerGameKillEventHandel(AbstractGame game, int type, int id, bool isLiving, int demage);

	public delegate void PlayerOwnConsortiaEventHandle();

	public delegate void PlayerItemComposeEventHandle(int composeType);

	public delegate void GameKillDropEventHandel(AbstractGame game, int type, int npcId, bool playResult);

	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private ePlayerState m_playerState;

	protected BaseGame m_game;

	private int m_playerId;

	protected GameClient m_client;

	protected Player m_players;

	private PlayerInfo m_character;

	private string m_account;

	private int m_immunity = 255;

	public bool m_toemview;

	public int FightPower;

	private bool m_isMinor;

	private bool m_isAASInfo;

	private long m_pingTime;

	private char[] m_pvepermissions;

	public long PingStart;

	private bool m_showPP;

	private List<BufferInfo> m_fightBuffInfo;

	private UsersPetinfo m_pet;

	private PlayerEquipInventory m_mainBag;

	private PlayerInventory m_propBag;

	private PlayerInventory m_fightBag;

	private PlayerInventory m_ConsortiaBag;

	private PlayerInventory m_storeBag;

	private PlayerInventory m_tempBag;

	private PlayerInventory m_caddyBag;

	private PlayerInventory m_farmBag;

	private PlayerInventory m_vegetable;

	private PlayerInventory m_food;

	private PlayerInventory m_petEgg;

	private PlayerBeadInventory m_BeadBag;

	private CardInventory m_cardBag;

	private PlayerFarm m_farm;

	private PetInventory m_petBag;

	private PlayerTreasure m_treasure;

	private PlayerProperty m_playerProp;

	private PlayerRank m_rank;

	private PlayerDice m_dice;

	private PlayerBattle m_battle;

	private PlayerActives m_actives;

	private AchievementInventory m_achievementInventory;

	private QuestInventory m_questInventory;

	private BufferList m_bufferList;

	private UserLabyrinthInfo m_Labyrinth;

	private List<UserGemStone> m_GemStone;

	private Dictionary<int, UserDrillInfo> m_userDrills;

	private List<ItemInfo> m_equipEffect;

	private int m_changed;

	private static readonly int[] StyleIndex = new int[15]
	{
		1, 2, 3, 4, 5, 6, 11, 13, 14, 15,
		16, 17, 18, 19, 20
	};

	public double GPAddPlus;

	public double OfferAddPlus = 1.0;

	public double GuildRichAddPlus = 1.0;

	public DateTime LastChatTime;

	public DateTime LastFigUpTime;

	public DateTime LastDrillUpTime;

	public DateTime LastOpenPack;

	public DateTime LastOpenCard;

	public DateTime LastAttachMail;

	public DateTime LastEnterWorldBoss;

	public bool KickProtect;

	private ItemInfo m_MainWeapon;

	private ItemInfo m_healstone;

	private ItemInfo m_currentSecondWeapon;

	public readonly string[] labyrinthGolds = new string[40]
	{
		"0|0", "2|2", "0|0", "2|2", "0|0", "2|3", "0|0", "3|3", "0|0", "3|4",
		"0|0", "3|4", "0|0", "4|5", "0|0", "4|5", "0|0", "4|6", "0|0", "5|6",
		"0|0", "5|7", "0|0", "5|7", "0|0", "6|8", "0|0", "6|8", "0|0", "6|10",
		"0|0", "8|10", "0|0", "8|11", "0|0", "8|11", "0|0", "10|12", "0|0", "10|12"
	};

	private List<int> _viFarms;

	private Dictionary<int, int> _friends;

	private BaseRoom m_currentRoom;

	public int CurrentRoomIndex;

	public int CurrentRoomTeam;

	public int beadRequestBtn1;

	public int beadRequestBtn2;

	public int beadRequestBtn3;

	public int beadIndex;

	public int WorldBossMap;

	public bool IsInWorldBossRoom;

	public bool IsInChristmasRoom;

	public byte States;

	public bool isPowerFullUsed;

	public int winningStreak;

	public Dictionary<int, CardInfo> Card = new Dictionary<int, CardInfo>();

	public CardInfo[] CardsTakeOut = new CardInfo[9];

	public int canTakeOut;

	public int takeoutCount;

	public int X;

	public int Y;

	public int MarryMap;

	private BaseSevenDoubleRoom m_currentSevenDoubleRoom;

	private MarryRoom m_currentMarryRoom;

	public int Hot_X;

	public int Hot_Y;

	public int HotMap;

	private UTF8Encoding m_converter;

	private static char[] permissionChars = new char[4] { '1', '3', '7', 'F' };

	private Dictionary<string, object> m_tempProperties = new Dictionary<string, object>();

	public ePlayerState PlayerState
	{
		get
		{
			return m_playerState;
		}
		set
		{
			m_playerState = value;
		}
	}

	public BaseGame game
	{
		get
		{
			return m_game;
		}
		set
		{
			m_game = value;
		}
	}

	public int Immunity
	{
		get
		{
			return m_immunity;
		}
		set
		{
			m_immunity = value;
		}
	}

	public int PlayerId => m_playerId;

	public bool Toemview
	{
		get
		{
			return m_toemview;
		}
		set
		{
			m_toemview = value;
		}
	}

	public string Account => m_account;

	public PlayerInfo PlayerCharacter => m_character;

	public UserMatchInfo MatchInfo => m_battle.MatchInfo;

	public GameClient Client => m_client;

	public Player Players => m_players;

	public bool IsActive => m_client.IsConnected;

	public IPacketLib Out => m_client.Out;

	public bool IsMinor
	{
		get
		{
			return m_isMinor;
		}
		set
		{
			m_isMinor = value;
		}
	}

	public bool IsAASInfo
	{
		get
		{
			return m_isAASInfo;
		}
		set
		{
			m_isAASInfo = value;
		}
	}

	public long PingTime
	{
		get
		{
			return m_pingTime;
		}
		set
		{
			m_pingTime = value;
			GSPacketIn pkg = Out.SendNetWork(this, m_pingTime);
			if (m_currentRoom != null)
			{
				m_currentRoom.SendToAll(pkg, this);
			}
		}
	}

	public bool ShowPP
	{
		get
		{
			return m_showPP;
		}
		set
		{
			m_showPP = value;
		}
	}

	public List<BufferInfo> FightBuffs
	{
		get
		{
			return m_fightBuffInfo;
		}
		set
		{
			m_fightBuffInfo = value;
		}
	}

	public UsersPetinfo Pet => m_pet;

	public PlayerBattle BattleData => m_battle;

	public PlayerActives Actives => m_actives;

	public PlayerDice Dice => m_dice;

	public PlayerProperty PlayerProp => m_playerProp;

	public PlayerRank Rank => m_rank;

	public PlayerTreasure Treasure => m_treasure;

	public PetInventory PetBag => m_petBag;

	public PlayerFarm Farm => m_farm;

	public PlayerEquipInventory MainBag => m_mainBag;

	public PlayerInventory PropBag => m_propBag;

	public PlayerInventory FightBag => m_fightBag;

	public PlayerInventory TempBag => m_tempBag;

	public PlayerInventory ConsortiaBag => m_ConsortiaBag;

	public PlayerInventory StoreBag => m_storeBag;

	public PlayerInventory CaddyBag => m_caddyBag;

	public PlayerInventory FarmBag => m_farmBag;

	public PlayerInventory Vegetable => m_vegetable;

	public PlayerInventory Food => m_food;

	public PlayerInventory PetEgg => m_petEgg;

	public PlayerBeadInventory BeadBag => m_BeadBag;

	public CardInventory CardBag => m_cardBag;

	public AchievementInventory AchievementInventory => m_achievementInventory;

	public QuestInventory QuestInventory => m_questInventory;

	public BufferList BufferList => m_bufferList;

	public UserLabyrinthInfo Labyrinth
	{
		get
		{
			return m_Labyrinth;
		}
		set
		{
			m_Labyrinth = value;
		}
	}

	public List<UserGemStone> GemStone
	{
		get
		{
			return m_GemStone;
		}
		set
		{
			m_GemStone = value;
		}
	}

	public Dictionary<int, UserDrillInfo> UserDrills
	{
		get
		{
			return m_userDrills;
		}
		set
		{
			m_userDrills = value;
		}
	}

	public List<ItemInfo> EquipEffect
	{
		get
		{
			return m_equipEffect;
		}
		set
		{
			m_equipEffect = value;
		}
	}

	public bool CanUseProp { get; set; }

	public bool CanX2Exp { get; set; }

	public bool CanX3Exp { get; set; }

	public int Level
	{
		get
		{
			return m_character.Grade;
		}
		set
		{
			if (value != m_character.Grade)
			{
				m_character.Grade = value;
				OnLevelUp(value);
				OnPropertiesChanged();
			}
		}
	}

	public int LevelPlusBlood => LevelMgr.FindLevel(m_character.Grade).Blood;

	public ItemTemplateInfo MainWeapon
	{
		get
		{
			if (m_MainWeapon == null)
			{
				return null;
			}
			string text = m_MainWeapon.Template.TemplateID.ToString();
			if (text.Substring(4) == "2")
			{
				return ItemMgr.FindItemTemplate(m_MainWeapon.TemplateID);
			}
			return m_MainWeapon.Template;
		}
	}

	public ItemInfo Healstone
	{
		get
		{
			if (m_healstone == null)
			{
				return null;
			}
			return m_healstone;
		}
	}

	public ItemInfo SecondWeapon
	{
		get
		{
			if (m_currentSecondWeapon == null)
			{
				return null;
			}
			return m_currentSecondWeapon;
		}
	}

	public string ProcessLabyrinthAward { get; set; }

	public List<int> ViFarms => _viFarms;

	public Dictionary<int, int> Friends => _friends;

	public BaseRoom CurrentRoom
	{
		get
		{
			return m_currentRoom;
		}
		set
		{
			BaseRoom baseRoom = Interlocked.Exchange(ref m_currentRoom, value);
			if (baseRoom != null)
			{
				RoomMgr.ExitRoom(baseRoom, this);
			}
		}
	}

	public int GamePlayerId { get; set; }

	public long WorldbossBood { get; set; }

	public long AllWorldDameBoss { get; set; }

	public BaseSevenDoubleRoom CurrentSevenDoubleRoom
	{
		get
		{
			return m_currentSevenDoubleRoom;
		}
		set
		{
			m_currentSevenDoubleRoom = value;
		}
	}

	public MarryRoom CurrentMarryRoom
	{
		get
		{
			return m_currentMarryRoom;
		}
		set
		{
			m_currentMarryRoom = value;
		}
	}

	public bool IsInMarryRoom => m_currentMarryRoom != null;

	public int ServerID { get; set; }

	public Dictionary<string, object> TempProperties => m_tempProperties;

	public event PlayerEventHandle UseBuffer;

	public event PlayerEventHandle LevelUp;

	public event PlayerItemPropertyEventHandle AfterUsingItem;

	public event PlayerGameOverEventHandle GameOver;

	public event PlayerMissionOverEventHandle MissionOver;

	public event PlayerMissionTurnOverEventHandle MissionTurnOver;

	public event PlayerItemStrengthenEventHandle ItemStrengthen;

	public event PlayerShopEventHandle Paid;

	public event PlayerAdoptPetEventHandle AdoptPetEvent;

	public event PlayerNewGearEventHandle NewGearEvent;

	public event PlayerCropPrimaryEventHandle CropPrimaryEvent;

	public event PlayerSeedFoodPetEventHandle SeedFoodPetEvent;

	public event PlayerUserToemGemstoneEventHandle UserToemGemstonetEvent;

	public event PlayerUnknowQuestConditionEventHandle UnknowQuestConditionEvent;

	public event PlayerItemInsertEventHandle ItemInsert;

	public event PlayerItemFusionEventHandle ItemFusion;

	public event PlayerItemMeltEventHandle ItemMelt;

	public event PlayerGameKillEventHandel AfterKillingLiving;

	public event PlayerOwnConsortiaEventHandle GuildChanged;

	public event PlayerItemComposeEventHandle ItemCompose;

	public event GameKillDropEventHandel GameKillDrop;

	public GamePlayer(int playerId, string account, GameClient client, PlayerInfo info)
	{
		m_playerId = playerId;
		m_account = account;
		m_client = client;
		m_character = info;
		m_mainBag = new PlayerEquipInventory(this);
		m_BeadBag = new PlayerBeadInventory(this);
		m_propBag = new PlayerInventory(this, saveTodb: true, 49, 1, 0, autoStack: true);
		m_ConsortiaBag = new PlayerInventory(this, saveTodb: true, 100, 11, 0, autoStack: true);
		m_storeBag = new PlayerInventory(this, saveTodb: true, 20, 12, 0, autoStack: true);
		m_fightBag = new PlayerInventory(this, saveTodb: false, 3, 3, 0, autoStack: false);
		m_tempBag = new PlayerInventory(this, saveTodb: false, 100, 4, 0, autoStack: true);
		m_caddyBag = new PlayerInventory(this, saveTodb: false, 30, 5, 0, autoStack: true);
		m_farmBag = new PlayerInventory(this, saveTodb: true, 30, 13, 0, autoStack: true);
		m_vegetable = new PlayerInventory(this, saveTodb: true, 30, 14, 0, autoStack: true);
		m_food = new PlayerInventory(this, saveTodb: true, 30, 34, 0, autoStack: true);
		m_petEgg = new PlayerInventory(this, saveTodb: true, 30, 35, 0, autoStack: true);
		m_cardBag = new CardInventory(this, saveTodb: true, 100, 0);
		m_farm = new PlayerFarm(this, saveTodb: true, 30, 0);
		m_petBag = new PetInventory(this, saveTodb: true, 10, 8, 0);
		m_treasure = new PlayerTreasure(this, saveTodb: true);
		m_rank = new PlayerRank(this, saveTodb: true);
		m_playerProp = new PlayerProperty(this);
		m_dice = new PlayerDice(this);
		m_battle = new PlayerBattle(this, saveTodb: true);
		m_actives = new PlayerActives(this, saveTodb: true);
		m_questInventory = new QuestInventory(this);
		m_achievementInventory = new AchievementInventory(this);
		m_bufferList = new BufferList(this);
		m_fightBuffInfo = new List<BufferInfo>();
		m_equipEffect = new List<ItemInfo>();
		m_GemStone = new List<UserGemStone>();
		m_userDrills = new Dictionary<int, UserDrillInfo>();
		m_Labyrinth = null;
		GPAddPlus = 1.0;
		m_toemview = true;
		X = 646;
		Y = 1241;
		MarryMap = 0;
		LastChatTime = DateTime.Today;
		LastFigUpTime = DateTime.Today;
		LastDrillUpTime = DateTime.Today;
		LastOpenPack = DateTime.Today;
		beadRequestBtn1 = 0;
		beadRequestBtn2 = 0;
		beadRequestBtn3 = 0;
		beadIndex = 0;
		m_showPP = false;
		m_converter = new UTF8Encoding();
	}

	public PlayerInventory GetInventory(eBageType bageType)
	{
		return bageType switch
		{
			eBageType.MainBag => m_mainBag,
			eBageType.PropBag => m_propBag,
			eBageType.FightBag => m_fightBag,
			eBageType.TempBag => m_tempBag,
			eBageType.CaddyBag => m_caddyBag,
			eBageType.Consortia => m_ConsortiaBag,
			eBageType.Store => m_storeBag,
			eBageType.FarmBag => m_farmBag,
			eBageType.Vegetable => m_vegetable,
			eBageType.BeadBag => m_BeadBag,
			eBageType.Food => m_food,
			eBageType.PetEgg => m_petEgg,
			_ => throw new NotSupportedException($"Did not support this type bag: {bageType} PlayerID: {PlayerCharacter.ID} Nickname: {PlayerCharacter.NickName}"),
		};
	}

	public string GetInventoryName(eBageType bageType)
	{
		return bageType switch
		{
			eBageType.MainBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.Equip"),
			eBageType.PropBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.Prop"),
			eBageType.FightBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.FightBag"),
			eBageType.FarmBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.FarmBag"),
			eBageType.BeadBag => LanguageMgr.GetTranslation("Game.Server.GameObjects.BeadBag"),
			_ => bageType.ToString(),
		};
	}

	public PlayerInventory GetItemInventory(ItemTemplateInfo template)
	{
		return GetInventory(template.BagType);
	}

	public ItemInfo GetItemAt(eBageType bagType, int place)
	{
		return GetInventory(bagType)?.GetItemAt(place);
	}

	public ItemInfo GetItemByTemplateID(int templateID)
	{
		PlayerInventory inventory = GetInventory(eBageType.MainBag);
		ItemInfo itemByTemplateID = inventory.GetItemByTemplateID(31, templateID);
		if (itemByTemplateID == null)
		{
			inventory = GetInventory(eBageType.PropBag);
			itemByTemplateID = inventory.GetItemByTemplateID(0, templateID);
		}
		if (itemByTemplateID == null)
		{
			inventory = GetInventory(eBageType.Consortia);
			itemByTemplateID = inventory.GetItemByTemplateID(0, templateID);
		}
		return itemByTemplateID;
	}

	public int GetItemCount(int templateId)
	{
		return m_propBag.GetItemCount(templateId) + m_mainBag.GetItemCount(templateId) + m_ConsortiaBag.GetItemCount(templateId);
	}

	public bool AddItem(ItemInfo item)
	{
		AbstractInventory itemInventory = GetItemInventory(item.Template);
		return itemInventory.AddItem(item, itemInventory.BeginSlot);
	}

	public bool StackItemToAnother(ItemInfo item)
	{
		AbstractInventory itemInventory = GetItemInventory(item.Template);
		return itemInventory.StackItemToAnother(item);
	}

	public void UpdateItem(ItemInfo item)
	{
		m_mainBag.UpdateItem(item);
		m_propBag.UpdateItem(item);
	}

	public bool RemoveItem(ItemInfo item)
	{
		if (item.BagType == m_farmBag.BagType)
		{
			return m_farmBag.RemoveItem(item);
		}
		if (item.BagType == m_propBag.BagType)
		{
			return m_propBag.RemoveItem(item);
		}
		if (item.BagType == m_BeadBag.BagType)
		{
			return m_BeadBag.RemoveItem(item);
		}
		if (item.BagType == m_fightBag.BagType)
		{
			return m_fightBag.RemoveItem(item);
		}
		return m_mainBag.RemoveItem(item);
	}

	public void ClearFightBuffOneMatch()
	{
		List<BufferInfo> list = new List<BufferInfo>();
		foreach (BufferInfo fightBuff in FightBuffs)
		{
			if (fightBuff != null && fightBuff.Type >= 400 && fightBuff.Type <= 406)
			{
				list.Add(fightBuff);
			}
		}
		foreach (BufferInfo item in list)
		{
			FightBuffs.Remove(item);
		}
		list.Clear();
	}

	public void UpdateFightBuff(BufferInfo info)
	{
		int num = -1;
		for (int i = 0; i < FightBuffs.Count; i++)
		{
			if (info != null && info.Type == FightBuffs[i].Type)
			{
				FightBuffs[i] = info;
				num = info.Type;
			}
		}
		if (num == -1)
		{
			FightBuffs.Add(info);
		}
	}

	public void UpdatePveResult(string type, int value, bool isWin)
	{
		int num = 0;
		string text = "";
		switch (type)
		{
		case "qx":
			if (!isWin)
			{
				List<ItemInfo> info = null;
				DropInventory.CopyAllDrop(value, ref info);
				int num3 = value - 70000;
				if (value >= 70006)
				{
					num3 -= 2;
				}
				string title = "Phần thưởng tham gia Đảo hải tặc đợt " + num3;
				if (info != null)
				{
					WorldEventMgr.SendItemsToMail(info, PlayerCharacter.ID, PlayerCharacter.NickName, title);
				}
			}
			num = 0;
			break;
		case "yearmonter":
			Actives.Info.DamageNum = value;
			Actives.CreateYearMonterBoxState();
			num = 0;
			break;
		case "consortiaboss":
		{
			int num2 = value / 800;
			num = value / 1200;
			text = $"Lần tấn công này nhận {num2} cống hiến và {num} điểm vinh dự.";
			AddRichesOffer(num2);
			ConsortiaBossMgr.UpdateBlood(PlayerCharacter.ConsortiaID, value);
			ConsortiaBossMgr.UpdateRank(PlayerCharacter.ConsortiaID, value, num2, num, PlayerCharacter.NickName, PlayerCharacter.ID);
			break;
		}
		case "worldboss":
		{
			int num2 = value / 400;
			num = value / 1200;
			text = $"Lần tấn công này nhận {num2} danh vọng và {num} điểm vinh dự.";
			AddDamageScores(num2);
			RoomMgr.WorldBossRoom.UpdateRank(num2, num, PlayerCharacter.NickName);
			RoomMgr.WorldBossRoom.ReduceBlood(value);
			if (isWin)
			{
				RoomMgr.WorldBossRoom.FightOver();
			}
			break;
		}
		}
		AddHonor(num);
		if (!string.IsNullOrEmpty(text))
		{
			SendMessage(text);
		}
	}

	public void SendItemNotice(ItemInfo info, int typeView, int typeGet, string Name)
	{
		if (info == null)
		{
			return;
		}
		GSPacketIn gSPacketIn = new GSPacketIn(14);
		gSPacketIn.WriteString(PlayerCharacter.NickName);
		gSPacketIn.WriteInt(typeGet);
		gSPacketIn.WriteInt(info.TemplateID);
		gSPacketIn.WriteBoolean(info.IsBinds);
		gSPacketIn.WriteInt(typeView);
		gSPacketIn.WriteInt(info.Count);
		if (typeView == 3)
		{
			gSPacketIn.WriteString(Name);
		}
		if (info.IsTips || info.Template.Quality > 5)
		{
			GamePlayer[] allPlayers = WorldMgr.GetAllPlayers();
			GamePlayer[] array = allPlayers;
			foreach (GamePlayer gamePlayer in array)
			{
				gamePlayer.Out.SendTCP(gSPacketIn);
			}
		}
	}

	public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eItemNotice typeView, eItemNotice typeGet, string Name)
	{
		PlayerInventory inventory = GetInventory(bagType);
		if (cloneItem != null)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			if (!inventory.StackItemToAnother(cloneItem) && !inventory.AddItem(cloneItem))
			{
				list.Add(cloneItem);
			}
			BagFullSendToMail(list);
			SendItemNotice(cloneItem, (int)typeView, (int)typeGet, Name);
			return true;
		}
		return false;
	}

	public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count, eItemNotice typeView, eItemNotice typeGet)
	{
		return AddTemplate(cloneItem, bagType, count, typeView, typeGet, "Hộp bí ẩn");
	}

	public bool AddTemplate(ItemInfo cloneItem)
	{
		return AddTemplate(cloneItem, cloneItem.Template.BagType, cloneItem.Count, eItemNotice.NoneTypeView, eItemNotice.GoodsTipTypeView, "Hộp bí ẩn");
	}

	public bool AddTemplate(List<ItemInfo> infos, int count, eItemNotice typeView, eItemNotice typeGet)
	{
		if (infos != null)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (ItemInfo info in infos)
			{
				info.IsBinds = true;
				info.Count = count;
				if (!StackItemToAnother(info) && !AddItem(info))
				{
					list.Add(info);
				}
			}
			BagFullSendToMail(list);
			return true;
		}
		return false;
	}

	public bool AddTemplate(List<ItemInfo> infos, eItemNotice typeView, eItemNotice typeGet)
	{
		if (infos != null)
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (ItemInfo info in infos)
			{
				info.IsBinds = true;
				if (!StackItemToAnother(info) && !AddItem(info))
				{
					list.Add(info);
				}
			}
			BagFullSendToMail(list);
			return true;
		}
		return false;
	}

	public bool AddTemplate(List<ItemInfo> infos)
	{
		return AddTemplate(infos, eItemNotice.NoneTypeView, eItemNotice.GoodsTipTypeView);
	}

	public void BagFullSendToMail(List<ItemInfo> infos)
	{
		if (infos.Count <= 0)
		{
			return;
		}
		if (GameProperties.BagMailEnable)
		{
			WorldMgr.AddItemToMailBag(m_character.ID, infos);
			SendMessage("Hành trang đã đầy, vật phẩm chuyển vào -Túi ẩn- Vui lòng chờ 15p để nhận lại vật phẩm qua thư.");
			return;
		}
		bool flag = false;
		using (new PlayerBussiness())
		{
			flag = SendItemsToMail(infos, "", "Hành trang đã đầy. Gửi vào thư.", eMailType.BuyItem);
		}
		if (flag)
		{
			Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
		}
	}

	public bool FindEmptySlot(eBageType bagType)
	{
		PlayerInventory inventory = GetInventory(bagType);
		inventory.FindFirstEmptySlot();
		return inventory.FindFirstEmptySlot() > 0;
	}

	public bool RemoveTemplate(int templateId, int count)
	{
		int itemCount = m_mainBag.GetItemCount(templateId);
		int itemCount2 = m_propBag.GetItemCount(templateId);
		int itemCount3 = m_ConsortiaBag.GetItemCount(templateId);
		int num = itemCount + itemCount2 + itemCount3;
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null && num >= count)
		{
			if (itemCount > 0 && count > 0 && RemoveTempate(eBageType.MainBag, itemTemplateInfo, (itemCount > count) ? count : itemCount))
			{
				count = ((count >= itemCount) ? (count - itemCount) : 0);
			}
			if (itemCount2 > 0 && count > 0 && RemoveTempate(eBageType.PropBag, itemTemplateInfo, (itemCount2 > count) ? count : itemCount2))
			{
				count = ((count >= itemCount2) ? (count - itemCount2) : 0);
			}
			if (itemCount3 > 0 && count > 0 && RemoveTempate(eBageType.Consortia, itemTemplateInfo, (itemCount3 > count) ? count : itemCount3))
			{
				count = ((count >= itemCount3) ? (count - itemCount3) : 0);
			}
			if (count == 0)
			{
				return true;
			}
			if (log.IsErrorEnabled)
			{
				log.Error($"Item Remover Error：PlayerId {m_playerId} Remover TemplateId{templateId} Is Not Zero!");
			}
		}
		return false;
	}

	public bool RemoveTempate(eBageType bagType, ItemTemplateInfo template, int count)
	{
		return GetInventory(bagType)?.RemoveTemplate(template.TemplateID, count) ?? false;
	}

	public bool RemoveTemplate(ItemTemplateInfo template, int count)
	{
		return GetItemInventory(template)?.RemoveTemplate(template.TemplateID, count) ?? false;
	}

	public bool ClearTempBag()
	{
		TempBag.ClearBag();
		return true;
	}

	public bool ClearFightBag()
	{
		FightBag.ClearBag();
		return true;
	}

	public void ClearCaddyBag()
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < CaddyBag.Capalility; i++)
		{
			ItemInfo itemAt = CaddyBag.GetItemAt(i);
			if (itemAt != null)
			{
				ItemInfo itemInfo = ItemInfo.CloneFromTemplate(itemAt.Template, itemAt);
				itemInfo.Count = 1;
				list.Add(itemInfo);
			}
		}
		CaddyBag.ClearBag();
		AddTemplate(list);
	}

	public void ClearStoreBag()
	{
		for (int i = 0; i < StoreBag.Capalility; i++)
		{
			ItemInfo itemAt = StoreBag.GetItemAt(i);
			if (itemAt == null)
			{
				continue;
			}
			if (itemAt.Template.BagType == eBageType.PropBag)
			{
				int place = PropBag.FindFirstEmptySlot();
				if (PropBag.StackItemToAnother(itemAt) || PropBag.AddItemTo(itemAt, place))
				{
					StoreBag.TakeOutItem(itemAt);
				}
			}
			else
			{
				int place = MainBag.FindFirstEmptySlot(31);
				if (MainBag.StackItemToAnother(itemAt) || MainBag.AddItemTo(itemAt, place))
				{
					StoreBag.TakeOutItem(itemAt);
				}
			}
		}
		List<ItemInfo> items = StoreBag.GetItems();
		if (items.Count > 0)
		{
			StoreBag.SendAllItemsToMail("Hệ thống", "Vật phẩm trả về từ Tiệm rèn.", eMailType.StoreCanel);
		}
	}

	public bool IsConsortia()
	{
		ConsortiaInfo consortiaInfo = ConsortiaMgr.FindConsortiaInfo(PlayerCharacter.ConsortiaID);
		return consortiaInfo != null;
	}

	public void OnUseBuffer()
	{
		if (UseBuffer != null)
		{
			UseBuffer(this);
		}
	}

	public void AddBeadEffect(ItemInfo item)
	{
		m_equipEffect.Add(item);
	}

	public void BeginAllChanges()
	{
		BeginChanges();
		m_bufferList.BeginChanges();
		m_mainBag.BeginChanges();
		m_propBag.BeginChanges();
		m_BeadBag.BeginChanges();
		m_farmBag.BeginChanges();
		m_vegetable.BeginChanges();
		m_cardBag.BeginChanges();
	}

	public void CommitAllChanges()
	{
		CommitChanges();
		m_bufferList.CommitChanges();
		m_mainBag.CommitChanges();
		m_propBag.CommitChanges();
		m_BeadBag.BeginChanges();
		m_farmBag.CommitChanges();
		m_vegetable.CommitChanges();
		m_cardBag.CommitChanges();
	}

	public void BeginChanges()
	{
		Interlocked.Increment(ref m_changed);
	}

	public void CommitChanges()
	{
		Interlocked.Decrement(ref m_changed);
		OnPropertiesChanged();
	}

	protected void OnPropertiesChanged()
	{
		if (m_changed <= 0)
		{
			if (m_changed < 0)
			{
				log.Error("Player changed count < 0");
				Thread.VolatileWrite(ref m_changed, 0);
			}
			UpdateProperties();
		}
	}

	public void UpdateDrill(int index, UserDrillInfo drill)
	{
		m_userDrills[index] = drill;
	}

	public int GetDrillLevel(int place)
	{
		for (int i = 0; i < UserDrills.Count; i++)
		{
			if (UserDrills[i].BeadPlace == place)
			{
				return UserDrills[i].HoleLv;
			}
		}
		return 0;
	}

	public void UpdateBadgeId(int Id)
	{
		m_character.badgeID = Id;
	}

	public void UpdateTimeBox(int receiebox, int receieGrade, int needGetBoxTime)
	{
		m_character.receiebox = receiebox;
		m_character.receieGrade = receieGrade;
		m_character.needGetBoxTime = needGetBoxTime;
	}

	public string GetFightFootballStyle(int team)
	{
		if (team == 1)
		{
			return CreateFightFootballStyle().Split(';')[0];
		}
		return CreateFightFootballStyle().Split(';')[1];
	}

	public string CreateFightFootballStyle()
	{
		ItemInfo itemAt = GetItemAt(eBageType.MainBag, 0);
		string text = ((itemAt == null) ? "" : (itemAt.TemplateID + "|" + itemAt.Template.Pic));
		string text2 = text;
		string text3 = text;
		for (int i = 0; i < StyleIndex.Length; i++)
		{
			text2 += ",";
			text3 += ",";
			if (StyleIndex[i] == 11)
			{
				if (PlayerCharacter.Sex)
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(13573);
					object obj = text2;
					text2 = string.Concat(obj, itemTemplateInfo.TemplateID, "|", itemTemplateInfo.Pic);
					itemTemplateInfo = ItemMgr.FindItemTemplate(13572);
					object obj2 = text3;
					text3 = string.Concat(obj2, itemTemplateInfo.TemplateID, "|", itemTemplateInfo.Pic);
				}
				else
				{
					ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(13575);
					object obj3 = text2;
					text2 = string.Concat(obj3, itemTemplateInfo.TemplateID, "|", itemTemplateInfo.Pic);
					itemTemplateInfo = ItemMgr.FindItemTemplate(13574);
					object obj4 = text3;
					text3 = string.Concat(obj4, itemTemplateInfo.TemplateID, "|", itemTemplateInfo.Pic);
				}
			}
			else if (StyleIndex[i] == 6)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(70396);
				string text4 = itemTemplateInfo.TemplateID + "|" + itemTemplateInfo.Pic;
				text2 += text4;
				text3 += text4;
			}
			else
			{
				itemAt = GetItemAt(eBageType.MainBag, StyleIndex[i]);
				if (itemAt != null)
				{
					string text5 = itemAt.TemplateID + "|" + itemAt.Pic;
					text2 += text5;
					text3 += text5;
				}
			}
		}
		return text2 + ";" + text3;
	}

	public void RemoveFightFootballStyle()
	{
		ItemInfo itemAt = GetItemAt(eBageType.MainBag, 0);
		string text = ((itemAt == null) ? "" : (itemAt.TemplateID + "|" + itemAt.Template.Pic));
		for (int i = 0; i < StyleIndex.Length; i++)
		{
			text += ",";
			itemAt = GetItemAt(eBageType.MainBag, StyleIndex[i]);
			if (itemAt != null)
			{
				object obj = text;
				text = string.Concat(obj, itemAt.TemplateID, "|", itemAt.Pic);
			}
		}
		if (!string.IsNullOrEmpty(text))
		{
			PlayerCharacter.Style = text;
		}
		OnPropertiesChanged();
	}

	public void UpdateProperties()
	{
		Out.SendUpdatePrivateInfo(m_character);
		GSPacketIn pkg = Out.SendUpdatePublicPlayer(m_character, m_battle.MatchInfo);
		if (m_currentRoom != null)
		{
			m_currentRoom.SendToAll(pkg, this);
		}
	}

	public int AddGold(int value)
	{
		if (value > 0)
		{
			m_character.Gold += value;
			if (m_character.Gold == int.MinValue)
			{
				m_character.Gold = int.MaxValue;
				SendMessage("Vàng đã đạt gới hạn, không thể nhận thêm.");
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveGold(int value)
	{
		if (value > 0 && value <= m_character.Gold)
		{
			m_character.Gold -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddMoney(int value)
	{
		if (value > 0)
		{
			m_character.Money += value;
			if (m_character.Money == int.MinValue)
			{
				m_character.Money = int.MaxValue;
				SendMessage("Xu đã đạt gới hạn, không thể nhận thêm.");
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddHonor(int value)
	{
		if (value > 0)
		{
			m_character.myHonor += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddTotem(int value)
	{
		if (value > 0)
		{
			m_character.totemId += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddMaxHonor(int value)
	{
		if (value > 0)
		{
			m_character.MaxBuyHonor += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveMoney(int value)
	{
		if (value > 0 && value <= m_character.Money)
		{
			m_character.Money -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public bool ActiveMoneyEnable(int value)
	{
		if (!GameProperties.IsActiveMoney)
		{
			return MoneyDirect(value);
		}
		if (value < 1 || value > int.MaxValue)
		{
			return false;
		}
		if (Actives.Info.ActiveMoney >= value)
		{
			RemoveActiveMoney(value);
			RemoveMoney(value);
			return true;
		}
		SendMessage($"Xu năng động không đủ. Hiện tại bạn có {Actives.Info.ActiveMoney} Xu năng động");
		return false;
	}

	public int AddActiveMoney(int value)
	{
		if (value > 0 && GameProperties.IsActiveMoney)
		{
			Actives.Info.ActiveMoney += value;
			if (Actives.Info.ActiveMoney == int.MinValue)
			{
				Actives.Info.ActiveMoney = int.MaxValue;
				SendMessage("Xu năng động đã đạt gới hạn, không thể nhận thêm.");
			}
			else
			{
				SendHideMessage($"Hệ thống vừa thêm vào {value} Xu năng động , nâng Xu năng động lên {Actives.Info.ActiveMoney} Xu");
			}
			return value;
		}
		return 0;
	}

	public int RemoveActiveMoney(int value)
	{
		if (value > 0 && value <= Actives.Info.ActiveMoney)
		{
			Actives.Info.ActiveMoney -= value;
			SendHideMessage($"Bạn vừa tiêu hao {value} Xu năng động, còn lại {Actives.Info.ActiveMoney} Xu năng động");
			return value;
		}
		return 0;
	}

	public int AddLeagueMoney(int value)
	{
		if (value > 0)
		{
			m_character.LeagueMoney += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveLeagueMoney(int value)
	{
		if (value > 0 && value <= m_character.LeagueMoney)
		{
			m_character.LeagueMoney -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddHardCurrency(int value)
	{
		if (value > 0)
		{
			m_character.hardCurrency += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveHardCurrency(int value)
	{
		if (value > 0 && value <= m_character.hardCurrency)
		{
			m_character.hardCurrency -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public void AddPrestige(bool isWin)
	{
		BattleData.AddPrestige(isWin);
	}

	public void UpdateHonor(string honor)
	{
		PlayerCharacter.Honor = honor;
		if (Rank.IsRank(honor))
		{
			MainBag.UpdatePlayerProperties();
		}
	}

	public void UpdateRestCount()
	{
		BattleData.Update();
	}

	public void RemoveFistGetPet()
	{
		m_character.IsFistGetPet = false;
		m_character.LastRefreshPet = DateTime.Now.AddDays(-1.0);
	}

	public void RemoveLastRefreshPet()
	{
		m_character.LastRefreshPet = DateTime.Now;
	}

	public void UpdateAnswerSite(int id)
	{
		if (PlayerCharacter.AnswerSite < id)
		{
			PlayerCharacter.AnswerSite = id;
		}
		UpdateWeaklessGuildProgress();
		Out.SendWeaklessGuildProgress(PlayerCharacter);
	}

	public void UpdateWeaklessGuildProgress()
	{
		if (PlayerCharacter.weaklessGuildProgress == null)
		{
			PlayerCharacter.weaklessGuildProgress = Base64.decodeToByteArray(PlayerCharacter.WeaklessGuildProgressStr);
		}
		PlayerCharacter.CheckLevelFunction();
		if (PlayerCharacter.Grade == 1)
		{
			PlayerCharacter.openFunction(Step.POP_MOVE);
		}
		if (PlayerCharacter.IsOldPlayer)
		{
			PlayerCharacter.openFunction(Step.OLD_PLAYER);
		}
		PlayerCharacter.WeaklessGuildProgressStr = Base64.encodeByteArray(PlayerCharacter.weaklessGuildProgress);
	}

	public bool canUpLv(int exp, int _curLv)
	{
		List<int> list = GameProperties.VIPExp();
		return (exp >= list[0] && _curLv == 0) || (exp >= list[1] && _curLv == 1) || (exp >= list[2] && _curLv == 2) || (exp >= list[3] && _curLv == 3) || (exp >= list[4] && _curLv == 4) || (exp >= list[5] && _curLv == 5) || (exp >= list[6] && _curLv == 6) || (exp >= list[7] && _curLv == 7) || (exp >= list[8] && _curLv == 8) || (exp >= list[9] && _curLv == 9) || (exp >= list[10] && _curLv == 10) || (exp >= list[11] && _curLv == 11);
	}

	public void AddExpVip(int value)
	{
		List<int> list = GameProperties.VIPExp();
		if (value >= 10)
		{
			m_character.VIPExp += value / 10;
		}
		for (int i = 0; i < list.Count; i++)
		{
			int vIPExp = m_character.VIPExp;
			int vIPLevel = m_character.VIPLevel;
			if (vIPLevel == 12)
			{
				m_character.VIPExp = list[11];
				break;
			}
			if (vIPLevel < 12 && canUpLv(vIPExp, vIPLevel))
			{
				m_character.VIPLevel++;
			}
		}
		if (m_character.IsVIPExpire())
		{
			Out.SendOpenVIP(PlayerCharacter);
		}
	}

	public int AddCardSoul(int value)
	{
		if (value > 0)
		{
			m_character.CardSoul += value;
			if (m_character.CardSoul == int.MinValue)
			{
				m_character.CardSoul = int.MaxValue;
				SendMessage("Thẻ hồn đã đạt cảnh giới cao nhất, không thể nhận thêm.");
			}
			return value;
		}
		return 0;
	}

	public int RemoveCardSoul(int value)
	{
		if (value > 0 && value <= m_character.CardSoul)
		{
			m_character.CardSoul -= value;
			return value;
		}
		return 0;
	}

	public int AddScore(int value)
	{
		if (value > 0)
		{
			m_character.Score += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveScore(int value)
	{
		if (value > 0 && value <= m_character.Score)
		{
			m_character.Score -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddDamageScores(int value)
	{
		if (value > 0)
		{
			m_character.damageScores += value;
			if (m_character.damageScores == int.MinValue)
			{
				m_character.damageScores = int.MaxValue;
				SendMessage("Tích lũy đã đạt cảnh giới cao nhất, không thể nhận thêm.");
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveDamageScores(int value)
	{
		if (value > 0 && value <= m_character.damageScores)
		{
			m_character.damageScores -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddPetScore(int value)
	{
		if (value > 0)
		{
			m_character.petScore += value;
			if (m_character.petScore == int.MinValue)
			{
				m_character.petScore = int.MaxValue;
				SendMessage("Tích lũy đã đạt cảnh giới cao nhất, không thể nhận thêm.");
			}
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemovePetScore(int value)
	{
		if (value > 0 && value <= m_character.petScore)
		{
			m_character.petScore -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddmyHonor(int value)
	{
		if (value > 0)
		{
			m_character.myHonor += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemovemyHonor(int value)
	{
		if (value > 0 && value <= m_character.myHonor)
		{
			m_character.myHonor -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddMedal(int value)
	{
		if (value > 0)
		{
			m_character.medal += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveMedal(int value)
	{
		if (value > 0 && value <= m_character.medal)
		{
			m_character.medal -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddOffer(int value)
	{
		return AddOffer(value, IsRate: true);
	}

	public int AddOffer(int value, bool IsRate)
	{
		if (value > 0)
		{
			if (AntiAddictionMgr.ISASSon)
			{
				value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
			}
			if (IsRate)
			{
				value *= (((int)OfferAddPlus == 0) ? 1 : ((int)OfferAddPlus));
			}
			m_character.Offer += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveOffer(int value)
	{
		if (value > 0)
		{
			if (value >= m_character.Offer)
			{
				value = m_character.Offer;
			}
			m_character.Offer -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int RemoveGiftToken(int value)
	{
		if (value > 0 && value <= m_character.GiftToken)
		{
			m_character.GiftToken -= value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddGP(int gp)
	{
		if (gp >= 0)
		{
			if (AntiAddictionMgr.ISASSon)
			{
				gp = (int)((double)gp * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
			}
			gp = (int)((float)gp * RateMgr.GetRate(eRateType.Experience_Rate));
			if (GPAddPlus > 0.0)
			{
				gp = (int)((double)gp * GPAddPlus);
			}
			m_character.GP += gp;
			if (m_character.GP < 1)
			{
				m_character.GP = 1;
			}
			Level = LevelMgr.GetLevel(m_character.GP);
			int maxLevel = LevelMgr.MaxLevel;
			LevelInfo levelInfo = LevelMgr.FindLevel(maxLevel);
			if (Level == maxLevel && levelInfo != null)
			{
				m_character.GP = levelInfo.GP;
				int num = gp / 100;
				if (num > 0)
				{
					AddOffer(num);
					SendHideMessage($"Max level kinh nghiệm quy đổi thành {num} công trạng");
				}
			}
			UpdateFightPower();
			OnPropertiesChanged();
			return gp;
		}
		return 0;
	}

	public void UpdateLevel()
	{
		Level = LevelMgr.GetLevel(m_character.GP);
		int maxLevel = LevelMgr.MaxLevel;
		LevelInfo levelInfo = LevelMgr.FindLevel(maxLevel);
		if (Level == maxLevel && levelInfo != null)
		{
			m_character.GP = levelInfo.GP;
		}
	}

	public int RemoveGP(int gp)
	{
		if (gp > 0)
		{
			m_character.GP -= gp;
			if (m_character.GP < 1)
			{
				m_character.GP = 1;
			}
			int level = LevelMgr.GetLevel(m_character.GP);
			if (Level > level)
			{
				m_character.GP += gp;
			}
			UpdateLevel();
			return gp;
		}
		return 0;
	}

	public int AddRobRiches(int value)
	{
		if (value > 0)
		{
			if (AntiAddictionMgr.ISASSon)
			{
				value = (int)((double)value * AntiAddictionMgr.GetAntiAddictionCoefficient(PlayerCharacter.AntiAddiction));
			}
			m_character.RichesRob += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddRichesOffer(int value)
	{
		if (value > 0)
		{
			m_character.RichesOffer += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public int AddGiftToken(int value)
	{
		if (value > 0)
		{
			m_character.GiftToken += value;
			OnPropertiesChanged();
			return value;
		}
		return 0;
	}

	public bool CanEquip(ItemTemplateInfo item)
	{
		bool flag = true;
		string message = "";
		if (!item.CanEquip)
		{
			flag = false;
			message = LanguageMgr.GetTranslation("Game.Server.GameObjects.NoEquip");
		}
		else if (item.NeedSex != 0 && item.NeedSex != (m_character.Sex ? 1 : 2))
		{
			flag = false;
			message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanEquip");
		}
		else if (m_character.Grade < item.NeedLevel)
		{
			flag = false;
			message = LanguageMgr.GetTranslation("Game.Server.GameObjects.CanLevel");
		}
		if (!flag)
		{
			Out.SendMessage(eMessageType.ERROR, message);
		}
		return flag;
	}

	public void UpdateBaseProperties(int attack, int defence, int agility, int lucky, int hp)
	{
		if (attack != m_character.Attack || defence != m_character.Defence || agility != m_character.Agility || lucky != m_character.Luck)
		{
			m_character.Attack = attack;
			m_character.Defence = defence;
			m_character.Agility = agility;
			m_character.Luck = lucky;
			OnPropertiesChanged();
		}
		m_character.hp = (int)(((double)(hp + LevelPlusBlood + m_character.Defence / 10) + GetGoldBlood()) * GetBaseBlood());
	}

	public void UpdateStyle(string style, string colors, string skin)
	{
		if (style != m_character.Style || colors != m_character.Colors || skin != m_character.Skin)
		{
			m_character.Style = style;
			m_character.Colors = colors;
			m_character.Skin = skin;
			OnPropertiesChanged();
		}
	}

	public void UpdateFightPower()
	{
		int num = 0;
		FightPower = 0;
		int num2 = 0;
		num2 += PlayerCharacter.hp;
		num += PlayerCharacter.Attack;
		num += PlayerCharacter.Defence;
		num += PlayerCharacter.Agility;
		num += PlayerCharacter.Luck;
		FightPower += (int)((double)(num + 1000) * (GetBaseAttack() * GetBaseAttack() * GetBaseAttack() + 3.5 * GetBaseDefence() * GetBaseDefence() * GetBaseDefence()) / 100000000.0 + (double)num2 * 0.95);
		if (m_currentSecondWeapon != null)
		{
			FightPower += (int)((double)m_currentSecondWeapon.Template.Property7 * Math.Pow(1.1, m_currentSecondWeapon.StrengthenLevel));
		}
		PlayerCharacter.FightPower = FightPower;
	}

	public void UpdateHide(int hide)
	{
		if (hide != m_character.Hide)
		{
			m_character.Hide = hide;
			OnPropertiesChanged();
		}
	}

	public void UpdateWeapon(ItemInfo item)
	{
		if (item != m_MainWeapon)
		{
			m_MainWeapon = item;
			OnPropertiesChanged();
		}
	}

	public void UpdatePet(UsersPetinfo pet)
	{
		m_pet = pet;
	}

	public void UpdateHealstone(ItemInfo item)
	{
		m_healstone = item;
	}

	public bool RemoveHealstone()
	{
		ItemInfo itemAt = m_mainBag.GetItemAt(18);
		return itemAt != null && itemAt.Count > 0 && m_mainBag.RemoveCountFromStack(itemAt, 1);
	}

	public void UpdateSecondWeapon(ItemInfo item)
	{
		if (item != m_currentSecondWeapon)
		{
			m_currentSecondWeapon = item;
			OnPropertiesChanged();
		}
	}

	public void HideEquip(int categoryID, bool hide)
	{
		if (categoryID >= 0 && categoryID < 10)
		{
			EquipShowImp(categoryID, (!hide) ? 1 : 2);
		}
	}

	public void ApertureEquip(int level)
	{
		EquipShowImp(0, (level < 5) ? 1 : ((level < 7) ? 2 : 3));
	}

	private void EquipShowImp(int categoryID, int para)
	{
		UpdateHide((int)((double)m_character.Hide + Math.Pow(10.0, categoryID) * (double)(para - m_character.Hide / (int)Math.Pow(10.0, categoryID) % 10)));
	}

	public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int moneys, int SpareMoney)
	{
	}

	public bool Login()
	{
		if (WorldMgr.AddPlayer(m_character.ID, this))
		{
			try
			{
				if (LoadFromDatabase())
				{
					Out.SendLoginSuccess();
					Out.SendUpdatePublicPlayer(PlayerCharacter, BattleData.MatchInfo);
					Out.SendWeaklessGuildProgress(PlayerCharacter);
					Out.SendNecklaceStrength(PlayerCharacter);
					Out.SendMissionEnergy(PlayerCharacter);
					Out.SendUpdateOneKeyFinish(PlayerCharacter);
					Out.SendDateTime();
					Out.SendDailyAward(PlayerCharacter);
					LoadMarryMessage();
					if (!m_showPP)
					{
						m_playerProp.ViewCurrent();
						m_showPP = true;
					}
					int iD = PlayerCharacter.ID;
					Out.SendUserRanks(iD, Rank.GetRank());
					Out.SendLuckStoneEnable(iD);
					Out.SendActivityList(iD);
					Out.SendFindBackIncome(iD);
					Out.SendPlayerDrill(iD, UserDrills);
					Out.SendOpenVIP(PlayerCharacter);
					SendPkgLimitGrate();
					Out.SendKingBlessMain(iD);
					if (Actives.IsDiceOpen())
					{
						Out.SendDiceActiveOpen(Dice, iD);
					}
					if (GameProperties.IsPromotePackageOpen)
					{
						Out.SendGrowthPackageOpen(iD, Actives.Info.AvailTime);
					}
					Actives.SendEvent();
					MainBag.UpdatePlayerProperties();
					GameServer.Instance.LoginServer.SendGetLightriddleInfo(iD);
					m_playerState = ePlayerState.Online;
					return true;
				}
				WorldMgr.RemovePlayer(m_character.ID);
			}
			catch (Exception exception)
			{
				log.Error("Error Login!", exception);
			}
			return false;
		}
		return false;
	}

	public void SendPkgLimitGrate()
	{
		int iD = PlayerCharacter.ID;
		if (PlayerCharacter.Grade >= 20)
		{
			if (RoomMgr.WorldBossRoom.worldOpen)
			{
				Out.SendOpenWorldBoss(X, Y);
			}
			if (ActiveSystemMgr.IsLeagueOpen)
			{
				Out.SendLeagueNotice(iD, BattleData.MatchInfo.restCount, BattleData.maxCount, 1);
			}
			else
			{
				Out.SendLeagueNotice(iD, BattleData.MatchInfo.restCount, BattleData.maxCount, 2);
			}
			if (ActiveSystemMgr.IsFightFootballTime)
			{
				Out.SendFightFootballTimeOpenClose(iD, result: true);
			}
		}
		if (PlayerCharacter.Grade >= 30)
		{
			Out.SendPlayerFigSpiritinit(iD, GemStone);
		}
		if (PlayerCharacter.Grade >= 15)
		{
			if (ActiveSystemMgr.IsBattleGoundOpen)
			{
				Out.SendBattleGoundOpen(iD);
			}
			if (Actives.IsDragonBoatOpen())
			{
				Out.SendDragonBoat(PlayerCharacter);
			}
			if (ActiveSystemMgr.LanternriddlesOpen)
			{
				Out.SendLanternriddlesOpen(iD, isOpen: true);
			}
		}
		int grade = PlayerCharacter.Grade;
		if (PlayerCharacter.Grade >= 13 && Actives.IsPyramidOpen())
		{
			Out.SendPyramidOpenClose(Actives.PyramidConfig);
		}
		if (Actives.IsYearMonsterOpen())
		{
			Out.SendCatchBeastOpen(iD, isOpen: true);
		}
	}

	public void LoadMarryMessage()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		MarryApplyInfo[] playerMarryApply = playerBussiness.GetPlayerMarryApply(PlayerCharacter.ID);
		if (playerMarryApply == null)
		{
			return;
		}
		MarryApplyInfo[] array = playerMarryApply;
		foreach (MarryApplyInfo marryApplyInfo in array)
		{
			switch (marryApplyInfo.ApplyType)
			{
			case 1:
				Out.SendPlayerMarryApply(this, marryApplyInfo.ApplyUserID, marryApplyInfo.ApplyUserName, marryApplyInfo.LoveProclamation, marryApplyInfo.ID);
				break;
			case 2:
				Out.SendMarryApplyReply(this, marryApplyInfo.ApplyUserID, marryApplyInfo.ApplyUserName, marryApplyInfo.ApplyResult, isApplicant: true, marryApplyInfo.ID);
				if (!marryApplyInfo.ApplyResult)
				{
					Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
				}
				break;
			case 3:
				Out.SendPlayerDivorceApply(this, result: true, isProposer: false);
				break;
			}
		}
	}

	public void ChargeToUser()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		int money = 0;
		string title = "Thông báo nạp thẻ.";
		if (!playerBussiness.ChargeToUser(m_character.UserName, ref money, m_character.NickName))
		{
			return;
		}
		bool flag = false;
		if (GameProperties.IsDDTMoneyActive)
		{
			AddGiftToken(money);
			string content = $"Bạn vừa chuyển thành công {money} Xu khóa";
			if (money > 0)
			{
				flag = SendMailToUser(playerBussiness, content, title, eMailType.Default);
			}
		}
		else
		{
			AddMoney(money);
		}
		if (flag)
		{
			Out.SendMailResponse(PlayerCharacter.ID, eMailRespose.Receiver);
		}
	}

	public bool LoadFromDatabase()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		PlayerInfo userSingleByUserID = playerBussiness.GetUserSingleByUserID(m_character.ID);
		if (userSingleByUserID == null)
		{
			Out.SendKitoff(LanguageMgr.GetTranslation("UserLoginHandler.Forbid"));
			Client.Disconnect();
			return false;
		}
		m_character = userSingleByUserID;
		m_character.Texp = playerBussiness.GetUserTexpInfoSingle(m_character.ID);
		if (m_character.Texp.IsValidadteTexp())
		{
			m_character.Texp.texpCount = 0;
		}
		if (m_character.Grade > 19)
		{
			LoadGemStone(playerBussiness);
		}
		LoadDrills(playerBussiness);
		ChargeToUser();
		int[] updatedSlots = new int[3] { 0, 1, 2 };
		Out.SendUpdateInventorySlot(FightBag, updatedSlots);
		UpdateWeaklessGuildProgress();
		UpdateItemForUser(1);
		ChecVipkExpireDay();
		UpdateLevel();
		UpdatePet(m_petBag.GetPetIsEquip());
		if (m_character.IsValidadteTimeBox())
		{
			m_character.TimeBox = DateTime.Now;
			m_character.receiebox = 0;
			m_character.MaxBuyHonor = 0;
			m_character.GetSoulCount = 30;
			m_character.SpaPubGoldRoomLimit = 0;
			m_character.SpaPubMoneyRoomLimit = 0;
			m_character.lastLuckNum = 0;
			m_character.luckyNum = -1;
			m_farm.ResetFarmProp();
			m_battle.Reset();
			m_actives.ResetChristmas();
			m_actives.Info.activityTanabataNum = 0;
		}
		Dice.LoadFromDatabase();
		m_pvepermissions = (string.IsNullOrEmpty(m_character.PvePermission) ? InitPvePermission() : m_character.PvePermission.ToCharArray());
		LoadPvePermission();
		_friends = new Dictionary<int, int>();
		_friends = playerBussiness.GetFriendsIDAll(m_character.ID);
		_viFarms = new List<int>();
		m_character.State = 1;
		ClearStoreBag();
		ClearCaddyBag();
		playerBussiness.UpdateUserTexpInfo(m_character.Texp);
		playerBussiness.UpdatePlayer(m_character);
		return true;
	}

	public void TestQuest()
	{
		using ProduceBussiness produceBussiness = new ProduceBussiness();
		QuestInfo[] aLlQuest = produceBussiness.GetALlQuest();
		QuestInfo[] array = aLlQuest;
		foreach (QuestInfo info in array)
		{
			QuestInventory.AddQuest(info, out var _);
		}
	}

	public void SendConsortiaBossOpenClose(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(129, PlayerCharacter.ID);
		gSPacketIn.WriteByte(31);
		gSPacketIn.WriteByte((byte)type);
		SendTCP(gSPacketIn);
	}

	public void SendConsortiaBossInfo(ConsortiaInfo info)
	{
		RankingPersonInfo rankingPersonInfo = null;
		List<RankingPersonInfo> list = new List<RankingPersonInfo>();
		foreach (RankingPersonInfo value in info.RankList.Values)
		{
			if (value.Name == PlayerCharacter.NickName)
			{
				rankingPersonInfo = value;
			}
			else
			{
				list.Add(value);
			}
		}
		GSPacketIn gSPacketIn = new GSPacketIn(129, PlayerCharacter.ID);
		gSPacketIn.WriteByte(30);
		gSPacketIn.WriteByte((byte)info.bossState);
		gSPacketIn.WriteBoolean(rankingPersonInfo != null);
		if (rankingPersonInfo != null)
		{
			gSPacketIn.WriteInt(rankingPersonInfo.ID);
			gSPacketIn.WriteInt(rankingPersonInfo.TotalDamage);
			gSPacketIn.WriteInt(rankingPersonInfo.Honor);
			gSPacketIn.WriteInt(rankingPersonInfo.Damage);
		}
		gSPacketIn.WriteByte((byte)list.Count);
		foreach (RankingPersonInfo item in list)
		{
			gSPacketIn.WriteString(item.Name);
			gSPacketIn.WriteInt(item.ID);
			gSPacketIn.WriteInt(item.TotalDamage);
			gSPacketIn.WriteInt(item.Honor);
			gSPacketIn.WriteInt(item.Damage);
		}
		gSPacketIn.WriteByte((byte)info.extendAvailableNum);
		gSPacketIn.WriteDateTime(info.endTime);
		gSPacketIn.WriteInt(info.callBossLevel);
		SendTCP(gSPacketIn);
	}

	public GSPacketIn UpdateGoodsCount()
	{
		return Out.SendUpdateGoodsCount(PlayerCharacter, null, null);
	}

	public void LoadDrills(PlayerBussiness db)
	{
		m_userDrills = db.GetPlayerDrillByID(m_character.ID);
		if (m_userDrills.Count != 0)
		{
			return;
		}
		List<int> list = new List<int>();
		list.Add(13);
		list.Add(14);
		list.Add(15);
		list.Add(16);
		list.Add(17);
		list.Add(18);
		List<int> list2 = list;
		List<int> list3 = new List<int>();
		list3.Add(0);
		list3.Add(1);
		list3.Add(2);
		list3.Add(3);
		list3.Add(4);
		list3.Add(5);
		List<int> list4 = list3;
		for (int i = 0; i < list2.Count; i++)
		{
			UserDrillInfo userDrillInfo = new UserDrillInfo();
			userDrillInfo.UserID = m_character.ID;
			userDrillInfo.BeadPlace = list2[i];
			userDrillInfo.HoleLv = 0;
			userDrillInfo.HoleExp = 0;
			userDrillInfo.DrillPlace = list4[i];
			db.AddUserUserDrill(userDrillInfo);
			if (!m_userDrills.ContainsKey(userDrillInfo.DrillPlace))
			{
				m_userDrills.Add(userDrillInfo.DrillPlace, userDrillInfo);
			}
		}
	}

	public void LoadGemStone(PlayerBussiness db)
	{
		m_GemStone = db.GetSingleGemStones(m_character.ID);
		if (m_GemStone.Count == 0)
		{
			List<int> list = new List<int>();
			list.Add(11);
			list.Add(5);
			list.Add(2);
			list.Add(3);
			list.Add(13);
			List<int> list2 = list;
			List<int> list3 = new List<int>();
			list3.Add(100002);
			list3.Add(100003);
			list3.Add(100001);
			list3.Add(100004);
			list3.Add(100005);
			List<int> list4 = list3;
			for (int i = 0; i < list2.Count; i++)
			{
				UserGemStone userGemStone = new UserGemStone();
				userGemStone.ID = 0;
				userGemStone.UserID = m_character.ID;
				userGemStone.FigSpiritId = list4[i];
				userGemStone.FigSpiritIdValue = "0,0,0|0,0,1|0,0,2";
				userGemStone.EquipPlace = list2[i];
				m_GemStone.Add(userGemStone);
				db.AddUserGemStone(userGemStone);
			}
		}
	}

	public UserGemStone GetGemStone(int place)
	{
		foreach (UserGemStone item in m_GemStone)
		{
			if (place == item.EquipPlace)
			{
				return item;
			}
		}
		return null;
	}

	public void UpdateGemStone(int place, UserGemStone gem)
	{
		for (int i = 0; i < m_GemStone.Count; i++)
		{
			if (place == m_GemStone[i].EquipPlace)
			{
				m_GemStone[i] = gem;
				break;
			}
		}
	}

	public void UpdateItemForUser(object state)
	{
		m_battle.LoadFromDatabase();
		m_mainBag.LoadFromDatabase();
		m_propBag.LoadFromDatabase();
		m_ConsortiaBag.LoadFromDatabase();
		m_BeadBag.LoadFromDatabase();
		m_farmBag.LoadFromDatabase();
		m_petBag.LoadFromDatabase();
		m_storeBag.LoadFromDatabase();
		m_cardBag.LoadFromDatabase();
		m_questInventory.LoadFromDatabase(m_character.ID);
		m_achievementInventory.LoadFromDatabase(m_character.ID);
		m_bufferList.LoadFromDatabase(m_character.ID);
		m_treasure.LoadFromDatabase();
		m_rank.LoadFromDatabase();
		m_farm.LoadFromDatabase();
		m_actives.LoadFromDatabase();
	}

	public void ChecVipkExpireDay()
	{
		if (m_character.IsVIPExpire())
		{
			m_character.CanTakeVipReward = false;
		}
		else if (m_character.IsLastVIPPackTime())
		{
			m_character.CanTakeVipReward = true;
		}
		else
		{
			m_character.CanTakeVipReward = false;
		}
	}

	public void LastVIPPackTime()
	{
		m_character.LastVIPPackTime = DateTime.Now;
		m_character.CanTakeVipReward = false;
	}

	public void OpenVIP(int thoigian, DateTime ExpireDayOut)
	{
		int vIPLevel = m_character.VIPLevel;
		if (vIPLevel < 6 && thoigian == 180)
		{
			m_character.typeVIP = 1;
			m_character.VIPLevel = 6;
			m_character.VIPExp = 4000;
			m_character.VIPExpireDay = ExpireDayOut;
			m_character.VIPLastDate = DateTime.Now;
			m_character.VIPNextLevelDaysNeeded = 0;
			m_character.CanTakeVipReward = true;
		}
		else
		{
			m_character.typeVIP = 1;
			m_character.VIPLevel = 1;
			m_character.VIPExp = 0;
			m_character.VIPExpireDay = ExpireDayOut;
			m_character.VIPLastDate = DateTime.Now;
			m_character.VIPNextLevelDaysNeeded = 0;
			m_character.CanTakeVipReward = true;
		}
	}

	public void ContinousVIP(int thoigian, DateTime ExpireDayOut)
	{
		int vIPLevel = m_character.VIPLevel;
		if (vIPLevel < 6 && thoigian == 180)
		{
			m_character.VIPLevel = 6;
			m_character.VIPExp = 4000;
			m_character.VIPExpireDay = ExpireDayOut;
		}
		else
		{
			m_character.VIPExpireDay = ExpireDayOut;
		}
	}

	public UserLabyrinthInfo LoadLabyrinth()
	{
		if (m_Labyrinth == null)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			m_Labyrinth = playerBussiness.GetSingleLabyrinth(m_character.ID);
			if (m_Labyrinth == null)
			{
				m_Labyrinth = new UserLabyrinthInfo();
				m_Labyrinth.UserID = m_character.ID;
				m_Labyrinth.myProgress = 0;
				m_Labyrinth.myRanking = 0;
				m_Labyrinth.completeChallenge = true;
				m_Labyrinth.isDoubleAward = false;
				m_Labyrinth.currentFloor = 1;
				m_Labyrinth.accumulateExp = 0;
				m_Labyrinth.remainTime = 0;
				m_Labyrinth.currentRemainTime = 0;
				m_Labyrinth.cleanOutAllTime = 0;
				m_Labyrinth.cleanOutGold = 50;
				m_Labyrinth.tryAgainComplete = true;
				m_Labyrinth.isInGame = false;
				m_Labyrinth.isCleanOut = false;
				m_Labyrinth.serverMultiplyingPower = false;
				m_Labyrinth.LastDate = DateTime.Now;
				m_Labyrinth.ProcessAward = InitProcessAward();
				playerBussiness.AddUserLabyrinth(m_Labyrinth);
			}
			else
			{
				ProcessLabyrinthAward = m_Labyrinth.ProcessAward;
			}
		}
		return Labyrinth;
	}

	public string InitProcessAward()
	{
		string[] array = new string[99];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = i.ToString();
		}
		ProcessLabyrinthAward = string.Join("-", array);
		return ProcessLabyrinthAward;
	}

	public string CompleteGetAward(int floor)
	{
		string[] array = new string[floor];
		for (int i = 0; i < floor; i++)
		{
			array[i] = "i";
		}
		string[] array2 = m_Labyrinth.ProcessAward.Split('-');
		string text = string.Join("-", array);
		for (int j = floor; j < array2.Length; j++)
		{
			text = text + "-" + array2[j];
		}
		return text;
	}

	public bool isDoubleAward()
	{
		return m_Labyrinth != null && m_Labyrinth.isDoubleAward;
	}

	public void OutLabyrinth(bool isWin)
	{
		if (!isWin && m_Labyrinth != null && m_Labyrinth.currentFloor > 1)
		{
			SendLabyrinthTryAgain();
		}
		ResetLabyrinth();
	}

	public void ResetLabyrinth()
	{
		if (m_Labyrinth != null)
		{
			m_Labyrinth.isInGame = false;
			m_Labyrinth.completeChallenge = false;
			m_Labyrinth.ProcessAward = InitProcessAward();
		}
	}

	public void CalculatorClearnOutLabyrinth()
	{
		if (m_Labyrinth != null)
		{
			int num = 0;
			for (int i = m_Labyrinth.currentFloor; i <= m_Labyrinth.myProgress; i++)
			{
				num += 2;
			}
			num *= 60;
			m_Labyrinth.remainTime = num;
			m_Labyrinth.currentRemainTime = num;
			m_Labyrinth.cleanOutAllTime = num;
		}
	}

	public int[] CreateExps()
	{
		int[] array = new int[40];
		int num = 660;
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = num;
			num += 690;
		}
		return array;
	}

	public void UpdateLabyrinth(int floor, int m_missionInfoId, bool bigAward)
	{
		int[] array = CreateExps();
		int num = ((floor - 1 > array.Length) ? (array.Length - 1) : (floor - 1));
		num = ((num >= 0) ? num : 0);
		int num2 = array[num];
		string text = labyrinthGolds[num];
		int num3 = int.Parse(text.Split('|')[0]);
		int num4 = int.Parse(text.Split('|')[1]);
		if (m_Labyrinth == null)
		{
			return;
		}
		floor++;
		ProcessLabyrinthAward = CompleteGetAward(floor);
		m_Labyrinth.ProcessAward = ProcessLabyrinthAward;
		ItemInfo itemByTemplateID = PropBag.GetItemByTemplateID(0, 11916);
		if (itemByTemplateID == null || !RemoveTemplate(11916, 1))
		{
			m_Labyrinth.isDoubleAward = false;
		}
		if (m_Labyrinth.isDoubleAward)
		{
			int num5 = 2;
			num2 *= num5;
			num3 *= num5;
			num4 *= num5;
		}
		if (floor > m_Labyrinth.myProgress)
		{
			m_Labyrinth.myProgress = floor;
		}
		if (floor > m_Labyrinth.currentFloor)
		{
			m_Labyrinth.currentFloor = floor;
		}
		m_Labyrinth.accumulateExp += num2;
		string text2 = $"Bạn nhận được: {num2} exp";
		AddGP(num2);
		if (bigAward)
		{
			List<ItemInfo> list = CopyDrop(2, 40002);
			if (list != null)
			{
				foreach (ItemInfo item in list)
				{
					item.IsBinds = true;
					AddTemplate(item, item.Template.BagType, num3, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipBroadcastTypeView);
					text2 += $", {item.Template.Name} x{num3}";
				}
			}
			AddHardCurrency(num4);
			text2 = text2 + ", Vàng mê cung x" + num4;
		}
		SendHideMessage(text2);
	}

	public List<ItemInfo> CopyDrop(int SessionId, int m_missionInfoId)
	{
		List<ItemInfo> info = null;
		DropInventory.CopyDrop(m_missionInfoId, SessionId, ref info);
		return info;
	}

	public bool MoneyDirect(int value)
	{
		bool isDDTMoneyActive = GameProperties.IsDDTMoneyActive;
		MoneyType type = MoneyType.Money;
		if (isDDTMoneyActive)
		{
			type = MoneyType.DDTMoney;
		}
		return MoneyDirect(type, value);
	}

	public bool MoneyDirect(MoneyType type, int value)
	{
		switch (type)
		{
		case MoneyType.Money:
			if (PlayerCharacter.Money < value)
			{
				SendMessage("Xu không đủ, thao tác thất bại.");
				return false;
			}
			RemoveMoney(value);
			break;
		case MoneyType.DDTMoney:
			if (PlayerCharacter.GiftToken < value)
			{
				SendMessage("Xu khóa không đủ, thao tác thất bại.");
				return false;
			}
			RemoveGiftToken(value);
			break;
		}
		return true;
	}

	public bool SaveIntoDatabase()
	{
		try
		{
			if (m_character.IsDirty)
			{
				using PlayerBussiness playerBussiness = new PlayerBussiness();
				playerBussiness.UpdatePlayer(m_character);
				if (m_Labyrinth != null)
				{
					playerBussiness.UpdateLabyrinthInfo(m_Labyrinth);
				}
				foreach (UserDrillInfo value in m_userDrills.Values)
				{
					playerBussiness.UpdateUserDrillInfo(value);
				}
				foreach (UserGemStone item in m_GemStone)
				{
					playerBussiness.UpdateGemStoneInfo(item);
				}
			}
			MainBag.SaveToDatabase();
			PropBag.SaveToDatabase();
			ConsortiaBag.SaveToDatabase();
			BeadBag.SaveToDatabase();
			FarmBag.SaveToDatabase();
			PetBag.SaveToDatabase(saveAdopt: true);
			CardBag.SaveToDatabase();
			StoreBag.SaveToDatabase();
			Farm.SaveToDatabase();
			Treasure.SaveToDatabase();
			Rank.SaveToDatabase();
			QuestInventory.SaveToDatabase();
			AchievementInventory.SaveToDatabase();
			BufferList.SaveToDatabase();
			BattleData.SaveToDatabase();
			Actives.SaveToDatabase();
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Error saving player " + m_character.NickName + "!", exception);
			return false;
		}
	}

	public bool SaveBagIntoDatabase()
	{
		try
		{
			MainBag.SaveToDatabase();
			PropBag.SaveToDatabase();
			BeadBag.SaveToDatabase();
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Error saving Save Bag Into Database " + m_character.NickName + "!", exception);
			return false;
		}
	}

	public bool UpdateChangedPlaces()
	{
		try
		{
			MainBag.UpdateChangedPlaces();
			PropBag.UpdateChangedPlaces();
			BeadBag.UpdateChangedPlaces();
			return true;
		}
		catch (Exception exception)
		{
			log.Error("Error Update Changed Places " + m_character.NickName + "!", exception);
			return false;
		}
	}

	public virtual bool Quit()
	{
		try
		{
			try
			{
				if (CurrentRoom != null)
				{
					CurrentRoom.RemovePlayerUnsafe(this);
					CurrentRoom = null;
				}
				else
				{
					RoomMgr.WaitingRoom.RemovePlayer(this);
				}
				if (CurrentMarryRoom != null)
				{
					CurrentMarryRoom.RemovePlayer(this);
					CurrentMarryRoom = null;
				}
				if (m_currentSevenDoubleRoom != null)
				{
					CurrentSevenDoubleRoom.RemovePlayer(this);
					CurrentSevenDoubleRoom = null;
				}
				RoomMgr.WorldBossRoom.RemovePlayer(this);
				RoomMgr.ChristmasRoom.SetMonterDie(PlayerCharacter.ID);
				RoomMgr.ChristmasRoom.RemovePlayer(this);
				RoomMgr.ConsBatRoom.RemovePlayer(this);
				RoomMgr.CampBattleRoom.RemovePlayer(this);
				Actives.StopChristmasTimer();
				Actives.StopLabyrinthTimer();
				Actives.StopLightriddleTimer();
			}
			catch (Exception exception)
			{
				log.Error("Player exit Game Error!", exception);
			}
			m_character.State = 0;
			SaveIntoDatabase();
		}
		catch (Exception exception2)
		{
			log.Error("Player exit Error!!!", exception2);
		}
		finally
		{
			WorldMgr.RemovePlayer(m_character.ID);
		}
		return true;
	}

	public void ViFarmsAdd(int playerID)
	{
		if (!_viFarms.Contains(playerID))
		{
			_viFarms.Add(playerID);
		}
	}

	public void ViFarmsRemove(int playerID)
	{
		if (_viFarms.Contains(playerID))
		{
			_viFarms.Remove(playerID);
		}
	}

	public void FriendsAdd(int playerID, int relation)
	{
		if (!_friends.ContainsKey(playerID))
		{
			_friends.Add(playerID, relation);
		}
		else
		{
			_friends[playerID] = relation;
		}
	}

	public void FriendsRemove(int playerID)
	{
		if (_friends.ContainsKey(playerID))
		{
			_friends.Remove(playerID);
		}
	}

	public bool IsBlackFriend(int playerID)
	{
		return _friends == null || (_friends.ContainsKey(playerID) && _friends[playerID] == 1);
	}

	public void ClearConsortia()
	{
		PlayerCharacter.ClearConsortia();
		OnPropertiesChanged();
		QuestInventory.ClearConsortiaQuest();
		string translation = LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.Sender");
		string translation2 = LanguageMgr.GetTranslation("Game.Server.GameUtils.Title");
		ConsortiaBag.SendAllItemsToMail(translation, translation2, eMailType.StoreCanel);
	}

	public void AddRuneProperty(ItemInfo item, ref double defence, ref double attack)
	{
		RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
		if (runeTemplateInfo == null)
		{
			return;
		}
		string[] array = runeTemplateInfo.Attribute1.Split('|');
		string[] array2 = runeTemplateInfo.Attribute2.Split('|');
		int num = 0;
		int num2 = 0;
		if (item.Hole1 > runeTemplateInfo.BaseLevel)
		{
			if (array.Length > 1)
			{
				num = 1;
			}
			if (array2.Length > 1)
			{
				num2 = 1;
			}
		}
		int num3 = Convert.ToInt32(array[num]);
		Convert.ToInt32(array2[num2]);
		switch (runeTemplateInfo.Type1)
		{
		case 35:
			attack += num3;
			break;
		case 36:
			defence += num3;
			break;
		}
	}

	public double getHertAddition(double para1, double para2)
	{
		double a = para1 * Math.Pow(1.1, para2) - para1;
		return Math.Round(a);
	}

	public double GetBaseAttack()
	{
		double num = 0.0;
		double defence = 0.0;
		double attack = 0.0;
		double num2 = 0.0;
		UserRankInfo rank = Rank.GetRank(PlayerCharacter.Honor);
		if (rank != null)
		{
			num += (double)rank.Damage;
		}
		PlayerProp.totalDamage = (int)num;
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt = m_BeadBag.GetItemAt(i);
			if (itemAt != null)
			{
				AddRuneProperty(itemAt, ref defence, ref attack);
			}
		}
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Bead", attack);
		PlayerProp.UpadateBaseProp(isSelf: true, "Damage", "Suit", num2);
		List<UsersCardInfo> cards = m_cardBag.GetCards(0, 5);
		foreach (UsersCardInfo item in cards)
		{
			if (item.CardID != 0)
			{
				CardTemplateInfo cardTemplateInfo = CardMgr.FindCardTemplate(item.TemplateID, item.CardType);
				if (cardTemplateInfo != null)
				{
					num += (double)cardTemplateInfo.AddDamage;
				}
				num += (double)item.Damage;
			}
		}
		num += (double)TotemMgr.GetTotemProp(m_character.totemId, "dam");
		ItemInfo itemAt2 = m_mainBag.GetItemAt(6);
		if (itemAt2 != null)
		{
			double num3 = itemAt2.Template.Property7;
			int num4 = (itemAt2.IsGold ? 1 : 0);
			double para = itemAt2.StrengthenLevel + num4;
			num += getHertAddition(num3, para) + num3;
		}
		return num + attack + num2;
	}

	public double GetBaseDefence()
	{
		double num = 0.0;
		double defence = 0.0;
		double num2 = 0.0;
		double attack = 0.0;
		UserRankInfo rank = Rank.GetRank(PlayerCharacter.Honor);
		if (rank != null)
		{
			num += (double)rank.Guard;
		}
		PlayerProp.totalArmor = (int)num;
		for (int i = 0; i < 31; i++)
		{
			ItemInfo itemAt = m_BeadBag.GetItemAt(i);
			if (itemAt != null)
			{
				AddRuneProperty(itemAt, ref defence, ref attack);
			}
		}
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Bead", defence);
		PlayerProp.UpadateBaseProp(isSelf: true, "Armor", "Suit", num2);
		List<UsersCardInfo> cards = m_cardBag.GetCards(0, 5);
		foreach (UsersCardInfo item in cards)
		{
			if (item.CardID > 0)
			{
				CardTemplateInfo cardTemplateInfo = CardMgr.FindCardTemplate(item.TemplateID, item.CardType);
				if (cardTemplateInfo != null)
				{
					num += (double)cardTemplateInfo.AddGuard;
				}
				num += (double)item.Guard;
			}
		}
		num += (double)TotemMgr.GetTotemProp(m_character.totemId, "gua");
		ItemInfo itemAt2 = m_mainBag.GetItemAt(0);
		if (itemAt2 != null)
		{
			double num3 = itemAt2.Template.Property7;
			int num4 = (itemAt2.IsGold ? 1 : 0);
			double para = itemAt2.StrengthenLevel + num4;
			num += getHertAddition(num3, para) + num3;
		}
		ItemInfo itemAt3 = m_mainBag.GetItemAt(4);
		if (itemAt3 != null)
		{
			double num3 = itemAt3.Template.Property7;
			int num5 = (itemAt3.IsGold ? 1 : 0);
			double para2 = itemAt3.StrengthenLevel + num5;
			num += getHertAddition(num3, para2) + num3;
		}
		return num + defence + num2;
	}

	public double GetBaseAgility()
	{
		return 1.0 - (double)m_character.Agility * 0.001;
	}

	public double GetBaseBlood()
	{
		ItemInfo itemAt = MainBag.GetItemAt(12);
		if (itemAt != null)
		{
			return (100.0 + (double)itemAt.Template.Property1 + (double)PlayerCharacter.necklaceExpAdd) / 100.0;
		}
		return 1.0;
	}

	public double GetGoldBlood()
	{
		ItemInfo itemAt = MainBag.GetItemAt(0);
		ItemInfo itemAt2 = MainBag.GetItemAt(4);
		double num = 0.0;
		if (itemAt != null)
		{
			GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipCategoryID(itemAt.Template.CategoryID);
			if (itemAt.IsGold)
			{
				num += (double)goldEquipTemplateLoadInfo.Boold;
			}
		}
		if (itemAt2 != null)
		{
			GoldEquipTemplateLoadInfo goldEquipTemplateLoadInfo = GoldEquipMgr.FindGoldEquipCategoryID(itemAt2.Template.CategoryID);
			if (itemAt2.IsGold)
			{
				num += (double)goldEquipTemplateLoadInfo.Boold;
			}
		}
		return num;
	}

	public bool RemoveAt(eBageType bagType, int place)
	{
		return GetInventory(bagType)?.RemoveItemAt(place) ?? false;
	}

	public void UpdateBarrier(int barrier, string pic)
	{
		if (CurrentRoom != null)
		{
			CurrentRoom.Pic = pic;
			CurrentRoom.barrierNum = barrier;
			CurrentRoom.currentFloor = barrier;
		}
	}

	public bool DeletePropItem(int place)
	{
		FightBag.RemoveItemAt(place);
		return true;
	}

	public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving)
	{
		if (bag == 1)
		{
			ItemTemplateInfo itemTemplateInfo = PropItemMgr.FindFightingProp(templateId);
			if (isLiving && itemTemplateInfo != null)
			{
				OnUsingItem(itemTemplateInfo.TemplateID);
				if (place == -1 && CanUseProp)
				{
					return true;
				}
				ItemInfo itemAt = GetItemAt(eBageType.PropBag, place);
				if (itemAt != null && itemAt.IsValidItem() && itemAt.Count >= 0)
				{
					itemAt.Count--;
					UpdateItem(itemAt);
					return true;
				}
			}
		}
		else
		{
			ItemInfo itemAt2 = GetItemAt(eBageType.FightBag, place);
			if (itemAt2.TemplateID == templateId)
			{
				OnUsingItem(itemAt2.TemplateID);
				return RemoveAt(eBageType.FightBag, place);
			}
		}
		return false;
	}

	public void Disconnect()
	{
		m_client.Disconnect();
	}

	public void SendTCP(GSPacketIn pkg)
	{
		if (m_client.IsConnected)
		{
			m_client.SendTCP(pkg);
		}
	}

	public void ClearFootballCard()
	{
		for (int i = 0; i < CardsTakeOut.Length; i++)
		{
			CardsTakeOut[i] = null;
		}
	}

	public void TakeFootballCard(CardInfo card)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		for (int i = 0; i < CardsTakeOut.Length; i++)
		{
			if (card.place == i)
			{
				CardsTakeOut[i] = card;
				CardsTakeOut[i].IsTake = true;
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(card.templateID);
				if (itemTemplateInfo != null)
				{
					list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, card.count, 110));
				}
				takeoutCount--;
				break;
			}
		}
		if (list.Count <= 0)
		{
			return;
		}
		foreach (ItemInfo item in list)
		{
			AddTemplate(list);
		}
	}

	public void ShowAllFootballCard()
	{
		for (int i = 0; i < CardsTakeOut.Length; i++)
		{
			if (CardsTakeOut[i] == null)
			{
				CardsTakeOut[i] = Card[i];
				if (takeoutCount > 0)
				{
					TakeFootballCard(Card[i]);
				}
			}
		}
	}

	public void FootballTakeOut(bool isWin)
	{
		if (isWin)
		{
			canTakeOut = 2;
			takeoutCount = 2;
		}
		else
		{
			canTakeOut = 1;
			takeoutCount = 1;
		}
	}

	public void LoadMarryProp()
	{
		using PlayerBussiness playerBussiness = new PlayerBussiness();
		MarryProp marryProp = playerBussiness.GetMarryProp(PlayerCharacter.ID);
		PlayerCharacter.IsMarried = marryProp.IsMarried;
		PlayerCharacter.SpouseID = marryProp.SpouseID;
		PlayerCharacter.SpouseName = marryProp.SpouseName;
		PlayerCharacter.IsCreatedMarryRoom = marryProp.IsCreatedMarryRoom;
		PlayerCharacter.SelfMarryRoomID = marryProp.SelfMarryRoomID;
		PlayerCharacter.IsGotRing = marryProp.IsGotRing;
		Out.SendMarryProp(this, marryProp);
	}

	public override string ToString()
	{
		return $"Id:{PlayerId} nickname:{PlayerCharacter.NickName} room:{CurrentRoom} ";
	}

	public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players, eRoomType roomType, eGameType gameClass, int totalKillHealth, int count)
	{
		return ConsortiaMgr.ConsortiaFight(consortiaWin, consortiaLose, players, roomType, gameClass, totalKillHealth, count);
	}

	public void SendConsortiaFight(int consortiaID, int riches, string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(158);
		gSPacketIn.WriteInt(consortiaID);
		gSPacketIn.WriteInt(riches);
		gSPacketIn.WriteString(msg);
		GameServer.Instance.LoginServer.SendPacket(gSPacketIn);
	}

	public void LoadPvePermission()
	{
		PveInfo[] pveInfo = PveInfoMgr.GetPveInfo();
		PveInfo[] array = pveInfo;
		foreach (PveInfo pveInfo2 in array)
		{
			if (m_character.Grade > pveInfo2.LevelLimits)
			{
				bool flag = SetPvePermission(pveInfo2.ID, eHardLevel.Simple);
				if (flag)
				{
					flag = SetPvePermission(pveInfo2.ID, eHardLevel.Normal);
				}
				if (flag)
				{
					flag = SetPvePermission(pveInfo2.ID, eHardLevel.Hard);
				}
			}
		}
	}

	public char[] InitPvePermission()
	{
		char[] array = new char[50];
		for (int i = 0; i < array.Length; i++)
		{
			array[i] = '1';
		}
		return array;
	}

	public string ConverterPvePermission(char[] chArray)
	{
		string text = "";
		for (int i = 0; i < chArray.Length; i++)
		{
			text += chArray[i];
		}
		return text;
	}

	public bool SetPvePermission(int copyId, eHardLevel hardLevel)
	{
		if (copyId > m_pvepermissions.Length || copyId <= 0 || hardLevel == eHardLevel.Terror || m_pvepermissions[copyId - 1] != permissionChars[(int)hardLevel])
		{
			return true;
		}
		m_pvepermissions[copyId - 1] = permissionChars[(int)(hardLevel + 1)];
		m_character.PvePermission = ConverterPvePermission(m_pvepermissions);
		OnPropertiesChanged();
		return true;
	}

	public bool IsPvePermission(int copyId, eHardLevel hardLevel)
	{
		if (copyId > m_pvepermissions.Length || copyId <= 0)
		{
			return true;
		}
		if (hardLevel == eHardLevel.Epic)
		{
			return IsPveEpicPermission(copyId);
		}
		return m_pvepermissions[copyId - 1] >= permissionChars[(int)hardLevel];
	}

	public bool IsPveEpicPermission(int copyId)
	{
		string text = "1-2-3-4-5-6-7-8-9-10-11-12-13";
		bool result = false;
		if (text.Length > 0)
		{
			string[] array = text.Split('-');
			string[] array2 = array;
			foreach (string text2 in array2)
			{
				if (text2 == copyId.ToString())
				{
					result = true;
					break;
				}
			}
		}
		return result;
	}

	public void SendInsufficientMoney(int type)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(88, PlayerId);
		gSPacketIn.WriteByte((byte)type);
		gSPacketIn.WriteBoolean(val: false);
		SendTCP(gSPacketIn);
	}

	public void SendMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(0);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public void SendPrivateChat(int receiverID, string receiver, string sender, string msg, bool isAutoReply)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(37, PlayerCharacter.ID);
		gSPacketIn.WriteInt(receiverID);
		gSPacketIn.WriteString(receiver);
		gSPacketIn.WriteString(sender);
		gSPacketIn.WriteString(msg);
		gSPacketIn.WriteBoolean(isAutoReply);
		SendTCP(gSPacketIn);
	}

	public void SendHideMessage(string msg)
	{
		GSPacketIn gSPacketIn = new GSPacketIn(3);
		gSPacketIn.WriteInt(3);
		gSPacketIn.WriteString(msg);
		SendTCP(gSPacketIn);
	}

	public int LabyrinthTryAgainMoney()
	{
		for (int i = 0; i < Labyrinth.myProgress; i += 2)
		{
			if (Labyrinth.currentFloor == i)
			{
				return GameProperties.WarriorFamRaidPriceBig;
			}
		}
		return GameProperties.WarriorFamRaidPriceSmall;
	}

	public void SendLabyrinthTryAgain()
	{
		GSPacketIn gSPacketIn = new GSPacketIn(131, PlayerId);
		gSPacketIn.WriteByte(9);
		gSPacketIn.WriteInt(LabyrinthTryAgainMoney());
		SendTCP(gSPacketIn);
	}

	public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type)
	{
		using PlayerBussiness pb = new PlayerBussiness();
		List<ItemInfo> list = new List<ItemInfo>();
		foreach (ItemInfo item in items)
		{
			if (item.Template.MaxCount == 1)
			{
				for (int i = 0; i < item.Count; i++)
				{
					ItemInfo itemInfo = ItemInfo.CloneFromTemplate(item.Template, item);
					itemInfo.Count = 1;
					list.Add(itemInfo);
				}
			}
			else
			{
				list.Add(item);
			}
		}
		return SendItemsToMail(list, content, title, type, pb);
	}

	public bool SendItemsToMail(List<ItemInfo> items, string content, string title, eMailType type, PlayerBussiness pb)
	{
		bool result = true;
		for (int i = 0; i < items.Count; i += 5)
		{
			MailInfo mailInfo = new MailInfo();
			mailInfo.Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title"));
			mailInfo.Gold = 0;
			mailInfo.IsExist = true;
			mailInfo.Money = 0;
			mailInfo.Receiver = PlayerCharacter.NickName;
			mailInfo.ReceiverID = PlayerId;
			mailInfo.Sender = PlayerCharacter.NickName;
			mailInfo.SenderID = PlayerId;
			mailInfo.Type = (int)type;
			mailInfo.GiftToken = 0;
			MailInfo mailInfo2 = mailInfo;
			List<ItemInfo> list = new List<ItemInfo>();
			StringBuilder stringBuilder = new StringBuilder();
			StringBuilder stringBuilder2 = new StringBuilder();
			stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
			content = ((content != null) ? LanguageMgr.GetTranslation(content) : "");
			int num = i;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo2.Title = itemInfo.Template.Name;
				mailInfo2.Annex1 = itemInfo.ItemID.ToString();
				mailInfo2.Annex1Name = itemInfo.Template.Name;
				stringBuilder.Append("1、" + mailInfo2.Annex1Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("1、" + mailInfo2.Annex1Name + "x" + itemInfo.Count + ";");
			}
			num = i + 1;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo2.Annex2 = itemInfo.ItemID.ToString();
				mailInfo2.Annex2Name = itemInfo.Template.Name;
				stringBuilder.Append("2、" + mailInfo2.Annex2Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("2、" + mailInfo2.Annex2Name + "x" + itemInfo.Count + ";");
			}
			num = i + 2;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo2.Annex3 = itemInfo.ItemID.ToString();
				mailInfo2.Annex3Name = itemInfo.Template.Name;
				stringBuilder.Append("3、" + mailInfo2.Annex3Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("3、" + mailInfo2.Annex3Name + "x" + itemInfo.Count + ";");
			}
			num = i + 3;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo2.Annex4 = itemInfo.ItemID.ToString();
				mailInfo2.Annex4Name = itemInfo.Template.Name;
				stringBuilder.Append("4、" + mailInfo2.Annex4Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("4、" + mailInfo2.Annex4Name + "x" + itemInfo.Count + ";");
			}
			num = i + 4;
			if (items.Count > num)
			{
				ItemInfo itemInfo = items[num];
				if (itemInfo.ItemID == 0)
				{
					pb.AddGoods(itemInfo);
				}
				else
				{
					list.Add(itemInfo);
				}
				mailInfo2.Annex5 = itemInfo.ItemID.ToString();
				mailInfo2.Annex5Name = itemInfo.Template.Name;
				stringBuilder.Append("5、" + mailInfo2.Annex5Name + "x" + itemInfo.Count + ";");
				stringBuilder2.Append("5、" + mailInfo2.Annex5Name + "x" + itemInfo.Count + ";");
			}
			mailInfo2.AnnexRemark = stringBuilder.ToString();
			if (content == null && stringBuilder2.ToString() == null)
			{
				mailInfo2.Content = LanguageMgr.GetTranslation("Game.Server.GameUtils.Content");
			}
			else if (content != "")
			{
				mailInfo2.Content = content;
			}
			else
			{
				mailInfo2.Content = stringBuilder2.ToString();
			}
			if (pb.SendMail(mailInfo2))
			{
				foreach (ItemInfo item in list)
				{
					TakeOutItem(item);
				}
			}
			else
			{
				result = false;
			}
		}
		return result;
	}

	public bool SendItemToMail(int templateID, string content, string title)
	{
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateID);
		if (itemTemplateInfo == null)
		{
			return false;
		}
		if (content == "")
		{
			content = itemTemplateInfo.Name + "x1";
		}
		ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, 1, 104);
		itemInfo.IsBinds = true;
		return SendItemToMail(itemInfo, content, title, eMailType.Active);
	}

	public bool SendItemToMail(ItemInfo item, string content, string title, eMailType type)
	{
		using PlayerBussiness pb = new PlayerBussiness();
		return SendItemToMail(item, pb, content, title, type);
	}

	public bool SendItemToMail(ItemInfo item, PlayerBussiness pb, string content, string title, eMailType type)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Content = ((content != null) ? content : LanguageMgr.GetTranslation("Game.Server.GameUtils.Content"));
		mailInfo.Title = ((title != null) ? title : LanguageMgr.GetTranslation("Game.Server.GameUtils.Title"));
		mailInfo.Gold = 0;
		mailInfo.IsExist = true;
		mailInfo.Money = 0;
		mailInfo.GiftToken = 0;
		mailInfo.Receiver = PlayerCharacter.NickName;
		mailInfo.ReceiverID = PlayerCharacter.ID;
		mailInfo.Sender = PlayerCharacter.NickName;
		mailInfo.SenderID = PlayerCharacter.ID;
		mailInfo.Type = (int)type;
		MailInfo mailInfo2 = mailInfo;
		if (item.ItemID == 0)
		{
			pb.AddGoods(item);
		}
		mailInfo2.Annex1 = item.ItemID.ToString();
		mailInfo2.Annex1Name = item.Template.Name;
		if (pb.SendMail(mailInfo2))
		{
			TakeOutItem(item);
			return true;
		}
		return false;
	}

	public bool SendMailToUser(PlayerBussiness pb, string content, string title, eMailType type)
	{
		MailInfo mailInfo = new MailInfo();
		mailInfo.Content = content;
		mailInfo.Title = title;
		mailInfo.Gold = 0;
		mailInfo.IsExist = true;
		mailInfo.Money = 0;
		mailInfo.GiftToken = 0;
		mailInfo.Receiver = PlayerCharacter.NickName;
		mailInfo.ReceiverID = PlayerCharacter.ID;
		mailInfo.Sender = PlayerCharacter.NickName;
		mailInfo.SenderID = PlayerCharacter.ID;
		mailInfo.Type = (int)type;
		MailInfo mailInfo2 = mailInfo;
		mailInfo2.Annex1 = "";
		mailInfo2.Annex1Name = "";
		return pb.SendMail(mailInfo2);
	}

	public bool TakeOutItem(ItemInfo item)
	{
		if (item.BagType == m_propBag.BagType)
		{
			return m_propBag.TakeOutItem(item);
		}
		if (item.BagType == m_fightBag.BagType)
		{
			return m_fightBag.TakeOutItem(item);
		}
		if (item.BagType == m_ConsortiaBag.BagType)
		{
			return m_ConsortiaBag.TakeOutItem(item);
		}
		if (item.BagType == m_BeadBag.BagType)
		{
			return m_BeadBag.TakeOutItem(item);
		}
		return m_mainBag.TakeOutItem(item);
	}

	public bool RemoveCountFromStack(ItemInfo item, int count)
	{
		if (item.BagType == m_propBag.BagType)
		{
			return m_propBag.RemoveCountFromStack(item, count);
		}
		if (item.BagType == m_ConsortiaBag.BagType)
		{
			return m_ConsortiaBag.RemoveCountFromStack(item, count);
		}
		if (item.BagType == m_BeadBag.BagType)
		{
			return m_BeadBag.RemoveCountFromStack(item, count);
		}
		return m_mainBag.RemoveCountFromStack(item, count);
	}

	public void AddGift(eGiftType type)
	{
		List<ItemInfo> list = new List<ItemInfo>();
		bool testActive = GameProperties.TestActive;
		switch (type)
		{
		case eGiftType.MONEY:
			if (testActive)
			{
				AddMoney(GameProperties.FreeMoney);
			}
			break;
		case eGiftType.SMALL_EXP:
		{
			string[] array = GameProperties.FreeExp.Split('|');
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(array[0]));
			if (itemTemplateInfo != null)
			{
				list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, Convert.ToInt32(array[1]), 102));
			}
			break;
		}
		case eGiftType.BIG_EXP:
		{
			string[] array = GameProperties.BigExp.Split('|');
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(array[0]));
			if (itemTemplateInfo != null && testActive)
			{
				list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, Convert.ToInt32(array[1]), 102));
			}
			break;
		}
		case eGiftType.PET_EXP:
		{
			string[] array = GameProperties.PetExp.Split('|');
			ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(Convert.ToInt32(array[0]));
			if (itemTemplateInfo != null && testActive)
			{
				list.Add(ItemInfo.CreateFromTemplate(itemTemplateInfo, Convert.ToInt32(array[1]), 102));
			}
			break;
		}
		}
		foreach (ItemInfo item in list)
		{
			item.IsBinds = true;
			AddTemplate(item, item.Template.BagType, item.Count, eItemNotice.GoodsTipTypeView, eItemNotice.GoodsTipBroadcastTypeView);
		}
	}

	public bool EquipItem(ItemInfo item, int place)
	{
		if (!item.CanEquip() || item.BagType != m_mainBag.BagType)
		{
			return false;
		}
		int num = m_mainBag.FindItemEpuipSlot(item.Template);
		int num2;
		switch (num)
		{
		case 9:
		case 10:
			num2 = place switch
			{
				10 => 0,
				9 => 0,
				_ => 1,
			};
			break;
		default:
			num2 = 1;
			break;
		}
		if (num2 == 0)
		{
			num = place;
		}
		else if ((num == 7 || num == 8) && (place == 7 || place == 8))
		{
			num = place;
		}
		return m_mainBag.MoveItem(item.Place, num, item.Count);
	}

	public void OnLevelUp(int grade)
	{
		if (LevelUp != null)
		{
			LevelUp(this);
		}
	}

	public void OnUsingItem(int templateID)
	{
		if (AfterUsingItem != null)
		{
			AfterUsingItem(templateID);
		}
	}

	public void OnGameOver(AbstractGame game, bool isWin, int gainXp)
	{
		if (game.RoomType == eRoomType.Match)
		{
			if (isWin)
			{
				m_character.Win++;
			}
			m_character.Total++;
		}
		if (GameOver != null)
		{
			GameOver(game, isWin, gainXp);
		}
	}

	public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum)
	{
		if (MissionOver != null)
		{
			MissionOver(game, missionId, isWin);
		}
		if (MissionTurnOver != null && isWin)
		{
			MissionTurnOver(game, missionId, turnNum);
		}
	}

	public void OnItemStrengthen(int categoryID, int level)
	{
		if (ItemStrengthen != null)
		{
			ItemStrengthen(categoryID, level);
		}
	}

	public void OnPaid(int money, int gold, int offer, int gifttoken, int medal, string payGoods)
	{
		if (Paid != null)
		{
			Paid(money, gold, offer, gifttoken, medal, payGoods);
		}
	}

	public void OnAdoptPetEvent()
	{
		if (AdoptPetEvent != null)
		{
			AdoptPetEvent();
		}
	}

	public void OnNewGearEvent(int CategoryID)
	{
		if (NewGearEvent != null)
		{
			NewGearEvent(CategoryID);
		}
	}

	public void OnCropPrimaryEvent()
	{
		if (CropPrimaryEvent != null)
		{
			CropPrimaryEvent();
		}
	}

	public void OnSeedFoodPetEvent()
	{
		if (SeedFoodPetEvent != null)
		{
			SeedFoodPetEvent();
		}
	}

	public void OnUserToemGemstoneEvent()
	{
		if (UserToemGemstonetEvent != null)
		{
			UserToemGemstonetEvent();
		}
	}

	public void OnUnknowQuestConditionEvent()
	{
		if (UnknowQuestConditionEvent != null)
		{
			UnknowQuestConditionEvent();
		}
	}

	public void OnItemInsert()
	{
		if (ItemInsert != null)
		{
			ItemInsert();
		}
	}

	public void OnItemFusion(int fusionType)
	{
		if (ItemFusion != null)
		{
			ItemFusion(fusionType);
		}
	}

	public void OnItemMelt(int categoryID)
	{
		if (ItemMelt != null)
		{
			ItemMelt(categoryID);
		}
	}

	public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage)
	{
		if (AfterKillingLiving != null)
		{
			AfterKillingLiving(game, type, id, isLiving, damage);
		}
		if (GameKillDrop != null && !isLiving)
		{
			GameKillDrop(game, type, id, isLiving);
		}
	}

	public void OnGuildChanged()
	{
		if (GuildChanged != null)
		{
			GuildChanged();
		}
	}

	public void OnItemCompose(int composeType)
	{
		if (ItemCompose != null)
		{
			ItemCompose(composeType);
		}
	}
}
