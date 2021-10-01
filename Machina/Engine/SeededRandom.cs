using System;
using System.Collections.Generic;
using Machina.Data;

namespace Machina.Engine
{
    public class SeededRandom
    {
        private int seed_impl;

        public SeededRandom()
        {
            Seed = SeededRandom.GenerateSeed();
            Dirty = new NoiseBasedRNG((uint) SeededRandom.GenerateSeed());
        }

        public int Seed
        {
            get => this.seed_impl;
            set
            {
                Clean = new NoiseBasedRNG((uint) value);
                MachinaGame.Print("seed set: ", value);
                this.seed_impl = value;
            }
        }

        public NoiseBasedRNG Clean { get; private set; }

        public NoiseBasedRNG Dirty { get; }

        public static int GenerateSeed()
        {
            return (int) DateTime.Now.Ticks & 0x0000FFFF;
        }

        public void CleanShuffle<T>(IList<T> list)
        {
            Clean.Shuffle(list);
        }

        public void DirtyShuffle<T>(IList<T> list)
        {
            Dirty.Shuffle(list);
        }
    }
}
