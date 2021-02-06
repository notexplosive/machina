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
        private BoundingRect boundingRect;
        private NinepatchSpriteSheet spriteSheet;

        public NinepatchRenderer(Actor actor, NinepatchSpriteSheet spriteSheet) : base(actor)
        {
            this.boundingRect = RequireComponent<BoundingRect>();
            this.spriteSheet = spriteSheet;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            var outerDestinationRect = this.boundingRect.Rect;
            var innerDestinationRect = new Rectangle(
                outerDestinationRect.Left + this.spriteSheet.rects.LeftBuffer,
                outerDestinationRect.Top + this.spriteSheet.rects.TopBuffer,
                outerDestinationRect.Width - this.spriteSheet.rects.LeftBuffer - this.spriteSheet.rects.RightBuffer,
                outerDestinationRect.Height - this.spriteSheet.rects.TopBuffer - this.spriteSheet.rects.BottomBuffer);

            var destinationRects = new NinepatchRects(outerDestinationRect, innerDestinationRect);

            spriteSheet.DrawFull(spriteBatch, destinationRects);
        }
    }
}
