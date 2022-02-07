using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public static class FlowLayout
    {
        private class Rows
        {
            public Rows(int defaultWidth)
            {
                DefaultWidth = defaultWidth;

                CurrentRow = new Row(DefaultWidth);
                Content.Add(CurrentRow);
            }

            public Row CurrentRow { get; private set; }
            public int DefaultWidth { get; }
            public List<Row> Content { get; } = new List<Row>();
            public Point UsedSize => new Point(DefaultWidth, TotalHeight + CurrentRow.Height);
            public int TotalHeight { get; private set; }

            public void CreateNextRowAndAdd(LayoutNode child)
            {
                TotalHeight += CurrentRow.Height;
                CurrentRow = new Row(DefaultWidth);
                CurrentRow.AddItem(child);
                Content.Add(CurrentRow);
            }

            public LayoutNode[] GetLayoutNodesOfEachRow()
            {
                var nodes = new LayoutNode[Content.Count];

                for (int i = 0; i < Content.Count; i++)
                {
                    nodes[i] = Content[i].GetLayoutNode($"row {i}");
                }

                return nodes;
            }
        }

        private class Row
        {
            public Row(int width)
            {
                TotalWidth = width;
                RemainingWidth = width;
            }

            public List<LayoutNode> Content { get; } = new List<LayoutNode>();
            public int Height { get; private set; }
            public int TotalWidth { get; }
            public int RemainingWidth { get; private set; }

            public void AddItem(LayoutNode child)
            {
                RemainingWidth -= child.Size.Width.ActualSize;
                Height = Math.Max(Height, child.Size.Height.ActualSize);
                Content.Add(child);
            }

            public LayoutNode GetLayoutNode(string rowNodeName)
            {
                return LayoutNode.HorizontalParent(rowNodeName, LayoutSize.Pixels(TotalWidth - RemainingWidth, Height), LayoutStyle.Empty, Content.ToArray());
            }
        }

        // Should eventually be called "HorizontalLeftToRightFlowParent"
        public static LayoutNode FlowParent(string name, LayoutSize size, FlowLayoutStyle style, params LayoutNode[] children)
        {
            var workableAreaStyle = new LayoutStyle(margin: style.Margin, alignment: style.Alignment);

            // "Horizontal"Parent here doesn't matter because we only have one child. It would be nice to have an API where we just say
            // "parent of single thing" where we clarify orientation agnosticism when we guarantee only having one child
            // While we're at it, it sucks that we have to give the node a name, then immediately ask for the node we just named
            var workableArea = LayoutNode.HorizontalParent("throwAwayParent", size, workableAreaStyle, LayoutNode.Leaf("workableArea", LayoutSize.StretchedBoth())).Bake().GetNode("workableArea");
            var rows = new Rows(workableArea.Size.X);

            foreach (var child in children)
            {
                if (rows.CurrentRow.RemainingWidth >= child.Size.Width.ActualSize)
                {
                    rows.CurrentRow.AddItem(child);
                }
                else
                {
                    rows.CreateNextRowAndAdd(child);
                }
            }

            // again the root node being "Horizontal" doesn't matter
            return LayoutNode.HorizontalParent(name, size, workableAreaStyle,
                LayoutNode.VerticalParent("rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            );
        }
    }

    public struct FlowLayoutStyle
    {
        public static FlowLayoutStyle Empty => new FlowLayoutStyle();

        public int PaddingBetweenRows { get; }
        public int PaddingBetweenItemsInEachRow { get; }
        public Alignment Alignment { get; }
        public Point Margin { get; }

        public FlowLayoutStyle(Point margin = default, int paddingBetweenRows = default, int paddingBetweenItemsInEachRow = default, Alignment alignment = default)
        {
            Margin = margin;
            PaddingBetweenItemsInEachRow = paddingBetweenItemsInEachRow;
            PaddingBetweenRows = paddingBetweenRows;
            Alignment = alignment;
        }
    }
}
