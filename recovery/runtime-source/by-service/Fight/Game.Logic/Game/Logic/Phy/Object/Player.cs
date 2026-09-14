using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using Bussiness.Managers;
using Game.Logic.Actions;
using Game.Logic.Effects;
using Game.Logic.PetEffects;
using Game.Logic.Phy.Maths;
using Game.Logic.Spells;
using SqlDataProvider.Data;

namespace Game.Logic.Phy.Object;

public class Player : TurnedLiving
{
	private IGamePlayer m_player;

	private UsersPetinfo m_pet;

	private ItemTemplateInfo m_weapon;

	private ItemInfo m_DeputyWeapon;

	private ItemInfo m_Healstone;

	private int m_mainBallId;

	private int m_spBallId;

	private int m_AddWoundBallId;

	private int m_MultiBallId;

	private BallInfo m_currentBall;

	private int m_energy;

	private bool m_isActive;

	private int m_prop;

	public Point TargetPoint;

	public int GainGP;

	public int GainOffer;

	public bool LockDirection;

	public int TotalCure;

	private bool m_canGetProp;

	public int TotalAllHurt;

	public int TotalAllHitTargetCount;

	public int TotalAllShootCount;

	public int TotalAllKill;

	public int TotalAllExperience;

	public int TotalAllScore;

	public int TotalAllCure;

	public int CanTakeOut;

	public bool FinishTakeCard;

	public bool HasPaymentTakeCard;

	public int BossCardCount;

	public bool Ready;

	private int m_loadingProcess;

	private int m_shootCount;

	private int m_ballCount;

	private int m_changeSpecialball;

	private ArrayList m_tempBoxes = new ArrayList();

	private int deputyWeaponResCount;

	private static readonly int CARRY_TEMPLATE_ID = 10016;

	private int m_flyCoolDown = 2;

	public IGamePlayer PlayerDetail => m_player;

	public UsersPetinfo Pet => m_pet;

	public ItemTemplateInfo Weapon => m_weapon;

	public ItemInfo DeputyWeapon => m_DeputyWeapon;

	public bool IsActive => m_isActive;

	public int Prop
	{
		get
		{
			return m_prop;
		}
		set
		{
			m_prop = value;
		}
	}

	public bool CanGetProp
	{
		get
		{
			return m_canGetProp;
		}
		set
		{
			if (m_canGetProp != value)
			{
				m_canGetProp = value;
			}
		}
	}

	public int LoadingProcess
	{
		get
		{
			return m_loadingProcess;
		}
		set
		{
			if (m_loadingProcess != value)
			{
				m_loadingProcess = value;
				if (m_loadingProcess >= 100)
				{
					OnLoadingCompleted();
				}
			}
		}
	}

	public int Energy
	{
		get
		{
			return m_energy;
		}
		set
		{
			m_energy = value;
		}
	}

	public BallInfo CurrentBall => m_currentBall;

	public bool IsSpecialSkill => m_currentBall.ID == m_spBallId;

	public int ChangeSpecialBall
	{
		get
		{
			return m_changeSpecialball;
		}
		set
		{
			m_changeSpecialball = value;
		}
	}

	public int ShootCount
	{
		get
		{
			return m_shootCount;
		}
		set
		{
			if (m_shootCount != value)
			{
				m_shootCount = value;
				m_game.SendGameUpdateShootCount(this);
			}
		}
	}

	public int BallCount
	{
		get
		{
			return m_ballCount;
		}
		set
		{
			if (m_ballCount != value)
			{
				m_ballCount = value;
			}
		}
	}

	public int deputyWeaponCount => deputyWeaponResCount;

	public int flyCount => m_flyCoolDown;

	public event PlayerEventHandle PlayerBeginMoving;

	public event PlayerEventHandle AfterPlayerShooted;

	public event PlayerEventHandle BeforePlayerShoot;

	public event PlayerEventHandle LoadingCompleted;

	public event PlayerEventHandle PlayerShoot;

	public event PlayerEventHandle PlayerBuffSkillPet;

	public event PlayerEventHandle CollidByObject;

