using Machina.Data;
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

        public ThreepatchRenderer(Actor actor, NinepatchSpriteSheet spriteSheet, Orientation orientation) : base(actor, spriteSheet)
        {
            this.orientation = orientation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var outerDestinationRect = this.boundingRect.Rect;
            var innerDestinationRect = this.GenerateInnerDestinationRect(outerDestinationRect);
            var destinationRects = new NinepatchRects(outerDestinationRect, innerDestinationRect);

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