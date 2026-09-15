using System;

namespace Game.Logic.Phy.Object;

public class TurnedLiving : Living
{
	protected int m_delay;

	public int DefaultDelay;

	private int m_dander;

	private int m_psychic = 20;

	private int m_petMaxMP = 100;

	private int m_petMP = 10;

	public int Delay
	{
		get
		{
			return m_delay;
		}
		set
		{
			m_delay = value;
		}
	}

	public int psychic
	{
		get
		{
			return m_psychic;
		}
		set
		{
			m_psychic = value;
		}
	}

	public int PetMaxMP
	{
		get
		{
			return m_petMaxMP;
		}
		set
		{
			m_petMaxMP = value;
		}
	}

	public int PetMP
	{
		get
		{
			return m_petMP;
		}
		set
		{
			m_petMP = value;
		}
	}

	public int Dander
	{
		get
		{
			return m_dander;
		}
		set
		{
			m_dander = value;
		}
	}

	public TurnedLiving(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction)
		: base(id, game, team, name, modelId, maxBlood, immunity, direction)
	{
	}

	public override void Reset()
	{
		base.Reset();
	}

	public void AddDelay(int value)
	{
		m_delay += value;
	}

	public override void PrepareSelfTurn()
	{
		base.PrepareSelfTurn();
	}

	public void AddPetMP(int value)
	{
		if (value <= 0)
		{
			return;
		}
		if (base.IsLiving && PetMP < PetMaxMP)
		{
			m_petMP += value;
			if (m_petMP > PetMaxMP)
			{
				m_petMP = PetMaxMP;
			}
		}
		else
		{
			m_petMP = PetMaxMP;
		}
	}

	public void AddDander(int value)
	{
		if (value > 0 && base.IsLiving)
		{
			SetDander(m_dander + value);
		}
	}

	public void SetDander(int value)
	{
		m_dander = Math.Min(value, 200);
		if (base.SyncAtTime)
		{
			m_game.SendGameUpdateDander(this);
		}
	}

	public virtual void StartGame()
	{
	}

	public virtual void Skip(int spendTime)
	{
		if (base.IsAttacking)
		{
			StopAttacking();
			m_game.SendFightStatus(this, 0);
			m_game.CheckState(0);
		}
	}
}
