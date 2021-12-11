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

        private void BakeGroup(BakedLayout inProgressLayout, LayoutNode parentNode, Point startingLocation, int parentNestingLevel)
        {
            var isVertical = parentNode.Orientation == Orientation.Vertical;
            var groupSize = GetMeasuredSize(parentNode.Size);

            int remainingAlongSize = GetRemainingAlongSizeFromEasyNodes(parentNode, groupSize);

            var perpendicularStretchSize = isVertical ? groupSize.X - parentNode.Margin.X * 2 : groupSize.Y - parentNode.Margin.Y * 2;
            HandleStretchedNodes(parentNode, remainingAlongSize, perpendicularStretchSize);

            // At this point, everything except the dependent edge of FixedAspect nodes have been measured.
            foreach (var child in parentNode.Children)
            {
                if (child.Size.IsFixedAspectRatio())
                {
                    var aspect = child.Size.GetAspectRatio();

                    // either X or Y of the FixedAspect has already been measured, we need to now calculate the other one
                    if (CanMeasureEdge(child.Size.Width))
                    {
                        var x = GetMeasuredEdge(child.Size.Width);
                        this.sizeLookupTable[child.Size.Height] = (int) (x * aspect.HeightOverWidth);
                    }
                    else
                    {
                        var y = GetMeasuredEdge(child.Size.Height);
                        this.sizeLookupTable[child.Size.Width] = (int) (y * aspect.WidthOverHeight);
                    }
                }
            }

            // Place elements
            int currentNestingLevel = parentNestingLevel + 1;
            var nextPosition = startingLocation + new Point(parentNode.Margin.X, parentNode.Margin.Y);
            foreach (var child in parentNode.Children)
            {
                var childPosition = nextPosition;
                AddLayoutNode(inProgressLayout, childPosition, child, currentNestingLevel);
                var alongValue = GetMeasuredEdge(child.Size.GetValueFromOrientation(parentNode.Orientation)) + parentNode.Padding;

                nextPosition += isVertical ? new Point(0, alongValue) : new Point(alongValue, 0);

                if (child.HasChildren)
                {
                    BakeGroup(inProgressLayout, child, childPosition, currentNestingLevel);
                }
            }
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
            var parentAspectRatio = new AspectRatio(GetMeasuredSize(parentNode.Size));
            foreach (var child in parentNode.Children)
            {
                if (child.Size.IsStretchedAlong(parentNode.Orientation, parentAspectRatio))
                {
                    stretchAlongCount++;
                }

                if (child.Size.IsStretchedPerpendicular(parentNode.Orientation, parentAspectRatio))
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
                    if (child.Size.IsStretchedAlong(parentNode.Orientation, parentAspectRatio))
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
                    if (child.Size.IsStretchedPerpendicular(parentNode.Orientation, parentAspectRatio))
                    {
                        this.sizeLookupTable[child.Size.GetValueFromOrientation(OrientationUtils.Opposite(parentNode.Orientation))] = perpendicularStretchSize;
                    }
                }
            }
        }
    }
}
