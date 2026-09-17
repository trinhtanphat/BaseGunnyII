using System;
using System.Collections.Generic;
using System.Drawing;
using Bussiness.Managers;
using Game.Base.Packets;
using Game.Logic;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Fighting.Server.GameObjects
{
    public class BotProxyPlayer : IGamePlayer, IBotGamePlayer
    {
        private const int SelfHealTemplateId = 10012;
        private const int TeamHealTemplateId = 10009;
        private const int FlyTemplateId = 10016;
        private const int DamageBoostTemplateId = 10004;
        private const int MultiBallTemplateId = 10003;

        private readonly PlayerInfo m_character;
        private readonly ItemTemplateInfo m_weapon;
        private readonly double m_baseAttack;
        private readonly double m_baseDefence;
        private readonly double m_baseBlood;
        private int m_gamePlayerId;
        private int m_serverId;
        private bool m_canUseProp;
        private List<int> m_equipEffect;
        private int m_selfHealUses;
        private int m_teamHealUses;
        private int m_flyUses;
        private int m_damageBoostUses;
        private int m_multiBallUses;

        public BotProxyPlayer(IGamePlayer source, int botId)
        {
            PlayerInfo src = source.PlayerCharacter;
            m_character = new PlayerInfo();
            m_character.ID = botId;
            m_character.UserName = "bot_" + Math.Abs(botId);
            m_character.NickName = "NPC " + src.NickName;
            m_character.Sex = src.Sex;
            m_character.Hide = src.Hide;
            m_character.Style = src.Style;
            m_character.Colors = src.Colors;
            m_character.Skin = src.Skin;
            m_character.GP = src.GP;
            m_character.Grade = src.Grade;
            m_character.Repute = src.Repute;
            m_character.Attack = src.Attack;
            m_character.Defence = src.Defence;
            m_character.Agility = src.Agility;
            m_character.Luck = src.Luck;
            m_character.FightPower = src.FightPower;
            m_character.Nimbus = src.Nimbus;
            m_character.Win = 0;
            m_character.Total = 0;
            m_character.Offer = 0;
            m_character.ConsortiaID = 0;
            m_character.ConsortiaName = string.Empty;
            CopyWeakGuildProgress(src, m_character);
            m_weapon = source.MainWeapon;
            m_baseAttack = source.GetBaseAttack();
            m_baseDefence = source.GetBaseDefence();
            m_baseBlood = source.GetBaseBlood();
            m_equipEffect = new List<int>();
        }

        private static void CopyWeakGuildProgress(PlayerInfo source, PlayerInfo target)
        {
            var property = typeof(PlayerInfo).GetProperty("weaklessGuildProgress");
            if (property == null || property.PropertyType != typeof(byte[]) || !property.CanRead || !property.CanWrite)
                return;

            byte[] progress = property.GetValue(source, null) as byte[];
            int length = progress == null ? 2 : Math.Max(2, progress.Length);
            byte[] clone = new byte[length];
            if (progress != null)
                Array.Copy(progress, clone, progress.Length);
            property.SetValue(target, clone, null);
        }

        public bool IsBot { get { return true; } }
        public PlayerInfo PlayerCharacter { get { return m_character; } }
        public ItemTemplateInfo MainWeapon { get { return m_weapon; } }
        public ItemInfo SecondWeapon { get { return null; } }
        public bool CanUseProp { get { return m_canUseProp; } set { m_canUseProp = value; } }
        public int GamePlayerId { get { return m_gamePlayerId; } set { m_gamePlayerId = value; } }
        public int ServerID { get { return m_serverId; } set { m_serverId = value; } }
        public List<int> EquipEffect { get { return m_equipEffect; } set { m_equipEffect = value ?? new List<int>(); } }

        public double GetBaseBlood() { return m_baseBlood; }
        public double GetGoldBlood() { return 0; }
        public double GetBaseAttack() { return m_baseAttack; }
        public double GetBaseDefence() { return m_baseDefence; }

        public int AddGP(int value) { return 0; }
        public int RemoveGP(int value) { return 0; }
        public int AddGold(int value) { return 0; }
        public int RemoveGold(int value) { return 0; }
        public int AddMoney(int value) { return 0; }
        public int RemoveMoney(int value) { return 0; }
        public int AddGiftToken(int value) { return 0; }
        public int RemoveGiftToken(int value) { return 0; }
        public int AddMedal(int value) { return 0; }
        public int RemoveMedal(int value) { return 0; }
        public int AddOffer(int value) { return 0; }
        public int RemoveOffer(int value) { return 0; }
        public bool AddTemplate(ItemInfo cloneItem, eBageType bagType, int count) { return false; }
        public bool ClearTempBag() { return true; }
        public bool ClearFightBag() { return true; }
        public bool UsePropItem(AbstractGame game, int bag, int place, int templateId, bool isLiving) { return false; }
        public void OnKillingLiving(AbstractGame game, int type, int id, bool isLiving, int damage) { }
        public void OnGameOver(AbstractGame game, bool isWin, int gainXp) { }
        public void OnMissionOver(AbstractGame game, bool isWin, int missionId, int turnNum) { }
        public int ConsortiaFight(int consortiaWin, int consortiaLose, Dictionary<int, Player> players,
            eRoomType roomType, eGameType gameClass, int totalKillHealth, int count) { return 0; }
        public void SendConsortiaFight(int consortiaID, int riches, string msg) { }
        public bool SetPvePermission(int missionId, eHardLevel hardLevel) { return true; }
        public bool IsPvePermission(int missionId, eHardLevel hardLevel) { return true; }
        public void Disconnect() { }
        public void SendInsufficientMoney(int type) { }
        public void SendMessage(string msg) { }
        public void SendTCP(GSPacketIn pkg) { }
        public void LogAddMoney(AddMoneyType masterType, AddMoneyType sonType, int userId, int money, int spareMoney) { }

        private static Player FindBestTarget(PVPGame game, Player player)
        {
            Player target = null;
            double bestScore = double.MaxValue;
            double bestDistance = double.MaxValue;
            foreach (Player candidate in game.GetAllFightPlayers())
            {
                if (!candidate.IsLiving || candidate.Blood <= 0 || candidate.Team == player.Team)
                    continue;

                double teamDistance = 0;
                int allyCount = 0;
                foreach (Player ally in game.GetAllFightPlayers())
                {
                    if (!ally.IsLiving || ally.Blood <= 0 || ally.Team != player.Team)
                        continue;
                    teamDistance += candidate.Distance(ally.X, ally.Y);
                    allyCount++;
                }

                double distance = allyCount == 0 ? candidate.Distance(player.X, player.Y) : teamDistance / allyCount;
                double score = distance + candidate.Blood * 0.35;
                if (target == null || score < bestScore - 0.01 ||
                    (Math.Abs(score - bestScore) <= 0.01 && candidate.Blood < target.Blood) ||
                    (Math.Abs(score - bestScore) <= 0.01 && candidate.Blood == target.Blood && distance < bestDistance))
                {
                    bestScore = score;
                    bestDistance = distance;
                    target = candidate;
                }
            }
            return target;
        }

        private static BotTrajectoryProbe ProbeTrajectory(Player player, Player target,
            int force, int angle)
        {
            BallInfo ball = BallMgr.FindBall(player.CurrentBall.ID);
            if (ball == null || player.Game == null || player.Game.Map == null)
                return new BotTrajectoryProbe(BotTrajectoryOutcome.None, player.X, player.Y);

            List<Rectangle> targetBounds = target.GetDirectBoudRect();
            var map = player.Game.Map;
            Point shootPoint = player.GetShootPoint();
            return BotAimTrajectory.Probe(shootPoint.X, shootPoint.Y, force, angle, ball.Mass,
                map.airResistance * ball.DragIndex,
                map.gravity * ball.Weight * ball.Mass, map.wind * ball.Wind,
                targetBounds, ball.Radii, map.Bound.Width, map.Bound.Height,
                delegate(Rectangle rect) { return map.IsRectangleEmpty(rect); },
                delegate(int impactX, int impactY)
                {
                    return target.Distance(new Point(impactX, impactY));
                });
        }

        private static bool IsTrajectoryViable(Player player, Player target,
            int force, int angle)
        {
            BallInfo ball = BallMgr.FindBall(player.CurrentBall.ID);
            if (ball == null || player.Game == null || player.Game.Map == null)
                return false;

            List<Rectangle> targetBounds = target.GetDirectBoudRect();
            var map = player.Game.Map;
            Point shootPoint = player.GetShootPoint();
            return BotAimTrajectory.IsViable(shootPoint.X, shootPoint.Y, force, angle, ball.Mass,
                map.airResistance * ball.DragIndex,
                map.gravity * ball.Weight * ball.Mass, map.wind * ball.Wind,
                targetBounds, ball.Radii, map.Bound.Width, map.Bound.Height,
                delegate(Rectangle rect) { return map.IsRectangleEmpty(rect); },
                delegate(int impactX, int impactY)
                {
                    return target.Distance(new Point(impactX, impactY));
                });
        }

        private static bool TryFindAccurateShot(Player player, Player target,
            out int aimX, out int aimY, out int force, out int angle)
        {
            float[] timeSeeds = { 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f };
            int[] yOffsets = { 0, -8, 8, -16, 16 };
            foreach (int yOffset in yOffsets)
            {
                foreach (float timeSeed in timeSeeds)
                {
                    int candidateX = target.X;
                    int candidateY = target.Y + yOffset;
                    int candidateForce = 0;
                    int candidateAngle = 0;
                    player.GetShootForceAndAngle(ref candidateX, ref candidateY, player.CurrentBall.ID,
                        1, 5, 1, timeSeed, ref candidateForce, ref candidateAngle);
                    if (candidateForce > 0 && IsTrajectoryViable(player, target,
                        candidateForce, candidateAngle))
                    {
                        aimX = candidateX;
                        aimY = candidateY;
                        force = candidateForce;
                        angle = candidateAngle;
                        return true;
                    }
                }
            }

            aimX = 0;
            aimY = 0;
            force = 0;
            angle = 0;
            return false;
        }

        private static bool IsTerrainImpactSafe(PVPGame game, Player player, Point impact, int blastRadius)
        {
            double safetyRadius = Math.Max(45, blastRadius * 1.15);
            foreach (Player ally in game.GetAllFightPlayers())
            {
                if (!ally.IsLiving || ally.Blood <= 0 || ally.Team != player.Team || ally.Id == player.Id)
                    continue;
                if (ally.Distance(impact) < safetyRadius)
                    return false;
            }
            return true;
        }

        private static bool TryFindTerrainClearShot(PVPGame game, Player player, Player target,
            out int force, out int angle, out double progress)
        {
            float[] timeSeeds = { 0.6f, 0.7f, 0.8f, 0.9f, 1.0f, 1.1f, 1.25f };
            int[] yOffsets = { 0, -16, 16, -32, 32, -56, 56, -80, 80 };
            BallInfo ball = BallMgr.FindBall(player.CurrentBall.ID);
            if (ball == null)
            {
                force = 0;
                angle = 0;
                progress = 0;
                return false;
            }

            double startDistance = target.Distance(new Point(player.X, player.Y));
            double bestScore = double.MaxValue;
            int bestForce = 0;
            int bestAngle = 0;
            double bestProgress = 0;

            foreach (int yOffset in yOffsets)
            {
                foreach (float timeSeed in timeSeeds)
                {
                    int candidateX = target.X;
                    int candidateY = target.Y + yOffset;
                    int candidateForce = 0;
                    int candidateAngle = 0;
                    player.GetShootForceAndAngle(ref candidateX, ref candidateY, player.CurrentBall.ID,
                        1, 5, 1, timeSeed, ref candidateForce, ref candidateAngle);
                    if (candidateForce <= 0)
                        continue;

                    BotTrajectoryProbe probe = ProbeTrajectory(player, target, candidateForce, candidateAngle);
                    if (probe.Outcome != BotTrajectoryOutcome.Terrain)
                        continue;

                    Point impact = new Point(probe.ImpactX, probe.ImpactY);
                    if (!IsTerrainImpactSafe(game, player, impact, ball.Radii))
                        continue;

                    double targetDistance = target.Distance(impact);
                    double candidateProgress = startDistance - targetDistance;
                    if (candidateProgress < -25)
                        continue;

                    double score = targetDistance - candidateProgress * 0.45 + Math.Abs(yOffset) * 0.15;
                    if (bestForce == 0 || score < bestScore)
                    {
                        bestScore = score;
                        bestForce = candidateForce;
                        bestAngle = candidateAngle;
                        bestProgress = candidateProgress;
                    }
                }
            }

            force = bestForce;
            angle = bestAngle;
            progress = bestProgress;
            return bestForce > 0;
        }

        private static bool TryFindFlyShot(Player player, Player target,
            out int force, out int angle, out double improvement)
        {
            BallInfo flyBall = BallMgr.FindBall(3);
            if (flyBall == null || player.Game == null || player.Game.Map == null)
            {
                force = 0;
                angle = 0;
                improvement = 0;
                return false;
            }

            int direction = target.X >= player.X ? 1 : -1;
            int gap = Math.Abs(target.X - player.X);
            int[] xOffsets =
            {
                direction * Math.Min(300, Math.Max(140, gap / 2)),
                direction * Math.Min(420, Math.Max(180, gap * 2 / 3)),
                direction * Math.Min(220, Math.Max(120, gap / 3))
            };
            int[] yOffsets = { -80, -40, 0, 40 };
            float[] timeSeeds = { 0.7f, 0.9f, 1.1f, 1.3f };
            var map = player.Game.Map;
            Point shootPoint = player.GetShootPoint();
            double currentDistance = target.Distance(new Point(player.X, player.Y));
            double bestDistance = currentDistance;
            int bestForce = 0;
            int bestAngle = 0;

            foreach (int xOffset in xOffsets)
            {
                foreach (int yOffset in yOffsets)
                {
                    foreach (float timeSeed in timeSeeds)
                    {
                        int candidateX = player.X + xOffset;
                        int candidateY = target.Y + yOffset;
                        int candidateForce = 0;
                        int candidateAngle = 0;
                        player.GetShootForceAndAngle(ref candidateX, ref candidateY, 3,
                            1, 5, 1, timeSeed, ref candidateForce, ref candidateAngle);
                        if (candidateForce <= 0)
                            continue;

                        BotTrajectoryProbe probe = BotAimTrajectory.ProbeTerrain(
                            shootPoint.X, shootPoint.Y, candidateForce, candidateAngle,
                            flyBall.Mass, map.airResistance * flyBall.DragIndex,
                            map.gravity * flyBall.Weight * flyBall.Mass, map.wind * flyBall.Wind,
                            map.Bound.Width, map.Bound.Height,
                            delegate(Rectangle rect) { return map.IsRectangleEmpty(rect); });
                        if (probe.Outcome != BotTrajectoryOutcome.Terrain)
                            continue;

                        if (Math.Abs(probe.ImpactX - player.X) < 100 || probe.ImpactY <= 10)
                            continue;

                        double landingDistance = target.Distance(new Point(probe.ImpactX, probe.ImpactY));
                        if (landingDistance + 70 >= currentDistance)
                            continue;

                        if (bestForce == 0 || landingDistance < bestDistance)
                        {
                            bestDistance = landingDistance;
                            bestForce = candidateForce;
                            bestAngle = candidateAngle;
                        }
                    }
                }
            }

            force = bestForce;
            angle = bestAngle;
            improvement = currentDistance - bestDistance;
            return bestForce > 0;
        }

        private static bool TryUseTemplate(Player player, int templateId)
        {
            ItemTemplateInfo item = ItemMgr.FindItemTemplate(templateId);
            return item != null && player.UseItem(item);
        }

        private bool TryUseSupportSkill(PVPGame game, Player player)
        {
            int estimatedMaxBlood = Math.Max(1, (int)Math.Round(m_baseBlood));
            if (m_selfHealUses < 2 && player.Blood * 100 <= estimatedMaxBlood * 40)
            {
                if (TryUseTemplate(player, SelfHealTemplateId))
                {
                    m_selfHealUses++;
                    return true;
                }
            }

            if (m_teamHealUses < 1)
            {
                int woundedAllies = 0;
                foreach (Player ally in game.GetAllFightPlayers())
                {
                    if (ally.IsLiving && ally.Blood > 0 && ally.Team == player.Team &&
                        ally.Blood * 100 <= estimatedMaxBlood * 55)
                        woundedAllies++;
                }
                if (woundedAllies >= 2 && TryUseTemplate(player, TeamHealTemplateId))
                {
                    m_teamHealUses++;
                    return true;
                }
            }
            return false;
        }

        private void TryUseOffensiveSkill(Player player, Player target)
        {
            double distance = target.Distance(new Point(player.X, player.Y));
            if (m_damageBoostUses < 2 && target.Blood > Math.Max(250, m_baseBlood * 0.25))
            {
                if (TryUseTemplate(player, DamageBoostTemplateId))
                {
                    m_damageBoostUses++;
                    return;
                }
            }

            if (m_multiBallUses < 1 && distance < 520)
            {
                if (TryUseTemplate(player, MultiBallTemplateId))
                    m_multiBallUses++;
            }
        }

        public void TakeTurn(PVPGame game, Player player)
        {
            TryUseSupportSkill(game, player);

            Player target = FindBestTarget(game, player);
            if (target == null)
            {
                player.Skip(0);
                return;
            }

            player.Direction = target.X >= player.X ? 1 : -1;
            int aimX;
            int aimY;
            int force;
            int angle;
            if (TryFindAccurateShot(player, target, out aimX, out aimY, out force, out angle))
            {
                TryUseOffensiveSkill(player, target);
                Point shootPoint = player.GetShootPoint();
                player.Shoot(shootPoint.X, shootPoint.Y, force, angle);
                return;
            }

            int clearForce;
            int clearAngle;
            double clearProgress;
            bool canClear = TryFindTerrainClearShot(game, player, target,
                out clearForce, out clearAngle, out clearProgress);

            int flyForce;
            int flyAngle;
            double flyImprovement;
            bool canFly = m_flyUses < 2 && TryFindFlyShot(player, target,
                out flyForce, out flyAngle, out flyImprovement);

            if (canClear && (clearProgress >= 45 || !canFly))
            {
                Point shootPoint = player.GetShootPoint();
                player.Shoot(shootPoint.X, shootPoint.Y, clearForce, clearAngle);
                return;
            }

            if (canFly && TryUseTemplate(player, FlyTemplateId))
            {
                m_flyUses++;
                Point shootPoint = player.GetShootPoint();
                player.Shoot(shootPoint.X, shootPoint.Y, flyForce, flyAngle);
                return;
            }

            if (canClear)
            {
                Point shootPoint = player.GetShootPoint();
                player.Shoot(shootPoint.X, shootPoint.Y, clearForce, clearAngle);
                return;
            }

            player.Skip(0);
        }
    }
}
