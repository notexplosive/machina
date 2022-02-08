using Microsoft.Xna.Framework;
using System;
using System.Diagnostics;
using System.Text;

namespace Machina.Data.Layout
{
    public class LayoutNode
    {
        /// <summary>
        /// This is only called by the static constructor methods. Most constructors ignore everything past "size"
        /// </summary>
        private LayoutNode(LayoutNodeName name, LayoutSize size, Orientation orientation, LayoutStyle style, LayoutNode[] children)
        {
            Name = name;
            Size = size;
            Orientation = orientation;
            Children = children;
            Style = style;
        }

        public static LayoutNode StretchedSpacer()
        {
            return new LayoutNode(LayoutNodeName.Nameless, LayoutSize.StretchedBoth(), Orientation.Horizontal, LayoutStyle.Empty, null);
        }

        public static LayoutNode Spacer(int size)
        {
            return new LayoutNode(LayoutNodeName.Nameless, LayoutSize.Square(size),
                /*Ignored params:*/
                Orientation.Horizontal,
                LayoutStyle.Empty,
                null);
        }

        public static LayoutNode Leaf(string name, LayoutSize size)
        {
            return new LayoutNode(name, size,
                /*Ignored params:*/
                Orientation.Horizontal,
                LayoutStyle.Empty,
                null);
        }

        public static RawLayout VerticalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new RawLayout(new LayoutNode(name, size, Orientation.Vertical, style, children));
        }

        public static RawLayout HorizontalParent(string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            return new RawLayout(new LayoutNode(name, size, Orientation.Horizontal, style, children));
        }

        public static RawLayout NamelessOneOffParent(LayoutSize size, LayoutStyle style, LayoutNode child)
        {
            // Horizontal/Vertical does not matter here
            return HorizontalParent("root", size, style, child);
        }

        public static RawLayout OneOffParent(string name, LayoutSize size, LayoutStyle style, LayoutNode child)
        {
            // Horizontal/Vertical does not matter here
            return HorizontalParent(name, size, style, child);
        }

        public static RawLayout OrientedParent(Orientation orientation, string name, LayoutSize size, LayoutStyle style, params LayoutNode[] children)
        {
            if (orientation == Orientation.Horizontal)
            {
                return HorizontalParent(name, size, style, children);
            }
            else
            {
                return VerticalParent(name, size, style, children);
            }
        }

        public LayoutNode FindChildNodeWithName(string targetName)
        {
            if (Name.Exists && Name.Text == targetName)
            {
                return this;
            }

            foreach (var child in Children)
            {
                var foundChild = child.FindChildNodeWithName(targetName);

                if (foundChild != null)
                {
                    return foundChild;
                }
            }

            return null;
        }

        public readonly LayoutNode[] Children = Array.Empty<LayoutNode>();
        public bool HasChildren => Children != null;
        public LayoutNodeName Name { get; }
        public LayoutSize Size { get; }
        public Orientation Orientation { get; }
        public Point Margin => Style.Margin;
        public int Padding => Style.Padding;
        public Alignment Alignment => Style.Alignment;
        public LayoutStyle Style { get; }

        /// <summary>
        /// Returns a LayoutNode just like this one with the same children, only resized
        /// </summary>
        /// <param name="newSize"></param>
        /// <returns></returns>
        public RawLayout GetResized(Point newSize)
        {
            return new RawLayout(new LayoutNode(Name, LayoutSize.Pixels(newSize.X, newSize.Y), Orientation, Style, Children));
        }

        /// <summary>
        /// Returns a LayoutNode just like this one with the same children, only realigned
        /// </summary>
        /// <returns></returns>
        public RawLayout GetRealigned(Alignment newAlignment)
        {
            return new RawLayout(new LayoutNode(Name, Size, Orientation, new LayoutStyle(margin: Margin, padding: Padding, alignment: newAlignment), Children));
        }

        public override string ToString()
        {
            var childCount = HasChildren ? Children.Length : 0;
            return $"{Name}, {Size}, {childCount} children";
        }
    }
}
