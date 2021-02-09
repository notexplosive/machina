using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class BoundingRectRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        public BoundingRectRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            Rectangle rect = this.boundingRect.Rect;
            spriteBatch.DrawRectangle(rect, Color.Red, 1, this.actor.depth - 0.000001f);
            spriteBatch.DrawCircle(this.actor.Position, Math.Min(rect.Width, rect.Height) / 4, 16, Color.Red, 1, this.actor.depth - 0.000001f);
        }
    }
}
