using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Logic
{
    public static class PvpSpawnResolver
    {
        public static Point TakeSafeSpawn(List<Point> candidates, Func<Point, bool> isSafe, Func<int, int> nextIndex)
        {
            if (candidates == null) throw new ArgumentNullException("candidates");
            if (isSafe == null) throw new ArgumentNullException("isSafe");
            if (nextIndex == null) throw new ArgumentNullException("nextIndex");
            while (candidates.Count > 0)
            {
                int index = nextIndex(candidates.Count);
                if (index < 0 || index >= candidates.Count) throw new ArgumentOutOfRangeException("nextIndex");
                Point point = candidates[index];
                candidates.RemoveAt(index);
                if (isSafe(point)) return point;
            }
            return Point.Empty;
        }
    }
}
