using Machina.Components;
using Microsoft.Xna.Framework;
using System;
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
            return new Point(GetMeasuredEdge(size.Width), GetMeasuredEdge(size.Height));
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

        private void BakeGroup(BakedLayout inProgressLayout, LayoutNode parentNode, Point parentNodeLocation, int parentNestingLevel)
        {
            var isVertical = parentNode.Orientation == Orientation.Vertical;
            var groupSize = GetMeasuredSize(parentNode.Size);

            int remainingAlongSize = GetRemainingAlongSizeFromEasyNodes(parentNode, groupSize);

            var perpendicularStretchSize = isVertical ? groupSize.X - parentNode.Margin.X * 2 : groupSize.Y - parentNode.Margin.Y * 2;
            HandleStretchedNodes(parentNode, remainingAlongSize, perpendicularStretchSize);

            // handle fixed aspect ratio (todo: extract method)
            foreach (var child in parentNode.Children)
            {
                if (child.Size.IsFixedAspectRatio())
                {

                    // Right now both sides of FixedAspectRatio think they're streched, we need to figure out which one is actually stretched based on how much room is thinks it has
                    var aspectRatioOfAvailableSpace = new AspectRatio(GetMeasuredSize(child.Size));
                    var childAspectRatio = child.Size.GetAspectRatio();
                    var isStretchedAlong = AspectRatio.IsStretchedAlong(childAspectRatio, aspectRatioOfAvailableSpace, parentNode.Orientation);
                    var isStretchedPerpendicular = AspectRatio.IsStretchedPerpendicular(childAspectRatio, aspectRatioOfAvailableSpace, parentNode.Orientation);
                    var isStretchedBoth = isStretchedAlong && isStretchedPerpendicular;

                    var oppositeOrientation = OrientationUtils.Opposite(parentNode.Orientation); // todo: parentNode.OppositeOrientation

                    // If it's stretched both we do nothing because we already assume it's stretched on both sides
                    if (!isStretchedBoth)
                    {
                        if (isStretchedAlong)
                        {
                            var alongSize = GetMeasuredEdge(child.Size.GetValueFromOrientation(parentNode.Orientation));
                            this.sizeLookupTable[child.Size.GetValueFromOrientation(oppositeOrientation)] = (int) (alongSize * childAspectRatio.AlongOverPerpendicular(OrientationUtils.Opposite(parentNode.Orientation)));
                        }

                        if (isStretchedPerpendicular)
                        {
                            var perpendicularSize = GetMeasuredEdge(child.Size.GetValueFromOrientation(oppositeOrientation));
                            this.sizeLookupTable[child.Size.GetValueFromOrientation(parentNode.Orientation)] = (int) (perpendicularSize * childAspectRatio.AlongOverPerpendicular(parentNode.Orientation));
                        }
                    }
                }
            }

            // Place elements
            PlaceAndBakeMeasuredElements(inProgressLayout, parentNode, parentNodeLocation, parentNestingLevel);
        }

        private void PlaceAndBakeMeasuredElements(BakedLayout inProgressLayout, LayoutNode parentNode, Point parentNodeLocation, int parentNestingLevel)
        {
            int currentNestingLevel = parentNestingLevel + 1;
            Point totalUsedSpace = CalculateTotalUsedSpace(parentNode);

            var nextPosition = parentNodeLocation
                + parentNode.Alignment.GetRelativePositionOfElement(GetMeasuredSize(parentNode.Size), totalUsedSpace)
                + parentNode.Alignment.AddPostionDeltaFromMargin(parentNode.Margin)
                ;

            foreach (var child in parentNode.Children)
            {
                var childPosition = nextPosition;
                AddLayoutNode(inProgressLayout, childPosition, child, currentNestingLevel);
                var alongValue = GetMeasuredEdge(child.Size.GetValueFromOrientation(parentNode.Orientation)) + parentNode.Padding;

                nextPosition += OrientationUtils.GetPointForAlongNode(parentNode.Orientation, alongValue);

                if (child.HasChildren)
                {
                    BakeGroup(inProgressLayout, child, childPosition, currentNestingLevel);
                }
            }
        }

        private Point CalculateTotalUsedSpace(LayoutNode parentNode)
        {
            var totalUsedAlongSpace = 0;
            var totalUsedPerpendicularSpace = 0;

            foreach (var child in parentNode.Children)
            {
                totalUsedAlongSpace += GetMeasuredEdge(child.Size.GetValueFromOrientation(parentNode.Orientation));
                totalUsedAlongSpace += parentNode.Padding;
                totalUsedPerpendicularSpace = Math.Max(totalUsedPerpendicularSpace, GetMeasuredEdge(child.Size.GetValueFromOrientation(OrientationUtils.Opposite(parentNode.Orientation))));
            }

            // subtract 1 padding since the previous loop adds an extra (thanks foreach)
            totalUsedAlongSpace -= parentNode.Padding;

            return OrientationUtils.GetPointFromAlongPerpendicular(parentNode.Orientation, totalUsedAlongSpace, totalUsedPerpendicularSpace);
        }

        private static int GetRemainingAlongSizeFromEasyNodes(LayoutNode parentNode, Point groupSize)
        {
            var isVertical = parentNode.Orientation == Orientation.Vertical;
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? parentNode.Margin.Y : parentNode.Margin.X;
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var lastIndex = parentNode.Children.Length - 1;
            var index = 0;
            foreach (var child in parentNode.Children)
            {
                if (child.Size.IsMeasurableAlong(parentNode.Orientation))
                {
                    remainingAlongSize -= child.Size.GetValueFromOrientation(parentNode.Orientation).ActualSize;
                }

                if (index != lastIndex)
                {
                    remainingAlongSize -= parentNode.Padding;
                }

                index++;
            }

            return remainingAlongSize;
        }

        private void HandleStretchedNodes(LayoutNode parentNode, int remainingAlongSize, int perpendicularStretchSize)
        {
            int stretchAlongCount = 0;
            int stretchPerpendicularCount = 0;
            foreach (var child in parentNode.Children)
            {
                if (child.Size.IsStretchedAlong(parentNode.Orientation))
                {
                    stretchAlongCount++;
                }

                if (child.Size.IsStretchedPerpendicular(parentNode.Orientation))
                {
                    stretchPerpendicularCount++;
                }
            }

            // Update size of along stretch elements
            if (stretchAlongCount > 0)
            {
                var alongSizeOfEachStretchedChild = remainingAlongSize / stretchAlongCount;

                foreach (var child in parentNode.Children)
                {
                    if (child.Size.IsStretchedAlong(parentNode.Orientation))
                    {
                        this.sizeLookupTable[child.Size.GetValueFromOrientation(parentNode.Orientation)] = alongSizeOfEachStretchedChild;

                        // todo: we could do the fixed aspect stuff here, that way we don't need to look it up in the table later
                    }
                }
            }

            // Update perp elements (we can inline this in the first loop
            if (stretchPerpendicularCount > 0)
            {
                foreach (var child in parentNode.Children)
                {
                    if (child.Size.IsStretchedPerpendicular(parentNode.Orientation))
                    {
                        this.sizeLookupTable[child.Size.GetValueFromOrientation(OrientationUtils.Opposite(parentNode.Orientation))] = perpendicularStretchSize;
                    }
                }
            }
        }
    }
}
