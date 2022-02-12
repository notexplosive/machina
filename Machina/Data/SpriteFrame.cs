using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data
{
    public class SpriteFrame
    {
        private readonly int frame;
        public readonly ChooseFrameAnimation animation;
        public readonly SpriteSheet spriteSheet;

        public SpriteFrame(SpriteSheet spriteSheet, int frame)
        {
            this.frame = frame;
            this.spriteSheet = spriteSheet;
            this.animation = new ChooseFrameAnimation(frame);
        }

        public Point Size => this.spriteSheet.GetSourceRectForFrame(this.frame).Size;
        public void Draw(SpriteBatch spriteBatch, Vector2 position, float scale, float angle, XYBool flip, Depth depth, Color color, bool isCentered = true)
        {
            this.spriteSheet.DrawFrame(spriteBatch, this.frame, position, scale, angle, flip, depth, color, isCentered);
        }
    }
}