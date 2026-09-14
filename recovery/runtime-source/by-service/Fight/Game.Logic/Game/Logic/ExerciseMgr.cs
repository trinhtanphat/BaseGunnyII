using System;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic;

public class ExerciseMgr
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private static Dictionary<int, ExerciseInfo> _exercises;

	private static ReaderWriterLock m_lock;

	private static ThreadSafeRandom rand;

	public static bool Init()
	{
		try
		{
			m_lock = new ReaderWriterLock();
			_exercises = new Dictionary<int, ExerciseInfo>();
			rand = new ThreadSafeRandom();
			return LoadExercise(_exercises);
		}
		catch (Exception exception)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("ExercisesMgr", exception);
			}
			return false;
		}
	}

	public static bool ReLoad()
	{
		try
		{
			Dictionary<int, ExerciseInfo> dictionary = new Dictionary<int, ExerciseInfo>();
			if (LoadExercise(dictionary))
			{
				m_lock.AcquireWriterLock(-1);
				try
				{
					_exercises = dictionary;
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
				log.Error("ExerciseMgr", exception);
			}
		}
		return false;
	}

	private static bool LoadExercise(Dictionary<int, ExerciseInfo> Exercise)
	{
		using (PlayerBussiness playerBussiness = new PlayerBussiness())
		{
			ExerciseInfo[] allExercise = playerBussiness.GetAllExercise();
			ExerciseInfo[] array = allExercise;
			foreach (ExerciseInfo exerciseInfo in array)
			{
				if (!Exercise.ContainsKey(exerciseInfo.Grage))
				{
					Exercise.Add(exerciseInfo.Grage, exerciseInfo);
				}
			}
		}
		return true;
	}

	public static ExerciseInfo FindExercise(int Grage)
	{
		if (Grage == 0)
		{
			Grage = 1;
		}
		m_lock.AcquireReaderLock(-1);
		try
		{
			if (_exercises.ContainsKey(Grage))
			{
				return _exercises[Grage];
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

	public static int GetMaxLevel()
	{
		if (_exercises == null)
		{
			Init();
		}
		return _exercises.Values.Count;
	}

	public static int GetExercise(int GP, string type)
	{
		int result = 0;
		for (int i = 1; i <= GetMaxLevel(); i++)
		{
			switch (type)
			{
			case "L":
				result = FindExercise(i).ExerciseL;
				break;
			case "H":
				result = FindExercise(i).ExerciseH;
				break;
			case "D":
				result = FindExercise(i).ExerciseD;
				break;
			case "AG":
				result = FindExercise(i).ExerciseAG;
				break;
			case "A":
				result = FindExercise(i).ExerciseA;
				break;
			}
			if (GP <= FindExercise(i).GP)
			{
				break;
			}
		}
		return result;
	}
}
