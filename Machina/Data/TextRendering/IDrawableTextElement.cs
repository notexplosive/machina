using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace Machina.Data.TextRendering
{
    public interface IDrawableTextElement
    {
        public string TokenText { get; }
        public Point Size { get; }
        int CharacterLength { get; }

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation);
        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, int substringLength);
        void Draw(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth);
        void DrawDropShadow(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth, Color dropShadowColor);
        Point SizeOfCharacter(int characterIndex);
        char GetCharacterAt(int characterIndex);
    }

    public struct GenericDrawable : IDrawableTextElement
    {
        public GenericDrawable(Point size)
        {
            Size = size;
        }

        public string TokenText => "{symbol}";

        public Point Size { get; }

        public int CharacterLength => 1;

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation)
        {
            return new RenderableText(this, TokenText, totalAvailableRectLocation, nodeLocation);
        }

        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, int substringLength)
        {
            return CreateRenderableText(totalAvailableRectLocation, nodeLocation);
        }

        public void Draw(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth)
        {

        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, Point origin, Point offset, float angle, Point additionalOffset, Depth depth, Color dropShadowColor)
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
