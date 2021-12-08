using Machina.Components;
using System;

namespace Machina.Data
{
    public struct XYPair<T>
    {
        public T X;
        public T Y;

        public XYPair(T x, T y)
        {
            this.X = x;
            this.Y = y;
        }

        public T GetValueFromOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return X;
            }

            if (orientation == Orientation.Vertical)
            {
                return Y;
            }

            throw new ArgumentException("Invalid orientation");
        }
    }
}