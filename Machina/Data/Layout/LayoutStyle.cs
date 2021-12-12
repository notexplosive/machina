using Microsoft.Xna.Framework;

namespace Machina.Data.Layout
{
    public struct LayoutStyle
    {
        public Point Margin { get; }
        public int Padding { get; }
        public Alignment Alignment { get; }

        public LayoutStyle(Point margin = default, int padding = default, Alignment alignment = default)
        {
            Margin = margin;
            Padding = padding;
            Alignment = alignment;
        }

        public static readonly LayoutStyle Empty = new LayoutStyle();
    }
}
