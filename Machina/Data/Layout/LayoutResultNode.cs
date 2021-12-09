using Microsoft.Xna.Framework;

namespace Machina.Data.Layout
{
    public struct LayoutResultNode
    {
        public Point PositionRelativeToRoot { get; }
        public Point Size { get; }
        public Rectangle Rectangle { get; }
        public int NestingLevel { get; }

        public LayoutResultNode(Point position, Point size, int nestingLevel)
        {
            PositionRelativeToRoot = position;
            Size = size;
            Rectangle = new Rectangle(PositionRelativeToRoot, Size);
            NestingLevel = nestingLevel;
        }

        public override string ToString()
        {
            return $"{Size}, level={NestingLevel}";
        }
    }
}
