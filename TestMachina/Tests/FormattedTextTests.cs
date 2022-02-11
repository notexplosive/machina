using FluentAssertions;
using Machina.Data.TextRendering;
using Microsoft.Xna.Framework;
using Xunit;

namespace TestMachina.Tests
{
    public class FormattedTextTests
    {
        [Fact]
        public void formatted_text_pinning()
        {
            var font1 = new MonospacedFontMetrics(new Point(4, 4));
            var font2 = new MonospacedFontMetrics(new Point(4, 5));

            var subject = new FormattedText(new FormattedTextFragment("Hello in blue. ", font1, Color.Blue), new FormattedTextFragment("Hello in red!", font2, Color.Red));

            subject.FormattedTokens().Should().HaveCount(11);
        }
    }
}