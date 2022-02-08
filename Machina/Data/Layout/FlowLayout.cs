using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class OverflowRule
    {
        private OverflowRule(bool haltOnFailure, bool deletesWholeRow)
        {
            HaltImmediatelyUponFailure = haltOnFailure;
            DeletesWholeRowUponFailure = deletesWholeRow;
        }

        public static OverflowRule PermitExtraRows = new OverflowRule(false, false);
        public static OverflowRule HaltOnIllegal = new OverflowRule(true, false);
        public static OverflowRule CancelRowOnIllegal = new OverflowRule(true, true);

        public bool HaltImmediatelyUponFailure { get; }
        public bool DeletesWholeRowUponFailure { get; }
    }

    public static class FlowLayout
    {
        private class Rows
        {
            public Rows(Point availableSize, FlowLayoutStyle flowLayoutStyle)
            {
                Orientation = Orientation.Horizontal;
                AvailableAlongSize = availableSize.X;
                AvailablePerpendicularSize = availableSize.Y;
                Style = flowLayoutStyle;

                CurrentRow = new Row(AvailableAlongSize, Style);
                Content.Add(CurrentRow);
            }

            private Row CurrentRow { get; set; }
            public Orientation Orientation { get; }
            public int AvailableAlongSize { get; }
            public int AvailablePerpendicularSize { get; }
            public FlowLayoutStyle Style { get; }
            public List<Row> Content { get; } = new List<Row>();
            public Point UsedSize => Orientation.GetPointFromAlongPerpendicular(AvailableAlongSize, PerpendicularSizeOfAllContent + CurrentRow.UsedPerpendicularSize + TotalPaddingBetweenRows);
            public int TotalPaddingBetweenRows => Content.Count * Style.PaddingBetweenRows;
            public int PerpendicularSizeOfAllContent { get; private set; }
            public bool IsFull { get; private set; }
            public int RemainingAlongSizeInCurrentRow => CurrentRow.RemainingAlongSize;

            public void CreateNextRowAndAdd(LayoutNode itemToAdd)
            {
                if (IsFull)
                {
                    return;
                }

                PerpendicularSizeOfAllContent += CurrentRow.UsedPerpendicularSize;
                CurrentRow = new Row(AvailableAlongSize, Style);
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

                if (UsedSize.OppositeAxisValue(Orientation.ToAxis()) > AvailablePerpendicularSize)
                {
                    if (Style.OverflowRule.HaltImmediatelyUponFailure)
                    {
                        IsFull = true;
                        CurrentRow.PopLastItem();
                    }

                    if (Style.OverflowRule.DeletesWholeRowUponFailure)
                    {
                        PopLastRow();
                    }
                }

            }

            private void PopLastRow()
            {
                Content.RemoveAt(Content.Count - 1);
                CurrentRow = new Row(AvailableAlongSize, Style);
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

            // It sucks that we have to give the node a name, then immediately ask for the node we just named. But I'm not sure where that API should live.
            var workableArea = LayoutNode.NamelessOneOffParent(size, workableAreaStyle, LayoutNode.Leaf("workableArea", LayoutSize.StretchedBoth())).Bake().GetNode("workableArea");
            var rows = new Rows(workableArea.Size, style);

            foreach (var child in children)
            {
                if (rows.RemainingAlongSizeInCurrentRow >= child.Size.Width.ActualSize)
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

            return LayoutNode.OneOffParent(name, size, workableAreaStyle,
                LayoutNode.OrientedParent(Orientation.Horizontal.Opposite(), "rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            );
        }
    }
}
