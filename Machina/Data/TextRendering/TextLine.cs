using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly Point textPosition;

        public TextLine(string content, SpriteFontMetrics fontMetrics, Rectangle bounds, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.textPosition = new Point(0, positionY);

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                this.textPosition.X = bounds.Location.X;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X + 1;
                this.textPosition.X = bounds.Location.X + widthOffset;
            }
            else
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X / 2 + 1 - bounds.Width / 2;
                this.textPosition.X = bounds.Location.X + widthOffset;
            }
        }
    }
}
