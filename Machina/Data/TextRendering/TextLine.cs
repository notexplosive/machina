using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct AssembledTextLines : IEnumerable<TextLine>
    {
        private readonly Rectangle totalAvailableRect;
        private readonly Alignment alignment;
        private readonly IFontMetrics fontMetrics;
        private readonly float spaceWidth;
        private readonly string[] words;
        private readonly StringBuilder stringBuilder;
        private readonly List<TextLine> textLines;
        private readonly TextLinesPendingInfo pendingInfo;

        private class TextLinesPendingInfo
        {
            public float widthOfCurrentLine;
            public int currentWordIndex;
            public int currentY;
        }

        public int Count => this.textLines.Count;

        public AssembledTextLines(string text, IFontMetrics fontMetrics, Rectangle totalAvailableRect, Alignment alignment, Overflow overflow)
        {
            this.stringBuilder = new StringBuilder();
            this.textLines = new List<TextLine>();
            this.totalAvailableRect = totalAvailableRect;
            this.alignment = alignment;
            this.fontMetrics = fontMetrics;
            this.spaceWidth = this.fontMetrics.MeasureString(" ").X;

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
            this.pendingInfo = new TextLinesPendingInfo();

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

        private bool HasRoomForNextWordOnCurrentLine()
        {
            var word = this.words[this.pendingInfo.currentWordIndex];
            return HasRoomForWordOnCurrentLine(word);
        }

        private bool HasRoomForWordOnCurrentLine(string word)
        {
            var widthAfterAppend = this.pendingInfo.widthOfCurrentLine + this.fontMetrics.MeasureString(word).X + this.spaceWidth;
            return widthAfterAppend < this.totalAvailableRect.Width;
        }

        private void AppendNextWord()
        {
            var word = this.words[this.pendingInfo.currentWordIndex];
            if (word == "\n")
            {
                AppendLinebreak();
            }
            else
            {
                AppendWord(word);
            }

            this.pendingInfo.currentWordIndex++;
        }

        private void AppendWord(string word)
        {
            this.pendingInfo.widthOfCurrentLine += this.fontMetrics.MeasureString(word).X + this.spaceWidth;
            this.stringBuilder.Append(word);
            this.stringBuilder.Append(' ');
        }

        private bool HasLineInBuffer()
        {
            return this.stringBuilder.Length > 0;
        }

        private void AddCurrentLineInBuffer()
        {
            this.textLines.Add(new TextLine(this.stringBuilder.ToString(), this.fontMetrics, this.totalAvailableRect.Size, this.pendingInfo.currentY, this.alignment.Horizontal));
            this.stringBuilder.Clear();
        }

        private void AppendLinebreak()
        {
            AddCurrentLineInBuffer();
            this.pendingInfo.currentY += this.fontMetrics.LineSpacing;
            this.pendingInfo.widthOfCurrentLine = 0;
        }

        private bool IsAtEnd()
        {
            return this.pendingInfo.currentWordIndex == this.words.Length;
        }

        public IList<TextLine> Lines => this.textLines;

        private bool HasRoomForMoreLines()
        {
            // LineSpaceing is multiplied by 2 because we need to estimate the bottom of the text, not the top
            return this.pendingInfo.currentY + this.fontMetrics.LineSpacing * 2 <= this.totalAvailableRect.Height;
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
                    this.pendingInfo.widthOfCurrentLine -= widthOfLastCharacter;
                    Elide();
                }
            }
        }

        public IEnumerator<TextLine> GetEnumerator()
        {
            return this.textLines.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return this.textLines.GetEnumerator();
        }

        public TextLine this[int i]
        {
            get { return this.textLines[i]; }
        }
    }

    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly Point positionRelativeToTopLeftOfText;

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.positionRelativeToTopLeftOfText.Y = positionY;

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                this.positionRelativeToTopLeftOfText.X = 0;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = availableSpace.X - (int)fontMetrics.MeasureString(content).X + 1;
                this.positionRelativeToTopLeftOfText.X = widthOffset;
            }
            else
            {
                var widthOffset = availableSpace.X - (int)fontMetrics.MeasureString(content).X / 2 + 1 - availableSpace.X / 2;
                this.positionRelativeToTopLeftOfText.X = widthOffset;
            }

        }
    }
}
