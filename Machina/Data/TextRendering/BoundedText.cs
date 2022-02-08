using Machina.Components;
using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct BoundedText
    {
        private readonly Alignment alignment;
        private readonly BakedLayout bakedLayout;
        public IFontMetrics FontMetrics { get; }
        public Rectangle TotalAvailableRect { get; }

        public AssembledTextLines Lines { get; }

        public BoundedText(string text, IFontMetrics font, Rectangle rect, Alignment alignment, Overflow overflow)
        {
            this.FontMetrics = font;
            this.TotalAvailableRect = rect;
            this.alignment = alignment;
            Lines = new AssembledTextLines(text, font, TotalAvailableRect.Size, this.alignment, overflow);

            var lineIndex = 0;
            var childNodes = new List<LayoutNode>();


            foreach (var line in Lines)
            {
                childNodes.Add(
                    line.CreateLayoutNode($"line {lineIndex}")
                );
                lineIndex++;
            }

            var layout = LayoutNode.VerticalParent("root", LayoutSize.Pixels(this.TotalAvailableRect.Size), new LayoutStyle(alignment: this.alignment),
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
                renderableTexts.Add(new RenderableText(this.FontMetrics, line.TextContent, worldPos, textColor, drawOffset, angle, depth, this.TotalAvailableRect.Location, GetRectOfLine(lineIndex)));
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
            return new Point(LeftOfText(), this.GetRectOfLine(0).Location.Y);
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
