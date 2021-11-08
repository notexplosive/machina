using System.Diagnostics;
using Microsoft.Xna.Framework;

namespace Machina.Data
{
    public enum NinepatchIndex
    {
        TopLeft = 0,
        TopCenter = 1,
        TopRight = 2,
        LeftCenter = 3,
        Center = 4,
        RightCenter = 5,
        BottomLeft = 6,
        BottomCenter = 7,
        BottomRight = 8
    }

    public enum Side
    {
        Right,
        Bottom,
        Left,
        Top
    }

    public struct NinepatchRects
    {
        public readonly Rectangle[] raw;
        public readonly int[] sidePixelBuffers;
        public readonly Rectangle inner;
        public readonly Rectangle outer;
        public readonly bool isValidNinepatch;

        public NinepatchRects(Rectangle outer, Rectangle inner)
        {
            Debug.Assert(outer.Contains(inner), "InnerRect is not contained by OuterRect");

            var topBuffer = inner.Top - outer.Top;
            var rightBuffer = outer.Right - inner.Right;
            var leftBuffer = inner.Left - outer.Left;
            var bottomBuffer = outer.Bottom - inner.Bottom;

            this.sidePixelBuffers = new int[4]
            {
                rightBuffer,
                bottomBuffer,
                leftBuffer,
                topBuffer
            };

            this.raw = new Rectangle[9]
            {
                new Rectangle(outer.Left, outer.Top, this.sidePixelBuffers[(int) Side.Left],
                    this.sidePixelBuffers[(int) Side.Top]),
                new Rectangle(inner.Left, outer.Top, inner.Width, this.sidePixelBuffers[(int) Side.Top]),
                new Rectangle(inner.Right, outer.Top, this.sidePixelBuffers[(int) Side.Right],
                    this.sidePixelBuffers[(int) Side.Top]),
                new Rectangle(outer.Left, inner.Top, this.sidePixelBuffers[(int) Side.Left], inner.Height),
                inner,
                new Rectangle(inner.Right, inner.Top, this.sidePixelBuffers[(int) Side.Right], inner.Height),
                new Rectangle(outer.Left, inner.Bottom, this.sidePixelBuffers[(int) Side.Left],
                    this.sidePixelBuffers[(int) Side.Bottom]),
                new Rectangle(inner.Left, inner.Bottom, inner.Width, this.sidePixelBuffers[(int) Side.Bottom]),
                new Rectangle(inner.Right, inner.Bottom, this.sidePixelBuffers[(int) Side.Right],
                    this.sidePixelBuffers[(int) Side.Bottom])
            };
            this.inner = inner;
            this.outer = outer;

            this.isValidNinepatch = true;
            foreach (var rect in this.raw)
            {
                if (rect.Width * rect.Height == 0)
                {
                    this.isValidNinepatch = false;
                }
            }
        }

        public Rectangle TopLeft => this.raw[(int) NinepatchIndex.TopLeft];
        public Rectangle TopCenter => this.raw[(int) NinepatchIndex.TopCenter];
        public Rectangle TopRight => this.raw[(int) NinepatchIndex.TopRight];
        public Rectangle LeftCenter => this.raw[(int) NinepatchIndex.LeftCenter];
        public Rectangle Center => this.raw[(int) NinepatchIndex.Center];
        public Rectangle RightCenter => this.raw[(int) NinepatchIndex.RightCenter];
        public Rectangle BottomLeft => this.raw[(int) NinepatchIndex.BottomLeft];
        public Rectangle BottomCenter => this.raw[(int) NinepatchIndex.BottomCenter];
        public Rectangle BottomRight => this.raw[(int) NinepatchIndex.BottomRight];
        public int LeftBuffer => this.sidePixelBuffers[(int) Side.Left];
        public int RightBuffer => this.sidePixelBuffers[(int) Side.Right];
        public int TopBuffer => this.sidePixelBuffers[(int) Side.Top];
        public int BottomBuffer => this.sidePixelBuffers[(int) Side.Bottom];

        public bool IsValidHorizontalThreepatch
        {
            get
            {
                foreach (var rect in new[] {LeftCenter, Center, RightCenter})
                {
                    if (rect.Width * rect.Height == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }

        public bool IsValidVerticalThreepatch
        {
            get
            {
                foreach (var rect in new[] {TopCenter, Center, BottomCenter})
                {
                    if (rect.Width * rect.Height == 0)
                    {
                        return false;
                    }
                }

                return true;
            }
        }
    }
}