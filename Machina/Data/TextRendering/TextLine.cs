﻿using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public string TextContent { get; }
        private Point ContentSize { get; }

        private readonly Point availableSpace;
        private readonly HorizontalAlignment horizontalAlignment;

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, HorizontalAlignment horizontalAlignment)
        {
            TextContent = content;
            this.availableSpace = availableSpace;
            this.horizontalAlignment = horizontalAlignment;

            var effectiveWidth = (int)fontMetrics.MeasureString(content).X;
            ContentSize = new Point(effectiveWidth, fontMetrics.LineSpacing);
        }

        public UnbakedLayout CreateLayoutNode(string name)
        {
            return LayoutNode.HorizontalParent($"{name} parent", LayoutSize.Pixels(availableSpace.X, ContentSize.Y), new LayoutStyle(alignment: new Alignment(horizontalAlignment)),
                LayoutNode.Leaf(name, LayoutSize.Pixels(ContentSize))
            );
        }
    }
}
