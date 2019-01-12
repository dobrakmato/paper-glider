namespace Level
{
    /// <summary>
    /// To allow easier replacement of dirty Unity's random we
    /// use this anti-pattern utility class.
    /// </summary>
    public static class LevelRandom
    {
        private static readonly Random Random = new Random();

        public static void ResetBySeeder()
        {
            Random.ResetBySeeder();
        }

        public static int Range(int min, int max)
        {
            return Random.Range(min, max);
        }

        public static float Range(float min, float max)
        {
            return Random.Range(min, max);
        }
    }
}