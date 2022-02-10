using Machina.Data.Layout;
using Microsoft.Xna.Framework;

namespace Machina.Data.TextRendering
{
    public struct TextInputToken
    {
        public TextInputToken(string tokenText, TextInputFragment parentFragment)
        {
            ShouldBeCounted = false;
            ParentFragment = parentFragment;
            Text = tokenText;

            if (tokenText == "\n")
            {
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.Spacer(new Point(0, parentFragment.FontMetrics.LineSpacing)),
                    FlowLayoutInstruction.Linebreak
                };
            }
            else
            {
                ShouldBeCounted = true;

                var tokenSize = parentFragment.FontMetrics.MeasureStringRounded(tokenText);
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.NamelessLeaf(LayoutSize.Pixels(tokenSize))
                };
            }
        }

        public bool ShouldBeCounted { get; }
        public TextInputFragment ParentFragment { get; }
        public string Text { get; }
        public FlowLayout.LayoutNodeOrInstruction[] Nodes { get; }
    }
}
