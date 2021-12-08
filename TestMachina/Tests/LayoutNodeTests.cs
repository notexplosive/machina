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
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), LayoutStyle.Empty, Orientation.Vertical,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), new LayoutStyle(new Point(10, 5), 0), Orientation.Vertical,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_padding()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), new LayoutStyle(Point.Zero, 5), Orientation.Vertical,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin_and_padding()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), new LayoutStyle(padding: 5, margin: new Point(3, 6)), Orientation.Vertical,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void nested_layout_test()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 100), LayoutStyle.Empty, Orientation.Vertical,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Parent("item-2", LayoutSize.StretchedHorizontally(20), new LayoutStyle(new Point(2, 3), 0), Orientation.Horizontal,
                        LayoutNode.Leaf("item-2a", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2b", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2c", LayoutSize.StretchedHorizontally(10))
                    ),
                LayoutNode.Parent("item-3", LayoutSize.StretchedBoth(), new LayoutStyle(new Point(0, 2), 3), Orientation.Vertical,
                        LayoutNode.Leaf("item-3a", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3b", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3c", LayoutSize.StretchedHorizontally(10))
                    )
            );

            var layoutResult = layout.Build();

            Approvals.Verify(DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void resize_and_rebake_test()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 50), LayoutStyle.Empty, Orientation.Vertical,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Parent("item-2", LayoutSize.StretchedHorizontally(20), new LayoutStyle(new Point(2, 3), 0), Orientation.Horizontal,
                        LayoutNode.Leaf("item-2a", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2b", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2c", LayoutSize.StretchedHorizontally(10))
                    ),
                LayoutNode.Parent("item-3", LayoutSize.StretchedBoth(), new LayoutStyle(new Point(5, 2), 1), Orientation.Vertical,
                        LayoutNode.Leaf("item-3a", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3b", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3c", LayoutSize.StretchedHorizontally(10))
                    )
            );

            var resizedLayout = layout.GetResized(new LayoutSize(60, 60));

            var firstBakeResult = layout.Build();
            var secondBakeResult = resizedLayout.Build();

            Approvals.Verify(DrawResult(firstBakeResult) + "\n\n" + DrawResult(secondBakeResult));
        }


        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void spacer_test()
        {
            var layout = LayoutNode.Parent("root", new LayoutSize(50, 10), new LayoutStyle(Point.Zero, 0), Orientation.Horizontal,
                LayoutNode.Spacer(LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("nudged-item", LayoutSize.StretchedVertically(10)),
                LayoutNode.Spacer(LayoutSize.StretchedVertically(5))
            );

            var firstBakeResult = layout.Build();

            Approvals.Verify(DrawResult(firstBakeResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void create_window_test()
        {
            var headerHeight = 8;
            var layout = LayoutNode.Parent("root", new LayoutSize(80, 40), new LayoutStyle(Point.Zero, 0), Orientation.Vertical,
                LayoutNode.Parent("header", LayoutSize.StretchedHorizontally(headerHeight), new LayoutStyle(padding: 2), Orientation.Horizontal,
                    LayoutNode.Spacer(LayoutSize.StretchedBoth()),
                    LayoutNode.Leaf("minimize", new LayoutSize(headerHeight, headerHeight)),
                    LayoutNode.Leaf("fullscreen", new LayoutSize(headerHeight, headerHeight)),
                    LayoutNode.Leaf("close", new LayoutSize(headerHeight, headerHeight))
                ),
                LayoutNode.Leaf("canvas", LayoutSize.StretchedBoth())
            );

            var firstBakeResult = layout.Build();

            Approvals.Verify(DrawResult(firstBakeResult));
        }
    }
}