using Machina.Components;

namespace Machina.Data
{
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
    }
}
