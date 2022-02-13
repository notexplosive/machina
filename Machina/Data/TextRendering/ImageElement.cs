using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public delegate void BoundedDrawFunction(SpriteBatch spriteBatch, TextDrawingArgs args);

    public readonly struct ImageElement : IDrawableTextElement
    {
        public ImageElement(Point size, BoundedDrawFunction drawFunction)
        {
            Size = size;
            this.drawFunction = drawFunction;
        }

        public string TokenText => "{symbol}";

        public Point Size { get; }

        private readonly BoundedDrawFunction drawFunction;

        public int CharacterLength => 1;

        public RenderableText CreateRenderableText(Point origin, Point topLeft, Point offset, int? substringLength = null)
        {
            // Ignores the substringLength argument
            return new RenderableText(this, TokenText, origin, topLeft, offset);
        }

        public void Draw(SpriteBatch spriteBatch, string text, TextDrawingArgs args)
        {
            this.drawFunction(spriteBatch, args);
        }

        public void DrawDropShadow(SpriteBatch spriteBatch, string text, TextDrawingArgs args, Color dropShadowColor)
        {
            this.drawFunction(spriteBatch, args);
        }

        public char GetCharacterAt(int characterIndex)
        {
            return '#';
        }

        public Point SizeOfCharacter(int characterIndex)
        {
            return Size;
        }

        public IDrawableTextElement ShrinkBy(int amountThatMustBeRemoved)
        {
            return new ImageElement(Size, EmptyDrawFunction);
        }

        public IDrawableTextElement AppendEllipse()
        {
            // Does nothing, this is technically a bug
            // should be something like: new TextElement("...", FontMetrics, Color)
            return new ImageElement(Size, EmptyDrawFunction);
        }

        private void EmptyDrawFunction(SpriteBatch spriteBatch, TextDrawingArgs args)
        {
            // this function is intentionally left blank
        }
    }
}
