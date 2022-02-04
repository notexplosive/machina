using Machina.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct TextMeasurer
    {
        private readonly IFontMetrics fontMetrics;
        private readonly Rectangle totalAvailableRect;

        public AssembledTextLines Lines { get; }

        public TextMeasurer(string text, IFontMetrics font, Rectangle rect, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, Overflow overflow)
        {
            this.fontMetrics = font;
            this.totalAvailableRect = rect;
            Lines = new AssembledTextLines(text, font, totalAvailableRect.Size, new Alignment(horizontalAlignment, verticalAlignment), overflow);
        }

        public List<RenderableText> GetRenderedLines(Vector2 worldPos, Point drawOffset, Color textColor, float angle, Depth depth)
        {
            var renderableTexts = new List<RenderableText>();

            foreach (var line in Lines)
            {
                renderableTexts.Add(new RenderableText(this.fontMetrics, line, worldPos, textColor, drawOffset, angle, depth, new Vector2(this.totalAvailableRect.X, Lines.TopOfText + this.totalAvailableRect.Y)));
            }

            return renderableTexts;
        }

        public Point TopLeftOfText()
        {
            int yOffset = Lines.TopOfText;
            int xOffset = LeftOfText();

            return new Point(xOffset, yOffset);
        }

        private int LeftOfText()
        {
            var xOffset = 0;
            var hasFirstOffset = false;
            foreach (var line in Lines)
            {
                var lineRelativePositionX = line.PositionRelativeToTopLeftOfRect.X;
                if (!hasFirstOffset)
                {
                    xOffset = lineRelativePositionX;
                    hasFirstOffset = true;
                }
                else
                {
                    xOffset = Math.Min(line.PositionRelativeToTopLeftOfRect.X, xOffset);
                }
            }

            return xOffset;
        }
    }
}
