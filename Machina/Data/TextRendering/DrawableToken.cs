using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct DrawableToken : IDrawableTextElement
    {
        public Point Size { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public string TokenText { get; }
        public int CharacterLength => TokenText.Length;

        public DrawableToken(string tokenText, IFontMetrics fontMetrics, Color color)
        {
            FontMetrics = fontMetrics;
            Color = color;
            TokenText = tokenText;

            var tokenSize = FontMetrics.MeasureStringRounded(TokenText);

            if (TokenText == "\n")
            {
                Size = new Point(0, tokenSize.Y);
            }
            else
            {
                Size = tokenSize;
            }
        }

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation)
        {
            return new RenderableText(FontMetrics, TokenText, totalAvailableRectLocation, Color, nodeLocation);
        }

        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, int substringLength)
        {
            return new RenderableText(FontMetrics, TokenText.Substring(0, substringLength), totalAvailableRectLocation, Color, nodeLocation);
        }
    }
}
