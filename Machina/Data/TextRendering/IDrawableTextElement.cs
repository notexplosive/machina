using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public struct TextDrawingArgs
    {
        public Point Origin { get; set; }
        public Point Position { get; set; }
        public float Angle { get; set; }
        public Point AdditionalOffset { get; set; }
        public Depth Depth { get; set; }

        public Vector2 ResultOrigin()
        {
            return Origin.ToVector2();
        }

        public Vector2 ResultOffset()
        {
            return AdditionalOffset.ToVector2() - Position.ToVector2();
        }
    }

    public interface IDrawableTextElement
    {
        public string TokenText { get; }
        public Point Size { get; }
        int CharacterLength { get; }
        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation, int? substringLength = null);
        void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args);
        void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor);
        Point SizeOfCharacter(int characterIndex);
        char GetCharacterAt(int characterIndex);
    }
}
