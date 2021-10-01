namespace Machina.ThirdParty
{
    /*
     *    Provides a noise function which is fast and had good distribution.
     *    
     *    Original algorithm by Squirrel Eiserloh,
     *    presented at "Math for Game Programmers: Noise-based RNG", Game Developers Conference, 2017.
     *    
     *    Adapted to C# by NotExplosive
     */
    public static class Squirrel3
    {
        private const uint NOISE1 = 0xB5297A4D; // 0110 1000 1110 0011 0001 1101 1010 0100
        private const uint NOISE2 = 0x68E31DA4; // 1011 0101 0010 1001 0111 1010 0100 1101
        private const uint NOISE3 = 0x1B56C4E9; // 0001 1011 0101 0110 1100 0100 1110 1001

        // These primes need to be orders of magnitude different
        private const uint PRIME1 = 198491317;
        private const uint PRIME2 = 6542989;

        public static uint Noise(int position, uint seed)
        {
            var mangledBits = (uint) position;
            mangledBits *= Squirrel3.NOISE1;
            mangledBits += seed;
            mangledBits ^= mangledBits >> 8;
            mangledBits += Squirrel3.NOISE2;
            mangledBits ^= mangledBits << 8;
            mangledBits *= Squirrel3.NOISE3;
            mangledBits ^= mangledBits >> 8;
            return mangledBits;
        }

        public static uint Noise2D(int x, int y, uint seed)
        {
            return Squirrel3.Noise(x + (int) (Squirrel3.PRIME1 * y), seed);
        }

        public static uint Noise3D(int x, int y, int z, uint seed)
        {
            return Squirrel3.Noise(x + (int) (Squirrel3.PRIME1 * y) + (int) (Squirrel3.PRIME2 * z), seed);
        }
    }
}
