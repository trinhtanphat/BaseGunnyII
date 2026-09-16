using System;
using System.Drawing;
using Fighting.Server.GameObjects;
class BotAimTrajectoryHarness
{
    static Rectangle[] One(Rectangle r) { return new Rectangle[] { r }; }
    static int Main()
    {
        Rectangle target = new Rectangle(90, 45, 10, 10);
        Func<Rectangle, bool> clear = delegate { return true; };
        Func<int, int, double> far = delegate { return 999; };
        bool clearHit = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            One(target), 10, 200, 100, clear, far);
        Func<Rectangle, bool> wall = delegate(Rectangle r) { return r.Left < 40; };
        bool blocked = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            One(target), 10, 200, 100, wall, far);
        Func<Rectangle, bool> nearTarget = delegate(Rectangle r) { return r.Left < 84; };
        bool blastHit = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            One(target), 12, 200, 100, nearTarget, delegate { return 11; });
        bool edgeSplashAccepted = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            One(new Rectangle(95, 45, 10, 10)), 8, 200, 100,
            delegate(Rectangle r) { return r.Left < 89; }, delegate { return 6; });
        bool boundarySplashRejected = !BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            One(new Rectangle(95, 45, 10, 10)), 8, 200, 100,
            delegate(Rectangle r) { return r.Left < 89; }, delegate { return 8; });
        Rectangle[] splitBounds = {
            new Rectangle(90, 45, 10, 10),
            new Rectangle(150, 45, 10, 10)
        };
        bool splitBoundsRejected = !BotAimTrajectory.IsViable(110, 50, 100, 0, 1, 0, 0, 0,
            splitBounds, 12, 220, 100, delegate(Rectangle r) { return r.Left < 119; }, far);
        Func<Rectangle, bool> thinWall = delegate(Rectangle r) { return !(r.Right >= 29 && r.Left <= 32); };
        bool tunnelingBlocked = BotAimTrajectory.IsViable(0, 50, 500, 0, 1, 0, 0, 0,
            One(target), 10, 200, 100, thinWall, far);
        Console.WriteLine("CLEAR_HIT=" + clearHit);
        Console.WriteLine("BLOCKED=" + blocked);
        Console.WriteLine("BLAST_HIT=" + blastHit);
        Console.WriteLine("EDGE_SPLASH_ACCEPTED=" + edgeSplashAccepted);
        Console.WriteLine("BOUNDARY_SPLASH_REJECTED=" + boundarySplashRejected);
        Console.WriteLine("SPLIT_BOUNDS_REJECTED=" + splitBoundsRejected);
        Console.WriteLine("TUNNELING_BLOCKED=" + tunnelingBlocked);
        bool ok = clearHit && !blocked && blastHit && edgeSplashAccepted && boundarySplashRejected &&
            splitBoundsRejected && !tunnelingBlocked;
        Console.WriteLine("BOT_AIM_TRAJECTORY_BEHAVIOR_PASS=" + ok);
        return ok ? 0 : 9;
    }
}