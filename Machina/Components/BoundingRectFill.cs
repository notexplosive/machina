using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    public class BoundingRectFill : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Color color;
        private readonly Depth depthOffset = new Depth(0);

        public BoundingRectFill(Actor actor, Color color) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.color = color;
        }

        public BoundingRectFill(Actor actor, Color color, Depth depthOffset) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.color = color;
            this.depthOffset = depthOffset;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.FillRectangle(this.boundingRect.Rect, this.color, transform.Depth + this.depthOffset);
        }
    }
}
