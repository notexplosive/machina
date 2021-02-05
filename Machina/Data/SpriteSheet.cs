using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    abstract class SpriteSheet
    {
        protected Texture2D texture;
        public LinearFrameAnimation DefaultAnimation
        {
            get
            {
                return new LinearFrameAnimation(this.FrameCount);
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

        public abstract void DrawFrame(int index, SpriteBatch spriteBatch, Vector2 position, float scale);
    }
}
