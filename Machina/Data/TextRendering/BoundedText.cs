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
        private readonly BakedFlowLayout bakedLayout;
        private readonly Dictionary<int, string> tokenLookup;

        public IFontMetrics FontMetrics { get; }
        public Rectangle TotalAvailableRect { get; }

        public BoundedText(string text, IFontMetrics font, Rectangle rect, Alignment alignment, Overflow overflow)
        {
            FontMetrics = font;
            TotalAvailableRect = rect;
            this.alignment = alignment;
            this.tokenLookup = new Dictionary<int, string>();

            var tokens = CreateTokens(text);

            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();
            var tokenIndex = 0;

            foreach (var token in tokens)
            {
                if (token == "\n")
                {
                    childNodes.Add(LayoutNode.NamelessLeaf(LayoutSize.Pixels(0, FontMetrics.LineSpacing)));
                    childNodes.Add(FlowLayoutInstruction.Linebreak);
                }
                else
                {
                    // Reducing the MeasuredString result to a Point (truncating floats to ints) is that a problem?
                    var size = FontMetrics.MeasureString(token).ToPoint();
                    var id = $"token-{tokenIndex}";
                    this.tokenLookup[tokenIndex] = token;
                    childNodes.Add(LayoutNode.Leaf(id, LayoutSize.Pixels(size)));
                    tokenIndex++;
                }
            }

            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(TotalAvailableRect.Size), new FlowLayoutStyle(alignment: this.alignment),
                childNodes.ToArray()
            );

            this.bakedLayout = layout.Bake();
        }

        public static string[] CreateTokens(string text)
        {
            var words = new List<string>();
            var pendingWord = new StringBuilder();
            foreach (var character in text)
            {
                if (character == ' ' || character == '\n')
                {
                    words.Add(pendingWord.ToString());
                    pendingWord.Clear();
                    words.Add(character.ToString());
                }
                else
                {
                    pendingWord.Append(character);
                }
            }

            if (pendingWord.Length > 0)
            {
                words.Add(pendingWord.ToString());
            }


            return words.ToArray();
        }

        public List<RenderableText> GetRenderedLines(Vector2 worldPos, Point drawOffset, Color textColor, float angle, Depth depth)
        {
            var renderableTexts = new List<RenderableText>();

            var lineIndex = 0;
            var tokenIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                var textContent = new StringBuilder();
                foreach (var item in row)
                {
                    textContent.Append(this.tokenLookup[tokenIndex]);
                    tokenIndex++;
                }

                renderableTexts.Add(new RenderableText(FontMetrics, textContent.ToString(), worldPos, textColor, drawOffset, angle, depth, TotalAvailableRect.Location, row.UsedRectangle));
                lineIndex++;
            }

            return renderableTexts;
        }

        public Rectangle GetRectOfLine(int lineIndex)
        {
            return bakedLayout.GetRow(lineIndex).UsedRectangle;
        }

        public Point TopLeftOfText()
        {
            return new Point(LeftOfText(), GetRectOfLine(0).Location.Y);
        }

        private int LeftOfText()
        {
            var xOffset = 0;
            var hasFirstOffset = false;
            var lineIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                var lineRelativePositionX = row.UsedRectangle.Location.X;
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
