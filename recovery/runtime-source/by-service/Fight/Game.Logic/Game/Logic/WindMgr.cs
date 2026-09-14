using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class WindMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static readonly int[] CategoryID = new int[9] { 1001, 1002, 1003, 1004, 1005, 1005, 1007, 1008, 1009 };

	private static readonly int[] WindID = new int[11]
	{
		0, 1, 2, 3, 4, 5, 6, 7, 8, 9,
		10
	};

	private static readonly string[] fontWind = new string[11]
	{
		"•", "1", "2", "3", "4", "5", "6", "7", "8", "9",
		"0"
	};

	private static readonly Color[] c = new Color[8]
	{
		Color.Yellow,
		Color.Red,
		Color.Blue,
		Color.Green,
		Color.Orange,
		Color.Aqua,
		Color.DarkCyan,
		Color.Purple
	};

	private static readonly string[] font = new string[3] { "Verdana", "Comic Sans MS", "Tahoma" };

	private static Dictionary<int, WindInfo> _winds;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_winds = new Dictionary<int, WindInfo>();
			rand = new ThreadSafeRandom();
			return LoadWinds(_winds);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("WindInfoMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, WindInfo> winds = new Dictionary<int, WindInfo>();
			if (LoadWinds(winds))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_winds = winds;
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
				log.Error("WindMgr", exception);
			}
		}
		return false;
	}

	public static byte[] ReadImageFile(string imageLocation)
	{
		FileInfo fileInfo = new FileInfo(imageLocation);
		long length = fileInfo.Length;
		FileStream input = new FileStream(imageLocation, FileMode.Open, FileAccess.Read);
		BinaryReader binaryReader = new BinaryReader(input);
		return binaryReader.ReadBytes((int)length);
	}

	public static bool isSmall(string wind)
	{
		return wind == fontWind[0] || wind == fontWind[1];
	}

	public static byte[] CreateVane(string wind)
	{
		int num = 1;
		int width = 18;
		if (isSmall(wind))
		{
			width = 10;
		}
		Bitmap bitmap = new Bitmap(width, 32);
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		try
		{
			graphics.Clear(Color.Transparent);
			int num2 = rand.Next(7);
			Brush brush = new SolidBrush(c[num2]);
			StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			int num3 = rand.Next(WindMgr.font.Length);
			Font font = new Font(WindMgr.font[num3], 16f, FontStyle.Bold);
			Point point = new Point(8, 16);
			if (isSmall(wind))
			{
				if (wind == fontWind[0])
				{
					font = new Font(WindMgr.font[num3], 10f, FontStyle.Regular);
				}
				point = new Point(4, 16);
			}
			float num4 = ThreadSafeRandom.NextStatic(-num, num);
			graphics.TranslateTransform(point.X, point.Y);
			graphics.RotateTransform(num4);
			graphics.DrawString(wind.ToString(), font, brush, 1f, 1f, stringFormat);
			graphics.RotateTransform(0f - num4);
			graphics.TranslateTransform(2f, 0f - (float)point.Y);
			MemoryStream memoryStream = new MemoryStream();
			bitmap.Save(memoryStream, ImageFormat.Png);
			return memoryStream.ToArray();
		}
		finally
		{
			graphics.Dispose();
			bitmap.Dispose();
		}
	}

	private static bool LoadWinds(Dictionary<int, WindInfo> Winds)
	{
		int[] windID = WindID;
		foreach (int num in windID)
		{
			WindInfo windInfo = new WindInfo();
			byte[] array = CreateVane(fontWind[num]);
			if (array == null || array.Length <= 0)
			{
				if (log.IsErrorEnabled)
				{
					log.Error("Load Wind Error!");
				}
				return false;
			}
			windInfo.WindID = num;
			windInfo.WindPic = array;
			if (!Winds.ContainsKey(num))
			{
				Winds.Add(num, windInfo);
			}
		}
		return true;
	}

	public static List<WindInfo> GetWind()
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			List<WindInfo> list = new List<WindInfo>();
			for (int i = 0; i < _winds.Values.Count; i++)
			{
				list.Add(_winds[i]);
			}
			if (list.Count > 0)
			{
				return list;
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static WindInfo FindWind(int ID)
	{
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_winds.ContainsKey(ID))
			{
				return _winds[ID];
			}
		}
		catch
		{
		}
		finally
		{
			m_lock.ReleaseReaderLock();
		}
		return null;
	}

	public static byte GetWindID(int wind, int pos)
	{
		if (wind < 10)
		{
			switch (pos)
			{
			case 1:
				return 10;
			case 3:
				return (byte)((wind == 0) ? 10u : ((uint)wind));
			}
		}
		if (wind >= 10 && wind < 20)
		{
			switch (pos)
			{
			case 1:
				return 1;
			case 3:
				return (byte)((wind - 10 == 0) ? 10u : ((uint)(wind - 10)));
			}
		}
		if (wind >= 20 && wind < 30)
		{
			switch (pos)
			{
			case 1:
				return 2;
			case 3:
				return (byte)((wind - 20 == 0) ? 10u : ((uint)(wind - 20)));
			}
		}
		if (wind >= 30 && wind < 40)
		{
			switch (pos)
			{
			case 1:
				return 3;
			case 3:
				return (byte)((wind - 30 == 0) ? 10u : ((uint)(wind - 30)));
			}
		}
		if (wind >= 40 && wind < 50)
		{
			switch (pos)
			{
			case 1:
				return 4;
			case 3:
				return (byte)((wind - 40 == 0) ? 10u : ((uint)(wind - 40)));
			}
		}
		return 0;
	}
}
