using Machina.ThirdParty;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// Random Number Generator based on a Noise function
    /// </summary>
    public class NoiseBasedRNG
    {
        private static int idPool;
        private readonly int id;
        private readonly uint seed;
        private int currentPosition;

        public NoiseBasedRNG(uint seed)
        {
            this.seed = seed;
            this.currentPosition = 0;
            this.id = idPool++;
        }

        public NoiseBasedRNG(int seedX, int seedY) : this(Squirrel3.Noise(seedX, (uint)seedY))
        {
        }

        public NoiseBasedRNG(uint seed, int x, int y) : this(Squirrel3.Noise2D(x, y, seed))
        {

        }

        public int Next()
        {
            return (int)Squirrel3.Noise(this.currentPosition++, this.seed);
        }

        public int Next(int maximum)
        {
            if (maximum == 0)
            {
                return 0;
            }
            return Math.Abs(Next()) % maximum;
        }

        public int Next(int minimum, int maximum)
        {
            Debug.Assert(minimum < maximum);
            return Next(maximum - minimum) + minimum;
        }

        public bool NextBool()
        {
            return Next() % 2 == 0;
        }

        public float NextFloat()
        {
            return (float)NextDouble();
        }

        public double NextDouble()
        {
            var max = int.MaxValue / 2;
            return Next(max) / (double)max;
        }

        public void Shuffle<T>(IList<T> list)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }

        public float NextRadian()
        {
            return NextFloat() * MathF.PI * 2;
        }

        public override string ToString()
        {
            return "[" + this.id + "]" + " seed " + this.seed + " at " + this.currentPosition;
        }
    }
}
