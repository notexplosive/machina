using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;

namespace Machina.Data.TextRendering
{
    public class SpriteFontMetrics : IFontMetrics
    {
        private readonly SpriteFont font;

        public SpriteFontMetrics(SpriteFont font)
        {
            this.font = font;
        }

        public int LineSpacing => this.font.LineSpacing;
        public Vector2 MeasureString(string text)
        {
            return this.font.MeasureString(text);
        }
    }
}
