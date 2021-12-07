using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class LayoutGroup<T> : LayoutElement, ILayoutGroup<T>
    {
        public int Padding { get; private set; }
        public Point MarginSize { get; private set; }
        private readonly List<ILayoutElement> elements = new List<ILayoutElement>();
        public Orientation Orientation { get; }

        public LayoutGroup(Orientation orientation)
        {
            Orientation = orientation;
        }

        public ILayoutGroup<T> SetMarginSize(Point marginSize)
        {
            MarginSize = marginSize;
            return this;
        }

        public ILayoutGroup<T> SetPaddingBetweenElements(int padding)
        {
            this.Padding = padding;
            return this;
        }

        public List<ILayoutElement> GetAllElements()
        {
            return this.elements;
        }

        public ILayoutElement AddElement(ILayoutElement element)
        {
            this.elements.Add(element);
            return element;
        }

        public ILayoutElement AddElement(string name, Point size, Action<T> callback)
        {
            return AddElement(new LayoutElement { Size = size });
        }

        public ILayoutGroup<T> AddVerticallyStretchedElement(string name, int width, Action<T> callback)
        {
            AddElement(new LayoutElement().SetWidth(width).StretchVertically());
            return this;
        }

        public ILayoutGroup<T> AddHorizontallyStretchedElement(string name, int height, Action<T> callback)
        {
            AddElement(new LayoutElement().SetHeight(height).StretchHorizontally());
            return this;
        }

        public ILayoutGroup<T> AddBothStretchedElement(string name, Action<T> callback)
        {
            AddElement(new LayoutElement().StretchHorizontally().StretchVertically());
            return this;
        }

        public ILayoutGroup<T> AddSpecificSizeElement(string name, Point point, Action<T> callback)
        {
            AddElement(new LayoutElement().SetWidth(point.X).SetHeight(point.Y));
            return this;
        }

        public ILayoutGroup<T> AddHorizontallyStretchedSpacer()
        {
            AddElement(new LayoutElement().StretchHorizontally());
            return this;
        }

        public ILayoutGroup<T> AddVerticallyStretchedSpacer()
        {
            AddElement(new LayoutElement().StretchVertically());
            return this;
        }

        public static void ExecuteLayout(ILayoutGroup<T> group)
        {
            var isVertical = group.Orientation == Orientation.Vertical;
            var groupSize = group.Size;
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? group.MarginSize.Y : group.MarginSize.X;

            var elements = group.GetAllElements();
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var stretchAlong = new List<ILayoutElement>();
            var stretchPerpendicular = new List<ILayoutElement>();

            var last = elements.Count - 1;
            var index = 0;

            // Determine true size of elements, subtract sizes
            foreach (var element in elements)
            {
                if (!element.IsStretchedAlong(group.Orientation))
                {
                    if (isVertical)
                    {
                        remainingAlongSize -= element.Size.Y;
                    }
                    else
                    {
                        remainingAlongSize -= element.Size.X;
                    }
                }
                else
                {
                    stretchAlong.Add(element);
                }

                if (element.IsStretchPerpendicular(group.Orientation))
                {
                    stretchPerpendicular.Add(element);
                }

                if (index != last)
                {
                    remainingAlongSize -= group.Padding;
                }

                index++;
            }

            // Update size of stretch elements
            if (stretchAlong.Count > 0)
            {
                var alongSizeOfEachStretchedElement = remainingAlongSize / stretchAlong.Count;

                if (totalAlongSize != 0)
                {
                    //Debug.Assert(alongSizeOfEachStretchedElement > 0, "Not enough room to lay out stretch elements");
                }

                foreach (var alongElement in stretchAlong)
                {
                    if (isVertical)
                    {
                        alongElement.SetHeight(alongSizeOfEachStretchedElement);
                    }
                    else
                    {
                        alongElement.SetWidth(alongSizeOfEachStretchedElement);
                    }
                }
            }

            if (stretchPerpendicular.Count > 0)
            {
                foreach (var perpElement in stretchPerpendicular)
                {
                    if (isVertical)
                    {
                        perpElement.SetWidth(groupSize.X - group.MarginSize.X * 2);
                    }
                    else
                    {
                        perpElement.SetHeight(groupSize.Y - group.MarginSize.Y * 2);
                    }
                }
            }

            // Place elements
            var nextLocation = new Point(group.Position.X + group.MarginSize.X, group.Position.Y + group.MarginSize.Y);
            foreach (var element in elements)
            {
                element.Position = nextLocation + element.Offset;
                if (isVertical)
                {
                    nextLocation += new Point(0, element.Size.Y + group.Padding);
                }
                else
                {
                    nextLocation += new Point(element.Size.X + group.Padding, 0);
                }
            }

            // If we have groups within groups, now we layout the subgroups.
            foreach (var element in elements)
            {
                if (element is ILayoutGroup<T> subgroup)
                {
                    ExecuteLayout(subgroup);
                }
            }
        }
    }
}
