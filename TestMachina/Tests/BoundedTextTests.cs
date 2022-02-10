using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Components;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    [UseReporter(typeof(DiffReporter))]
    public class BoundedTextTests
    {
        [Fact]
        public void text_measurer_generates_accurate_output()
        {
            var textMeasurer = new BoundedText(
                new TextInputFragment("Hello world", new MonospacedFontMetrics(new Point(2, 3))),
                new Rectangle(new Point(0, 0), new Point(200, 200)),
                Alignment.Center,
                Overflow.Ignore);

            // textMeasurer.Lines.Should().HaveCount(1);

            var expectedX = 89;
            textMeasurer.GetRectOfLine(0).Location.X.Should().Be(expectedX);
            var localTextPos = textMeasurer.TopLeftOfText();
            localTextPos.Should().Be(new Point(expectedX, 99));
        }

        [Fact]
        public void pinning_tests_for_text_measurer_centered()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new BoundedText(
                new TextInputFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics),
                new Rectangle(Point.Zero, new Point(100, 200)),
                Alignment.Center,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(6, 96));

            // textMeasurer.Lines.Should().HaveCount(3);
            // textMeasurer.Lines[0].TextContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.GetRectOfLine(0).Location.X.Should().Be(6);
            textMeasurer.GetRectOfLine(0).Location.Y.Should().Be(96);

            // textMeasurer.Lines[1].TextContent.Should().Be("referencing some meme here in this string.");
            textMeasurer.GetRectOfLine(1).Location.X.Should().Be(8);
            textMeasurer.GetRectOfLine(1).Location.Y.Should().Be(99);

            // textMeasurer.Lines[2].TextContent.Should().Be("But then I changed my mind.");
            textMeasurer.GetRectOfLine(2).Location.X.Should().Be(23);
            textMeasurer.GetRectOfLine(2).Location.Y.Should().Be(102);
        }

        [Fact]
        public void pinning_tests_for_text_measurer_topleft()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new BoundedText(
                new TextInputFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics),
                new Rectangle(Point.Zero, new Point(100, 200)),
                Alignment.TopLeft,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(0, 0));

            // textMeasurer.Lines.Should().HaveCount(3);
            // textMeasurer.Lines[0].TextContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.GetRectOfLine(0).Location.X.Should().Be(0);
            textMeasurer.GetRectOfLine(0).Location.Y.Should().Be(0);

            // textMeasurer.Lines[1].TextContent.Should().Be("referencing some meme here in this string.");
            textMeasurer.GetRectOfLine(1).Location.X.Should().Be(0);
            textMeasurer.GetRectOfLine(1).Location.Y.Should().Be(3);

            // textMeasurer.Lines[2].TextContent.Should().Be("But then I changed my mind.");
            textMeasurer.GetRectOfLine(2).Location.X.Should().Be(0);
            textMeasurer.GetRectOfLine(2).Location.Y.Should().Be(6);
        }

        [Fact]
        public void pinning_tests_for_text_measurer_bottomright()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new BoundedText(
                new TextInputFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics),
                new Rectangle(Point.Zero, new Point(100, 200)),
                Alignment.BottomRight,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(12, 191));

            // textMeasurer.Lines.Should().HaveCount(3);
            // textMeasurer.Lines[0].TextContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.GetRectOfLine(0).Location.X.Should().Be(12);
            textMeasurer.GetRectOfLine(0).Location.Y.Should().Be(191);

            // textMeasurer.Lines[1].TextContent.Should().Be("referencing some meme here in this string.");
            textMeasurer.GetRectOfLine(1).Location.X.Should().Be(16);
            textMeasurer.GetRectOfLine(1).Location.Y.Should().Be(194);

            // textMeasurer.Lines[2].TextContent.Should().Be("But then I changed my mind.");
            textMeasurer.GetRectOfLine(2).Location.X.Should().Be(46);
            textMeasurer.GetRectOfLine(2).Location.Y.Should().Be(197);
        }

        [Fact]
        public void pinning_tests_for_text_measurer_centered_large_middle_string()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new BoundedText(
                new TextInputFragment("Short top string\nlooooooooooong.... middle.... string\nshort bottom", fontMetrics),
                new Rectangle(Point.Zero, new Point(100, 200)),
                Alignment.Center,
                Overflow.Elide);

            textMeasurer.TopLeftOfText().Should().Be(new Point(14, 96));

            // textMeasurer.Lines.Should().HaveCount(3);
            // textMeasurer.Lines[0].TextContent.Should().Be("Short top string");
            textMeasurer.GetRectOfLine(0).Location.X.Should().Be(34);
            textMeasurer.GetRectOfLine(0).Location.Y.Should().Be(96);

            // textMeasurer.Lines[1].TextContent.Should().Be("looooooooooong.... middle.... string");
            textMeasurer.GetRectOfLine(1).Location.X.Should().Be(14);
            textMeasurer.GetRectOfLine(1).Location.Y.Should().Be(99);

            // textMeasurer.Lines[2].TextContent.Should().Be("short bottom");
            textMeasurer.GetRectOfLine(2).Location.X.Should().Be(38);
            textMeasurer.GetRectOfLine(2).Location.Y.Should().Be(102);
        }

        [Fact]
        public void pinning_test_for_renderable_text_where_position_is_top_left_of_rect()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new BoundedText(
                new TextInputFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics),
                new Rectangle(new Point(350, 250), new Point(100, 200)),
                Alignment.BottomRight,
                Overflow.Elide);

            var renderedLines = textMeasurer.GetRenderedText(Color.Red);

            renderedLines.Should().HaveCount(42);

            renderedLines[0].Origin.Should().Be(new Point(350, 250));
            renderedLines[0].Offset.Should().Be(new Point(-12, -191));
            renderedLines[0].Text.Should().Be("This");

            renderedLines[18].Text.Should().Be("referencing");
            renderedLines[18].Origin.Should().Be(new Point(350, 250));
            renderedLines[18].Offset.Should().Be(new Point(-16, -194));

            renderedLines[31].Text.Should().Be("But");
            renderedLines[31].Origin.Should().Be(new Point(350, 250));
            renderedLines[31].Offset.Should().Be(new Point(-46, -197));
        }

        [Fact]
        public void text_rendering_pinning_approval()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(
                new TextInputFragment("I'm the rootinest tootinest gunslinger on this here side of the mississouri.\n\nSo watch out!", fontMetrics),
                rect,
                Alignment.Center,
                Overflow.Elide);

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void rendered_text_know_what_parts_of_the_string_they_own()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(
                new TextInputFragment("This is a string used for testing.", fontMetrics),
                rect,
                Alignment.Center,
                Overflow.Elide);

            var renderedText = textMeasurer.GetRenderedText(Color.White);

            renderedText.Should().HaveCount(13);
            renderedText[0].CharacterPosition.Should().Be(0);
            renderedText[0].CharacterLength.Should().Be(4);

            renderedText[1].CharacterPosition.Should().Be(4);
            renderedText[1].CharacterLength.Should().Be(1);

            renderedText[2].CharacterPosition.Should().Be(5);
            renderedText[2].CharacterLength.Should().Be(2);

            renderedText[3].CharacterPosition.Should().Be(7);
            renderedText[3].CharacterLength.Should().Be(1);
        }

        [Fact]
        public void newlines_are_not_counted_as_rendered_text()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("New\nLine", fontMetrics), rect, Alignment.Center, Overflow.Elide);

            var renderedText = textMeasurer.GetRenderedText(Color.White);

            renderedText.Should().HaveCount(2);
            renderedText[0].CharacterPosition.Should().Be(0);
            renderedText[0].CharacterLength.Should().Be(3);

            renderedText[1].CharacterPosition.Should().Be(3);
            renderedText[1].CharacterLength.Should().Be(4);
        }

        [Fact]
        public void occlude_zero_characters_by_default()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("This is a test", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White);

            textList.Should().HaveCount(7);
            textList[6].Text.Should().Be("test");
        }

        [Fact]
        public void occlude_characters_in_last_token()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("This is a test", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White, 1);

            textList.Should().HaveCount(7);
            textList[6].Text.Should().Be("tes");
        }

        [Fact]
        public void occlude_entire_last_token()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("This is a test", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White, 4);

            textList.Should().HaveCount(6);
            textList[5].Text.Should().Be(" ");
        }

        [Fact]
        public void occlude_last_two_token()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("This is a test", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White, 5);

            textList.Should().HaveCount(5);
            textList[4].Text.Should().Be("a");
        }

        [Fact]
        public void occlude_half_of_string()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("Ragglest the Fragglest", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White, 12);

            textList.Should().HaveCount(3);
            textList[2].Text.Should().Be("t");
        }

        [Fact]
        public void occlude_whole_string()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("Hi", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White, 2);

            textList.Should().HaveCount(0);
        }

        [Fact]
        public void occlude_way_too_much()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var rect = new Rectangle(Point.Zero, new Point(60, 40));

            var textMeasurer = new BoundedText(new TextInputFragment("Hi", fontMetrics), rect, Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText(Color.White, 999999);

            textList.Should().HaveCount(0);
        }
    }
}