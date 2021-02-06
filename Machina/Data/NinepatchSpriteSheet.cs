using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{

    class NinepatchSpriteSheet
    {
        public readonly NinepatchRects rects;
        private readonly Texture2D texture;

        public NinepatchSpriteSheet(Texture2D texture, Rectangle outerRect, Rectangle innerRect)
        {
            this.texture = texture;
            Debug.Assert(texture.Width >= outerRect.Width, "Texture is to small");
            Debug.Assert(texture.Height >= outerRect.Height, "Texture is to small");

            this.rects = new NinepatchRects(outerRect, innerRect);
        }

        public void DrawDebug(SpriteBatch spriteBatch)
        {
            spriteBatch.Draw(texture, this.rects.outer, this.rects.outer, Color.White);

            foreach (var frame in this.rects.raw)
            {
                spriteBatch.DrawRectangle(frame, Color.White);
            }
        }

        public void DrawSection(SpriteBatch spriteBatch, NinepatchIndex index, Rectangle destinationRect)
        {
            var sourceRect = this.rects.raw[(int) index];
            spriteBatch.Draw(texture, destinationRect, sourceRect, Color.White);
        }

        public void DrawFull(SpriteBatch spriteBatch, NinepatchRects destinationRects)
        {
            for (int i = 0; i < 9; i++)
            {
                spriteBatch.Draw(texture, destinationRects.raw[i], this.rects.raw[i], Color.White);
                //if debug: spriteBatch.DrawRectangle(destinationRects.raw[i], Color.White);
            }
        }
    }
}
