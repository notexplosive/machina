using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    /// <summary>
    /// Flow Layout is given a restricted set of bounds and then attempts to lay out the items in a way to fill those bounds. Linebreaking where necessary.
    /// </summary>
    public static class FlowLayout
    {
        public static RawFlowLayout HorizontalFlowParent(string name, LayoutSize size, FlowLayoutStyle style, params LayoutNodeOrInstruction[] children)
        {
            return OrientedFlowParent(Orientation.Horizontal, name, size, style, children);
        }

        public static RawFlowLayout VerticalFlowParent(string name, LayoutSize size, FlowLayoutStyle style, params LayoutNodeOrInstruction[] children)
        {
            return OrientedFlowParent(Orientation.Vertical, name, size, style, children);
        }

        private static RawFlowLayout OrientedFlowParent(Orientation orientation, string name, LayoutSize size, FlowLayoutStyle style, params LayoutNodeOrInstruction[] children)
        {
            var workableAreaStyle = new LayoutStyle(margin: style.Margin, alignment: style.Alignment);

            var workableArea = LayoutNode.NamelessOneOffParent(size, workableAreaStyle, LayoutNode.Leaf("workableArea", LayoutSize.StretchedBoth())).Bake().GetNode("workableArea");
            var rows = new FlowLayoutRows(workableArea.Size, style, orientation);

            foreach (var item in children)
            {
                if (item.IsLayoutNode)
                {
                    if (!rows.CanFitItemPerpendicular(item))
                    {
                        break;
                    }

                    if (rows.CanFitItemAlongCurrentRow(item))
                    {
                        rows.AddItemToCurrentRow(item);
                    }
                    else
                    {
                        rows.CreateNextRowAndAdd(item);
                    }
                }
                else if (item.IsInstruction)
                {
                    rows.ConsumeInstruction(item);
                }
            }

            return new RawFlowLayout(name, size, workableAreaStyle, orientation, style, rows);
        }

        public class LayoutNodeOrInstruction
        {
            private LayoutNode InternalLayoutNode { get; }
            private FlowLayoutInstruction InternalInstruction { get; }
            public bool IsLayoutNode => InternalLayoutNode != null;
            public bool IsInstruction => InternalInstruction != null;

            public LayoutNodeOrInstruction(LayoutNode layoutNode)
            {
                InternalLayoutNode = layoutNode;
            }

            public LayoutNodeOrInstruction(FlowLayoutInstruction flowInstruction)
            {
                InternalInstruction = flowInstruction;
            }

            public static implicit operator LayoutNode(LayoutNodeOrInstruction self)
            {
                return self.InternalLayoutNode;
            }

            public static implicit operator LayoutNodeOrInstruction(LayoutNode node)
            {
                return new LayoutNodeOrInstruction(node);
            }

            public static implicit operator FlowLayoutInstruction(LayoutNodeOrInstruction self)
            {
                return self.InternalInstruction;
            }

            public static implicit operator LayoutNodeOrInstruction(FlowLayoutInstruction instruction)
            {
                return new LayoutNodeOrInstruction(instruction);
            }
        }
    }

    internal class FlowLayoutRows
    {
        public FlowLayoutRows(Point availableSize, FlowLayoutStyle flowLayoutStyle, Orientation orientation)
        {
            Orientation = orientation;
            AvailableAlongSize = availableSize.AxisValue(orientation.ToAxis());
            AvailablePerpendicularSize = availableSize.OppositeAxisValue(orientation.ToAxis());
            Style = flowLayoutStyle;

            CurrentRow = new FlowLayoutRow(AvailableAlongSize, Style, Orientation);
            Content.Add(CurrentRow);
        }

        private FlowLayoutRow CurrentRow { get; set; }
        public Orientation Orientation { get; }
        public int AvailableAlongSize { get; }
        public int AvailablePerpendicularSize { get; }
        public FlowLayoutStyle Style { get; }
        public List<FlowLayoutRow> Content { get; } = new List<FlowLayoutRow>();
        public Point UsedSize => Orientation.GetPointFromAlongPerpendicular(AvailableAlongSize, PerpendicularSizeOfAllRowsExceptCurrent + CurrentRow.UsedPerpendicularSize + TotalPaddingBetweenRows);
        public int TotalPaddingBetweenRows => Content.Count * Style.PaddingBetweenRows;
        public int PerpendicularSizeOfAllRowsExceptCurrent { get; private set; }
        public bool StopAddingNewItems { get; private set; }
        public bool StopAddingNewRows { get; private set; }
        public int RemainingAlongSizeInCurrentRow => CurrentRow.RemainingAlongSize;
        public bool HadOverflow => StopAddingNewRows || StopAddingNewItems;

        public void CreateNextRowAndAdd(LayoutNode itemToAdd)
        {
            if (HadOverflow)
            {
                return;
            }

            if (CurrentRow.Content.Count == 0)
            {
                AddItemToCurrentRow(itemToAdd);
            }
            else if (!HasRoomForAnotherRow(itemToAdd))
            {
                AddItemToCurrentRow(itemToAdd);
            }
            else
            {
                AddNewRow();
                AddItemToCurrentRow(itemToAdd);
            }
        }

        private bool HasRoomForAnotherRow(LayoutNode itemToAdd)
        {
            if (Style.OverflowRule.HasInfiniteRows)
            {
                return true;
            }

            var possibleNewRow = new FlowLayoutRow(AvailableAlongSize, Style, Orientation);
            possibleNewRow.AddItem(itemToAdd);
            var totalSizeAfterAddingRow = UsedSize.OppositeAxisValue(Orientation.ToAxis()) + possibleNewRow.UsedPerpendicularSize;
            return totalSizeAfterAddingRow <= AvailablePerpendicularSize;
        }

        private void AddNewRow()
        {
            if (HadOverflow)
            {
                return;
            }

            PerpendicularSizeOfAllRowsExceptCurrent += CurrentRow.UsedPerpendicularSize;
            CurrentRow = new FlowLayoutRow(AvailableAlongSize, Style, Orientation);
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
            if (StopAddingNewItems)
            {
                return;
            }

            CurrentRow.AddItem(itemToAdd);

            var overflowedAlong = CurrentRow.UsedAlongSize > AvailableAlongSize;
            var overflowedPerpendicular = UsedSize.OppositeAxisValue(Orientation.ToAxis()) > AvailablePerpendicularSize;
            var failed = overflowedPerpendicular || overflowedAlong;
            if (failed)
            {
            }

        }

        public bool CanFitItemAlongCurrentRow(LayoutNode item)
        {
            return RemainingAlongSizeInCurrentRow >= item.Size.GetValueFromOrientation(Orientation).ActualSize;
        }

        public void ConsumeInstruction(FlowLayoutInstruction instruction)
        {
            if (instruction == FlowLayoutInstruction.Linebreak)
            {
                if (HasRoomForAnotherRow(LayoutNode.NamelessLeaf(LayoutSize.Pixels(0, 0))))
                {
                    AddNewRow();
                }
            }
        }

        public Point[] GetUsedSpaceOfEachRow()
        {
            var sizes = new Point[Content.Count];

            for (int i = 0; i < Content.Count; i++)
            {
                sizes[i] = Orientation.GetPointFromAlongPerpendicular(Content[i].UsedAlongSize, Content[i].UsedPerpendicularSize);
            }

            return sizes;
        }

        public bool CanFitItemPerpendicular(LayoutNode item)
        {
            if (Style.OverflowRule.HasInfiniteRows)
            {
                return true;
            }
            var usedPerpendicular = PerpendicularSizeOfAllRowsExceptCurrent;
            var itemPerpendicular = item.Size.GetValueFromOrientation(Orientation.Opposite()).ActualSize;
            return AvailablePerpendicularSize >= usedPerpendicular + itemPerpendicular;
        }
    }

    internal class FlowLayoutRow
    {
        public FlowLayoutRow(int availableAlongSize, FlowLayoutStyle style, Orientation orientation)
        {
            Orientation = orientation;
            AvailableAlongSize = availableAlongSize;
            FlowLayoutStyle = style;
        }

        private LayoutStyle RowStyle => new LayoutStyle(alignment: FlowLayoutStyle.AlignmentWithinRow, padding: FlowLayoutStyle.PaddingBetweenItemsInEachRow);
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

        private RawLayout GetRowAsFlex(string rowNodeName)
        {
            return FlexLayout.OrientedFlexParent(Orientation, rowNodeName, new FlexLayoutStyle(style: RowStyle), Content.ToArray());
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
            EstimatedSize = GetRowAsFlex("flex").Bake().GetNode("flex").Size;
        }
    }

    public class FlowLayoutInstruction
    {
        private FlowLayoutInstruction()
        {

        }

        public static FlowLayoutInstruction Linebreak = new FlowLayoutInstruction();
    }
}
