using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct ImageTextFragment : ITextInputFragment
    {
        public ImageTextFragment(Point size, BoundedDrawFunction drawFunction)
        {
            Size = size;
            this.drawFunction = drawFunction;
        }

        public Point Size { get; }

        private readonly BoundedDrawFunction drawFunction;

        public FormattedTextToken[] Tokens()
        {
            return new FormattedTextToken[] { new FormattedTextToken(new ImageToken(Size, this.drawFunction)) };
        }
    }
}