	public Player(IGamePlayer player, int id, BaseGame game, int team, int maxBlood)
		: base(id, game, team, "", "", maxBlood, 0, 1)
	{
		m_rect = new Rectangle(-15, -20, 30, 30);
		m_player = player;
		m_player.GamePlayerId = id;
		m_isActive = true;
		m_canGetProp = true;
		Grade = player.PlayerCharacter.Grade;
		if (base.AutoBoot)
		{
			base.VaneOpen = true;
		}
		else
		{
			base.VaneOpen = player.PlayerCharacter.IsWeakGuildFinish(9);
		}
		m_pet = player.Pet;
		if (m_pet != null && game != null && game.RoomType != eRoomType.FightFootballTime)
		{
			base.isPet = true;
			base.PetEffects.PetBaseAtt = GetPetBaseAtt();
			InitPetSkillEffect(m_pet);
		}
		InitFightBuffer(player.FightBuffs);
		TotalAllHurt = 0;
		TotalAllHitTargetCount = 0;
		TotalAllShootCount = 0;
		TotalAllKill = 0;
		TotalAllExperience = 0;
		TotalAllScore = 0;
		TotalAllCure = 0;
		m_DeputyWeapon = m_player.SecondWeapon;
		m_Healstone = m_player.Healstone;
		ChangeSpecialBall = 0;
		if (m_DeputyWeapon != null)
		{
			deputyWeaponResCount = m_DeputyWeapon.StrengthenLevel + 1;
		}
		else
		{
			deputyWeaponResCount = 1;
		}
		m_weapon = m_player.MainWeapon;
		if (m_weapon != null)
		{
			BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(m_weapon.TemplateID);
			m_mainBallId = ballConfigInfo.Common;
			m_spBallId = ballConfigInfo.Special;
			m_AddWoundBallId = ballConfigInfo.CommonAddWound;
			m_MultiBallId = ballConfigInfo.CommonMultiBall;
		}
		m_loadingProcess = 0;
		m_prop = 0;
		InitEqupedEffect(m_player.EquipEffect);
		m_energy = (m_player.PlayerCharacter.AgiAddPlus + m_player.PlayerCharacter.Agility) / 30 + 240;
		m_maxBlood = m_player.PlayerCharacter.hp;
		if (base.FightBuffers.ConsortionAddMaxBlood > 0)
		{
			m_maxBlood += m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood / 100;
		}
		m_maxBlood += m_player.PlayerCharacter.HpAddPlus + base.FightBuffers.WorldBossHP + base.FightBuffers.WorldBossHP_MoneyBuff;
	}

	public int GetPetBaseAtt()
	{
		try
		{
			string[] array = m_pet.SkillEquip.Split('|');
			for (int i = 0; i < array.Length; i++)
			{
				int skillID = Convert.ToInt32(array[i].Split(',')[0]);
				PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(skillID);
				if (petSkillInfo != null && petSkillInfo.Damage > 0)
				{
					return petSkillInfo.Damage;
				}
			}
		}
		catch (Exception ex)
		{
			Console.WriteLine("______________GetPetBaseAtt ERROR______________");
			Console.WriteLine(ex.Message);
			Console.WriteLine(ex.StackTrace);
			Console.WriteLine("_______________________________________________");
			return 0;
		}
		return 0;
	}

	public override void Reset()
	{
		if (m_game.RoomType == eRoomType.Dungeon || m_game.RoomType == eRoomType.SpecialActivityDungeon)
		{
			m_game.Cards = new int[21];
		}
		else
		{
			m_game.Cards = new int[9];
		}
		base.Dander = 0;
		base.PetMP = 10;
		base.psychic = 40;
		base.IsLiving = true;
		FinishTakeCard = false;
		m_Healstone = m_player.Healstone;
		m_changeSpecialball = 0;
		m_DeputyWeapon = m_player.SecondWeapon;
		m_weapon = m_player.MainWeapon;
		BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(m_weapon.TemplateID);
		m_mainBallId = ballConfigInfo.Common;
		m_spBallId = ballConfigInfo.Special;
		m_AddWoundBallId = ballConfigInfo.CommonAddWound;
		m_MultiBallId = ballConfigInfo.CommonMultiBall;
		BaseDamage = m_player.GetBaseAttack();
		BaseGuard = m_player.GetBaseDefence();
		Attack = m_player.PlayerCharacter.Attack;
		Defence = m_player.PlayerCharacter.Defence;
		Agility = m_player.PlayerCharacter.Agility;
		Lucky = m_player.PlayerCharacter.Luck;
		m_maxBlood = m_player.PlayerCharacter.hp;
		BaseDamage += m_player.PlayerCharacter.DameAddPlus + base.FightBuffers.WorldBossAttrack_MoneyBuff;
		if (base.FightBuffers.ConsortionAddDamage > 0)
		{
			BaseDamage += base.FightBuffers.ConsortionAddDamage;
		}
		BaseGuard += m_player.PlayerCharacter.GuardAddPlus;
		Attack += m_player.PlayerCharacter.AttackAddPlus;
		Defence += m_player.PlayerCharacter.DefendAddPlus;
		Agility += m_player.PlayerCharacter.AgiAddPlus;
		Lucky += m_player.PlayerCharacter.LuckAddPlus;
		m_maxBlood = m_player.PlayerCharacter.hp;
		if (base.FightBuffers.ConsortionAddMaxBlood > 0)
		{
			m_maxBlood += m_maxBlood * base.FightBuffers.ConsortionAddMaxBlood / 100;
		}
		m_maxBlood += m_player.PlayerCharacter.HpAddPlus + base.FightBuffers.WorldBossHP + base.FightBuffers.WorldBossHP_MoneyBuff;
		if (base.FightBuffers.ConsortionAddProperty > 0)
		{
			Attack += base.FightBuffers.ConsortionAddProperty;
			Defence += base.FightBuffers.ConsortionAddProperty;
			Agility += base.FightBuffers.ConsortionAddProperty;
			Lucky += base.FightBuffers.ConsortionAddProperty;
		}
		m_energy = (int)Agility / 30 + 240;
		if (base.FightBuffers.ConsortionAddEnergy > 0)
		{
			m_energy += base.FightBuffers.ConsortionAddEnergy;
		}
		m_currentBall = BallMgr.FindBall(m_mainBallId);
		m_shootCount = 1;
		m_ballCount = 1;
		m_prop = 0;
		CurrentIsHitTarget = false;
		TotalCure = 0;
		TotalHitTargetCount = 0;
		TotalHurt = 0;
		TotalKill = 0;
		TotalShootCount = 0;
		LockDirection = false;
		GainGP = 0;
		GainOffer = 0;
		Ready = false;
		PlayerDetail.ClearTempBag();
		m_delay = GetTurnDelay();
		LoadingProcess = 0;
		base.Reset();
	}

