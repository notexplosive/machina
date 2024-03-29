﻿using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Data;
using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    [UseReporter(typeof(DiffReporter))]
    public class LayoutNodeTests
    {
        [Fact]
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

            var resizedLayout = layout.RootNode.GetResized(new Point(60, 60));

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
                var resizedLayout = layout.RootNode.GetResized(new Point(i * 10, i * 10));
                resizedLayout.Bake();
            }
        }

        [Fact]
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
        public void fixed_aspect_no_spacers()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(36, 64), LayoutStyle.Empty,
                LayoutNode.HorizontalParent("fixed-aspect", LayoutSize.FixedAspectRatio(16, 9), new LayoutStyle(margin: new Point(3, 3)),
                    LayoutNode.Leaf("tall-item", LayoutSize.StretchedVertically(15))
                )
            );

            var firstBakeResult = layout.Bake(); // wide in tall
            var secondBakeResult = layout.RootNode.GetResized(new Point(64, 36)).Bake(); // wide in wide (perfect match)
            var thirdBakeResult = layout.RootNode.GetResized(new Point(100, 36)).Bake();

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(firstBakeResult)
                + "\n\n" +
                LayoutNodeUtils.DrawResult(secondBakeResult)
                + "\n\n" +
                LayoutNodeUtils.DrawResult(thirdBakeResult)
                );
        }

        [Fact]
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

        [Fact]
        public void fixed_aspect_with_alignment()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(50, 80), new LayoutStyle(alignment: Alignment.Center),
                    LayoutNode.Leaf("fixed-aspect", LayoutSize.FixedAspectRatio(16, 9))
            );

            var firstBakeResult = layout.Bake();

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(firstBakeResult)
                );
        }

        [Fact]
        public void all_alignment_scenarios_single_object()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(20, 20), new LayoutStyle(margin: new Point(3, 1), padding: 2, Alignment.TopLeft),
                LayoutNode.Leaf("obj", LayoutSize.Pixels(6, 4))
            );

            var result = new[] {
                layout.Bake(),
                layout.RootNode.GetRealigned(Alignment.TopCenter).Bake(),
                layout.RootNode.GetRealigned(Alignment.TopRight).Bake(),
                layout.RootNode.GetRealigned(Alignment.CenterLeft).Bake(),
                layout.RootNode.GetRealigned(Alignment.Center).Bake(),
                layout.RootNode.GetRealigned(Alignment.CenterRight).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomLeft).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomCenter).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomRight).Bake(),
            };

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(result[0])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[1])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[2])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[3])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[4])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[5])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[6])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[7])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[8])
                );
        }

        [Fact]
        public void all_alignment_scenarios_multiple_objects_vertical()
        {
            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(20, 20), new LayoutStyle(margin: new Point(3, 1), padding: 2, Alignment.TopLeft),
                LayoutNode.Leaf("objA", LayoutSize.Pixels(6, 4)),
                LayoutNode.Leaf("objB", LayoutSize.Pixels(6, 4))
            );

            var result = new[] {
                layout.Bake(),
                layout.RootNode.GetRealigned(Alignment.TopCenter).Bake(),
                layout.RootNode.GetRealigned(Alignment.TopRight).Bake(),
                layout.RootNode.GetRealigned(Alignment.CenterLeft).Bake(),
                layout.RootNode.GetRealigned(Alignment.Center).Bake(),
                layout.RootNode.GetRealigned(Alignment.CenterRight).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomLeft).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomCenter).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomRight).Bake(),
            };

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(result[0])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[1])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[2])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[3])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[4])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[5])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[6])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[7])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[8])
                );
        }

        [Fact]
        public void all_alignment_scenarios_multiple_objects_horizontal()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(20, 20), new LayoutStyle(margin: new Point(3, 1), padding: 2, Alignment.TopLeft),
                LayoutNode.Leaf("objA", LayoutSize.Pixels(4, 6)),
                LayoutNode.Leaf("objB", LayoutSize.Pixels(4, 6))
            );

            var result = new[] {
                layout.Bake(),
                layout.RootNode.GetRealigned(Alignment.TopCenter).Bake(),
                layout.RootNode.GetRealigned(Alignment.TopRight).Bake(),
                layout.RootNode.GetRealigned(Alignment.CenterLeft).Bake(),
                layout.RootNode.GetRealigned(Alignment.Center).Bake(),
                layout.RootNode.GetRealigned(Alignment.CenterRight).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomLeft).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomCenter).Bake(),
                layout.RootNode.GetRealigned(Alignment.BottomRight).Bake(),
            };

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(result[0])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[1])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[2])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[3])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[4])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[5])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[6])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[7])
                + "\n\n" +
                LayoutNodeUtils.DrawResult(result[8])
                );
        }

        [Fact]
        public void does_not_stretch_evenly()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(20, 5), new LayoutStyle(margin: new Point(1, 1), padding: 1, Alignment.TopLeft),
                LayoutNode.Leaf("a", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("b", LayoutSize.StretchedBoth()),
                LayoutNode.Leaf("c", LayoutSize.StretchedBoth())
            );

            var result = layout.Bake();

            Approvals.Verify(
                LayoutNodeUtils.DrawResult(result)
                );
        }

        [Fact]
        public void bake_a_stretched_node_without_parent()
        {
            var threw = false;
            try
            {
                new RawLayout(LayoutNode.Leaf("Stretch", LayoutSize.StretchedBoth())).Bake();
            }
            catch (ImpossibleLayoutException)
            {
                threw = true;
            }

            threw.Should().BeTrue();
        }

        [Fact]
        public void alignment_with_varied_height_things_bottom()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(68, 20), new LayoutStyle(padding: 3, alignment: Alignment.BottomCenter),
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
        public void alignment_with_varied_height_things_centered()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(68, 20), new LayoutStyle(padding: 3, alignment: Alignment.Center),
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
        public void alignment_with_varied_height_things_top_right()
        {
            var layout = LayoutNode.HorizontalParent("root", LayoutSize.Pixels(68, 20), new LayoutStyle(padding: 3, alignment: Alignment.TopRight),
                LayoutNode.Leaf("itemA", LayoutSize.Pixels(10, 3)),
                LayoutNode.Leaf("itemB", LayoutSize.Pixels(12, 10)),
                LayoutNode.Leaf("itemC", LayoutSize.Pixels(8, 5)),
                LayoutNode.Leaf("itemD", LayoutSize.Pixels(9, 10)),
                LayoutNode.Leaf("itemE", LayoutSize.Pixels(7, 8))
            );

            var result = layout.Bake();
            Approvals.Verify(LayoutNodeUtils.DrawResult(result));
        }
    }
}