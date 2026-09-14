using System;
using System.Configuration;
using System.IO;
using System.Net;
using System.Reflection;
using System.Text;
using System.Web.Security;
using Bussiness.CenterService;
using SqlDataProvider.Data;
using log4net;

namespace Bussiness.Interface;

public abstract class BaseInterface
{
	protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	public static string GetInterName => ConfigurationManager.AppSettings["InterName"].ToLower();

	public static string GetLoginKey => ConfigurationManager.AppSettings["LoginKey"];

	public static string GetChargeKey => ConfigurationManager.AppSettings["ChargeKey"];

	public static string LoginUrl => ConfigurationManager.AppSettings["LoginUrl"];

	public virtual int ActiveGold => int.Parse(ConfigurationManager.AppSettings["DefaultGold"]);

	public virtual int ActiveMoney => int.Parse(ConfigurationManager.AppSettings["DefaultMoney"]);

	public static string GetNameBySite(string user, string site)
	{
		if (!string.IsNullOrEmpty(site))
		{
			string value = ConfigurationManager.AppSettings[$"LoginKey_{site}"];
			if (!string.IsNullOrEmpty(value))
			{
				user = $"{site}_{user}";
			}
		}
		return user;
	}

	public static DateTime ConvertIntDateTime(double d)
	{
		DateTime minValue = DateTime.MinValue;
		return TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1)).AddSeconds(d);
	}

	public static int ConvertDateTimeInt(DateTime time)
	{
		DateTime dateTime = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
		double totalSeconds = (time - dateTime).TotalSeconds;
		return (int)totalSeconds;
	}

	public static string md5(string str)
	{
		return FormsAuthentication.HashPasswordForStoringInConfigFile(str, "md5").ToLower();
	}

	public static string RequestContent(string Url)
	{
		return RequestContent(Url, 2560);
	}

	public static string RequestContent(string Url, int byteLength)
	{
		byte[] array = new byte[byteLength];
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
		httpWebRequest.ContentType = "text/plain";
		HttpWebResponse httpWebResponse = (HttpWebResponse)httpWebRequest.GetResponse();
		Stream responseStream = httpWebResponse.GetResponseStream();
		int count = responseStream.Read(array, 0, array.Length);
		string result = Encoding.UTF8.GetString(array, 0, count);
		responseStream.Close();
		return result;
	}

	public static string RequestContent(string Url, string param, string code)
	{
		Encoding encoding = Encoding.GetEncoding(code);
		byte[] bytes = encoding.GetBytes(param);
		encoding.GetString(bytes);
		byte[] array = new byte[2560];
		HttpWebRequest httpWebRequest = (HttpWebRequest)WebRequest.Create(Url);
		httpWebRequest.ServicePoint.Expect100Continue = false;
		httpWebRequest.Method = "POST";
		httpWebRequest.ContentType = "application/x-www-form-urlencoded";
		httpWebRequest.ContentLength = bytes.Length;
		using (Stream stream = httpWebRequest.GetRequestStream())
		{
			stream.Write(bytes, 0, bytes.Length);
		}
		using WebResponse webResponse = httpWebRequest.GetResponse();
		HttpWebResponse httpWebResponse = (HttpWebResponse)webResponse;
		Stream responseStream = httpWebResponse.GetResponseStream();
		int count = responseStream.Read(array, 0, array.Length);
		return Encoding.UTF8.GetString(array, 0, count);
	}

	public static BaseInterface CreateInterface()
	{
		return GetInterName switch
		{
			"qunying" => new QYInterface(),
			"sevenroad" => new SRInterface(),
			"duowan" => new DWInterface(),
			_ => null,
		};
	}

	public virtual PlayerInfo CreateLogin(string name, string password, ref string message, ref int isFirst, string IP, ref bool isError, bool firstValidate, ref bool isActive, string site, string nickname)
	{
		try
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			bool isExist = true;
			DateTime forbidDate = DateTime.Now;
			PlayerInfo player = playerBussiness.LoginGame(name, ref isFirst, ref isExist, ref isError, firstValidate, ref forbidDate, nickname);
			if (player == null)
			{
				if (!playerBussiness.ActivePlayer(ref player, name, password, sex: true, ActiveGold, ActiveMoney, IP, site))
				{
					player = null;
					message = LanguageMgr.GetTranslation("BaseInterface.LoginAndUpdate.Fail");
				}
				else
				{
					isActive = true;
					using CenterServiceClient centerServiceClient = new CenterServiceClient();
					centerServiceClient.ActivePlayer(isActive: true);
				}
			}
			else
			{
				if (!isExist)
				{
					message = LanguageMgr.GetTranslation("ManageBussiness.Forbid1", forbidDate.Year, forbidDate.Month, forbidDate.Day, forbidDate.Hour, forbidDate.Minute);
					return null;
				}
				using CenterServiceClient centerServiceClient2 = new CenterServiceClient();
				centerServiceClient2.CreatePlayer(player.ID, name, password, isFirst == 0);
			}
			return player;
		}
		catch (Exception exception)
		{
			log.Error("LoginAndUpdate", exception);
		}
		return null;
	}

	public virtual PlayerInfo LoginGame(string name, string pass, ref bool isFirst)
	{
		try
		{
			using CenterServiceClient centerServiceClient = new CenterServiceClient();
			int userID = 0;
			if (centerServiceClient.ValidateLoginAndGetID(name, pass, ref userID, ref isFirst))
			{
				PlayerInfo playerInfo = new PlayerInfo();
				playerInfo.ID = userID;
				playerInfo.UserName = name;
				return playerInfo;
			}
		}
		catch (Exception exception)
		{
			log.Error("LoginGame", exception);
		}
		return null;
	}

	public virtual string[] UnEncryptLogin(string content, ref int result, string site)
	{
		try
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(site))
			{
				text = ConfigurationManager.AppSettings[$"LoginKey_{site}"];
			}
			if (string.IsNullOrEmpty(text))
			{
				text = GetLoginKey;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = content.Split('|');
				if (array.Length > 3)
				{
					string text2 = md5(array[0] + array[1] + array[2] + text);
					if (text2 == array[3].ToLower())
					{
						return array;
					}
					result = 5;
				}
				else
				{
					result = 2;
				}
			}
			else
			{
				result = 4;
			}
		}
		catch (Exception exception)
		{
			log.Error("UnEncryptLogin", exception);
		}
		return new string[0];
	}

	public virtual string[] UnEncryptCharge(string content, ref int result, string site)
	{
		try
		{
			string text = string.Empty;
			if (!string.IsNullOrEmpty(site))
			{
				text = ConfigurationManager.AppSettings[$"ChargeKey_{site}"];
			}
			if (string.IsNullOrEmpty(text))
			{
				text = GetChargeKey;
			}
			if (!string.IsNullOrEmpty(text))
			{
				string[] array = content.Split('|');
				string text2 = md5(array[0] + array[1] + array[2] + array[3] + array[4] + text);
				if (array.Length > 5)
				{
					if (text2 == array[5].ToLower())
					{
						return array;
					}
					result = 7;
				}
				else
				{
					result = 8;
				}
			}
			else
			{
				result = 6;
			}
		}
		catch (Exception exception)
		{
			log.Error("UnEncryptCharge", exception);
		}
		return new string[0];
	}

	public virtual string[] UnEncryptSentReward(string content, ref int result, string key)
	{
		try
		{
			string[] array = content.Split('#');
			if (array.Length == 8)
			{
				string text = ConfigurationManager.AppSettings["SentRewardTimeSpan"];
				int num = int.Parse(string.IsNullOrEmpty(text) ? "1" : text);
				TimeSpan timeSpan = (string.IsNullOrEmpty(array[6]) ? new TimeSpan(1, 1, 1) : (DateTime.Now - ConvertIntDateTime(double.Parse(array[6]))));
				if (timeSpan.Days == 0 && timeSpan.Hours == 0 && timeSpan.Minutes < num)
				{
					if (string.IsNullOrEmpty(key))
					{
						return array;
					}
					string text2 = md5(array[2] + array[3] + array[4] + array[5] + array[6] + key);
					if (text2 == array[7].ToLower())
					{
						return array;
					}
					result = 5;
				}
				else
				{
					result = 7;
				}
			}
			else
			{
				result = 6;
			}
		}
		catch (Exception exception)
		{
			log.Error("UnEncryptSentReward", exception);
		}
		return new string[0];
	}

	public virtual bool GetUserSex(string name)
	{
		return true;
	}
}
