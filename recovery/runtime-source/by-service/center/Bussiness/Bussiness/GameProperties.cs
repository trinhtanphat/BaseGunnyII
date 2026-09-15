using System;
using System.Collections.Generic;
using System.Reflection;
using Game.Base.Config;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness;

public abstract class GameProperties
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	[ConfigProperty("Edition", "µ±Ç°ÓÎÏ·°æ±¾", "5498628")]
	public static readonly string EDITION;

	[ConfigProperty("MustComposeGold", "ºÏ³ÉÏûºÄ½ð±Ò¼Û\u00b8ñ", 1000)]
	public static readonly int PRICE_COMPOSE_GOLD;

	[ConfigProperty("MustFusionGold", "ÈÛÁ¶ÏûºÄ½ð±Ò¼Û\u00b8ñ", 400)]
	public static readonly int PRICE_FUSION_GOLD;

	[ConfigProperty("MustStrengthenGold", "Ç¿»\u00af½ð±ÒÏûºÄ¼Û\u00b8ñ", 1000)]
	public static readonly int PRICE_STRENGHTN_GOLD;

	[ConfigProperty("CheckRewardItem", "ÑéÖ¤Âë½±ÀøÎïÆ·", 11001)]
	public static readonly int CHECK_REWARD_ITEM;

	[ConfigProperty("CheckCount", "×î\u00b4óÑéÖ¤ÂëÊ§°Ü\u00b4ÎÊý", 2)]
	public static readonly int CHECK_MAX_FAILED_COUNT;

	[ConfigProperty("HymenealMoney", "Çó»éµÄ¼Û\u00b8ñ", 300)]
	public static readonly int PRICE_PROPOSE;

	[ConfigProperty("DivorcedMoney", "Àë»éµÄ¼Û\u00b8ñ", 1499)]
	public static readonly int PRICE_DIVORCED;

	[ConfigProperty("DivorcedDiscountMoney", "Àë»éµÄ¼Û\u00b8ñ", 999)]
	public static readonly int PRICE_DIVORCED_DISCOUNT;

	[ConfigProperty("MarryRoomCreateMoney", "½á»é·¿¼äµÄ¼Û\u00b8ñ,2Ð¡Ê±¡¢3Ð¡Ê±¡¢4Ð¡Ê±ÓÃ¶ººÅ·Ö\u00b8ô", "2000,2700,3400")]
	public static readonly string PRICE_MARRY_ROOM;

	[ConfigProperty("BoxAppearCondition", "Ïä×ÓÎïÆ·ÌáÊ¾µÄµÈ¼¶", 4)]
	public static readonly int BOX_APPEAR_CONDITION;

	[ConfigProperty("DisableCommands", "½ûÖ¹Ê¹ÓÃµÄÃüÁî", "")]
	public static readonly string DISABLED_COMMANDS;

	[ConfigProperty("AssState", "·À³ÁÃÔÏµÍ³µÄ¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", false)]
	public static bool ASS_STATE;

	[ConfigProperty("DailyAwardState", "Ã¿ÈÕ½±Àø¿ª¹Ø,True\u00b4ò¿ª,False¹Ø±Õ", true)]
	public static bool DAILY_AWARD_STATE;

	[ConfigProperty("Cess", "½»Ò×¿ÛË°", 0.1)]
	public static readonly double Cess;

	[ConfigProperty("BeginAuction", "ÅÄÂòÊ±ÆðÊ¼Ëæ»úÊ±¼ä", 20)]
	public static int BeginAuction;

	[ConfigProperty("EndAuction", "ÅÄÂòÊ±½áÊøËæ»úÊ±¼ä", 40)]
	public static int EndAuction;

	[ConfigProperty("HotSpringExp", "Kinh nghiệm Spa", "1|2")]
	public static readonly string HotSpringExp;

	[ConfigProperty("ConsortiaStrengthenEx", "Kinh nghiệm", "1|2")]
	public static readonly string ConsortiaStrengthenEx;

	[ConfigProperty("RuneLevelUpExp", "Kinh nghiệm châu báu", "1|2")]
	public static readonly string RuneLevelUpExp;

	[ConfigProperty("RunePackageID", "RunePackageID", "1|2")]
	public static readonly string RunePackageID;

	[ConfigProperty("OpenRunePackageMoney", "OpenRunePackageMoney", "1|2")]
	public static readonly string OpenRunePackageMoney;

	[ConfigProperty("OpenRunePackageRange", "OpenRunePackageRange", "1|2")]
	public static readonly string OpenRunePackageRange;

	[ConfigProperty("VIPExpForEachLv", "VIPExpForEachLv", "1|2")]
	public static readonly string VIPExpForEachLv;

	[ConfigProperty("HoleLevelUpExpList", "HoleLevelUpExpList", "1|2")]
	public static readonly string HoleLevelUpExpList;

	[ConfigProperty("VIPStrengthenEx", "VIPStrengthenEx", "1|2")]
	public static readonly string VIPStrengthenEx;

	[ConfigProperty("TestActive", "TestActive", false)]
	public static readonly bool TestActive;

	[ConfigProperty("WishBeadLimitLv", "WishBeadLimitLv", 12)]
	public static readonly int WishBeadLimitLv;

	[ConfigProperty("IsWishBeadLimit", "IsWishBeadLimit", false)]
	public static readonly bool IsWishBeadLimit;

	[ConfigProperty("BagMailEnable", "BagMailEnable", true)]
	public static readonly bool BagMailEnable;

	[ConfigProperty("FreeMoney", "µ±Ç°ÓÎÏ·°æ±¾", 9990000)]
	public static readonly int FreeMoney;

	[ConfigProperty("FreeExp", "µ±Ç°ÓÎÏ·°æ±¾", "11901|1")]
	public static readonly string FreeExp;

	[ConfigProperty("BigExp", "µ±Ç°ÓÎÏ·°æ±¾", "11906|99")]
	public static readonly string BigExp;

	[ConfigProperty("PetExp", "µ±Ç°ÓÎÏ·°æ±¾", "334103|999")]
	public static readonly string PetExp;

	[ConfigProperty("IsActiveMoney", "IsActiveMoney", true)]
	public static readonly bool IsActiveMoney;

	[ConfigProperty("IsDDTMoneyActive", "IsDDTMoneyActive", false)]
	public static readonly bool IsDDTMoneyActive;

	[ConfigProperty("NewChickenBeginTime", "NewChickenBeginTime", "2013/12/17 0:00:00")]
	public static readonly string NewChickenBeginTime;

	[ConfigProperty("NewChickenEndTime", "NewChickenEndTime", "2013/12/25 0:00:00")]
	public static readonly string NewChickenEndTime;

	[ConfigProperty("NewChickenEagleEyePrice", "NewChickenEagleEyePrice", "3000, 2000, 1000")]
	public static readonly string NewChickenEagleEyePrice;

	[ConfigProperty("NewChickenOpenCardPrice", "NewChickenOpenCardPrice", "2500, 2000, 1500, 1000, 500")]
	public static readonly string NewChickenOpenCardPrice;

	[ConfigProperty("NewChickenFlushPrice", "NewChickenFlushPrice", 10000)]
	public static readonly int NewChickenFlushPrice;

	[ConfigProperty("DiceBeginTime", "DiceBeginTime", "2013/12/17 0:00:00")]
	public static readonly string DiceBeginTime;

	[ConfigProperty("DiceEndTime", "DiceEndTime", "2013/12/25 0:00:00")]
	public static readonly string DiceEndTime;

	[ConfigProperty("DiceRefreshPrice", "DiceRefreshPrice", 40000)]
	public static readonly int DiceRefreshPrice;

	[ConfigProperty("CommonDicePrice", "CommonDicePrice", 30000)]
	public static readonly int CommonDicePrice;

	[ConfigProperty("DoubleDicePrice", "DoubleDicePrice", 40000)]
	public static readonly int DoubleDicePrice;

	[ConfigProperty("BigDicePrice", "BigDicePrice", 50000)]
	public static readonly int BigDicePrice;

	[ConfigProperty("SmallDicePrice", "SmallDicePrice", 60000)]
	public static readonly int SmallDicePrice;

	[ConfigProperty("PyramidBeginTime", "PyramidBeginTime", "2013/12/17 0:00:00")]
	public static readonly string PyramidBeginTime;

	[ConfigProperty("PyramidEndTime", "NewChickenEndTime", "2013/12/25 0:00:00")]
	public static readonly string PyramidEndTime;

	[ConfigProperty("PyramidRevivePrice", "PyramidRevivePrice", "10000, 30000, 50000")]
	public static readonly string PyramidRevivePrice;

	[ConfigProperty("PyramydTurnCardPrice", "PyramydTurnCardPrice", 5000)]
	public static readonly int PyramydTurnCardPrice;

	[ConfigProperty("DragonBoatBeginDate", "DragonBoatBeginDate", "2013/12/19 0:00:00")]
	public static readonly string DragonBoatBeginDate;

	[ConfigProperty("DragonBoatEndDate", "DragonBoatEndDate", "2013/12/26 0:00:00")]
	public static readonly string DragonBoatEndDate;

	[ConfigProperty("FightFootballTime", "FightFootballTime", "19|60")]
	public static readonly string FightFootballTime;

	[ConfigProperty("LuckStarActivityBeginDate", "LuckStarActivityBeginDate", "2013/12/1 0:00:00")]
	public static readonly string LuckStarActivityBeginDate;

	[ConfigProperty("LuckStarActivityEndDate", "LuckStarActivityEndDate", "2014/12/24 0:00:00")]
	public static readonly string LuckStarActivityEndDate;

	[ConfigProperty("MinUseNum", "MinUseNum", 1000)]
	public static readonly int MinUseNum;

	[ConfigProperty("YearMonsterBeginDate", "YearMonsterBeginDate", "2014/1/17 0:00:00")]
	public static readonly string YearMonsterBeginDate;

	[ConfigProperty("YearMonsterEndDate", "YearMonsterEndDate", "2014/2/25 0:00:00")]
	public static readonly string YearMonsterEndDate;

	[ConfigProperty("YearMonsterBoxInfo", "YearMonsterBoxInfo", "112370,5|112371,30|112372,90|112373,150|112374,300")]
	public static readonly string YearMonsterBoxInfo;

	[ConfigProperty("YearMonsterBuffMoney", "YearMonsterBuffMoney", 300)]
	public static readonly int YearMonsterBuffMoney;

	[ConfigProperty("YearMonsterFightNum", "YearMonsterFightNum", 1)]
	public static readonly int YearMonsterFightNum;

	[ConfigProperty("YearMonsterHP", "YearMonsterHP", 3000000)]
	public static readonly int YearMonsterHP;

	[ConfigProperty("YearMonsterOpenLevel", "YearMonsterOpenLevel", 15)]
	public static readonly int YearMonsterOpenLevel;

	[ConfigProperty("DiceGameAwardAndCount", "DiceGameAwardAndCount", "32|16|8|4|2|1")]
	public static readonly string DiceGameAwardAndCount;

	[ConfigProperty("LightRiddleAnswerScore", "LightRiddleAnswerScore", "29|9")]
	public static readonly string LightRiddleAnswerScore;

	[ConfigProperty("LightRiddleBeginDate", "LightRiddleBeginDate", "2014/2/13 0:00:00")]
	public static readonly string LightRiddleBeginDate;

	[ConfigProperty("LightRiddleEndDate", "LightRiddleEndDate", "2014/2/28 0:00:00")]
	public static readonly string LightRiddleEndDate;

	[ConfigProperty("LightRiddleBeginTime", "LightRiddleBeginTime", "2014/2/13 12:30:00")]
	public static readonly string LightRiddleBeginTime;

	[ConfigProperty("LightRiddleEndTime", "LightRiddleEndTime", "2014/2/13 13:00:00")]
	public static readonly string LightRiddleEndTime;

	[ConfigProperty("LightRiddleFreeHitNum", "LightRiddleFreeHitNum", 2)]
	public static readonly int LightRiddleFreeHitNum;

	[ConfigProperty("LightRiddleFreeComboNum", "LightRiddleFreeComboNum", 2)]
	public static readonly int LightRiddleFreeComboNum;

	[ConfigProperty("LightRiddleComboMoney", "LightRiddleComboMoney", 30)]
	public static readonly int LightRiddleComboMoney;

	[ConfigProperty("LightRiddleHitMoney", "LightRiddleHitMoney", 30)]
	public static readonly int LightRiddleHitMoney;

	[ConfigProperty("LightRiddleAnswerTime", "LightRiddleAnswerTime", 15)]
	public static readonly int LightRiddleAnswerTime;

	[ConfigProperty("LightRiddleOpenLevel", "LightRiddleOpenLevel", 15)]
	public static readonly int LightRiddleOpenLevel;

	[ConfigProperty("WarriorFamRaidPricePerMin", "WarriorFamRaidPricePerMin", 10)]
	public static readonly int WarriorFamRaidPricePerMin;

	[ConfigProperty("WarriorFamRaidPriceSmall", "WarriorFamRaidPriceSmall", 30000)]
	public static readonly int WarriorFamRaidPriceSmall;

	[ConfigProperty("WarriorFamRaidPriceBig", "WarriorFamRaidPriceBig ", 40000)]
	public static readonly int WarriorFamRaidPriceBig;

	[ConfigProperty("WarriorFamRaidDDTPrice", "WarriorFamRaidDDTPrice", 5000)]
	public static readonly int WarriorFamRaidDDTPrice;

	[ConfigProperty("WarriorFamRaidTimeRemain", "WarriorFamRaidTimeRemain", 120)]
	public static readonly int WarriorFamRaidTimeRemain;

	[ConfigProperty("ChristmasBeginDate", "ChristmasBeginDate", "2013/12/17 0:00:00")]
	public static readonly string ChristmasBeginDate;

	[ConfigProperty("ChristmasEndDate", "ChristmasEndDate", "2013/12/25 0:00:00")]
	public static readonly string ChristmasEndDate;

	[ConfigProperty("ChristmasGifts", "ChristmasGifts", "201148,10|201149,35|201150,70|201151,120|201152,220|201153,370|201154,650|201155,1000|201156,100")]
	public static readonly string ChristmasGifts;

	[ConfigProperty("ChristmasGiftsMaxNum", "ChristmasGiftsMaxNum", 1000)]
	public static readonly int ChristmasGiftsMaxNum;

	[ConfigProperty("ChristmasBuildSnowmanDoubleMoney", "ChristmasBuildSnowmanDoubleMoney", 10)]
	public static readonly int ChristmasBuildSnowmanDoubleMoney;

	[ConfigProperty("ChristmasBuyTimeMoney", "ChristmasBuyTimeMoney", 150)]
	public static readonly int ChristmasBuyTimeMoney;

	[ConfigProperty("ChristmasMinute", "ChristmasMinute", 60)]
	public static readonly int ChristmasMinute;

	[ConfigProperty("ChristmasBuyMinute", "ChristmasBuyMinute", 10)]
	public static readonly int ChristmasBuyMinute;

	[ConfigProperty("DragonBoatByMoney", "DragonBoatByMoney", "100:10,10")]
	public static readonly string DragonBoatByMoney;

	[ConfigProperty("DragonBoatByProps", "DragonBoatByProps", "1:10,10")]
	public static readonly string DragonBoatByProps;

	[ConfigProperty("DragonBoatMaxScore", "DragonBoatMaxScore", 30000)]
	public static readonly int DragonBoatMaxScore;

	[ConfigProperty("DragonBoatMinScore", "DragonBoatMinScore", 13000)]
	public static readonly int DragonBoatMinScore;

	[ConfigProperty("DragonBoatAreaMinScore", "DragonBoatAreaMinScore", 20000)]
	public static readonly int DragonBoatAreaMinScore;

	[ConfigProperty("DragonBoatProp", "DragonBoatProp", 11690)]
	public static readonly int DragonBoatProp;

	[ConfigProperty("DragonBoatConvertHours", "DragonBoatConvertHours", 72)]
	public static readonly int DragonBoatConvertHours;

	[ConfigProperty("PromotePackagePrice", "PromotePackagePrice", 3600)]
	public static readonly int PromotePackagePrice;

	[ConfigProperty("IsPromotePackageOpen", "IsPromotePackageOpen", false)]
	public static readonly bool IsPromotePackageOpen;

	[ConfigProperty("SearchGoodsFreeLimit", "SearchGoodsFreeLimit", 0)]
	public static readonly int SearchGoodsFreeLimit;

	[ConfigProperty("SearchGoodsPayMoney", "SearchGoodsPayMoney", 20)]
	public static readonly int SearchGoodsPayMoney;

	[ConfigProperty("SearchGoodsFreeCount", "SearchGoodsFreeCount", 3)]
	public static readonly int SearchGoodsFreeCount;

	[ConfigProperty("SearchGoodsTakeCardMoney", "SearchGoodsTakeCardMoney", "0|50|120")]
	public static readonly string SearchGoodsTakeCardMoney;

	private static void Load(Type type)
	{
		using ServiceBussiness sb = new ServiceBussiness();
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsStatic)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), inherit: false);
				if (customAttributes.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
					fieldInfo.SetValue(null, LoadProperty(attrib, sb));
				}
			}
		}
	}

	private static void Save(Type type)
	{
		using ServiceBussiness sb = new ServiceBussiness();
		FieldInfo[] fields = type.GetFields();
		foreach (FieldInfo fieldInfo in fields)
		{
			if (fieldInfo.IsStatic)
			{
				object[] customAttributes = fieldInfo.GetCustomAttributes(typeof(ConfigPropertyAttribute), inherit: false);
				if (customAttributes.Length != 0)
				{
					ConfigPropertyAttribute attrib = (ConfigPropertyAttribute)customAttributes[0];
					SaveProperty(attrib, sb, fieldInfo.GetValue(null));
				}
			}
		}
	}

	private static object LoadProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb)
	{
		string key = attrib.Key;
		ServerProperty serverProperty = sb.GetServerPropertyByKey(key);
		if (serverProperty == null)
		{
			serverProperty = new ServerProperty();
			serverProperty.Key = key;
			serverProperty.Value = attrib.DefaultValue.ToString();
			log.Error("Cannot find server property " + key + ",keep it default value!");
		}
		try
		{
			return Convert.ChangeType(serverProperty.Value, attrib.DefaultValue.GetType());
		}
		catch (Exception exception)
		{
			log.Error("Exception in GameProperties Load: ", exception);
			return null;
		}
	}

	private static void SaveProperty(ConfigPropertyAttribute attrib, ServiceBussiness sb, object value)
	{
		try
		{
			sb.UpdateServerPropertyByKey(attrib.Key, value.ToString());
		}
		catch (Exception exception)
		{
			log.Error("Exception in GameProperties Save: ", exception);
		}
	}

	public static void Refresh()
	{
		log.Info("Refreshing game properties!");
		Load(typeof(GameProperties));
	}

	public static List<int> getProp(string prop)
	{
		List<int> list = new List<int>();
		string[] array = prop.Split('|');
		string[] array2 = array;
		foreach (string value in array2)
		{
			list.Add(Convert.ToInt32(value));
		}
		return list;
	}

	public static List<int> VIPExp()
	{
		return getProp(VIPExpForEachLv);
	}

	public static List<int> RuneExp()
	{
		return getProp(RuneLevelUpExp);
	}

	public static int ConsortiaStrengExp(int Lv)
	{
		return getProp(ConsortiaStrengthenEx)[Lv];
	}

	public static int VIPStrengthenExp(int vipLv)
	{
		return getProp(VIPStrengthenEx)[vipLv];
	}

	public static int HoleLevelUpExp(int lv)
	{
		return getProp(HoleLevelUpExpList)[lv];
	}

	public static int[] ConvertStringArrayToIntArray(string str)
	{
		List<int> list = new List<int>();
		string[] array = new string[3] { "99999", "999999", "9999999" };
		switch (str)
		{
		case "NewChickenEagleEyePrice":
			array = NewChickenEagleEyePrice.Split(',');
			break;
		case "NewChickenOpenCardPrice":
			array = NewChickenOpenCardPrice.Split(',');
			break;
		case "PyramidRevivePrice":
			array = PyramidRevivePrice.Split(',');
			break;
		}
		string[] array2 = array;
		foreach (string value in array2)
		{
			list.Add(Convert.ToInt32(value));
		}
		return list.ToArray();
	}

	public static void Save()
	{
		log.Info("Saving game properties into db!");
		Save(typeof(GameProperties));
	}
}
