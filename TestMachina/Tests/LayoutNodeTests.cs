using ApprovalTests;
using ApprovalTests.Reporters;
using Machina.Data.Layout;
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
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 100), LayoutStyle.Empty,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 100), new LayoutStyle(new Point(10, 5), 0),
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_padding()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 100), new LayoutStyle(Point.Zero, 5),
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void linear_layout_test_with_margin_and_padding()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 100), new LayoutStyle(padding: 5, margin: new Point(3, 6)),
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.Leaf("item-2", LayoutSize.StretchedHorizontally(20)),
                LayoutNode.Leaf("item-3", LayoutSize.StretchedBoth())
            );

            var layoutResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void nested_layout_test()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 100), LayoutStyle.Empty,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.HorizontalParent("item-2", LayoutSize.StretchedHorizontally(20), new LayoutStyle(new Point(2, 3), 0),
                        LayoutNode.Leaf("item-2a", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2b", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2c", LayoutSize.StretchedHorizontally(10))
                    ),
                LayoutNode.VerticalParent("item-3", LayoutSize.StretchedBoth(), new LayoutStyle(new Point(0, 2), 3),
                        LayoutNode.Leaf("item-3a", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3b", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3c", LayoutSize.StretchedHorizontally(10))
                    )
            );

            var layoutResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(layoutResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void resize_and_rebake_test()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 50), LayoutStyle.Empty,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.HorizontalParent("item-2", LayoutSize.StretchedHorizontally(20), new LayoutStyle(new Point(2, 3), 0),
                        LayoutNode.Leaf("item-2a", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2b", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2c", LayoutSize.StretchedHorizontally(10))
                    ),
                LayoutNode.VerticalParent("item-3", LayoutSize.StretchedBoth(), new LayoutStyle(new Point(5, 2), 1),
                        LayoutNode.Leaf("item-3a", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3b", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3c", LayoutSize.StretchedHorizontally(10))
                    )
            );

            var resizedLayout = layout.GetResized(new Point(60, 60));

            var firstBakeResult = layout.Bake();
            var secondBakeResult = resizedLayout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(firstBakeResult) + "\n\n" + LayoutNodeUtils.DrawResult(secondBakeResult));
        }

        [Fact]
        public void resize_performance_test()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 50), LayoutStyle.Empty,
                LayoutNode.Leaf("item-1", LayoutSize.StretchedHorizontally(10)),
                LayoutNode.HorizontalParent("item-2", LayoutSize.StretchedHorizontally(20), new LayoutStyle(new Point(2, 3), 5),
                        LayoutNode.Leaf("item-2a", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2b", LayoutSize.StretchedHorizontally(10)),
                        LayoutNode.Leaf("item-2c", LayoutSize.StretchedHorizontally(10))
                    ),
                LayoutNode.VerticalParent("item-3", LayoutSize.StretchedBoth(), new LayoutStyle(new Point(5, 2), 1),
                        LayoutNode.Leaf("item-3a", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3b", LayoutSize.StretchedBoth()),
                        LayoutNode.Leaf("item-3c", LayoutSize.StretchedHorizontally(10))
                    )
            );

            for (int i = 0; i < 1000; i++)
            {
                var resizedLayout = layout.GetResized(new Point(i * 10, i * 10));
                resizedLayout.Bake();
            }
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void spacer_test()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(50, 20), LayoutStyle.Empty,
                LayoutNode.StretchedSpacer(),
                LayoutNode.Leaf("nudged-item", LayoutSize.StretchedVertically(10)),
                LayoutNode.Spacer(5)
            );

            var firstBakeResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(firstBakeResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void vertical_stretch_test()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 80), LayoutStyle.Empty,
                LayoutNode.HorizontalParent("group-1", LayoutSize.Pixels(50, 20), new LayoutStyle(margin: new Point(3, 3)),
                    LayoutNode.Leaf("tall-item", LayoutSize.StretchedVertically(15)),
                    LayoutNode.Leaf("both-item", LayoutSize.StretchedBoth())
                ),
                LayoutNode.StretchedSpacer(),
                LayoutNode.VerticalParent("group-2", LayoutSize.Pixels(50, 20), new LayoutStyle(margin: new Point(3, 3)),
                    LayoutNode.Leaf("tall-item-2", LayoutSize.StretchedVertically(15)),
                    LayoutNode.Leaf("both-item-2", LayoutSize.StretchedBoth())
                )
            );

            var firstBakeResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(firstBakeResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void create_window_test()
        {
            var headerHeight = 8;
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(80, 40), LayoutStyle.Empty,
                LayoutNode.HorizontalParent("header", LayoutSize.StretchedHorizontally(headerHeight), new LayoutStyle(padding: 2),
                    LayoutNode.StretchedSpacer(),
                    LayoutNode.Leaf("minimize", LayoutSize.Square(headerHeight)),
                    LayoutNode.Leaf("fullscreen", LayoutSize.Square(headerHeight)),
                    LayoutNode.Leaf("close", LayoutSize.Square(headerHeight))
                ),
                LayoutNode.Leaf("canvas", LayoutSize.StretchedBoth())
            );

            var firstBakeResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(firstBakeResult));
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void fixed_aspect_no_spacers()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(36, 64), LayoutStyle.Empty,
                LayoutNode.HorizontalParent("fixed-aspect", LayoutSize.FixedAspectRatio(16, 9), new LayoutStyle(margin: new Point(3, 3)),
                    LayoutNode.Leaf("tall-item", LayoutSize.StretchedVertically(15))
                )
            );

            var firstBakeResult = layout.Bake(); // wide in tall
            var secondBakeResult = layout.GetResized(new Point(64, 36)).Bake(); // wide in wide (perfect match)
            var thirdBakeResult = layout.GetResized(new Point(100, 36)).Bake();

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(firstBakeResult)
                + "\n\n" +
                LayoutNodeUtils.DrawResult(secondBakeResult)
                + "\n\n" +
                LayoutNodeUtils.DrawResult(thirdBakeResult)
                );
        }

        [Fact]
        [UseReporter(typeof(DiffReporter))]
        public void fixed_aspect_with_spacers()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 80), LayoutStyle.Empty,
                LayoutNode.StretchedSpacer(),
                LayoutNode.HorizontalParent("aligner", LayoutSize.StretchedBoth(), LayoutStyle.Empty,
                    LayoutNode.StretchedSpacer(),
                    LayoutNode.Leaf("fixed-aspect", LayoutSize.FixedAspectRatio(16, 9)),
                    LayoutNode.StretchedSpacer()
                    ),
                LayoutNode.StretchedSpacer()
            );

            var firstBakeResult = layout.Bake();

            Approvals.Verify(LayoutNodeUtils.DrawResult(firstBakeResult));
        }
    }
}