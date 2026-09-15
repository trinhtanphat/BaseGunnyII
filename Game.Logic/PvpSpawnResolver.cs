using System;
using System.Collections.Generic;
using System.Drawing;

namespace Game.Logic
{
    public static class PvpSpawnResolver
    {
        public static Point TakeSafeSpawn(List<Point> candidates, Func<Point, bool> isSafe, Func<int, int> nextIndex)
        {
            return TakeSafeSpawn(candidates, isSafe, nextIndex, null);
        }

        public static Point TakeSafeSpawn(List<Point> candidates, Func<Point, bool> isSafe,
            Func<int, int> nextIndex, HashSet<int> reservedSpawnX)
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
                if (reservedSpawnX != null && reservedSpawnX.Contains(point.X)) continue;
                if (isSafe(point))
                {
                    if (reservedSpawnX != null) reservedSpawnX.Add(point.X);
                    return point;
                }
            }
            return Point.Empty;
        }
    }
}
