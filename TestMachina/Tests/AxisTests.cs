using FluentAssertions;
using Machina.Data;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using Xunit;

namespace TestMachina.Tests
{
    public class AxisTests
    {
        private List<Point> AxisSpecificAction(Point rawAnchorPos, Point rawTargetPos)
        {
            var foundItems = new List<Point>();

            var anchorPos = new AxisPoint(rawAnchorPos);
            var targetPos = new AxisPoint(rawTargetPos);

            AxisUtils.DoForBothAxes((Axis axis) =>
            {
                if (anchorPos.OppositeAxisValue(axis) == targetPos.OppositeAxisValue(axis))
                {
                    int difference = anchorPos.AxisValue(axis) - targetPos.AxisValue(axis);
                    int normalizedDifference = Math.Sign(difference);
                    for (int i = targetPos.AxisValue(axis); i != targetPos.AxisValue(axis) + difference; i += normalizedDifference)
                    {
                        var z = i + normalizedDifference;
                        foundItems.Add(AxisUtils.CreatePoint(z, targetPos.OppositeAxisValue(axis), axis));
                    }
                }
            });

            return foundItems;
        }

        [Fact]
        public void axis_specific_same_x()
        {
            var result = AxisSpecificAction(new Point(4, 3), new Point(4, 0));

            result.Should().BeEquivalentTo(new List<Point> { new Point(4, 1), new Point(4, 2), new Point(4, 3) });
        }

        [Fact]
        public void axis_specific_same_y()
        {
            var result = AxisSpecificAction(new Point(4, 3), new Point(8, 3));

            result.Should().BeEquivalentTo(new List<Point> { new Point(7, 3), new Point(6, 3), new Point(5, 3), new Point(4, 3) });
        }

        [Fact]
        public void axis_specific_different_xy()
        {
            var result = AxisSpecificAction(new Point(4, 3), new Point(5, 7));

            result.Should().BeEquivalentTo(new List<Point> { });
        }
    }
}
