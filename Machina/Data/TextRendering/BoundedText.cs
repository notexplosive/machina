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
        private readonly List<Tuple<BakedFlowLayout.BakedRow, List<TextOutputFragment>>> fragmentRows;
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
            this.fragmentRows = null;

            foreach (var outputFragment in formattedText.OutputFragments())
            {
                this.allOutputFragments.Add(outputFragment);
            }

            this.fragmentRows = BakeFromFragments();

            // first, prune anything that overflows, this will catch any middle lines that happen to be very long
            var prunedTuples = new List<Tuple<BakedFlowLayout.BakedRow, List<TextOutputFragment>>>();
            foreach (var tuple in this.fragmentRows)
            {
                var layoutRow = tuple.Item1;
                prunedTuples.Add(tuple);
                if (layoutRow.UsedRectangle.Width > TotalAvailableSize.X)
                {
                    break;
                }
            }

            // adopt the pruned list
            this.fragmentRows = prunedTuples;

            // shrink the oversized row
            var finalRow = this.fragmentRows[this.fragmentRows.Count - 1];
            bool needsEllipse = false;
            while (finalRow.Item2.Count > 0)
            {
                var lastIndex = finalRow.Item2.Count - 1;
                var lastFragment = finalRow.Item2[lastIndex];
                int ellipseSize = lastFragment.Drawable.EllipseWidth();
                var overflowAmount = OverflowAmount(finalRow.Item2);

                if (overflowAmount > 0 || needsEllipse)
                {
                    needsEllipse = true;
                    var shrinkAmount = overflowAmount + ellipseSize;
                    if (shrinkAmount >= lastFragment.Drawable.Size.X)
                    {
                        finalRow.Item2.RemoveAt(lastIndex);
                    }
                    else
                    {
                        var lastDrawable = lastFragment.Drawable;
                        lastDrawable = lastDrawable.ShrinkBy(shrinkAmount);
                        lastDrawable = lastDrawable.AppendEllipse();
                        finalRow.Item2[lastIndex] = new TextOutputFragment(lastDrawable, lastFragment.CharacterPosition);
                        break;
                    }
                }
                else
                {
                    break;
                }
            }

            // replenish allOutputFragments with fragmentRows
            this.allOutputFragments.Clear();
            foreach (var tuple in this.fragmentRows)
            {
                foreach (var fragment in tuple.Item2)
                {
                    this.allOutputFragments.Add(fragment);
                }
            }

            // bake again
            this.fragmentRows = BakeFromFragments();

            foreach (var outputFragment in this.allOutputFragments)
            {
                // if (outputFragment.WillBeRendered)
                {
                    this.renderedFragments.Add(outputFragment);
                }
            }
        }

        private int OverflowAmount(List<TextOutputFragment> row)
        {
            var usedSize = 0;
            foreach (var item in row)
            {
                usedSize += item.Drawable.Size.X;
            }
            return usedSize - TotalAvailableSize.X;
        }

        private List<Tuple<BakedFlowLayout.BakedRow, List<TextOutputFragment>>> BakeFromFragments()
        {
            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();

            foreach (var token in this.allOutputFragments)
            {
                childNodes.AddRange(token.Nodes);
            }

            var layout = FlowLayout.HorizontalFlowParent(
                "root",
                LayoutSize.Pixels(TotalAvailableSize),
                new FlowLayoutStyle(
                    alignment: this.alignment,
                    alignmentWithinRow: new Alignment(this.alignment.Horizontal, VerticalAlignment.Bottom),
                    overflowRule: OverflowRule.LastRowKeepsGoing),
                childNodes.ToArray()
            );

            var bakedLayout = layout.Bake();

            var fragmentIndex = 0;
            var fragmentRows = new List<Tuple<BakedFlowLayout.BakedRow, List<TextOutputFragment>>>();
            foreach (var row in bakedLayout.Rows)
            {
                var fragmentList = new List<TextOutputFragment>();
                foreach (var item in row)
                {
                    fragmentList.Add(this.allOutputFragments[fragmentIndex]);
                    fragmentIndex++;
                }
                fragmentRows.Add(new Tuple<BakedFlowLayout.BakedRow, List<TextOutputFragment>>(row, fragmentList));
            }

            return fragmentRows;
        }

        public List<RenderableText> GetRenderedText(Point origin = default, Point topLeft = default, int occludedCharactersCount = 0)
        {
            var result = new List<RenderableText>();

            var renderCutoffIndex = TotalCharacterCount - occludedCharactersCount;

            var tokenIndex = 0;
            foreach (var row in this.fragmentRows)
            {
                foreach (var tokenNode in row.Item1)
                {
                    var outputFragment = this.renderedFragments[tokenIndex];
                    tokenIndex++;

                    if (!outputFragment.WillBeRendered)
                    {
                        continue;
                    }

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
                }
            }

            return result;
        }

        public Rectangle GetRectOfLine(int lineIndex)
        {
            return fragmentRows[lineIndex].Item1.UsedRectangle;
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
            foreach (var row in this.fragmentRows)
            {
                var lineRelativePositionX = row.Item1.UsedRectangle.Location.X;
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
