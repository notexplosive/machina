using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    class Image : IAsset
    {
        private readonly SpriteSheet spriteSheet;
        private readonly int frame;

        public Image(SpriteSheet spriteSheet, int frame)
        {
            Debug.Assert(frame >= 0 && frame < spriteSheet.FrameCount);
            this.spriteSheet = spriteSheet;
            this.frame = frame;
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, float angle, bool flipX, bool flipY, float layerDepth, Color color)
        {
            spriteSheet.DrawFrame(frame, spriteBatch, position, scale, angle, flipX, flipY, layerDepth, color);
        }

        public void OnCleanup()
        {

        }
    }
}
