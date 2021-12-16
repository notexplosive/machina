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
            size.SetOppositeAxisValue(this.alongAxis, RestrictedPerpendicularSize);
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
                newSize.SetOppositeAxisValue(this.alongAxis, pendingLine.TotalSize.OppositeAxisValue(alongAxis));
                return newSize;
            }

            public Rectangle[] Bake()
            {
                AxisPoint currentPos = AxisPoint.Zero;
                var pendingRectangles = new List<Rectangle>();
                foreach (var line in lines)
                {
                    foreach (var size in line.sizes)
                    {
                        pendingRectangles.Add(new Rectangle(currentPos.AsPoint(this.alongAxis), size));
                        currentPos.SetAxisValue(this.alongAxis, currentPos.AxisValue(this.alongAxis) + size.X);
                    }
                    currentPos.SetOppositeAxisValue(this.alongAxis, currentPos.OppositeAxisValue(this.alongAxis) + line.TotalSize.Y);
                    currentPos.SetAxisValue(this.alongAxis, 0);
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
                this.sizes.Add(addedSize);
                this.pendingTotalSize = TotalSizeIfAdded(addedSize);
            }

            public Point TotalSizeIfAdded(Point pendingAddedSize)
            {
                var result = AxisPoint.Zero;
                result.SetAxisValue(alongAxis, MeasureNewAlongSize(pendingAddedSize));
                result.SetOppositeAxisValue(alongAxis, MeasureNewPerpendicularSize(pendingAddedSize));
                return result.AsPoint();
            }

            private int MeasureNewAlongSize(Point newSize)
            {
                return this.pendingTotalSize.AxisValue(alongAxis) + new AxisPoint(newSize).AxisValue(alongAxis);
            }

            private int MeasureNewPerpendicularSize(Point newSize)
            {
                return Math.Max(this.pendingTotalSize.OppositeAxisValue(alongAxis), newSize.OppositeAxisValue(alongAxis));
            }

            public Point TotalSize => this.pendingTotalSize;
        }
    }
}
