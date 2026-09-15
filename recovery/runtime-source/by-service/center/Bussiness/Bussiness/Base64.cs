namespace Bussiness;

public class Base64
{
	private static readonly string BASE64_CHARS = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789+/=";

	public static string encodeByteArray(byte[] param1)
	{
		string text = "";
		byte[] array = new byte[4];
		for (int i = 0; i < param1.Length; i += 4)
		{
			byte[] array2 = new byte[3];
			int num = 0;
			for (num = 0; num < param1.Length; num++)
			{
				if (num < 3)
				{
					if (num + i > param1.Length)
					{
						break;
					}
					array2[num] = param1[num + i];
				}
			}
			array[0] = (byte)((array2[0] & 0xFC) >> 2);
			array[1] = (byte)(((array2[0] & 3) << 4) | (array2[1] >> 4));
			array[2] = (byte)(((array2[1] & 0xF) << 2) | (array2[2] >> 6));
			array[3] = (byte)(array2[2] & 0x3F);
			for (int j = array2.Length; j < 3; j++)
			{
				array[j + 1] = 64;
			}
			for (int k = 0; k < array.Length; k++)
			{
				text += BASE64_CHARS.Substring(array[k], 1);
			}
		}
		text = text.Substring(0, param1.Length - 1);
		return text + "=";
	}

	public static byte[] decodeToByteArray2(string param1)
	{
		byte[] array = new byte[param1.Length];
		byte[] array2 = new byte[4];
		for (int i = 0; i < param1.Length; i += 4)
		{
			int num = 0;
			int num2 = 0;
			do
			{
				num2 = i + num;
				if (num < 4)
				{
					array2[num] = (byte)BASE64_CHARS.IndexOf(param1.Substring(num2, 1));
				}
				num++;
			}
			while (num2 < param1.Length);
			for (int j = 0; j < array2.Length && array2[j] != 64; j++)
			{
				array[i + j] = array2[j];
			}
		}
		return array;
	}

	public static byte[] decodeToByteArray(string param1)
	{
		byte[] array = new byte[param1.Length];
		byte[] array2 = new byte[4];
		byte[] array3 = new byte[3];
		for (int i = 0; i < param1.Length; i += 4)
		{
			int num = 0;
			int num2 = 0;
			do
			{
				num2 = i + num;
				if (num < 4)
				{
					array2[num] = (byte)BASE64_CHARS.IndexOf(param1.Substring(num2, 1));
				}
				num++;
			}
			while (num2 < param1.Length);
			array3[0] = (byte)((array2[0] << 2) + ((array2[1] & 0x30) >> 4));
			array3[1] = (byte)(((array2[1] & 0xF) << 4) + ((array2[2] & 0x3C) >> 2));
			array3[2] = (byte)(((array2[2] & 3) << 6) + array2[3]);
			for (int j = 0; j < array3.Length && array2[j + 1] != 64; j++)
			{
				array[i + j] = array3[j];
			}
		}
		return array;
	}
}
