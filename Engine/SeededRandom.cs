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
                this.CleanRandom = new Random(value);
                MachinaGame.Print("seed set: ", value);
                this.seed_impl = value;
            }
        }
        public Random CleanRandom
        {
            get; private set;
        }
        public Random DirtyRandom
        {
            get;
        }

        public SeededRandom()
        {
            this.Seed = GenerateSeed();
            this.DirtyRandom = new Random();
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
    }
}
