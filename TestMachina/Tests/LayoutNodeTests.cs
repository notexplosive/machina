using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Components;
using Machina.Data;
using Microsoft.Xna.Framework;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    public class LayoutNodeTests
    {
        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), Orientation.Vertical)
                .AddChildren(
                    new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                    new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                    new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
                );

            var layoutResult = layout.Bake();

            var item_1 = layoutResult.Get("item-1");
            item_1.Width.Should().Be(50);
            item_1.Height.Should().Be(10);
            item_1.X.Should().Be(0);
            item_1.Y.Should().Be(0);
            var item_2 = layoutResult.Get("item-2");
            item_2.Width.Should().Be(50);
            item_2.Height.Should().Be(20);
            item_2.X.Should().Be(0);
            item_2.Y.Should().Be(10);
            var item_3 = layoutResult.Get("item-3");
            item_3.Width.Should().Be(50);
            item_3.Height.Should().Be(70);
            item_3.X.Should().Be(0);
            item_3.Y.Should().Be(30);

            var drawPanel = new AsciiDrawPanel(new Point(50, 100));
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key);
            }

            Approvals.Verify(drawPanel.GetImage());
        }

        // test with margin

        // test with padding

        // test with deep nests
    }
}