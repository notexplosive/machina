using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    internal class ThreepatchRenderer : NinepatchRenderer
    {
        private readonly Orientation orientation;

        public ThreepatchRenderer(Actor actor, NinepatchSheet spriteSheet, Orientation orientation) : base(actor,
            spriteSheet)
        {
            this.orientation = orientation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            if (this.orientation == Orientation.Horizontal)
            {
                Sheet.DrawHorizontalThreepatch(spriteBatch, this.boundingRect.Rect, this.actor.transform.Depth);
            }
            else
            {
                Sheet.DrawVerticalThreepatch(spriteBatch, this.boundingRect.Rect, this.actor.transform.Depth);
            }
        }
    }
}