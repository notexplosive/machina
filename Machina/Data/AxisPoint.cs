using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public struct AxisPoint
    {
        private Point point;
        private readonly Axis alongAxis;

        public AxisPoint(Axis alongAxis, int along = 0, int perpendicular = 0)
        {
            this.alongAxis = alongAxis;
            this.point = Point.Zero;
            SetAlong(along);
            SetPerpendicular(perpendicular);
        }

        public AxisPoint(Orientation orientation, int along = 0, int perpendicular = 0) : this(orientation.ToAxis(), along, perpendicular)
        {

        }

        public static AxisPoint Zero(Axis axis) => new AxisPoint(axis);

        public int Along()
        {
            if (this.alongAxis == Axis.X)
            {
                return point.X;
            }

            return point.Y;
        }

        public void SetAlong(int value)
        {
            if (this.alongAxis == Axis.X)
            {
                point.X = value;
            }
            else
            {
                point.Y = value;
            }
        }

        public void SetPerpendicular(int value)
        {
            var axis = AxisUtils.Opposite(this.alongAxis);
            if (axis == Axis.X)
            {
                point.X = value;
            }
            else
            {
                point.Y = value;
            }
        }

        public int Perpendicular()
        {
            if (AxisUtils.Opposite(this.alongAxis) == Axis.X)
            {
                return point.X;
            }

            return point.Y;
        }

        public Point AsPoint()
        {
            if (this.alongAxis == Axis.X)
            {
                return new Point(Along(), Perpendicular());
            }
            else
            {
                return new Point(Perpendicular(), Along());
            }
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
