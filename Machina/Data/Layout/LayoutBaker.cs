using Machina.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class LayoutBaker
    {
        public readonly Dictionary<ILayoutEdge, int> sizeLookupTable = new Dictionary<ILayoutEdge, int>();
        private readonly LayoutNode rootNode;

        public BakedLayout LayoutResult { get; } // todo, remove this

        public LayoutBaker(LayoutNode rootNode)
        {
            this.rootNode = rootNode;
            this.LayoutResult = new BakedLayout(new LayoutResultNode(Point.Zero, GetMeasuredSize(rootNode.Size), 0));
        }

        public int MeasureEdge(ILayoutEdge edge)
        {
            if (!edge.IsStretched)
            {
                return edge.ActualSize;
            }

            return sizeLookupTable[edge];
        }

        public Point GetMeasuredSize(LayoutSize size)
        {
            return new Point(MeasureEdge(size.X), MeasureEdge(size.Y));
        }

        public void AddLayoutNode(Point position, LayoutNode node, int nestingLevel)
        {
            if (node.Name.Exists)
            {
                this.LayoutResult.Add(node.Name.Text, new LayoutResultNode(position, GetMeasuredSize(node.Size), nestingLevel));
            }
        }

        public BakedLayout Bake()
        {
            return BakeAtLocation(Point.Zero);
        }

        public BakedLayout BakeAtLocation(Point startingLocation)
        {
            int nestingLevel = 0;
            AddLayoutNode(startingLocation, this.rootNode, nestingLevel);
            return BakeGroup(this.rootNode, startingLocation, nestingLevel);
        }

        private BakedLayout BakeGroup(LayoutNode currentNode, Point startingLocation, int parentNestingLevel)
        {
            var isVertical = currentNode.Orientation == Orientation.Vertical;
            var groupSize = GetMeasuredSize(currentNode.Size);
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? currentNode.Margin.Y : currentNode.Margin.X;

            var elements = currentNode.Children;
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var stretchAlong = new List<LayoutNode>();
            var stretchPerpendicular = new List<LayoutNode>();

            var last = elements.Length - 1;
            var index = 0;

            foreach (var element in elements)
            {
                if (!element.Size.IsStretchedAlong(currentNode.Orientation))
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

                if (element.Size.IsStretchedPerpendicular(currentNode.Orientation))
                {
                    stretchPerpendicular.Add(element);
                }

                if (index != last)
                {
                    remainingAlongSize -= currentNode.Padding;
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
                        sizeLookupTable[alongElement.Size.Y] = alongSizeOfEachStretchedElement;
                    }
                    else
                    {
                        sizeLookupTable[alongElement.Size.X] = alongSizeOfEachStretchedElement;
                    }
                }
            }

            // We're using the same value for all perpendicular stretches, maybe we can simplify this?
            var perpendicularStretchSize = isVertical ? groupSize.X - currentNode.Margin.X * 2 : groupSize.Y - currentNode.Margin.Y * 2;

            if (stretchPerpendicular.Count > 0)
            {
                foreach (var perpElement in stretchPerpendicular)
                {
                    if (isVertical)
                    {
                        sizeLookupTable[perpElement.Size.X] = perpendicularStretchSize;
                    }
                    else
                    {
                        sizeLookupTable[perpElement.Size.Y] = perpendicularStretchSize;
                    }
                }
            }

            int currentNestingLevel = parentNestingLevel + 1;

            // Place elements
            var nextLocation = startingLocation + new Point(currentNode.Margin.X, currentNode.Margin.Y);
            foreach (var element in elements)
            {
                var elementPosition = nextLocation;
                AddLayoutNode(elementPosition, element, currentNestingLevel);
                if (isVertical)
                {
                    nextLocation += new Point(0, MeasureEdge(element.Size.Y) + currentNode.Padding);
                }
                else
                {
                    nextLocation += new Point(MeasureEdge(element.Size.X) + currentNode.Padding, 0);
                }

                if (element.HasChildren)
                {
                    BakeGroup(element, elementPosition, currentNestingLevel);
                }
            }

            return LayoutResult;
        }
    }
}
