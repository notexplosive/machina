using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.Layout
{
    internal class LayoutBaker
    {
        private readonly LayoutNode rootNode;
        private readonly LayoutMeasurer measurer;

        public LayoutBaker(LayoutNode rootNode)
        {
            this.rootNode = rootNode;
            this.measurer = new LayoutMeasurer();
        }

        public void AddNodeToLayout(BakedLayout inProgressLayout, Point position, LayoutNode node, int nestingLevel)
        {
            if (node.Name.Exists)
            {
                inProgressLayout.Add(node.Name.Text, new BakedLayoutNode(position, this.measurer.GetMeasuredSize(node.Size), nestingLevel));
            }
        }

        public BakedLayout Bake()
        {
            if (!this.rootNode.Size.IsMeasurableAlong(Orientation.Horizontal) || !this.rootNode.Size.IsMeasurableAlong(Orientation.Vertical))
            {
                throw new ImpossibleLayoutException("Root node is not a constant size");
            }

            var bakedLayout = new BakedLayout(this.rootNode);
            BakeAtLocation(bakedLayout, Point.Zero);
            return bakedLayout;
        }

        public void BakeAtLocation(BakedLayout inProgressLayout, Point startingLocation)
        {
            int nestingLevel = 0;
            AddNodeToLayout(inProgressLayout, startingLocation, this.rootNode, nestingLevel);
            BakeGroup(inProgressLayout, this.rootNode, startingLocation, nestingLevel);
        }

        private void BakeGroup(BakedLayout inProgressLayout, LayoutNode parentNode, Point parentNodeLocation, int parentNestingLevel)
        {
            var isVertical = parentNode.Orientation == Orientation.Vertical;
            var groupSize = this.measurer.GetMeasuredSize(parentNode.Size);

            int remainingAlongSize = GetRemainingAlongSizeFromEasyNodes(parentNode, groupSize);

            var perpendicularStretchSize = isVertical ? groupSize.X - parentNode.Margin.X * 2 : groupSize.Y - parentNode.Margin.Y * 2;
            HandleStretchedNodes(parentNode, remainingAlongSize, perpendicularStretchSize);

            // Place elements
            PlaceAndBakeMeasuredElements(inProgressLayout, parentNode, parentNodeLocation, parentNestingLevel + 1);
        }

        private void PlaceAndBakeMeasuredElements(BakedLayout inProgressLayout, LayoutNode parentNode, Point parentNodeLocation, int currentNestingLevel)
        {
            var nextPosition = parentNodeLocation
                + parentNode.Alignment.GetRelativePositionOfElement(this.measurer.GetMeasuredSize(parentNode.Size), CalculateTotalUsedSpace(parentNode))
                + parentNode.Alignment.AddPostionDeltaFromMargin(parentNode.Margin)
                ;

            foreach (var child in parentNode.Children)
            {
                var childPosition = nextPosition;
                AddNodeToLayout(inProgressLayout, childPosition, child, currentNestingLevel);

                nextPosition += parentNode.Orientation.GetPointForAlongAxis(this.measurer.MeasureEdgeOfNode(child, parentNode.Orientation) + parentNode.Padding);

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
                totalUsedAlongSpace += this.measurer.MeasureEdgeOfNode(child, parentNode.Orientation);
                totalUsedAlongSpace += parentNode.Padding;
                totalUsedPerpendicularSpace = Math.Max(totalUsedPerpendicularSpace, this.measurer.MeasureEdgeOfNode(child, parentNode.Orientation.Opposite()));
            }

            // subtract 1 padding since the previous loop adds an extra (thanks foreach)
            totalUsedAlongSpace -= parentNode.Padding;

            return parentNode.Orientation.GetPointFromAlongPerpendicular(totalUsedAlongSpace, totalUsedPerpendicularSpace);
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
                var fractionalLossIncrement = (float) remainingAlongSize / stretchAlongCount % 1;
                var fractionalLoss = 0f;

                foreach (var child in parentNode.Children)
                {
                    if (child.Size.IsStretchedAlong(parentNode.Orientation))
                    {
                        fractionalLoss += fractionalLossIncrement;
                        int extraPixelIfApplicable = 0;
                        var epsilon = 0.001f;
                        if (1 - fractionalLoss < epsilon)
                        {
                            extraPixelIfApplicable = 1;
                            fractionalLoss -= 1;
                        }

                        this.measurer.Add(child.Size.GetValueFromOrientation(parentNode.Orientation), alongSizeOfEachStretchedChild + extraPixelIfApplicable);
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
                        this.measurer.Add(child.Size.GetValueFromOrientation(parentNode.Orientation.Opposite()), perpendicularStretchSize);
                        HandleFixedAspectRatio(parentNode, child);
                    }
                }
            }
        }

        private void HandleFixedAspectRatio(LayoutNode parentNode, LayoutNode child)
        {
            if (child.Size.IsFixedAspectRatio())
            {

                // Right now both sides of FixedAspectRatio think they're streched, we need to figure out which one is actually stretched based on how much room is thinks it has
                var aspectRatioOfAvailableSpace = new AspectRatio(this.measurer.GetMeasuredSize(child.Size));
                var childAspectRatio = child.Size.GetAspectRatio();
                var isStretchedAlong = AspectRatio.IsStretchedAlong(childAspectRatio, aspectRatioOfAvailableSpace, parentNode.Orientation);
                var isStretchedPerpendicular = AspectRatio.IsStretchedPerpendicular(childAspectRatio, aspectRatioOfAvailableSpace, parentNode.Orientation);
                var isStretchedBoth = isStretchedAlong && isStretchedPerpendicular;

                var oppositeOrientation = parentNode.Orientation.Opposite();

                // If it's stretched both we do nothing because we already assume it's stretched on both sides
                if (!isStretchedBoth)
                {
                    if (isStretchedAlong)
                    {
                        var alongSize = this.measurer.MeasureEdgeOfNode(child, parentNode.Orientation);
                        this.measurer.Add(child.Size.GetValueFromOrientation(oppositeOrientation), (int) (alongSize * childAspectRatio.AlongOverPerpendicular(oppositeOrientation)));
                    }

                    if (isStretchedPerpendicular)
                    {
                        var perpendicularSize = this.measurer.MeasureEdgeOfNode(child, oppositeOrientation);
                        this.measurer.Add(child.Size.GetValueFromOrientation(parentNode.Orientation), (int) (perpendicularSize * childAspectRatio.AlongOverPerpendicular(parentNode.Orientation)));
                    }
                }
            }
        }
    }
}
