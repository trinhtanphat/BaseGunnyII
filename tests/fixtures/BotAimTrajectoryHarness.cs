using System;
using System.Drawing;
using Fighting.Server.GameObjects;
class BotAimTrajectoryHarness
{
    static int Main()
    {
        Rectangle target = new Rectangle(90, 45, 10, 10);
        Func<Rectangle, bool> clear = delegate { return true; };
        bool clearHit = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            target, 10, 200, 100, clear);
        Func<Rectangle, bool> wall = delegate(Rectangle r) { return r.Left < 40; };
        bool blocked = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            target, 10, 200, 100, wall);
        Func<Rectangle, bool> nearTarget = delegate(Rectangle r) { return r.Left < 84; };
        bool blastHit = BotAimTrajectory.IsViable(0, 50, 100, 0, 1, 0, 0, 0,
            target, 12, 200, 100, nearTarget);
        Console.WriteLine("CLEAR_HIT=" + clearHit);
        Console.WriteLine("BLOCKED=" + blocked);
        Console.WriteLine("BLAST_HIT=" + blastHit);
        bool ok = clearHit && !blocked && blastHit;
        Console.WriteLine("BOT_AIM_TRAJECTORY_BEHAVIOR_PASS=" + ok);
        return ok ? 0 : 9;
    }
}