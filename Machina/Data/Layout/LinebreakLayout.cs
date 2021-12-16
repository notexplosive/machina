using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data.Layout
{
    public class LinebreakLayout
    {
        public int RestrictedAlongSize { get; }
        public int RestrictedPerpendicularSize { get; }

        public List<Point> items = new List<Point>();
        private readonly Axis alongAxis;

        public LinebreakLayout(Axis alongAxis, int restrictAlong = int.MaxValue, int restrictPerpendicular = int.MaxValue)
        {
            this.alongAxis = alongAxis;
            RestrictedAlongSize = restrictAlong;
            RestrictedPerpendicularSize = restrictPerpendicular;
        }

        public LinebreakLayout AddItem(Point size)
        {
            items.Add(size);
            return this;
        }

        public Point TotalSize()
        {
            var size = AxisPoint.Zero;
            size.SetAxisValue(this.alongAxis, RestrictedAlongSize);
            size.SetAxisValue(AxisUtils.Opposite(this.alongAxis), RestrictedPerpendicularSize);
            return size.AsPoint();
        }

        public Rectangle[] Flow()
        {
            var perpendicularAxis = AxisUtils.Opposite(alongAxis);
            var layoutBlock = new LayoutBlock(alongAxis);
            var currentLine = new LayoutLine(alongAxis);

            foreach (var size in items)
            {
                if (currentLine.TotalSizeIfAdded(size).AxisValue(alongAxis) <= RestrictedAlongSize)
                {
                    currentLine.Add(size);
                }
                else
                {
                    if (layoutBlock.TotalSizeIfAdded(currentLine).AxisValue(perpendicularAxis) <= RestrictedPerpendicularSize) // duplicat1
                    {
                        layoutBlock.AddLine(currentLine);
                        currentLine = new LayoutLine(alongAxis);
                    }
                    else
                    {
                        break; // no more room
                    }
                }
            }


            if (layoutBlock.TotalSizeIfAdded(currentLine).AxisValue(perpendicularAxis) <= RestrictedPerpendicularSize) // duplicat2
            {
                layoutBlock.AddLine(currentLine);
            }



            return layoutBlock.Bake();
        }

        private class LayoutBlock
        {
            private readonly List<LayoutLine> lines = new List<LayoutLine>();
            private readonly Axis alongAxis;
            private AxisPoint pendingSize;

            public LayoutBlock(Axis alongAxis)
            {
                this.alongAxis = alongAxis;
            }

            public void AddLine(LayoutLine currentLayoutLine)
            {
                this.lines.Add(currentLayoutLine);
                this.pendingSize = TotalSizeIfAdded(currentLayoutLine);
            }

            public AxisPoint TotalSizeIfAdded(LayoutLine pendingLine)
            {
                var newSize = this.pendingSize;
                var oppositeAxis = AxisUtils.Opposite(this.alongAxis);
                newSize.SetAxisValue(oppositeAxis, pendingLine.TotalSize.AxisValue(oppositeAxis));
                return newSize;
            }

            public Rectangle[] Bake()
            {
                Point currentPos = Point.Zero;
                var pendingRectangles = new List<Rectangle>();
                foreach (var line in lines)
                {
                    foreach (var size in line.sizes)
                    {
                        pendingRectangles.Add(new Rectangle(currentPos, size));
                        currentPos.X += size.X;
                    }
                    currentPos.Y += line.TotalSize.Y;
                    currentPos.X = 0;
                }

                return pendingRectangles.ToArray();
            }
        }

        private class LayoutLine
        {
            public readonly List<Point> sizes = new List<Point>();
            private readonly Axis alongAxis;
            private Point pendingTotalSize = Point.Zero;

            public LayoutLine(Axis alongAxis)
            {
                this.alongAxis = alongAxis;
            }

            public void Add(Point addedSize)
            {
                var oppositeAxis = AxisUtils.Opposite(alongAxis);
                this.sizes.Add(addedSize);
                this.pendingTotalSize = TotalSizeIfAdded(addedSize);
            }

            public Point TotalSizeIfAdded(Point pendingAddedSize)
            {
                var result = AxisPoint.Zero;
                var oppositeAxis = AxisUtils.Opposite(alongAxis);
                result.SetAxisValue(alongAxis, MeasureNewAlongSize(pendingAddedSize));
                result.SetAxisValue(oppositeAxis, MeasureNewPerpendicularSize(pendingAddedSize));
                return result.AsPoint();
            }

            private int MeasureNewAlongSize(Point newSize)
            {
                return this.pendingTotalSize.AxisValue(alongAxis) + new AxisPoint(newSize).AxisValue(alongAxis);
            }

            private int MeasureNewPerpendicularSize(Point newSize)
            {
                var oppositeAxis = AxisUtils.Opposite(alongAxis);
                return Math.Max(this.pendingTotalSize.AxisValue(oppositeAxis), newSize.AxisValue(oppositeAxis));
            }

            public Point TotalSize => this.pendingTotalSize;
        }
    }
}
