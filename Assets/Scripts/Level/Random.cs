using System;

namespace Level
{
    /// <summary>
    /// Very good random class. Nobody will touch my precious instance of this Random.
    /// Not even Unity itself!
    ///
    /// Also it is based on Xorshift algorithm.
    /// </summary>
    public class Random
    {
        private uint _state;

        public void ResetBySeeder()
        {
            _state = (uint) Seeder.ComputeSeed();
        }

        private uint Next()
        {
            // https://en.wikipedia.org/wiki/Xorshift
            _state ^= _state << 13;
            _state ^= _state >> 17;
            _state ^= _state << 5;
            return _state;
        }

        private int NextInt()
        {
            return (int) Next();
        }

        private float NextFloat()
        {
            return Next() / (float) UInt32.MaxValue;
        }

        public int Range(int min, int max)
        {
            return min + (int) (Next() % (max - min));
        }

        public float Range(float min, float max)
        {
            return min + NextFloat() * (max - min);
        }
    }
}