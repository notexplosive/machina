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

            var drawPanel = new AsciiDrawPanel(new Point(50, 100));
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key);
            }

            Approvals.Verify(drawPanel.GetImage());
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, margin: new Point(10, 5))
                .AddChildren(
                    new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                    new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                    new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
                );

            var layoutResult = layout.Bake();

            var drawPanel = new AsciiDrawPanel(new Point(50, 100));
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key);
            }

            Approvals.Verify(drawPanel.GetImage());
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_padding()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, padding: 5)
                .AddChildren(
                    new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                    new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                    new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
                );

            var layoutResult = layout.Bake();

            var drawPanel = new AsciiDrawPanel(new Point(50, 100));
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key);
            }

            Approvals.Verify(drawPanel.GetImage());
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin_and_padding()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, padding: 5, margin: new Point(3, 6))
                .AddChildren(
                    new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                    new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                    new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
                );

            var layoutResult = layout.Bake();

            var drawPanel = new AsciiDrawPanel(new Point(50, 100));
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key);
            }

            Approvals.Verify(drawPanel.GetImage());
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void nested_layout_test()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical)
                .AddChildren(
                    new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                    new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20)), orientation: Orientation.Horizontal, margin: new Point(2, 3))
                        .AddChildren(
                            new LayoutNode("item-2a", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                            new LayoutNode("item-2b", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                            new LayoutNode("item-2c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                        ),
                    new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()), Orientation.Vertical)
                        .AddChildren(
                            new LayoutNode("item-3a", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                            new LayoutNode("item-3b", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                            new LayoutNode("item-3c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                        )
                );

            var layoutResult = layout.Bake();

            var drawPanel = new AsciiDrawPanel(new Point(50, 100));
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key);
            }

            Approvals.Verify(drawPanel.GetImage());
        }
    }
}