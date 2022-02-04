using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public string TextContent { get; }
        public Point PositionRelativeToTopLeftOfRect { get; }
        public Point ContentSize { get; }

        private readonly Point availableSpace;
        private readonly HorizontalAlignment horizontalAlignment;

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            TextContent = content;
            this.availableSpace = availableSpace;
            this.horizontalAlignment = horizontalAlignment;

            var relativePosition = Point.Zero;
            relativePosition.Y = positionY;

            var effectiveWidth = (int)fontMetrics.MeasureString(content).X;

            var layout = LayoutNode.HorizontalParent("textLineParent", LayoutSize.Pixels(availableSpace), new LayoutStyle(alignment: new Alignment(horizontalAlignment)),
                LayoutNode.Leaf("textLineContent", LayoutSize.Pixels(effectiveWidth, fontMetrics.LineSpacing))
            );


            var bakedLayout = layout.Bake();
            relativePosition.X = bakedLayout.GetNode("textLineContent").PositionRelativeToRoot.X;

            PositionRelativeToTopLeftOfRect = relativePosition;
            ContentSize = new Point(effectiveWidth, fontMetrics.LineSpacing);
        }

        public LayoutNode CreateLayoutNode(string name)
        {
            return LayoutNode.HorizontalParent($"{name} parent", LayoutSize.Pixels(availableSpace.X, ContentSize.Y), new LayoutStyle(alignment: new Alignment(horizontalAlignment)),
                LayoutNode.Leaf(name, LayoutSize.Pixels(ContentSize))
            );
        }
    }
}
