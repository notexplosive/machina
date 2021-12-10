using Machina.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    internal class LayoutBaker
    {
        public readonly Dictionary<ILayoutEdge, int> sizeLookupTable = new Dictionary<ILayoutEdge, int>();
        private readonly LayoutNode rootNode;

        public LayoutBaker(LayoutNode rootNode)
        {
            this.rootNode = rootNode;
        }

        public int GetMeasuredEdge(ILayoutEdge edge)
        {
            if (edge.IsConstant)
            {
                return edge.ActualSize;
            }

            return this.sizeLookupTable[edge];
        }

        public bool CanMeasureEdge(ILayoutEdge edge)
        {
            return this.sizeLookupTable.ContainsKey(edge);
        }

        public Point GetMeasuredSize(LayoutSize size)
        {
            return new Point(GetMeasuredEdge(size.X), GetMeasuredEdge(size.Y));
        }

        public void AddLayoutNode(BakedLayout inProgressLayout, Point position, LayoutNode node, int nestingLevel)
        {
            if (node.Name.Exists)
            {
                inProgressLayout.Add(node.Name.Text, new NodePositionAndSize(position, GetMeasuredSize(node.Size), nestingLevel));
            }
        }

        public BakedLayout Bake()
        {
            var bakedLayout = new BakedLayout(this.rootNode);
            BakeAtLocation(bakedLayout, Point.Zero);
            return bakedLayout;
        }

        public void BakeAtLocation(BakedLayout inProgressLayout, Point startingLocation)
        {
            int nestingLevel = 0;
            AddLayoutNode(inProgressLayout, startingLocation, this.rootNode, nestingLevel);
            BakeGroup(inProgressLayout, this.rootNode, startingLocation, nestingLevel);
        }

        private void BakeGroup(BakedLayout inProgressLayout, LayoutNode currentNode, Point startingLocation, int parentNestingLevel)
        {
            var isVertical = currentNode.Orientation == Orientation.Vertical;
            var groupSize = GetMeasuredSize(currentNode.Size);

            int remainingAlongSize = GetRemainingAlongSizeFromEasyNodes(currentNode, groupSize);

            var perpendicularStretchSize = isVertical ? groupSize.X - currentNode.Margin.X * 2 : groupSize.Y - currentNode.Margin.Y * 2;
            HandleStretchedNodes(currentNode, remainingAlongSize, perpendicularStretchSize);

            // At this point, everything except the dependent edge of FixedAspect nodes have been measured.
            foreach (var element in currentNode.Children)
            {
                if (element.Size.IsFixedAspectRatio())
                {
                    var aspect = element.Size.GetAspectRatio();

                    // either X or Y of the FixedAspect has already been measured, we need to now calculate the other one
                    if (CanMeasureEdge(element.Size.X))
                    {
                        var x = GetMeasuredEdge(element.Size.X);
                        this.sizeLookupTable[element.Size.Y] = (int) (x * aspect.HeightOverWidth);
                    }
                    else
                    {
                        var y = GetMeasuredEdge(element.Size.Y);
                        this.sizeLookupTable[element.Size.X] = (int) (y * aspect.WidthOverHeight);
                    }
                }
            }

            // Place elements
            int currentNestingLevel = parentNestingLevel + 1;
            var nextLocation = startingLocation + new Point(currentNode.Margin.X, currentNode.Margin.Y);
            foreach (var element in currentNode.Children)
            {
                var elementPosition = nextLocation;
                AddLayoutNode(inProgressLayout, elementPosition, element, currentNestingLevel);
                var alongValue = GetMeasuredEdge(element.Size.GetValueFromOrientation(currentNode.Orientation)) + currentNode.Padding;

                nextLocation += isVertical ? new Point(0, alongValue) : new Point(alongValue, 0);

                if (element.HasChildren)
                {
                    BakeGroup(inProgressLayout, element, elementPosition, currentNestingLevel);
                }
            }
        }

        private static int GetRemainingAlongSizeFromEasyNodes(LayoutNode currentNode, Point groupSize)
        {
            var isVertical = currentNode.Orientation == Orientation.Vertical;
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? currentNode.Margin.Y : currentNode.Margin.X;
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var lastIndex = currentNode.Children.Length - 1;
            var index = 0;
            foreach (var element in currentNode.Children)
            {
                if (element.Size.IsMeasurableAlong(currentNode.Orientation))
                {
                    remainingAlongSize -= element.Size.GetValueFromOrientation(currentNode.Orientation).ActualSize;
                }

                if (index != lastIndex)
                {
                    remainingAlongSize -= currentNode.Padding;
                }

                index++;
            }

            return remainingAlongSize;
        }

        private void HandleStretchedNodes(LayoutNode currentNode, int remainingAlongSize, int perpendicularStretchSize)
        {
            int stretchAlongCount = 0;
            int stretchPerpendicularCount = 0;
            var parentAspectRatio = new AspectRatio(GetMeasuredSize(currentNode.Size));
            foreach (var element in currentNode.Children)
            {
                if (element.Size.IsStretchedAlong(currentNode.Orientation, parentAspectRatio))
                {
                    stretchAlongCount++;
                }

                if (element.Size.IsStretchedPerpendicular(currentNode.Orientation, parentAspectRatio))
                {
                    stretchPerpendicularCount++;
                }
            }

            // Update size of along stretch elements
            if (stretchAlongCount > 0)
            {
                var alongSizeOfEachStretchedElement = remainingAlongSize / stretchAlongCount;

                foreach (var element in currentNode.Children)
                {
                    if (element.Size.IsStretchedAlong(currentNode.Orientation, parentAspectRatio))
                    {
                        this.sizeLookupTable[element.Size.GetValueFromOrientation(currentNode.Orientation)] = alongSizeOfEachStretchedElement;

                        // todo: we could do the fixed aspect stuff here, that way we don't need to look it up in the table later
                    }
                }
            }

            // Update perp elements (we can inline this in the first loop
            if (stretchPerpendicularCount > 0)
            {
                foreach (var element in currentNode.Children)
                {
                    if (element.Size.IsStretchedPerpendicular(currentNode.Orientation, parentAspectRatio))
                    {
                        this.sizeLookupTable[element.Size.GetValueFromOrientation(OrientationUtils.Opposite(currentNode.Orientation))] = perpendicularStretchSize;
                    }
                }
            }
        }
    }
}
