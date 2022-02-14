using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Data;
using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    [UseReporter(typeof(DiffReporter))]
    public class FlowLayoutTests
    {
        [Fact]
        public void can_create_flow_layout()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(30, 30), FlowLayoutStyle.Empty,
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(10, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_can_have_margin()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 40), new FlowLayoutStyle(margin: new Point(5)),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(10, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_can_have_padding_between_rows()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(30, 30), new FlowLayoutStyle(paddingBetweenRows: 3),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(10, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_supports_vertical_alignment()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(30, 30), new FlowLayoutStyle(alignment: Alignment.BottomLeft),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(10, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }


        [Fact]
        public void flow_layout_supports_horizontal_alignment()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(alignment: Alignment.CenterRight),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_supports_horizontal_alignment_center()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(alignment: Alignment.Center),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_supports_padding_between_individual_items()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(paddingBetweenItemsInEachRow: 4),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_estimates_accurate_height()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 30), FlowLayoutStyle.Empty,
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 5)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(13, 7)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(12, 3)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 12)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(8, 15)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(10, 5))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_can_permit_overflow_extra_rows()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 20), new FlowLayoutStyle(overflowRule: OverflowRule.Free),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(8, 10)),
                LayoutNode.Leaf("itemG", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemH", LayoutSize.Pixels(4, 4))
            );

            var result = layout.Bake();
            Approvals.Verify("There are supposed to be a bunch of skipped pixels here: \n\n" + LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_halts_on_illegal_overflow()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 20), new FlowLayoutStyle(overflowRule: OverflowRule.EverythingMustBeInside),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(8, 12)),
                LayoutNode.Leaf("itemG", LayoutSize.Pixels(9, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_halts_on_illegal_but_keep_last_overflow()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 20), new FlowLayoutStyle(overflowRule: OverflowRule.LastRowKeepsGoing),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("really-wide-item", LayoutSize.Pixels(21, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_allows_forced_linebreaks()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(40, 40), FlowLayoutStyle.Empty,
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 5)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(13, 7)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 5)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(8, 10)),
                LayoutNode.Leaf("itemG", LayoutSize.Pixels(9, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_can_be_vertical()
        {
            var layout = FlowLayout.VerticalFlowParent("root", LayoutSize.Pixels(25, 40), new FlowLayoutStyle(paddingBetweenItemsInEachRow: 4),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_can_be_vertical_with_alignment()
        {
            var layout = FlowLayout.VerticalFlowParent("root", LayoutSize.Pixels(25, 40), new FlowLayoutStyle(alignment: Alignment.BottomRight),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 10))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void flow_layout_can_address_individual_rows()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(25, 40), new FlowLayoutStyle(alignment: Alignment.Center),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 10)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 10)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(13, 10)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 10)),
                LayoutNode.Leaf("itemG", LayoutSize.Pixels(7, 10))
            );

            var result = layout.Bake();

            result.GetRow(0).ItemCount.Should().Be(2);
            result.GetRow(1).ItemCount.Should().Be(1);
            result.GetRow(2).ItemCount.Should().Be(2);
            result.GetRow(3).ItemCount.Should().Be(2);

            result.GetRow(2).GetItemNode(1).Size.Should().Be(new Point(13, 10));

            result.GetRow(0).Node.Rectangle.Should().Be(new Rectangle(new Point(0, 0), new Point(25, 10)));
            result.GetRow(1).Node.Rectangle.Should().Be(new Rectangle(new Point(0, 10), new Point(25, 10)));
            result.GetRow(2).Node.Rectangle.Should().Be(new Rectangle(new Point(0, 20), new Point(25, 10)));
            result.GetRow(3).Node.Rectangle.Should().Be(new Rectangle(new Point(0, 30), new Point(25, 10)));

            result.GetRow(0).UsedRectangle.Should().Be(new Rectangle(new Point(3, 0), new Point(19, 10)));
            result.GetRow(1).UsedRectangle.Should().Be(new Rectangle(new Point(8, 10), new Point(9, 10)));
            result.GetRow(2).UsedRectangle.Should().Be(new Rectangle(new Point(1, 20), new Point(23, 10)));
            result.GetRow(3).UsedRectangle.Should().Be(new Rectangle(new Point(5, 30), new Point(14, 10)));


            int totalItems = 0;
            foreach (var row in result.Rows)
            {
                foreach (var item in row)
                {
                    item.NestingLevel.Should().Be(3);
                    totalItems++;
                }
            }

            totalItems.Should().Be(7);
        }

        [Fact]
        public void verify_used_rows_horizontal()
        {
            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(25, 20), new FlowLayoutStyle(alignment: Alignment.Center),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 5)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 5)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 5)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 5)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(13, 5)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 5)),
                LayoutNode.Leaf("itemG", LayoutSize.Pixels(7, 5))
            );

            var result = layout.Bake();

            Approvals.Verify(
                $"Layout\n{LayoutNodeUtils.DrawResult(result)}\n\nUsed Row Rectangles:\n{LayoutNodeUtils.DrawUsedRectangles(result, result.Rows)}\n\nJust Items:\n{LayoutNodeUtils.DrawItems(result, result.Rows)}"
            );
        }

        [Fact]
        public void verify_used_rows_vertical()
        {
            var layout = FlowLayout.VerticalFlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(alignment: Alignment.Center, paddingBetweenItemsInEachRow: 2, paddingBetweenRows: 2),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(12, 5)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(7, 5)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(9, 5)),
                FlowLayoutInstruction.Linebreak,
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(10, 5)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(13, 5)),
                LayoutNode.Leaf("itemF", LayoutSize.Pixels(7, 5)),
                LayoutNode.Leaf("itemG", LayoutSize.Pixels(7, 5))
            );

            var result = layout.Bake();

            Approvals.Verify(
                $"Layout\n{LayoutNodeUtils.DrawResult(result)}\n\nUsed Row Rectangles:\n{LayoutNodeUtils.DrawUsedRectangles(result, result.Rows)}\n\nJust Items:\n{LayoutNodeUtils.DrawItems(result, result.Rows)}"
            );
        }
    }
}