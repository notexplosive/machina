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
            subject.FormattedTokens().Should().HaveCount(11);
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
            subject.FormattedTokens().Should().HaveCount(5);
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
    }
}