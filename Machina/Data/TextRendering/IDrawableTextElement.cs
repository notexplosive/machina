using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public interface IDrawableTextElement
    {
        public string TokenText { get; }
        public Point Size { get; }
        int CharacterLength { get; }
        public RenderableText CreateRenderableText(Point origin, Point offset, int? substringLength = null);
        void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args);
        void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor);
        Point SizeOfCharacter(int characterIndex);
        char GetCharacterAt(int characterIndex);
    }
}
