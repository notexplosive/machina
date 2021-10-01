using System;
using System.Collections.Generic;
using System.Diagnostics;
using Machina.ThirdParty;

namespace Machina.Data
{
    /// <summary>
    ///     Random Number Generator based on a Noise function
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
            this.id = NoiseBasedRNG.idPool++;
        }

        public NoiseBasedRNG(uint seed, int x) : this(Squirrel3.Noise(x, seed))
        {
        }

        public NoiseBasedRNG(int seedX, int seedY) : this(Squirrel3.Noise(seedX, (uint) seedY))
        {
        }

        public NoiseBasedRNG(uint seed, int x, int y) : this(Squirrel3.Noise2D(x, y, seed))
        {
        }

        /// <summary>
        ///     Sets the seed from a string, if the string is parsable as an int (eg: "100") then it's parsed.
        ///     Otherwise we sum the bytes
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public static uint SeedFromString(string arg)
        {
            var canParse = uint.TryParse(arg, out var parsed);
            if (canParse)
            {
                return parsed;
            }

            var index = 0;
            var total = 0;
            foreach (var ch in arg)
            {
                total ^= ch << index;
                index++;
            }

            return (uint) total;
        }

        public int Next()
        {
            return (int) Squirrel3.Noise(this.currentPosition++, this.seed);
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
            return (float) NextDouble();
        }

        public double NextDouble()
        {
            var max = int.MaxValue / 2;
            return Next(max) / (double) max;
        }

        public void Shuffle<T>(IList<T> list)
        {
            var n = list.Count;
            while (n > 1)
            {
                n--;
                var k = Next(n + 1);
                var value = list[k];
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
