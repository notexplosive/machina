using Microsoft.Xna.Framework;
using System;

namespace Machina.Data
{
    public class Orientation
    {
        public static readonly Orientation Horizontal = new Orientation();
        public static readonly Orientation Vertical = new Orientation();

        private Orientation()
        {
        }

        public override bool Equals(object obj)
        {
            return (obj == (object) this);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public static bool operator ==(Orientation left, Orientation right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(Orientation left, Orientation right)
        {
            return !(left == right);
        }
    }

    public static class OrientationUtils
    {
        public static Orientation Opposite(Orientation orientation)
        {
            if (orientation == Orientation.Vertical)
            {
                return Orientation.Horizontal;
            }

            return Orientation.Vertical;
        }

        public static Point GetPointForAlongNode(Orientation orientation, int value)
        {
            return orientation == Orientation.Vertical ? new Point(0, value) : new Point(value, 0);
        }

        public static Point GetPointFromAlongPerpendicular(Orientation orientation, int along, int perpendicular)
        {
            return orientation == Orientation.Vertical ? new Point(perpendicular, along) : new Point(along, perpendicular);
        }
    }
}
