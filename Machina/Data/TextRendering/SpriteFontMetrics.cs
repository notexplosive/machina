using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public class SpriteFontMetrics : IFontMetrics
    {
        public SpriteFont Font { get; }

        public SpriteFontMetrics(SpriteFont font)
        {
            Font = font;
        }

        public int LineSpacing => Font.LineSpacing;
        public Vector2 MeasureString(string text)
        {
            return Font.MeasureString(text);
        }

        public static implicit operator SpriteFontMetrics(SpriteFont font)
        {
            return new SpriteFontMetrics(font);
        }
    }
}
