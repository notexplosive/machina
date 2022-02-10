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
        public readonly string[] Tokens;
        public IFontMetrics FontMetrics { get; }

        public TextInputFragment(string text, IFontMetrics fontMetrics)
        {
            Tokens = CreateTokens(text);
            FontMetrics = fontMetrics;
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
        private readonly int totalCharacterCount;

        public Rectangle TotalAvailableRect { get; }

        public BoundedText(Rectangle rect, Alignment alignment, Overflow overflow, params TextInputFragment[] textFragments)
        {
            TotalAvailableRect = rect;
            this.alignment = alignment;
            this.tokenLookup = new Dictionary<int, TextOutputFragment>();
            this.totalCharacterCount = 0;

            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();
            var tokenIndex = 0;

            foreach (var textFragment in textFragments)
            {
                foreach (var token in textFragment.Tokens)
                {
                    if (token == "\n")
                    {
                        childNodes.Add(LayoutNode.Spacer(new Point(0, textFragment.FontMetrics.LineSpacing)));
                        childNodes.Add(FlowLayoutInstruction.Linebreak);
                    }
                    else
                    {
                        this.tokenLookup[tokenIndex] = new TextOutputFragment(token, textFragment.FontMetrics);

                        var tokenSize = textFragment.FontMetrics.MeasureStringRounded(token);
                        childNodes.Add(LayoutNode.NamelessLeaf(LayoutSize.Pixels(tokenSize)));
                        tokenIndex++;
                    }
                }
            }

            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(TotalAvailableRect.Size), new FlowLayoutStyle(alignment: this.alignment),
                childNodes.ToArray()
            );

            this.bakedLayout = layout.Bake();


            // We count "total characters" as all characters that we actually end up using.
            // newlines are not counted as characters.
            foreach (var outputFragment in this.tokenLookup.Values)
            {
                this.totalCharacterCount += outputFragment.Text.Length;
            }
        }


        public List<RenderableText> GetRenderedText(Color textColor, int occludedCharactersCount = 0)
        {
            var result = new List<RenderableText>();

            var renderCutoffIndex = this.totalCharacterCount - occludedCharactersCount;

            var tokenIndex = 0;
            var characterIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                foreach (var tokenNode in row)
                {
                    var outputFragment = this.tokenLookup[tokenIndex];
                    var pendingRenderableText = new RenderableText(outputFragment.FontMetrics, outputFragment.Text, characterIndex, TotalAvailableRect.Location, textColor, tokenNode.Rectangle.Location);

                    var lastCharacterInThisText = pendingRenderableText.CharacterPosition + pendingRenderableText.CharacterLength;
                    if (renderCutoffIndex <= lastCharacterInThisText)
                    {
                        var substringLength = renderCutoffIndex - lastCharacterInThisText + pendingRenderableText.CharacterLength;

                        if (substringLength <= 0)
                        {
                            return result;
                        }

                        result.Add(pendingRenderableText.WithText(outputFragment.Text.Substring(0, substringLength)));
                        return result;
                    }

                    result.Add(pendingRenderableText);
                    characterIndex += outputFragment.Text.Length;
                    tokenIndex++;
                }
            }

            return result;
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
