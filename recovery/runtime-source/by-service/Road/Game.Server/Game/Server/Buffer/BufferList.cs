using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection;
using System.Threading;
using Bussiness;
using Game.Base.Packets;
using Game.Server.GameObjects;
using SqlDataProvider.Data;
using log4net;

namespace Game.Server.Buffer;

public class BufferList
{
	private static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

	private object m_lock;

	protected List<AbstractBuffer> m_buffers;

	protected ArrayList m_clearList;

	protected volatile sbyte m_changesCount;

	private GamePlayer m_player;

	protected ArrayList m_changedBuffers = new ArrayList();

	private int m_changeCount;

	public BufferList(GamePlayer player)
	{
		m_player = player;
		m_lock = new object();
		m_buffers = new List<AbstractBuffer>();
		m_clearList = new ArrayList();
	}

	public void LoadFromDatabase(int playerId)
	{
		lock (m_lock)
		{
			using (PlayerBussiness playerBussiness = new PlayerBussiness())
			{
				BufferInfo[] userBuffer = playerBussiness.GetUserBuffer(playerId);
				BeginChanges();
				BufferInfo[] array = userBuffer;
				foreach (BufferInfo info in array)
				{
					CreateBuffer(info)?.Start(m_player);
				}
				ConsortiaBufferInfo[] userConsortiaBuffer = playerBussiness.GetUserConsortiaBuffer(m_player.PlayerCharacter.ConsortiaID);
				ConsortiaBufferInfo[] array2 = userConsortiaBuffer;
				foreach (ConsortiaBufferInfo info2 in array2)
				{
					CreateConsortiaBuffer(info2)?.Start(m_player);
				}
				CommitChanges();
			}
			Update();
			m_player.ClearFightBuffOneMatch();
		}
	}

	public void SaveToDatabase()
	{
		lock (m_lock)
		{
			using PlayerBussiness playerBussiness = new PlayerBussiness();
			foreach (AbstractBuffer buffer in m_buffers)
			{
				playerBussiness.SaveBuffer(buffer.Info);
			}
			foreach (BufferInfo clear in m_clearList)
			{
				playerBussiness.SaveBuffer(clear);
			}
			m_clearList.Clear();
		}
	}

	public bool AddBuffer(AbstractBuffer buffer)
	{
		lock (m_buffers)
		{
			m_buffers.Add(buffer);
		}
		OnBuffersChanged(buffer);
		return true;
	}

	public bool RemoveBuffer(AbstractBuffer buffer)
	{
		lock (m_buffers)
		{
			if (m_buffers.Remove(buffer))
			{
				m_clearList.Add(buffer.Info);
			}
		}
		OnBuffersChanged(buffer);
		return true;
	}

	public void UpdateBuffer(AbstractBuffer buffer)
	{
		OnBuffersChanged(buffer);
	}

	protected void OnBuffersChanged(AbstractBuffer buffer)
	{
		if (!m_changedBuffers.Contains(buffer))
		{
			m_changedBuffers.Add(buffer);
		}
		if (m_changeCount <= 0 && m_changedBuffers.Count > 0)
		{
			UpdateChangedBuffers();
		}
	}

	public void BeginChanges()
	{
		Interlocked.Increment(ref m_changeCount);
	}

