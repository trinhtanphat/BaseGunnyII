using System;
using System.Drawing;
using Fighting.Server.GameObjects;

class BotAimTrajectoryTunnelingHarness
{
    static int Main()
    {
        Rectangle target = new Rectangle(90, 45, 10, 10);
        Func<Rectangle, bool> thinWall = delegate(Rectangle r)
        {
            return !(r.Right >= 29 && r.Left <= 32);
        };
        bool viable = BotAimTrajectory.IsViable(0, 50, 500, 0, 1, 0, 0, 0,
            target, 10, 200, 100, thinWall);
        Console.WriteLine("TUNNELING_CANDIDATE_VIABLE=" + viable);
        bool ok = !viable;
        Console.WriteLine("BOT_AIM_TUNNELING_BEHAVIOR_PASS=" + ok);
        return ok ? 0 : 9;
    }
}
