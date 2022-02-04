using Machina.Components;
using Machina.Data.Layout;
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
        private readonly Alignment alignment;
        private readonly BakedLayout bakedLayout;

        public AssembledTextLines Lines { get; }

        public TextMeasurer(string text, IFontMetrics font, Rectangle rect, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, Overflow overflow)
        {
            this.fontMetrics = font;
            this.totalAvailableRect = rect;
            this.alignment = new Alignment(horizontalAlignment, verticalAlignment);
            Lines = new AssembledTextLines(text, font, totalAvailableRect.Size, this.alignment, overflow);

            var lineIndex = 0;
            var childNodes = new List<LayoutNode>();


            foreach (var line in Lines)
            {
                childNodes.Add(
                    line.CreateLayoutNode($"line {lineIndex}")
                );
                lineIndex++;
            }

            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(this.totalAvailableRect.Size), new LayoutStyle(alignment: this.alignment),
                childNodes.ToArray()
            );

            this.bakedLayout = layout.Bake();
        }

        public List<RenderableText> GetRenderedLines(Vector2 worldPos, Point drawOffset, Color textColor, float angle, Depth depth)
        {
            var renderableTexts = new List<RenderableText>();

            var lineIndex = 0;
            foreach (var line in Lines)
            {
                renderableTexts.Add(new RenderableText(this.fontMetrics, line, worldPos, textColor, drawOffset, angle, depth, this.totalAvailableRect.Location, GetRectOfLine(lineIndex)));
                lineIndex++;
            }

            return renderableTexts;
        }

        public Rectangle GetRectOfLine(int lineIndex)
        {
            return bakedLayout.GetNode($"line {lineIndex}").Rectangle;
        }

        public Point TopLeftOfText()
        {
            int yOffset = GetRectOfLine(0).Location.Y;
            int xOffset = LeftOfText();

            return new Point(xOffset, yOffset);
        }

        private int LeftOfText()
        {
            var xOffset = 0;
            var hasFirstOffset = false;
            var lineIndex = 0;
            foreach (var line in Lines)
            {
                var lineRelativePositionX = GetRectOfLine(lineIndex).Location.X;
                if (!hasFirstOffset)
                {
                    xOffset = lineRelativePositionX;
                    hasFirstOffset = true;
                }
                else
                {
                    xOffset = Math.Min(lineRelativePositionX, xOffset);
                }

                lineIndex++;
            }

            return xOffset;
        }
    }
}
