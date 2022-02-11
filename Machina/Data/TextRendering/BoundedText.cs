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
        private readonly FormattedText formattedText;

        public int TotalCharacterCount => this.formattedText.TotalCharacterCount;
        public Rectangle TotalAvailableRect { get; }

        public BoundedText(Rectangle rect, Alignment alignment, Overflow overflow, FormattedText formattedText = default)
        {
            TotalAvailableRect = rect;
            this.alignment = alignment;
            this.tokenLookup = new Dictionary<int, TextOutputFragment>();
            this.formattedText = formattedText;

            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();
            var tokenIndex = 0;

            foreach (var token in formattedText.FormattedTokens())
            {
                var output = token.OutputFragment();
                childNodes.AddRange(output.Nodes);

                if (output.WillBeRendered)
                {
                    this.tokenLookup[tokenIndex] = output;
                    tokenIndex++;
                }
            }

            var layout = FlowLayout.HorizontalFlowParent("root", LayoutSize.Pixels(TotalAvailableRect.Size), new FlowLayoutStyle(alignment: this.alignment),
                childNodes.ToArray()
            );

            this.bakedLayout = layout.Bake();
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
                    var pendingRenderableText = outputFragment.CreateRenderableText(characterIndex, TotalAvailableRect.Location, tokenNode.Rectangle.Location);

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
