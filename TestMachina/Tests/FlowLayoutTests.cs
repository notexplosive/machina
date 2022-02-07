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

        // TODO: Vertical alignment aligns the whole workableArea
        // TODO: Horizontal alignment aligns individual rows
        // TODO: Varied heights of elements
        // TODO: Overflow
    }
}