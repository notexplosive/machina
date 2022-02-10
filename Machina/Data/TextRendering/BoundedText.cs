using Machina.Components;
using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.TextRendering
{
    public readonly struct BoundedText
    {
        private readonly Alignment alignment;
        private readonly BakedFlowLayout bakedLayout;
        private readonly Dictionary<int, TextOutputFragment> tokenLookup;
        public int TotalCharacterCount { get; }
        public Rectangle TotalAvailableRect { get; }

        public BoundedText(Rectangle rect, Alignment alignment, Overflow overflow, params ITextInputFragment[] textFragments)
        {
            TotalAvailableRect = rect;
            this.alignment = alignment;
            this.tokenLookup = new Dictionary<int, TextOutputFragment>();
            TotalCharacterCount = 0;

            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();
            var tokenIndex = 0;

            foreach (var textFragment in textFragments)
            {
                foreach (var token in textFragment.Tokens())
                {
                    childNodes.AddRange(token.Nodes);

                    if (token.ShouldBeCounted)
                    {
                        this.tokenLookup[tokenIndex] = new TextOutputFragment(token.Text, token.ParentFragment.FontMetrics, token.ParentFragment.Color);
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
                TotalCharacterCount += outputFragment.Text.Length;
            }
        }


        public List<RenderableText> GetRenderedText(int occludedCharactersCount = 0)
        {
            var result = new List<RenderableText>();

            var renderCutoffIndex = TotalCharacterCount - occludedCharactersCount;

            var tokenIndex = 0;
            var characterIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                foreach (var tokenNode in row)
                {
                    var outputFragment = this.tokenLookup[tokenIndex];
                    var pendingRenderableText = new RenderableText(outputFragment.FontMetrics, outputFragment.Text, characterIndex, TotalAvailableRect.Location, outputFragment.Color, tokenNode.Rectangle.Location);

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
