using FluentAssertions;
using Machina.Components;
using Machina.Data.Layout;
using Xunit;

namespace TestMachina.Tests
{
    public class AspectRatioTests
    {
        private void TestStretchInNestedAspectRatios(AspectRatio inner, AspectRatio outer, bool shouldBeStretchedHorizontal, bool shouldBeStretchedVertical)
        {
            AspectRatio.IsStretchedAlong(inner, outer, Orientation.Horizontal)
                .Should().Be(shouldBeStretchedHorizontal);

            AspectRatio.IsStretchedAlong(inner, outer, Orientation.Vertical)
                .Should().Be(shouldBeStretchedVertical);
        }

        [Fact]
        public void test_16x9_in_16x9()
        {
            TestStretchInNestedAspectRatios(new AspectRatio(16, 9), new AspectRatio(16, 9), shouldBeStretchedHorizontal: true, shouldBeStretchedVertical: true);
        }

        [Fact]
        public void test_16x9_in_16x10()
        {
            TestStretchInNestedAspectRatios(new AspectRatio(16, 9), new AspectRatio(16, 10), shouldBeStretchedHorizontal: true, shouldBeStretchedVertical: false);
        }

        [Fact]
        public void test_16x9_in_16x8()
        {
            TestStretchInNestedAspectRatios(new AspectRatio(16, 9), new AspectRatio(16, 8), shouldBeStretchedHorizontal: false, shouldBeStretchedVertical: true);
        }

        [Fact]
        public void test_16x9_in_17x9()
        {
            TestStretchInNestedAspectRatios(new AspectRatio(16, 9), new AspectRatio(17, 9), shouldBeStretchedHorizontal: false, shouldBeStretchedVertical: true);
        }

        [Fact]
        public void test_9x16_in_9x17()
        {
            TestStretchInNestedAspectRatios(new AspectRatio(9, 16), new AspectRatio(9, 17), shouldBeStretchedHorizontal: true, shouldBeStretchedVertical: false);
        }

        [Fact]
        public void basic_descriptors()
        {
            new AspectRatio(8, 8).Describe().Should().Be(AspectRatio.Description.Square);
            new AspectRatio(4, 3).Describe().Should().Be(AspectRatio.Description.Wide);
            new AspectRatio(16, 9).Describe().Should().Be(AspectRatio.Description.Wide);
            new AspectRatio(9, 16).Describe().Should().Be(AspectRatio.Description.Tall);
        }

        [Fact]
        public void invalid_input()
        {
            var subject = new AspectRatio(-5, -2);
            subject.IsSquare().Should().BeTrue();
        }
    }
}