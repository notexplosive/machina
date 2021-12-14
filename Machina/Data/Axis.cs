using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public enum Axis
    {
        X,
        Y
    }

    public static class AxisUtils
    {
        public static Axis Opposite(Axis axis)
        {
            if (axis == Axis.X)
            {
                return Axis.Y;
            }

            return Axis.X;
        }

        public static Point GetPoint(Point source, Axis axis)
        {
            if (axis == Axis.X)
            {
                return new Point(source.X, source.Y);
            }

            return new Point(source.Y, source.X);
        }

        public static int GetAxisValue(Point source, Axis axis)
        {
            if (axis == Axis.X)
            {
                return source.X;
            }

            return source.Y;
        }

        public static Point CreatePoint(int x, int y, Axis axis)
        {
            if (axis == Axis.X)
            {
                return new Point(x, y);
            }

            return new Point(y, x);
        }

        public static void DoForBothAxes(Action<Axis> function)
        {
            function(Axis.X);
            function(Axis.Y);
        }
    }
}
