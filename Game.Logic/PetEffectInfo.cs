namespace Game.Logic
{
public class PetEffectInfo
{
	private bool m_critActive;

	private int m_petDelay;

	private bool m_activePetHit;

	private bool m_activeGuard;

	private bool m_isPetUseSkill;

	private int m_petBaseAtt;

	private int m_petSkillStase;

	private int m_addDameValue;

	private int m_addGuardValue;

	private int m_addAttackValue;

	private int m_addLuckValue;

	private int m_reduceDefendValue;

	public bool CritActive
	{
		get
		{
			return m_critActive;
		}
		set
		{
			m_critActive = value;
		}
	}

	public int PetDelay
	{
		get
		{
			return m_petDelay;
		}
		set
		{
			m_petDelay = value;
		}
	}

	public bool ActivePetHit
	{
		get
		{
			return m_activePetHit;
		}
		set
		{
			m_activePetHit = value;
		}
	}

	public bool ActiveGuard
	{
		get
		{
			return m_activeGuard;
		}
		set
		{
			m_activeGuard = value;
		}
	}

	public bool IsPetUseSkill
	{
		get
		{
			return m_isPetUseSkill;
		}
		set
		{
			m_isPetUseSkill = value;
		}
	}

	public int PetBaseAtt
	{
		get
		{
			return m_petBaseAtt;
		}
		set
		{
			m_petBaseAtt = value;
		}
	}

	public int PetSkillStase
	{
		get
		{
			return m_petSkillStase;
		}
		set
		{
			m_petSkillStase = value;
		}
	}

	public int AddDameValue
	{
		get
		{
			return m_addDameValue;
		}
		set
		{
			m_addDameValue = value;
		}
	}

	public int AddGuardValue
	{
		get
		{
			return m_addGuardValue;
		}
		set
		{
			m_addGuardValue = value;
		}
	}

	public int AddAttackValue
	{
		get
		{
			return m_addAttackValue;
		}
		set
		{
			m_addAttackValue = value;
		}
	}

	public int AddLuckValue
	{
		get
		{
			return m_addLuckValue;
		}
		set
		{
			m_addLuckValue = value;
		}
	}

	public int ReduceDefendValue
	{
		get
		{
			return m_reduceDefendValue;
		}
		set
		{
			m_reduceDefendValue = value;
		}
	}
}

}
