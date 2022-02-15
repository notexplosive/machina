using Microsoft.Xna.Framework;
using System;
using System.Collections;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class BakedFlowLayout : IBakedLayout
    {
        private readonly BakedLayout bakedLayout;
        private readonly RawFlowLayout rawFlowLayout;
        private readonly BakedRow[] rows;

        public Point UsedSpace { get; }

        public LayoutNode OriginalRoot => this.bakedLayout.OriginalRoot;

        public BakedFlowLayout(BakedLayout bakedLayout, RawFlowLayout rawFlowLayout)
        {
            this.bakedLayout = bakedLayout;
            this.rawFlowLayout = rawFlowLayout;
            this.rows = new BakedRow[this.rawFlowLayout.RowCount];

            for (int rowIndex = 0; rowIndex < this.rawFlowLayout.RowCount; rowIndex++)
            {
                var rowItems = this.rawFlowLayout.GetItemNodes(rowIndex);
                this.rows[rowIndex] = new BakedRow(bakedLayout, this.rawFlowLayout.GetRowName(rowIndex), this.rawFlowLayout.GetRowUsedSpace(rowIndex), rowItems);
            }

            UsedSpace = CalculateUsedSpace();
        }

        private Point CalculateUsedSpace()
        {
            var orientation = OriginalRoot.Orientation;
            int? finalAlongNear = null;
            int? finalPerpendicularNear = null;
            int? finalAlongFar = null;
            int? finalPerpendicularFar = null;

            foreach (var row in Rows)
            {
                foreach (var node in row)
                {
                    var alongNear = node.Rectangle.Location.AxisValue(orientation.ToAxis());
                    var perpendicularNear = node.Rectangle.Location.OppositeAxisValue(orientation.ToAxis());

                    var alongFar = alongNear + node.Rectangle.Size.AxisValue(orientation.ToAxis());
                    var perpendicularFar = perpendicularNear + node.Rectangle.Size.OppositeAxisValue(orientation.ToAxis());

                    if (!finalAlongNear.HasValue)
                    {
                        finalAlongNear = alongNear;
                        finalAlongFar = alongFar;
                        finalPerpendicularNear = perpendicularNear;
                        finalPerpendicularFar = perpendicularFar;
                    }
                    else
                    {
                        finalAlongNear = Math.Min(finalAlongNear.Value, alongNear);
                        finalAlongFar = Math.Max(finalAlongFar.Value, alongFar);
                        finalPerpendicularNear = Math.Min(finalPerpendicularNear.Value, perpendicularNear);
                        finalPerpendicularFar = Math.Max(finalPerpendicularFar.Value, perpendicularFar);
                    }
                }
            }

            if (finalAlongNear.HasValue)
            {
                return orientation.GetPointFromAlongPerpendicular(finalAlongFar.Value - finalAlongNear.Value, finalPerpendicularFar.Value - finalPerpendicularNear.Value);
            }

            return Point.Zero;
        }

        public BakedLayoutNode GetNode(string nodeName)
        {
            return this.bakedLayout.GetNode(nodeName);
        }

        public IEnumerable<string> AllResultNodeNames()
        {
            return this.bakedLayout.AllResultNodeNames();
        }

        public BakedRow GetRow(int rowIndex)
        {
            return this.rows[rowIndex];
        }

        public BakedRow GetLastRow()
        {
            return this.rows[RowCount - 1];
        }

        public int RowCount => this.rows.Length;

        public IEnumerable<BakedRow> Rows => this.rows;

        public class BakedRow : IEnumerable<BakedLayoutNode>
        {
            private readonly BakedLayoutNode rowNode;
            private readonly Point rowUsedSpace;
            private readonly BakedLayoutNode[] itemNodes;

            public BakedRow(BakedLayout bakedLayout, string rowName, Point rowUsedSpace, LayoutNode[] itemNodes)
            {
                this.rowUsedSpace = rowUsedSpace;
                this.rowNode = bakedLayout.GetNode(rowName);
                this.itemNodes = bakedLayout.GetDirectChildrenOfNode(rowName);
            }

            public BakedLayoutNode Node => this.rowNode;

            public BakedLayoutNode GetItemNode(int itemIndex)
            {
                return this.itemNodes[itemIndex];
            }

            public BakedLayoutNode GetLastItemNode()
            {
                if (ItemCount == 0)
                {
                    return new BakedLayoutNode(Point.Zero, Point.Zero, 0);
                }
                return this.itemNodes[ItemCount - 1];
            }

            public IEnumerator<BakedLayoutNode> GetEnumerator()
            {
                foreach (var item in this.itemNodes)
                {
                    yield return item;
                }
            }

            IEnumerator IEnumerable.GetEnumerator()
            {
                return this.itemNodes.GetEnumerator();
            }

            private Point GetTopLeftUsedPosition()
            {
                if (this.itemNodes.Length == 0)
                {
                    return TotalRectangle.Location;
                }

                var topLeft = this.itemNodes[0].PositionRelativeToRoot;
                foreach (var itemNode in itemNodes)
                {
                    var currentItem = itemNode.PositionRelativeToRoot;
                    topLeft.X = Math.Min(topLeft.X, currentItem.X);
                    topLeft.Y = Math.Min(topLeft.Y, currentItem.Y);
                }

                return topLeft;
            }

            public int ItemCount => this.itemNodes.Length;

            public Rectangle UsedRectangle => new Rectangle(GetTopLeftUsedPosition(), this.rowUsedSpace);

            public Rectangle TotalRectangle => rowNode.Rectangle;
        }
    }
}
