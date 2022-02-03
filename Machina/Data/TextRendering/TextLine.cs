using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly Point positionRelativeToTopLeftOfRect;

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.positionRelativeToTopLeftOfRect.Y = positionY;

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                this.positionRelativeToTopLeftOfRect.X = 0;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = availableSpace.X - (int)fontMetrics.MeasureString(content).X + 1;
                this.positionRelativeToTopLeftOfRect.X = widthOffset;
            }
            else
            {
                var widthOffset = availableSpace.X - (int)fontMetrics.MeasureString(content).X / 2 + 1 - availableSpace.X / 2;
                this.positionRelativeToTopLeftOfRect.X = widthOffset;
            }

        }
    }
}
