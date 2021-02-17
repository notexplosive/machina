using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class BoundedTextRenderer : BaseComponent
    {
        private readonly string text;
        private readonly SpriteFont font;
        private readonly BoundingRect boundingRect;

        public BoundedTextRenderer(Actor actor, string text, SpriteFont font) : base(actor)
        {
            this.text = text;
            this.font = font;
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var measurer = new TextMeasurer(this.text, this.font, this.boundingRect.Rect);

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

            spriteBatch.DrawString(this.font, measurer.Build(), this.boundingRect.TopLeft, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, Transform.Depth);
        }
    }

    struct TextMeasurer
    {
        float widthOfCurrentLine;
        private int currentWordIndex;
        private int currentHeight;
        private readonly string[] splitText;
        private readonly StringBuilder builder;
        private readonly SpriteFont font;
        private readonly Rectangle rect;
        private readonly float spaceWidth;

        public TextMeasurer(string text, SpriteFont font, Rectangle rect)
        {
            this.widthOfCurrentLine = 0f;
            this.splitText = text.Split(' ');
            this.builder = new StringBuilder();
            this.currentWordIndex = 0;
            this.font = font;
            this.rect = rect;
            this.spaceWidth = this.font.MeasureString(" ").X;
            this.currentHeight = 0;
        }

        public bool CanAppendNextWord()
        {
            var word = this.splitText[currentWordIndex];
            return CanAppendWord(word);
        }

        private bool CanAppendWord(string word)
        {
            var widthAfterAppend = this.widthOfCurrentLine + this.font.MeasureString(word).X + spaceWidth;
            return widthAfterAppend < rect.Width;
        }

        public void AppendNextWord()
        {
            var word = this.splitText[currentWordIndex];
            AppendWord(word);
            currentWordIndex++;
        }

        private void AppendWord(string word)
        {
            this.widthOfCurrentLine += this.font.MeasureString(word).X + spaceWidth;
            this.builder.Append(word);
            this.builder.Append(' ');
        }

        public void AppendLinebreak()
        {
            this.currentHeight += this.font.LineSpacing;
            this.builder.Append('\n');
            this.widthOfCurrentLine = 0;
        }

        public bool IsAtEnd()
        {
            return this.currentWordIndex == this.splitText.Length;
        }

        public string Build()
        {
            return this.builder.ToString();
        }

        public bool CanAppendLinebreak()
        {
            return currentHeight + this.font.LineSpacing < this.rect.Height;
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
                if (this.builder.Length > 0)
                {
                    var widthOfLastCharacter = this.font.MeasureString(this.builder[this.builder.Length - 1].ToString()).X;
                    this.builder.Remove(this.builder.Length - 1, 1);
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