	public void InitFightBuffer(List<BufferInfo> buffers)
	{
		foreach (BufferInfo buffer in buffers)
		{
			switch (buffer.Type)
			{
			case 101:
				base.FightBuffers.ConsortionAddBloodGunCount = buffer.Value;
				break;
			case 102:
				base.FightBuffers.ConsortionAddDamage = buffer.Value;
				break;
			case 103:
				base.FightBuffers.ConsortionAddCritical = buffer.Value;
				break;
			case 104:
				base.FightBuffers.ConsortionAddMaxBlood = buffer.Value;
				break;
			case 105:
				base.FightBuffers.ConsortionAddProperty = buffer.Value;
				break;
			case 106:
				base.FightBuffers.ConsortionReduceEnergyUse = buffer.Value;
				break;
			case 107:
				base.FightBuffers.ConsortionAddEnergy = buffer.Value;
				break;
			case 108:
				base.FightBuffers.ConsortionAddEffectTurn = buffer.Value;
				break;
			case 109:
				base.FightBuffers.ConsortionAddOfferRate = buffer.Value;
				break;
			case 110:
				base.FightBuffers.ConsortionAddPercentGoldOrGP = buffer.Value;
				break;
			case 111:
				base.FightBuffers.ConsortionAddSpellCount = buffer.Value;
				break;
			case 112:
				base.FightBuffers.ConsortionReduceDander = buffer.Value;
				break;
			case 400:
				base.FightBuffers.WorldBossHP = buffer.Value;
				break;
			case 401:
				base.FightBuffers.WorldBossAttrack = buffer.Value;
				break;
			case 402:
				base.FightBuffers.WorldBossHP_MoneyBuff = buffer.Value;
				break;
			case 403:
				base.FightBuffers.WorldBossAttrack_MoneyBuff = buffer.Value;
				break;
			case 404:
				base.FightBuffers.WorldBossMetalSlug = buffer.Value;
				break;
			case 405:
				base.FightBuffers.WorldBossAncientBlessings = buffer.Value;
				break;
			case 406:
				base.FightBuffers.WorldBossAddDamage = buffer.Value;
				break;
			default:
				Console.WriteLine($"Not Found FightBuff Type {buffer.Type} Value {buffer.Value}");
				break;
			}
		}
	}

