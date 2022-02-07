using System;

namespace Machina.Data.Layout
{
    /// <summary>
    /// Flex layout has exactly enough room to fit. You feed it LayoutNode parameters and it creates a LayoutNode that is exactly fit for the children and style.
    /// </summary>
    public static class FlexLayout
    {
        public static LayoutNode HorizontalFlexParent(string name, LayoutStyle style, params LayoutNode[] children)
        {
            return LayoutNode.HorizontalParent(name, FlexParentSize(Orientation.Horizontal, name, style, children), style, children);
        }

        public static LayoutNode VerticalFlexParent(string name, LayoutStyle style, params LayoutNode[] children)
        {
            return LayoutNode.VerticalParent(name, FlexParentSize(Orientation.Vertical, name, style, children), style, children);
        }

        private static LayoutSize FlexParentSize(Orientation orientation, string name, LayoutStyle style, LayoutNode[] children)
        {
            var totalPadding = children.Length * style.Padding;
            var totalAlongMargin = style.Margin.AxisValue(orientation.ToAxis()) * 2;
            var totalPerpendicularMargin = style.Margin.AxisValue(orientation.Opposite().ToAxis()) * 2;

            var measuredAlongSize = 0;
            var largestPerpendicularSize = 0;

            foreach (var child in children)
            {
                measuredAlongSize += child.Size.GetValueFromOrientation(orientation).ActualSize;
                largestPerpendicularSize = Math.Max(largestPerpendicularSize, child.Size.GetValueFromOrientation(orientation.Opposite()).ActualSize);
            }


            var alongSize = measuredAlongSize + totalAlongMargin + totalPadding;
            var perpendicularSize = largestPerpendicularSize + totalPerpendicularMargin;

            if (orientation == Orientation.Horizontal)
            {
                return LayoutSize.Pixels(alongSize, perpendicularSize);
            }
            else
            {
                return LayoutSize.Pixels(perpendicularSize, alongSize);
            }
        }
    }
}
