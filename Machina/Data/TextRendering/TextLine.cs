using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly int adjustedX;
        public readonly int nonAdjustedY;

        public TextLine(string content, IFontMetrics fontMetrics, Rectangle bounds, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.adjustedX = 0;
            this.nonAdjustedY = positionY;

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                this.adjustedX = bounds.Location.X;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X + 1;
                this.adjustedX = bounds.Location.X + widthOffset;
            }
            else
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X / 2 + 1 - bounds.Width / 2;
                this.adjustedX = bounds.Location.X + widthOffset;
            }
        }
    }
}
