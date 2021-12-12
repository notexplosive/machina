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

        public static bool IsStretchedAlong(AspectRatio inner, AspectRatio outer, Orientation along)
        {
            if (inner.WidthOverHeight == outer.WidthOverHeight)
            {
                return true;
            }


            if (inner.WidthOverHeight > outer.WidthOverHeight)
            {
                if (along == Orientation.Horizontal)
                {
                    return true;
                }
            }

            if (inner.WidthOverHeight < outer.WidthOverHeight)
            {
                if (along == Orientation.Vertical)
                {
                    return true;
                }
            }

            return false;
        }

        public static bool IsStretchedPerpendicular(AspectRatio inner, AspectRatio outer, Orientation along)
        {
            if (inner.WidthOverHeight == outer.WidthOverHeight)
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

        public float AlongOverPerpendicular(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return WidthOverHeight;
            }
            else
            {
                return HeightOverWidth;
            }
        }
    }
}
