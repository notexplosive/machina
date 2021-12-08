using Machina.Components;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data
{
    public class LayoutNode
    {
        public LayoutNode(string name, LayoutSize size, Orientation orientation = Orientation.Horizontal, LayoutNode[] children = null, Point margin = default, int padding = 0)
        {
            Name = name;
            Size = size;
            Orientation = orientation;
            Margin = margin;
            Padding = padding;
            Children = children;
        }

        public LayoutResult Bake()
        {
            return Bake(new LayoutResult(), Point.Zero);
        }

        private LayoutResult Bake(LayoutResult layoutResult, Point startingLocation)
        {
            if (layoutResult == null)
            {
                layoutResult = new LayoutResult();
            }

            var isVertical = Orientation == Orientation.Vertical;
            var groupSize = Size.ComputeConstSize();
            var totalAlongSize = isVertical ? groupSize.Y : groupSize.X;
            var alongMargin = isVertical ? Margin.Y : Margin.X;

            var elements = Children;
            var remainingAlongSize = totalAlongSize - alongMargin * 2;
            var stretchAlong = new List<LayoutNode>();
            var stretchPerpendicular = new List<LayoutNode>();

            var last = elements.Length - 1;
            var index = 0;

            foreach (var element in elements)
            {
                if (!element.Size.IsStretchedAlong(Orientation))
                {
                    if (isVertical)
                    {
                        remainingAlongSize -= element.Size.Y as ConstLayoutEdge;
                    }
                    else
                    {
                        remainingAlongSize -= element.Size.X as ConstLayoutEdge;
                    }
                }
                else
                {
                    stretchAlong.Add(element);
                }

                if (element.Size.IsStretchedPerpendicular(Orientation))
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

                foreach (var alongElement in stretchAlong)
                {
                    if (isVertical)
                    {
                        alongElement.Size.Y = new ConstLayoutEdge(alongSizeOfEachStretchedElement);
                    }
                    else
                    {
                        alongElement.Size.X = new ConstLayoutEdge(alongSizeOfEachStretchedElement);
                    }
                }
            }

            if (stretchPerpendicular.Count > 0)
            {
                foreach (var perpElement in stretchPerpendicular)
                {
                    if (isVertical)
                    {
                        perpElement.Size.X = new ConstLayoutEdge(groupSize.X - Margin.X * 2);
                    }
                    else
                    {
                        perpElement.Size.Y = new ConstLayoutEdge(groupSize.Y - Margin.Y * 2);
                    }
                }
            }

            // Place elements
            var nextLocation = startingLocation + new Point(Margin.X, Margin.Y);
            foreach (var element in elements)
            {
                var elementPosition = nextLocation;
                layoutResult.Add(elementPosition, element);
                if (isVertical)
                {
                    nextLocation += new Point(0, element.Size.Y as ConstLayoutEdge + Padding);
                }
                else
                {
                    nextLocation += new Point(element.Size.X as ConstLayoutEdge + Padding, 0);
                }

                if (element.HasChildren)
                {
                    element.Bake(layoutResult, elementPosition);
                }
            }

            return layoutResult;
        }

        private readonly LayoutNode[] Children;
        private bool HasChildren => Children != null;
        public string Name { get; }
        public LayoutSize Size { get; }
        public Orientation Orientation { get; }
        public Point Margin { get; }
        public int Padding { get; }
    }

    public class LayoutSize : XYPair<LayoutEdge>
    {
        public LayoutSize(LayoutEdge x, LayoutEdge y) : base(x, y)
        {
        }

        public LayoutSize(int x, int y) : base(new ConstLayoutEdge(x), new ConstLayoutEdge(y))
        {
        }

        public bool IsDynamic => X is StretchedLayoutEdge || Y is StretchedLayoutEdge;

        public bool IsStretchedAlong(Orientation orientation)
        {
            return GetValueFromOrientation(orientation) is StretchedLayoutEdge;
        }

        public bool IsStretchedPerpendicular(Orientation orientation)
        {
            return GetValueFromOrientation(OrientationUtils.Opposite(orientation)) is StretchedLayoutEdge;
        }

        public Point ComputeConstSize()
        {
            if (!IsDynamic)
            {
                return new Point(X as ConstLayoutEdge, Y as ConstLayoutEdge);
            }

            throw new Exception("Cannot compute const size of dynamic node");
        }
    }

    public abstract class LayoutEdge
    {
    }

    public class ConstLayoutEdge : LayoutEdge
    {
        public ConstLayoutEdge(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public static implicit operator int(ConstLayoutEdge d)
        {
            return d.Value;
        }
    }

    public class StretchedLayoutEdge : LayoutEdge
    {
        public StretchedLayoutEdge()
        {
        }
    }

    public class LayoutResult
    {
        private Dictionary<string, Rectangle> content = new Dictionary<string, Rectangle>();

        public void Add(Point position, LayoutNode node)
        {
            Debug.Assert(!node.Size.IsDynamic);
            var rect = new Rectangle(position, new Point(node.Size.X as ConstLayoutEdge, node.Size.Y as ConstLayoutEdge));
            this.content.Add(node.Name, rect);
        }

        public Rectangle Get(string name)
        {
            return content[name];
        }

        public Rectangle[] GetAll()
        {
            var result = new Rectangle[this.content.Values.Count];
            this.content.Values.CopyTo(result, 0);
            return result;
        }

        public IEnumerable<string> Keys()
        {
            return this.content.Keys;
        }
    }
}
