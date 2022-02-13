using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data.TextRendering
{
    public readonly struct SpriteFrameFragment : ITextInputFragment
    {
        private readonly SpriteFrame spriteFrame;

        public SpriteFrameFragment(SpriteFrame spriteFrame)
        {
            this.spriteFrame = spriteFrame;
        }

        public FormattedTextToken[] Tokens()
        {
            return new FormattedTextToken[] { new FormattedTextToken(new ImageToken(this.spriteFrame.Size, DrawFunction)) };
        }

        private void DrawFunction(SpriteBatch spriteBatch, Rectangle bounds, Depth depth)
        {
            this.spriteFrame.Draw(spriteBatch, new Vector2(bounds.Left, bounds.Top), 1f, /*angle*/0, XYBool.False, depth, Color.White, false);
        }
    }
}
