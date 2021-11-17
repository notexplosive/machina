using System.Diagnostics;
using Machina.Engine;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using MonoGame.Extended;

namespace Machina.Data
{
    /// <summary>
    ///     SpriteSheet that assumes the texture is arranged in a grid of frames where each frame is the same size
    /// </summary>
    public class GridBasedSpriteSheet : SpriteSheet
    {
        private readonly int columnCount;
        private readonly int frameCount;
        public readonly Point frameSize;
        private readonly int rowCount;

        public GridBasedSpriteSheet(string textureName, Point frameSize) : this(
            MachinaClient.Assets.GetTexture(textureName), frameSize)
        {
        }

        public GridBasedSpriteSheet(Texture2D texture, Point frameSize) : base(texture)
        {
            Debug.Assert(texture.Width % frameSize.X == 0, "Texture does not evenly divide by cell width");
            Debug.Assert(texture.Height % frameSize.Y == 0, "Texture does not evenly divide by cell height");

            this.frameSize = frameSize;
            this.columnCount = texture.Width / frameSize.X;
            this.rowCount = texture.Height / frameSize.Y;
            this.frameCount = this.columnCount * this.rowCount;
        }

        public override int FrameCount => this.frameCount;

        public override Rectangle GetSourceRectForFrame(int index)
        {
            var x = index % this.columnCount;
            var y = index / this.columnCount;
            return new Rectangle(new Point(x * this.frameSize.X, y * this.frameSize.Y), this.frameSize);
        }

        public override void DrawFrame(SpriteBatch spriteBatch, int index, Vector2 position, float scale, float angle,
            PointBool flip, Depth layerDepth, Color tintColor, bool isCentered = true)
        {
            Debug.Assert(index >= 0 && index <= this.frameCount, "Index out of range");

            var sourceRect = GetSourceRectForFrame(index);

            var adjustedFrameSize = this.frameSize.ToVector2() * scale;
            var destRect = new Rectangle(position.ToPoint(), adjustedFrameSize.ToPoint());

            var offset = Vector2.Zero;
            if (isCentered)
            {
                offset = this.frameSize.ToVector2() / 2;
            }

            spriteBatch.Draw(this.texture, destRect, sourceRect, tintColor, angle, offset,
                (flip.x ? SpriteEffects.FlipHorizontally : SpriteEffects.None) |
                (flip.y ? SpriteEffects.FlipVertically : SpriteEffects.None), layerDepth.AsFloat);

            if (this.DebugMe)
            {
                spriteBatch.Draw(this.texture, new Vector2(0, 0), Color.White);
                spriteBatch.DrawRectangle(new Rectangle(0, 0, this.texture.Width, this.texture.Height), Color.Red);
                spriteBatch.DrawRectangle(sourceRect, Color.White);
            }
        }
    }
}