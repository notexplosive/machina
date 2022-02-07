using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public static class PointExtensions
    {
        public static Point WithJustAxisValue(this Point point, Axis axis)
        {
            if (axis == Axis.X)
            {
                return new Point(point.X, 0);
            }
            else
            {
                return new Point(0, point.Y);
            }
        }

        public static int AxisValue(this Point point, Axis axis)
        {
            if (axis == Axis.X)
            {
                return point.X;
            }
            else
            {
                return point.Y;
            }
        }

        public static int OppositeAxisValue(this Point point, Axis axis)
        {
            if (axis == Axis.Y)
            {
                return point.X;
            }
            else
            {
                return point.Y;
            }
        }

        public static int SetOppositeAxisValue(ref this Point point, Axis axis, int value)
        {
            if (axis == Axis.Y)
            {
                return point.X = value;
            }
            else
            {
                return point.Y = value;
            }
        }

        public static int SetAxisValue(ref this Point point, Axis axis, int value)
        {
            if (axis == Axis.X)
            {
                return point.X = value;
            }
            else
            {
                return point.Y = value;
            }
        }
    }
}
