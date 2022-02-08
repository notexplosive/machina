using Microsoft.Xna.Framework;
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
            public Rows(Point availableSize, FlowLayoutStyle flowLayoutStyle)
            {
                AvailableWidth = availableSize.X;
                AvailableHeight = availableSize.Y;
                Style = flowLayoutStyle;

                CurrentRow = new Row(AvailableWidth, Style);
                Content.Add(CurrentRow);
            }

            private Row CurrentRow { get; set; }
            public int AvailableWidth { get; }
            public int AvailableHeight { get; }
            public FlowLayoutStyle Style { get; }
            public List<Row> Content { get; } = new List<Row>();
            public Point UsedSize => new Point(AvailableWidth, HeightOfAllContent + CurrentRow.UsedPerpendicularSize + TotalPaddingBetweenRows);
            public int TotalPaddingBetweenRows => Content.Count * Style.PaddingBetweenRows;
            public int HeightOfAllContent { get; private set; }
            public bool IsFull { get; private set; }
            public int RemainingWidthInCurrentRow => CurrentRow.RemainingAlongSize;

            public void CreateNextRowAndAdd(LayoutNode itemToAdd)
            {
                if (IsFull)
                {
                    return;
                }

                HeightOfAllContent += CurrentRow.UsedPerpendicularSize;
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
            public Row(int availableAlongSize, FlowLayoutStyle style)
            {
                Orientation = Orientation.Horizontal;
                AvailableAlongSize = availableAlongSize;
                FlowLayoutStyle = style;
            }

            private LayoutStyle RowStyle => new LayoutStyle(alignment: FlowLayoutStyle.Alignment, padding: FlowLayoutStyle.PaddingBetweenItemsInEachRow);
            public List<LayoutNode> Content { get; } = new List<LayoutNode>();
            public Orientation Orientation { get; }
            public int AvailableAlongSize { get; }
            public FlowLayoutStyle FlowLayoutStyle { get; }
            public int RemainingAlongSize => AvailableAlongSize - UsedAlongSize;
            public int UsedAlongSize => EstimatedSize.AxisValue(Orientation.ToAxis());
            public int UsedPerpendicularSize => EstimatedSize.OppositeAxisValue(Orientation.ToAxis());
            public Point EstimatedSize { get; private set; }

            public void AddItem(LayoutNode child)
            {
                Content.Add(child);
                UpdateEstimatedSize();
            }

            private LayoutNode GetLayoutNodeAsFlex(string rowNodeName)
            {
                return FlexLayout.OrientedFlexParent(Orientation, rowNodeName, RowStyle, Content.ToArray());
            }

            public LayoutNode GetLayoutNode(string rowNodeName)
            {
                var size = Orientation.GetPointFromAlongPerpendicular(AvailableAlongSize, UsedPerpendicularSize);
                return LayoutNode.OrientedParent(Orientation, rowNodeName, LayoutSize.Pixels(size), RowStyle, Content.ToArray());
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

            // It sucks that we have to give the node a name, then immediately ask for the node we just named
            var workableArea = LayoutNode.OneOffParent(size, workableAreaStyle, LayoutNode.Leaf("workableArea", LayoutSize.StretchedBoth())).Bake().GetNode("workableArea");
            var rows = new Rows(workableArea.Size, style);

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

            return LayoutNode.OneOffParent(size, workableAreaStyle,
                LayoutNode.OrientedParent(Orientation.Horizontal.Opposite(), "rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            );
        }
    }
}
