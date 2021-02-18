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
        private readonly ToggleStateOnClick checkboxState;
        private readonly Clickable clickable;
        private readonly BoundingRect boundingRect;
        private readonly SpriteSheet checkboxSpriteSheet;
        private readonly Image checkmark;

        public CheckboxRenderer(Actor actor, SpriteSheet spriteSheet, Image checkmark, ToggleStateOnClick checkboxState, Clickable clickable) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.checkboxState = checkboxState;
            this.clickable = clickable;
            this.checkboxSpriteSheet = spriteSheet;
            this.checkmark = checkmark;

        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            int frame;
            if (this.clickable.IsHovered)
            {
                frame = 1;

                if (this.clickable.IsPrimedForLeftMouseButton)
                {
                    frame = 2;
                }
            }
            else
            {
                frame = 0;
            }

            this.checkboxSpriteSheet.DrawFrame(frame, spriteBatch, this.boundingRect.Rect.Center.ToVector2(), 1f, 0f, false, false, transform.Depth, Color.White);

            if (this.checkboxState.IsChecked)
            {
                this.checkmark.Draw(spriteBatch, this.boundingRect.Rect.Center.ToVector2(), 1f, 0f, false, false, transform.Depth - 000000.1f, Color.White);
            }
        }
    }
}
