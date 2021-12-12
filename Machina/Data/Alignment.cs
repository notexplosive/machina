using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public enum VerticalAlignment
    {
        Top,
        Center,
        Bottom
    }

    public enum HorizontalAlignment
    {
        Left,
        Center,
        Right
    }

    public enum Overflow
    {
        Elide,
        Ignore
    }

    public struct Alignment
    {
        public HorizontalAlignment Horizontal { get; }
        public VerticalAlignment Vertical { get; }

        public Alignment(HorizontalAlignment horizontal = HorizontalAlignment.Left, VerticalAlignment vertical = VerticalAlignment.Top)
        {
            Horizontal = horizontal;
            Vertical = vertical;
        }

        public static Alignment TopLeft { get; } = new Alignment();
        public static Alignment TopRight { get; } = new Alignment(horizontal: HorizontalAlignment.Right);
        public static Alignment TopCenter { get; } = new Alignment(horizontal: HorizontalAlignment.Center, vertical: VerticalAlignment.Top);
        public static Alignment BottomCenter { get; } = new Alignment(horizontal: HorizontalAlignment.Center, vertical: VerticalAlignment.Bottom);
        public static Alignment BottomRight { get; } = new Alignment(horizontal: HorizontalAlignment.Right, vertical: VerticalAlignment.Bottom);
        public static Alignment BottomLeft { get; } = new Alignment(horizontal: HorizontalAlignment.Left, vertical: VerticalAlignment.Bottom);
        public static Alignment Center { get; } = new Alignment(horizontal: HorizontalAlignment.Center, vertical: VerticalAlignment.Center);
        public static Alignment CenterRight { get; } = new Alignment(horizontal: HorizontalAlignment.Right, vertical: VerticalAlignment.Center);
        public static Alignment CenterLeft { get; } = new Alignment(horizontal: HorizontalAlignment.Left, vertical: VerticalAlignment.Center);

        public Point AddPostionDeltaFromMargin(Point margin)
        {
            var xFactor = 1;
            int yFactor = 1;
            if (Horizontal == HorizontalAlignment.Right)
            {
                xFactor = -1;
            }

            if (Vertical == VerticalAlignment.Bottom)
            {
                yFactor = -1;
            }
            return new Point(margin.X * xFactor, margin.Y * yFactor);
        }

        public Point GetRelativePositionOfElement(Point availableSpace, Point totalUsedSpace)
        {
            var result = Point.Zero;

            if (Horizontal == HorizontalAlignment.Center)
            {
                result.X = availableSpace.X / 2 - totalUsedSpace.X / 2;
            }

            if (Horizontal == HorizontalAlignment.Right)
            {
                result.X = availableSpace.X - totalUsedSpace.X;
            }

            if (Vertical == VerticalAlignment.Center)
            {
                result.Y = availableSpace.Y / 2 - totalUsedSpace.Y / 2;
            }

            if (Vertical == VerticalAlignment.Bottom)
            {
                result.Y = availableSpace.Y - totalUsedSpace.Y;
            }


            // assumes top left is default
            return result;
        }
    }
}
