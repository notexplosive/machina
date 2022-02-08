using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class OverflowRule
    {
        private OverflowRule(bool haltOnFailure)
        {
            HaltImmediatelyUponFailure = haltOnFailure;
        }

        public static OverflowRule PermitExtraRows = new OverflowRule(false);
        public static OverflowRule HaltOnIllegal = new OverflowRule(true);

        public bool HaltImmediatelyUponFailure { get; }
    }

    public static class FlowLayout
    {
        private class Rows
        {
            public Rows(int availableWidth, int availableHeight, FlowLayoutStyle flowLayoutStyle)
            {
                AvailableWidth = availableWidth;
                AvailableHeight = availableHeight;
                Style = flowLayoutStyle;

                CurrentRow = new Row(AvailableWidth, Style);
                Content.Add(CurrentRow);
            }

            private Row CurrentRow { get; set; }
            public int AvailableWidth { get; }
            public int AvailableHeight { get; }
            public FlowLayoutStyle Style { get; }
            public List<Row> Content { get; } = new List<Row>();
            public Point UsedSize => new Point(AvailableWidth, HeightOfAllContent + CurrentRow.Height + TotalPaddingBetweenRows);
            public int TotalPaddingBetweenRows => Content.Count * Style.PaddingBetweenRows;
            public int HeightOfAllContent { get; private set; }
            public bool IsFull { get; private set; }
            public int RemainingWidthInCurrentRow => CurrentRow.RemainingWidth;

            public void CreateNextRowAndAdd(LayoutNode itemToAdd)
            {
                if (IsFull)
                {
                    return;
                }

                HeightOfAllContent += CurrentRow.Height;
                CurrentRow = new Row(AvailableWidth, Style);
                AddItemToCurrentRow(itemToAdd);
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

            public void AddItemToCurrentRow(LayoutNode itemToAdd)
            {
                if (IsFull)
                {
                    return;
                }

                CurrentRow.AddItem(itemToAdd);

                if (UsedSize.Y > AvailableHeight && Style.OverflowRule.HaltImmediatelyUponFailure)
                {
                    IsFull = true;
                    CurrentRow.PopLastItem();
                }

            }
        }

        private class Row
        {
            public Row(int availableWidth, FlowLayoutStyle style)
            {
                AvailableWidth = availableWidth;
                FlowLayoutStyle = style;
            }

            private LayoutStyle RowStyle => new LayoutStyle(alignment: FlowLayoutStyle.Alignment, padding: FlowLayoutStyle.PaddingBetweenItemsInEachRow);
            public List<LayoutNode> Content { get; } = new List<LayoutNode>();
            public int Height => EstimatedSize.Y;
            public int AvailableWidth { get; }
            public FlowLayoutStyle FlowLayoutStyle { get; }
            public int RemainingWidth => AvailableWidth - UsedWidth;
            public int UsedWidth => EstimatedSize.X;
            public Point EstimatedSize { get; private set; }

            public void AddItem(LayoutNode child)
            {
                Content.Add(child);
                UpdateEstimatedSize();
            }

            private LayoutNode GetLayoutNodeAsFlex(string rowNodeName)
            {
                return FlexLayout.HorizontalFlexParent(rowNodeName, RowStyle, Content.ToArray());
            }

            public LayoutNode GetLayoutNode(string rowNodeName)
            {
                return LayoutNode.HorizontalParent(rowNodeName, LayoutSize.Pixels(AvailableWidth, Height), RowStyle, Content.ToArray());
            }

            public void PopLastItem()
            {
                Content.RemoveAt(Content.Count - 1);
                UpdateEstimatedSize();
            }

            public void UpdateEstimatedSize()
            {
                EstimatedSize = GetLayoutNodeAsFlex("flex").Bake().GetNode("flex").Size;
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
            var rows = new Rows(workableArea.Size.X, workableArea.Size.Y, style);

            foreach (var child in children)
            {
                if (rows.RemainingWidthInCurrentRow >= child.Size.Width.ActualSize)
                {
                    rows.AddItemToCurrentRow(child);
                }
                else
                {
                    rows.CreateNextRowAndAdd(child);
                }

                if (rows.IsFull)
                {
                    break;
                }
            }

            // again the root node being "Horizontal" doesn't matter, we really need that "ParentOfSingleThing" static function
            return LayoutNode.HorizontalParent(name, size, workableAreaStyle,
                // this "vertical" does matter because we stack the rows vertically, LTR and RTL would be vertical but TTB and BTT would be horizontal
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
        public OverflowRule OverflowRule { get; }
        public Point Margin { get; }

        public FlowLayoutStyle(
            Point margin = default,
            int paddingBetweenRows = default,
            int paddingBetweenItemsInEachRow = default,
            Alignment alignment = default,
            OverflowRule overflowRule = default)
        {
            if (overflowRule == default)
            {
                overflowRule = OverflowRule.PermitExtraRows;
            }

            Margin = margin;
            PaddingBetweenItemsInEachRow = paddingBetweenItemsInEachRow;
            PaddingBetweenRows = paddingBetweenRows;
            Alignment = alignment;
            OverflowRule = overflowRule;
        }
    }
}
