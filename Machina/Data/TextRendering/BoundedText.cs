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
        private readonly List<FragmentAndRow> fragmentRows;
        private readonly List<TextOutputFragment> renderedFragments;
        private readonly FormattedText formattedText;

        public int TotalCharacterCount => this.formattedText.TotalCharacterCount;
        public Point TotalAvailableSize { get; }
        public Point UsedSize { get; }

        public BoundedText(Point size, Alignment alignment, Overflow overflow = Overflow.Ignore, FormattedText formattedText = default)
        {
            TotalAvailableSize = size;
            this.alignment = alignment;
            this.renderedFragments = new List<TextOutputFragment>();
            this.formattedText = formattedText;
            this.fragmentRows = null;
            UsedSize = Point.Zero;

            var allOutputFragments = new List<TextOutputFragment>();
            foreach (var outputFragment in formattedText.OutputFragments())
            {
                allOutputFragments.Add(outputFragment);
            }

            this.fragmentRows = BakeFromFragments(allOutputFragments, overflow);

            if (overflow == Overflow.Elide)
            {

                // first, prune anything that overflows, this will catch any middle lines that happen to be very long
                var prunedTuples = new List<FragmentAndRow>();
                foreach (var tuple in this.fragmentRows)
                {
                    var layoutRow = tuple.BakedRow;
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
                while (finalRow.Fragment.Count > 0)
                {
                    var lastIndex = finalRow.Fragment.Count - 1;
                    var lastFragment = finalRow.Fragment[lastIndex];
                    int ellipseSize = lastFragment.Drawable.EllipseWidth();
                    var overflowAmount = OverflowAmount(finalRow.Fragment);

                    if (overflowAmount > 0 || needsEllipse)
                    {
                        needsEllipse = true;
                        var shrinkAmount = overflowAmount + ellipseSize;
                        if (shrinkAmount >= lastFragment.Drawable.Size.X)
                        {
                            finalRow.Fragment.RemoveAt(lastIndex);
                        }
                        else
                        {
                            var lastDrawable = lastFragment.Drawable;
                            lastDrawable = lastDrawable.ShrinkBy(shrinkAmount);
                            lastDrawable = lastDrawable.AppendEllipse();
                            finalRow.Fragment[lastIndex] = new TextOutputFragment(lastDrawable, lastFragment.CharacterPosition);
                            break;
                        }
                    }
                    else
                    {
                        break;
                    }
                }

                // replenish allOutputFragments with fragmentRows
                allOutputFragments.Clear();
                foreach (var tuple in this.fragmentRows)
                {
                    foreach (var fragment in tuple.Fragment)
                    {
                        allOutputFragments.Add(fragment);
                    }
                }

                // bake again
                this.fragmentRows = BakeFromFragments(allOutputFragments, overflow);
            }

            // collect final fragments
            foreach (var outputFragment in allOutputFragments)
            {
                this.renderedFragments.Add(outputFragment);
            }

            if (this.fragmentRows.Count > 0)
            {
                UsedSize = this.fragmentRows[0].WholeLayout.UsedSpace;
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

        private List<FragmentAndRow> BakeFromFragments(List<TextOutputFragment> allOutputFragments, Overflow overflow)
        {
            var childNodes = new List<FlowLayout.LayoutNodeOrInstruction>();

            foreach (var token in allOutputFragments)
            {
                childNodes.AddRange(token.Nodes);
            }

            var layout = FlowLayout.HorizontalFlowParent(
                "root",
                LayoutSize.Pixels(TotalAvailableSize),
                new FlowLayoutStyle(
                    alignment: this.alignment,
                    alignmentWithinRow: new Alignment(this.alignment.Horizontal, VerticalAlignment.Bottom),
                    overflowRule: overflow == Overflow.Elide ? OverflowRule.LastRowKeepsGoing : OverflowRule.Free),
                childNodes.ToArray()
            );

            var bakedLayout = layout.Bake();

            var fragmentIndex = 0;
            var fragmentRows = new List<FragmentAndRow>();
            foreach (var row in bakedLayout.Rows)
            {
                var fragmentList = new List<TextOutputFragment>();
                foreach (var item in row)
                {
                    fragmentList.Add(allOutputFragments[fragmentIndex]);
                    fragmentIndex++;
                }
                fragmentRows.Add(new FragmentAndRow(bakedLayout, row, fragmentList));
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
                foreach (var tokenNode in row.BakedRow)
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
            return fragmentRows[lineIndex].BakedRow.UsedRectangle;
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
                var lineRelativePositionX = row.BakedRow.UsedRectangle.Location.X;
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
