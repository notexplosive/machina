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
        public static string DrawResult(LayoutResult layoutResult)
        {
            var drawPanel = new AsciiDrawPanel(layoutResult.RootRectangle.Size);
            foreach (var key in layoutResult.Keys())
            {
                var rect = layoutResult.Get(key);
                drawPanel.DrawRectangle(rect, '.');
                drawPanel.DrawStringAt(rect.Location + new Point(1, 1), key.Text);
            }
            return drawPanel.GetImage();
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), Orientation.Vertical, new[] {
                new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            });

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, margin: new Point(10, 5), children: new[] {
                new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            });

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_padding()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, padding: 5, children: new[]
            {
                new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            });

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin_and_padding()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, padding: 5, margin: new Point(3, 6), children: new[]
            {
                new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            });

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void nested_layout_test()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, children: new LayoutNode[] {
                new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20)), orientation: Orientation.Horizontal, margin: new Point(2, 3), children: new[] {
                        new LayoutNode("item-2a", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        new LayoutNode("item-2b", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        new LayoutNode("item-2c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    }),
                new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()), Orientation.Vertical, margin: new Point(0, 2), padding: 3, children: new[] {
                        new LayoutNode("item-3a", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        new LayoutNode("item-3b", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        new LayoutNode("item-3c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    })
            });

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void resize_and_rebake_test()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 50), orientation: Orientation.Vertical, children: new LayoutNode[] {
                new LayoutNode("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                new LayoutNode("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20)), orientation: Orientation.Horizontal, margin: new Point(2, 3), children: new[] {
                        new LayoutNode("item-2a", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        new LayoutNode("item-2b", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        new LayoutNode("item-2c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    }),
                new LayoutNode("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()), Orientation.Vertical, margin: new Point(5, 2), padding: 1, children: new[] {
                        new LayoutNode("item-3a", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        new LayoutNode("item-3b", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        new LayoutNode("item-3c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    })
            });

            var resizedLayout = layout.GetResized(new LayoutSize(60, 60));

            var firstBakeResult = layout.Build();
            var secondBakeResult = resizedLayout.Build();

            Approvals.Verify(DrawResult(firstBakeResult) + "\n\n" + DrawResult(secondBakeResult));
        }
    }
}