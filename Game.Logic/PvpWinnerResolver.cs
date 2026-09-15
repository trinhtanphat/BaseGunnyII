namespace Game.Logic
{
    public static class PvpWinnerResolver
    {
        public static int Resolve(bool redAlive, bool blueAlive)
        {
            if (redAlive == blueAlive)
            {
                return 0;
            }

            return redAlive ? 1 : 2;
        }
    }
}