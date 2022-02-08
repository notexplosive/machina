using System;

namespace Machina.Data.Layout
{
    public struct FlexLayoutStyle
    {
        public static FlexLayoutStyle Empty => new FlexLayoutStyle();

        public LayoutStyle InnerStyle { get; }
        public int? MinAlongSize { get; }

        public FlexLayoutStyle(LayoutStyle style = default, int? minAlongSize = null)
        {
            InnerStyle = style;
            MinAlongSize = minAlongSize;
        }
    }

    /// <summary>
    /// Flex layout has exactly enough room to fit. You feed it LayoutNode parameters and it creates a LayoutNode that is exactly fit for the children and style.
    /// </summary>
    public static class FlexLayout
    {
        public static LayoutNode HorizontalFlexParent(string name, FlexLayoutStyle style, params LayoutNode[] children)
        {
            return LayoutNode.HorizontalParent(name, FlexParentSize(Orientation.Horizontal, style, children), style.InnerStyle, children);
        }

        public static LayoutNode VerticalFlexParent(string name, FlexLayoutStyle style, params LayoutNode[] children)
        {
            return LayoutNode.VerticalParent(name, FlexParentSize(Orientation.Vertical, style, children), style.InnerStyle, children);
        }

        public static LayoutNode OrientedFlexParent(Orientation orientation, string name, FlexLayoutStyle style, params LayoutNode[] children)
        {
            if (orientation == Orientation.Vertical)
            {
                return VerticalFlexParent(name, style, children);
            }
            else
            {
                return HorizontalFlexParent(name, style, children);
            }
        }

        private static LayoutSize FlexParentSize(Orientation orientation, FlexLayoutStyle style, LayoutNode[] children)
        {
            var totalPadding = (children.Length - 1) * style.InnerStyle.Padding;
            var totalAlongMargin = style.InnerStyle.Margin.AxisValue(orientation.ToAxis()) * 2;
            var totalPerpendicularMargin = style.InnerStyle.Margin.AxisValue(orientation.Opposite().ToAxis()) * 2;

            var measuredAlongSize = 0;
            var largestPerpendicularSize = 0;

            foreach (var child in children)
            {
                measuredAlongSize += child.Size.GetValueFromOrientation(orientation).ActualSize;
                largestPerpendicularSize = Math.Max(largestPerpendicularSize, child.Size.GetValueFromOrientation(orientation.Opposite()).ActualSize);
            }

            var alongSize = measuredAlongSize + totalAlongMargin + totalPadding;
            if (style.MinAlongSize.HasValue)
            {
                alongSize = Math.Max(alongSize, style.MinAlongSize.Value);
            }

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
