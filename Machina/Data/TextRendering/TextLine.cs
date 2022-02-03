using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly int nonAdjustedX;
        public readonly int nonAdjustedY;
        private readonly Rectangle bounds;

        public TextLine(string content, IFontMetrics fontMetrics, Rectangle bounds, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.bounds = bounds;
            this.textContent = content;
            this.nonAdjustedY = positionY;

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                this.nonAdjustedX = 0;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X + 1;
                this.nonAdjustedX = widthOffset;
            }
            else
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X / 2 + 1 - bounds.Width / 2;
                this.nonAdjustedX = widthOffset;
            }

        }
    }
}
