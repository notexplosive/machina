﻿using Microsoft.Xna.Framework;
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
        private readonly SpriteFontMetrics fontMetrics;
        private readonly Rectangle totalAvailableRect;
        private readonly float spaceWidth;

        public TextMeasurer(string text, SpriteFontMetrics font, Rectangle rect, HorizontalAlignment horizontalAlignment,
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

            if (HasNextTextLine())
            {
                AddNextTextLine();
            }
        }

        public bool HasRoomForNextWordOnCurrentLine()
        {
            var word = this.words[this.currentWordIndex];
            return HasRoomForWordOnCurrentLine(word);
        }

        private bool HasRoomForWordOnCurrentLine(string word)
        {
            var widthAfterAppend = this.widthOfCurrentLine + this.fontMetrics.MeasureString(word).X + this.spaceWidth;
            return widthAfterAppend < this.totalAvailableRect.Width;
        }

        public void AppendNextWord()
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

        public bool HasNextTextLine()
        {
            return this.stringBuilder.Length > 0;
        }

        public void AddNextTextLine()
        {
            this.textLines.Add(new TextLine(this.stringBuilder.ToString(), this.fontMetrics, this.totalAvailableRect,
                this.totalAvailableRect.Y + this.currentY, this.horizontalAlignment));
            this.stringBuilder.Clear();
        }

        public void AppendLinebreak()
        {
            AddNextTextLine();
            this.currentY += this.fontMetrics.LineSpacing;
            this.widthOfCurrentLine = 0;
        }

        public bool IsAtEnd()
        {
            return this.currentWordIndex == this.words.Length;
        }

        public ICollection<TextLine> Lines => this.textLines;

        public bool HasRoomForMoreLines()
        {
            // LineSpaceing is multiplied by 2 because we need to estimate the bottom of the text, not the top
            return this.currentY + this.fontMetrics.LineSpacing * 2 <= this.totalAvailableRect.Height;
        }

        public void Elide()
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
    }
}