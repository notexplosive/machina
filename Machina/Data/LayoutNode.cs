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
        private LayoutNode(LayoutNodeName name, LayoutSize size, Orientation orientation = Orientation.Horizontal, LayoutNode[] children = null, Point margin = default, int padding = 0)
        {
            Name = name;
            Size = size;
            Orientation = orientation;
            Margin = margin;
            Padding = padding;
            Children = children;
        }

        public static LayoutNode Spacer(LayoutSize size)
        {
            return new LayoutNode(LayoutNodeName.Nameless, size);
        }

        public static LayoutNode Leaf(string name, LayoutSize size)
        {
            return new LayoutNode(name, size);
        }

        public static LayoutNode VerticalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new LayoutNode(name, size, Orientation.Vertical, margin: style.Margin, padding: style.Padding, children: children);
        }

        public static LayoutNode HorizontalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new LayoutNode(name, size, Orientation.Horizontal, margin: style.Margin, padding: style.Padding, children: children);
        }

        public Rectangle GetRectangle(Point position, LayoutResult layoutResult)
        {
            return new Rectangle(position, GetMeasuredSize(layoutResult));
        }

        public Point GetMeasuredSize(LayoutResult layoutResult)
        {
            return new Point(layoutResult.GetEdgeValue(Size.X), layoutResult.GetEdgeValue(Size.Y));
        }

        public LayoutResult Build()
        {
            return Build(Point.Zero);
        }

        public LayoutResult Build(Point startingLocation)
        {
            var layoutResult = new LayoutResult(this);
            layoutResult.AddLayoutNode(startingLocation, this);
            return Build(layoutResult, startingLocation);
        }

        private LayoutResult Build(LayoutResult layoutResult, Point startingLocation)
        {
            var isVertical = Orientation == Orientation.Vertical;
            var groupSize = GetMeasuredSize(layoutResult);
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
                        remainingAlongSize -= element.Size.Y.ActualSize;
                    }
                    else
                    {
                        remainingAlongSize -= element.Size.X.ActualSize;
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
                        layoutResult.sizeLookupTable[alongElement.Size.Y] = alongSizeOfEachStretchedElement;
                    }
                    else
                    {
                        layoutResult.sizeLookupTable[alongElement.Size.X] = alongSizeOfEachStretchedElement;
                    }
                }
            }

            var perpendicularStretchSize = groupSize.X - Margin.X * 2;

            if (stretchPerpendicular.Count > 0)
            {
                foreach (var perpElement in stretchPerpendicular)
                {
                    if (isVertical)
                    {
                        layoutResult.sizeLookupTable[perpElement.Size.X] = perpendicularStretchSize;
                    }
                    else
                    {
                        layoutResult.sizeLookupTable[perpElement.Size.Y] = perpendicularStretchSize;
                    }
                }
            }

            // Place elements
            var nextLocation = startingLocation + new Point(Margin.X, Margin.Y);
            foreach (var element in elements)
            {
                var elementPosition = nextLocation;
                layoutResult.AddLayoutNode(elementPosition, element);
                if (isVertical)
                {
                    nextLocation += new Point(0, layoutResult.GetEdgeValue(element.Size.Y) + Padding);
                }
                else
                {
                    nextLocation += new Point(layoutResult.GetEdgeValue(element.Size.X) + Padding, 0);
                }

                if (element.HasChildren)
                {
                    element.Build(layoutResult, elementPosition);
                }
            }

            return layoutResult;
        }

        private readonly LayoutNode[] Children;
        private bool HasChildren => Children != null;
        public LayoutNodeName Name { get; }
        public LayoutSize Size { get; }
        public Orientation Orientation { get; }
        public Point Margin { get; }
        public int Padding { get; }

        /// <summary>
        /// Returns a LayoutNode just like this one
        /// </summary>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public LayoutNode GetResized(LayoutSize newSize)
        {
            return new LayoutNode(Name, newSize, Orientation, Children, Margin, Padding);
        }
    }

    public struct LayoutStyle
    {
        public readonly Point Margin { get; }
        public readonly int Padding { get; }

        public LayoutStyle(Point margin = default, int padding = default)
        {
            Margin = margin;
            Padding = padding;
        }

        public static readonly LayoutStyle Empty = new LayoutStyle();
    }

    public struct LayoutNodeName
    {
        private readonly string internalString;
        public bool IsNameless => this.internalString == null;

        public LayoutNodeName(string text)
        {
            this.internalString = text;
        }

        public static implicit operator LayoutNodeName(string text)
        {
            return new LayoutNodeName(text);
        }

        public override int GetHashCode()
        {
            return HashCode.Combine(this.internalString);
        }

        public string Text
        {
            get
            {
                if (Exists)
                {
                    return this.internalString;
                }

                throw new Exception("Node does not have a name (is spacer or null)");
            }
        }

        public bool Exists => this.internalString != null;

        public static LayoutNodeName Nameless => new LayoutNodeName(null);
    }

    public struct LayoutSize
    {
        public readonly ILayoutEdge X;
        public readonly ILayoutEdge Y;

        public LayoutSize(ILayoutEdge x, ILayoutEdge y)
        {
            X = x;
            Y = y;
        }

        public static LayoutSize Pixels(int x, int y) => new LayoutSize(new ConstLayoutEdge(x), new ConstLayoutEdge(y));
        public static LayoutSize Square(int x) => new LayoutSize(new ConstLayoutEdge(x), new ConstLayoutEdge(x));
        public static LayoutSize StretchedVertically(int width) => new LayoutSize(new ConstLayoutEdge(width), new StretchedLayoutEdge());
        public static LayoutSize StretchedHorizontally(int height) => new LayoutSize(new StretchedLayoutEdge(), new ConstLayoutEdge(height));
        public static LayoutSize StretchedBoth() => new LayoutSize(new StretchedLayoutEdge(), new StretchedLayoutEdge());

        public bool IsStretchedAlong(Orientation orientation)
        {
            return GetValueFromOrientation(orientation) is StretchedLayoutEdge;
        }

        public bool IsStretchedPerpendicular(Orientation orientation)
        {
            return GetValueFromOrientation(OrientationUtils.Opposite(orientation)) is StretchedLayoutEdge;
        }

        public ILayoutEdge GetValueFromOrientation(Orientation orientation)
        {
            if (orientation == Orientation.Horizontal)
            {
                return X;
            }

            if (orientation == Orientation.Vertical)
            {
                return Y;
            }

            throw new ArgumentException("Invalid orientation");
        }
    }

    public interface ILayoutEdge
    {
        public bool IsStretched { get; }
        public int ActualSize { get; }
    }

    public struct ConstLayoutEdge : ILayoutEdge
    {
        public ConstLayoutEdge(int value)
        {
            Value = value;
        }

        public int Value { get; }

        public static implicit operator int(ConstLayoutEdge edge)
        {
            return edge.Value;
        }

        public bool IsStretched => false;
        public int ActualSize => Value;
    }

    public struct StretchedLayoutEdge : ILayoutEdge
    {
        public bool IsStretched => true;
        public int ActualSize => throw new Exception("StretchedLayoutEdge does not have an actual size");

        /// <summary>
        /// Do not delete! Important hack here
        /// </summary>
        /// <returns></returns>
        public override int GetHashCode()
        {
            // Hacky thing to make every single instance of StretchedLayoutEdge unique
            if (!this.hash.HasValue)
            {
                this.hash = hashPool++;
            }
            return this.hash.Value;
        }

        private static int hashPool = 0;
        private int? hash;
    }

    public class LayoutResult
    {
        public readonly Dictionary<ILayoutEdge, int> sizeLookupTable = new Dictionary<ILayoutEdge, int>();
        private readonly Dictionary<LayoutNodeName, Rectangle> content = new Dictionary<LayoutNodeName, Rectangle>();
        private Rectangle? rootRectangle = null;


        public LayoutResult(LayoutNode rootNode)
        {
            if (this.rootRectangle == null)
            {
                this.rootRectangle = rootNode.GetRectangle(Point.Zero, this);
            }
        }

        public int GetEdgeValue(ILayoutEdge edge)
        {
            if (edge is ConstLayoutEdge constEdge)
            {
                return constEdge;
            }

            return sizeLookupTable[edge];
        }

        public void AddLayoutNode(Point position, LayoutNode node)
        {
            if (node.Name.Exists)
            {
                var rect = node.GetRectangle(position, this);
                this.content.Add(node.Name, rect);
            }
        }

        public Rectangle Get(LayoutNodeName name)
        {
            return content[name];
        }

        public Rectangle[] GetAll()
        {
            var result = new Rectangle[this.content.Values.Count];
            this.content.Values.CopyTo(result, 0);
            return result;
        }

        public IEnumerable<LayoutNodeName> Keys()
        {
            return this.content.Keys;
        }

        public Rectangle RootRectangle => this.rootRectangle.Value;
    }
}
