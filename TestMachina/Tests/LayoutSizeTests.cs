using FluentAssertions;
using Machina.Components;
using Machina.Data.Layout;
using Xunit;

namespace TestMachina.Tests
{
    public class LayoutSizeTests
    {
        [Fact]
        public void pixels()
        {
            var size = LayoutSize.Pixels(30, 50);

            size.IsFixedAspectRatio().Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Horizontal).Should().BeTrue();
            size.IsMeasurableAlong(Orientation.Vertical).Should().BeTrue();
            size.IsStretchedAlong(Orientation.Vertical).Should().BeFalse();
            size.IsStretchedPerpendicular(Orientation.Vertical).Should().BeFalse();
        }

        [Fact]
        public void square()
        {
            var size = LayoutSize.Square(30);

            size.IsFixedAspectRatio().Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Horizontal).Should().BeTrue();
            size.IsMeasurableAlong(Orientation.Vertical).Should().BeTrue();
            size.IsStretchedAlong(Orientation.Vertical).Should().BeFalse();
            size.IsStretchedPerpendicular(Orientation.Vertical).Should().BeFalse();
        }

        [Fact]
        public void stretch_both()
        {
            var size = LayoutSize.StretchedBoth();

            size.IsFixedAspectRatio().Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Horizontal).Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Vertical).Should().BeFalse();
            size.IsStretchedAlong(Orientation.Vertical).Should().BeTrue();
            size.IsStretchedPerpendicular(Orientation.Vertical).Should().BeTrue();
        }

        [Fact]
        public void stretch_vertically()
        {
            var size = LayoutSize.StretchedVertically(30);

            size.IsFixedAspectRatio().Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Horizontal).Should().BeTrue();
            size.IsMeasurableAlong(Orientation.Vertical).Should().BeFalse();
            size.IsStretchedAlong(Orientation.Vertical).Should().BeTrue();
            size.IsStretchedPerpendicular(Orientation.Vertical).Should().BeFalse();
        }

        [Fact]
        public void stretch_horizontally()
        {
            var size = LayoutSize.StretchedHorizontally(30);

            size.IsFixedAspectRatio().Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Horizontal).Should().BeFalse();
            size.IsMeasurableAlong(Orientation.Vertical).Should().BeTrue();
            size.IsStretchedAlong(Orientation.Vertical).Should().BeFalse();
            size.IsStretchedPerpendicular(Orientation.Vertical).Should().BeTrue();
        }
    }
}