using System;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    internal class BoundingRectRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        public BoundingRectRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var rect = this.boundingRect.Rect;
            spriteBatch.DrawRectangle(rect, Color.Red, 1, (this.actor.transform.Depth - 1).AsFloat);
            spriteBatch.DrawCircle(this.actor.transform.Position, Math.Min(rect.Width, rect.Height) / 4, 16, Color.Red,
                1, (this.actor.transform.Depth - 1).AsFloat);
        }
    }
}