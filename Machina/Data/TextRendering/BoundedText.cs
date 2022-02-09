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
                    childNodes.Add(LayoutNode.Spacer(new Point(0, FontMetrics.LineSpacing)));
                    childNodes.Add(FlowLayoutInstruction.Linebreak);
                }
                else
                {
                    this.tokenLookup[tokenIndex] = token;

                    var tokenSize = FontMetrics.MeasureStringRounded(token);
                    var glyphNodes = new List<LayoutNode>();

                    foreach (var character in token)
                    {
                        var characterSize = FontMetrics.MeasureStringRounded(character.ToString());

                        glyphNodes.Add(LayoutNode.NamelessLeaf(LayoutSize.Pixels(characterSize)));
                    }

                    childNodes.Add(LayoutNode.NamelessHorizontalParent(LayoutSize.Pixels(tokenSize), LayoutStyle.Empty, glyphNodes.ToArray()));
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

            var tokenIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                foreach (var tokenNode in row)
                {
                    var tokenText = this.tokenLookup[tokenIndex];
                    renderableTexts.Add(new RenderableText(FontMetrics, tokenText, worldPos, textColor, drawOffset, angle, depth, TotalAvailableRect.Location, tokenNode.Rectangle));
                    tokenIndex++;
                }
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
