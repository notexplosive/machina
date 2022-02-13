using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using TestMachina.Utility;
using Xunit;

namespace TestMachina.Tests
{
    [UseReporter(typeof(DiffReporter))]
    public class FormattedTextTests
    {
        [Fact]
        public void formatted_text_gets_right_number_of_tokens()
        {
            var font1 = new MonospacedFontMetrics(new Point(4, 4));
            var font2 = new MonospacedFontMetrics(new Point(4, 5));

            var subject = new FormattedText(
                new FormattedTextFragment("Hello in blue. ", font1, Color.Blue),
                new FormattedTextFragment("Hello in red!", font2, Color.Red)
                );

            subject.OutputString.Should().Be("Hello in blue. Hello in red!");
            subject.OutputFragments().Should().HaveCount(11);
        }

        [Fact]
        public void formatted_text_can_render_itself()
        {
            var font1 = new MonospacedFontMetrics(new Point(4, 4));
            var font2 = new MonospacedFontMetrics(new Point(4, 5));

            var subject = new FormattedText(
                new FormattedTextFragment("Hello in blue. ", font1, Color.Blue),
                new FormattedTextFragment("Hello in red!", font2, Color.Red)
                );

            var renderedText = subject.GetRenderedText(Point.Zero);

            Approvals.Verify(TextMeasureUtils.DrawRenderedText(new AsciiDrawPanel(new Point(113, 5)), renderedText));
        }

        [Fact]
        public void formatted_text_from_string()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var subject = FormattedText.FromString("Hello in blue.", font, Color.Blue);

            subject.OutputString.Should().Be("Hello in blue.");
            subject.OutputFragments().Should().HaveCount(5);
        }

        [Fact]
        public void parse_instructions_to_make_formatted_text()
        {
            var font = new MonospacedFontMetrics(new Point(4, 4));
            var subject = FormattedText.FromParseString("Hello in [#color:0000ff]blue[#color:ffffff].", font, Color.White);

            subject.OutputString.Should().Be("Hello in blue.");
            var renderedText = subject.GetRenderedText(Point.Zero);

            renderedText.Count.Should().Be(6);
        }

        [Fact]
        public void output_fragments_know_what_parts_of_the_string_they_own()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var subject = new FormattedText(new FormattedTextFragment("This is a string used for testing.", fontMetrics, Color.White));

            var outputFragment = new List<TextOutputFragment>(subject.OutputFragments());

            outputFragment.Should().HaveCount(13);
            outputFragment[0].CharacterPosition.Should().Be(0);
            outputFragment[0].CharacterLength.Should().Be(4);

            outputFragment[1].CharacterPosition.Should().Be(4);
            outputFragment[1].CharacterLength.Should().Be(1);

            outputFragment[2].CharacterPosition.Should().Be(5);
            outputFragment[2].CharacterLength.Should().Be(2);

            outputFragment[3].CharacterPosition.Should().Be(7);
            outputFragment[3].CharacterLength.Should().Be(1);
        }

        [Fact]
        public void formatted_text_can_include_things_that_are_not_text()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var subject = new FormattedText(
                new FormattedTextFragment("Text", fontMetrics, Color.White),
                new ImageTextFragment(new Point(20, 20)),
                new FormattedTextFragment("and more text", fontMetrics, Color.White)
            );
            var outputFragment = new List<TextOutputFragment>(subject.OutputFragments());

            subject.OutputString.Should().Be("Text{symbol}and more text");

            outputFragment.Should().HaveCount(7);


            outputFragment[0].CharacterPosition.Should().Be(0);
            outputFragment[0].CharacterLength.Should().Be(4);

            outputFragment[1].CharacterPosition.Should().Be(4);
            outputFragment[1].CharacterLength.Should().Be(1);

            outputFragment[2].CharacterPosition.Should().Be(5);
            outputFragment[2].CharacterLength.Should().Be(3);


            outputFragment[3].CharacterPosition.Should().Be(8);
            outputFragment[3].CharacterLength.Should().Be(1);

            outputFragment[4].CharacterPosition.Should().Be(9);
            outputFragment[4].CharacterLength.Should().Be(4);

            outputFragment[5].CharacterPosition.Should().Be(13);
            outputFragment[5].CharacterLength.Should().Be(1);

            outputFragment[6].CharacterPosition.Should().Be(14);
            outputFragment[6].CharacterLength.Should().Be(4);
        }

        [Fact]
        public void formatted_text_draws_things_that_are_not_text()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(4, 4));
            var subject = new FormattedText(
                new FormattedTextFragment("Text", fontMetrics, Color.White),
                new ImageTextFragment(new Point(20, 20)),
                new FormattedTextFragment("and more text", fontMetrics, Color.White)
            );
            var outputFragment = new List<TextOutputFragment>(subject.OutputFragments());

            subject.OutputString.Should().Be("Text{symbol}and more text");

            outputFragment.Should().HaveCount(7);

            Approvals.Verify(TextMeasureUtils.DrawRenderedText(new AsciiDrawPanel(new Point(88, 20)), subject.GetRenderedText(Point.Zero)));
        }
    }
}