using ApprovalTests;
using ApprovalTests.Reporters;
using FluentAssertions;
using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;
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
    }
}