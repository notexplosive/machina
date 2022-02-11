﻿using Machina.Data.Layout;
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
                ShouldBeCounted = false;
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.Spacer(new Point(0, fontMetrics.LineSpacing)),
                    FlowLayoutInstruction.Linebreak
                };
            }
            else
            {
                ShouldBeCounted = true;
                var tokenSize = fontMetrics.MeasureStringRounded(tokenText);
                Nodes = new FlowLayout.LayoutNodeOrInstruction[] {
                    LayoutNode.NamelessLeaf(LayoutSize.Pixels(tokenSize))
                };
            }
        }

        public FlowLayout.LayoutNodeOrInstruction[] Nodes { get; }
        public string Text { get; }
        public IFontMetrics FontMetrics { get; }
        public Color Color { get; }
        public bool ShouldBeCounted { get; }
    }
}
