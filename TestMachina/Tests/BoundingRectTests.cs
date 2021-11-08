using FluentAssertions;
using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public class BoundingRectTests
    {
        [Fact]
        public void center_to_bounds_with_no_starting_offset()
        {
            var actor = new Actor("Sammy Square", null);
            var startingPosition = new Vector2(300, 300);
            actor.transform.Position = startingPosition;
            var boundingRect = new BoundingRect(actor, new Point(32, 32));
            var startingCenter = boundingRect.Rect.Center;

            boundingRect.CenterToBounds();

            boundingRect.Rect.Center.Should().BeEquivalentTo(startingCenter); // Center should not have moved
            new Point(boundingRect.Rect.Top, boundingRect.Rect.Left).Should()
                .BeEquivalentTo(startingPosition.ToPoint());
        }

        [Fact]
        public void center_to_bounds_with_starting_offset()
        {
            var actor = new Actor("Rodney Rectangle", null);
            actor.transform.Position = new Vector2(300, 300);
            var boundingRect = new BoundingRect(actor, new Point(32, 64));
            boundingRect.SetOffset(new Vector2(20, 20));
            var startingCenter = boundingRect.Rect.Center;
            var startingTopLeft = new Point(boundingRect.Rect.Top, boundingRect.Rect.Left);

            boundingRect.CenterToBounds();

            boundingRect.Rect.Center.Should().BeEquivalentTo(startingCenter); // Center should not have moved
            new Point(boundingRect.Rect.Top, boundingRect.Rect.Left).Should().BeEquivalentTo(startingTopLeft);
        }
    }
}