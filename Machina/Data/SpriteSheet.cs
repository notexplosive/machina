using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    abstract class SpriteSheet : IAsset
    {
        protected readonly Texture2D texture;
        public IFrameAnimation DefaultAnimation
        {
            get
            {
                return new LinearFrameAnimation(0, FrameCount);
            }
        }

        public abstract int FrameCount
        {
            get;
        }

        public SpriteSheet(Texture2D texture)
        {
            this.texture = texture;
        }

        public abstract void DrawFrame(int index, SpriteBatch spriteBatch, Vector2 position, float scale, float angle, bool flipX, bool flipY, float layerDepth, Color tintColor);

        public void OnCleanup()
        {
            // This function is intentionally left blank
        }
    }
}
