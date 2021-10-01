using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

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

    public enum Overflow
    {
        Elide,
        Ignore
    }

    public class BoundedTextRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Depth depthOffset;
        private readonly HorizontalAlignment horizontalAlignment;
        private readonly Overflow overflow;
        private readonly VerticalAlignment verticalAlignment;
        private Color dropShadowColor;
        private bool isDropShadowEnabled;
        public Color TextColor;

        public BoundedTextRenderer(Actor actor, string text, SpriteFont font,
            Color textColor,
            HorizontalAlignment horizontalAlignment = HorizontalAlignment.Left,
            VerticalAlignment verticalAlignment = VerticalAlignment.Top,
            Overflow overflow = Overflow.Elide,
            Depth depthOffset = default) : base(actor)
        {
            Debug.Assert(text != null);

            Text = text;
            Font = font;
            this.boundingRect = RequireComponent<BoundingRect>();
            this.TextColor = textColor;
            this.horizontalAlignment = horizontalAlignment;
            this.verticalAlignment = verticalAlignment;
            this.depthOffset = depthOffset;
            this.overflow = overflow;
        }

        public BoundedTextRenderer(Actor actor, string text, SpriteFont font) : this(actor, text, font, Color.White)
        {
        }

        public string Text { get; set; }

        public SpriteFont Font { get; set; }

        public Point DrawOffset { get; set; }

        public Point TextLocalPos => GetTextLocalPos(CreateMeasuredText());

        public Point TextWorldPos => transform.Position.ToPoint() + TextLocalPos;

        private TextMeasurer CreateMeasuredText()
        {
            var measurer = new TextMeasurer(Text, Font, this.boundingRect.Rect, this.horizontalAlignment,
                this.verticalAlignment);

            while (!measurer.IsAtEnd())
            {
                if (measurer.HasRoomForNextWordOnCurrentLine())
                {
                    measurer.AppendNextWord();
                }
                else
                {
                    if (measurer.HasRoomForMoreLines())
                    {
                        measurer.AppendLinebreak();
                    }
                    else
                    {
                        if (this.overflow == Overflow.Elide)
                        {
                            measurer.Elide();
                        }
                        else
                        {
                            measurer.AppendNextWord();
                        }

                        break;
                    }
                }
            }

            if (measurer.HasNextTextLine())
            {
                measurer.AddNextTextLine();
            }

            return measurer;
        }

        public Point GetTextLocalPos(TextMeasurer measurer)
        {
            var yOffset = 0;
            if (this.verticalAlignment == VerticalAlignment.Center)
            {
                yOffset = this.boundingRect.Height / 2 - Font.LineSpacing / 2 * measurer.Lines.Count;
            }
            else if (this.verticalAlignment == VerticalAlignment.Bottom)
            {
                yOffset = this.boundingRect.Height - Font.LineSpacing * measurer.Lines.Count;
            }

            var xOffset = 0;
            foreach (var line in measurer.Lines)
            {
                xOffset = line.textPosition.X - (int) transform.Position.X;
                break;
            }

            return new Point(xOffset, yOffset);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var measurer = CreateMeasuredText();

            var localPos = GetTextLocalPos(measurer);

            foreach (var line in measurer.Lines)
            {
                var pivotPos = transform.Position;
                var pos = new Vector2(line.textPosition.X, line.textPosition.Y + localPos.Y) + DrawOffset.ToVector2() -
                          pivotPos;
                pos.Floor();
                var depth = transform.Depth + this.depthOffset;
                var finalDropShadowColor = new Color(this.dropShadowColor,
                    this.dropShadowColor.A / 255f * (this.TextColor.A / 255f));

                spriteBatch.DrawString(Font, line.textContent, pivotPos, this.TextColor, transform.Angle,
                    -pos, 1f, SpriteEffects.None, depth);
                if (this.isDropShadowEnabled)
                {
                    spriteBatch.DrawString(Font, line.textContent, pivotPos, finalDropShadowColor, transform.Angle,
                        -(pos + new Vector2(1, 1)), 1f, SpriteEffects.None, depth + 1);
                }
            }
        }

        public override void DebugDraw(SpriteBatch spriteBatch)
        {
            var measurer = CreateMeasuredText();
            var localPos = GetTextLocalPos(measurer);
            spriteBatch.DrawCircle(new CircleF(localPos, 5f), 10, Color.Teal, 5f);
            foreach (var line in measurer.Lines)
            {
                var pos = new Vector2(line.textPosition.X, line.textPosition.Y + localPos.Y);
                spriteBatch.DrawCircle(new CircleF(pos, 5), 10, Color.Red, 5f);
                spriteBatch.DrawLine(pos, pos + DrawOffset.ToVector2(), Color.Orange);
                spriteBatch.DrawCircle(new CircleF(pos + DrawOffset.ToVector2(), 5), 10, Color.Orange, 5f);
            }
        }

        public BoundedTextRenderer EnableDropShadow(Color color)
        {
            this.dropShadowColor = color;
            this.isDropShadowEnabled = true;
            return this;
        }
    }

    public struct TextLine
    {
        public string textContent;
        public readonly Point textPosition;

        public TextLine(string content, SpriteFont font, Rectangle bounds, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.textPosition = new Point(0, positionY);

            if (horizontalAlignment == HorizontalAlignment.Left)
            {
                this.textPosition.X = bounds.Location.X;
            }
            else if (horizontalAlignment == HorizontalAlignment.Right)
            {
                var widthOffset = bounds.Width - (int) font.MeasureString(content).X + 1;
                this.textPosition.X = bounds.Location.X + widthOffset;
            }
            else
            {
                var widthOffset = bounds.Width - (int) font.MeasureString(content).X / 2 + 1 - bounds.Width / 2;
                this.textPosition.X = bounds.Location.X + widthOffset;
            }
        }
    }

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
        private readonly SpriteFont font;
        private readonly Rectangle totalAvailableRect;
        private readonly float spaceWidth;

        public TextMeasurer(string text, SpriteFont font, Rectangle rect, HorizontalAlignment horizontalAlignment,
            VerticalAlignment verticalAlignment)
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
            this.font = font;
            this.totalAvailableRect = rect;
            this.spaceWidth = this.font.MeasureString(" ").X;
            this.currentY = 0;
            this.textLines = new List<TextLine>();
            this.horizontalAlignment = horizontalAlignment;
            this.verticalAlignment = verticalAlignment;
        }

        public bool HasRoomForNextWordOnCurrentLine()
        {
            var word = this.words[this.currentWordIndex];
            return HasRoomForWordOnCurrentLine(word);
        }

        private bool HasRoomForWordOnCurrentLine(string word)
        {
            var widthAfterAppend = this.widthOfCurrentLine + this.font.MeasureString(word).X + this.spaceWidth;
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
            this.widthOfCurrentLine += this.font.MeasureString(word).X + this.spaceWidth;
            this.stringBuilder.Append(word);
            this.stringBuilder.Append(' ');
        }

        public bool HasNextTextLine()
        {
            return this.stringBuilder.Length > 0;
        }

        public void AddNextTextLine()
        {
            this.textLines.Add(new TextLine(this.stringBuilder.ToString(), this.font, this.totalAvailableRect,
                this.totalAvailableRect.Y + this.currentY, this.horizontalAlignment));
            this.stringBuilder.Clear();
        }

        public void AppendLinebreak()
        {
            AddNextTextLine();
            this.currentY += this.font.LineSpacing;
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
            return this.currentY + this.font.LineSpacing * 2 <= this.totalAvailableRect.Height;
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
                    var widthOfLastCharacter = this.font.MeasureString(this.stringBuilder[^1].ToString()).X;
                    this.stringBuilder.Remove(this.stringBuilder.Length - 1, 1);
                    this.widthOfCurrentLine -= widthOfLastCharacter;
                    Elide();
                }
            }
        }
    }
}
