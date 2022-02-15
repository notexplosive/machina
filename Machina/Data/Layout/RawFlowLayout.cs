using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class RawFlowLayout : AbstractRawLayout<BakedFlowLayout>
    {
        internal RawFlowLayout(string name, LayoutSize size, LayoutStyle workableAreaStyle, Orientation orientation, FlowLayoutStyle style, FlowLayoutRows rows) : base(
            LayoutNode.OneOffParent(name, size, workableAreaStyle,
                LayoutNode.OrientedParent(orientation.Opposite(), "rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            ))
        {
            this.orientation = orientation;
            this.rowNodes = rows.GetLayoutNodesOfEachRow();
            this.rowUsedSpace = rows.GetUsedSpaceOfEachRow();
        }

        public LayoutNode[] GetItemNodes(int rowIndex)
        {
            return this.rowNodes[rowIndex].Children;
        }


        // ew parallel arrays
        private readonly LayoutNode[] rowNodes;
        private readonly Point[] rowUsedSpace;

        private Orientation orientation;

        public string GetRowName(int rowIndex)
        {
            return this.rowNodes[rowIndex].Name.Text;
        }

        public Point GetRowUsedSpace(int rowIndex)
        {
            return this.rowUsedSpace[rowIndex];
        }

        public override BakedFlowLayout Bake()
        {
            return new BakedFlowLayout(DefaultBake(), this);
        }

        public int RowCount => this.rowNodes.Length;
    }
}
