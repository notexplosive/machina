using FluentAssertions;
using Machina.Data;
using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public class BakedLayoutTests
    {
        [Fact]
        public void can_get_baked_layout_node_by_name()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(20, 5), new LayoutStyle(margin: new Point(1, 1), padding: 1, Alignment.TopLeft),
                LayoutNode.Leaf("a", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("b", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("c", LayoutSize.StretchedBoth())
            );

            var subject = layout.Bake();

            var acquiredNode = subject.GetNode("b");
            acquiredNode.Size.Should().Be(new Point(5, 3));
        }
    }
}