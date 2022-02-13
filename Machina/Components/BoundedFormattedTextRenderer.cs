using System;
using System.Collections.Generic;
using System.Diagnostics;
using Machina.Data;
using Machina.Data.TextRendering;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    public class BoundedFormattedTextRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Alignment alignment;
        private readonly Depth depthOffset;
        private readonly Overflow overflow;
        private Color dropShadowColor;
        private bool isDropShadowEnabled;
        public Point DrawOffset { get; set; }
        public int OccludedIndex { get; set; }
        public int TextLength => BoundedText.TotalCharacterCount;

        public BoundedFormattedTextRenderer(Actor actor, Alignment alignment = default, Overflow overflow = Overflow.Elide, Depth depthOffset = default, FormattedText formattedText = default) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.alignment = alignment;
            this.depthOffset = depthOffset;
            this.overflow = overflow;
            SetText(formattedText);

            this.boundingRect.onSizeChange += OnSizeChange;
        }

        private void OnSizeChange(Point size)
        {
            UpdatedBoundedText();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            List<RenderableText> renderableTexts = BoundedText.GetRenderedText(transform.Position.ToPoint(), this.boundingRect.TopLeft.ToPoint(), OccludedIndex);

            foreach (var renderableText in renderableTexts)
            {
                renderableText.Draw(spriteBatch, DrawOffset, transform.Angle, transform.Depth + this.depthOffset);
                if (this.isDropShadowEnabled)
                {
                    renderableText.DrawDropShadow(spriteBatch, this.dropShadowColor, DrawOffset, transform.Angle, transform.Depth + this.depthOffset);
                }
            }
        }

        private FormattedText formattedText;

        public BoundedText BoundedText { get; private set; }

        public void SetText(FormattedText formattedText)
        {
            this.formattedText = formattedText;
            UpdatedBoundedText();
        }

        private void UpdatedBoundedText()
        {
            BoundedText = new BoundedText(this.boundingRect.Size, this.alignment, this.overflow, this.formattedText);
        }

        public BoundedFormattedTextRenderer EnableDropShadow(Color color)
        {
            this.dropShadowColor = color;
            this.isDropShadowEnabled = true;
            return this;
        }

        public void OccludeAll()
        {
            OccludedIndex = TextLength;
        }
    }
}