using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Reflection;
using Game.Logic.AI;
using Game.Logic.AI.Npc;
using Game.Logic.Actions;
using Game.Server.Managers;
using SqlDataProvider.Data;
using log4net;

namespace Game.Logic.Phy.Object;

public class SimpleBoss : TurnedLiving
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private NpcInfo m_npcInfo;

	private ABrain m_ai;

	private List<SimpleNpc> m_child = new List<SimpleNpc>();

	private List<SimpleBoss> m_boss = new List<SimpleBoss>();

	private Dictionary<Player, int> m_mostHateful;

	public int TotalCure;

	public NpcInfo NpcInfo => m_npcInfo;

	public List<SimpleNpc> Child => m_child;

	public int CurrentLivingNpcNum
	{
		get
		{
			int num = 0;
			foreach (SimpleNpc item in Child)
			{
				if (!item.IsLiving)
				{
					num++;
				}
			}
			return Child.Count - num;
		}
	}

	public List<SimpleBoss> Boss => m_boss;

	public int CurrentLivingBossNum
	{
		get
		{
			int num = 0;
			foreach (SimpleBoss item in Boss)
			{
				if (!item.IsLiving)
				{
					num++;
				}
			}
			return Boss.Count - num;
		}
	}

	public SimpleBoss(int id, BaseGame game, NpcInfo npcInfo, int direction, int type, string actions)
		: base(id, game, npcInfo.Camp, npcInfo.Name, npcInfo.ModelID, npcInfo.Blood, npcInfo.Immunity, direction)
	{
		switch (type)
		{
		case 0:
			base.Type = eLivingType.SimpleBoss;
			break;
		case 1:
			base.Type = eLivingType.ClearEnemy;
			break;
		default:
			base.Type = (eLivingType)type;
			break;
		}
		base.ActionStr = actions;
		m_mostHateful = new Dictionary<Player, int>();
		m_npcInfo = npcInfo;
		m_ai = ScriptMgr.CreateInstance(npcInfo.Script) as ABrain;
		if (m_ai == null)
		{
			log.ErrorFormat("Can't create abrain :{0}", npcInfo.Script);
			m_ai = SimpleBrain.Simple;
		}
		m_ai.Game = m_game;
		m_ai.Body = this;
		try
		{
			m_ai.OnCreated();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss Created error:{1}", arg);
		}
	}

	public override void Reset()
	{
		if (base.Config.IsWorldBoss)
		{
			m_maxBlood = int.MaxValue;
		}
		else
		{
			m_maxBlood = m_npcInfo.Blood;
		}
		BaseDamage = m_npcInfo.BaseDamage;
		BaseGuard = m_npcInfo.BaseGuard;
		Attack = m_npcInfo.Attack;
		Defence = m_npcInfo.Defence;
		Agility = m_npcInfo.Agility;
		Lucky = m_npcInfo.Lucky;
		Grade = m_npcInfo.Level;
		Experience = m_npcInfo.Experience;
		m_delay = (int)Agility;
		SetRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		SetRelateDemagemRect(m_npcInfo.X, m_npcInfo.Y, m_npcInfo.Width, m_npcInfo.Height);
		base.Reset();
	}

	public override void Die()
	{
		base.Die();
	}

	public override void Die(int delay)
	{
		base.Die(delay);
	}

	public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
		if (source is Player)
		{
			Player key = source as Player;
			int num = damageAmount + criticalAmount;
			if (m_mostHateful.ContainsKey(key))
			{
				m_mostHateful[key] += num;
			}
			else
			{
				m_mostHateful.Add(key, num);
			}
		}
		return result;
	}

	public Player FindMostHatefulPlayer()
	{
		if (m_mostHateful.Count > 0)
		{
			KeyValuePair<Player, int> keyValuePair = m_mostHateful.ElementAt(0);
			foreach (KeyValuePair<Player, int> item in m_mostHateful)
			{
				if (keyValuePair.Value < item.Value)
				{
					keyValuePair = item;
				}
			}
			return keyValuePair.Key;
		}
		return null;
	}

	public void CreateChild(int id, int x, int y, int disToSecond, int maxCount, int direction)
	{
		if (CurrentLivingNpcNum < maxCount)
		{
			if (maxCount - CurrentLivingNpcNum >= 2)
			{
				Child.Add(((PVEGame)base.Game).CreateNpc(id, x + disToSecond, y, 1, direction));
				Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1, direction));
			}
			else if (maxCount - CurrentLivingNpcNum == 1)
			{
				Child.Add(((PVEGame)base.Game).CreateNpc(id, x, y, 1, direction));
			}
		}
	}

	public void CreateChild(int id, Point[] brithPoint, int maxCount, int maxCountForOnce, int type, int direction)
	{
		int num = base.Game.Random.Next(0, maxCountForOnce);
		for (int i = 0; i < num; i++)
		{
			int num2 = base.Game.Random.Next(0, brithPoint.Length);
			CreateChild(id, brithPoint[num2].X, brithPoint[num2].Y, 4, maxCount, direction);
		}
	}

	public void CreateBoss(int id, int x, int y, int direction, int disToSecond, int maxCount, string action)
	{
		CreateBoss(id, x, y, direction, 1, disToSecond, maxCount, action);
	}

	public void CreateBoss(int id, int x, int y, int direction, int type, int disToSecond, int maxCount, string action)
	{
		if (CurrentLivingBossNum < maxCount)
		{
			if (maxCount - CurrentLivingNpcNum >= 2)
			{
				Boss.Add(((PVEGame)base.Game).CreateBoss(id, x + disToSecond, y, direction, type, action));
				Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, type, action));
			}
			else if (maxCount - CurrentLivingBossNum == 1)
			{
				Boss.Add(((PVEGame)base.Game).CreateBoss(id, x, y, direction, type, action));
			}
		}
	}

	public void RandomSay(string[] msg, int type, int delay, int finishTime)
	{
		int num = base.Game.Random.Next(0, 2);
		if (num == 1)
		{
			int num2 = base.Game.Random.Next(0, msg.Count());
			string msg2 = msg[num2];
			m_game.AddAction(new LivingSayAction(this, msg2, type, delay, finishTime));
		}
	}

	public override void PrepareNewTurn()
	{
		base.PrepareNewTurn();
		try
		{
			m_ai.OnBeginNewTurn();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss BeginNewTurn error:{1}", arg);
		}
	}

	public override void PrepareSelfTurn()
	{
		base.PrepareSelfTurn();
		AddDelay(m_npcInfo.Delay);
		try
		{
			m_ai.OnBeginSelfTurn();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss BeginSelfTurn error:{1}", arg);
		}
	}

	public override void StartAttacking()
	{
		base.StartAttacking();
		try
		{
			m_ai.OnStartAttacking();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss StartAttacking error:{1}", arg);
		}
		if (base.IsAttacking)
		{
			StopAttacking();
		}
	}

	public override void StopAttacking()
	{
		base.StopAttacking();
		try
		{
			m_ai.OnStopAttacking();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss StopAttacking error:{1}", arg);
		}
	}

	public override void Dispose()
	{
		base.Dispose();
		try
		{
			m_ai.Dispose();
		}
		catch (Exception arg)
		{
			log.ErrorFormat("SimpleBoss Dispose error:{1}", arg);
		}
	}
}
