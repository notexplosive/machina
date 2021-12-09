using Machina.Components;
using System;

namespace Machina.Data.Layout
{
    public struct LayoutSize
    {
        public readonly ILayoutEdge X;
        public readonly ILayoutEdge Y;

        private LayoutSize(ILayoutEdge x, ILayoutEdge y)
        {
            X = x;
            Y = y;
        }

        public static LayoutSize Pixels(int x, int y) => new LayoutSize(new ConstLayoutEdge(x), new ConstLayoutEdge(y));
        public static LayoutSize Square(int x) => new LayoutSize(new ConstLayoutEdge(x), new ConstLayoutEdge(x));
        public static LayoutSize StretchedVertically(int width) => new LayoutSize(new ConstLayoutEdge(width), new StretchedLayoutEdge());
        public static LayoutSize StretchedHorizontally(int height) => new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(height));
        public static LayoutSize StretchedBoth() => new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge());

        public bool IsStretchedAlong(Orientation orientation)
        {
            return GetValueFromOrientation(orientation) is StretchedLayoutEdge;
        }

        public bool IsStretchedPerpendicular(Orientation orientation)
        {
            return GetValueFromOrientation(OrientationUtils.Opposite(orientation)) is StretchedLayoutEdge;
        }

        public ILayoutEdge GetValueFromOrientation(Orientation orientation)
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

        private struct ConstLayoutEdge : ILayoutEdge
        {
            public ConstLayoutEdge(int value)
            {
                Value = value;
            }

            public int Value { get; }

            public static implicit operator int(ConstLayoutEdge edge)
            {
                return edge.Value;
            }

            public bool IsStretched => false;
            public int ActualSize => Value;

            public override string ToString()
            {
                return ActualSize.ToString();
            }
        }

        private struct StretchedLayoutEdge : ILayoutEdge
        {
            public bool IsStretched => true;
            public int ActualSize => throw new Exception("StretchedLayoutEdge does not have an actual size");

            /// <summary>
            /// Do not delete! Important hack here
            /// </summary>
            /// <returns></returns>
            public override int GetHashCode()
            {
                // Hacky thing to make every single instance of StretchedLayoutEdge unique
                if (!this.hash.HasValue)
                {
                    this.hash = hashPool++;
                }
                return this.hash.Value;
            }

            private static int hashPool = 0;
            private int? hash;

            public override string ToString()
            {
                return $"stretched {this.hash}";
            }
        }

        public override string ToString()
        {
            return $"{X}, {Y}";
        }
    }
}
