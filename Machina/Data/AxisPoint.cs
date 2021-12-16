using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public struct AxisPoint
    {
        private Point point;

        public AxisPoint(Point point)
        {
            this.point = point;
        }

        public static AxisPoint Zero => new AxisPoint(Point.Zero);

        public int AxisValue(Axis axis)
        {
            if (axis == Axis.X)
            {
                return point.X;
            }

            return point.Y;
        }

        public void SetAxisValue(Axis axis, int value)
        {
            if (axis == Axis.X)
            {
                point.X = value;
            }
            else
            {
                point.Y = value;
            }
        }

        public void SetOppositeAxisValue(Axis oppositeAxis, int value)
        {
            var axis = AxisUtils.Opposite(oppositeAxis);
            if (axis == Axis.X)
            {
                point.X = value;
            }
            else
            {
                point.Y = value;
            }
        }

        public int OppositeAxisValue(Axis axis)
        {
            if (AxisUtils.Opposite(axis) == Axis.X)
            {
                return point.X;
            }

            return point.Y;
        }

        public Point AsPoint(Axis firstAxis = Axis.X)
        {
            if (firstAxis == Axis.X)
            {
                return new Point(AxisValue(Axis.X), AxisValue(Axis.Y));
            }

            return new Point(AxisValue(Axis.Y), AxisValue(Axis.X));
        }

        public override bool Equals(object obj)
        {
            return obj is AxisPoint point &&
                   this.point.Equals(point.point);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(point);
        }

        public static bool operator ==(AxisPoint left, AxisPoint right)
        {
            return left.Equals(right);
        }

        public static bool operator !=(AxisPoint left, AxisPoint right)
        {
            return !(left == right);
        }
    }
}
