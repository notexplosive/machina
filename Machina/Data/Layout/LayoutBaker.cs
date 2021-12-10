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

        public int MeasureEdge(ILayoutEdge edge)
        {
            if (!edge.IsStretched)
            {
                return edge.ActualSize;
            }

            return this.sizeLookupTable[edge];
        }

        public Point GetMeasuredSize(LayoutSize size)
        {
            return new Point(MeasureEdge(size.X), MeasureEdge(size.Y));
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

            int remainingAlongSize = GetRemainingAlongSizeFromEasyNodes(currentNode, isVertical, groupSize);

            var perpendicularStretchSize = isVertical ? groupSize.X - currentNode.Margin.X * 2 : groupSize.Y - currentNode.Margin.Y * 2;
            HandleStretchedNodes(currentNode, remainingAlongSize, perpendicularStretchSize);

            int currentNestingLevel = parentNestingLevel + 1;

            // Place elements
            var nextLocation = startingLocation + new Point(currentNode.Margin.X, currentNode.Margin.Y);
            foreach (var element in currentNode.Children)
            {
                var elementPosition = nextLocation;
                AddLayoutNode(inProgressLayout, elementPosition, element, currentNestingLevel);
                var alongValue = MeasureEdge(element.Size.GetValueFromOrientation(currentNode.Orientation)) + currentNode.Padding;

                nextLocation += isVertical ? new Point(0, alongValue) : new Point(alongValue, 0);

                if (element.HasChildren)
                {
                    BakeGroup(inProgressLayout, element, elementPosition, currentNestingLevel);
                }
            }
        }

        private static int GetRemainingAlongSizeFromEasyNodes(LayoutNode currentNode, bool isVertical, Point groupSize)
        {
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
            foreach (var element in currentNode.Children)
            {
                if (element.Size.IsStretchedAlong(currentNode.Orientation))
                {
                    stretchAlongCount++;
                }

                if (element.Size.IsStretchedPerpendicular(currentNode.Orientation))
                {
                    stretchPerpendicularCount++;
                }
            }

            // Update size of stretch elements
            if (stretchAlongCount > 0)
            {
                var alongSizeOfEachStretchedElement = remainingAlongSize / stretchAlongCount;

                foreach (var element in currentNode.Children)
                {
                    if (element.Size.IsStretchedAlong(currentNode.Orientation))
                    {
                        this.sizeLookupTable[element.Size.GetValueFromOrientation(currentNode.Orientation)] = alongSizeOfEachStretchedElement;
                    }
                }
            }

            if (stretchPerpendicularCount > 0)
            {
                foreach (var element in currentNode.Children)
                {
                    if (element.Size.IsStretchedPerpendicular(currentNode.Orientation))
                    {
                        this.sizeLookupTable[element.Size.GetValueFromOrientation(OrientationUtils.Opposite(currentNode.Orientation))] = perpendicularStretchSize;
                    }
                }
            }
        }
    }
}
