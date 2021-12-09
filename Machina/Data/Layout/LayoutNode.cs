using Machina.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data.Layout
{
    public class LayoutNode
    {
        private LayoutNode(LayoutNodeName name, LayoutSize size, Orientation orientation = Orientation.Horizontal, LayoutNode[] children = null, Point margin = default, int padding = 0)
        {
            Name = name;
            Size = size;
            Orientation = orientation;
            Margin = margin;
            Padding = padding;
            Children = children;
        }

        public static LayoutNode StretchedSpacer()
        {
            return new LayoutNode(LayoutNodeName.Nameless, LayoutSize.StretchedBoth());
        }

        public static LayoutNode Spacer(int size)
        {
            return new LayoutNode(LayoutNodeName.Nameless, LayoutSize.Square(size));
        }

        public static LayoutNode Leaf(string name, LayoutSize size)
        {
            return new LayoutNode(name, size);
        }

        public static LayoutNode VerticalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new LayoutNode(name, size, Orientation.Vertical, margin: style.Margin, padding: style.Padding, children: children);
        }

        public static LayoutNode HorizontalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new LayoutNode(name, size, Orientation.Horizontal, margin: style.Margin, padding: style.Padding, children: children);
        }

        public LayoutResult Build()
        {
            return Build(Point.Zero);
        }

        public LayoutResult Build(Point startingLocation)
        {
            var layoutIntermediate = new LayoutIntermediate(this);
            int nestingLevel = 0;
            layoutIntermediate.AddLayoutNode(startingLocation, this, nestingLevel);
            return Build(layoutIntermediate, startingLocation, nestingLevel);
        }

        private LayoutResult Build(LayoutIntermediate layoutIntermediate, Point startingLocation, int parentNestingLevel)
        {
            var isVertical = Orientation == Orientation.Vertical;
            var groupSize = layoutIntermediate.GetMeasuredSize(Size);
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? Margin.Y : Margin.X;

            var elements = Children;
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var stretchAlong = new List<LayoutNode>();
            var stretchPerpendicular = new List<LayoutNode>();

            var last = elements.Length - 1;
            var index = 0;

            foreach (var element in elements)
            {
                if (!element.Size.IsStretchedAlong(Orientation))
                {
                    if (isVertical)
                    {
                        remainingAlongSize -= element.Size.Y.ActualSize;
                    }
                    else
                    {
                        remainingAlongSize -= element.Size.X.ActualSize;
                    }
                }
                else
                {
                    stretchAlong.Add(element);
                }

                if (element.Size.IsStretchedPerpendicular(Orientation))
                {
                    stretchPerpendicular.Add(element);
                }

                if (index != last)
                {
                    remainingAlongSize -= Padding;
                }

                index++;
            }

            // Update size of stretch elements
            if (stretchAlong.Count > 0)
            {
                var alongSizeOfEachStretchedElement = remainingAlongSize / stretchAlong.Count;

                foreach (var alongElement in stretchAlong)
                {
                    if (isVertical)
                    {
                        layoutIntermediate.sizeLookupTable[alongElement.Size.Y] = alongSizeOfEachStretchedElement;
                    }
                    else
                    {
                        layoutIntermediate.sizeLookupTable[alongElement.Size.X] = alongSizeOfEachStretchedElement;
                    }
                }
            }

            // We're using the same value for all perpendicular stretches, maybe we can simplify this?
            var perpendicularStretchSize = isVertical ? groupSize.X - Margin.X * 2 : groupSize.Y - Margin.Y * 2;

            if (stretchPerpendicular.Count > 0)
            {
                foreach (var perpElement in stretchPerpendicular)
                {
                    if (isVertical)
                    {
                        layoutIntermediate.sizeLookupTable[perpElement.Size.X] = perpendicularStretchSize;
                    }
                    else
                    {
                        layoutIntermediate.sizeLookupTable[perpElement.Size.Y] = perpendicularStretchSize;
                    }
                }
            }

            int myNestingLevel = parentNestingLevel + 1;

            // Place elements
            var nextLocation = startingLocation + new Point(Margin.X, Margin.Y);
            foreach (var element in elements)
            {
                var elementPosition = nextLocation;
                layoutIntermediate.AddLayoutNode(elementPosition, element, myNestingLevel);
                if (isVertical)
                {
                    nextLocation += new Point(0, layoutIntermediate.MeasureEdge(element.Size.Y) + Padding);
                }
                else
                {
                    nextLocation += new Point(layoutIntermediate.MeasureEdge(element.Size.X) + Padding, 0);
                }

                if (element.HasChildren)
                {
                    element.Build(layoutIntermediate, elementPosition, myNestingLevel);
                }
            }

            return layoutIntermediate.LayoutResult;
        }

        private readonly LayoutNode[] Children;
        private bool HasChildren => Children != null;
        public LayoutNodeName Name { get; }
        public LayoutSize Size { get; }
        public Orientation Orientation { get; }
        public Point Margin { get; }
        public int Padding { get; }

        /// <summary>
        /// Returns a LayoutNode just like this one
        /// </summary>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public LayoutNode GetResized(LayoutSize newSize)
        {
            return new LayoutNode(Name, newSize, Orientation, Children, Margin, Padding);
        }

        public override string ToString()
        {
            var childCount = HasChildren ? Children.Length : 0;
            return $"{Name}, {Size}, {childCount} children";
        }
    }
}
