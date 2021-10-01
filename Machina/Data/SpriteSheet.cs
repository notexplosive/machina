using Machina.Components;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data
{
    public abstract class SpriteSheet : IAsset
    {
        protected readonly Texture2D texture;
        public bool DebugMe = false;

        public SpriteSheet(Texture2D texture)
        {
            this.texture = texture;
        }

        public Texture2D SourceTexture => this.texture;

        public IFrameAnimation DefaultAnimation => new LinearFrameAnimation(0, FrameCount);

        public abstract int FrameCount { get; }

        public void OnCleanup()
        {
            // This function is intentionally left blank
        }

        public abstract void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, float scale, float angle,
            PointBool flip, Depth layerDepth, Color tintColor, bool isCentered = true);

        public void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, Depth layerDepth, float angle = 0f)
        {
            DrawFrame(spriteBatch, index, position, 1f, angle, PointBool.False, layerDepth, Color.White);
        }

        public void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, Depth layerDepth, Color color,
            bool isCentered = true)
        {
            DrawFrame(spriteBatch, index, position, 1f, 0f, PointBool.False, layerDepth, color, isCentered);
        }

        public void DrawFrame(SpriteBatch spriteBatch, int index, Transform transform, float scale = 1f,
            PointBool flip = default)
        {
            DrawFrame(spriteBatch, index, transform.Position, scale, transform.Angle, flip, transform.Depth,
                Color.White);
        }

        public abstract Rectangle GetSourceRectForFrame(int index);
    }
}
