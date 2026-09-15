using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.Buffer;
using Game.Server.GameObjects;
using Game.Server.Packets;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Managers;

public class AwardMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, DailyAwardInfo> _dailyAward;

	private static bool _dailyAwardState;

	private static ReaderWriterLock m_lock;

	public static bool DailyAwardState
	{
		get
		{
			return _dailyAwardState;
		}
		set
		{
			_dailyAwardState = value;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, DailyAwardInfo> dictionary = new Dictionary<int, DailyAwardInfo>();
			if (LoadDailyAward(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_dailyAward = dictionary;
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
				log.Error("AwardMgr", exception);
			}
		}
		return false;
	}

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_dailyAward = new Dictionary<int, DailyAwardInfo>();
			_dailyAwardState = false;
			return LoadDailyAward(_dailyAward);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("AwardMgr", exception);
			}
			return false;
		}
	}

	private static bool LoadDailyAward(Dictionary<int, DailyAwardInfo> awards)
	{
		using (ProduceBussiness produceBussiness = new ProduceBussiness())
		{
			DailyAwardInfo[] allDailyAward = produceBussiness.GetAllDailyAward();
			DailyAwardInfo[] array = allDailyAward;
			foreach (DailyAwardInfo dailyAwardInfo in array)
			{
				if (!awards.ContainsKey(dailyAwardInfo.ID))
				{
					awards.Add(dailyAwardInfo.ID, dailyAwardInfo);
				}
			}
		}
		return true;
	}

	public static DailyAwardInfo[] GetAllAwardInfo()
	{
		DailyAwardInfo[] array = null;
		m_lock.AcquireReaderLock(-1);
		try
		{
			array = _dailyAward.Values.ToArray();
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		if (array != null)
		{
			return array;
		}
		return new DailyAwardInfo[0];
	}

	public static bool AddDailyAward(GamePlayer player)
	{
		if (DateTime.Now.Date == player.PlayerCharacter.LastAward.Date)
		{
			return false;
		}
		player.PlayerCharacter.DayLoginCount++;
		player.PlayerCharacter.LastAward = DateTime.Now;
		DailyAwardInfo[] allAwardInfo = GetAllAwardInfo();
		DailyAwardInfo[] array = allAwardInfo;
		foreach (DailyAwardInfo dailyAwardInfo in array)
		{
			if (dailyAwardInfo.Type == 0)
			{
				ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(dailyAwardInfo.TemplateID);
				if (itemTemplateInfo != null)
				{
					AbstractBuffer abstractBuffer = BufferList.CreateBufferHour(itemTemplateInfo, dailyAwardInfo.ValidDate);
					abstractBuffer.Start(player);
					return true;
				}
			}
		}
		return false;
	}

	public static bool AddSignAwards(GamePlayer player, int DailyLog)
	{
		DailyAwardInfo[] allAwardInfo = GetAllAwardInfo();
		new StringBuilder();
		string value = string.Empty;
		bool flag = false;
		int templateId = 0;
		int num = 1;
		int validDate = 0;
		bool isBinds = true;
		bool result = false;
		DailyAwardInfo[] array = allAwardInfo;
		foreach (DailyAwardInfo dailyAwardInfo in array)
		{
			flag = true;
			switch (DailyLog)
			{
			case 9:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			case 3:
				if (dailyAwardInfo.Type == DailyLog)
				{
					num = dailyAwardInfo.Count;
					player.AddGiftToken(num);
					result = true;
				}
				break;
			case 26:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			case 17:
				if (dailyAwardInfo.Type == DailyLog)
				{
					templateId = dailyAwardInfo.TemplateID;
					num = dailyAwardInfo.Count;
					validDate = dailyAwardInfo.ValidDate;
					isBinds = dailyAwardInfo.IsBinds;
					result = true;
				}
				break;
			}
		}
		ItemTemplateInfo itemTemplateInfo = ItemMgr.FindItemTemplate(templateId);
		if (itemTemplateInfo != null)
		{
			int num2 = num;
			for (int j = 0; j < num2; j += itemTemplateInfo.MaxCount)
			{
				int count = ((j + itemTemplateInfo.MaxCount > num2) ? (num2 - j) : itemTemplateInfo.MaxCount);
				ItemInfo itemInfo = ItemInfo.CreateFromTemplate(itemTemplateInfo, count, 113);
				itemInfo.ValidDate = validDate;
				itemInfo.IsBinds = isBinds;
				if (!player.AddTemplate(itemInfo, itemInfo.Template.BagType, itemInfo.Count, eItemNotice.GoodsTipTypeView, eItemNotice.OpenTypeView))
				{
					flag = true;
					using PlayerBussiness playerBussiness = new PlayerBussiness();
					itemInfo.UserID = 0;
					playerBussiness.AddGoods(itemInfo);
					MailInfo mailInfo = new MailInfo();
					mailInfo.Annex1 = itemInfo.ItemID.ToString();
					mailInfo.Content = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Content", itemInfo.Template.Name);
					mailInfo.Gold = 0;
					mailInfo.Money = 0;
					mailInfo.Receiver = player.PlayerCharacter.NickName;
					mailInfo.ReceiverID = player.PlayerCharacter.ID;
					mailInfo.Sender = mailInfo.Receiver;
					mailInfo.SenderID = mailInfo.ReceiverID;
					mailInfo.Title = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Title", itemInfo.Template.Name);
					mailInfo.Type = 15;
					playerBussiness.SendMail(mailInfo);
					value = LanguageMgr.GetTranslation("AwardMgr.AddDailyAward.Mail");
				}
			}
		}
		if (flag && !string.IsNullOrEmpty(value))
		{
			player.Out.SendMailResponse(player.PlayerCharacter.ID, eMailRespose.Receiver);
		}
		return result;
	}
}
