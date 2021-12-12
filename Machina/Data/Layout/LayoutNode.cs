using Machina.Components;
using Microsoft.Xna.Framework;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace Machina.Data.Layout
{
    public class LayoutNode
    {
        /// <summary>
        /// This is only called by the static constructor methods. Most constructors ignore everything past "size"
        /// </summary>
        private LayoutNode(LayoutNodeName name, LayoutSize size, Orientation orientation, LayoutNode[] children, Point margin, int padding, Alignment alignment)
        {
            Name = name;
            Size = size;
            Orientation = orientation;
            Margin = margin;
            Padding = padding;
            Children = children;
            Alignment = alignment;

            Baker = new LayoutBaker(this);
        }

        public BakedLayout Bake()
        {
            return Baker.Bake();
        }

        public static LayoutNode StretchedSpacer()
        {
            return new LayoutNode(LayoutNodeName.Nameless, LayoutSize.StretchedBoth(),
                /*Ignored params:*/
                Orientation.Horizontal,
                null,
                Point.Zero,
                0,
                Alignment.TopLeft
                );
        }

        public static LayoutNode Spacer(int size)
        {
            return new LayoutNode(LayoutNodeName.Nameless, LayoutSize.Square(size),
                /*Ignored params:*/
                Orientation.Horizontal,
                null,
                Point.Zero,
                0,
                Alignment.TopLeft
                );
        }

        public static LayoutNode Leaf(string name, LayoutSize size)
        {
            return new LayoutNode(name, size,
                /*Ignored params:*/
                Orientation.Horizontal,
                null,
                Point.Zero,
                0,
                Alignment.TopLeft
                );
        }

        public static LayoutNode VerticalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new LayoutNode(name, size, Orientation.Vertical, children, style.Margin, style.Padding, style.Alignment);
        }

        public static LayoutNode HorizontalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new LayoutNode(name, size, Orientation.Horizontal, children, style.Margin, style.Padding, style.Alignment);
        }

        public readonly LayoutNode[] Children;
        private LayoutBaker Baker { get; }
        public bool HasChildren => Children != null;
        public LayoutNodeName Name { get; }
        public LayoutSize Size { get; }
        public Orientation Orientation { get; }
        public Point Margin { get; }
        public int Padding { get; }
        public Alignment Alignment { get; }

        /// <summary>
        /// Returns a LayoutNode just like this one with the same children, only resized
        /// </summary>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public LayoutNode GetResized(Point newSize)
        {
            return new LayoutNode(Name, LayoutSize.Pixels(newSize.X, newSize.Y), Orientation, Children, Margin, Padding, Alignment);
        }

        /// <summary>
        /// Returns a LayoutNode just like this one with the same children, only realigned
        /// </summary>
        /// <returns></returns>
        public LayoutNode GetRealigned(Alignment newAlignment)
        {
            return new LayoutNode(Name, Size, Orientation, Children, Margin, Padding, newAlignment);
        }

        public override string ToString()
        {
            var childCount = HasChildren ? Children.Length : 0;
            return $"{Name}, {Size}, {childCount} children";
        }
    }
}
