using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public interface IDrawableTextElement
    {
        public string TokenText { get; }
        public Point Size { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
    }

    public readonly struct DrawableToken : IDrawableTextElement
    {
        public Point Size { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public string TokenText { get; }

        public DrawableToken(string tokenText, IFontMetrics fontMetrics, Color color)
        {
            FontMetrics = fontMetrics;
            Color = color;
            TokenText = tokenText;
            Size = FontMetrics.MeasureStringRounded(TokenText);
        }
    }

    public readonly struct FormattedTextToken
    {
        public FormattedTextToken(IDrawableTextElement drawable)
        {
            Drawable = drawable;
        }

        public IDrawableTextElement Drawable { get; }

        public TextOutputFragment CreateOutputFragment(int characterIndex)
        {
            return new TextOutputFragment(Drawable, characterIndex);
        }
    }
}
