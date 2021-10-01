using Machina.Data;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public class UnorderedPairTest
    {
        [Fact]
        public void unordered_pair_pinning_test()
        {
            var pair1 = new UnorderedPair<Point>(new Point(0, 1), new Point(5, 5));
            var sameAsPair1 = new UnorderedPair<Point>(new Point(0, 1), new Point(5, 5));
            var p1 = new Point(5, 5);
            var p2 = new Point(0, 1);
            var pair1Flipped = new UnorderedPair<Point>(p1, p2);
            var pair2 = new UnorderedPair<Point>(new Point(0, -5), new Point(5, 5));

            Assert.True(pair1.Equals(pair1), "UnorderedPair.Equals works on self");
            Assert.True(pair1.Equals(sameAsPair1), "UnorderedPair.Equals works on identical set");
            Assert.True(pair1.Equals(pair1Flipped), "UnorderedPair.Equals works on flipped set");
            Assert.False(pair1.Equals(pair2), "UnorderedPair.Equals should be false for 2 different pairs");

            Assert.True(pair1 == sameAsPair1, "UnorderedPair == works on identical set");
            Assert.True(pair1 == pair1Flipped, "UnorderedPair == works on flipped set");
            Assert.False(pair1 == pair2, "UnorderedPair == should be false for 2 different pairs");
        }
    }
}
