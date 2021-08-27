using FluentAssertions;
using Machina.Data;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestMachina.Tests
{
    public class NoiseTests
    {
        [Fact]
        public void next_double_returns_less_than_1()
        {
            var random = new NoiseBasedRNG(12345);

            for (int i = 0; i < 100; i++)
            {
                var result = random.NextDouble();
                result.Should().BeGreaterOrEqualTo(0.0f);
                result.Should().BeLessOrEqualTo(1.0f);
            }
        }

        [Fact]
        public void next_int_sanity()
        {
            var random = new NoiseBasedRNG(12345);

            for (int i = 0; i < 60; i++)
                random.Next(i).Should().BeLessOrEqualTo(i);
        }

        [Fact]
        public void next_int_range_sanity()
        {
            var random = new NoiseBasedRNG(12345);

            for (int i = 0; i < 60; i++)
            {
                var randomResult = random.Next(i, i + 60);
                randomResult.Should().BeLessOrEqualTo(i + 60);
                randomResult.Should().BeGreaterOrEqualTo(i);
            }
        }

        [Fact]
        public void shuffle_sanity()
        {
            var random = new NoiseBasedRNG(12345);

            var array = new int[] { 1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16 };
            random.Shuffle(array);
            array.Should().ContainInOrder(14, 11, 4, 6, 7, 5, 10, 1, 2, 8, 16, 3, 15, 9, 12, 13);
        }

        [Fact]
        public void create_seed_from_two_small_numbers()
        {
            ushort x = 0xa6;
            ushort y = 0xb8;

            var seed = NoiseBasedRNG.CreateSeedFromTwoNumbers(x, y);

            seed.Should().Be(0x00_a6_00_b8);
        }

        [Fact]
        public void create_seed_from_two_medium_numbers()
        {
            ushort x = 0xf4a6;
            ushort y = 0xeeb8;

            var seed = NoiseBasedRNG.CreateSeedFromTwoNumbers(x, y);

            seed.Should().Be(0xf4_a6_ee_b8);
        }

        [Fact]
        public void create_seed_from_large_numbers()
        {
            // Demonstrates that my 2-number seed algorithm is kinda bad. I think there was a GDC talk about this problem
            uint x = 0xde_f4a6;
            uint y = 0xab_eeb8;
            // x's 0xde and y's 0xab get truncated out. This is a problem

            var seed = NoiseBasedRNG.CreateSeedFromTwoNumbers((ushort)x, (ushort)y);

            seed.Should().Be(0xf4_a6_ee_b8);
        }
    }
}
