using System;
using System.Collections;
using System.Configuration;
using System.IO;
using System.Reflection;
using System.Text;
using System.Threading;
using log4net;

namespace Bussiness;

public class LanguageMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Hashtable LangsSentences = new Hashtable();

	private static string LanguageFile => ConfigurationManager.AppSettings["LanguagePath"];

	public static bool Setup(string path)
	{
		return Reload(path);
	}

	public static bool Reload(string path)
	{
		try
		{
			Hashtable hashtable = LoadLanguage(path);
			if (hashtable.Count > 0)
			{
				Interlocked.Exchange(ref LangsSentences, hashtable);
				return true;
			}
		}
		catch (Exception exception)
		{
			log.Error("Load language file error:", exception);
		}
		return false;
	}

	private static Hashtable LoadLanguage(string path)
	{
		Hashtable hashtable = new Hashtable();
		string text = path + LanguageFile;
		if (!File.Exists(text))
		{
			log.Error("Language file : " + text + " not found !");
		}
		else
		{
			string[] c = File.ReadAllLines(text, Encoding.UTF8);
			IList list = new ArrayList(c);
			foreach (string item in list)
			{
				if (!item.StartsWith("#") && item.IndexOf(':') != -1)
				{
					string[] array = new string[2]
					{
						item.Substring(0, item.IndexOf(':')),
						item.Substring(item.IndexOf(':') + 1)
					};
					array[1] = array[1].Replace("\t", "");
					hashtable[array[0]] = array[1];
				}
			}
		}
		return hashtable;
	}

	public static string GetTranslation(string translateId, params object[] args)
	{
		if (!LangsSentences.ContainsKey(translateId))
		{
			return translateId;
		}
		string text = (string)LangsSentences[translateId];
		try
		{
			text = string.Format(text, args);
		}
		catch (Exception exception)
		{
			log.Error("Parameters number error, ID: " + translateId + " (Arg count=" + args.Length + ")", exception);
		}
		if (text != null)
		{
			return text;
		}
		return translateId;
	}
}
