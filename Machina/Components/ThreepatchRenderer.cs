using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    enum Orientation
    {
        Horizontal,
        Vertical
    }

    class ThreepatchRenderer : NinepatchRenderer
    {
        private Orientation orientation;

        public ThreepatchRenderer(Actor actor, NinepatchSheet spriteSheet, Orientation orientation) : base(actor, spriteSheet)
        {
            this.orientation = orientation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var destinationRects = this.spriteSheet.GenerateDestinationRects(this.boundingRect.Rect);

            if (this.orientation == Orientation.Horizontal)
            {
                spriteSheet.DrawHorizontalThreepatch(spriteBatch, destinationRects, this.actor.depth);
            }
            else
            {
                spriteSheet.DrawVerticalThreepatch(spriteBatch, destinationRects, this.actor.depth);
            }
        }
    }
}