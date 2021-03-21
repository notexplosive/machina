using Machina.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public abstract class SpriteSheet : IAsset
    {
        public bool DebugMe = false;
        protected readonly Texture2D texture;
        public Texture2D SourceTexture => this.texture;
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

        public abstract void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, float scale, float angle, PointBool flip, Depth layerDepth, Color tintColor, bool isCentered = true);

        public void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, Depth layerDepth, float angle = 0f)
        {
            DrawFrame(spriteBatch, index, position, 1f, angle, new PointBool(false, false), layerDepth, Color.White);
        }

        public void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, Depth layerDepth, Color color)
        {
            DrawFrame(spriteBatch, index, position, 1f, 0f, new PointBool(false, false), layerDepth, color);
        }

        public void DrawFrame(SpriteBatch spriteBatch, int index, Transform transform, float scale = 1f, PointBool flip = default)
        {
            DrawFrame(spriteBatch, index, transform.Position, scale, transform.Angle, flip, transform.Depth, Color.White);
        }

        public abstract Rectangle GetSourceRectForFrame(int index);

        public void OnCleanup()
        {
            // This function is intentionally left blank
        }
    }
}
