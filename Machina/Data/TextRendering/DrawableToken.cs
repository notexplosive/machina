using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
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

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation)
        {
            return new RenderableText(FontMetrics, TokenText, totalAvailableRectLocation, Color, nodeLocation);
        }

        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, string newText)
        {
            return new RenderableText(FontMetrics, newText, totalAvailableRectLocation, Color, nodeLocation);
        }
    }
}
