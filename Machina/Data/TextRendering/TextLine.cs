using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public string TextContent { get; }
        public Point PositionRelativeToTopLeftOfRect { get; }
        public Point ContentSize { get; }
        public LayoutNode LayoutNode { get; }

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            TextContent = content;
            var relativePosition = Point.Zero;
            relativePosition.Y = positionY;

            var effectiveWidth = (int)fontMetrics.MeasureString(content).X;

            LayoutNode = LayoutNode.HorizontalParent("textLineParent", LayoutSize.Pixels(availableSpace), new LayoutStyle(alignment: new Alignment(horizontalAlignment)),
                LayoutNode.Leaf("textLineContent", LayoutSize.Pixels(effectiveWidth, fontMetrics.LineSpacing))
            );


            var bakedLayout = LayoutNode.Bake();
            relativePosition.X = bakedLayout.GetNode("textLineContent").PositionRelativeToRoot.X;

            PositionRelativeToTopLeftOfRect = relativePosition;
            ContentSize = bakedLayout.GetNode("textLineContent").Size;
        }
    }
}
