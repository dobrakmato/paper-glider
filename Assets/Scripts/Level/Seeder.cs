using System;

namespace Level
{
    public class Seeder
    {
        public static int ComputeSeed()
        {
            var dt = DateTime.UtcNow;
            var result = 17;

            result = result * 31 + dt.Year;
            result = result * 31 + dt.Month;
            result = result * 31 + dt.Day;
            
            return result;
        }
    }
}