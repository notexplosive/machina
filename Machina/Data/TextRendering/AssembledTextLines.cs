﻿using Microsoft.Xna.Framework;
using System.Collections;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.TextRendering
{
    public readonly struct AssembledTextLines : IEnumerable<TextLine>
    {
        private readonly Point totalAvailableSpace;
        private readonly Alignment alignment;
        private readonly IFontMetrics fontMetrics;
        private readonly string[] words;
        private readonly StringBuilder stringBuilder;
        private readonly List<TextLine> textLines;
        private readonly TextLinesPendingInfo pendingInfo;
        public int Count => this.textLines.Count;

        private class TextLinesPendingInfo
        {
            public float widthOfCurrentLine;
            public int currentWordIndex;
            public int currentY;
        }

        public AssembledTextLines(string text, IFontMetrics fontMetrics, Point totalAvailableSpace, Alignment alignment, Overflow overflow)
        {
            this.stringBuilder = new StringBuilder();
            this.textLines = new List<TextLine>();
            this.totalAvailableSpace = totalAvailableSpace;
            this.alignment = alignment;
            this.fontMetrics = fontMetrics;

            this.words = CreateTokens(text);
            this.pendingInfo = new TextLinesPendingInfo();

            Assemble(overflow);
        }

        public static string[] CreateTokens(string text)
        {
            var splitLines = text.Trim().Split('\n');
            var words = new List<string>();
            foreach (var textLine in splitLines)
            {
                var pendingWord = new StringBuilder();
                foreach (var character in textLine)
                {
                    if (character == ' ')
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


                words.Add("\n"); // Re-add the newline as a sentinal value
            }

            return words.ToArray();
        }

        private void Assemble(Overflow overflow)
        {
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
            var widthAfterAppend = this.pendingInfo.widthOfCurrentLine + this.fontMetrics.MeasureString(word).X;
            return widthAfterAppend < this.totalAvailableSpace.X;
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
            this.pendingInfo.widthOfCurrentLine += this.fontMetrics.MeasureString(word).X;
            this.stringBuilder.Append(word);
        }

        private bool HasLineInBuffer()
        {
            return this.stringBuilder.Length > 0;
        }

        private void AddCurrentLineInBuffer()
        {
            this.textLines.Add(new TextLine(this.stringBuilder.ToString(), this.fontMetrics, this.totalAvailableSpace, this.alignment.Horizontal));
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

        private bool HasRoomForMoreLines()
        {
            // LineSpacing is multiplied by 2 because we need to estimate the bottom of the text, not the top
            return this.pendingInfo.currentY + this.fontMetrics.LineSpacing * 2 <= this.totalAvailableSpace.Y;
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
}
