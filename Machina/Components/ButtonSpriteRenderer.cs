using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Components
{
    internal class ButtonSpriteRenderer : BaseComponent
    {
        private readonly BoundingRect boundingRect;

        private readonly Clickable clickable;
        private readonly IFrameAnimation frames;
        private readonly SpriteSheet spriteSheet;
        private HoverSprite currentFrame;

        public ButtonSpriteRenderer(Actor actor, SpriteSheet spriteSheet, IFrameAnimation frames) : base(actor)
        {
            this.clickable = RequireComponent<Clickable>();
            this.boundingRect = RequireComponent<BoundingRect>();
            this.spriteSheet = spriteSheet;
            this.frames = frames;
            this.currentFrame = HoverSprite.Idle;
        }

        public override void Update(float dt)
        {
            if (this.clickable.IsHovered)
            {
                if (this.clickable.IsPrimedForLeftMouseButton)
                {
                    this.currentFrame = HoverSprite.Pressed;
                }
                else
                {
                    this.currentFrame = HoverSprite.Hovered;
                }
            }
            else
            {
                this.currentFrame = HoverSprite.Idle;
            }
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var frame = (int) this.currentFrame;
            var center =
                this.boundingRect.Rect.Center
                    .ToVector2(); // - this.spriteSheet.GetSourceRectForFrame(frame).Size.ToVector2() / 2;
            this.spriteSheet.DrawFrame(spriteBatch, this.frames.GetFrame(frame), center, transform.Depth);
        }

        private enum HoverSprite
        {
            Idle,
            Hovered,
            Pressed
        }
    }
}
