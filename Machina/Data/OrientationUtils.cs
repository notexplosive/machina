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

        public Orientation Opposite()
        {
            if (this == Vertical)
            {
                return Horizontal;
            }

            return Vertical;
        }

        public Point GetPointForAlongAxis(int value)
        {
            return this == Vertical ? new Point(0, value) : new Point(value, 0);
        }

        public Point GetPointFromAlongPerpendicular(int along, int perpendicular)
        {
            return this == Orientation.Vertical ? new Point(perpendicular, along) : new Point(along, perpendicular);
        }
    }
}
