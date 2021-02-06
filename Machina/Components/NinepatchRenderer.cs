using Machina.Data;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Components
{
    class NinepatchRenderer : DrawOnlyComponent
    {
        protected BoundingRect boundingRect;
        protected NinepatchSpriteSheet spriteSheet;

        public NinepatchRenderer(Actor actor, NinepatchSpriteSheet spriteSheet) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.spriteSheet = spriteSheet;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var outerDestinationRect = this.boundingRect.Rect;
            var innerDestinationRect = this.GenerateInnerDestinationRect(outerDestinationRect);
            var destinationRects = new NinepatchRects(outerDestinationRect, innerDestinationRect);

            spriteSheet.DrawFullNinepatch(spriteBatch, destinationRects);
        }

        protected Rectangle GenerateInnerDestinationRect(Rectangle outerDestinationRect)
        {
            return new Rectangle(
                outerDestinationRect.Left + this.spriteSheet.rects.LeftBuffer,
                outerDestinationRect.Top + this.spriteSheet.rects.TopBuffer,
                outerDestinationRect.Width - this.spriteSheet.rects.LeftBuffer - this.spriteSheet.rects.RightBuffer,
                outerDestinationRect.Height - this.spriteSheet.rects.TopBuffer - this.spriteSheet.rects.BottomBuffer);
        }
    }
}
