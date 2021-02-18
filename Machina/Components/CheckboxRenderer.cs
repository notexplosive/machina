using Machina.Data;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class CheckboxRenderer : BaseComponent
    {
        private readonly ICheckboxStateProvider checkboxState;
        private readonly Clickable clickable;
        private readonly BoundingRect boundingRect;
        private readonly SpriteSheet checkboxSpriteSheet;
        private readonly Image checkmark;
        private readonly IFrameAnimation animation;

        public CheckboxRenderer(Actor actor, SpriteSheet spriteSheet, Image checkmark, ICheckboxStateProvider checkboxState, Clickable clickable, IFrameAnimation animation) : base(actor)
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

            this.checkboxSpriteSheet.DrawFrame(this.animation.GetFrame(frameIndex), spriteBatch, this.boundingRect.Rect.Center.ToVector2(), 1f, 0f, false, false, transform.Depth, Color.White);

            if (this.checkboxState.GetIsChecked())
            {
                this.checkmark.Draw(spriteBatch, this.boundingRect.Rect.Center.ToVector2(), 1f, 0f, false, false, transform.Depth - 000000.1f, Color.White);
            }
        }
    }
}
