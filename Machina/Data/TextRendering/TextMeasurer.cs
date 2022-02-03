using Machina.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct TextMeasurer
    {
        private readonly VerticalAlignment verticalAlignment;
        private readonly IFontMetrics fontMetrics;
        private readonly Rectangle totalAvailableRect;

        public TextLines Lines { get; }

        public TextMeasurer(string text, IFontMetrics font, Rectangle rect, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, Overflow overflow)
        {
            this.fontMetrics = font;
            this.totalAvailableRect = rect;
            this.verticalAlignment = verticalAlignment;
            Lines = new TextLines(text, font, totalAvailableRect, new Alignment(horizontalAlignment, verticalAlignment), overflow);
        }

        public List<RenderableText> GetRenderedLines(Vector2 worldPos, Point drawOffset, Color textColor, float angle, Depth depth)
        {
            var renderableTexts = new List<RenderableText>();

            foreach (var line in Lines)
            {
                renderableTexts.Add(new RenderableText(this.fontMetrics, line, worldPos, textColor, drawOffset, angle, depth, TopOfText() + this.totalAvailableRect.Y, this.totalAvailableRect.X));
            }

            return renderableTexts;
        }

        public Point TopLeftOfText()
        {
            int yOffset = TopOfText();
            int xOffset = LeftOfText();

            return new Point(xOffset, yOffset);
        }

        private int LeftOfText()
        {
            var xOffset = 0;
            var hasFirstOffset = false;
            foreach (var line in Lines)
            {
                var lineRelativePositionX = line.positionRelativeToTopLeftOfText.X;
                if (!hasFirstOffset)
                {
                    xOffset = lineRelativePositionX;
                    hasFirstOffset = true;
                }
                else
                {
                    xOffset = Math.Min(line.positionRelativeToTopLeftOfText.X, xOffset);
                }
            }

            return xOffset;
        }

        public int TopOfText()
        {
            var boundsHeight = this.totalAvailableRect.Height;

            var yOffset = 0;
            if (this.verticalAlignment == VerticalAlignment.Center)
            {
                yOffset = boundsHeight / 2 - this.fontMetrics.LineSpacing / 2 * Lines.Count;
            }
            else if (this.verticalAlignment == VerticalAlignment.Bottom)
            {
                yOffset = boundsHeight - this.fontMetrics.LineSpacing * Lines.Count;
            }

            return yOffset;
        }
    }
}
