using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextOutputFragment
    {
        public TextOutputFragment(string token, IFontMetrics fontMetrics, Color color)
        {
            Text = token;
            FontMetrics = fontMetrics;
            Color = color;
        }

        public string Text { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
    }
}
