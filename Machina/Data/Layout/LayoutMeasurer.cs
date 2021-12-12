using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    internal class LayoutMeasurer
    {
        public readonly Dictionary<ILayoutEdge, int> sizeLookupTable = new Dictionary<ILayoutEdge, int>();
        public int MeasureEdge(ILayoutEdge edge)
        {
            if (edge.IsConstant)
            {
                return edge.ActualSize;
            }

            return this.sizeLookupTable[edge];
        }

        public int MeasureEdgeOfNode(LayoutNode node, Orientation orientation)
        {
            return MeasureEdge(node.Size.GetValueFromOrientation(orientation));
        }

        public bool CanMeasureEdge(ILayoutEdge edge)
        {
            return this.sizeLookupTable.ContainsKey(edge);
        }
        public Point GetMeasuredSize(LayoutSize size)
        {
            return new Point(MeasureEdge(size.Width), MeasureEdge(size.Height));
        }

        public void Add(ILayoutEdge key, int value)
        {
            this.sizeLookupTable[key] = value;
        }
    }
}
