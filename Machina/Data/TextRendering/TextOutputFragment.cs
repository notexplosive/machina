using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public readonly struct TextOutputFragment
    {
        public TextOutputFragment(string tokenText, IFontMetrics fontMetrics, Color color)
        {
            Text = tokenText;
            FontMetrics = fontMetrics;
            Color = color;

            if (tokenText == "\n")
            {
                WillBeRendered = false;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.Spacer(new Point(0, fontMetrics.LineSpacing)),
                    FlowLayoutInstruction.Linebreak
                };
            }
            else
            {
                WillBeRendered = true;
                var tokenSize = fontMetrics.MeasureStringRounded(tokenText);
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.NamelessLeaf(LayoutSize.Pixels(tokenSize))
                };
            }
        }

        public readonly FlowLayout.LayoutNodeOrInstruction[] Nodes;
        public string Text { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public bool WillBeRendered { get; }
    }
}
