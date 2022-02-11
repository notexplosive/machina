﻿using Machina.Data.Layout;
using Microsoft.Xna.Framework;
using System;

namespace Machina.Data.TextRendering
{
    public readonly struct TextOutputFragment
    {
        public TextOutputFragment(string tokenText, IFontMetrics fontMetrics, Color color, int characterPosition)
        {
            CharacterPosition = characterPosition;
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
        public int CharacterPosition { get; }
        public int CharacterLength => Text.Length;

        public RenderableText CreateRenderableText(Point totalAvailableRectLocation, Point nodeLocation)
        {
            return new RenderableText(FontMetrics, Text, totalAvailableRectLocation, Color, nodeLocation);
        }

        public RenderableText CreateRenderableTextWithDifferentString(Point totalAvailableRectLocation, Point nodeLocation, string newText)
        {
            return new RenderableText(FontMetrics, newText, totalAvailableRectLocation, Color, nodeLocation);
        }
    }
}
