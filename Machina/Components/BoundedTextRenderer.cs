using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
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
            FontMetrics = new SpriteFontMetrics(font);
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
        public SpriteFontMetrics FontMetrics { get; set; }

        public Point DrawOffset { get; set; }

        public Point TextLocalPos => CreateMeasuredText().GetTextLocalPos(FontMetrics, this.boundingRect.Height, (int)transform.Position.X);

        public Point TextWorldPos => transform.Position.ToPoint() + TextLocalPos;

        private TextMeasurer CreateMeasuredText()
        {
            var measurer = new TextMeasurer(Text, FontMetrics, this.boundingRect.Rect, this.horizontalAlignment, this.verticalAlignment, this.overflow);

            return measurer;
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
            var localPos = measurer.GetTextLocalPos(FontMetrics, this.boundingRect.Height, (int)transform.Position.X);
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
            var localPos = measurer.GetTextLocalPos(FontMetrics, this.boundingRect.Height, (int)transform.Position.X);
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
}