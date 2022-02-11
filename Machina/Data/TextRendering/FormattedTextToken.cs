using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public struct FormattedTextToken
    {
        public FormattedTextToken(string tokenText, FormattedTextFragment parentFragment)
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
        public FormattedTextFragment ParentFragment { get; }
        public string Text { get; }
        public FlowLayout.LayoutNodeOrInstruction[] Nodes { get; }

        public TextOutputFragment OutputFragment()
        {
            return new TextOutputFragment(Text, ParentFragment.FontMetrics, ParentFragment.Color);
        }
    }
}
