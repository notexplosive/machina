using Machina.Components;
using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;

namespace Machina.Data.Layout
{
    public struct LayoutSize
    {
        public readonly ILayoutEdge Width;
        public readonly ILayoutEdge Height;

        private LayoutSize(ILayoutEdge x, ILayoutEdge y)
        {
            Width = x;
            Height = y;
        }

        public static LayoutSize Pixels(int x, int y) => new LayoutSize(new ConstLayoutEdge(x), new ConstLayoutEdge(y));
        public static LayoutSize Pixels(Point xy) => new LayoutSize(new ConstLayoutEdge(xy.X), new ConstLayoutEdge(xy.Y));
        public static LayoutSize Square(int x) => new LayoutSize(new ConstLayoutEdge(x), new ConstLayoutEdge(x));
        public static LayoutSize StretchedVertically(int width) => new LayoutSize(new ConstLayoutEdge(width), new StretchedLayoutEdge());
        public static LayoutSize StretchedHorizontally(int height) => new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(height));
        public static LayoutSize StretchedBoth() => new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge());
        public static LayoutSize FixedAspectRatio(int width, int height) => new LayoutSize(new FixedAspectRatioLayoutEdge(width), new FixedAspectRatioLayoutEdge(height));

        public bool IsFixedAspectRatio()
        {
            var xIsFixed = Width is FixedAspectRatioLayoutEdge;
            var yIsFixed = Height is FixedAspectRatioLayoutEdge;
            Debug.Assert(xIsFixed == yIsFixed); // it only makes sense to have both edges be fixed

            return xIsFixed && yIsFixed;
        }

        public AspectRatio GetAspectRatio()
        {
            return new AspectRatio(Width.AspectSize, Height.AspectSize);
        }

        public bool IsStretchedAlong(Orientation orientation, AspectRatio parentAspect)
        {
            var edge = GetValueFromOrientation(orientation);
            if (edge is StretchedLayoutEdge)
            {
                return true;
            }

            if (edge is FixedAspectRatioLayoutEdge)
            {
                var parentAspectRatio = parentAspect;
                var childAspectRatio = GetAspectRatio();
                return AspectRatio.IsStretchedAlong(childAspectRatio, parentAspectRatio, orientation);
            }

            return false;
        }

        public bool IsStretchedPerpendicular(Orientation orientation, AspectRatio parentAspect)
        {
            var edge = GetValueFromOrientation(OrientationUtils.Opposite(orientation));
            if (edge is StretchedLayoutEdge)
            {
                return true;
            }

            if (edge is FixedAspectRatioLayoutEdge)
            {
                var parentAspectRatio = parentAspect;
                var childAspectRatio = GetAspectRatio();
                return AspectRatio.IsStretchedPerpendicular(childAspectRatio, parentAspectRatio, orientation);
            }

            return false;
        }

        public bool IsMeasurableAlong(Orientation orientation)
        {
            return GetValueFromOrientation(orientation) is ConstLayoutEdge;
        }

        public ILayoutEdge GetValueFromOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return Width;
            }

            if (orientation == Orientation.Vertical)
            {
                return Height;
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

            public bool IsConstant => true;
            public int ActualSize => Value;
            public int AspectSize => Value;

            public override string ToString()
            {
                return ActualSize.ToString();
            }
        }

        private struct StretchedLayoutEdge : ILayoutEdge
        {
            public bool IsConstant => false;
            public int ActualSize => throw new Exception("StretchedLayoutEdge does not have an actual size");
            public int AspectSize => throw new Exception("StretchedLayoutEdge does not have an aspect size");

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

        private struct FixedAspectRatioLayoutEdge : ILayoutEdge
        {
            public FixedAspectRatioLayoutEdge(int size)
            {
                AspectSize = size;
                this.hash = null;
            }

            public bool IsConstant => false;

            public int ActualSize => throw new Exception("FixedAspectRatioLayoutEdge does not have an actual size");

            public int AspectSize { get; }

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
                return $"fixed aspect ratio {ActualSize}";
            }
        }

        public override string ToString()
        {
            return $"{Width}, {Height}";
        }
    }
}
