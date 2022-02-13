using System;
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
        public Point TextLocalPos => CreateMeasuredText().TopLeftOfText();
        public Point TextWorldPos => this.boundingRect.TopLeft.ToPoint() + TextLocalPos;
        public int OccludedIndex { get; set; }

        private BoundedText CreateMeasuredText()
        {
            var measurer = new BoundedText(this.boundingRect.Size, new Alignment(this.horizontalAlignment, this.verticalAlignment), this.overflow, new FormattedText(new FormattedTextFragment(Text, FontMetrics, TextColor)));

            return measurer;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            List<RenderableText> renderableTexts = CreateMeasuredText().GetRenderedText(this.boundingRect.TopLeft.ToPoint(), OccludedIndex);

            foreach (var renderableText in renderableTexts)
            {
                renderableText.Draw(spriteBatch, DrawOffset, transform.Angle, transform.Depth + this.depthOffset);
                if (this.isDropShadowEnabled)
                {
                    renderableText.DrawDropShadow(spriteBatch, this.dropShadowColor, DrawOffset, transform.Angle, transform.Depth + this.depthOffset);
                }
            }
        }

        public BoundedTextRenderer EnableDropShadow(Color color)
        {
            this.dropShadowColor = color;
            this.isDropShadowEnabled = true;
            return this;
        }

        public void OccludeAll()
        {
            OccludedIndex = Text.Length;
        }
    }
}