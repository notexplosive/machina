using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public static class PointExtensions
    {
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
    }
}