	public void CommitChanges()
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
		if (num <= 0 && m_changedBuffers.Count > 0)
		{
			UpdateChangedBuffers();
		}
	}

	public void UpdateChangedBuffers()
	{
		List<BufferInfo> list = new List<BufferInfo>();
		Dictionary<int, BufferInfo> dictionary = new Dictionary<int, BufferInfo>();
		foreach (AbstractBuffer changedBuffer in m_changedBuffers)
		{
			if (changedBuffer.Info.TemplateID > 100)
			{
				list.Add(changedBuffer.Info);
			}
		}
		List<AbstractBuffer> allBuffers = GetAllBuffers();
		foreach (AbstractBuffer item in allBuffers)
		{
			if (IsConsortiaBuff(item.Info.Type) && m_player.IsConsortia())
			{
				dictionary.Add(item.Info.TemplateID, item.Info);
			}
		}
		BufferInfo[] infos = list.ToArray();
		GSPacketIn pkg = m_player.Out.SendUpdateBuffer(m_player, infos);
		if (m_player.CurrentRoom != null)
		{
			m_player.CurrentRoom.SendToAll(pkg, m_player);
		}
		m_player.Out.SendUpdateConsotiaBuffer(m_player, dictionary);
		m_changedBuffers.Clear();
		dictionary.Clear();
	}

	public bool IsConsortiaBuff(int type)
	{
		return type > 100 && type < 115;
	}

	public virtual AbstractBuffer GetOfType(Type bufferType)
	{
		lock (m_buffers)
		{
			foreach (AbstractBuffer buffer in m_buffers)
			{
				if (buffer.GetType().Equals(bufferType))
				{
					return buffer;
				}
			}
		}
		return null;
	}

	public List<AbstractBuffer> GetAllBufferByTemplate()
	{
		List<AbstractBuffer> list = new List<AbstractBuffer>();
		lock (m_lock)
		{
			foreach (AbstractBuffer buffer in m_buffers)
			{
				if (buffer.Info.TemplateID > 100)
				{
					list.Add(buffer);
				}
			}
		}
		return list;
	}

	public List<AbstractBuffer> GetAllBuffers()
	{
		List<AbstractBuffer> list = new List<AbstractBuffer>();
		lock (m_lock)
		{
			foreach (AbstractBuffer buffer in m_buffers)
			{
				list.Add(buffer);
			}
		}
		return list;
	}

	public void Update()
	{
		List<AbstractBuffer> allBuffers = GetAllBuffers();
		foreach (AbstractBuffer item in allBuffers)
		{
			try
			{
				if (!item.Check())
				{
					item.Stop();
				}
			}
			catch (Exception message)
			{
				log.Error(message);
			}
		}
	}

	public static AbstractBuffer CreateConsortiaBuffer(ConsortiaBufferInfo info)
	{
		BufferInfo bufferInfo = new BufferInfo();
		bufferInfo.TemplateID = info.BufferID;
		bufferInfo.BeginDate = info.BeginDate;
		bufferInfo.ValidDate = info.ValidDate;
		bufferInfo.Value = info.Value;
		bufferInfo.Type = info.Type;
		bufferInfo.ValidCount = 1;
		bufferInfo.IsExist = true;
		return CreateBuffer(bufferInfo);
	}

	public static AbstractBuffer CreateBuffer(ItemTemplateInfo template, int ValidDate)
	{
		BufferInfo bufferInfo = new BufferInfo();
		bufferInfo.TemplateID = template.TemplateID;
		bufferInfo.BeginDate = DateTime.Now;
		bufferInfo.ValidDate = ValidDate * 24 * 60;
		bufferInfo.Value = template.Property2;
		bufferInfo.Type = template.Property1;
		bufferInfo.ValidCount = 1;
		bufferInfo.IsExist = true;
		return CreateBuffer(bufferInfo);
	}

	public static AbstractBuffer CreateBufferHour(ItemTemplateInfo template, int ValidHour)
	{
		BufferInfo bufferInfo = new BufferInfo();
		bufferInfo.TemplateID = template.TemplateID;
		bufferInfo.BeginDate = DateTime.Now;
		bufferInfo.ValidDate = ValidHour * 60;
		bufferInfo.Value = template.Property2;
		bufferInfo.Type = template.Property1;
		bufferInfo.ValidCount = 1;
		bufferInfo.IsExist = true;
		return CreateBuffer(bufferInfo);
	}

	public static AbstractBuffer CreateBufferMinutes(ItemTemplateInfo template, int ValidMinutes)
	{
		BufferInfo bufferInfo = new BufferInfo();
		bufferInfo.TemplateID = template.TemplateID;
		bufferInfo.BeginDate = DateTime.Now;
		bufferInfo.ValidDate = ValidMinutes;
		bufferInfo.Value = template.Property2;
		bufferInfo.Type = template.Property1;
		bufferInfo.ValidCount = 1;
		bufferInfo.IsExist = true;
		return CreateBuffer(bufferInfo);
	}

	public static AbstractBuffer CreatePayBuffer(int type, int Value, int ValidMinutes)
	{
		BufferInfo bufferInfo = new BufferInfo();
		bufferInfo.TemplateID = 0;
		bufferInfo.BeginDate = DateTime.Now;
		bufferInfo.ValidDate = ValidMinutes;
		bufferInfo.Value = Value;
		bufferInfo.Type = type;
		bufferInfo.ValidCount = 1;
		bufferInfo.IsExist = true;
		return CreateBuffer(bufferInfo);
	}

	public static AbstractBuffer CreatePayBuffer(int type, int Value, int ValidMinutes, int id)
	{
		BufferInfo bufferInfo = new BufferInfo();
		bufferInfo.TemplateID = id;
		bufferInfo.BeginDate = DateTime.Now;
		bufferInfo.ValidDate = ValidMinutes;
		bufferInfo.Value = Value;
		bufferInfo.Type = type;
		bufferInfo.ValidCount = 1;
		bufferInfo.IsExist = true;
		return CreateBuffer(bufferInfo);
	}

	public static AbstractBuffer CreateBuffer(BufferInfo info)
	{
		AbstractBuffer result = null;
		switch (info.Type)
		{
		case 11:
			result = new KickProtectBuffer(info);
			break;
		case 12:
			result = new OfferMultipleBuffer(info);
			break;
		case 13:
			result = new GPMultipleBuffer(info);
			break;
		case 15:
			result = new PropsBuffer(info);
			break;
		case 26:
			result = new HonorBuffer(info);
			break;
		case 74:
			result = new DefendBuffer(info);
			break;
		case 75:
			result = new AttackBuffer(info);
			break;
		case 76:
			result = new GuardBuffer(info);
			break;
		case 77:
			result = new AgiBuffer(info);
			break;
		case 78:
			result = new DameBuffer(info);
			break;
		case 79:
			result = new HpBuffer(info);
			break;
		case 80:
			result = new LuckBuffer(info);
			break;
		case 101:
			result = new ConsortionAddBloodGunCountBuffer(info);
			break;
		case 102:
			result = new ConsortionAddDamageBuffer(info);
			break;
		case 103:
			result = new ConsortionAddCriticalBuffer(info);
			break;
		case 104:
			result = new ConsortionAddMaxBloodBuffer(info);
			break;
		case 105:
			result = new ConsortionAddPropertyBuffer(info);
			break;
		case 106:
			result = new ConsortionReduceEnergyUseBuffer(info);
			break;
		case 107:
			result = new ConsortionAddEnergyBuffer(info);
			break;
		case 108:
			result = new ConsortionAddEffectTurnBuffer(info);
			break;
		case 109:
			result = new ConsortionAddOfferRateBuffer(info);
			break;
		case 110:
			result = new ConsortionAddPercentGoldOrGPBuffer(info);
			break;
		case 111:
			result = new ConsortionAddSpellCountBuffer(info);
			break;
		case 112:
			result = new ConsortionReduceDanderBuffer(info);
			break;
		case 114:
			result = new ActivityDungeonBubbleBuffer(info);
			break;
		case 115:
			result = new ActivityDungeonNetBuffer(info);
			break;
		case 400:
			result = new WorldBossHPBuffer(info);
			break;
		case 401:
			result = new WorldBossAttrackBuffer(info);
			break;
		case 402:
			result = new WorldBossHP_MoneyBuffBuffer(info);
			break;
		case 403:
			result = new WorldBossAttrack_MoneyBuffBuffer(info);
			break;
		case 404:
			result = new WorldBossMetalSlugBuffer(info);
			break;
		case 405:
			result = new WorldBossAncientBlessingsBuffer(info);
			break;
		case 406:
			result = new WorldBossAddDamageBuffer(info);
			break;
		}
		return result;
	}
}
