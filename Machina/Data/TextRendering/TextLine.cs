using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextLine
    {
        public readonly string textContent;
        public readonly Point positionRelativeToTopLeftOfRect;

        public TextLine(string content, IFontMetrics fontMetrics, Point availableSpace, int positionY,
            HorizontalAlignment horizontalAlignment)
        {
            this.textContent = content;
            this.positionRelativeToTopLeftOfRect.Y = positionY;

            var effectiveWidth = (int)fontMetrics.MeasureString(content).X;

            var layout = LayoutNode.HorizontalParent("textLineParent", LayoutSize.Pixels(availableSpace), new LayoutStyle(alignment: new Alignment(horizontalAlignment)),
                LayoutNode.Leaf("textLineContent", LayoutSize.Pixels(effectiveWidth, fontMetrics.LineSpacing))
            );

            this.positionRelativeToTopLeftOfRect.X = layout.Bake().GetNode("textLineContent").PositionRelativeToRoot.X;
        }
    }
}
