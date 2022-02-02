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
        public void bounded_text_renderer_generates_accurate_output()
        {
            var verticalAlignment = VerticalAlignment.Center;
            var fontMetrics = new MonospacedFontMetrics(new Point(2, 3));
            var boundsHeight = 200;
            int worldPosX = 0;

            var textMeasurer = new TextMeasurer(
                "Hello world",
                fontMetrics,
                new Rectangle(new Point(worldPosX, 0), new Point(200, boundsHeight)),
                HorizontalAlignment.Center,
                verticalAlignment,
                Overflow.Ignore);

            textMeasurer.Lines.Should().HaveCount(1);

            var expectedX = 89;
            textMeasurer.Lines[0].textPosition.Should().Be(new Point(expectedX, 0));
            var localTextPos = textMeasurer.GetTextLocalPos(boundsHeight, worldPosX);
            localTextPos.Should().Be(new Point(expectedX, 99));
        }
    }
}