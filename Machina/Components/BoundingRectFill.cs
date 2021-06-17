using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    public class BoundingRectFill : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Color color;

        public BoundingRectFill(Actor actor, Color color) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.color = color;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(this.boundingRect.Rect, this.color, transform.Depth);
        }
    }
}
