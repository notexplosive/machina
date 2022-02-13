using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data.TextRendering
{
    public struct ImageToken : IDrawableTextElement
    {
        public ImageToken(Point size)
        {
            Size = size;
        }

        public string TokenText => "{symbol}";

        public Point Size { get; }

        public int CharacterLength => 1;

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation, int? substringLength = null)
        {
            // Ignores the substringLength argument
            return new RenderableText(this, TokenText, totalAvailableRectLocation, nodeLocation);
        }

        public void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args)
        {

        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor)
        {

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
