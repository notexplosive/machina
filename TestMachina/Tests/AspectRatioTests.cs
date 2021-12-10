using FluentAssertions;
using Machina.Components;
using Machina.Data.Layout;
using Xunit;

namespace TestMachina.Tests
{
    public class AspectRatioTests
    {

        [Fact]
        public void tall_in_tall()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Tall, AspectRatio.Description.Tall, Orientation.Vertical)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Tall, AspectRatio.Description.Tall, Orientation.Horizontal)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Tall, AspectRatio.Description.Tall, Orientation.Vertical)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Tall, AspectRatio.Description.Tall, Orientation.Horizontal)
                .Should().BeTrue();
        }

        [Fact]
        public void wide_in_wide()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Wide, AspectRatio.Description.Wide, Orientation.Horizontal)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Wide, AspectRatio.Description.Wide, Orientation.Vertical)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Wide, AspectRatio.Description.Wide, Orientation.Horizontal)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Wide, AspectRatio.Description.Wide, Orientation.Vertical)
                .Should().BeTrue();
        }

        [Fact]
        public void square_in_square()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Square, AspectRatio.Description.Square, Orientation.Horizontal)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Square, AspectRatio.Description.Square, Orientation.Vertical)
                .Should().BeTrue();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Square, AspectRatio.Description.Square, Orientation.Horizontal)
                .Should().BeTrue();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Square, AspectRatio.Description.Square, Orientation.Vertical)
                .Should().BeTrue();
        }

        [Fact]
        public void square_in_tall()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Square, AspectRatio.Description.Tall, Orientation.Horizontal)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Square, AspectRatio.Description.Tall, Orientation.Vertical)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Square, AspectRatio.Description.Tall, Orientation.Horizontal)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Square, AspectRatio.Description.Tall, Orientation.Vertical)
                .Should().BeTrue();
        }

        [Fact]
        public void wide_in_tall()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Wide, AspectRatio.Description.Tall, Orientation.Horizontal)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Wide, AspectRatio.Description.Tall, Orientation.Vertical)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Wide, AspectRatio.Description.Tall, Orientation.Horizontal)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Wide, AspectRatio.Description.Tall, Orientation.Vertical)
                .Should().BeTrue();
        }

        [Fact]
        public void tall_in_wide()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Tall, AspectRatio.Description.Wide, Orientation.Vertical)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Tall, AspectRatio.Description.Wide, Orientation.Horizontal)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Tall, AspectRatio.Description.Wide, Orientation.Vertical)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Tall, AspectRatio.Description.Wide, Orientation.Horizontal)
                .Should().BeTrue();
        }

        [Fact]
        public void tall_in_square()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Tall, AspectRatio.Description.Square, Orientation.Vertical)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Tall, AspectRatio.Description.Square, Orientation.Horizontal)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Tall, AspectRatio.Description.Square, Orientation.Vertical)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Tall, AspectRatio.Description.Square, Orientation.Horizontal)
                .Should().BeTrue();
        }

        [Fact]
        public void wide_in_square()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Wide, AspectRatio.Description.Square, Orientation.Horizontal)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Wide, AspectRatio.Description.Square, Orientation.Vertical)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Wide, AspectRatio.Description.Square, Orientation.Horizontal)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Wide, AspectRatio.Description.Square, Orientation.Vertical)
                .Should().BeTrue();
        }

        [Fact]
        public void square_in_wide()
        {
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Square, AspectRatio.Description.Wide, Orientation.Vertical)
                .Should().BeTrue();
            AspectRatio.IsStretchedAlong(AspectRatio.Description.Square, AspectRatio.Description.Wide, Orientation.Horizontal)
                .Should().BeFalse();

            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Square, AspectRatio.Description.Wide, Orientation.Vertical)
                .Should().BeFalse();
            AspectRatio.IsStretchedPerpendicular(AspectRatio.Description.Square, AspectRatio.Description.Wide, Orientation.Horizontal)
                .Should().BeTrue();
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