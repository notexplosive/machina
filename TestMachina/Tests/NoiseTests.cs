using FluentAssertions;
using Machina.Data;
using Xunit;

namespace TestMachina.Tests
{
    public class NoiseTests
    {
        [Fact]
        public void next_double_returns_less_than_1()
        {
            var random = new NoiseBasedRNG(12345);

            for (var i = 0; i < 100; i++)
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

            for (var i = 0; i < 60; i++)
            {
                random.Next(i).Should().BeLessOrEqualTo(i);
            }
        }

        [Fact]
        public void next_int_range_sanity()
        {
            var random = new NoiseBasedRNG(12345);

            for (var i = 0; i < 60; i++)
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

            var array = new[] {1, 2, 3, 4, 5, 6, 7, 8, 9, 10, 11, 12, 13, 14, 15, 16};
            random.Shuffle(array);
            array.Should().ContainInOrder(14, 11, 4, 6, 7, 5, 10, 1, 2, 8, 16, 3, 15, 9, 12, 13);
        }
    }
}
