using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using MonoGame.Extended;
using System.Text;

namespace Machina.Data
{
    /// <summary>
    /// SpriteSheet that assumes the texture is arranged in a grid of frames where each frame is the same size
    /// </summary>
    class GridBasedSpriteSheet : SpriteSheet
    {
        public readonly Point frameSize;

        private readonly int columnCount;
        private readonly int rowCount;
        private readonly int frameCount;

        public override int FrameCount
        {
            get
            {
                return this.frameCount;
            }
        }

        public GridBasedSpriteSheet(Texture2D texture, Point frameSize) : base(texture)
        {
            Debug.Assert(texture.Width % frameSize.X == 0, "Texture does not evenly divide by cell width");
            Debug.Assert(texture.Height % frameSize.Y == 0, "Texture does not evenly divide by cell height");

            this.frameSize = frameSize;
            this.columnCount = texture.Width / frameSize.X;
            this.rowCount = texture.Height / frameSize.Y;
            this.frameCount = columnCount * rowCount;
        }

        public override void DrawFrame(int index, SpriteBatch spriteBatch, Vector2 position, float scale, float angle, bool flipX, bool flipY)
        {
            Debug.Assert(index >= 0 && index <= this.frameCount, "Index out of range");

            int x = index % this.columnCount;
            int y = index / this.columnCount;
            var sourceRect = new Rectangle(new Point(x * frameSize.X, y * frameSize.Y), frameSize);

            /*
            spriteBatch.Draw(this.texture, new Vector2(0, 0), Color.White);
            spriteBatch.DrawRectangle(new Rectangle(0, 0, this.texture.Width, this.texture.Height), Color.Red);
            spriteBatch.DrawRectangle(sourceRect, Color.White);
            */

            var adjustedFrameSize = (this.frameSize.ToVector2() * scale);
            var destRect = new Rectangle(position.ToPoint(), adjustedFrameSize.ToPoint());

            spriteBatch.Draw(this.texture, destRect, sourceRect, Color.White, angle, frameSize.ToVector2() / 2,
                (flipX ? SpriteEffects.FlipHorizontally : SpriteEffects.None) | (flipY ? SpriteEffects.FlipVertically : SpriteEffects.None), 0);
        }
    }
}
