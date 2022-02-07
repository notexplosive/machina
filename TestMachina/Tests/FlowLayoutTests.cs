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
    public class FlexLayoutTests
    {
        [Fact]
        public void can_create_flex_layout()
        {
            var layout = FlexLayout.HorizontalFlexParent("root", LayoutStyle.Empty,
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 3)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(8, 5)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 8))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        [Fact]
        public void can_create_vertical_flex_layout()
        {

        }

        [Fact]
        public void flex_layout_supports_margin()
        {

        }

        [Fact]
        public void flex_layout_supports_padding()
        {

        }

        [Fact]
        public void flex_considers_alignment_when_things_have_varied_perpendicular_size()
        {

        }
    }

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

        // TODO: Padding between individual items
        // TODO: Varied heights of elements
        // TODO: Overflow
    }
}