using ApprovalTests;
using ApprovalTests.Reporters;
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(30, 30), FlowLayoutStyle.Empty,
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 40), new FlowLayoutStyle(margin: new Point(5)),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(30, 30), new FlowLayoutStyle(paddingBetweenRows: 3),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(30, 30), new FlowLayoutStyle(alignment: Alignment.BottomLeft),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(alignment: Alignment.CenterRight),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(alignment: Alignment.Center),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 30), new FlowLayoutStyle(paddingBetweenItemsInEachRow: 4),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 30), FlowLayoutStyle.Empty,
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 20), new FlowLayoutStyle(overflowRule: OverflowRule.PermitExtraRows),
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 20), new FlowLayoutStyle(overflowRule: OverflowRule.HaltOnIllegal),
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
        public void flow_layout_cancels_whole_row_on_failure()
        {
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(40, 20), new FlowLayoutStyle(overflowRule: OverflowRule.CancelRowOnIllegal),
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

        // TODO: Overflow rules
        // - Cancel row on Failure
        // - Finish Row on Illegal Item: Allow the illegal item, finish the row, and then stop adding items.
        // - Halt on Illegal Item and Elide: Instead of emitting an illegal item, emit an "elide" node instead (provided by user)

        // TODO: Forced linebreaks

        // TODO: Now do it in Vertical
    }
}