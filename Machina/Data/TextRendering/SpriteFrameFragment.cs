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
            return new FormattedTextToken[] { new FormattedTextToken(new ImageElement(this.spriteFrame.Size, DrawFunction)) };
        }

        private void DrawFunction(SpriteBatch spriteBatch, TextDrawingArgs args)
        {
            this.spriteFrame.Draw(spriteBatch, args.FinalPosition(), 1f, args.Angle, XYBool.False, args.Depth, Color.White, false);
        }
    }
}
