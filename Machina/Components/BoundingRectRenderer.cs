using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class BoundingRectRenderer : DrawOnlyComponent
    {
        private BoundingRect boundingRect;

        public BoundingRectRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, Color.Pink);
            spriteBatch.DrawCircle(this.actor.position, 3, 5, Color.Pink);
        }
    }
}
