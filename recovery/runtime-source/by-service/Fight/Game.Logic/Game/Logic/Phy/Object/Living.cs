using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Game.Base.Packets;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.PetEffects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class Living : Physics
{
	protected BaseGame m_game;

	protected int m_maxBlood;

	protected int m_blood;

	private int m_team;

	private string m_name;

	private string m_action;

	private string m_modelId;

	private Rectangle m_demageRect;

	private int m_state;

	private int m_doAction;

	public int m_direction;

	private eLivingType m_type;

	private Random rand;

	public double BaseDamage = 10.0;

	public double BaseGuard = 10.0;

	public double Defence = 10.0;

	public double Attack = 10.0;

	public double Agility = 10.0;

	public double Lucky = 10.0;

	public int Grade = 1;

	public int Experience = 10;

	public float CurrentDamagePlus;

	public float CurrentShootMinus;

	public bool IgnoreArmor;

	public bool AddArmor;

	public bool ControlBall;

	public int ReduceCritFisrtGem;

	public int ReduceCritSecondGem;

	public int DefenFisrtGem;

	public int DefenSecondGem;

	public int DefendActiveGem;

	public int AttackGemLimit;

	public bool NoHoleTurn;

	public bool CurrentIsHitTarget;

	public int FlyingPartical;

	public int TurnNum;

	public int TotalHurt;

	public int TotalDameLiving;

	public int TotalHitTargetCount;

	public int TotalShootCount;

	public int TotalKill;

	public int MaxBeatDis;

	public int EffectsCount;

	public int ShootMovieDelay;

	public List<int> ScoreArr;

	private PetEffectList m_petEffectList;

	private EffectList m_effectList;

	public bool EffectTrigger;

	public bool PetEffectTrigger;

	protected bool m_syncAtTime;

	private bool m_isPet;

	private PetEffectInfo m_petEffects;

	private bool m_vaneOpen;

	private FightBufferInfo m_fightBufferInfo;

	private bool m_autoBoot;

	private LivingConfig m_config;

	private int m_FallCount;

	private int m_FindCount;

	private bool m_isAttacking;

	protected static int MOVE_SPEED = 2;

	protected static int GHOST_MOVE_SPEED = 8;

	protected static int STEP_X = 1;

	protected static int STEP_Y = 7;

	private int m_specialSkillDelay;

	private bool m_isFrost;

	private bool m_isHide;

	private bool m_isNoHole;

	private bool m_isSeal;

	public bool AutoBoot
	{
		get
		{
			return m_autoBoot;
		}
		set
		{
			m_autoBoot = value;
		}
	}

	public LivingConfig Config
	{
		get
		{
			return m_config;
		}
		set
		{
			m_config = value;
		}
	}

	public PetEffectInfo PetEffects
	{
		get
		{
			return m_petEffects;
		}
		set
		{
			m_petEffects = value;
		}
	}

	public FightBufferInfo FightBuffers
	{
		get
		{
			return m_fightBufferInfo;
		}
		set
		{
			m_fightBufferInfo = value;
		}
	}

	public bool VaneOpen
	{
		get
		{
			return m_vaneOpen;
		}
		set
		{
			m_vaneOpen = value;
		}
	}

	public bool isPet
	{
		get
		{
			return m_isPet;
		}
		set
		{
			m_isPet = value;
		}
	}

	public string ActionStr
	{
		get
		{
			return m_action;
		}
		set
		{
			m_action = value;
		}
	}

	public BaseGame Game => m_game;

	public string Name => m_name;

	public string ModelId => m_modelId;

	public int Team => m_team;

	public bool SyncAtTime
	{
		get
		{
			return m_syncAtTime;
		}
		set
		{
			m_syncAtTime = value;
		}
	}

	public int FallCount
	{
		get
		{
			return m_FallCount;
		}
		set
		{
			m_FallCount = value;
		}
	}

	public int FindCount
	{
		get
		{
			return m_FindCount;
		}
		set
		{
			m_FindCount = value;
		}
	}

	public int Direction
	{
		get
		{
			return m_direction;
		}
		set
		{
			if (m_direction != value)
			{
				m_direction = value;
				SetRect(-m_rect.X - m_rect.Width, m_rect.Y, m_rect.Width, m_rect.Height);
				SetRectBomb(-m_rectBomb.X - m_rectBomb.Width, m_rectBomb.Y, m_rectBomb.Width, m_rectBomb.Height);
				SetRelateDemagemRect(-m_demageRect.X - m_demageRect.Width, m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
				if (m_syncAtTime)
				{
					m_game.SendLivingUpdateDirection(this);
				}
			}
		}
	}

	public eLivingType Type
	{
		get
		{
			return m_type;
		}
		set
		{
			m_type = value;
		}
	}

	public bool IsSay { get; set; }

	public EffectList EffectList => m_effectList;

	public PetEffectList PetEffectList => m_petEffectList;

	public bool IsAttacking => m_isAttacking;

	public int SpecialSkillDelay
	{
		get
		{
			return m_specialSkillDelay;
		}
		set
		{
			m_specialSkillDelay = value;
		}
	}

	public bool IsFrost
	{
		get
		{
			return m_isFrost;
		}
		set
		{
			if (m_isFrost != value)
			{
				m_isFrost = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateFrozenState(this);
				}
			}
		}
	}

	public bool IsNoHole
	{
		get
		{
			return m_isNoHole;
		}
		set
		{
			if (m_isNoHole != value)
			{
				m_isNoHole = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateNoHoleState(this);
				}
			}
		}
	}

	public bool IsHide
	{
		get
		{
			return m_isHide;
		}
		set
		{
			if (m_isHide != value)
			{
				m_isHide = value;
				if (m_syncAtTime)
				{
					m_game.SendGameUpdateHideState(this);
				}
			}
		}
	}

	public int State
	{
		get
		{
			return m_state;
		}
		set
		{
			if (m_state != value)
			{
				m_state = value;
				if (m_syncAtTime)
				{
					m_game.SendLivingUpdateAngryState(this);
				}
			}
		}
	}

	public int DoAction
	{
		get
		{
			return m_doAction;
		}
		set
		{
			if (m_doAction != value)
			{
				m_doAction = value;
			}
		}
	}

	public int MaxBlood => m_maxBlood;

	public int Blood
	{
		get
		{
			return m_blood;
		}
		set
		{
			m_blood = value;
		}
	}

	public event LivingEventHandle Died;

	public event LivingTakedDamageEventHandle BeforeTakeDamage;

	public event LivingTakedDamageEventHandle TakePlayerDamage;

	public event LivingEventHandle BeginNextTurn;

	public event LivingEventHandle BeginSelfTurn;

	public event LivingEventHandle BeginAttacking;

	public event LivingEventHandle BeginAttacked;

	public event LivingEventHandle EndAttacking;

	public event KillLivingEventHanlde AfterKillingLiving;

	public event KillLivingEventHanlde AfterKilledByLiving;

	public Living(int id, BaseGame game, int team, string name, string modelId, int maxBlood, int immunity, int direction)
		: base(id)
	{
		m_vaneOpen = false;
		m_isPet = false;
		m_action = "";
		m_game = game;
		m_team = team;
		m_name = name;
		m_modelId = modelId;
		m_maxBlood = maxBlood;
		m_direction = direction;
		m_state = 0;
		m_doAction = -1;
		MaxBeatDis = 100;
		AddArmor = false;
		ReduceCritFisrtGem = 0;
		ReduceCritSecondGem = 0;
		DefenFisrtGem = 0;
		DefenSecondGem = 0;
		DefendActiveGem = 0;
		AttackGemLimit = 0;
		m_effectList = new EffectList(this, immunity);
		m_petEffectList = new PetEffectList(this, immunity);
		m_fightBufferInfo = new FightBufferInfo();
		SetupPetEffect();
		m_config = new LivingConfig();
		m_syncAtTime = true;
		m_type = eLivingType.Living;
		rand = new Random();
		ScoreArr = new List<int>();
		m_autoBoot = false;
	}

	public void SetupPetEffect()
	{
		m_petEffects = new PetEffectInfo();
		m_petEffects.CritActive = false;
		m_petEffects.ActivePetHit = false;
		m_petEffects.PetDelay = 0;
		m_petEffects.PetBaseAtt = 0;
		m_petEffects.PetSkillStase = 0;
		m_petEffects.ActiveGuard = false;
	}

	public void SetRelateDemagemRect(int x, int y, int width, int height)
	{
		m_demageRect.X = x;
		m_demageRect.Y = y;
		m_demageRect.Width = width;
		m_demageRect.Height = height;
	}

	public void DemagemRect(int x, int y, int width, int height)
	{
		m_demageRect.Width = width;
		m_demageRect.Height = height;
	}

	public Point GetShootPoint()
	{
		if (this is SimpleBoss)
		{
			if (m_direction <= 0)
			{
				return new Point(X + ((SimpleBoss)this).NpcInfo.FireX, Y + ((SimpleBoss)this).NpcInfo.FireY);
			}
			return new Point(X - ((SimpleBoss)this).NpcInfo.FireX, Y + ((SimpleBoss)this).NpcInfo.FireY);
		}
		if (m_direction <= 0)
		{
			return new Point(X + m_rect.X - 5, Y + m_rect.Y - 5);
		}
		return new Point(X - m_rect.X + 5, Y + m_rect.Y - 5);
	}

	public Rectangle GetDirectDemageRect()
	{
		if (m_direction <= 0)
		{
			return new Rectangle(X + m_demageRect.X, Y + m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
		}
		return new Rectangle(X - m_demageRect.X, Y + m_demageRect.Y, m_demageRect.Width, m_demageRect.Height);
	}

	public List<Rectangle> GetDirectBoudRect()
	{
		List<Rectangle> list = new List<Rectangle>();
		list.Add((m_direction > 0) ? new Rectangle(X - base.Bound.X, Y + base.Bound.Y, base.Bound.Width, base.Bound.Height) : new Rectangle(X + base.Bound.X, Y + base.Bound.Y, base.Bound.Width, base.Bound.Height));
		list.Add((m_direction > 0) ? new Rectangle(X - base.Bound1.X, Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height) : new Rectangle(X + base.Bound1.X, Y + base.Bound1.Y, base.Bound1.Width, base.Bound1.Height));
		return list;
	}

	public double Distance(Point p)
	{
		List<double> list = new List<double>();
		Rectangle directDemageRect = GetDirectDemageRect();
		for (int i = directDemageRect.X; i <= directDemageRect.X + directDemageRect.Width; i += 10)
		{
			list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (directDemageRect.Y - p.Y) * (directDemageRect.Y - p.Y)));
			list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (directDemageRect.Y + directDemageRect.Height - p.Y) * (directDemageRect.Y + directDemageRect.Height - p.Y)));
		}
		for (int j = directDemageRect.Y; j <= directDemageRect.Y + directDemageRect.Height; j += 10)
		{
			list.Add(Math.Sqrt((directDemageRect.X - p.X) * (directDemageRect.X - p.X) + (j - p.Y) * (j - p.Y)));
			list.Add(Math.Sqrt((directDemageRect.X + directDemageRect.Width - p.X) * (directDemageRect.X + directDemageRect.Width - p.X) + (j - p.Y) * (j - p.Y)));
		}
		return list.Min();
	}

	public double BoundDistance(Point p)
	{
		List<double> list = new List<double>();
		foreach (Rectangle item in GetDirectBoudRect())
		{
			for (int i = item.X; i <= item.X + item.Width; i += 10)
			{
				list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (item.Y - p.Y) * (item.Y - p.Y)));
				list.Add(Math.Sqrt((i - p.X) * (i - p.X) + (item.Y + item.Height - p.Y) * (item.Y + item.Height - p.Y)));
			}
			for (int j = item.Y; j <= item.Y + item.Height; j += 10)
			{
				list.Add(Math.Sqrt((item.X - p.X) * (item.X - p.X) + (j - p.Y) * (j - p.Y)));
				list.Add(Math.Sqrt((item.X + item.Width - p.X) * (item.X + item.Width - p.X) + (j - p.Y) * (j - p.Y)));
			}
		}
		return list.Min();
	}

	public virtual void Reset()
	{
		m_blood = m_maxBlood;
		m_isFrost = false;
		m_isHide = false;
		m_isNoHole = false;
		m_isLiving = true;
		TurnNum = 0;
		TotalHurt = 0;
		TotalKill = 0;
		TotalShootCount = 0;
		TotalHitTargetCount = 0;
	}

	public virtual void PickBox(Box box)
	{
		box.UserID = base.Id;
		box.Die();
		if (m_syncAtTime)
		{
			m_game.SendGamePickBox(this, box.Id, 0, "");
		}
	}

	public virtual void PickBall(Ball ball)
	{
		ball.Die();
		string currentAction = ball.CurrentAction;
		ball.PlayMovie(ball.ActionMapping[currentAction], 1000, 0);
	}

	public override void PrepareNewTurn()
	{
		ShootMovieDelay = 0;
		CurrentDamagePlus = 1f;
		CurrentShootMinus = 1f;
		IgnoreArmor = false;
		ControlBall = false;
		NoHoleTurn = false;
		CurrentIsHitTarget = false;
		PrepareAttackGemLilit();
		PrepareDefendGem();
		OnBeginNewTurn();
	}

	public virtual void PrepareSelfTurn()
	{
		OnBeginSelfTurn();
	}

	public void StartAttacked()
	{
		OnStartAttacked();
	}

	public void PrepareAttackGemLilit()
	{
		if (AttackGemLimit > 0)
		{
			AttackGemLimit--;
		}
	}

	public void PrepareDefendGem()
	{
		if (DefenFisrtGem > 0 && DefenSecondGem > 0)
		{
			int[] array = new int[2] { DefenFisrtGem, DefenSecondGem };
			int num = rand.Next(array.Length);
			DefendActiveGem = array[num];
		}
		else
		{
			DefendActiveGem = DefenFisrtGem;
		}
	}

	public virtual void StartAttacking()
	{
		if (!m_isAttacking)
		{
			m_isAttacking = true;
			OnStartAttacking();
		}
	}

	public virtual void StopAttacking()
	{
		if (m_isAttacking)
		{
			m_isAttacking = false;
			OnStopAttacking();
		}
	}

	public override void CollidedByObject(Physics phy)
	{
		if (phy is SimpleBomb)
		{
			((SimpleBomb)phy).Bomb();
		}
	}

	public override void StartMoving()
	{
		StartMoving(0, 30);
	}

	public virtual void StartMoving(int delay, int speed)
	{
		if (!Config.IsFly)
		{
			if (m_map.IsEmpty(X, Y))
			{
				FallFrom(X, Y, null, delay, 0, speed);
			}
			base.StartMoving();
		}
	}

	public void SetXY(int x, int y, int delay)
	{
		m_game.AddAction(new LivingDirectSetXYAction(this, x, y, delay));
	}

	public void AddEffect(AbstractEffect effect, int delay)
	{
		m_game.AddAction(new LivingDelayEffectAction(this, effect, delay));
	}

	public void Say(string msg, int type, int delay, int finishTime)
	{
		m_game.AddAction(new LivingSayAction(this, msg, type, delay, finishTime));
	}

	public void Say(string msg, int type, int delay)
	{
		m_game.AddAction(new LivingSayAction(this, msg, type, delay, 1000));
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed)
	{
		return MoveTo(x, y, action, delay, sAction, speed, null);
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback)
	{
		return MoveTo(x, y, action, delay, sAction, speed, callback, 0);
	}

	public bool MoveTo(int x, int y, string action, int delay, string sAction, int speed, LivingCallBack callback, int delayCallback)
	{
		if (m_x == x && m_y == y)
		{
			return false;
		}
		if (x < 0 || x > m_map.Bound.Width)
		{
			return false;
		}
		List<Point> list = new List<Point>();
		int x2 = m_x;
		int y2 = m_y;
		int num = ((x > x2) ? 1 : (-1));
		if (action == "fly")
		{
			Point item = new Point(x, y);
			Point point = new Point(x2, y2);
			Point point2 = new Point(x - point.X, y - point.Y);
			while (point2.Length() > (double)speed)
			{
				point2.Normalize(speed);
				point = new Point(point.X + point2.X, point.Y + point2.Y);
				point2 = new Point(x - point.X, y - point.Y);
				if (!(point != Point.Empty))
				{
					list.Add(item);
					break;
				}
				list.Add(point);
			}
		}
		else
		{
			while ((x - x2) * num > 0)
			{
				Point point3 = m_map.FindNextWalkPoint(x2, y2, num, speed * STEP_X, speed * STEP_Y);
				if (!(point3 != Point.Empty))
				{
					break;
				}
				list.Add(point3);
				x2 = point3.X;
				y2 = point3.Y;
			}
		}
		if (list.Count > 0)
		{
			m_game.AddAction(new LivingMoveToAction(this, list, action, delay, speed, sAction, callback, delayCallback));
			return true;
		}
		return false;
	}

	public bool FallFrom(int x, int y, string action, int delay, int type, int speed)
	{
		return FallFrom(x, y, action, delay, type, speed, null);
	}

	public bool FallFrom(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		Point point = m_map.FindYLineNotEmptyPoint(x, y);
		if (point == Point.Empty)
		{
			point = new Point(x, m_game.Map.Bound.Height + 1);
		}
		if (Y < point.Y)
		{
			m_game.AddAction(new LivingFallingAction(this, point.X, point.Y, speed, action, delay, type, callback));
			return true;
		}
		return false;
	}

	public bool FallFromTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		m_game.AddAction(new LivingFallingAction(this, x, y, speed, action, delay, type, callback));
		return true;
	}

	public bool JumpTo(int x, int y, string action, int delay, int type)
	{
		return JumpTo(x, y, action, delay, type, 20, null);
	}

	public bool JumpTo(int x, int y, string ation, int delay, int type, LivingCallBack callback)
	{
		return JumpTo(x, y, ation, delay, type, 20, callback);
	}

	public bool JumpTo(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		Point point = m_map.FindYLineNotEmptyPoint(x, y);
		if (point.Y < Y)
		{
			m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
			return true;
		}
		return false;
	}

	public bool JumpToSpeed(int x, int y, string action, int delay, int type, int speed, LivingCallBack callback)
	{
		Point point = m_map.FindYLineNotEmptyPoint(x, y);
		int y2 = point.Y;
		m_game.AddAction(new LivingJumpAction(this, point.X, point.Y, speed, action, delay, type, callback));
		return true;
	}

	public void ChangeDirection(int direction, int delay)
	{
		if (delay > 0)
		{
			m_game.AddAction(new LivingChangeDirectionAction(this, direction, delay));
		}
		else
		{
			Direction = direction;
		}
	}

	public double getHertAddition(ItemInfo item)
	{
		if (item == null)
		{
			return 0.0;
		}
		double num = item.Template.Property7;
		double y = item.StrengthenLevel;
		double a = num * Math.Pow(1.1, y) - num;
		return Math.Round(a) + num;
	}

	protected int MakeDamage(Living target)
	{
		if (target.Config.IsChristmasBoss)
		{
			return 1;
		}
		double baseDamage = BaseDamage;
		double num = target.BaseGuard;
		double num2 = target.Defence;
		double attack = Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)getHertAddition((target as Player).DeputyWeapon);
			num += (double)num3;
			num2 += (double)num3;
		}
		if (IgnoreArmor)
		{
			num = 0.0;
			num2 = 0.0;
		}
		float currentDamagePlus = CurrentDamagePlus;
		float currentShootMinus = CurrentShootMinus;
		double num4 = 0.95 * (num - (double)(3 * Grade)) / (500.0 + num - (double)(3 * Grade));
		double num5 = ((!(num2 - Lucky < 0.0)) ? (0.95 * (num2 - Lucky) / (600.0 + num2 - Lucky)) : 0.0);
		double num6 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5)) * (double)currentDamagePlus * (double)currentShootMinus;
		new Point(X, Y);
		if (num6 < 0.0)
		{
			return 1;
		}
		return (int)num6;
	}

	public bool Beat(Living target, string action, int demageAmount, int criticalAmount, int delay, int livingCount, int attackEffect)
	{
		if (target == null || !target.IsLiving)
		{
			return false;
		}
		demageAmount = MakeDamage(target);
		OnBeforeTakedDamage(target, ref demageAmount, ref criticalAmount);
		StartAttacked();
		int num = (int)target.Distance(X, Y);
		if (num <= MaxBeatDis)
		{
			if (X - target.X > 0)
			{
				Direction = -1;
			}
			else
			{
				Direction = 1;
			}
			m_game.AddAction(new LivingBeatAction(this, target, demageAmount, criticalAmount, action, delay, livingCount, attackEffect));
			return true;
		}
		return false;
	}

	public bool RangeAttacking(int fx, int tx, string action, int delay, List<Player> players)
	{
		if (base.IsLiving)
		{
			m_game.AddAction(new LivingRangeAttackingAction(this, fx, tx, action, delay, players));
			return true;
		}
		return false;
	}

	public void GetShootForceAndAngle(ref int x, ref int y, int bombId, int minTime, int maxTime, int bombCount, float time, ref int force, ref int angle)
	{
		if (minTime >= maxTime)
		{
			return;
		}
		BallInfo ballInfo = BallMgr.FindBall(bombId);
		if (m_game == null || ballInfo == null)
		{
			return;
		}
		Map map = m_game.Map;
		Point shootPoint = GetShootPoint();
		float num = x - shootPoint.X;
		float num2 = y - shootPoint.Y;
		float af = map.airResistance * (float)ballInfo.DragIndex;
		float f = map.gravity * (float)ballInfo.Weight * (float)ballInfo.Mass;
		float f2 = map.wind * (float)ballInfo.Wind;
		float m = ballInfo.Mass;
		for (float num3 = time; num3 <= 4f; num3 += 0.6f)
		{
			double num4 = ComputeVx(num, m, af, f2, num3);
			double num5 = ComputeVy(num2, m, af, f, num3);
			if (!(num5 < 0.0) || !(num4 * (double)m_direction > 0.0))
			{
				continue;
			}
			double num6 = Math.Sqrt(num4 * num4 + num5 * num5);
			if (num6 < 2000.0)
			{
				force = (int)num6;
				angle = (int)(Math.Atan(num5 / num4) / Math.PI * 180.0);
				if (num4 < 0.0)
				{
					angle += 180;
				}
				break;
			}
		}
		x = shootPoint.X;
		y = shootPoint.Y;
	}

	public bool ShootPoint(int x, int y, int bombId, int minTime, int maxTime, int bombCount, float time, int delay)
	{
		m_game.AddAction(new LivingShootAction(this, bombId, x, y, 0, 0, bombCount, minTime, maxTime, time, delay));
		return true;
	}

	public bool IsFriendly(Living living)
	{
		return !(living is Player) && living.Team == Team;
	}

	public bool Shoot(int bombId, int x, int y, int force, int angle, int bombCount, int delay)
	{
		m_game.AddAction(new LivingShootAction(this, bombId, x, y, force, angle, bombCount, delay, 0, 0f, 0));
		return true;
	}

	public static double ComputeVx(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 0.7;
	}

	public static double ComputeVy(double dx, float m, float af, float f, float t)
	{
		return (dx - (double)(f / m * t * t / 2f)) / (double)t + (double)(af / m) * dx * 1.3;
	}

	public static double ComputDX(double v, float m, float af, float f, float dt)
	{
		return v * (double)dt + ((double)f - (double)af * v) / (double)m * (double)dt * (double)dt;
	}

	public bool ShootImp(int bombId, int x, int y, int force, int angle, int bombCount, int shootCount)
	{
		BallInfo ballInfo = BallMgr.FindBall(bombId);
		Tile shape = BallMgr.FindTile(bombId);
		BombType ballType = BallMgr.GetBallType(bombId);
		int num = (int)(m_map.wind * 10f);
		if (ballInfo != null)
		{
			GSPacketIn gSPacketIn = new GSPacketIn(91, base.Id);
			gSPacketIn.Parameter1 = base.Id;
			gSPacketIn.WriteByte(2);
			gSPacketIn.WriteInt(num);
			gSPacketIn.WriteBoolean(num > 0);
			gSPacketIn.WriteByte(m_game.GetVane(num, 1));
			gSPacketIn.WriteByte(m_game.GetVane(num, 2));
			gSPacketIn.WriteByte(m_game.GetVane(num, 3));
			gSPacketIn.WriteInt(bombCount);
			float num2 = 0f;
			SimpleBomb simpleBomb = null;
			for (int i = 0; i < bombCount; i++)
			{
				double num3 = 1.0;
				int num4 = 0;
				switch (i)
				{
				case 1:
					num3 = 0.9;
					num4 = -5;
					break;
				case 2:
					num3 = 1.1;
					num4 = 5;
					break;
				}
				int num5 = (int)((double)force * num3 * Math.Cos((double)(angle + num4) / 180.0 * Math.PI));
				int num6 = (int)((double)force * num3 * Math.Sin((double)(angle + num4) / 180.0 * Math.PI));
				SimpleBomb simpleBomb2 = new SimpleBomb(m_game.PhysicalId++, ballType, this, m_game, ballInfo, shape, ControlBall);
				simpleBomb2.SetXY(x, y);
				simpleBomb2.setSpeedXY(num5, num6);
				m_map.AddPhysical(simpleBomb2);
				simpleBomb2.StartMoving();
				if (i == 0)
				{
					simpleBomb = simpleBomb2;
				}
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteBoolean(simpleBomb2.DigMap);
				gSPacketIn.WriteInt(simpleBomb2.Id);
				gSPacketIn.WriteInt(x);
				gSPacketIn.WriteInt(y);
				gSPacketIn.WriteInt(num5);
				gSPacketIn.WriteInt(num6);
				gSPacketIn.WriteInt(simpleBomb2.BallInfo.ID);
				if (FlyingPartical != 0)
				{
					gSPacketIn.WriteString(FlyingPartical.ToString());
				}
				else
				{
					gSPacketIn.WriteString(ballInfo.FlyingPartical);
				}
				gSPacketIn.WriteInt(simpleBomb2.BallInfo.Radii * 1000 / 4);
				gSPacketIn.WriteInt((int)simpleBomb2.BallInfo.Power * 1000);
				gSPacketIn.WriteInt(simpleBomb2.Actions.Count);
				foreach (BombAction action in simpleBomb2.Actions)
				{
					gSPacketIn.WriteInt(action.TimeInt);
					gSPacketIn.WriteInt(action.Type);
					gSPacketIn.WriteInt(action.Param1);
					gSPacketIn.WriteInt(action.Param2);
					gSPacketIn.WriteInt(action.Param3);
					gSPacketIn.WriteInt(action.Param4);
				}
				num2 = Math.Max(num2, simpleBomb2.LifeTime);
			}
			int num7 = 2000;
			if (m_game.RoomType != eRoomType.FightFootballTime)
			{
				int count = simpleBomb.PetActions.Count;
				if (count > 0 && PetEffects.PetBaseAtt > 0)
				{
					if (simpleBomb.PetActions[0].Type == -1)
					{
						gSPacketIn.WriteInt(0);
					}
					else
					{
						gSPacketIn.WriteInt(count);
						foreach (BombAction petAction in simpleBomb.PetActions)
						{
							gSPacketIn.WriteInt(petAction.Param1);
							gSPacketIn.WriteInt(petAction.Param2);
							gSPacketIn.WriteInt(petAction.Param4);
							gSPacketIn.WriteInt(petAction.Param3);
						}
					}
					gSPacketIn.WriteInt(1);
				}
				else
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(0);
				}
			}
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteInt(0);
			gSPacketIn.WriteBoolean(val: false);
			if (m_game.RoomType == eRoomType.FightFootballTime)
			{
				gSPacketIn.WriteInt(m_game.redScore);
				gSPacketIn.WriteInt(m_game.blueScore);
				gSPacketIn.WriteInt((this as Player).PlayerDetail.PlayerCharacter.ID);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(ScoreArr.Count);
				for (int j = 0; j < ScoreArr.Count; j++)
				{
					gSPacketIn.WriteInt(0);
					gSPacketIn.WriteInt(ScoreArr[j]);
				}
			}
			else
			{
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
				gSPacketIn.WriteInt(0);
			}
			m_game.SendToAll(gSPacketIn);
			if (m_game.RoomType == eRoomType.ActivityDungeon)
			{
				num7 = 0;
			}
			m_game.WaitTime((int)((num2 + 2f + (float)(bombCount / 3)) * 1000f) + num7 + PetEffects.PetDelay + SpecialSkillDelay);
			return true;
		}
		return false;
	}

	public void PlayMovie(string action, int delay, int MovieTime)
	{
		m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
	}

	public void PlayMovie(string action, int delay, int MovieTime, LivingCallBack call)
	{
		m_game.AddAction(new LivingPlayeMovieAction(this, action, delay, MovieTime));
	}

	public void SetNiutou(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 33, state);
		}
	}

	public void SetIndian(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 34, state);
		}
	}

	public void SetTargeting(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendPlayerPicture(this, 7, state);
		}
	}

	public void AddRemoveEnergy(int value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "energy", value.ToString());
		}
	}

	public void NoFly(bool value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "nofly", value.ToString());
		}
	}

	public void SetSeal(bool state)
	{
		if (m_isSeal != state)
		{
			m_isSeal = state;
			if (m_syncAtTime)
			{
				m_game.SendGamePlayerProperty(this, "silenceMany", state.ToString());
			}
		}
	}

	public void SetHidden(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "visible", state.ToString());
		}
	}

	public void SpeedMultX(int value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "speedX", value.ToString());
		}
	}

	public void SpeedMultY(int value)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "speedY", value.ToString());
		}
	}

	public void OnSmallMap(bool state)
	{
		if (m_syncAtTime)
		{
			m_game.SendGamePlayerProperty(this, "onSmallMap", state.ToString());
		}
	}

	public bool GetSealState()
	{
		return m_isSeal;
	}

	public void Seal(Player player, int type, int delay)
	{
		m_game.AddAction(new LivingSealAction(this, player, type, delay));
	}

	public virtual int AddBlood(int value)
	{
		return AddBlood(value, 0);
	}

	public virtual int AddBlood(int value, int type)
	{
		m_blood += value;
		if (m_blood > m_maxBlood)
		{
			m_blood = m_maxBlood;
		}
		if (m_syncAtTime)
		{
			m_game.SendGameUpdateHealth(this, type, value);
		}
		return value;
	}

	public virtual bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		if (Config.IsHelper && (this is SimpleNpc || this is SimpleBoss) && source is Player)
		{
			return false;
		}
		bool result = false;
		if (!IsFrost && m_blood > 0)
		{
			if (source != this || source.Team == Team)
			{
				OnBeforeTakedDamage(source, ref damageAmount, ref criticalAmount);
				StartAttacked();
			}
			int num = damageAmount + criticalAmount;
			m_blood -= num;
			if (m_syncAtTime)
			{
				if (this is SimpleBoss && ((SimpleBoss)this).NpcInfo.ID == 0)
				{
					m_game.SendGameUpdateHealth(this, 6, damageAmount + criticalAmount);
				}
				else
				{
					m_game.SendGameUpdateHealth(this, 1, damageAmount + criticalAmount);
				}
			}
			OnAfterTakedDamage(source, damageAmount, criticalAmount);
			if (m_blood <= 0 && m_game.RoomType != eRoomType.FightFootballTime)
			{
				if (criticalAmount > 0 && this is Player)
				{
					m_game.AddAction(new FightAchievementAction(source, 7, source.Direction, 1200));
				}
				Die();
			}
			source.OnAfterKillingLiving(this, damageAmount, criticalAmount);
			result = true;
		}
		EffectList.StopEffect(typeof(IceFronzeEffect));
		EffectList.StopEffect(typeof(HideEffect));
		EffectList.StopEffect(typeof(NoHoleEffect));
		return result;
	}

	public void SetIceFronze(Living living)
	{
		new IceFronzeEffect(2).Start(this);
		BeginNextTurn -= SetIceFronze;
	}

	public virtual bool PetTakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		if (Config.IsHelper && (this is SimpleNpc || this is SimpleBoss))
		{
			return false;
		}
		bool result = false;
		if (m_blood > 0)
		{
			m_blood -= damageAmount + criticalAmount;
			if (m_blood <= 0)
			{
				Die();
			}
			result = true;
		}
		return result;
	}

	public virtual void Die(int delay)
	{
		if (base.IsLiving && m_game != null)
		{
			m_game.AddAction(new LivingDieAction(this, delay));
		}
	}

	public override void Die()
	{
		if (m_blood > 0)
		{
			m_blood = 0;
			m_doAction = -1;
			if (m_syncAtTime)
			{
				m_game.SendGameUpdateHealth(this, 6, 0);
			}
		}
		if (base.IsLiving)
		{
			if (IsAttacking)
			{
				StopAttacking();
			}
			base.Die();
			OnDied();
			m_game.CheckState(0);
		}
	}

	protected void OnDied()
	{
		if (Died != null)
		{
			Died(this);
		}
		if (this is Player && Game is PVEGame)
		{
			((PVEGame)Game).DoOther();
		}
	}

	protected void OnBeforeTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (BeforeTakeDamage != null)
		{
			BeforeTakeDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	public void OnTakedDamage(Living source, ref int damageAmount, ref int criticalAmount)
	{
		if (TakePlayerDamage != null)
		{
			TakePlayerDamage(this, source, ref damageAmount, ref criticalAmount);
		}
	}

	protected void OnBeginNewTurn()
	{
		if (BeginNextTurn != null)
		{
			BeginNextTurn(this);
		}
	}

	protected void OnBeginSelfTurn()
	{
		if (BeginSelfTurn != null)
		{
			BeginSelfTurn(this);
		}
	}

	protected void OnStartAttacked()
	{
		if (BeginAttacked != null)
		{
			BeginAttacked(this);
		}
	}

	protected void OnStartAttacking()
	{
		if (BeginAttacking != null)
		{
			BeginAttacking(this);
		}
	}

	protected void OnStopAttacking()
	{
		if (EndAttacking != null)
		{
			EndAttacking(this);
		}
	}

	public virtual void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
	{
		if (target.Team != Team)
		{
			CurrentIsHitTarget = true;
			TotalHurt += damageAmount + criticalAmount;
			if (!target.IsLiving)
			{
				TotalKill++;
			}
			m_game.CurrentTurnTotalDamage = damageAmount + criticalAmount;
			m_game.TotalHurt += damageAmount + criticalAmount;
		}
		if (AfterKillingLiving != null)
		{
			AfterKillingLiving(this, target, damageAmount, criticalAmount);
		}
	}

	public void OnAfterTakedDamage(Living target, int damageAmount, int criticalAmount)
	{
		if (AfterKilledByLiving != null)
		{
			AfterKilledByLiving(this, target, damageAmount, criticalAmount);
		}
	}

	public void CallFuction(LivingCallBack func, int delay)
	{
		if (m_game != null)
		{
			m_game.AddAction(new LivingCallFunctionAction(this, func, delay));
		}
	}
}
