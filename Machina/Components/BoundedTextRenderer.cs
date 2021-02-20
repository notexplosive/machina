using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    class BoundedTextRenderer : BaseComponent
    {
        public string Text
        {
            get; set;
        }
        public readonly SpriteFont font;
        private readonly BoundingRect boundingRect;
        private readonly Color textColor;
        private readonly HorizontalAlignment horizontalAlignment;
        private readonly VerticalAlignment verticalAlignment;

        public BoundedTextRenderer(Actor actor, string text, SpriteFont font, Color textColor, HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left, VerticalAlignment verticalAlignment = VerticalAlignment.Top) : base(actor)
        {
            this.Text = text;
            this.font = font;
            this.boundingRect = RequireComponent<BoundingRect>();
            this.textColor = textColor;
            this.horizontalAlignment = horizontalAlignment;
            this.verticalAlignment = verticalAlignment;
        }

        public BoundedTextRenderer(Actor actor, string text, SpriteFont font) : this(actor, text, font, Color.White) { }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var measurer = new TextMeasurer(this.Text, this.font, this.boundingRect.Rect, this.horizontalAlignment, this.verticalAlignment);

            while (!measurer.IsAtEnd())
            {
                if (measurer.CanAppendNextWord())
                {
                    measurer.AppendNextWord();
                }
                else
                {
                    if (measurer.CanAppendLinebreak())
                    {
                        measurer.AppendLinebreak();
                    }
                    else
                    {
                        measurer.Elide();
                        break;
                    }
                }
            }

            measurer.AppendLinebreak();

            var yOffset = 0;
            if (verticalAlignment == VerticalAlignment.Center)
            {
                yOffset = this.boundingRect.Height / 2 - font.LineSpacing / 2 * measurer.Lines.Count;
            }
            else if (verticalAlignment == VerticalAlignment.Bottom)
            {
                yOffset = this.boundingRect.Height - font.LineSpacing * measurer.Lines.Count;
            }

            foreach (var line in measurer.Lines)
            {
                spriteBatch.DrawString(this.font, line.textContent, new Vector2(line.positionX, line.positionY + yOffset), this.textColor, 0, Vector2.Zero, 1f, SpriteEffects.None, transform.Depth.AsFloat);
            }
        }
    }

    struct TextLine
    {
        public string textContent;
        public readonly int positionY;
        public readonly int positionX;

        public TextLine(string content, SpriteFont font, Rectangle bounds, int positionY, HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.positionY = positionY;

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                positionX = bounds.Location.X;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = bounds.Width - (int) font.MeasureString(content).X + 1;
                this.positionX = bounds.Location.X + widthOffset;
            }
            else
            {
                var widthOffset = bounds.Width - (int) font.MeasureString(content).X / 2 + 1 - bounds.Width / 2;
                this.positionX = bounds.Location.X + widthOffset;
            }
        }
    }

    struct TextMeasurer
    {
        private float widthOfCurrentLine;
        private int currentWordIndex;
        private int currentY;
        private readonly List<TextLine> textLines;
        private readonly HorizontalAlignment horizontalAlignment;
        private readonly VerticalAlignment verticalAlignment;
        private readonly string[] words;
        private readonly StringBuilder stringBuilder;
        private readonly SpriteFont font;
        private readonly Rectangle totalAvailableRect;
        private readonly float spaceWidth;

        public TextMeasurer(string text, SpriteFont font, Rectangle rect, HorizontalAlignment horizontalAlignment, VerticalAlignment verticalAlignment)
        {
            this.widthOfCurrentLine = 0f;
            this.words = text.Split(' ');
            this.stringBuilder = new StringBuilder();
            this.currentWordIndex = 0;
            this.font = font;
            this.totalAvailableRect = rect;
            this.spaceWidth = this.font.MeasureString(" ").X;
            this.currentY = 0;
            this.textLines = new List<TextLine>();
            this.horizontalAlignment = horizontalAlignment;
            this.verticalAlignment = verticalAlignment;
        }

        public bool CanAppendNextWord()
        {
            var word = this.words[currentWordIndex];
            return CanAppendWord(word);
        }

        private bool CanAppendWord(string word)
        {
            var widthAfterAppend = this.widthOfCurrentLine + this.font.MeasureString(word).X + spaceWidth;
            return widthAfterAppend < totalAvailableRect.Width;
        }

        public void AppendNextWord()
        {
            var word = this.words[currentWordIndex];
            AppendWord(word);
            currentWordIndex++;
        }

        private void AppendWord(string word)
        {
            this.widthOfCurrentLine += this.font.MeasureString(word).X + spaceWidth;
            this.stringBuilder.Append(word);
            this.stringBuilder.Append(' ');
        }

        public void AppendLinebreak()
        {
            this.textLines.Add(new TextLine(this.stringBuilder.ToString(), font, totalAvailableRect, this.totalAvailableRect.Y + currentY, horizontalAlignment));
            this.currentY += this.font.LineSpacing;
            this.stringBuilder.Clear();
            this.widthOfCurrentLine = 0;
        }

        public bool IsAtEnd()
        {
            return this.currentWordIndex == this.words.Length;
        }

        public ICollection<TextLine> Lines => this.textLines;

        public bool CanAppendLinebreak()
        {
            // LineSpaceing is multiplied by 2 because we need to estimate the bottom of the text, not the top
            return currentY + this.font.LineSpacing * 2 < this.totalAvailableRect.Height;
        }

        public void Elide()
        {
            var ellipses = "...";
            if (CanAppendWord(ellipses))
            {
                AppendWord(ellipses);
            }
            else
            {
                if (this.stringBuilder.Length > 0)
                {
                    var widthOfLastCharacter = this.font.MeasureString(this.stringBuilder[this.stringBuilder.Length - 1].ToString()).X;
                    this.stringBuilder.Remove(this.stringBuilder.Length - 1, 1);
                    this.widthOfCurrentLine -= widthOfLastCharacter;
                    Elide();
                }
                else
                {
                    // If we're here that means we have literally no room to render anything
                }
            }
        }
    }
}
