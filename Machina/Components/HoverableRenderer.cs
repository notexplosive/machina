using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class HoverableRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;
        private bool isHoveredFromCallbacks;

        public HoverableRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
            this.hoverable.OnHoverStart += IndicateHoverStarted;
            this.hoverable.OnHoverEnd += IndicateHoverEnded;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.hoverable.IsHovered)
            {
                spriteBatch.FillRectangle(this.boundingRect.Rect, new Color(Color.Blue, 0.25f), this.actor.depth - 0.000001f);
            }

            if (this.isHoveredFromCallbacks)
            {
                spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.Orange, 2f, this.actor.depth - 0.000002f);
            }
        }

        public override void OnRemove()
        {
            this.hoverable.OnHoverStart -= IndicateHoverStarted;
            this.hoverable.OnHoverEnd -= IndicateHoverEnded;
        }

        private void IndicateHoverStarted()
        {
            this.isHoveredFromCallbacks = true;
        }

        private void IndicateHoverEnded()
        {
            this.isHoveredFromCallbacks = false;
        }
    }
}
