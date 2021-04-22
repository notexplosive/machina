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
    }
}
