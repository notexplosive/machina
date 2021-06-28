using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Engine
{
    public class SeededRandom
    {
        private int seed_impl;

        public int Seed
        {
            get => this.seed_impl;
            set
            {
                Clean = new Random(value);
                MachinaGame.Print("seed set: ", value);
                this.seed_impl = value;
            }
        }
        public Random Clean
        {
            get; private set;
        }
        public Random Dirty
        {
            get;
        }

        public SeededRandom()
        {
            Seed = GenerateSeed();
            Dirty = new Random();
        }

        public static int GenerateSeed()
        {
            return (int) DateTime.Now.Ticks & 0x0000FFFF;
        }

        /// <summary>
        /// Sets the seed from a string, if the string is parsable as an int (eg: "100") then it's parsed.
        /// Otherwise we sum the bytes
        /// </summary>
        /// <param name="arg"></param>
        /// <returns></returns>
        public void SetSeedFromString(string arg)
        {
            bool canParse = int.TryParse(arg, out int parsed);
            if (canParse)
            {
                Seed = parsed;
            }
            else
            {
                int index = 0;
                int total = 0;
                foreach (var ch in arg)
                {
                    total += ch * index;
                    index++;
                }

                Seed = total;
            }
        }

        public void CleanShuffle<T>(IList<T> list)
        {
            Shuffle(list, Clean);
        }

        public void DirtyShuffle<T>(IList<T> list)
        {
            Shuffle(list, Dirty);
        }

        private void Shuffle<T>(IList<T> list, Random random)
        {
            int n = list.Count;
            while (n > 1)
            {
                n--;
                int k = random.Next(n + 1);
                T value = list[k];
                list[k] = list[n];
                list[n] = value;
            }
        }
    }
}
