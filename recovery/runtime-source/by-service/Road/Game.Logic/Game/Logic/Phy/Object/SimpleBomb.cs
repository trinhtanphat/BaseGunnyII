using System;
using System.Collections.Generic;
using System.Drawing;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.Phy.Actions;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Maths;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class SimpleBomb : BombObject
{
	private Living m_owner;

	private BaseGame m_game;

	protected Tile m_shape;

	protected int m_radius;

	protected int m_petRadius;

	protected double m_power;

	protected List<BombAction> m_actions;

	protected List<BombAction> m_petActions;

	protected BombType m_type;

	protected bool m_controled;

	private float m_lifeTime;

	private BallInfo m_info;

	private bool m_bombed;

	private bool digMap;

	public bool DigMap => digMap;

	public BallInfo BallInfo => m_info;

	public Living Owner => m_owner;

	public List<BombAction> PetActions => m_petActions;

	public List<BombAction> Actions => m_actions;

	public float LifeTime => m_lifeTime;

	public SimpleBomb(int id, BombType type, Living owner, BaseGame game, BallInfo info, Tile shape, bool controled)
		: base(id, info.Mass, info.Weight, info.Wind, info.DragIndex)
	{
		m_owner = owner;
		m_game = game;
		m_info = info;
		m_shape = shape;
		m_type = type;
		m_power = info.Power;
		m_radius = info.Radii;
		m_controled = controled;
		m_bombed = false;
		m_petRadius = 80;
		m_lifeTime = 0f;
		if (m_info.IsSpecial())
		{
			digMap = false;
		}
		else
		{
			digMap = true;
		}
	}

	public override void StartMoving()
	{
		base.StartMoving();
		m_actions = new List<BombAction>();
		m_petActions = new List<BombAction>();
		int lifeTime = m_game.LifeTime;
		while (m_isMoving && m_isLiving)
		{
			m_lifeTime += 0.04f;
			Point point = CompleteNextMovePoint(0.04f);
			MoveTo(point.X, point.Y);
			if (m_isLiving)
			{
				if (Math.Round(m_lifeTime * 100f) % 40.0 == 0.0 && point.Y > 0)
				{
					m_game.AddTempPoint(point.X, point.Y);
				}
				if (m_controled && base.vY > 0f)
				{
					Living living = m_map.FindNearestEnemy(m_x, m_y, 150.0, m_owner);
					if (living != null)
					{
						Point point2;
						if (living is SimpleBoss)
						{
							Rectangle directDemageRect = living.GetDirectDemageRect();
							point2 = new Point(directDemageRect.X - m_x, directDemageRect.Y - m_y);
						}
						else
						{
							point2 = new Point(living.X - m_x, living.Y - m_y);
						}
						point2 = point2.Normalize(1000);
						setSpeedXY(point2.X, point2.Y);
						UpdateForceFactor(0f, 0f, 0f);
						m_controled = false;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_SPEED, point2.X, point2.Y, 0, 0));
					}
				}
			}
			if (m_bombed)
			{
				m_bombed = false;
				BombImp();
			}
		}
	}

	protected override void CollideObjects(Physics[] list)
	{
		foreach (Physics physics in list)
		{
			physics.CollidedByObject(this);
			m_actions.Add(new BombAction(m_lifeTime, ActionType.PICK, physics.Id, 0, 0, 0));
		}
	}

	protected override void CollideGround()
	{
		base.CollideGround();
		Bomb();
	}

	public void Bomb()
	{
		StopMoving();
		m_isLiving = false;
		m_bombed = true;
		m_owner.Game.SendFightStatus(m_owner, 0);
	}

	private void BombImp()
	{
		Point collidePoint = GetCollidePoint();
		List<Living> list = m_map.FindHitByHitPiont(collidePoint, m_radius);
		foreach (Living item in list)
		{
			if (item.IsNoHole || item.NoHoleTurn)
			{
				item.NoHoleTurn = true;
				if (!m_info.IsSpecial())
				{
					digMap = false;
				}
			}
			item.SyncAtTime = false;
		}
		m_owner.SyncAtTime = false;
		try
		{
			if (digMap)
			{
				m_map.Dig(m_x, m_y, m_shape, null);
			}
			m_actions.Add(new BombAction(m_lifeTime, ActionType.BOMB, m_x, m_y, digMap ? 1 : 0, 0));
			switch (m_type)
			{
			case BombType.FORZEN:
				foreach (Living item2 in list)
				{
					if (m_owner is SimpleBoss && new IceFronzeEffect(100).Start(item2))
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, item2.Id, 0, 0, 0));
						continue;
					}
					if (new IceFronzeEffect(2).Start(item2))
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, item2.Id, 0, 0, 0));
						continue;
					}
					m_actions.Add(new BombAction(m_lifeTime, ActionType.FORZEN, -1, 0, 0, 0));
					m_actions.Add(new BombAction(m_lifeTime, ActionType.CHANGE_STATE, item2.Id, 0, 0, 0));
				}
				break;
			case BombType.CURE:
				foreach (Living item3 in list)
				{
					double num4 = ((!m_map.FindPlayers(GetCollidePoint(), m_radius)) ? 1.0 : 0.4);
					int num5;
					if (m_info.ID == 10009)
					{
						num5 = (int)(m_lifeTime * 2000f);
					}
					else
					{
						num5 = (int)((double)((Player)m_owner).PlayerDetail.SecondWeapon.Template.Property7 * Math.Pow(1.1, ((Player)m_owner).PlayerDetail.SecondWeapon.StrengthenLevel) * num4);
						if (m_owner.FightBuffers.ConsortionAddBloodGunCount > 0)
						{
							num5 += m_owner.FightBuffers.ConsortionAddBloodGunCount;
						}
					}
					if (item3 is Player)
					{
						item3.AddBlood(num5);
						((Player)item3).TotalCure += num5;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, item3.Id, item3.Blood, num5, 0));
					}
					if ((item3 is SimpleBoss || item3 is SimpleNpc) && item3.Config.IsHelper)
					{
						item3.AddBlood(num5);
						((SimpleBoss)item3).TotalCure += num5;
						m_actions.Add(new BombAction(m_lifeTime, ActionType.CURE, item3.Id, item3.Blood, num5, 0));
					}
				}
				break;
			case BombType.FLY:
				if (m_y > 10 && m_lifeTime > 0.04f)
				{
					if (!m_map.IsEmpty(m_x, m_y))
					{
						PointF point = new PointF(0f - base.vX, 0f - base.vY);
						point = point.Normalize(5f);
						m_x -= (int)point.X;
						m_y -= (int)point.Y;
					}
					m_owner.SetXY(m_x, m_y);
					m_owner.StartMoving();
					m_actions.Add(new BombAction(m_lifeTime, ActionType.TRANSLATE, m_x, m_y, 0, 0));
					m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, m_owner.Id, m_owner.X, m_owner.Y, m_owner.IsLiving ? 1 : 0));
				}
				break;
			default:
			{
				int num = 0;
				int num2 = 0;
				foreach (Living item4 in list)
				{
					if (m_owner.IsFriendly(item4))
					{
						continue;
					}
					num = MakeDamage(item4);
					num2 = 0;
					if (num != 0)
					{
						if (m_game.RoomType == eRoomType.FightFootballTime && item4 is SimpleNpc)
						{
							num = GetFightFootballPoint((item4 as SimpleNpc).NpcInfo.ID);
							if (m_owner.Team == 1)
							{
								m_game.blueScore += num;
							}
							else
							{
								m_game.redScore += num;
							}
							m_owner.ScoreArr.Add(num);
						}
						else
						{
							num2 = MakeCriticalDamage(item4, num);
						}
						m_owner.OnTakedDamage(m_owner, ref num, ref num2);
						if (item4.TakeDamage(m_owner, ref num, ref num2, "Fire"))
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.KILL_PLAYER, item4.Id, num + num2, (num2 == 0) ? 1 : 2, item4.Blood));
						}
						else
						{
							m_actions.Add(new BombAction(m_lifeTime, ActionType.UNFORZEN, item4.Id, 0, 0, 0));
						}
						if (m_owner is Player && item4 is SimpleBoss)
						{
							m_owner.TotalDameLiving += num2 + num;
						}
						if (item4 is Player)
						{
							int num3 = ((Player)item4).Dander;
							if (m_owner.FightBuffers.ConsortionReduceDander > 0)
							{
								num3 -= num3 * m_owner.FightBuffers.ConsortionReduceDander / 100;
								((Player)item4).Dander = num3;
							}
							m_actions.Add(new BombAction(m_lifeTime, ActionType.DANDER, item4.Id, num3, 0, 0));
						}
						if ((item4 is SimpleBoss || item4 is SimpleNpc) && m_game.RoomType != eRoomType.FightFootballTime)
						{
							((PVEGame)m_game).OnShooted();
							if (item4.DoAction > -1)
							{
								m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item4.Id, 0, 0, item4.DoAction));
							}
						}
					}
					else if (item4 is SimpleBoss || item4 is SimpleNpc)
					{
						m_actions.Add(new BombAction(m_lifeTime, ActionType.DO_ACTION, item4.Id, 0, 0, 0));
					}
					m_owner.OnAfterKillingLiving(item4, num, num2);
					if (item4.IsLiving)
					{
						item4.StartMoving((int)((m_lifeTime + 1f) * 1000f), 12);
						m_actions.Add(new BombAction(m_lifeTime, ActionType.START_MOVE, item4.Id, item4.X, item4.Y, item4.IsLiving ? 1 : 0));
					}
				}
				List<Living> list2 = m_map.FindHitByHitPiont(collidePoint, m_petRadius);
				if (!m_owner.isPet || !m_owner.PetEffects.ActivePetHit)
				{
					break;
				}
				foreach (Living item5 in list2)
				{
					if (item5 == m_owner)
					{
						continue;
					}
					num = MakePetDamage(item5, collidePoint);
					if (num <= 0)
					{
						continue;
					}
					num = num * m_owner.PetEffects.PetBaseAtt / 100;
					num2 = MakeCriticalDamage(item5, num);
					if (item5.PetTakeDamage(m_owner, ref num, ref num2, "PetFire"))
					{
						if (item5 is Player)
						{
							m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, item5.Id, num + num2, ((Player)item5).Dander, item5.Blood));
						}
						else
						{
							m_petActions.Add(new BombAction(m_lifeTime, ActionType.PET, item5.Id, num + num2, 0, item5.Blood));
						}
					}
				}
				if (list2.Count == 0)
				{
					m_petActions.Add(new BombAction(0f, ActionType.NULLSHOOT, 0, 0, 0, 0));
				}
				m_owner.PetEffects.ActivePetHit = false;
				break;
			}
			}
			Die();
		}
		finally
		{
			m_owner.SyncAtTime = true;
			foreach (Living item6 in list)
			{
				item6.SyncAtTime = true;
			}
		}
	}

	protected int MakeDamage(Living target)
	{
		if (target.Config.IsChristmasBoss)
		{
			return 1;
		}
		if (m_game.RoomType == eRoomType.FightFootballTime && target is Player)
		{
			return 1;
		}
		double baseDamage = m_owner.BaseDamage;
		double num = target.BaseGuard;
		double num2 = target.Defence;
		double attack = m_owner.Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)target.getHertAddition((target as Player).DeputyWeapon);
			num += (double)num3;
			num2 += (double)num3;
		}
		if (m_owner.IgnoreArmor)
		{
			num = 0.0;
			num2 = 0.0;
		}
		float currentDamagePlus = m_owner.CurrentDamagePlus;
		float currentShootMinus = m_owner.CurrentShootMinus;
		double num4 = 0.95 * (num - (double)(3 * m_owner.Grade)) / (500.0 + num - (double)(3 * m_owner.Grade));
		double num5 = ((!(num2 - m_owner.Lucky < 0.0)) ? (0.95 * (num2 - m_owner.Lucky) / (600.0 + num2 - m_owner.Lucky)) : 0.0);
		double num6 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5)) * (double)currentDamagePlus * (double)currentShootMinus;
		Point p = new Point(X, Y);
		double num7 = target.Distance(p);
		if (num7 >= (double)m_radius)
		{
			return 0;
		}
		if (m_owner is Player && target is Player && target != m_owner)
		{
			m_game.AddAction(new FightAchievementAction(m_owner, 3, m_owner.Direction, 1200));
		}
		num6 = num6 * (1.0 - num7 / (double)m_radius / 4.0) + (double)m_owner.FightBuffers.WorldBossAddDamage;
		if (num6 < 0.0)
		{
			return 1;
		}
		return (int)num6;
	}

	protected int MakePetDamage(Living target, Point p)
	{
		if (target.Config.IsWorldBoss || m_owner.Game.RoomType == eRoomType.ActivityDungeon)
		{
			return 0;
		}
		if (target.Config.IsChristmasBoss)
		{
			return 1;
		}
		double baseDamage = m_owner.BaseDamage;
		double num = target.BaseGuard;
		double num2 = target.Defence;
		double attack = m_owner.Attack;
		if (target.AddArmor && (target as Player).DeputyWeapon != null)
		{
			int num3 = (int)target.getHertAddition((target as Player).DeputyWeapon);
			num += (double)num3;
			num2 += (double)num3;
		}
		if (m_owner.IgnoreArmor)
		{
			num = 0.0;
			num2 = 0.0;
		}
		float currentDamagePlus = m_owner.CurrentDamagePlus;
		double num4 = 0.95 * (num - (double)(3 * m_owner.Grade)) / (500.0 + num - (double)(3 * m_owner.Grade));
		double num5 = ((!(num2 - m_owner.Lucky < 0.0)) ? (0.95 * (num2 - m_owner.Lucky) / (600.0 + num2 - m_owner.Lucky)) : 0.0);
		double num6 = baseDamage * (1.0 + attack * 0.001) * (1.0 - (num4 + num5 - num4 * num5)) * (double)currentDamagePlus;
		if (num6 < 0.0)
		{
			return 1;
		}
		return (int)num6;
	}

	protected int MakeCriticalDamage(Living target, int baseDamage)
	{
		double lucky = m_owner.Lucky;
		bool flag = lucky * 45.0 / (800.0 + lucky) > (double)m_game.Random.Next(100);
		if (m_owner.PetEffects.CritActive)
		{
			flag = true;
			m_owner.PetEffects.CritActive = false;
		}
		if (flag)
		{
			int num = target.ReduceCritFisrtGem + target.ReduceCritSecondGem;
			int num2 = (int)((0.5 + lucky * 0.00015) * (double)baseDamage);
			num2 = num2 * (100 - num) / 100;
			if (m_owner.FightBuffers.ConsortionAddCritical > 0)
			{
				num2 += m_owner.FightBuffers.ConsortionAddCritical;
			}
			return num2;
		}
		return 0;
	}

	protected int GetFightFootballPoint(int livingID)
	{
		return livingID switch
		{
			10005 => 1,
			10006 => 2,
			10007 => 3,
			10008 => 4,
			10009 => 5,
			_ => 1,
		};
	}

	protected override void FlyoutMap()
	{
		m_actions.Add(new BombAction(m_lifeTime, ActionType.FLY_OUT, 0, 0, 0, 0));
		base.FlyoutMap();
	}
}
