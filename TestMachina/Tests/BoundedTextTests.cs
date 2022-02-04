using FluentAssertions;
using Machina.Components;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public class BoundedTextTests
    {
        [Fact]
        public void text_measurer_generates_accurate_output()
        {
            var textMeasurer = new TextMeasurer(
                "Hello world",
                new MonospacedFontMetrics(new Point(2, 3)),
                new Rectangle(new Point(0, 0), new Point(200, 200)),
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                Overflow.Ignore);

            textMeasurer.Lines.Should().HaveCount(1);

            var expectedX = 88;
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.X.Should().Be(expectedX);
            var localTextPos = textMeasurer.TopLeftOfText();
            localTextPos.Should().Be(new Point(expectedX, 99));
        }

        [Fact]
        public void pinning_tests_for_text_measurer_centered()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new TextMeasurer(
                "This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.",
                fontMetrics,
                new Rectangle(Point.Zero, new Point(100, 200)),
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(6, 97));

            textMeasurer.Lines.Should().HaveCount(3);
            textMeasurer.Lines[0].textContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.X.Should().Be(6);
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.Y.Should().Be(0);

            textMeasurer.Lines[1].textContent.Should().Be("referencing some meme here in this string. ");
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.X.Should().Be(7);
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.Y.Should().Be(3);

            textMeasurer.Lines[2].textContent.Should().Be("But then I changed my mind. ");
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.X.Should().Be(22);
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.Y.Should().Be(6);
        }

        [Fact]
        public void pinning_tests_for_text_measurer_topleft()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new TextMeasurer(
                "This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.",
                fontMetrics,
                new Rectangle(Point.Zero, new Point(100, 200)),
                HorizontalAlignment.Left,
                VerticalAlignment.Top,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(0, 0));

            textMeasurer.Lines.Should().HaveCount(3);
            textMeasurer.Lines[0].textContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.X.Should().Be(0);
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.Y.Should().Be(0);

            textMeasurer.Lines[1].textContent.Should().Be("referencing some meme here in this string. ");
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.X.Should().Be(0);
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.Y.Should().Be(3);

            textMeasurer.Lines[2].textContent.Should().Be("But then I changed my mind. ");
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.X.Should().Be(0);
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.Y.Should().Be(6);
        }

        [Fact]
        public void pinning_tests_for_text_measurer_bottomright()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new TextMeasurer(
                "This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.",
                fontMetrics,
                new Rectangle(Point.Zero, new Point(100, 200)),
                HorizontalAlignment.Right,
                VerticalAlignment.Bottom,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(12, 191));

            textMeasurer.Lines.Should().HaveCount(3);
            textMeasurer.Lines[0].textContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.X.Should().Be(12);
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.Y.Should().Be(0);

            textMeasurer.Lines[1].textContent.Should().Be("referencing some meme here in this string. ");
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.X.Should().Be(14);
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.Y.Should().Be(3);

            textMeasurer.Lines[2].textContent.Should().Be("But then I changed my mind. ");
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.X.Should().Be(44);
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.Y.Should().Be(6);
        }

        [Fact]
        public void pinning_tests_for_text_measurer_centered_large_middle_string()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new TextMeasurer(
                "Short top string\nlooooooooooong.... middle.... string\nshort bottom",
                fontMetrics,
                new Rectangle(Point.Zero, new Point(100, 200)),
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(13, 97));

            textMeasurer.Lines.Should().HaveCount(3);
            textMeasurer.Lines[0].textContent.Should().Be("Short top string ");
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.X.Should().Be(33);
            textMeasurer.Lines[0].positionRelativeToTopLeftOfRect.Y.Should().Be(0);

            textMeasurer.Lines[1].textContent.Should().Be("looooooooooong.... middle.... string ");
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.X.Should().Be(13);
            textMeasurer.Lines[1].positionRelativeToTopLeftOfRect.Y.Should().Be(3);

            textMeasurer.Lines[2].textContent.Should().Be("short bottom ");
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.X.Should().Be(37);
            textMeasurer.Lines[2].positionRelativeToTopLeftOfRect.Y.Should().Be(6);
        }

        [Fact]
        public void pinning_test_for_renderable_text_where_position_is_top_left_of_rect()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new TextMeasurer(
                "This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.",
                fontMetrics,
                new Rectangle(new Point(350, 250), new Point(100, 200)),
                HorizontalAlignment.Right,
                VerticalAlignment.Bottom,
                Overflow.Elide);

            var renderedLines = textMeasurer.GetRenderedLines(new Vector2(350, 250), new Point(-5, 0), Color.Red, 0f, 0);

            renderedLines.Should().HaveCount(3);

            renderedLines[0].PivotPosition.Should().Be(new Vector2(350, 250));
            renderedLines[0].OffsetFromPivot.Should().Be(new Vector2(-7, -191));

            renderedLines[1].PivotPosition.Should().Be(new Vector2(350, 250));
            renderedLines[1].OffsetFromPivot.Should().Be(new Vector2(-9, -194));

            renderedLines[2].PivotPosition.Should().Be(new Vector2(350, 250));
            renderedLines[2].OffsetFromPivot.Should().Be(new Vector2(-39, -197));
        }

        [Fact]
        public void pinning_test_for_renderable_text_where_position_is_center_of_rect()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));
            var rect = new Rectangle(new Point(350, 250), new Point(100, 200));

            var textMeasurer = new TextMeasurer(
                "This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.",
                fontMetrics,
                rect,
                HorizontalAlignment.Right,
                VerticalAlignment.Bottom,
                Overflow.Elide);

            var renderedLines = textMeasurer.GetRenderedLines(rect.Center.ToVector2(), new Point(-5, 0), Color.Red, 0f, 0);

            renderedLines.Should().HaveCount(3);

            renderedLines[0].PivotPosition.Should().Be(new Vector2(400, 350));
            renderedLines[0].OffsetFromPivot.Should().Be(new Vector2(43, -91));

            renderedLines[1].PivotPosition.Should().Be(new Vector2(400, 350));
            renderedLines[1].OffsetFromPivot.Should().Be(new Vector2(41, -94));

            renderedLines[2].PivotPosition.Should().Be(new Vector2(400, 350));
            renderedLines[2].OffsetFromPivot.Should().Be(new Vector2(11, -97));
        }
    }
}