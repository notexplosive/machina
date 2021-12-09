using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class LayoutIntermediate
    {
        public readonly Dictionary<ILayoutEdge, int> sizeLookupTable = new Dictionary<ILayoutEdge, int>();
        public LayoutResult LayoutResult { get; }

        public LayoutIntermediate(LayoutNode rootNode)
        {
            this.LayoutResult = new LayoutResult(new LayoutResultNode(Point.Zero, GetMeasuredSize(rootNode.Size), 0));
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
    }
}
