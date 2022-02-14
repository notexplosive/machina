using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Components;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
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
                new Point(200, 200),
                Alignment.Center,
                Overflow.Ignore,
                new FormattedText(new FormattedTextFragment("Hello world", new MonospacedFontMetrics(new Point(2, 3)), Color.White)));

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
                new Point(100, 200),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(new FormattedTextFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics, Color.White)));

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
                new Point(100, 200),
                Alignment.TopLeft,
                Overflow.Elide,
                new FormattedText(new FormattedTextFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics, Color.White)));

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
                new Point(100, 200),
                Alignment.BottomRight,
                Overflow.Elide,
                new FormattedText(new FormattedTextFragment("This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.", fontMetrics, Color.White)));

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
                new Point(100, 200),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(new FormattedTextFragment("Short top string\nlooooooooooong.... middle.... string\nshort bottom", fontMetrics, Color.White)));

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
        public void text_rendering_pinning_approval()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));

            var textMeasurer = new BoundedText(
                new Point(60, 40),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(new FormattedTextFragment("I'm the rootinest tootinest gunslinger on this here side of the mississouri.\n\nSo watch out!", fontMetrics, Color.White)));

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void newlines_are_not_counted_as_rendered_text()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));

            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("New\nLine", fontMetrics, Color.White)));

            var renderedText = textMeasurer.GetRenderedText();

            renderedText.Should().HaveCount(2);
            renderedText[0].Text.Should().Be("New");
            renderedText[1].Text.Should().Be("Line");
        }

        [Fact]
        public void occlude_zero_characters_by_default()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));

            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("This is a test", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText();

            textList.Should().HaveCount(7);
            textList[6].Text.Should().Be("test");
        }

        [Fact]
        public void occlude_characters_in_last_token()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));

            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("This is a test", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText(occludedCharactersCount: 1);

            textList.Should().HaveCount(7);
            textList[6].Text.Should().Be("tes");
        }

        [Fact]
        public void occlude_entire_last_token()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));

            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("This is a test", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText(occludedCharactersCount: 4);

            textList.Should().HaveCount(6);
            textList[5].Text.Should().Be(" ");
        }

        [Fact]
        public void occlude_last_two_token()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));


            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("This is a test", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText(occludedCharactersCount: 5);

            textList.Should().HaveCount(5);
            textList[4].Text.Should().Be("a");
        }

        [Fact]
        public void occlude_half_of_string()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));


            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("Ragglest the Fragglest", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText(occludedCharactersCount: 12);

            textList.Should().HaveCount(3);
            textList[2].Text.Should().Be("t");
        }

        [Fact]
        public void occlude_whole_string()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));


            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("Hi", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText(occludedCharactersCount: 2);

            textList.Should().HaveCount(0);
        }

        [Fact]
        public void occlude_way_too_much()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));


            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide, new FormattedText(new FormattedTextFragment("Hi", fontMetrics, Color.White)));
            var textList = textMeasurer.GetRenderedText(occludedCharactersCount: 999999);

            textList.Should().HaveCount(0);
        }

        [Fact]
        public void bounded_text_can_have_zero_fragments()
        {
            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide);
            var textList = textMeasurer.GetRenderedText();

            textList.Should().HaveCount(0);
        }

        [Fact]
        public void bounded_text_can_have_multiple_fragments()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));


            var textMeasurer = new BoundedText(new Point(60, 40), Alignment.Center, Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("He", fontMetrics, Color.White),
                    new FormattedTextFragment("llo ", fontMetrics, Color.White),
                    new FormattedTextFragment("World!", fontMetrics, Color.White)
                ));
            var textList = textMeasurer.GetRenderedText();

            textList.Should().HaveCount(4);
            textList[0].Text.Should().Be("He");
            textList[1].Text.Should().Be("llo");
            textList[2].Text.Should().Be(" ");
            textList[3].Text.Should().Be("World!");
        }

        [Fact]
        public void bounded_text_can_support_different_font_sizes()
        {
            var small = new MonospacedFontMetrics(new Point(4, 4));
            var tall = new MonospacedFontMetrics(new Point(4, 8));
            var medium = new MonospacedFontMetrics(new Point(6, 6));
            var big = new MonospacedFontMetrics(new Point(12, 12));

            var textMeasurer = new BoundedText(
                new Point(82, 20),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("Go", small, Color.White),
                    new FormattedTextFragment("big", big, Color.White),
                    new FormattedTextFragment("or", tall, Color.White),
                    new FormattedTextFragment("go", medium, Color.White),
                    new FormattedTextFragment("home", small, Color.White)
            ));

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void changing_fonts_can_cause_natural_linebreaks()
        {
            var small = new MonospacedFontMetrics(new Point(4, 4));
            var medium = new MonospacedFontMetrics(new Point(6, 6));

            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("Changing fonts can cause a natural line", small, Color.White),
                    new FormattedTextFragment("break", medium, Color.White)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void multi_line_with_image()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.TopLeft,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("Wordswords\n", font, Color.Orange),
                    new ImageTextFragment(new Point(8, 8), null),
                    new FormattedTextFragment("words words", font, Color.LightBlue)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void several_linebreaks_back_to_back()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.TopLeft,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("One line\nTwo lines\n\nThree lines\n\n\nDone!", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_one_line_one_long_word()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 4),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("goooooooooooooo!", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_one_line_long_word()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 4),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("let's goooooooooooooo!", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_long_word()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("I'm about to walk\noff the edge of the string\nhere I goooooooooooooo!", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_words_on_next_line()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("I'm about to walk\noff the edge of the string\nhere I go! and more words words words", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_words_on_next_line_exact_length()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    // the typo in "aand" is intentional here
                    new FormattedTextFragment("I'm about to walk\noff the edge of the string\nhere I go! aand more words words words", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_words_many_short_words()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(50, 30),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("I'm about to walk\noff the edge of the string\nhere I go!\na b c d e f g", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }

        [Fact]
        public void elide_words_narrow_box()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var textMeasurer = new BoundedText(
                new Point(10, 30),
                Alignment.Center,
                Overflow.Elide,
                new FormattedText(
                    new FormattedTextFragment("O Nothing fits in this box", font, Color.Orange)
                )
            );

            Approvals.Verify(TextMeasureUtils.DrawResult(textMeasurer));
        }
    }
}