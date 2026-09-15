using System;

namespace Bussiness;

public class ThreadSafeRandom
{
	private static Random randomStatic = new Random();

	private Random random = new Random();

	public static int NextStatic()
	{
		lock (randomStatic)
		{
			return randomStatic.Next();
		}
	}

	public static int NextStatic(int maxValue)
	{
		lock (randomStatic)
		{
			return randomStatic.Next(maxValue);
		}
	}

	public static int NextStatic(int minValue, int maxValue)
	{
		lock (randomStatic)
		{
			return randomStatic.Next(minValue, maxValue);
		}
	}

	public static void NextStatic(byte[] keys)
	{
		lock (randomStatic)
		{
			randomStatic.NextBytes(keys);
		}
	}

	public int Next()
	{
		lock (random)
		{
			return random.Next();
		}
	}

	public int Next(int maxValue)
	{
		lock (random)
		{
			return random.Next(maxValue);
		}
	}

	public int Next(int minValue, int maxValue)
	{
		lock (random)
		{
			return random.Next(minValue, maxValue);
		}
	}

	public void Shuffer<T>(T[] array)
	{
		for (int num = array.Length; num > 1; num--)
		{
			int num2 = random.Next(num);
			T val = array[num2];
			array[num2] = array[num - 1];
			array[num - 1] = val;
		}
	}

	public static void ShufferStatic<T>(T[] array)
	{
		for (int num = array.Length; num > 1; num--)
		{
			int num2 = randomStatic.Next(num);
			T val = array[num2];
			array[num2] = array[num - 1];
			array[num - 1] = val;
		}
	}
}
