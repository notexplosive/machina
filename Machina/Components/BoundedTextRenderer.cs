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
    public class FontMetrics
    {
        private SpriteFont font;

        public FontMetrics(SpriteFont font)
        {
            this.font = font;
        }

        public int LineSpacing => this.font.LineSpacing;
        public Vector2 MeasureString(string text)
        {
            return this.font.MeasureString(text);
        }
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
            FontMetrics = new FontMetrics(font);
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
        public SpriteFont Font { get; }
        public FontMetrics FontMetrics { get; set; }

        public Point DrawOffset { get; set; }

        public Point TextLocalPos => GetTextLocalPos(CreateMeasuredText());

        public Point TextWorldPos => transform.Position.ToPoint() + TextLocalPos;

        private TextMeasurer CreateMeasuredText()
        {
            var measurer = new TextMeasurer(Text, FontMetrics, this.boundingRect.Rect, this.horizontalAlignment, this.verticalAlignment, this.overflow);

            return measurer;
        }

        public Point GetTextLocalPos(TextMeasurer measurer)
        {
            var yOffset = 0;
            if (this.verticalAlignment == VerticalAlignment.Center)
            {
                yOffset = this.boundingRect.Height / 2 - FontMetrics.LineSpacing / 2 * measurer.Lines.Count;
            }
            else if (this.verticalAlignment == VerticalAlignment.Bottom)
            {
                yOffset = this.boundingRect.Height - FontMetrics.LineSpacing * measurer.Lines.Count;
            }

            var xOffset = 0;
            foreach (var line in measurer.Lines)
            {
                xOffset = line.textPosition.X - (int)transform.Position.X;
                break;
            }

            return new Point(xOffset, yOffset);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            List<RenderableText> renderableTexts = GetRenderedLines();

            foreach (var renderableText in renderableTexts)
            {
                renderableText.Draw(spriteBatch);
                if (this.isDropShadowEnabled)
                {
                    renderableText.DrawDropShadow(spriteBatch, this.dropShadowColor);
                }
            }
        }

        private List<RenderableText> GetRenderedLines()
        {
            var renderableTexts = new List<RenderableText>();

            var measurer = CreateMeasuredText();
            var localPos = GetTextLocalPos(measurer);
            foreach (var line in measurer.Lines)
            {
                var pivotPos = transform.Position;
                var offset = new Vector2(line.textPosition.X, line.textPosition.Y + localPos.Y) + DrawOffset.ToVector2() -
                          pivotPos;
                offset.Floor();

                renderableTexts.Add(new RenderableText(Font, line.textContent, pivotPos, TextColor, -offset, transform.Angle, transform.Depth + this.depthOffset));
            }

            return renderableTexts;
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

    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly Point textPosition;

        public TextLine(string content, FontMetrics fontMetrics, Rectangle bounds, int positionY,
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
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X + 1;
                this.textPosition.X = bounds.Location.X + widthOffset;
            }
            else
            {
                var widthOffset = bounds.Width - (int)fontMetrics.MeasureString(content).X / 2 + 1 - bounds.Width / 2;
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
        private readonly FontMetrics fontMetrics;
        private readonly Rectangle totalAvailableRect;
        private readonly float spaceWidth;

        public TextMeasurer(string text, FontMetrics font, Rectangle rect, HorizontalAlignment horizontalAlignment,
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