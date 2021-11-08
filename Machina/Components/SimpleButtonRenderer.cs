using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Components
{
    internal class SimpleButtonRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;
        private readonly Clickable clickable;

        public SimpleButtonRenderer(Actor actor) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.clickable = RequireComponent<Clickable>();
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            spriteBatch.DrawRectangle(this.boundingRect.Rect, this.clickable.IsHovered ? Color.Blue : Color.Red,
                this.clickable.IsPrimedForAnyButton ? 5 : 1);
        }
    }
}