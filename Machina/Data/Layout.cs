using Machina.Components;
using Machina.Engine;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace Machina.Data
{
    public static class Layout
    {
        public interface IElement
        {
            Point Size { get; }
            Point Position { get; set; }
            Point Offset { get; }

            bool IsStretchedAlong(Orientation orientation);
            bool IsStretchPerpendicular(Orientation orientation);
            IElement SetHeight(int height);
            IElement SetWidth(int width);
            public IElement StretchHorizontally();
            public IElement StretchVertically();
        }

        public class Element : IElement
        {
            private Point size;

            private bool isStretchedVertically = false;
            private bool isStretchedHorizontally = false;

            public IElement StretchVertically()
            {
                this.isStretchedVertically = true;
                return this;
            }

            public IElement StretchHorizontally()
            {
                this.isStretchedHorizontally = true;
                return this;
            }

            public bool IsStretchedAlong(Orientation orientation)
            {
                if (orientation == Orientation.Horizontal)
                {
                    return this.isStretchedHorizontally;
                }

                if (orientation == Orientation.Vertical)
                {
                    return this.isStretchedVertically;
                }

                return false;
            }

            public bool IsStretchPerpendicular(Orientation orientation)
            {
                if (orientation == Orientation.Horizontal)
                {
                    return this.isStretchedVertically;
                }

                if (orientation == Orientation.Vertical)
                {
                    return this.isStretchedHorizontally;
                }

                return false;
            }

            public Point Size { get => this.size; set => this.size = value; }
            public Point Offset { get; set; }
            public Point Position { get; set; }
            public Rectangle Rect => new Rectangle(Position - Offset, Size);
            public IElement SetHeight(int height)
            {
                this.size.Y = height;
                return this;
            }

            public IElement SetWidth(int width)
            {
                this.size.X = width;
                return this;
            }
        }
        public interface IGroup<TCreationData>
        {
            public IGroup<TCreationData> SetPaddingBetweenElements(int padding);
            public IGroup<TCreationData> SetMarginSize(Point marginSize);
            public void ExecuteLayout();
            public IGroup<TCreationData> HorizontallyStretchedSpacer();
            public IElement AddElement(string name, Point size, Action<TCreationData> callback); // prefer AddSpecificElement
            public IGroup<TCreationData> AddVerticallyStretchedElement(string name, int width, Action<TCreationData> callback);
            public IGroup<TCreationData> AddHorizontallyStretchedElement(string name, int height, Action<TCreationData> callback);
            public IGroup<TCreationData> AddBothStretchedElement(string name, Action<TCreationData> callback);
            public IGroup<TCreationData> AddSpecificSizeElement(string name, Point point, Action<TCreationData> callback);
            public IGroup<TCreationData> VerticallyStretchedSpacer();
        }

        public class Group<T> : Element, IGroup<T>
        {
            public int Padding { get; private set; }
            public Orientation Orientation { get; }
            public Point MarginSize { get; private set; }
            private readonly List<IElement> elements = new List<IElement>();

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

            public void ExecuteLayout()
            {
                var isVertical = Orientation == Orientation.Vertical;
                var groupSize = Size;
                var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
                var alongMargin = isVertical ? MarginSize.Y : MarginSize.X;

                var elements = GetAllElements();
                var remainingAlongSize = totalAlongSize - alongMargin * 2;
                var stretchAlong = new List<IElement>();
                var stretchPerpendicular = new List<IElement>();

                var last = elements.Count - 1;
                var index = 0;

                // Determine true size of elements, subtract sizes
                foreach (var element in elements)
                {
                    if (!element.IsStretchedAlong(this.Orientation))
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

                    if (element.IsStretchPerpendicular(this.Orientation))
                    {
                        stretchPerpendicular.Add(element);
                    }

                    if (index != last)
                    {
                        remainingAlongSize -= Padding;
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
                            perpElement.SetWidth(groupSize.X - this.MarginSize.X * 2);
                        }
                        else
                        {
                            perpElement.SetHeight(groupSize.Y - this.MarginSize.Y * 2);
                        }
                    }
                }

                // Place elements
                var nextLocation = new Point(Position.X + this.MarginSize.X, Position.Y + this.MarginSize.Y);
                foreach (var element in elements)
                {
                    element.Position = nextLocation + element.Offset;
                    if (isVertical)
                    {
                        nextLocation += new Point(0, element.Size.Y + Padding);
                    }
                    else
                    {
                        nextLocation += new Point(element.Size.X + Padding, 0);
                    }
                }

                // If we have groups within groups, now we layout the subgroups.
                foreach (var element in elements)
                {
                    if (element is IGroup<T> subgroup)
                    {
                        subgroup.ExecuteLayout();
                    }
                }
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
        }
    }
}
