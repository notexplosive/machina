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

        [Fact]
        public void can_obtain_all_result_nodes()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(20, 5), new LayoutStyle(margin: new Point(1, 1), padding: 1, Alignment.TopLeft),
                LayoutNode.Leaf("a", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("b", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("c", LayoutSize.StretchedBoth())
            );

            var subject = layout.Bake();

            var result = subject.GetAllResultNodes();
            result.Should().HaveCount(4);

            result.Should().Contain(subject.GetNode("root"));
            result.Should().Contain(subject.GetNode("a"));
            result.Should().Contain(subject.GetNode("b"));
            result.Should().Contain(subject.GetNode("c"));
        }

        [Fact]
        public void can_obtain_all_result_nodes_in_order()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(20, 5), new LayoutStyle(margin: new Point(1, 1), padding: 1, Alignment.TopLeft),
                LayoutNode.Leaf("a", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("b", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("c", LayoutSize.StretchedBoth())
            );

            var subject = layout.Bake();

            var result = subject.GetAllResultNodesInHierarchyOrder();
            result.Should().HaveCount(4);

            result.Should().ContainInOrder(subject.GetNode("root"), subject.GetNode("a"), subject.GetNode("b"), subject.GetNode("c"));
        }

        [Fact]
        public void can_obtain_child_nodes_of_particular_node()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(20, 5), new LayoutStyle(margin: new Point(1, 1), padding: 1, Alignment.TopLeft),
                LayoutNode.HorizontalParent("a", LayoutSize.StretchedBoth(), LayoutStyle.Empty,
                    LayoutNode.Leaf("a1", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("a2", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("a3", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("a4", LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("a5", LayoutSize.StretchedBoth())
                ),
                LayoutNode.Leaf("b", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("c", LayoutSize.StretchedBoth())
            );

            var subject = layout.Bake();

            subject.GetDirectChildrenOfNode("a").Should().ContainInOrder(subject.GetNode("a1"), subject.GetNode("a2"), subject.GetNode("a3"), subject.GetNode("a4"), subject.GetNode("a5"));
        }
    }
}