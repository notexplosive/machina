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
            var layout = FlexLayout.VerticalFlexParent("root", LayoutStyle.Empty,
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
        public void flex_layout_supports_margin()
        {
            var layout = FlexLayout.HorizontalFlexParent("root", new LayoutStyle(margin: new Point(5, 5)),
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
        public void flex_layout_supports_padding()
        {
            var layout = FlexLayout.HorizontalFlexParent("root", new LayoutStyle(margin: new Point(5, 5), padding: 3),
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
        public void flex_considers_alignment_when_things_have_varied_perpendicular_size()
        {
            var layout = FlexLayout.HorizontalFlexParent("root", new LayoutStyle(margin: new Point(5, 5), padding: 3, alignment: Alignment.BottomCenter),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 3)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(8, 5)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 8))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }

        // TODO: Max Width
        // TODO: Max Height
        // TODO: Min Width
        // TODO: Min Height
    }
}