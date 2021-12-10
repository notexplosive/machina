using Machina.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.Layout
{
    public struct AspectRatio
    {
        public enum Description
        {
            Square,
            Wide,
            Tall
        }

        public AspectRatio(int width, int height)
        {
            Width = Math.Max(0, width);
            Height = Math.Max(0, height);
        }

        public AspectRatio(Point point) : this(point.X, point.Y)
        {

        }

        public int Width { get; }
        public int Height { get; }
        public float WidthOverHeight => (float) Width / Height;
        public float HeightOverWidth => (float) Height / Width;

        public bool IsNarrowAndTall()
        {
            return Width < Height;
        }

        public bool IsWideAndShort()
        {
            return Width > Height;
        }

        public bool IsSquare()
        {
            return Width == Height;
        }

        public Description Describe()
        {
            if (IsSquare())
            {
                return Description.Square;
            }

            if (IsWideAndShort())
            {
                return Description.Wide;
            }

            if (IsNarrowAndTall())
            {
                return Description.Tall;
            }

            throw new Exception("Impossible aspect ratio");
        }

        public bool IsStretchedAlongAssumingParentHasSameAspectClass(Orientation orientation)
        {
            if (IsSquare())
            {
                return true;
            }

            if (IsNarrowAndTall())
            {
                return orientation == Orientation.Vertical;
            }

            if (IsWideAndShort())
            {
                return orientation == Orientation.Horizontal;
            }

            throw new Exception("Impossible aspect ratio");
        }

        public static bool IsStretchedAlong(Description inner, Description outer, Orientation along)
        {
            if (inner == Description.Square && outer == Description.Square)
            {
                return true;
            }

            if (outer != inner)
            {
                if (inner == Description.Square)
                {
                    return OrientationUtils.Opposite(NaturalLongSide(outer)) == along;
                }

                if (inner == Description.Tall)
                {
                    return NaturalLongSide(inner) == along;
                }

                if (inner == Description.Wide)
                {
                    return NaturalLongSide(inner) == along;
                }
            }
            else
            {
                return NaturalLongSide(outer) == along;
            }

            throw new Exception("Missing case");
        }

        public static bool IsStretchedPerpendicular(Description inner, Description outer, Orientation along)
        {
            if (inner == Description.Square && outer == Description.Square)
            {
                return true;
            }

            return !IsStretchedAlong(inner, outer, along);
        }

        public static Orientation NaturalLongSide(Description description)
        {
            switch (description)
            {
                case Description.Tall:
                    return Orientation.Vertical;
                case Description.Wide:
                    return Orientation.Horizontal;
            }

            throw new Exception("Does not have natural long side");
        }
    }
}
