using Machina.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public struct TextMeasurer
    {
        private float widthOfCurrentLine;
        private int currentWordIndex;
        private int currentY;
        private readonly List<TextLine> textLines;
        private readonly HorizontalAlignment horizontalAlignment;
        private readonly VerticalAlignment verticalAlignment;
        private readonly string[] words;
        private readonly StringBuilder stringBuilder;
        private readonly IFontMetrics fontMetrics;
        private readonly Rectangle totalAvailableRect;
        private readonly float spaceWidth;

        public TextMeasurer(string text, IFontMetrics font, Rectangle rect, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment, Overflow overflow)
        {
            this.widthOfCurrentLine = 0f;

            var splitLines = text.Trim().Split('\n');
            var words = new List<string>();
            foreach (var textLine in splitLines)
            {
                var splitWords = textLine.Split(' ');
                foreach (var word in splitWords)
                {
                    words.Add(word);
                }

                words.Add("\n"); // Re-add the newline as a sentinal value
            }

            this.words = words.ToArray();
            this.stringBuilder = new StringBuilder();
            this.currentWordIndex = 0;
            this.fontMetrics = font;
            this.totalAvailableRect = rect;
            this.spaceWidth = this.fontMetrics.MeasureString(" ").X;
            this.currentY = 0;
            this.textLines = new List<TextLine>();
            this.horizontalAlignment = horizontalAlignment;
            this.verticalAlignment = verticalAlignment;

            while (!IsAtEnd())
            {
                if (HasRoomForNextWordOnCurrentLine())
                {
                    AppendNextWord();
                }
                else
                {
                    if (HasRoomForMoreLines())
                    {
                        AppendLinebreak();
                    }
                    else
                    {
                        if (overflow == Overflow.Elide)
                        {
                            Elide();
                        }
                        else
                        {
                            AppendNextWord();
                        }

                        break;
                    }
                }
            }

            if (HasLineInBuffer())
            {
                AddCurrentLineInBuffer();
            }
        }

        public List<RenderableText> GetRenderedLines(Vector2 worldPos, Point drawOffset, Color textColor, float angle, Depth depth, int xAdjustment)
        {
            var renderableTexts = new List<RenderableText>();

            foreach (var line in Lines)
            {
                renderableTexts.Add(new RenderableText(this.fontMetrics, line, worldPos, textColor, drawOffset, angle, depth, UsedRectPosition().Y, xAdjustment));
            }

            return renderableTexts;
        }

        private bool HasRoomForNextWordOnCurrentLine()
        {
            var word = this.words[this.currentWordIndex];
            return HasRoomForWordOnCurrentLine(word);
        }

        private bool HasRoomForWordOnCurrentLine(string word)
        {
            var widthAfterAppend = this.widthOfCurrentLine + this.fontMetrics.MeasureString(word).X + this.spaceWidth;
            return widthAfterAppend < this.totalAvailableRect.Width;
        }

        private void AppendNextWord()
        {
            var word = this.words[this.currentWordIndex];
            if (word == "\n")
            {
                AppendLinebreak();
            }
            else
            {
                AppendWord(word);
            }

            this.currentWordIndex++;
        }

        private void AppendWord(string word)
        {
            this.widthOfCurrentLine += this.fontMetrics.MeasureString(word).X + this.spaceWidth;
            this.stringBuilder.Append(word);
            this.stringBuilder.Append(' ');
        }

        private bool HasLineInBuffer()
        {
            return this.stringBuilder.Length > 0;
        }

        private void AddCurrentLineInBuffer()
        {
            this.textLines.Add(new TextLine(this.stringBuilder.ToString(), this.fontMetrics, this.totalAvailableRect.Size,
                this.totalAvailableRect.Y + this.currentY, this.horizontalAlignment));
            this.stringBuilder.Clear();
        }

        private void AppendLinebreak()
        {
            AddCurrentLineInBuffer();
            this.currentY += this.fontMetrics.LineSpacing;
            this.widthOfCurrentLine = 0;
        }

        private bool IsAtEnd()
        {
            return this.currentWordIndex == this.words.Length;
        }

        public IList<TextLine> Lines => this.textLines;

        private bool HasRoomForMoreLines()
        {
            // LineSpaceing is multiplied by 2 because we need to estimate the bottom of the text, not the top
            return this.currentY + this.fontMetrics.LineSpacing * 2 <= this.totalAvailableRect.Height;
        }

        private void Elide()
        {
            var ellipses = "...";
            if (HasRoomForWordOnCurrentLine(ellipses))
            {
                AppendWord(ellipses);
            }
            else
            {
                if (this.stringBuilder.Length > 0)
                {
                    var widthOfLastCharacter = this.fontMetrics.MeasureString(this.stringBuilder[^1].ToString()).X;
                    this.stringBuilder.Remove(this.stringBuilder.Length - 1, 1);
                    this.widthOfCurrentLine -= widthOfLastCharacter;
                    Elide();
                }
            }
        }

        public Point UsedRectPosition()
        {
            var boundsHeight = this.totalAvailableRect.Height;
            var yOffset = 0;
            if (this.verticalAlignment == VerticalAlignment.Center)
            {
                yOffset = boundsHeight / 2 - this.fontMetrics.LineSpacing / 2 * Lines.Count;
            }
            else if (this.verticalAlignment == VerticalAlignment.Bottom)
            {
                yOffset = boundsHeight - this.fontMetrics.LineSpacing * Lines.Count;
            }

            var xOffset = 0;
            foreach (var line in Lines)
            {
                xOffset = line.nonAdjustedX;
                break;
            }

            return new Point(xOffset, yOffset);
        }
    }
}
