using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class OverflowRule
    {
        private OverflowRule(bool haltOnFailure, bool deletesWholeRow, bool doNotAddMoreRowsAfterFailure)
        {
            HaltImmediatelyUponFailure = haltOnFailure;
            DeletesWholeRowUponFailure = deletesWholeRow;
            DoNotAddMoreRowsAfterFailure = doNotAddMoreRowsAfterFailure;
        }

        public static OverflowRule PermitExtraRows = new OverflowRule(false, false, false);
        public static OverflowRule HaltOnIllegal = new OverflowRule(true, false, true);
        public static OverflowRule CancelRowOnIllegal = new OverflowRule(true, true, true);
        public static OverflowRule FinishRowOnIllegal = new OverflowRule(false, false, true);

        public bool HaltImmediatelyUponFailure { get; }
        public bool DeletesWholeRowUponFailure { get; }
        public bool DoNotAddMoreRowsAfterFailure { get; }
    }

    public class FlowLayoutNode
    {
        private LayoutNode InternalLayoutNode { get; }
        private FlowLayout.Instruction InternalInstruction { get; }
        public bool IsLayoutNode => InternalLayoutNode != null;
        public bool IsInstruction => InternalInstruction != null;

        public FlowLayoutNode(LayoutNode layoutNode)
        {
            InternalLayoutNode = layoutNode;
        }

        public FlowLayoutNode(FlowLayout.Instruction flowInstruction)
        {
            InternalInstruction = flowInstruction;
        }

        public static implicit operator LayoutNode(FlowLayoutNode self)
        {
            return self.InternalLayoutNode;
        }

        public static implicit operator FlowLayoutNode(LayoutNode node)
        {
            return new FlowLayoutNode(node);
        }

        public static implicit operator FlowLayout.Instruction(FlowLayoutNode self)
        {
            return self.InternalInstruction;
        }

        public static implicit operator FlowLayoutNode(FlowLayout.Instruction instruction)
        {
            return new FlowLayoutNode(instruction);
        }
    }

    public static class FlowLayout
    {
        public class Instruction
        {
            private Instruction()
            {

            }

            public static Instruction Linebreak = new Instruction();
        }

        private class Rows
        {
            public Rows(Point availableSize, FlowLayoutStyle flowLayoutStyle, Orientation orientation)
            {
                Orientation = orientation;
                AvailableAlongSize = availableSize.AxisValue(orientation.ToAxis());
                AvailablePerpendicularSize = availableSize.OppositeAxisValue(orientation.ToAxis());
                Style = flowLayoutStyle;

                CurrentRow = new Row(AvailableAlongSize, Style, Orientation);
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
            public bool StopAddingNewRows { get; private set; }
            public int RemainingAlongSizeInCurrentRow => CurrentRow.RemainingAlongSize;

            public void CreateNextRowAndAdd(LayoutNode itemToAdd)
            {
                if (IsFull || StopAddingNewRows)
                {
                    return;
                }

                AddNewRow();
                AddItemToCurrentRow(itemToAdd);
            }

            private void AddNewRow()
            {
                if (IsFull || StopAddingNewRows)
                {
                    return;
                }

                PerpendicularSizeOfAllContent += CurrentRow.UsedPerpendicularSize;
                CurrentRow = new Row(AvailableAlongSize, Style, Orientation);
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

                    if (Style.OverflowRule.DoNotAddMoreRowsAfterFailure)
                    {
                        StopAddingNewRows = true;
                    }
                }

            }

            private void PopLastRow()
            {
                Content.RemoveAt(Content.Count - 1);
                CurrentRow = new Row(AvailableAlongSize, Style, Orientation);
            }

            public bool CanFitItem(LayoutNode item)
            {
                return RemainingAlongSizeInCurrentRow >= item.Size.GetValueFromOrientation(Orientation).ActualSize;
            }

            public void ConsumeInstruction(Instruction instruction)
            {
                if (instruction == Instruction.Linebreak)
                {
                    AddNewRow();
                }
            }
        }

        private class Row
        {
            public Row(int availableAlongSize, FlowLayoutStyle style, Orientation orientation)
            {
                Orientation = orientation;
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

        public static LayoutNode HorizontalFlowParent(string name, LayoutSize size, FlowLayoutStyle style, params FlowLayoutNode[] children)
        {
            return OrientedFlowParent(Orientation.Horizontal, name, size, style, children);
        }

        public static LayoutNode VerticalFlowParent(string name, LayoutSize size, FlowLayoutStyle style, params FlowLayoutNode[] children)
        {
            return OrientedFlowParent(Orientation.Vertical, name, size, style, children);
        }

        private static LayoutNode OrientedFlowParent(Orientation orientation, string name, LayoutSize size, FlowLayoutStyle style, params FlowLayoutNode[] children)
        {
            var workableAreaStyle = new LayoutStyle(margin: style.Margin, alignment: style.Alignment);

            // It sucks that we have to give the node a name, then immediately ask for the node we just named. But I'm not sure where that API should live.
            var workableArea = LayoutNode.NamelessOneOffParent(size, workableAreaStyle, LayoutNode.Leaf("workableArea", LayoutSize.StretchedBoth())).Bake().GetNode("workableArea");
            var rows = new Rows(workableArea.Size, style, orientation);

            foreach (var item in children)
            {
                if (item.IsLayoutNode)
                {
                    if (rows.CanFitItem(item))
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

                if (rows.IsFull)
                {
                    break;
                }
            }

            return LayoutNode.OneOffParent(name, size, workableAreaStyle,
                LayoutNode.OrientedParent(orientation.Opposite(), "rows", LayoutSize.Pixels(rows.UsedSize), new LayoutStyle(padding: style.PaddingBetweenRows),
                    rows.GetLayoutNodesOfEachRow()
                )
            );
        }
    }
}
