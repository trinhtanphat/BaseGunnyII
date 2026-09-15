using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;

namespace Bussiness;

public class CheckCode
{
	private enum RandomStringMode
	{
		LowerLetter,
		UpperLetter,
		Letter,
		Digital,
		Mix
	}

	public static ThreadSafeRandom rand = new ThreadSafeRandom();

	private static Color[] c = new Color[2]
	{
		Color.Gray,
		Color.DimGray
	};

	private static string[] font = new string[5] { "Verdana", "Terminal", "Comic Sans MS", "Arial", "Tekton Pro" };

	private static char[] digitals = new char[9] { '1', '2', '3', '4', '5', '6', '7', '8', '9' };

	private static char[] lowerLetters = new char[21]
	{
		'a', 'b', 'c', 'd', 'e', 'f', 'h', 'k', 'm', 'n',
		'p', 'q', 'r', 's', 't', 'u', 'v', 'w', 'x', 'y',
		'z'
	};

	private static char[] upperLetters = new char[22]
	{
		'A', 'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'M',
		'N', 'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X',
		'Y', 'Z'
	};

	private static char[] letters = new char[50]
	{
		'a', 'b', 'c', 'd', 'e', 'f', 'g', 'h', 'i', 'j',
		'k', 'l', 'm', 'n', 'p', 'q', 'r', 's', 't', 'u',
		'v', 'w', 'x', 'y', 'z', 'A', 'B', 'C', 'D', 'E',
		'F', 'G', 'H', 'I', 'J', 'K', 'L', 'M', 'N', 'P',
		'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y', 'Z'
	};

	private static char[] mix = new char[51]
	{
		'2', '3', '4', '5', '6', '7', '8', '9', 'a', 'b',
		'c', 'd', 'e', 'f', 'h', 'k', 'm', 'n', 'p', 'q',
		'r', 's', 't', 'u', 'v', 'w', 'x', 'y', 'z', 'A',
		'B', 'C', 'D', 'E', 'F', 'G', 'H', 'K', 'M', 'N',
		'P', 'Q', 'R', 'S', 'T', 'U', 'V', 'W', 'X', 'Y',
		'Z'
	};

	public static byte[] CreateImage(string randomcode)
	{
		int num = 30;
		int width = randomcode.Length * 30;
		Bitmap bitmap = new Bitmap(width, 32);
		Graphics graphics = Graphics.FromImage(bitmap);
		graphics.SmoothingMode = SmoothingMode.HighQuality;
		try
		{
			graphics.Clear(Color.Transparent);
			int num2 = rand.Next(2);
			Brush brush = new SolidBrush(c[num2]);
			for (int i = 0; i < 1; i++)
			{
				int num3 = rand.Next(bitmap.Width / 2);
				int num4 = rand.Next(bitmap.Width * 3 / 4, bitmap.Width);
				int num5 = rand.Next(bitmap.Height);
				int num6 = rand.Next(bitmap.Height);
				graphics.DrawBezier(new Pen(c[num2], 2f), num3, num5, (num3 + num4) / 4, 0f, (num3 + num4) * 3 / 4, bitmap.Height, num4, num6);
			}
			char[] array = randomcode.ToCharArray();
			StringFormat stringFormat = new StringFormat(StringFormatFlags.NoClip);
			stringFormat.Alignment = StringAlignment.Center;
			stringFormat.LineAlignment = StringAlignment.Center;
			for (int j = 0; j < array.Length; j++)
			{
				int num7 = rand.Next(5);
				Font font = new Font(CheckCode.font[num7], 22f, FontStyle.Bold);
				Point point = new Point(16, 16);
				float num8 = ThreadSafeRandom.NextStatic(-num, num);
				graphics.TranslateTransform(point.X, point.Y);
				graphics.RotateTransform(num8);
				graphics.DrawString(array[j].ToString(), font, brush, 1f, 1f, stringFormat);
				graphics.RotateTransform(0f - num8);
				graphics.TranslateTransform(2f, -point.Y);
			}
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

	private static string GenerateRandomString(int length, RandomStringMode mode)
	{
		string text = string.Empty;
		if (length == 0)
		{
			return text;
		}
		switch (mode)
		{
		case RandomStringMode.Digital:
		{
			for (int j = 0; j < length; j++)
			{
				text += digitals[rand.Next(0, digitals.Length)];
			}
			break;
		}
		case RandomStringMode.LowerLetter:
		{
			for (int l = 0; l < length; l++)
			{
				text += lowerLetters[rand.Next(0, lowerLetters.Length)];
			}
			break;
		}
		case RandomStringMode.UpperLetter:
		{
			for (int m = 0; m < length; m++)
			{
				text += upperLetters[rand.Next(0, upperLetters.Length)];
			}
			break;
		}
		case RandomStringMode.Letter:
		{
			for (int k = 0; k < length; k++)
			{
				text += letters[rand.Next(0, letters.Length)];
			}
			break;
		}
		default:
		{
			for (int i = 0; i < length; i++)
			{
				text += mix[rand.Next(0, mix.Length)];
			}
			break;
		}
		}
		return text;
	}

	public static string GenerateCheckCode()
	{
		return GenerateRandomString(4, RandomStringMode.Mix);
	}
}
