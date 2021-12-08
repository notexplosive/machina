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
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), Orientation.Vertical, LayoutStyle.Empty,
                LayoutNode.Leaf("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                LayoutNode.Leaf("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                LayoutNode.Leaf("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, new LayoutStyle(new Point(10, 5), 0),
                LayoutNode.Leaf("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                LayoutNode.Leaf("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                LayoutNode.Leaf("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_padding()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), Orientation.Vertical, new LayoutStyle(Point.Zero, 5),
                LayoutNode.Leaf("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                LayoutNode.Leaf("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                LayoutNode.Leaf("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin_and_padding()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), orientation: Orientation.Vertical, new LayoutStyle(padding: 5, margin: new Point(3, 6)),
                LayoutNode.Leaf("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                LayoutNode.Leaf("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20))),
                LayoutNode.Leaf("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()))
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void nested_layout_test()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), Orientation.Vertical, LayoutStyle.Empty,
                LayoutNode.Leaf("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                LayoutNode.Parent("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20)), Orientation.Horizontal, new LayoutStyle(new Point(2, 3), 0),
                        LayoutNode.Leaf("item-2a", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        LayoutNode.Leaf("item-2b", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        LayoutNode.Leaf("item-2c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    ),
                LayoutNode.Parent("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()), Orientation.Vertical, new LayoutStyle(new Point(0, 2), 3),
                        LayoutNode.Leaf("item-3a", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        LayoutNode.Leaf("item-3b", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        LayoutNode.Leaf("item-3c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    )
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void resize_and_rebake_test()
        {
            var layout = new LayoutNode("root", new LayoutSize(50, 50), orientation: Orientation.Vertical, children: new LayoutNode[] {
                LayoutNode.Leaf("item-1", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                LayoutNode.Parent("item-2", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(20)), Orientation.Horizontal, new LayoutStyle(new Point(2, 3), 0),
                        LayoutNode.Leaf("item-2a", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        LayoutNode.Leaf("item-2b", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10))),
                        LayoutNode.Leaf("item-2c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    ),
                LayoutNode.Parent("item-3", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge()), Orientation.Vertical, new LayoutStyle(new Point(5, 2), 1),
                        LayoutNode.Leaf("item-3a", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        LayoutNode.Leaf("item-3b", new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                        LayoutNode.Leaf("item-3c", new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(10)))
                    )
            });

            var resizedLayout = layout.GetResized(new LayoutSize(60, 60));

            var firstBakeResult = layout.Build();
            var secondBakeResult = resizedLayout.Build();

            Approvals.Verify(DrawResult(firstBakeResult) + "\n\n" + DrawResult(secondBakeResult));
        }


        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void spacer_test()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 10), Orientation.Horizontal, new LayoutStyle(Point.Zero, 0),
                LayoutNode.Spacer(new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge())),
                LayoutNode.Leaf("item-2", new LayoutSize(new ConstLayoutEdge(10), new StretchedLayoutEdge())),
                LayoutNode.Spacer(new LayoutSize(new ConstLayoutEdge(5), new StretchedLayoutEdge()))
            );

            var firstBakeResult = layout.Build();

            Approvals.Verify(DrawResult(firstBakeResult));
        }
    }
}