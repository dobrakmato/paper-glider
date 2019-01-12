using System;

namespace Level
{
    public static class Seeder
    {
        /// <summary>
        /// Return a seed (integer value) for the current day.
        /// </summary>
        /// <returns></returns>
        public static int ComputeSeed()
        {
            var dt = DateTime.UtcNow;
            var result = 17;

            result = result * 31 + dt.Year;
            result = result * 31 + dt.Month;
            result = result * 31 + dt.Day;

            return Hash(result);
        }

        private static int Hash(int x)
        {
            // https://stackoverflow.com/a/12996028/721809
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = ((x >> 16) ^ x) * 0x45d9f3b;
            x = (x >> 16) ^ x;
            return x;
        }
    }
}