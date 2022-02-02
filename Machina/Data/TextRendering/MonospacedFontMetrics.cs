using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public class MonospacedFontMetrics : IFontMetrics
    {
        public Point Size { get; }

        public MonospacedFontMetrics(Point size)
        {
            this.Size = size;
        }

        public int LineSpacing => Size.Y;

        public Vector2 MeasureString(string text)
        {
            return new Vector2(Size.X * text.Length, LineSpacing);
        }
    }
}
