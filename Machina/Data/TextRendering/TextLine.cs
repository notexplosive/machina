using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public string textContent { get; }
        public Point positionRelativeToTopLeftOfRect { get; }

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            textContent = content;
            var relativePosition = Point.Zero;
            relativePosition.Y = positionY;

            var effectiveWidth = (int)fontMetrics.MeasureString(content).X;

            var layout = LayoutNode.HorizontalParent("textLineParent", LayoutSize.Pixels(availableSpace), new LayoutStyle(alignment: new Alignment(horizontalAlignment)),
                LayoutNode.Leaf("textLineContent", LayoutSize.Pixels(effectiveWidth, fontMetrics.LineSpacing))
            );

            relativePosition.X = layout.Bake().GetNode("textLineContent").PositionRelativeToRoot.X;

            positionRelativeToTopLeftOfRect = relativePosition;
        }
    }
}
