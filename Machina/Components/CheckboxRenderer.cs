using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    internal class CheckboxRenderer : BaseComponent
    {
        private readonly IFrameAnimation animation;
        private readonly BoundingRect boundingRect;
        private readonly SpriteSheet checkboxSpriteSheet;
        private readonly ICheckboxStateProvider checkboxState;
        private readonly Image checkmark;
        private readonly Clickable clickable;

        public CheckboxRenderer(Actor actor, SpriteSheet spriteSheet, Image checkmark,
            ICheckboxStateProvider checkboxState, Clickable clickable, IFrameAnimation animation) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.checkboxState = checkboxState;
            this.clickable = clickable;
            this.checkboxSpriteSheet = spriteSheet;
            this.checkmark = checkmark;
            this.animation = animation;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int frameIndex;
            if (this.clickable.IsHovered)
            {
                frameIndex = 1;

                if (this.clickable.IsPrimedForLeftMouseButton)
                {
                    frameIndex = 2;
                }
            }
            else
            {
                frameIndex = 0;
            }

            this.checkboxSpriteSheet.DrawFrame(spriteBatch, this.animation.GetFrame(frameIndex),
                this.boundingRect.Rect.Center.ToVector2(), 1f, 0f, new PointBool(false, false), transform.Depth,
                Color.White);

            if (this.checkboxState.GetIsChecked())
            {
                this.checkmark.Draw(spriteBatch, this.boundingRect.Rect.Center.ToVector2(), 1f, 0f,
                    new PointBool(false, false), transform.Depth - 1, Color.White);
            }
        }
    }
}
