using Microsoft.Xna.Framework;

namespace Machina.Data.Layout
{
    public struct LayoutStyle
    {
        public readonly Point Margin { get; }
        public readonly int Padding { get; }

        public LayoutStyle(Point margin = default, int padding = default)
        {
            Margin = margin;
            Padding = padding;
        }

        public static readonly LayoutStyle Empty = new LayoutStyle();
    }
}
