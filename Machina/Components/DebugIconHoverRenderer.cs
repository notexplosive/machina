using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    public class DebugIconHoverRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Hoverable hoverable;

        public DebugIconHoverRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.hoverable = RequireComponent<Hoverable>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var color = Color.Transparent;
            if (this.hoverable.IsHovered)
            {
                color = new Color(Color.LightBlue, 0.5f);
            }

            spriteBatch.FillRectangle(this.boundingRect.Rect, color, transform.Depth + 1);
        }
    }
}
