namespace Game.Logic;

public class LivingConfig
{
	private bool m_isWorldBoss;

	private bool m_isChristmasBoss;

	private bool m_isHelper;

	private bool m_isConsortiaBoss;

	private bool m_isTurn;

	private bool m_isFly;

	private byte m_isBotom;

	private bool m_isShowBlood;

	private bool m_isShowSmallMapPoint;

	private int m_reduceBloodStart;

	public bool IsWorldBoss
	{
		get
		{
			return m_isWorldBoss;
		}
		set
		{
			m_isWorldBoss = value;
		}
	}

	public bool IsChristmasBoss
	{
		get
		{
			return m_isChristmasBoss;
		}
		set
		{
			m_isChristmasBoss = value;
		}
	}

	public bool IsHelper
	{
		get
		{
			return m_isHelper;
		}
		set
		{
			m_isHelper = value;
		}
	}

	public bool isConsortiaBoss
	{
		get
		{
			return m_isConsortiaBoss;
		}
		set
		{
			m_isConsortiaBoss = value;
		}
	}

	public bool IsTurn
	{
		get
		{
			return m_isTurn;
		}
		set
		{
			m_isTurn = value;
		}
	}

	public bool IsFly
	{
		get
		{
			return m_isFly;
		}
		set
		{
			m_isFly = value;
		}
	}

	public byte isBotom
	{
		get
		{
			return m_isBotom;
		}
		set
		{
			m_isBotom = value;
		}
	}

	public bool isShowBlood
	{
		get
		{
			return m_isShowBlood;
		}
		set
		{
			m_isShowBlood = value;
		}
	}

	public bool isShowSmallMapPoint
	{
		get
		{
			return m_isShowSmallMapPoint;
		}
		set
		{
			m_isShowSmallMapPoint = value;
		}
	}

	public int ReduceBloodStart
	{
		get
		{
			return m_reduceBloodStart;
		}
		set
		{
			m_reduceBloodStart = value;
		}
	}
}