	public void InitPetSkillEffect(UsersPetinfo pet)
	{
		string[] array = pet.SkillEquip.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = int.Parse(text.Split(',')[0]);
			PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(num);
			if (petSkillInfo == null)
			{
				break;
			}
			string[] array3 = petSkillInfo.ElementIDs.Split(',');
			int coldDown = petSkillInfo.ColdDown;
			int probability = petSkillInfo.Probability;
			int delay = petSkillInfo.Delay;
			int gameType = petSkillInfo.GameType;
			string[] array4 = array3;
			foreach (string text2 in array4)
			{
				string text3;
				switch (text3 = text2)
				{
				case "1017":
					new PetStopMovingEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1018":
				case "1019":
				case "1020":
					new PetAddDefendEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1021":
					new PetNoHoleEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1132":
					new PetReduceAttackEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1038":
					new PetFatalEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1070":
					new PetRemovePlusDameEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1072":
				case "1073":
					new PetPlusDameEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1049":
				case "1050":
					new PetPlusGuardEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1055":
					new PetRemovePlusGuardEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1106":
					new PetPlusOneMpEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1076":
				case "1077":
					new PetAttackAroundEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1082":
					new PetAlwayNoHoleEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1083":
				case "1084":
					new PetAddAttackEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1085":
				case "1086":
					new PetAddLuckEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1087":
				case "1088":
					new PetReduceDefendEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1089":
					new PetRemoveV3BatteryEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1200":
					new PetPlusAllTwoMpEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				case "1223":
				case "1253":
				case "1263":
					new PetPlusTwoMpEquipEffect(coldDown, probability, gameType, num, delay).Start(this);
					break;
				}
			}
		}
	}

	public void InitEqupedEffect(List<ItemInfo> equpedEffect)
	{
		base.EffectList.StopAllEffect();
		foreach (ItemInfo item in equpedEffect)
		{
			int num = 0;
			int num2 = 0;
			RuneTemplateInfo runeTemplateInfo = RuneMgr.FindRuneByTemplateID(item.TemplateID);
			string[] array = runeTemplateInfo.Attribute1.Split('|');
			string[] array2 = runeTemplateInfo.Attribute2.Split('|');
			if (item.Hole1 > runeTemplateInfo.BaseLevel)
			{
				if (array.Length > 1)
				{
					num = 1;
				}
				if (array2.Length > 1)
				{
					num2 = 1;
				}
			}
			int num3 = runeTemplateInfo.Type1;
			int num4 = Convert.ToInt32(array[num]);
			int probability = runeTemplateInfo.Rate1;
			if (num3 == 39)
			{
				ReduceCritFisrtGem = num4;
				ReduceCritSecondGem = num4;
				int type = runeTemplateInfo.Type2;
				if (DefenFisrtGem == 0)
				{
					DefenFisrtGem = type;
				}
				else
				{
					DefenSecondGem = type;
				}
			}
			if (num3 == 37 || num3 == 39)
			{
				num3 = runeTemplateInfo.Type2;
				num4 = Convert.ToInt32(array2[num2]);
				probability = runeTemplateInfo.Rate2;
			}
			switch (num3)
			{
			case 1:
				new AddAttackEffect(num4, probability).Start(this);
				break;
			case 2:
				new AddDefenceEffect(num4, probability, num3).Start(this);
				break;
			case 3:
				new AddAgilityEffect(num4, probability).Start(this);
				break;
			case 4:
				new AddLuckyEffect(num4, probability).Start(this);
				break;
			case 5:
				new AddDamageEffect(num4, probability).Start(this);
				break;
			case 6:
				new ReduceDamageEffect(num4, probability, num3).Start(this);
				break;
			case 7:
				new AddBloodEffect(num4, probability).Start(this);
				break;
			case 8:
				new FatalEffect(num4, probability).Start(this);
				break;
			case 9:
				new IceFronzeEquipEffect(num4, probability).Start(this);
				break;
			case 10:
				new NoHoleEquipEffect(num4, probability, num3).Start(this);
				break;
			case 11:
				new AtomBombEquipEffect(num4, probability).Start(this);
				break;
			case 12:
				new ArmorPiercerEquipEffect(num4, probability).Start(this);
				break;
			case 13:
				new AvoidDamageEffect(num4, probability, num3).Start(this);
				break;
			case 14:
				new MakeCriticalEffect(num4, probability).Start(this);
				break;
			case 15:
				new AssimilateDamageEffect(num4, probability, num3).Start(this);
				break;
			case 16:
				new AssimilateBloodEffect(num4, probability).Start(this);
				break;
			case 17:
				new SealEquipEffect(num4, probability).Start(this);
				break;
			case 18:
				new AddTurnEquipEffect(num4, probability, runeTemplateInfo.TemplateID).Start(this);
				break;
			case 19:
				new AddDanderEquipEffect(num4, probability, num3).Start(this);
				break;
			case 20:
				new ReflexDamageEquipEffect(num4, probability).Start(this);
				break;
			case 21:
				new ReduceStrengthEquipEffect(num4, probability).Start(this);
				break;
			case 22:
				new ContinueReduceBloodEquipEffect(num4, probability).Start(this);
				break;
			case 23:
				new LockDirectionEquipEffect(num4, probability).Start(this);
				break;
			case 24:
				new AddBombEquipEffect(num4, probability).Start(this);
				break;
			case 25:
				new ContinueReduceDamageEquipEffect(num4, probability).Start(this);
				break;
			case 26:
				new RecoverBloodEffect(num4, probability, num3).Start(this);
				break;
			default:
				Console.WriteLine("Not Found Effect: " + num3);
				break;
			}
		}
	}

	public Point StartFalling(bool direct)
	{
		return StartFalling(direct, 0, Living.MOVE_SPEED * 10);
	}

	public virtual Point StartFalling(bool direct, int delay, int speed)
	{
		Point point = m_map.FindYLineNotEmptyPoint(X, Y);
		if (point == Point.Empty)
		{
			point = new Point(X, m_game.Map.Bound.Height + 1);
		}
		if (point.Y == Y)
		{
			return Point.Empty;
		}
		if (direct)
		{
			SetXY(point);
			if (m_map.IsOutMap(point.X, point.Y))
			{
				base.Die();
				if (base.Game.CurrentLiving != this && base.Game.CurrentLiving is Player && this != null && base.Team != base.Game.CurrentLiving.Team)
				{
					Player player = base.Game.CurrentLiving as Player;
					player.PlayerDetail.OnKillingLiving(m_game, 1, base.Id, base.IsLiving, 0);
					base.Game.CurrentLiving.TotalKill++;
				}
			}
		}
		else
		{
			m_game.AddAction(new LivingFallingAction(this, point.X, point.Y, speed, null, delay, 0, null));
		}
		return point;
	}

	public bool ReduceEnergy(int value)
	{
		if (value > m_energy)
		{
			return false;
		}
		m_energy -= value;
		return true;
	}

	public override bool TakeDamage(Living source, ref int damageAmount, ref int criticalAmount, string msg)
	{
		if ((source == this || source.Team == base.Team) && damageAmount + criticalAmount >= m_blood)
		{
			damageAmount = m_blood - 1;
			criticalAmount = 0;
		}
		bool result = base.TakeDamage(source, ref damageAmount, ref criticalAmount, msg);
		if (base.IsLiving)
		{
			AddDander((damageAmount * 2 / 5 + 5) / 2);
		}
		return result;
	}

	public void UseSpecialSkill()
	{
		if (base.Dander >= 200)
		{
			SetBall(m_spBallId, special: true);
			m_ballCount = m_currentBall.Amount;
			SetDander(0);
		}
	}

	public void SetBall(int ballId)
	{
		SetBall(ballId, special: false);
	}

	public void SetBall(int ballId, bool special)
	{
		if (ballId != m_currentBall.ID)
		{
			if (BallMgr.FindBall(ballId) != null)
			{
				m_currentBall = BallMgr.FindBall(ballId);
			}
			m_game.SendGameUpdateBall(this, special);
		}
	}

	public void SetCurrentWeapon(ItemTemplateInfo item)
	{
		m_weapon = item;
		BallConfigInfo ballConfigInfo = BallConfigMgr.FindBall(m_weapon.TemplateID);
		if (ChangeSpecialBall > 0)
		{
			ballConfigInfo = BallConfigMgr.FindBall(70396);
		}
		m_mainBallId = ballConfigInfo.Common;
		m_spBallId = ballConfigInfo.Special;
		m_AddWoundBallId = ballConfigInfo.CommonAddWound;
		m_MultiBallId = ballConfigInfo.CommonMultiBall;
		SetBall(m_mainBallId);
	}

	public override void StartMoving()
	{
		if (m_map != null)
		{
			Point point = m_map.FindYLineNotEmptyPoint(m_x, m_y);
			if (point.IsEmpty)
			{
				m_y = m_map.Ground.Height;
			}
			else
			{
				m_x = point.X;
				m_y = point.Y;
			}
			if (point.IsEmpty)
			{
				m_syncAtTime = false;
				Die();
			}
		}
	}

	public override void StartMoving(int delay, int speed)
	{
		if (m_map != null)
		{
			Point point = m_map.FindYLineNotEmptyPoint(m_x, m_y);
			if (point.IsEmpty)
			{
				m_y = m_map.Ground.Height;
			}
			else
			{
				m_x = point.X;
				m_y = point.Y;
			}
			base.StartMoving(delay, speed);
			if (point.IsEmpty)
			{
				m_syncAtTime = false;
				Die();
			}
		}
	}

	public void StartRotate(int rotation, int speed, string endPlay, int delay)
	{
		m_game.AddAction(new LivingRotateTurnAction(this, rotation, speed, endPlay, delay));
	}

	public void StartSpeedMult(int x, int y)
	{
		StartSpeedMult(x, y, 3000);
	}

	public void StartSpeedMult(int x, int y, int delay)
	{
		Point point = new Point(x - X, y - Y);
		m_game.AddAction(new PlayerSpeedMultAction(this, new Point(X + point.X, Y + point.Y), delay));
	}

	public void StartGhostMoving()
	{
		if (!TargetPoint.IsEmpty)
		{
			Point point = new Point(TargetPoint.X - X, TargetPoint.Y - Y);
			if (point.Length() > 160.0)
			{
				point.Normalize(160);
			}
			m_game.AddAction(new GhostMoveAction(this, new Point(X + point.X, Y + point.Y)));
		}
	}

	public override void SetXY(int x, int y)
	{
		if (m_x == x && m_y == y)
		{
			return;
		}
		int num = Math.Abs(m_x - x);
		m_x = x;
		m_y = y;
		if (base.IsLiving)
		{
			m_energy -= Math.Abs(m_x - x);
			if (num > 0)
			{
				OnPlayerMoving();
			}
			return;
		}
		Rectangle rect = m_rect;
		rect.Offset(m_x, m_y);
		Physics[] array = m_map.FindPhysicalObjects(rect, this);
		Physics[] array2 = array;
		foreach (Physics physics in array2)
		{
			if (physics is Box)
			{
				Box box = physics as Box;
				PickBox(box);
				OpenBox(box.Id);
			}
		}
	}

	public override void Die()
	{
		if (base.IsLiving)
		{
			m_y -= 70;
			base.Die();
		}
	}

	public override void PickBox(Box box)
	{
		m_tempBoxes.Add(box);
		base.PickBox(box);
	}

	public void OpenBox(int boxId)
	{
		Box box = null;
		foreach (Box tempBox in m_tempBoxes)
		{
			if (tempBox.Id == boxId)
			{
				box = tempBox;
				break;
			}
		}
		if (box == null || box.Item == null)
		{
			return;
		}
		ItemInfo item = box.Item;
		switch (item.TemplateID)
		{
		case -1100:
			m_player.AddGiftToken(item.Count);
			break;
		case -300:
			m_player.AddMedal(item.Count);
			break;
		case -200:
			m_player.AddMoney(item.Count);
			m_player.LogAddMoney(AddMoneyType.Box, AddMoneyType.Box_Open, m_player.PlayerCharacter.ID, item.Count, m_player.PlayerCharacter.Money);
			break;
		case -100:
			m_player.AddGold(item.Count);
			break;
		default:
			if (item.Template.CategoryID == 10)
			{
				m_player.AddTemplate(item, eBageType.FightBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipTypeView);
			}
			else
			{
				m_player.AddTemplate(item, eBageType.TempBag, item.Count, eItemNotice.GoodsTipBroadcastTypeView, eItemNotice.GoodsTipTypeView);
			}
			break;
		}
		m_tempBoxes.Remove(box);
	}

	public override void PrepareNewTurn()
	{
		if (CurrentIsHitTarget)
		{
			TotalHitTargetCount++;
		}
		m_energy = (int)Agility / 30 + 240;
		if (base.FightBuffers.ConsortionAddEnergy > 0)
		{
			m_energy += base.FightBuffers.ConsortionAddEnergy;
		}
		base.PetEffects.PetSkillStase = 0;
		base.PetEffects.PetDelay = 0;
		base.SpecialSkillDelay = 0;
		m_shootCount = 1;
		m_ballCount = 1;
		EffectTrigger = false;
		PetEffectTrigger = false;
		m_flyCoolDown--;
		SetCurrentWeapon(PlayerDetail.MainWeapon);
		if (m_currentBall.ID != m_mainBallId)
		{
			m_currentBall = BallMgr.FindBall(m_mainBallId);
		}
		if (m_game.RoomType == eRoomType.FightFootballTime)
		{
			m_currentBall = BallMgr.FindBall(24);
		}
		if (!base.IsLiving)
		{
			StartGhostMoving();
			TargetPoint = Point.Empty;
		}
		SpeedMultX(3);
		base.PrepareNewTurn();
	}

	public override void PrepareSelfTurn()
	{
		base.PrepareSelfTurn();
		DefaultDelay = m_delay;
		if (base.IsFrost)
		{
			AddDelay(GetTurnDelay());
		}
	}

	public override void CollidedByObject(Physics phy)
	{
		base.CollidedByObject(phy);
		if (phy is SimpleBomb)
		{
			OnCollidedByObject();
		}
	}

	public override void StartAttacking()
	{
		if (!base.IsAttacking)
		{
			if (m_Healstone != null && m_blood < m_maxBlood)
			{
				AddBlood(m_Healstone.Template.Property2);
				m_player.RemoveHealstone();
			}
			AddDelay(GetTurnDelay());
			base.StartAttacking();
		}
	}

	private int GetTurnDelay()
	{
		return 1600 - 1200 * PlayerDetail.PlayerCharacter.Agility / (PlayerDetail.PlayerCharacter.Agility + 1200) + PlayerDetail.PlayerCharacter.Attack / 10;
	}

	public override void Skip(int spendTime)
	{
		if (base.IsAttacking)
		{
			base.Game.SendSkipNext(this);
			m_prop = 0;
			AddDelay(100);
			AddDander(20);
			AddPetMP(10);
			base.Skip(spendTime);
		}
	}

	public void PrepareShoot(byte speedTime)
	{
		int turnWaitTime = m_game.GetTurnWaitTime();
		int num = ((speedTime > turnWaitTime) ? turnWaitTime : speedTime);
		AddDelay(num * 20);
		TotalShootCount++;
	}

	public void PetUseKill(int skillID)
	{
		if (!CanUseSkill(skillID))
		{
			return;
		}
		PetSkillInfo petSkillInfo = PetMgr.FindPetSkill(skillID);
		if (base.PetMP > 0 && base.PetMP >= petSkillInfo.CostMP)
		{
			if (petSkillInfo.NewBallID != -1)
			{
				m_delay += petSkillInfo.Delay;
				SetBall(petSkillInfo.NewBallID);
			}
			base.PetMP -= petSkillInfo.CostMP;
			if (petSkillInfo.DamageCrit > 0)
			{
				base.PetEffects.CritActive = true;
				CurrentDamagePlus += petSkillInfo.DamageCrit / 100;
			}
			base.PetEffects.IsPetUseSkill = true;
			base.PetEffects.PetSkillStase = skillID;
			m_game.SendPetUseKill(this);
			OnPlayerBuffSkillPet();
		}
		else
		{
			m_player.SendMessage("Ma Pháp không đủ.");
		}
	}

	public bool CanUseSkill(int Id)
	{
		if (m_pet == null)
		{
			return false;
		}
		string[] array = m_pet.SkillEquip.Split('|');
		string[] array2 = array;
		foreach (string text in array2)
		{
			int num = int.Parse(text.Split(',')[0]);
			if (num == Id)
			{
				return true;
			}
		}
		return false;
	}

	public bool Shoot(int x, int y, int force, int angle)
	{
		if (m_shootCount == 1)
		{
			base.PetEffects.ActivePetHit = true;
		}
		if (m_shootCount > 0)
		{
			OnPlayerShoot();
			int bombId = m_currentBall.ID;
			if (m_ballCount == 1 && !IsSpecialSkill)
			{
				if (Prop == 20002)
				{
					bombId = m_MultiBallId;
				}
				if (Prop == 20008)
				{
					bombId = m_AddWoundBallId;
				}
			}
			if (!CurrentBall.IsSpecial())
			{
				OnBeforePlayerShoot();
			}
			if (IsSpecialSkill)
			{
				base.SpecialSkillDelay = 2000;
			}
			if (ShootImp(bombId, x, y, force, angle, m_ballCount, ShootCount))
			{
				m_shootCount--;
				if (m_shootCount <= 0 || !base.IsLiving)
				{
					StopAttacking();
					AddDelay(m_currentBall.Delay + m_weapon.Property8);
					AddDander(20);
					AddPetMP(10);
					m_prop = 0;
					if (CanGetProp)
					{
						int gold = 0;
						int money = 0;
						int giftToken = 0;
						int medal = 0;
						int honor = 0;
						int hardCurrency = 0;
						int token = 0;
						int dragonToken = 0;
						List<ItemInfo> info = null;
						if (DropInventory.FireDrop(m_game.RoomType, ref info) && info != null)
						{
							foreach (ItemInfo item in info)
							{
								ShopMgr.FindSpecialItemInfo(item, ref gold, ref money, ref giftToken, ref medal, ref honor, ref hardCurrency, ref token, ref dragonToken);
								if (item != null && base.VaneOpen)
								{
									PlayerDetail.AddTemplate(item, eBageType.FightBag, item.Count, eItemNotice.GoodsTipTypeView, eItemNotice.OpenTypeView);
								}
							}
							PlayerDetail.AddGold(gold);
							PlayerDetail.AddMoney(money);
							PlayerDetail.LogAddMoney(AddMoneyType.Game, AddMoneyType.Game_Shoot, PlayerDetail.PlayerCharacter.ID, money, PlayerDetail.PlayerCharacter.Money);
							PlayerDetail.AddGiftToken(giftToken);
							PlayerDetail.AddMedal(medal);
						}
					}
				}
				OnAfterPlayerShoot();
				return true;
			}
		}
		return false;
	}

	public bool CanUseItem(ItemTemplateInfo item)
	{
		return m_energy >= item.Property4 && (base.IsAttacking || (!base.IsLiving && base.Team == m_game.CurrentLiving.Team));
	}

	public bool UseItem(ItemTemplateInfo item)
	{
		if (CanUseItem(item))
		{
			m_energy -= item.Property4;
			m_delay += item.Property5;
			m_game.SendPlayerUseProp(this, -2, -2, item.TemplateID, this);
			SpellMgr.ExecuteSpell(m_game, this, item);
			return true;
		}
		return false;
	}

	public void UseFlySkill()
	{
		if (m_flyCoolDown > 0 && base.Game.RoomType == eRoomType.BattleRoom)
		{
			m_flyCoolDown--;
			m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
			SetBall(3);
		}
		else
		{
			m_game.SendPlayerUseProp(this, -2, -2, CARRY_TEMPLATE_ID);
			SetBall(3);
		}
	}

	public void UseSecondWeapon()
	{
		if (CanUseItem(m_DeputyWeapon.Template))
		{
			if (m_DeputyWeapon.Template.Property3 == 31)
			{
				int count = (int)getHertAddition(m_DeputyWeapon);
				new AddGuardEquipEffect(count, 1).Start(this);
			}
			else
			{
				SetCurrentWeapon(m_DeputyWeapon.Template);
			}
			ShootCount = 1;
			m_energy -= m_DeputyWeapon.Template.Property4;
			m_delay += m_DeputyWeapon.Template.Property5;
			m_game.SendPlayerUseProp(this, -2, -2, m_DeputyWeapon.Template.TemplateID);
			if (deputyWeaponResCount > 0)
			{
				deputyWeaponResCount--;
				m_game.SendUseDeputyWeapon(this, deputyWeaponResCount);
			}
		}
	}

	public void DeadLink()
	{
		m_isActive = false;
		if (base.IsLiving)
		{
			Die();
		}
	}

	public bool CheckShootPoint(int x, int y)
	{
		if (Math.Abs(X - x) > 100)
		{
			string userName = m_player.PlayerCharacter.UserName;
			string nickName = m_player.PlayerCharacter.NickName;
			m_player.Disconnect();
			return false;
		}
		return true;
	}

	protected void OnPlayerMoving()
	{
		if (PlayerBeginMoving != null)
		{
			PlayerBeginMoving(this);
		}
	}

	protected void OnAfterPlayerShoot()
	{
		if (AfterPlayerShooted != null)
		{
			AfterPlayerShooted(this);
		}
	}

	protected void OnBeforePlayerShoot()
	{
		if (BeforePlayerShoot != null)
		{
			BeforePlayerShoot(this);
		}
	}

	protected void OnLoadingCompleted()
	{
		if (LoadingCompleted != null)
		{
			LoadingCompleted(this);
		}
	}

	public void OnPlayerShoot()
	{
		if (PlayerShoot != null)
		{
			PlayerShoot(this);
		}
	}

	public void OnPlayerBuffSkillPet()
	{
		if (PlayerBuffSkillPet != null)
		{
			PlayerBuffSkillPet(this);
		}
	}

	protected void OnCollidedByObject()
	{
		if (CollidByObject != null)
		{
			CollidByObject(this);
		}
	}

	public override void OnAfterKillingLiving(Living target, int damageAmount, int criticalAmount)
	{
		base.OnAfterKillingLiving(target, damageAmount, criticalAmount);
		if (target is Player)
		{
			m_player.OnKillingLiving(m_game, 1, target.Id, target.IsLiving, damageAmount + criticalAmount);
			return;
		}
		int id = 0;
		if (target is SimpleBoss)
		{
			SimpleBoss simpleBoss = target as SimpleBoss;
			id = simpleBoss.NpcInfo.ID;
		}
		if (target is SimpleNpc)
		{
			SimpleNpc simpleNpc = target as SimpleNpc;
			id = simpleNpc.NpcInfo.ID;
		}
		m_player.OnKillingLiving(m_game, 2, id, target.IsLiving, damageAmount + criticalAmount);
	}
}
