using System.Diagnostics;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data
{
    public struct XYBool
    {
        public XYBool(bool x, bool y)
        {
            this.x = x;
            this.y = y;
        }

        public bool x;
        public bool y;

        public static XYBool False = new XYBool(false, false);
        public static XYBool True = new XYBool(true, true);
    }

    public class Image : IAsset
    {
        private readonly int frame;
        private readonly SpriteSheet spriteSheet;

        public Image(SpriteSheet spriteSheet, int frame)
        {
            Debug.Assert(frame >= 0 && frame < spriteSheet.FrameCount);
            this.spriteSheet = spriteSheet;
            this.frame = frame;
        }

        public void OnCleanup()
        {
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, float angle, XYBool flip,
            Depth layerDepth, Color color)
        {
            this.spriteSheet.DrawFrame(spriteBatch, this.frame, position, scale, angle, flip, layerDepth, color);
        }
    }
}