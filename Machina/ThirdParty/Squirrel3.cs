using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.ThirdParty
{
    /*
     *    Provides a noise function which is fast and had good distribution.
     *    
     *    Original algorithm by Squirrel Eiserloh, presented at "Math for Game Programmers: Noise-based RNG", Game Developers Conference, 2017.
     *    
     *    Adapted to C# by NotExplosive
     */
    public static class Squirrel3
    {
        private static readonly uint NOISE1 = 0xB5297A4D;  // 0110 1000 1110 0011 0001 1101 1010 0100
        private static readonly uint NOISE2 = 0x68E31DA4;  // 1011 0101 0010 1001 0111 1010 0100 1101
        private static readonly uint NOISE3 = 0x1B56C4E9;  // 0001 1011 0101 0110 1100 0100 1110 1001

        public static uint Noise(int position, uint seed)
        {
            uint mangledBits = (uint)position;
            mangledBits *= NOISE1;
            mangledBits += seed;
            mangledBits ^= (mangledBits >> 8);
            mangledBits += NOISE2;
            mangledBits ^= (mangledBits << 8);
            mangledBits *= NOISE3;
            mangledBits ^= (mangledBits >> 8);
            return mangledBits;
        }
    }
}
