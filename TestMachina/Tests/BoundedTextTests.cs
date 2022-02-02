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

            var expectedX = 89;
            textMeasurer.Lines[0].textPosition.Should().Be(new Point(expectedX, 0));
            var localTextPos = textMeasurer.GetTextLocalPos();
            localTextPos.Should().Be(new Point(expectedX, 99));
        }

        [Fact]
        public void pinning_tests_for_text_measurer()
        {
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));

            var textMeasurer = new TextMeasurer(
                "This is a very long string. I thought about referencing some meme here in this string.\nBut then I changed my mind.",
                fontMetrics,
                new Rectangle(Point.Zero, new Point(100, 200)),
                HorizontalAlignment.Center,
                VerticalAlignment.Center,
                Overflow.Elide);

            textMeasurer.Lines.Should().HaveCount(3);
            textMeasurer.Lines[0].textContent.Should().Be("This is a very long string. I thought about ");
            textMeasurer.Lines[0].textPosition.Y.Should().Be(0);

            textMeasurer.Lines[1].textContent.Should().Be("referencing some meme here in this string. ");
            textMeasurer.Lines[1].textPosition.Y.Should().Be(3);

            textMeasurer.Lines[2].textContent.Should().Be("But then I changed my mind. ");
            textMeasurer.Lines[2].textPosition.Y.Should().Be(6);
        }
    }
}