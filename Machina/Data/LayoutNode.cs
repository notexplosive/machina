﻿using Machina.Components;
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

        public Rectangle GetRectangle(Point position, LayoutResult layoutResult)
        {
            return new Rectangle(position, GetMeasuredSize(layoutResult));
        }

        public Point GetMeasuredSize(LayoutResult layoutResult)
        {
            return new Point(layoutResult.GetEdgeValue(Size.X), layoutResult.GetEdgeValue(Size.Y));
        }

        public LayoutResult Bake()
        {
            return Bake(new LayoutResult(this), Point.Zero);
        }

        private LayoutResult Bake(LayoutResult layoutResult, Point startingLocation)
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
                layoutResult.Add(elementPosition, element);
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

    public class LayoutSize
    {
        public readonly LayoutEdge X;
        public readonly LayoutEdge Y;

        public LayoutSize(LayoutEdge x, LayoutEdge y)
        {
            X = x;
            Y = y;
        }

        public LayoutSize(int x, int y)
        {
            X = new ConstLayoutEdge(x);
            Y = new ConstLayoutEdge(y);
        }

        public bool IsStretchedAlong(Orientation orientation)
        {
            return GetValueFromOrientation(orientation) is StretchedLayoutEdge;
        }

        public bool IsStretchedPerpendicular(Orientation orientation)
        {
            return GetValueFromOrientation(OrientationUtils.Opposite(orientation)) is StretchedLayoutEdge;
        }

        public LayoutEdge GetValueFromOrientation(Orientation orientation)
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

        public static implicit operator int(ConstLayoutEdge edge)
        {
            return edge.Value;
        }
    }

    public class StretchedLayoutEdge : LayoutEdge
    {
    }

    public class LayoutResult
    {
        public readonly Dictionary<LayoutEdge, int> sizeLookupTable = new Dictionary<LayoutEdge, int>();
        private Dictionary<string, Rectangle> content = new Dictionary<string, Rectangle>();
        private Rectangle? rootRectangle = null;


        public LayoutResult(LayoutNode rootNode)
        {
            if (this.rootRectangle == null)
            {
                this.rootRectangle = rootNode.GetRectangle(Point.Zero, this);
            }
        }

        public int GetEdgeValue(LayoutEdge edge)
        {
            if (edge is ConstLayoutEdge constEdge)
            {
                return constEdge;
            }

            return sizeLookupTable[edge];
        }

        public void Add(Point position, LayoutNode node)
        {
            var rect = node.GetRectangle(position, this);
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

        public Rectangle RootRectangle => this.rootRectangle.Value;
    }
}