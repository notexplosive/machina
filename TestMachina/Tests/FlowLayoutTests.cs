using ApprovalTests;
using ApprovalTests.Reporters;
using Machina.Data.Layout;
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
            var layout = FlowLayout.FlowParent("root", LayoutSize.Pixels(30, 30), LayoutStyle.Empty,
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
    }
}