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

        public LayoutNode OriginalRoot => this.bakedLayout.OriginalRoot;

        public BakedFlowLayout(BakedLayout bakedLayout, RawFlowLayout rawFlowLayout)
        {
            this.bakedLayout = bakedLayout;
            this.rawFlowLayout = rawFlowLayout;
            this.rows = new BakedRow[this.rawFlowLayout.RowCount];

            for (int rowIndex = 0; rowIndex < this.rawFlowLayout.RowCount; rowIndex++)
            {
                var rowItems = this.rawFlowLayout.GetItemNamesForRow(rowIndex);
                this.rows[rowIndex] = new BakedRow(bakedLayout, this.rawFlowLayout.GetRowName(rowIndex), this.rawFlowLayout.GetRowUsedSpace(rowIndex), rowItems);
            }
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

        public IEnumerable<BakedRow> Rows => this.rows;

        public class BakedRow : IEnumerable<BakedLayoutNode>
        {
            private readonly BakedLayoutNode rowNode;
            private readonly Point rowUsedSpace;
            private readonly BakedLayoutNode[] itemNodes;

            public BakedRow(BakedLayout bakedLayout, string rowName, Point rowUsedSpace, string[] rowItemNames)
            {
                this.rowUsedSpace = rowUsedSpace;
                this.rowNode = bakedLayout.GetNode(rowName);
                this.itemNodes = new BakedLayoutNode[rowItemNames.Length];

                for (int i = 0; i < this.itemNodes.Length; i++)
                {
                    this.itemNodes[i] = bakedLayout.GetNode(rowItemNames[i]);
                }
            }

            public BakedLayoutNode Node => this.rowNode;

            public BakedLayoutNode GetItemNode(int itemIndex)
            {
                return this.itemNodes[itemIndex];
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
