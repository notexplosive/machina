using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data.TextRendering
{
    public delegate void BoundedDrawFunction(SpriteBatch spriteBatch, Rectangle bounds, Depth depth);

    public readonly struct ImageToken : IDrawableTextElement
    {
        public ImageToken(Point size, BoundedDrawFunction drawFunction)
        {
            Size = size;
            this.drawFunction = drawFunction;
        }

        public string TokenText => "{symbol}";

        public Point Size { get; }

        private readonly BoundedDrawFunction drawFunction;

        public int CharacterLength => 1;

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation, int? substringLength = null)
        {
            // Ignores the substringLength argument
            return new RenderableText(this, TokenText, totalAvailableRectLocation, nodeLocation);
        }

        public void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args)
        {
            var finalPosition = args.ResultOrigin() - args.ResultOffset(); // duplicate below, probably wrong calculation anyway
            this.drawFunction(spriteBatch, new Rectangle(finalPosition.ToPoint(), Size), args.Depth);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor)
        {
            var finalPosition = args.ResultOrigin() - args.ResultOffset(); // duplicated above
            this.drawFunction(spriteBatch, new Rectangle(finalPosition.ToPoint(), Size), args.Depth);
        }

        public char GetCharacterAt(int characterIndex)
        {
            return '#';
        }

        public Point SizeOfCharacter(int characterIndex)
        {
            return Size;
        }
    }
}
