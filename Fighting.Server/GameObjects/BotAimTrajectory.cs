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
            float vx = (float)(force * Math.Cos(radians));
            float vy = (float)(force * Math.Sin(radians));
            float x = startX;
            float y = startY;
            Rectangle projectile = new Rectangle();            for (float life = 0; life <= MaxLife; life += Step)
            {
                float ax = (wind - airResistance * vx) / mass;
                float ay = (gravity - airResistance * vy) / mass;
                vx += ax * Step;
                vy += ay * Step;
                x += vx * Step;
                y += vy * Step;

                int px = (int)x;
                int py = (int)y;
                if (px < 0 || px >= mapWidth || py >= mapHeight)
                    return false;

                projectile = new Rectangle(px - 3, py - 3, 6, 6);
                if (projectile.IntersectsWith(targetBounds))
                    return true;

                if (!isRectangleEmpty(projectile))
                    return DistanceToRectangle(px, py, targetBounds) <= blastRadius;
            }
            return false;
        }
        private static double DistanceToRectangle(int x, int y, Rectangle rect)
        {
            int nearestX = x < rect.Left ? rect.Left : (x > rect.Right ? rect.Right : x);
            int nearestY = y < rect.Top ? rect.Top : (y > rect.Bottom ? rect.Bottom : y);
            int dx = x - nearestX;
            int dy = y - nearestY;
            return Math.Sqrt(dx * dx + dy * dy);
        }
    }
}