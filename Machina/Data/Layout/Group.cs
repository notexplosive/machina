using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

namespace Machina.Data.Layout
{
    public class Group<T> : Element, IGroup<T>
    {
        public int Padding { get; private set; }
        public Point MarginSize { get; private set; }
        private readonly List<IElement> elements = new List<IElement>();
        public Orientation Orientation { get; }

        public Group(Orientation orientation)
        {
            Orientation = orientation;
        }

        public IGroup<T> SetMarginSize(Point marginSize)
        {
            MarginSize = marginSize;
            return this;
        }

        public IGroup<T> SetPaddingBetweenElements(int padding)
        {
            this.Padding = padding;
            return this;
        }

        public List<IElement> GetAllElements()
        {
            return this.elements;
        }

        public IElement AddElement(IElement element)
        {
            this.elements.Add(element);
            return element;
        }

        public IElement AddElement(string name, Point size, Action<T> callback)
        {
            return AddElement(new Element { Size = size });
        }

        public IGroup<T> AddVerticallyStretchedElement(string name, int width, Action<T> callback)
        {
            AddElement(new Element().SetWidth(width).StretchVertically());
            return this;
        }

        public IGroup<T> AddHorizontallyStretchedElement(string name, int height, Action<T> callback)
        {
            AddElement(new Element().SetHeight(height).StretchHorizontally());
            return this;
        }

        public IGroup<T> AddBothStretchedElement(string name, Action<T> callback)
        {
            AddElement(new Element().StretchHorizontally().StretchVertically());
            return this;
        }

        public IGroup<T> AddSpecificSizeElement(string name, Point point, Action<T> callback)
        {
            AddElement(new Element().SetWidth(point.X).SetHeight(point.Y));
            return this;
        }

        public IGroup<T> HorizontallyStretchedSpacer()
        {
            AddElement(new Element().StretchHorizontally());
            return this;
        }

        public IGroup<T> VerticallyStretchedSpacer()
        {
            AddElement(new Element().StretchVertically());
            return this;
        }

        public static void ExecuteLayout(IGroup<T> group)
        {
            var isVertical = group.Orientation == Orientation.Vertical;
            var groupSize = group.Size;
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? group.MarginSize.Y : group.MarginSize.X;

            var elements = group.GetAllElements();
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var stretchAlong = new List<IElement>();
            var stretchPerpendicular = new List<IElement>();

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
                if (element is IGroup<T> subgroup)
                {
                    ExecuteLayout(subgroup);
                }
            }
        }
    }
}
