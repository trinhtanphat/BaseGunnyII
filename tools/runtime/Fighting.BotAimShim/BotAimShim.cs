using System;
using System.Collections.Generic;
using System.Drawing;
using Bussiness.Managers;
using Game.Logic;
using Game.Logic.Phy.Maps;
using Game.Logic.Phy.Object;
using SqlDataProvider.Data;

namespace Fighting.BotAimShim
{
    public static class BotAimShim
    {
        private const int SelfHealTemplateId = 10012;
        private const int TeamHealTemplateId = 10009;
        private const int FlyTemplateId = 10016;
        private const int DamageBoostTemplateId = 10004;
        private const int MultiBallTemplateId = 10003;

        private sealed class BotState
        {
            public int SelfHealUses;
            public int TeamHealUses;
            public int FlyUses;
            public int DamageBoostUses;
            public int MultiBallUses;
        }

        private static readonly object StateSync = new object();
        private static readonly Dictionary<long, BotState> States = new Dictionary<long, BotState>();

        private static BotState GetState(PVPGame game, Player player)
        {
            long key = ((long)game.Id << 32) ^ (uint)player.Id;
            lock (StateSync)
            {
                BotState state;
                if (!States.TryGetValue(key, out state))
                {
                    if (States.Count > 4096)
                        States.Clear();
                    state = new BotState();
                    States[key] = state;
                }
                return state;
            }
        }

        private static int EstimateMaxBlood(Player player)
        {
            double value = player.PlayerDetail == null ? player.Blood : player.PlayerDetail.GetBaseBlood();
            return Math.Max(player.Blood, Math.Max(1, (int)Math.Round(value)));
        }

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

                double distance = allyCount == 0
                    ? candidate.Distance(player.X, player.Y)
                    : teamDistance / allyCount;
                double score = distance + candidate.Blood * 0.35;

                if (target == null || score < bestScore - 0.01 ||
                    (Math.Abs(score - bestScore) <= 0.01 && candidate.Blood < target.Blood) ||
                    (Math.Abs(score - bestScore) <= 0.01 && candidate.Blood == target.Blood &&
                     distance < bestDistance))
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
            Map map = player.Game.Map;
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

        private static bool IsTrajectoryViable(Player player, Player target, int force, int angle)
        {
            BallInfo ball = BallMgr.FindBall(player.CurrentBall.ID);
            if (ball == null || player.Game == null || player.Game.Map == null)
                return false;

            List<Rectangle> targetBounds = target.GetDirectBoudRect();
            Map map = player.Game.Map;
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
            out int force, out int angle)
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

                    if (candidateForce > 0 &&
                        IsTrajectoryViable(player, target, candidateForce, candidateAngle))
                    {
                        force = candidateForce;
                        angle = candidateAngle;
                        return true;
                    }
                }
            }

            force = 0;
            angle = 0;
            return false;
        }

        private static bool IsTerrainImpactSafe(PVPGame game, Player player,
            Point impact, int blastRadius)
        {
            double safetyRadius = Math.Max(45, blastRadius * 1.15);
            foreach (Player ally in game.GetAllFightPlayers())
            {
                if (!ally.IsLiving || ally.Blood <= 0 || ally.Team != player.Team ||
                    ally.Id == player.Id)
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

                    BotTrajectoryProbe probe = ProbeTrajectory(player, target,
                        candidateForce, candidateAngle);
                    if (probe.Outcome != BotTrajectoryOutcome.Terrain)
                        continue;

                    Point impact = new Point(probe.ImpactX, probe.ImpactY);
                    if (!IsTerrainImpactSafe(game, player, impact, ball.Radii))
                        continue;

                    double targetDistance = target.Distance(impact);
                    double candidateProgress = startDistance - targetDistance;
                    if (candidateProgress < -25)
                        continue;

                    double score = targetDistance - candidateProgress * 0.45 +
                        Math.Abs(yOffset) * 0.15;
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
            Map map = player.Game.Map;
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

                        double landingDistance =
                            target.Distance(new Point(probe.ImpactX, probe.ImpactY));
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

        private static void TryUseSupportSkill(PVPGame game, Player player, BotState state)
        {
            int maxBlood = EstimateMaxBlood(player);
            if (state.SelfHealUses < 2 && player.Blood * 100 <= maxBlood * 40)
            {
                if (TryUseTemplate(player, SelfHealTemplateId))
                {
                    state.SelfHealUses++;
                    return;
                }
            }

            if (state.TeamHealUses < 1)
            {
                int woundedAllies = 0;
                foreach (Player ally in game.GetAllFightPlayers())
                {
                    if (!ally.IsLiving || ally.Blood <= 0 || ally.Team != player.Team)
                        continue;
                    int allyMaxBlood = EstimateMaxBlood(ally);
                    if (ally.Blood * 100 <= allyMaxBlood * 55)
                        woundedAllies++;
                }

                if (woundedAllies >= 2 && TryUseTemplate(player, TeamHealTemplateId))
                    state.TeamHealUses++;
            }
        }

        private static void TryUseOffensiveSkill(Player player, Player target, BotState state)
        {
            double distance = target.Distance(new Point(player.X, player.Y));
            if (state.DamageBoostUses < 2 &&
                target.Blood > Math.Max(250, EstimateMaxBlood(player) * 0.25))
            {
                if (TryUseTemplate(player, DamageBoostTemplateId))
                {
                    state.DamageBoostUses++;
                    return;
                }
            }

            if (state.MultiBallUses < 1 && distance < 520)
            {
                if (TryUseTemplate(player, MultiBallTemplateId))
                    state.MultiBallUses++;
            }
        }

        public static void TakeTurn(PVPGame game, Player player)
        {
            BotState state = GetState(game, player);
            TryUseSupportSkill(game, player, state);

            Player target = FindBestTarget(game, player);
            if (target == null)
            {
                player.Skip(0);
                return;
            }

            player.Direction = target.X >= player.X ? 1 : -1;

            int force;
            int angle;
            if (TryFindAccurateShot(player, target, out force, out angle))
            {
                TryUseOffensiveSkill(player, target, state);
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
            bool canFly = state.FlyUses < 2 &&
                TryFindFlyShot(player, target, out flyForce, out flyAngle, out flyImprovement);

            if (canClear && (clearProgress >= 45 || !canFly))
            {
                Point shootPoint = player.GetShootPoint();
                player.Shoot(shootPoint.X, shootPoint.Y, clearForce, clearAngle);
                return;
            }

            if (canFly && TryUseTemplate(player, FlyTemplateId))
            {
                state.FlyUses++;
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
