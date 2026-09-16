using System;
using System.Drawing;

namespace Fighting.Server.GameObjects
{
    internal static class BotAimTrajectory
    {
        private const float Step = 0.04f;
        private const float MaxLife = 6.0f;

        public static bool IsViable(float startX, float startY, int force, int angle,
            float mass, float airResistance, float gravity, float wind,
            Rectangle targetBounds, int blastRadius, int mapWidth, int mapHeight,
            Func<Rectangle, bool> isRectangleEmpty)
        {
            if (force <= 0 || mass <= 0 || isRectangleEmpty == null)
                return false;

            double radians = angle / 180.0 * Math.PI;
            float vx = (int)(force * Math.Cos(radians));
            float vy = (int)(force * Math.Sin(radians));
            float x = startX;
            float y = startY;
            int previousX = (int)x;
            int previousY = (int)y;
            for (float life = 0; life <= MaxLife; life += Step)
            {
                float ax = (wind - airResistance * vx) / mass;
                float ay = (gravity - airResistance * vy) / mass;
                vx += ax * Step;
                vy += ay * Step;
                x += vx * Step;
                y += vy * Step;

                int px = (int)x;
                int py = (int)y;
                int segment = TraceSegment(previousX, previousY, px, py, targetBounds, blastRadius,
                    mapWidth, mapHeight, isRectangleEmpty);
                if (segment != 0)
                    return segment > 0;
                previousX = px;
                previousY = py;
            }
            return false;
        }

        private static int TraceSegment(int x1, int y1, int x2, int y2, Rectangle targetBounds,
            int blastRadius, int mapWidth, int mapHeight, Func<Rectangle, bool> isRectangleEmpty)
        {
            int dx = x2 - x1;
            int dy = y2 - y1;
            int count = Math.Max(Math.Abs(dx), Math.Abs(dy));
            if (count == 0)
                return 0;
            bool useX = Math.Abs(dx) > Math.Abs(dy);
            int direction = useX ? dx / count : dy / count;
            for (int i = 1; i <= count; i += 3)
            {
                int px;
                int py;
                if (useX)
                {
                    px = x1 + i * direction;
                    py = x2 == x1 ? y1 : (px - x1) * (y2 - y1) / (x2 - x1) + y1;
                }
                else
                {
                    py = y1 + i * direction;
                    px = y2 == y1 ? x1 : (py - y1) * (x2 - x1) / (y2 - y1) + x1;
                }
                Rectangle projectile = new Rectangle(px - 3, py - 3, 6, 6);
                if (projectile.IntersectsWith(targetBounds))
                    return 1;
                if (!isRectangleEmpty(projectile))
                    return DistanceToRectangleCenter(px, py, targetBounds) < blastRadius ? 1 : -1;
                if (px < 0 || px >= mapWidth || py >= mapHeight)
                    return -1;
            }
            return 0;
        }
        private static double DistanceToRectangleCenter(int x, int y, Rectangle rect)
        {
            double centerX = rect.Left + rect.Width / 2.0;
            double centerY = rect.Top + rect.Height / 2.0;
            double dx = x - centerX;
            double dy = y - centerY;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}