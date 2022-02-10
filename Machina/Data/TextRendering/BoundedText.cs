using Machina.Components;
using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct TextInputFragment
    {
        public string[] tokens { get; }
        public IFontMetrics font { get; }

        public TextInputFragment(string text, IFontMetrics fontMetrics)
        {
            tokens = CreateTokens(text);
            font = fontMetrics;
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
    }

    public readonly struct TextOutputFragment
    {
        public TextOutputFragment(string token, IFontMetrics fontMetrics)
        {
            Text = token;
            FontMetrics = fontMetrics;
        }

        public string Text { get; }
        public IFontMetrics FontMetrics { get; }
    }

    public readonly struct BoundedText
    {
        private readonly Alignment alignment;
        private readonly BakedFlowLayout bakedLayout;
        private readonly Dictionary<int, TextOutputFragment> tokenLookup;
        public Rectangle TotalAvailableRect { get; }

        public BoundedText(TextInputFragment textFragment, Rectangle rect, Alignment alignment, Overflow overflow)
        {
            TotalAvailableRect = rect;
            this.alignment = alignment;
            this.tokenLookup = new Dictionary<int, TextOutputFragment>();

            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();
            var tokenIndex = 0;

            foreach (var token in textFragment.tokens)
            {
                if (token == "\n")
                {
                    childNodes.Add(LayoutNode.Spacer(new Point(0, textFragment.font.LineSpacing)));
                    childNodes.Add(FlowLayoutInstruction.Linebreak);
                }
                else
                {
                    this.tokenLookup[tokenIndex] = new TextOutputFragment(token, textFragment.font);

                    var tokenSize = textFragment.font.MeasureStringRounded(token);
                    var glyphNodes = new List<LayoutNode>();

                    foreach (var character in token)
                    {
                        var characterSize = textFragment.font.MeasureStringRounded(character.ToString());

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


        public List<RenderableText> GetRenderedText(Point drawOffset, Color textColor)
        {
            var renderableTexts = new List<RenderableText>();

            var tokenIndex = 0;
            var characterIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                foreach (var tokenNode in row)
                {
                    var outputFragment = this.tokenLookup[tokenIndex];
                    renderableTexts.Add(new RenderableText(outputFragment.FontMetrics, outputFragment.Text, characterIndex, TotalAvailableRect.Location, textColor, drawOffset, tokenNode.Rectangle));
                    characterIndex += outputFragment.Text.Length;
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
