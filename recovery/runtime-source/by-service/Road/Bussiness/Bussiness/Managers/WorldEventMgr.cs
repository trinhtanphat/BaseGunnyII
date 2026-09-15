using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Managers;

public class WorldEventMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, LuckyStartToptenAwardInfo> m_luckyStartToptenAward;

	private static Dictionary<int, LuckyStartToptenAwardInfo> m_lanternriddlesToptenAward;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom random = new ThreadSafeRandom();

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, LuckyStartToptenAwardInfo> dictionary = new Dictionary<int, LuckyStartToptenAwardInfo>();
			Dictionary<int, LuckyStartToptenAwardInfo> dictionary2 = new Dictionary<int, LuckyStartToptenAwardInfo>();
			if (LoadData(dictionary, dictionary2))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					m_luckyStartToptenAward = dictionary;
					m_lanternriddlesToptenAward = dictionary2;
					return true;
				}
				catch
				{
				}
				finally
				{
					m_lock.ReleaseWriterLock();
				}
			}
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ReLoad", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			m_luckyStartToptenAward = new Dictionary<int, LuckyStartToptenAwardInfo>();
			m_lanternriddlesToptenAward = new Dictionary<int, LuckyStartToptenAwardInfo>();
			return LoadData(m_luckyStartToptenAward, m_lanternriddlesToptenAward);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Init", exception);
			}
			return false;
		}
	}

	public static bool LoadData(Dictionary<int, LuckyStartToptenAwardInfo> luckyStarts, Dictionary<int, LuckyStartToptenAwardInfo> lanternriddles)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			LuckyStartToptenAwardInfo[] allLuckyStartToptenAward = playerBussiness.GetAllLuckyStartToptenAward();
			LuckyStartToptenAwardInfo[] array = allLuckyStartToptenAward;
			foreach (LuckyStartToptenAwardInfo luckyStartToptenAwardInfo in array)
			{
				if (!luckyStarts.Keys.Contains(luckyStartToptenAwardInfo.ID))
				{
					luckyStarts.Add(luckyStartToptenAwardInfo.ID, luckyStartToptenAwardInfo);
				}
			}
			LuckyStartToptenAwardInfo[] allLanternriddlesTopTenAward = playerBussiness.GetAllLanternriddlesTopTenAward();
			LuckyStartToptenAwardInfo[] array2 = allLanternriddlesTopTenAward;
			foreach (LuckyStartToptenAwardInfo luckyStartToptenAwardInfo2 in array2)
			{
				if (!lanternriddles.Keys.Contains(luckyStartToptenAwardInfo2.ID))
				{
					lanternriddles.Add(luckyStartToptenAwardInfo2.ID, luckyStartToptenAwardInfo2);
				}
			}
		}
		return true;
	}

	public static List<LuckyStartToptenAwardInfo> GetLuckyStartToptenAward()
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		foreach (LuckyStartToptenAwardInfo value in m_luckyStartToptenAward.Values)
		{
			list.Add(value);
		}
		return list;
	}

	public static List<LuckyStartToptenAwardInfo> GetLuckyStartAwardByRank(int rank)
	{
		int num = 0;
		switch (rank)
		{
		case 1:
			num = 11;
			break;
		case 2:
			num = 12;
			break;
		case 3:
			num = 13;
			break;
		case 4:
		case 5:
			num = 14;
			break;
		case 6:
		case 7:
			num = 15;
			break;
		case 8:
		case 9:
		case 10:
			num = 16;
			break;
		}
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		foreach (LuckyStartToptenAwardInfo value in m_luckyStartToptenAward.Values)
		{
			if (value.Type == num)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static List<LuckyStartToptenAwardInfo> GetLanternriddlesAwardByRank(int rank)
	{
		List<LuckyStartToptenAwardInfo> list = new List<LuckyStartToptenAwardInfo>();
		foreach (LuckyStartToptenAwardInfo value in m_lanternriddlesToptenAward.Values)
		{
			if (value.Type == rank)
			{
				list.Add(value);
			}
		}
		return list;
	}

	public static bool SendItemsToMail(List<ItemInfo> infos, int PlayerId, string Nickname, string title)
	{
		bool result = false;
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			List<ItemInfo> list = new List<ItemInfo>();
			foreach (ItemInfo info in infos)
			{
				if (info.Template.MaxCount == 1)
				{
					for (int i = 0; i < info.Count; i++)
					{
						ItemInfo itemInfo = ItemInfo.CloneFromTemplate(info.Template, info);
						itemInfo.Count = 1;
						list.Add(itemInfo);
					}
				}
				else
				{
					list.Add(info);
				}
			}
			for (int j = 0; j < list.Count; j += 5)
			{
				MailInfo mailInfo = new MailInfo();
				mailInfo.Title = title;
				mailInfo.Gold = 0;
				mailInfo.IsExist = true;
				mailInfo.Money = 0;
				mailInfo.Receiver = Nickname;
				mailInfo.ReceiverID = PlayerId;
				mailInfo.Sender = "Hệ thống";
				mailInfo.SenderID = 0;
				mailInfo.Type = 9;
				mailInfo.GiftToken = 0;
				MailInfo mailInfo2 = mailInfo;
				StringBuilder stringBuilder = new StringBuilder();
				StringBuilder stringBuilder2 = new StringBuilder();
				stringBuilder.Append(LanguageMgr.GetTranslation("Game.Server.GameUtils.CommonBag.AnnexRemark"));
				int num = j;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo2.Annex1 = itemInfo2.ItemID.ToString();
					mailInfo2.Annex1Name = itemInfo2.Template.Name;
					stringBuilder.Append("1、" + mailInfo2.Annex1Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("1、" + mailInfo2.Annex1Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 1;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo2.Annex2 = itemInfo2.ItemID.ToString();
					mailInfo2.Annex2Name = itemInfo2.Template.Name;
					stringBuilder.Append("2、" + mailInfo2.Annex2Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("2、" + mailInfo2.Annex2Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 2;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo2.Annex3 = itemInfo2.ItemID.ToString();
					mailInfo2.Annex3Name = itemInfo2.Template.Name;
					stringBuilder.Append("3、" + mailInfo2.Annex3Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("3、" + mailInfo2.Annex3Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 3;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo2.Annex4 = itemInfo2.ItemID.ToString();
					mailInfo2.Annex4Name = itemInfo2.Template.Name;
					stringBuilder.Append("4、" + mailInfo2.Annex4Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("4、" + mailInfo2.Annex4Name + "x" + itemInfo2.Count + ";");
				}
				num = j + 4;
				if (list.Count > num)
				{
					ItemInfo itemInfo2 = list[num];
					if (itemInfo2.ItemID == 0)
					{
						playerBussiness.AddGoods(itemInfo2);
					}
					mailInfo2.Annex5 = itemInfo2.ItemID.ToString();
					mailInfo2.Annex5Name = itemInfo2.Template.Name;
					stringBuilder.Append("5、" + mailInfo2.Annex5Name + "x" + itemInfo2.Count + ";");
					stringBuilder2.Append("5、" + mailInfo2.Annex5Name + "x" + itemInfo2.Count + ";");
				}
				mailInfo2.AnnexRemark = stringBuilder.ToString();
				mailInfo2.Content = stringBuilder2.ToString();
				result = playerBussiness.SendMail(mailInfo2);
			}
		}
		return result;
	}
}
