using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public struct FormattedTextToken
    {
        public FormattedTextToken(string tokenText, IFontMetrics fontMetrics, Color color)
        {
            FontMetrics = fontMetrics;
            Color = color;
            TokenText = tokenText;
            Size = FontMetrics.MeasureStringRounded(TokenText);
        }

        public string TokenText { get; }
        public Point Size { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
    }
}
