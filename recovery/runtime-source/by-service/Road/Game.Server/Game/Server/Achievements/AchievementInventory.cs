using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Threading;
using Bussiness;
using Bussiness.Managers;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Achievements;

public class AchievementInventory
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_lock;

	protected List<BaseAchievement> m_list;

	protected List<AchievementDataInfo> m_datas;

	protected ArrayList m_clearList;

	private GamePlayer m_player;

	private byte[] m_states;

	private UnicodeEncoding m_converter;

	protected List<BaseAchievement> m_changedAchievements = new List<BaseAchievement>();

	private int m_changeCount;

	public AchievementInventory(GamePlayer player)
	{
		m_converter = new UnicodeEncoding();
		m_player = player;
		m_lock = new object();
		m_list = new List<BaseAchievement>();
		m_clearList = new ArrayList();
		m_datas = new List<AchievementDataInfo>();
	}

	public void LoadFromDatabase(int playerId)
	{
		lock (m_lock)
		{
			m_states = InitAchievement();
			using (ProduceBussiness produceBussiness = new ProduceBussiness())
			{
				AchievementDataInfo[] allAchievementData = produceBussiness.GetAllAchievementData(playerId);
				BeginChanges();
				AchievementDataInfo[] array = allAchievementData;
				foreach (AchievementDataInfo achievementDataInfo in array)
				{
					AchievementInfo singleAchievement = AchievementMgr.GetSingleAchievement(achievementDataInfo.AchievementID);
					if (singleAchievement != null)
					{
						AddAchievement(new BaseAchievement(singleAchievement, achievementDataInfo));
					}
					AddAchievementData(achievementDataInfo);
				}
				CommitChanges();
			}
			List<BaseAchievement> list = m_list;
		}
	}

	private byte[] InitAchievement()
	{
		byte[] array = new byte[200];
		for (int i = 0; i < 200; i++)
		{
			array[i] = 0;
		}
		return array;
	}

	protected void OnQuestsChanged(BaseAchievement quest)
	{
		if (!m_changedAchievements.Contains(quest))
		{
			m_changedAchievements.Add(quest);
		}
		if (m_changeCount <= 0 && m_changedAchievements.Count > 0)
		{
			UpdateChangedQuests();
		}
	}

	private bool AddAchievement(BaseAchievement achivev)
	{
		lock (m_list)
		{
			m_list.Add(achivev);
		}
		OnQuestsChanged(achivev);
		achivev.AddToPlayer(m_player);
		return true;
	}

	public void Update(BaseAchievement achieve)
	{
		OnQuestsChanged(achieve);
	}

	private bool AddAchievementData(AchievementDataInfo data)
	{
		lock (m_list)
		{
			m_datas.Add(data);
		}
		return true;
	}

	public void SaveToDatabase()
	{
	}

	private void BeginChanges()
	{
		Interlocked.Increment(ref m_changeCount);
	}

	private void CommitChanges()
	{
		int num = Interlocked.Decrement(ref m_changeCount);
		if (num < 0)
		{
			if (log.IsErrorEnabled)
			{
				log.Error("Inventory changes counter is bellow zero (forgot to use BeginChanges?)!\n\n" + Environment.StackTrace);
			}
			Thread.VolatileWrite(ref m_changeCount, 0);
		}
		if (num <= 0 && m_changedAchievements.Count > 0)
		{
			UpdateChangedQuests();
		}
	}

	public void UpdateChangedQuests()
	{
		m_player.Out.SendAchievementDatas(m_player, m_changedAchievements.ToArray());
		m_changedAchievements.Clear();
	}
}
