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
            if (this.orientation == Orientation.Horizontal)
            {
                SpriteSheet.DrawHorizontalThreepatch(spriteBatch, this.boundingRect.Rect, this.actor.transform.Depth);
            }
            else
            {
                SpriteSheet.DrawVerticalThreepatch(spriteBatch, this.boundingRect.Rect, this.actor.transform.Depth);
            }
        }
    }
}