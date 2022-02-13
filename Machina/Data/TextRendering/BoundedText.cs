﻿using Machina.Components;
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
        private readonly List<TextOutputFragment> renderedFragments;
        private readonly List<TextOutputFragment> allOutputFragments;
        private readonly FormattedText formattedText;

        public int TotalCharacterCount => this.formattedText.TotalCharacterCount;
        public Point TotalAvailableSize { get; }

        public BoundedText(Point size, Alignment alignment, Overflow overflow, FormattedText formattedText = default)
        {
            TotalAvailableSize = size;
            this.alignment = alignment;
            this.renderedFragments = new List<TextOutputFragment>();
            this.allOutputFragments = new List<TextOutputFragment>();
            this.formattedText = formattedText;
            this.bakedLayout = null;

            foreach (var outputFragment in formattedText.OutputFragments())
            {
                this.allOutputFragments.Add(outputFragment);
            }

            var bakeResult = BakeFromTokens();
            this.bakedLayout = bakeResult.Item1;
            var numberOfRemovedTokens = bakeResult.Item2;

            for (int i = 0; i < numberOfRemovedTokens; i++)
            {
                this.allOutputFragments.RemoveAt(this.allOutputFragments.Count - 1);
            }

            if (OverflowAmount() > 0 || numberOfRemovedTokens > 0)
            {
                var lastToken = this.allOutputFragments[this.allOutputFragments.Count - 1];
                var lastDrawable = lastToken.Drawable;
                int ellipseSize = lastDrawable.EllipseWidth();
                var shrinkAmount = OverflowAmount() + ellipseSize;

                if (OverflowAmount() <= 0)
                {
                    if (shrinkAmount > 0)
                    {
                        lastDrawable = lastDrawable.ShrinkBy(shrinkAmount);
                    }
                    lastDrawable = lastDrawable.AppendEllipse();
                }
                else if (lastDrawable.Size.X > shrinkAmount)
                {
                    // eliding this element is enough to make room
                    lastDrawable = lastDrawable.ShrinkBy(shrinkAmount);
                    lastDrawable = lastDrawable.AppendEllipse();
                }

                this.allOutputFragments[this.allOutputFragments.Count - 1] = new TextOutputFragment(lastDrawable, lastToken.CharacterPosition);
            }

            this.bakedLayout = BakeFromTokens().Item1;

            foreach (var outputFragment in this.allOutputFragments)
            {
                if (outputFragment.WillBeRendered)
                {
                    this.renderedFragments.Add(outputFragment);
                }
            }
        }

        private Tuple<BakedFlowLayout, int> BakeFromTokens()
        {
            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();

            foreach (var token in this.allOutputFragments)
            {
                childNodes.Add(token.Node);
            }

            var layout = MakeLayout(childNodes);
            var numberOfRemovedTokens = 0;

            while (layout.HadOverflow)
            {
                childNodes.RemoveAt(childNodes.Count - 1);
                layout = MakeLayout(childNodes);
                numberOfRemovedTokens++;
            }

            return new Tuple<BakedFlowLayout, int>(layout.Bake(), numberOfRemovedTokens);
        }

        RawFlowLayout MakeLayout(List<FlowLayout.LayoutNodeOrInstruction> childNodes)
        {
            return FlowLayout.HorizontalFlowParent(
                "root",
                LayoutSize.Pixels(TotalAvailableSize),
                new FlowLayoutStyle(
                    alignment: this.alignment,
                    alignmentWithinRow: new Alignment(this.alignment.Horizontal, VerticalAlignment.Bottom),
                    overflowRule: OverflowRule.HaltOnIllegal),
                childNodes.ToArray()
            );
        }

        private int OverflowAmount()
        {
            var lastItemRect = this.bakedLayout.GetLastRow().GetLastItemNode().Rectangle;
            var bottomRightOfLastItem = lastItemRect.Location + lastItemRect.Size;
            var rightOverflow = bottomRightOfLastItem.X - TotalAvailableSize.X;
            var leftOverflow = -lastItemRect.Location.X;
            return leftOverflow + rightOverflow;
        }

        public List<RenderableText> GetRenderedText(Point origin = default, Point topLeft = default, int occludedCharactersCount = 0)
        {
            var result = new List<RenderableText>();

            var renderCutoffIndex = TotalCharacterCount - occludedCharactersCount;

            var tokenIndex = 0;
            foreach (var row in this.bakedLayout.Rows)
            {
                foreach (var tokenNode in row)
                {
                    var outputFragment = this.renderedFragments[tokenIndex];
                    var pendingRenderableText = outputFragment.Drawable.CreateRenderableText(origin, topLeft, tokenNode.Rectangle.Location);

                    var lastCharacterInThisText = outputFragment.CharacterPosition + outputFragment.CharacterLength;
                    if (renderCutoffIndex <= lastCharacterInThisText)
                    {
                        var substringLength = renderCutoffIndex - lastCharacterInThisText + outputFragment.CharacterLength;

                        if (substringLength <= 0)
                        {
                            return result;
                        }

                        result.Add(outputFragment.Drawable.CreateRenderableText(origin, topLeft, tokenNode.Rectangle.Location, substringLength));
                        return result;
                    }

                    result.Add(pendingRenderableText);
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
